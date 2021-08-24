using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using ExileCore;
using ExileCore.PoEMemory;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.Elements;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared;
using ExileCore.Shared.Enums;
using SharpDX;

namespace CoPilot
{
    public class AutoPilot
    {
	    // Most Logic taken from Alpha Plugin
	    private Coroutine autoPilotCoroutine;
	    private readonly Random random = new Random();
        private static Camera Camera => CoPilot.instance.GameController.Game.IngameState.Camera;
        private const string PORTAL_ID = "Metadata/MiscellaneousObjects/MultiplexPortal";
        private const string AreaTransition_ID = "Metadata/MiscellaneousObjects/AreaTransition";

        private Vector3 lastTargetPosition;
        private Vector3 lastPlayerPosition;
        private Entity followTarget;

        private bool hasUsedWp;
        private List<TaskNode> tasks = new List<TaskNode>();

        private int numRows, numCols;
        private byte[,] tiles;
        /// <summary>
        /// Clears all pathfinding values. Used on area transitions primarily.
        /// </summary>
        private void ResetPathing()
        {
            tasks = new List<TaskNode>();
            followTarget = null;
            lastTargetPosition = Vector3.Zero;
            lastPlayerPosition = Vector3.Zero;
            hasUsedWp = false;
        }
        
        private LabelOnGround GetBestPortalLabel()
        {
	        try
	        {
		        var portalLabels =
			        CoPilot.instance.GameController?.Game?.IngameState?.IngameUi?.ItemsOnGroundLabels.Where(x =>
				        x != null && x.IsVisible && x.Label != null && x.Label.IsValid && x.Label.IsVisible && x.ItemOnGround != null && 
				        (x.ItemOnGround.Metadata.ToLower().Contains("areatransition") || x.ItemOnGround.Metadata.ToLower().Contains("portal") ))
				        .OrderBy(x => Vector3.Distance(lastTargetPosition, x.ItemOnGround.Pos)).ToList();


		        return CoPilot.instance?.GameController?.Area?.CurrentArea?.IsHideout != null && (bool)CoPilot.instance.GameController?.Area?.CurrentArea?.IsHideout
			        ? portalLabels?[random.Next(portalLabels.Count)]
			        : portalLabels?.FirstOrDefault();
	        }
	        catch
	        {
		        return null;
	        }
        }
        private Entity GetBestPortal()
        {
	        try
	        {
		        var validPortals = CoPilot.instance.GameController.EntityListWrapper.Entities.Where(x =>
				        x?.Type == EntityType.AreaTransition ||
				        x?.Type == EntityType.Portal ||
				        x?.Type == EntityType.TownPortal ||
				        x?.Type == EntityType.IngameIcon)
			        .Where(x => x.IsTargetable && (x.Type != EntityType.IngameIcon ||
			                                       x.Type == EntityType.IngameIcon &&
			                                       x.Metadata.ToLower().Contains("portal")))
			        .OrderBy(x => Vector3.Distance(lastTargetPosition, x.Pos)).ToList();

		        return CoPilot.instance.GameController.Area.CurrentArea.IsHideout
			        ? validPortals[random.Next(validPortals.Count)]
			        : validPortals.FirstOrDefault();
	        }
	        catch
	        {
		        return null;
	        }
        }
        public void AreaChange()
        {
            ResetPathing();
            
            var terrain = CoPilot.instance.GameController.IngameState.Data.Terrain;
            var terrainBytes = CoPilot.instance.GameController.Memory.ReadBytes(terrain.LayerMelee.First, terrain.LayerMelee.Size);
            numCols = (int)(terrain.NumCols - 1) * 23;
            numRows = (int)(terrain.NumRows - 1) * 23;
            if ((numCols & 1) > 0)
                numCols++;

            tiles = new byte[numCols, numRows];
            var dataIndex = 0;
            for (var y = 0; y < numRows; y++)
            {
                for (var x = 0; x < numCols; x += 2)
                {
                    var b = terrainBytes[dataIndex + (x >> 1)];
                    tiles[x, y] = (byte)((b & 0xf) > 0 ? 1 : 255);
                    tiles[x+1, y] = (byte)((b >> 4) > 0 ? 1 : 255);
                }
                dataIndex += terrain.BytesPerRow;
            }

            terrainBytes = CoPilot.instance.GameController.Memory.ReadBytes(terrain.LayerRanged.First, terrain.LayerRanged.Size);
            numCols = (int)(terrain.NumCols - 1) * 23;
            numRows = (int)(terrain.NumRows - 1) * 23;
            if ((numCols & 1) > 0)
                numCols++;
            dataIndex = 0;
            for (var y = 0; y < numRows; y++)
            {
                for (var x = 0; x < numCols; x += 2)
                {
                    var b = terrainBytes[dataIndex + (x >> 1)];

                    var current = tiles[x, y];
                    if(current == 255)
                        tiles[x, y] = (byte)((b & 0xf) > 3 ? 2 : 255);
                    current = tiles[x+1, y];
                    if (current == 255)
                        tiles[x + 1, y] = (byte)((b >> 4) > 3 ? 2 : 255);
                }
                dataIndex += terrain.BytesPerRow;
            }
        }

