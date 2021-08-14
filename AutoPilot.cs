using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared;
using ExileCore.Shared.Enums;
using SharpDX;

namespace CoPilot
{
    public class AutoPilot
    {
	    // Most Logic taken from Alpha Plugin
        internal Coroutine autoPilotCoroutine;

        private Random random = new Random();
        private static Camera Camera => CoPilot.instance.GameController.Game.IngameState.Camera;		
        public Dictionary<uint, Entity> areaTransitions = new Dictionary<uint, Entity>();
		
        private Vector3 lastTargetPosition;
        private Vector3 lastPlayerPosition;
        private Entity followTarget;

        private bool hasUsedWp;

        
        private List<TaskNode> tasks = new List<TaskNode>();
        private DateTime nextBotAction = DateTime.Now;

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
            areaTransitions = new Dictionary<uint, Entity>();
            hasUsedWp = false;
        }
        public void AreaChange()
        {
            ResetPathing();

            //Load initial transitions!

            foreach (var transition in CoPilot.instance.GameController.EntityListWrapper.Entities.Where(I => I.Type == EntityType.AreaTransition ||
                I.Type == EntityType.Portal ||
                I.Type == EntityType.TownPortal).ToList().Where(transition => !areaTransitions.ContainsKey(transition.Id)))
            {
                areaTransitions.Add(transition.Id, transition);
            }


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
		        
		        if (CoPilot.instance.Settings.autoPilotToggleKey.PressedOnce())
		        {
			        CoPilot.instance.Settings.autoPilotEnabled.SetValueNoEvent(!CoPilot.instance.Settings.autoPilotEnabled.Value);
			        tasks = new List<TaskNode>();				
		        }

		        if (CoPilot.instance.localPlayer == null || !CoPilot.instance.localPlayer.IsAlive || !CoPilot.instance.Settings.Enable ||
		            !CoPilot.instance.Settings.autoPilotEnabled.Value ||
		            !CoPilot.instance.GameController.IsForeGroundCache || MenuWindow.IsOpened)
		        {
			        yield return null;
			        continue;
		        }


		        //Cache the current follow target (if present)
				followTarget = GetFollowingTarget();
				if (followTarget != null)
				{
					var distanceFromFollower = Vector3.Distance(CoPilot.instance.localPlayer.Pos, followTarget.Pos);
					//We are NOT within clear path distance range of leader. Logic can continue
					if (distanceFromFollower >= CoPilot.instance.Settings.autoPilotClearPathDistance.Value)
					{
						//Leader moved VERY far in one frame. Check for transition to use to follow them.
						var distanceMoved = Vector3.Distance(lastTargetPosition, followTarget.Pos);
						if (lastTargetPosition != Vector3.Zero && distanceMoved > CoPilot.instance.Settings.autoPilotClearPathDistance.Value)
						{
							var transition = areaTransitions.Values.OrderBy(I => Vector3.Distance(lastTargetPosition, I.Pos)).FirstOrDefault();
							if (transition != null && Vector3.Distance(lastTargetPosition, transition.Pos) < CoPilot.instance.Settings.autoPilotClearPathDistance.Value)
								tasks.Add(new TaskNode(transition.Pos, 200, TaskNodeType.Transition));
						}
						//We have no path, set us to go to leader pos.
						else if (tasks.Count == 0)
							tasks.Add(new TaskNode(followTarget.Pos, CoPilot.instance.Settings.autoPilotPathfindingNodeDistance));
						//We have a path. Check if the last task is far enough away from current one to add a new task node.
						else
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
						}
						else if (CoPilot.instance.Settings.autoPilotCloseFollow.Value)
						{
							//Close follow logic. We have no current tasks. Check if we should move towards leader
							if (distanceFromFollower >= CoPilot.instance.Settings.autoPilotPathfindingNodeDistance.Value)
								tasks.Add(new TaskNode(followTarget.Pos, CoPilot.instance.Settings.autoPilotPathfindingNodeDistance));
						}

						//Check if we should add quest loot logic. We're close to leader already
						var questLoot = GetLootableQuestItem();
						if (questLoot != null &&
						    Vector3.Distance(CoPilot.instance.localPlayer.Pos, questLoot.Pos) < CoPilot.instance.Settings.autoPilotClearPathDistance.Value &&
						    tasks.FirstOrDefault(I => I.Type == TaskNodeType.Loot) == null)
							tasks.Add(new TaskNode(questLoot.Pos, CoPilot.instance.Settings.autoPilotClearPathDistance, TaskNodeType.Loot));

						else if (!hasUsedWp && CoPilot.instance.Settings.autoPilottakeWaypoints)
						{
							//Check if there's a waypoint nearby
							var waypoint = CoPilot.instance.GameController.EntityListWrapper.Entities.SingleOrDefault(I => I.Type ==EntityType.Waypoint &&
								Vector3.Distance(CoPilot.instance.localPlayer.Pos, I.Pos) < CoPilot.instance.Settings.autoPilotClearPathDistance);

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
				else if (tasks.Count == 0 &&
				         lastTargetPosition != Vector3.Zero)
				{

					var transOptions = areaTransitions.Values.
						Where(I => Vector3.Distance(lastTargetPosition, I.Pos) < CoPilot.instance.Settings.autoPilotClearPathDistance).
						OrderBy(I => Vector3.Distance(lastTargetPosition, I.Pos)).ToArray();
					if (transOptions.Length > 0)
						tasks.Add(new TaskNode(transOptions[random.Next(transOptions.Length)].Pos, CoPilot.instance.Settings.autoPilotPathfindingNodeDistance.Value, TaskNodeType.Transition));
				}

				//We have our tasks, now we need to perform in game logic with them.
				if (DateTime.Now > nextBotAction && tasks.Count > 0)
				{
					var currentTask = tasks.First();
					var taskDistance = Vector3.Distance(CoPilot.instance.localPlayer.Pos, currentTask.WorldPosition);
					var playerDistanceMoved = Vector3.Distance(CoPilot.instance.localPlayer.Pos, lastPlayerPosition);

					//We are using a same map transition and have moved significnatly since last tick. Mark the transition task as done.
					if (currentTask.Type == TaskNodeType.Transition && 
					    playerDistanceMoved >= CoPilot.instance.Settings.autoPilotClearPathDistance.Value)
					{
						tasks.RemoveAt(0);
						if (tasks.Count > 0)
							currentTask = tasks.First();
						else
						{
							lastPlayerPosition = CoPilot.instance.localPlayer.Pos;
							yield return null;
						}
					}
					switch (currentTask.Type)
					{
						case TaskNodeType.Movement:
							nextBotAction = DateTime.Now.AddMilliseconds(CoPilot.instance.Settings.autoPilotInputFrequency + random.Next(CoPilot.instance.Settings.autoPilotInputFrequency));
							if (CoPilot.instance.Settings.autoPilotDashEnabled && CheckDashTerrain(currentTask.WorldPosition.WorldToGrid()))
								yield return null;
							yield return Mouse.SetCursorPosHuman2(WorldToValidScreenPosition(currentTask.WorldPosition));
							yield return new WaitTime(random.Next(25) + 30);
							Input.KeyDown(CoPilot.instance.Settings.autoPilotMoveKey);
							yield return new WaitTime(random.Next(25) + 30);
							Input.KeyUp(CoPilot.instance.Settings.autoPilotMoveKey);

							//Within bounding range. Task is complete
							//Note: Was getting stuck on close objects... testing hacky fix.
							if (taskDistance <= CoPilot.instance.Settings.autoPilotPathfindingNodeDistance.Value * 1.5)
								tasks.RemoveAt(0);
							break;
						case TaskNodeType.Loot:
						{
							nextBotAction = DateTime.Now.AddMilliseconds(CoPilot.instance.Settings.autoPilotInputFrequency + random.Next(CoPilot.instance.Settings.autoPilotInputFrequency));
							currentTask.AttemptCount++;
							var questLoot = GetLootableQuestItem();
							if (questLoot == null
							    || currentTask.AttemptCount > 2
							    || Vector3.Distance(CoPilot.instance.localPlayer.Pos, questLoot.Pos) >= CoPilot.instance.Settings.autoPilotClearPathDistance.Value)
								tasks.RemoveAt(0);

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
										nextBotAction = DateTime.Now.AddSeconds(1);
										break;
								}
							}

							break;
						}
						case TaskNodeType.Transition:
						{
							nextBotAction = DateTime.Now.AddMilliseconds(CoPilot.instance.Settings.autoPilotInputFrequency * 2 + random.Next(CoPilot.instance.Settings.autoPilotInputFrequency));
							var screenPos = WorldToValidScreenPosition(currentTask.WorldPosition);							
							if (taskDistance <= CoPilot.instance.Settings.autoPilotClearPathDistance.Value)
							{
								//Click the transition
								Input.KeyUp(CoPilot.instance.Settings.autoPilotMoveKey);
								yield return Mouse.SetCursorPosAndLeftClickHuman(screenPos, 100);
								nextBotAction = DateTime.Now.AddSeconds(1);
							}
							else
							{
								//Walk towards the transition
								yield return Mouse.SetCursorPosHuman2(screenPos);
								yield return new WaitTime(random.Next(25) + 30);
								Input.KeyDown(CoPilot.instance.Settings.autoPilotMoveKey);
								yield return new WaitTime(random.Next(25) + 30);
								Input.KeyUp(CoPilot.instance.Settings.autoPilotMoveKey);
							}
							currentTask.AttemptCount++;
							if (currentTask.AttemptCount > 3)
								tasks.RemoveAt(0);
							break;
						}

						case TaskNodeType.ClaimWaypoint:
						{
							if (Vector3.Distance(CoPilot.instance.localPlayer.Pos, currentTask.WorldPosition) > 150)
							{
								var screenPos = WorldToValidScreenPosition(currentTask.WorldPosition);
								Input.KeyUp(CoPilot.instance.Settings.autoPilotMoveKey);
								yield return new WaitTime(CoPilot.instance.Settings.autoPilotInputFrequency);
								yield return Mouse.SetCursorPosAndLeftClickHuman(screenPos, 100);
								nextBotAction = DateTime.Now.AddSeconds(1);
							}
							currentTask.AttemptCount++;
							if (currentTask.AttemptCount > 3)
								tasks.RemoveAt(0);
							break;
						}
					}
				}
				lastPlayerPosition = CoPilot.instance.localPlayer.Pos;
				yield return null;
            }
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
				nextBotAction = DateTime.Now.AddMilliseconds(500 + random.Next(CoPilot.instance.Settings.autoPilotInputFrequency));
				Mouse.SetCursorPos(WorldToValidScreenPosition(targetPosition.GridToWorld(followTarget == null ? CoPilot.instance.GameController.Player.Pos.Z : followTarget.Pos.Z)));
				Keyboard.KeyPress(CoPilot.instance.Settings.autoPilotDashKey);
				return true;
			}

			return false;
		}

		private Entity GetFollowingTarget()
		{
			var leaderName = CoPilot.instance.Settings.autoPilotLeader.Value.ToLower();
			try
			{
				return CoPilot.instance.GameController.Entities.First(x => x?.Type == EntityType.Player &&
				                                                           x.GetComponent<Player>()?.PlayerName?.ToLower() == leaderName);
			}
			// Sometimes we can get "Collection was modified; enumeration operation may not execute" exception
			catch
			{
				return null;
			}
		}

		private Entity GetLootableQuestItem()
		{
			try
			{
				return CoPilot.instance.GameController.EntityListWrapper.Entities
					.Where(e => e?.Type == EntityType.WorldItem)
					.Where(e => e.IsTargetable)
					.Where(e => e.GetComponent<WorldItem>() != null)
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
			if (!CoPilot.instance.Settings.autoPilotEnabled || CoPilot.instance.GameController.IsLoading || !CoPilot.instance.GameController.InGame)
				return;

			var x = 0;
			var dist = 0f;
			// Cache Task to prevent access while Collection is changing.
			var cachedTasks = tasks;
			if (cachedTasks?.Count > 0 && CoPilot.instance?.localPlayer?.Pos != null)
			{
				foreach (var task in cachedTasks.TakeWhile(task => task?.WorldPosition != null))
				{
					if (x == 0)
					{
						CoPilot.instance.Graphics.DrawLine(WorldToValidScreenPosition(CoPilot.instance.localPlayer.Pos),
							WorldToValidScreenPosition(task.WorldPosition), 2f, Color.Pink);
						dist = Vector3.Distance(CoPilot.instance.GameController.Player.Pos, task.WorldPosition);
					}
					else 
					{
						CoPilot.instance.Graphics.DrawLine(WorldToValidScreenPosition(task.WorldPosition),
							WorldToValidScreenPosition(cachedTasks[x - 1].WorldPosition), 2f, Color.Pink);
					}
					x++;
				}
			}

			if (CoPilot.instance.localPlayer != null)
			{
				var targetDist = Vector3.Distance(CoPilot.instance.localPlayer.Pos, lastTargetPosition);
				CoPilot.instance.Graphics.DrawText(
					$"Follow Enabled: {CoPilot.instance.Settings.autoPilotEnabled.Value}", new Vector2(500, 120));
				CoPilot.instance.Graphics.DrawText(
					$"Task Count: {x:D} Next WP Distance: {dist:F} Target Distance: {targetDist:F}",
					new Vector2(500, 140));
			}

			var counter = 0;
			var cachedAreaTransitions = areaTransitions;
			foreach (var transition in cachedAreaTransitions)
			{
				counter++;
				CoPilot.instance.Graphics.DrawText($"{transition.Key} at { transition.Value.Pos.X:F} { transition.Value.Pos.Y:F}", new Vector2(100, 120 + counter * 20));
			}

			CoPilot.instance.Graphics.DrawText("AutoPilot: Active", new Vector2(350, 120));
			CoPilot.instance.Graphics.DrawText("Coroutine: " + (autoPilotCoroutine.Running ? "Active" : "Dead"), new Vector2(350, 140));
			CoPilot.instance.Graphics.DrawLine(new Vector2(490, 120), new Vector2(490,160), 1, Color.White);
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