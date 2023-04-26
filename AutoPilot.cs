using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
    [SuppressMessage("Interoperability", "CA1416:Plattformkompatibilität überprüfen")]
    public class AutoPilot
    {
	    // Most Logic taken from Alpha Plugin
	    private Coroutine autoPilotCoroutine;
	    private readonly Random random = new Random();
        
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

        private PartyElementWindow GetLeaderPartyElement()
        {
	        try
	        {
				foreach (var partyElementWindow in PartyElements.GetPlayerInfoElementList())
				{
					if (string.Equals(partyElementWindow?.PlayerName?.ToLower(), CoPilot.Instance.Settings.autoPilotLeader.Value.ToLower(), StringComparison.CurrentCultureIgnoreCase))
					{
						return partyElementWindow;
					}
				}
				return null;
	        }
	        catch
	        {
		        return null;
	        }
        }

        private LabelOnGround GetBestPortalLabel(PartyElementWindow leaderPartyElement)
        {
	        try
	        {
				var currentZoneName = CoPilot.Instance.GameController?.Area.CurrentArea.DisplayName;
				if(leaderPartyElement.ZoneName.Equals(currentZoneName) || (!leaderPartyElement.ZoneName.Equals(currentZoneName) && (bool)CoPilot.Instance?.GameController?.Area?.CurrentArea?.IsHideout) || CoPilot.Instance.GameController?.Area?.CurrentArea?.RealLevel >= 68) // TODO: or is chamber of sins a7 or is epilogue
				{
					var portalLabels =
						CoPilot.Instance.GameController?.Game?.IngameState?.IngameUi?.ItemsOnGroundLabels.Where(x =>
							x != null && x.IsVisible && x.Label != null && x.Label.IsValid && x.Label.IsVisible && x.ItemOnGround != null && 
							(x.ItemOnGround.Metadata.ToLower().Contains("areatransition") || x.ItemOnGround.Metadata.ToLower().Contains("portal") ))
							.OrderBy(x => Vector3.Distance(lastTargetPosition, x.ItemOnGround.Pos)).ToList();


					return CoPilot.Instance?.GameController?.Area?.CurrentArea?.IsHideout != null && (bool)CoPilot.Instance.GameController?.Area?.CurrentArea?.IsHideout
						? portalLabels?[random.Next(portalLabels.Count)]
						: portalLabels?.FirstOrDefault();
				}
				return null;
	        }
	        catch
	        {
		        return null;
	        }
        }
		private Vector2 GetTpButton(PartyElementWindow leaderPartyElement)
		{
			try
			{
				var windowOffset = CoPilot.Instance.GameController.Window.GetWindowRectangle().TopLeft;
				var elemCenter = (Vector2) leaderPartyElement?.TpButton?.GetClientRectCache.Center;
				var finalPos = new Vector2(elemCenter.X + windowOffset.X, elemCenter.Y + windowOffset.Y);
				
				return finalPos;
			}
			catch
			{
				return Vector2.Zero;
			}
		}
		private Element GetTpConfirmation()
		{
			try
			{
				var ui = CoPilot.Instance.GameController?.IngameState?.IngameUi?.PopUpWindow;

				if (ui.Children[3].Children[0].Text.Equals("Are you sure you want to teleport to this player's location?"))
					return ui.Children[3].Children[2];
				
				return null;
			}
			catch
			{
				return null;
			}
		}
        public void AreaChange()
        {
            ResetPathing();
            
            var terrain = CoPilot.Instance.GameController.IngameState.Data.Terrain;
            var terrainBytes = CoPilot.Instance.GameController.Memory.ReadBytes(terrain.LayerMelee.First, terrain.LayerMelee.Size);
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

            terrainBytes = CoPilot.Instance.GameController.Memory.ReadBytes(terrain.LayerRanged.First, terrain.LayerRanged.Size);
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
	        autoPilotCoroutine = new Coroutine(AutoPilotLogic(), CoPilot.Instance, "AutoPilot");
	        Core.ParallelRunner.Run(autoPilotCoroutine);
	    }
        private IEnumerator MouseoverItem(Entity item)
        {
	        var uiLoot = CoPilot.Instance.GameController.IngameState.IngameUi.ItemsOnGroundLabels.FirstOrDefault(I => I.IsVisible && I.ItemOnGround.Id == item.Id);
	        if (uiLoot == null) yield return null;
	        var clickPos = uiLoot?.Label?.GetClientRect().Center;
	        if (clickPos != null)
	        {
		        Mouse.SetCursorPos(new Vector2(
			        clickPos.Value.X + random.Next(-15, 15),
			        clickPos.Value.Y + random.Next(-10, 10)));
	        }
	        
	        yield return new WaitTime(30 + random.Next(CoPilot.Instance.Settings.autoPilotInputFrequency));
        }
        private IEnumerator AutoPilotLogic()
        {
	        while (true)
	        {
		        if (!CoPilot.Instance.Settings.Enable.Value || !CoPilot.Instance.Settings.autoPilotEnabled.Value || CoPilot.Instance.localPlayer == null || !CoPilot.Instance.localPlayer.IsAlive || 
		            !CoPilot.Instance.GameController.IsForeGroundCache || MenuWindow.IsOpened || CoPilot.Instance.GameController.IsLoading || !CoPilot.Instance.GameController.InGame)
		        {
			        yield return new WaitTime(100);
			        continue;
		        }
		        
		        //Cache the current follow target (if present)
				followTarget = GetFollowingTarget();
				var leaderPartyElement = GetLeaderPartyElement();

				if (followTarget == null && !leaderPartyElement.ZoneName.Equals(CoPilot.Instance.GameController?.Area.CurrentArea.DisplayName)) {
					var portal = GetBestPortalLabel(leaderPartyElement);
					if (portal != null) {
						// Hideout -> Map || Chamber of Sins A7 -> Map
						tasks.Add(new TaskNode(portal, CoPilot.Instance.Settings.autoPilotPathfindingNodeDistance.Value, TaskNodeType.Transition));
					} else {
						// Swirly-able (inverted due to overlay)
						
						var tpConfirmation = GetTpConfirmation();
						if (tpConfirmation != null)
						{
							yield return Mouse.SetCursorPosHuman(tpConfirmation.GetClientRect()
								.Center);
							yield return new WaitTime(200);
							yield return Mouse.LeftClick();
							yield return new WaitTime(1000);
						}
						
						// TODO: change to tasks.Add
						var tpButton = GetTpButton(leaderPartyElement);
						if(!tpButton.Equals(Vector2.Zero))
						{
							yield return Mouse.SetCursorPosHuman(tpButton, false);
							yield return new WaitTime(200);
							yield return Mouse.LeftClick();
							yield return new WaitTime(200);
						}
					}
				} else if (followTarget != null) {
					// TODO: If in town, do not follow (optional)
					var distanceToLeader = Vector3.Distance(CoPilot.Instance.playerPosition, followTarget.Pos);
					//We are NOT within clear path distance range of leader. Logic can continue
					if (distanceToLeader >= CoPilot.Instance.Settings.autoPilotClearPathDistance.Value)
					{
						//Leader moved VERY far in one frame. Check for transition to use to follow them.
						var distanceMoved = Vector3.Distance(lastTargetPosition, followTarget.Pos);
						if (lastTargetPosition != Vector3.Zero && distanceMoved > CoPilot.Instance.Settings.autoPilotClearPathDistance.Value)
						{
							var transition = GetBestPortalLabel(leaderPartyElement);
							// Check for Portal within Screen Distance.
								if (transition != null && transition.ItemOnGround.DistancePlayer < 80)
									tasks.Add(new TaskNode(transition,200, TaskNodeType.Transition));
						}
						//We have no path, set us to go to leader pos.
						else if (tasks.Count == 0 && distanceMoved < 2000 && distanceToLeader > 200 && distanceToLeader < 2000)
						{
							tasks.Add(new TaskNode(followTarget.Pos, CoPilot.Instance.Settings.autoPilotPathfindingNodeDistance));
						}
							
						//We have a path. Check if the last task is far enough away from current one to add a new task node.
						else if (tasks.Count > 0)
						{
							var distanceFromLastTask = Vector3.Distance(tasks.Last().WorldPosition, followTarget.Pos);
							if (distanceFromLastTask >= CoPilot.Instance.Settings.autoPilotPathfindingNodeDistance)
								tasks.Add(new TaskNode(followTarget.Pos, CoPilot.Instance.Settings.autoPilotPathfindingNodeDistance));
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
						if (CoPilot.Instance.Settings.autoPilotCloseFollow.Value)
						{
							//Close follow logic. We have no current tasks. Check if we should move towards leader
							if (distanceToLeader >= CoPilot.Instance.Settings.autoPilotPathfindingNodeDistance.Value)
								tasks.Add(new TaskNode(followTarget.Pos, CoPilot.Instance.Settings.autoPilotPathfindingNodeDistance));
						}

						//Check if we should add quest loot logic. We're close to leader already
						var questLoot = GetQuestItem();
						if (questLoot != null &&
						    Vector3.Distance(CoPilot.Instance.playerPosition, questLoot.Pos) < CoPilot.Instance.Settings.autoPilotClearPathDistance.Value &&
						    tasks.FirstOrDefault(I => I.Type == TaskNodeType.Loot) == null)
							tasks.Add(new TaskNode(questLoot.Pos, CoPilot.Instance.Settings.autoPilotClearPathDistance, TaskNodeType.Loot));

						else if (!hasUsedWp && CoPilot.Instance.Settings.autoPilotTakeWaypoints)
						{
							//Check if there's a waypoint nearby
							var waypoint = CoPilot.Instance.GameController.EntityListWrapper.Entities.SingleOrDefault(I => I.Type ==EntityType.Waypoint &&
								Vector3.Distance(CoPilot.Instance.playerPosition, I.Pos) < CoPilot.Instance.Settings.autoPilotClearPathDistance);

							if (waypoint != null)
							{
								hasUsedWp = true;
								tasks.Add(new TaskNode(waypoint.Pos, CoPilot.Instance.Settings.autoPilotClearPathDistance, TaskNodeType.ClaimWaypoint));
							}

						}
					}
					if (followTarget?.Pos != null)
						lastTargetPosition = followTarget.Pos;
				}

				//We have our tasks, now we need to perform in game logic with them.
				if (tasks?.Count > 0)
				{
					var currentTask = tasks.First();
					var taskDistance = Vector3.Distance(CoPilot.Instance.playerPosition, currentTask.WorldPosition);
					var playerDistanceMoved = Vector3.Distance(CoPilot.Instance.playerPosition, lastPlayerPosition);

					//We are using a same map transition and have moved significnatly since last tick. Mark the transition task as done.
					if (currentTask.Type == TaskNodeType.Transition && 
					    playerDistanceMoved >= CoPilot.Instance.Settings.autoPilotClearPathDistance.Value)
					{
						tasks.RemoveAt(0);
						lastPlayerPosition = CoPilot.Instance.playerPosition;
						yield return null;
						continue;
					}
					switch (currentTask.Type)
					{
						case TaskNodeType.Movement:
							if (CoPilot.Instance.Settings.autoPilotDashEnabled && CheckDashTerrain(currentTask.WorldPosition.WorldToGrid()))
								yield return null;
							yield return Mouse.SetCursorPosHuman(Helper.WorldToValidScreenPosition(currentTask.WorldPosition));
							yield return new WaitTime(random.Next(25) + 30);
							Input.KeyDown(CoPilot.Instance.Settings.autoPilotMoveKey);
							yield return new WaitTime(random.Next(25) + 30);
							Input.KeyUp(CoPilot.Instance.Settings.autoPilotMoveKey);

							//Within bounding range. Task is complete
							//Note: Was getting stuck on close objects... testing hacky fix.
							if (taskDistance <= CoPilot.Instance.Settings.autoPilotPathfindingNodeDistance.Value * 1.5)
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
							    || Vector3.Distance(CoPilot.Instance.playerPosition, questLoot.Pos) >=
							    CoPilot.Instance.Settings.autoPilotClearPathDistance.Value)
							{
								tasks.RemoveAt(0);
								yield return null;
							}

							Input.KeyUp(CoPilot.Instance.Settings.autoPilotMoveKey);
							yield return new WaitTime(CoPilot.Instance.Settings.autoPilotInputFrequency);
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
							Input.KeyUp(CoPilot.Instance.Settings.autoPilotMoveKey);
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
							if (Vector3.Distance(CoPilot.Instance.playerPosition, currentTask.WorldPosition) > 150)
							{
								var screenPos = Helper.WorldToValidScreenPosition(currentTask.WorldPosition);
								Input.KeyUp(CoPilot.Instance.Settings.autoPilotMoveKey);
								yield return new WaitTime(CoPilot.Instance.Settings.autoPilotInputFrequency);
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
				lastPlayerPosition = CoPilot.Instance.playerPosition;
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
			var dir = targetPosition - CoPilot.Instance.GameController.Player.GridPos;
			dir.Normalize();

			var distanceBeforeWall = 0;
			var distanceInWall = 0;

			var shouldDash = false;
			var points = new List<System.Drawing.Point>();
			for (var i = 0; i < 500; i++)
			{
				var v2Point = CoPilot.Instance.GameController.Player.GridPos + i * dir;
				var point = new System.Drawing.Point((int)(CoPilot.Instance.GameController.Player.GridPos.X + i * dir.X),
					(int)(CoPilot.Instance.GameController.Player.GridPos.Y + i * dir.Y));

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
				Mouse.SetCursorPos(Helper.WorldToValidScreenPosition(targetPosition.GridToWorld(followTarget == null ? CoPilot.Instance.GameController.Player.Pos.Z : followTarget.Pos.Z)));
				Keyboard.KeyPress(CoPilot.Instance.Settings.autoPilotDashKey);
				return true;
			}

			return false;
		}

		private Entity GetFollowingTarget()
		{
			try
			{
				string leaderName = CoPilot.Instance.Settings.autoPilotLeader.Value.ToLower();
				return CoPilot.Instance.GameController.EntityListWrapper.ValidEntitiesByType[EntityType.Player].FirstOrDefault(x => string.Equals(x.GetComponent<Player>()?.PlayerName.ToLower(), leaderName, StringComparison.OrdinalIgnoreCase));
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
				return CoPilot.Instance.GameController.EntityListWrapper.Entities
					.Where(e => e?.Type == EntityType.WorldItem && e.IsTargetable && e.HasComponent<WorldItem>())
					.FirstOrDefault(e =>
					{
						var itemEntity = e.GetComponent<WorldItem>().ItemEntity;
						return CoPilot.Instance.GameController.Files.BaseItemTypes.Translate(itemEntity.Path).ClassName ==
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
			if (CoPilot.Instance.Settings.autoPilotToggleKey.PressedOnce())
			{
				CoPilot.Instance.Settings.autoPilotEnabled.SetValueNoEvent(!CoPilot.Instance.Settings.autoPilotEnabled.Value);
				tasks = new List<TaskNode>();				
			}
			
			if (!CoPilot.Instance.Settings.autoPilotEnabled || CoPilot.Instance.GameController.IsLoading || !CoPilot.Instance.GameController.InGame)
				return;

			try
			{
				var portalLabels =
					CoPilot.Instance.GameController?.Game?.IngameState?.IngameUi?.ItemsOnGroundLabels.Where(x =>
						x != null && x.IsVisible && x.Label != null && x.Label.IsValid && x.Label.IsVisible &&
						x.ItemOnGround != null &&
						(x.ItemOnGround.Metadata.ToLower().Contains("areatransition") ||
						 x.ItemOnGround.Metadata.ToLower().Contains("portal"))).ToList();

				foreach (var portal in portalLabels)
				{
					CoPilot.Instance.Graphics.DrawLine(portal.Label.GetClientRectCache.TopLeft, portal.Label.GetClientRectCache.TopRight, 2f,Color.Firebrick);
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
					CoPilot.Instance.Graphics.DrawText(
						"Current Task: " + cachedTasks[0].Type,
						new Vector2(500, 160));
					foreach (var task in cachedTasks.TakeWhile(task => task?.WorldPosition != null))
					{
						if (taskCount == 0)
						{
							CoPilot.Instance.Graphics.DrawLine(
								Helper.WorldToValidScreenPosition(CoPilot.Instance.playerPosition),
								Helper.WorldToValidScreenPosition(task.WorldPosition), 2f, Color.Pink);
							dist = Vector3.Distance(CoPilot.Instance.playerPosition, task.WorldPosition);
						}
						else
						{
							CoPilot.Instance.Graphics.DrawLine(Helper.WorldToValidScreenPosition(task.WorldPosition),
								Helper.WorldToValidScreenPosition(cachedTasks[taskCount - 1].WorldPosition), 2f, Color.Pink);
						}

						taskCount++;
					}
				}
				if (CoPilot.Instance.localPlayer != null)
				{
					var targetDist = Vector3.Distance(CoPilot.Instance.playerPosition, lastTargetPosition);
					CoPilot.Instance.Graphics.DrawText(
						$"Follow Enabled: {CoPilot.Instance.Settings.autoPilotEnabled.Value}", new System.Numerics.Vector2(500, 120));
					CoPilot.Instance.Graphics.DrawText(
						$"Task Count: {taskCount:D} Next WP Distance: {dist:F} Target Distance: {targetDist:F}",
						new System.Numerics.Vector2(500, 140));
					
				}
			}
			catch (Exception)
			{
				// ignored
			}

			CoPilot.Instance.Graphics.DrawText("AutoPilot: Active", new System.Numerics.Vector2(350, 120));
			CoPilot.Instance.Graphics.DrawText("Coroutine: " + (autoPilotCoroutine.Running ? "Active" : "Dead"), new System.Numerics.Vector2(350, 140));
			CoPilot.Instance.Graphics.DrawText("Leader: " + (followTarget != null ? "Found" : "Null"), new System.Numerics.Vector2(350, 160));
			CoPilot.Instance.Graphics.DrawLine(new System.Numerics.Vector2(490, 120), new System.Numerics.Vector2(490,180), 1, Color.White);
		}


		
    }
}