        public void StartCoroutine()
        {
	        autoPilotCoroutine = new Coroutine(AutoPilotLogic(), CoPilot.instance, "AutoPilot");
	        Core.ParallelRunner.Run(autoPilotCoroutine);
	    }
        private IEnumerator MouseoverItem(Entity item)
        {
	        var uiLoot = CoPilot.instance.GameController.IngameState.IngameUi.ItemsOnGroundLabels.FirstOrDefault(I => I.IsVisible && I.ItemOnGround.Id == item.Id);
	        if (uiLoot == null) yield return null;
	        var clickPos = uiLoot?.Label?.GetClientRect().Center;
	        if (clickPos != null)
	        {
		        Mouse.SetCursorPos(new Vector2(
			        clickPos.Value.X + random.Next(-15, 15),
			        clickPos.Value.Y + random.Next(-10, 10)));
	        }
	        
	        yield return new WaitTime(30 + random.Next(CoPilot.instance.Settings.autoPilotInputFrequency));
        }
        private IEnumerator AutoPilotLogic()
        {
	        while (true)
	        {
		        if (!CoPilot.instance.Settings.Enable.Value || !CoPilot.instance.Settings.autoPilotEnabled.Value || CoPilot.instance.localPlayer == null || !CoPilot.instance.localPlayer.IsAlive || 
		            
		            !CoPilot.instance.GameController.IsForeGroundCache || MenuWindow.IsOpened || CoPilot.instance.GameController.IsLoading || !CoPilot.instance.GameController.InGame)
		        {
			        yield return new WaitTime(100);
			        continue;
		        }
		        
		        //Cache the current follow target (if present)
				followTarget = GetFollowingTarget();
				if (followTarget != null)
				{
					var distanceToLeader = Vector3.Distance(CoPilot.instance.playerPosition, followTarget.Pos);
					//We are NOT within clear path distance range of leader. Logic can continue
					if (distanceToLeader >= CoPilot.instance.Settings.autoPilotClearPathDistance.Value)
					{
						//Leader moved VERY far in one frame. Check for transition to use to follow them.
						var distanceMoved = Vector3.Distance(lastTargetPosition, followTarget.Pos);
						if (lastTargetPosition != Vector3.Zero && distanceMoved > CoPilot.instance.Settings.autoPilotClearPathDistance.Value)
						{
							var transition = GetBestPortalLabel();
							// Check for Portal within Screen Distance.
								if (transition != null && transition.ItemOnGround.DistancePlayer < 80)
									tasks.Add(new TaskNode(transition,200, TaskNodeType.Transition));
						}
						//We have no path, set us to go to leader pos.
						else if (tasks.Count == 0 && distanceMoved < 2000 && distanceToLeader > 200 && distanceToLeader < 2000)
						{
							tasks.Add(new TaskNode(followTarget.Pos, CoPilot.instance.Settings.autoPilotPathfindingNodeDistance));
						}
							
						//We have a path. Check if the last task is far enough away from current one to add a new task node.
						else if (tasks.Count > 0)
						{
							var distanceFromLastTask = Vector3.Distance(tasks.Last().WorldPosition, followTarget.Pos);
							if (distanceFromLastTask >= CoPilot.instance.Settings.autoPilotPathfindingNodeDistance)
								tasks.Add(new TaskNode(followTarget.Pos, CoPilot.instance.Settings.autoPilotPathfindingNodeDistance));
						}
					}
					else
					{
						//Clear all tasks except for looting/claim portal (as those only get done when we're within range of leader. 
						if (tasks.Count > 0)
						{
							for (var i = tasks.Count - 1; i >= 0; i--)
								if (tasks[i].Type == TaskNodeType.Movement || tasks[i].Type == TaskNodeType.Transition)
									tasks.RemoveAt(i);
							yield return null;
						}
						if (CoPilot.instance.Settings.autoPilotCloseFollow.Value)
						{
							//Close follow logic. We have no current tasks. Check if we should move towards leader
							if (distanceToLeader >= CoPilot.instance.Settings.autoPilotPathfindingNodeDistance.Value)
								tasks.Add(new TaskNode(followTarget.Pos, CoPilot.instance.Settings.autoPilotPathfindingNodeDistance));
						}

						//Check if we should add quest loot logic. We're close to leader already
						var questLoot = GetQuestItem();
						if (questLoot != null &&
						    Vector3.Distance(CoPilot.instance.playerPosition, questLoot.Pos) < CoPilot.instance.Settings.autoPilotClearPathDistance.Value &&
						    tasks.FirstOrDefault(I => I.Type == TaskNodeType.Loot) == null)
							tasks.Add(new TaskNode(questLoot.Pos, CoPilot.instance.Settings.autoPilotClearPathDistance, TaskNodeType.Loot));

						else if (!hasUsedWp && CoPilot.instance.Settings.autoPilotTakeWaypoints)
						{
							//Check if there's a waypoint nearby
							var waypoint = CoPilot.instance.GameController.EntityListWrapper.Entities.SingleOrDefault(I => I.Type ==EntityType.Waypoint &&
								Vector3.Distance(CoPilot.instance.playerPosition, I.Pos) < CoPilot.instance.Settings.autoPilotClearPathDistance);

							if (waypoint != null)
							{
								hasUsedWp = true;
								tasks.Add(new TaskNode(waypoint.Pos, CoPilot.instance.Settings.autoPilotClearPathDistance, TaskNodeType.ClaimWaypoint));
							}

						}

					}
					if (followTarget?.Pos != null)
						lastTargetPosition = followTarget.Pos;
				}
				//Leader is null but we have tracked them this map.
				//Try using transition to follow them to their map
				else if (tasks.Count == 0) /* &&
				         lastTargetPosition != Vector3.Zero)*/
				{
					var portal = GetBestPortalLabel();
					if (portal == null || portal != null && portal.ItemOnGround?.DistancePlayer > 80 && CoPilot.instance.GameController?.Area?.CurrentArea?.RealLevel < 68)
					{
						foreach (var partyElementWindow in PartyElements.GetPlayerInfoElementList())
						{
							if (string.Equals(partyElementWindow?.PlayerName?.ToLower(), CoPilot.instance.Settings.autoPilotLeader.Value.ToLower(), StringComparison.CurrentCultureIgnoreCase))
							{
								var windowOffset = CoPilot.instance.GameController.Window.GetWindowRectangle().TopLeft;
								var elemCenter = (Vector2) partyElementWindow?.TpButton?.GetClientRectCache.Center;
								var finalPos = new Vector2(elemCenter.X + windowOffset.X, elemCenter.Y + windowOffset.Y);
								
								yield return Mouse.SetCursorPosHuman(finalPos, false);
								yield return new WaitTime(200);
								yield return Mouse.LeftClick();
								yield return new WaitTime(1500);

								if (CoPilot.instance.GameController.IngameState.IngameUi.ChildCount > 0)
								{
									var ui = (CoPilot.instance.GameController?.IngameState?.IngameUi?.Children).FirstOrDefault(x => x != null && x.ChildCount == 4 && x.Children[3].ChildCount == 5 && x.Children[3].Children[2].IsVisible && x.Children[3].Children[0].Text.Equals("Are you sure you want to teleport to this player's location?"));
									if (ui != null)
										yield return Mouse.SetCursorPosHuman(ui.Children[3].Children[2].GetClientRect()
											.Center);
									yield return new WaitTime(200);
									yield return Mouse.LeftClick();
									yield return new WaitTime(1000);
								}
							}
						}
					}
					else if (portal != null)
					{
						tasks.Add(new TaskNode(portal, CoPilot.instance.Settings.autoPilotPathfindingNodeDistance.Value, TaskNodeType.Transition));
					}
				}

				//We have our tasks, now we need to perform in game logic with them.
				if (tasks?.Count > 0)
				{
					var currentTask = tasks.First();
					var taskDistance = Vector3.Distance(CoPilot.instance.playerPosition, currentTask.WorldPosition);
					var playerDistanceMoved = Vector3.Distance(CoPilot.instance.playerPosition, lastPlayerPosition);

					//We are using a same map transition and have moved significnatly since last tick. Mark the transition task as done.
					if (currentTask.Type == TaskNodeType.Transition && 
					    playerDistanceMoved >= CoPilot.instance.Settings.autoPilotClearPathDistance.Value)
					{
						tasks.RemoveAt(0);
						lastPlayerPosition = CoPilot.instance.playerPosition;
						yield return null;
						continue;
					}
					switch (currentTask.Type)
					{
						case TaskNodeType.Movement:
							if (CoPilot.instance.Settings.autoPilotDashEnabled && CheckDashTerrain(currentTask.WorldPosition.WorldToGrid()))
								yield return null;
							yield return Mouse.SetCursorPosHuman(WorldToValidScreenPosition(currentTask.WorldPosition));
							yield return new WaitTime(random.Next(25) + 30);
							Input.KeyDown(CoPilot.instance.Settings.autoPilotMoveKey);
							yield return new WaitTime(random.Next(25) + 30);
							Input.KeyUp(CoPilot.instance.Settings.autoPilotMoveKey);

							//Within bounding range. Task is complete
							//Note: Was getting stuck on close objects... testing hacky fix.
							if (taskDistance <= CoPilot.instance.Settings.autoPilotPathfindingNodeDistance.Value * 1.5)
								tasks.RemoveAt(0);
							yield return null;
							yield return null;
							continue;
						case TaskNodeType.Loot:
						{
							currentTask.AttemptCount++;
							var questLoot = GetQuestItem();
							if (questLoot == null
							    || currentTask.AttemptCount > 2
							    || Vector3.Distance(CoPilot.instance.playerPosition, questLoot.Pos) >=
							    CoPilot.instance.Settings.autoPilotClearPathDistance.Value)
							{
								tasks.RemoveAt(0);
								yield return null;
							}

							Input.KeyUp(CoPilot.instance.Settings.autoPilotMoveKey);
							yield return new WaitTime(CoPilot.instance.Settings.autoPilotInputFrequency);
							//Pause for long enough for movement to hopefully be finished.
							if (questLoot != null)
							{
								var targetInfo = questLoot.GetComponent<Targetable>();
								switch (targetInfo.isTargeted)
								{
									case false:
										yield return MouseoverItem(questLoot);
										break;
									case true:
										yield return Mouse.LeftClick();
										yield return new WaitTime(1000);
										break;
								}
							}

							break;
						}
						case TaskNodeType.Transition:
						{
							
							//Click the transition
							Input.KeyUp(CoPilot.instance.Settings.autoPilotMoveKey);
							yield return new WaitTime(60);
							yield return Mouse.SetCursorPosAndLeftClickHuman(new Vector2(currentTask.LabelOnGround.Label.GetClientRect().Center.X, currentTask.LabelOnGround.Label.GetClientRect().Center.Y), 100);
							yield return new WaitTime(300);

							currentTask.AttemptCount++;
							if (currentTask.AttemptCount > 6)
								tasks.RemoveAt(0);
							{
								yield return null;
								continue;
							}
						}

						case TaskNodeType.ClaimWaypoint:
						{
							if (Vector3.Distance(CoPilot.instance.playerPosition, currentTask.WorldPosition) > 150)
							{
								var screenPos = WorldToValidScreenPosition(currentTask.WorldPosition);
								Input.KeyUp(CoPilot.instance.Settings.autoPilotMoveKey);
								yield return new WaitTime(CoPilot.instance.Settings.autoPilotInputFrequency);
								yield return Mouse.SetCursorPosAndLeftClickHuman(screenPos, 100);
								yield return new WaitTime(1000);
							}
							currentTask.AttemptCount++;
							if (currentTask.AttemptCount > 3)
								tasks.RemoveAt(0);
							{
								yield return null;
								continue;
							}
						}
					}
				}
				lastPlayerPosition = CoPilot.instance.playerPosition;
				yield return new WaitTime(50);
            }
	        // ReSharper disable once IteratorNeverReturns
        }
        
        private bool CheckDashTerrain(Vector2 targetPosition)
        {
	        if (tiles == null)
		        return false;
			//TODO: Completely re-write this garbage. 
			//It's not taking into account a lot of stuff, horribly inefficient and just not the right way to do this.
			//Calculate the straight path from us to the target (this would be waypoints normally)
			var dir = targetPosition - CoPilot.instance.GameController.Player.GridPos;
			dir.Normalize();

			var distanceBeforeWall = 0;
			var distanceInWall = 0;

			var shouldDash = false;
			var points = new List<System.Drawing.Point>();
			for (var i = 0; i < 500; i++)
			{
				var v2Point = CoPilot.instance.GameController.Player.GridPos + i * dir;
				var point = new System.Drawing.Point((int)(CoPilot.instance.GameController.Player.GridPos.X + i * dir.X),
					(int)(CoPilot.instance.GameController.Player.GridPos.Y + i * dir.Y));

				if (points.Contains(point))
					continue;
				if (Vector2.Distance(v2Point,targetPosition) < 2)
					break;

				points.Add(point);
				var tile = tiles[point.X, point.Y];


				//Invalid tile: Block dash
				if (tile == 255)
				{
					shouldDash = false;
					break;
				}
				else if (tile == 2)
				{
					if (shouldDash)
						distanceInWall++;
					shouldDash = true;
				}
				else if (!shouldDash)
				{
					distanceBeforeWall++;
					if (distanceBeforeWall > 10)					
						break;					
				}
			}

			if (distanceBeforeWall > 10 || distanceInWall < 5)
				shouldDash = false;

			if (shouldDash)
			{
				Mouse.SetCursorPos(WorldToValidScreenPosition(targetPosition.GridToWorld(followTarget == null ? CoPilot.instance.GameController.Player.Pos.Z : followTarget.Pos.Z)));
				Keyboard.KeyPress(CoPilot.instance.Settings.autoPilotDashKey);
				return true;
			}

			return false;
		}

		private Entity GetFollowingTarget()
		{
			try
			{
				return CoPilot.instance.GameController.Entities.First(x => x?.Type == EntityType.Player &&
				                                                           string.Equals(x.GetComponent<Player>()?.PlayerName?.ToLower(), CoPilot.instance.Settings.autoPilotLeader.Value.ToLower()));
			}
			// Sometimes we can get "Collection was modified; enumeration operation may not execute" exception
			catch
			{
				return null;
			}
		}

		private static Entity GetQuestItem()
		{
			try
			{
				return CoPilot.instance.GameController.EntityListWrapper.Entities
					.Where(e => e?.Type == EntityType.WorldItem && e.IsTargetable && e.HasComponent<WorldItem>())
					.FirstOrDefault(e =>
					{
						var itemEntity = e.GetComponent<WorldItem>().ItemEntity;
						return CoPilot.instance.GameController.Files.BaseItemTypes.Translate(itemEntity.Path).ClassName ==
						       "QuestItem";
					});
			}
			catch
			{
				return null;
			}
		}
		
		public void Render()
		{
			if (CoPilot.instance.Settings.autoPilotToggleKey.PressedOnce())
			{
				CoPilot.instance.Settings.autoPilotEnabled.SetValueNoEvent(!CoPilot.instance.Settings.autoPilotEnabled.Value);
				tasks = new List<TaskNode>();				
			}
			
			if (!CoPilot.instance.Settings.autoPilotEnabled || CoPilot.instance.GameController.IsLoading || !CoPilot.instance.GameController.InGame)
				return;

			try
			{
				var portalLabels =
					CoPilot.instance.GameController?.Game?.IngameState?.IngameUi?.ItemsOnGroundLabels.Where(x =>
						x != null && x.IsVisible && x.Label != null && x.Label.IsValid && x.Label.IsVisible &&
						x.ItemOnGround != null &&
						(x.ItemOnGround.Metadata.ToLower().Contains("areatransition") ||
						 x.ItemOnGround.Metadata.ToLower().Contains("portal"))).ToList();

				foreach (var portal in portalLabels)
				{
					CoPilot.instance.Graphics.DrawLine(portal.Label.GetClientRectCache.TopLeft, portal.Label.GetClientRectCache.TopRight, 2f,Color.Firebrick);
				}
			}
			catch (Exception)
			{
				//ignore
			}
			/*			 
			// Debug for UI Element
			try
			{
				foreach (var partyElementWindow in PartyElements.GetPlayerInfoElementList())
				{
					if (string.Equals(partyElementWindow.PlayerName.ToLower(), CoPilot.instance.Settings.autoPilotLeader.Value.ToLower(), StringComparison.CurrentCultureIgnoreCase))
					{
						var windowOffset = CoPilot.instance.GameController.Window.GetWindowRectangle().TopLeft;

						var elemCenter = partyElementWindow.TPButton.GetClientRectCache.Center;
						var finalPos = new Vector2(elemCenter.X + windowOffset.X, elemCenter.Y + windowOffset.Y);
						
						CoPilot.instance.Graphics.DrawText("Offset: " +windowOffset.ToString("F2"),new Vector2(300, 560));
						CoPilot.instance.Graphics.DrawText("Element: " +elemCenter.ToString("F2"),new Vector2(300, 580));
						CoPilot.instance.Graphics.DrawText("Final: " +finalPos.ToString("F2"),new Vector2(300, 600));
					}
				}
			}
			catch (Exception e)
			{
				
			}
			*/
			
			// Cache Task to prevent access while Collection is changing.
			try
			{
				var taskCount = 0;
				var dist = 0f;
				var cachedTasks = tasks;
				if (cachedTasks?.Count > 0)
				{
					CoPilot.instance.Graphics.DrawText(
						"Current Task: " + cachedTasks[0].Type,
						new Vector2(500, 160));
					foreach (var task in cachedTasks.TakeWhile(task => task?.WorldPosition != null))
					{
						if (taskCount == 0)
						{
							CoPilot.instance.Graphics.DrawLine(
								WorldToValidScreenPosition(CoPilot.instance.playerPosition),
								WorldToValidScreenPosition(task.WorldPosition), 2f, Color.Pink);
							dist = Vector3.Distance(CoPilot.instance.playerPosition, task.WorldPosition);
						}
						else
						{
							CoPilot.instance.Graphics.DrawLine(WorldToValidScreenPosition(task.WorldPosition),
								WorldToValidScreenPosition(cachedTasks[taskCount - 1].WorldPosition), 2f, Color.Pink);
						}

						taskCount++;
					}
				}
				if (CoPilot.instance.localPlayer != null)
				{
					var targetDist = Vector3.Distance(CoPilot.instance.playerPosition, lastTargetPosition);
					CoPilot.instance.Graphics.DrawText(
						$"Follow Enabled: {CoPilot.instance.Settings.autoPilotEnabled.Value}", new Vector2(500, 120));
					CoPilot.instance.Graphics.DrawText(
						$"Task Count: {taskCount:D} Next WP Distance: {dist:F} Target Distance: {targetDist:F}",
						new Vector2(500, 140));
					
				}
			}
			catch (Exception)
			{
				// ignored
			}

			CoPilot.instance.Graphics.DrawText("AutoPilot: Active", new Vector2(350, 120));
			CoPilot.instance.Graphics.DrawText("Coroutine: " + (autoPilotCoroutine.Running ? "Active" : "Dead"), new Vector2(350, 140));
			CoPilot.instance.Graphics.DrawText("Leader: " + (followTarget != null ? "Found" : "Null"), new Vector2(350, 160));
			CoPilot.instance.Graphics.DrawLine(new Vector2(490, 120), new Vector2(490,180), 1, Color.White);
		}


		private Vector2 WorldToValidScreenPosition(Vector3 worldPos)
		{
			var windowRect = CoPilot.instance.GameController.Window.GetWindowRectangle();
			var screenPos = Camera.WorldToScreen(worldPos);
			var result = screenPos + windowRect.Location;

			var edgeBounds = 50;
			if (!windowRect.Intersects(new RectangleF(result.X, result.Y, edgeBounds, edgeBounds)))
			{
				//Adjust for offscreen entity. Need to clamp the screen position using the game window info. 
				if (result.X < windowRect.TopLeft.X) result.X = windowRect.TopLeft.X + edgeBounds;
				if (result.Y < windowRect.TopLeft.Y) result.Y = windowRect.TopLeft.Y + edgeBounds;
				if (result.X > windowRect.BottomRight.X) result.X = windowRect.BottomRight.X - edgeBounds;
				if (result.Y > windowRect.BottomRight.Y) result.Y = windowRect.BottomRight.Y - edgeBounds;
			}
			return result;
		}
    }
}