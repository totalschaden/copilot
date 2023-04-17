using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared;
using ExileCore.Shared.Enums;
using SharpDX;

namespace CoPilot
{
    [SuppressMessage("Interoperability", "CA1416:Plattformkompatibilität überprüfen")]
    public class CoPilot : BaseSettingsPlugin<CoPilotSettings>
    {
        

        private const int Delay = 45;

        private const int MouseAutoSnapRange = 250;
        internal static CoPilot Instance;
        internal AutoPilot autoPilot = new AutoPilot();
        private readonly Summons summons = new Summons();

        private DateTime autoAttackRunning;
        private DateTime autoAttackUpdate;

        private bool bladeBlastReady;
        private List<Buff> buffs;

        
        private List<Entity> corpses = new List<Entity>();
        private List<Entity> enemys = new List<Entity>();
        private bool isAttacking;
        private bool isCasting;
        private bool isMoving;
        private DateTime lastCurse;
        private DateTime lastCustom;
        private DateTime lastDelveFlare;
        private DateTime lastStackSkill;
        internal DateTime lastTimeAny;
        internal Entity localPlayer;
        internal Life player;
        internal Vector3 playerPosition;
        private Coroutine skillCoroutine;
        internal List<ActorSkill> skills = new List<ActorSkill>();
        private bool updateBladeBlast;
        private List<ActorVaalSkill> vaalSkills = new List<ActorVaalSkill>();

        private int cwdtCounter = 0;



        public override bool Initialise()
        {
            if (Instance == null)
                Instance = this;
            GameController.LeftPanel.WantUse(() => Settings.Enable);
            skillCoroutine = new Coroutine(WaitForSkillsAfterAreaChange(), this);
            Core.ParallelRunner.Run(skillCoroutine);
            Input.RegisterKey(Settings.autoPilotToggleKey.Value);
            Settings.autoPilotToggleKey.OnValueChanged += () => { Input.RegisterKey(Settings.autoPilotToggleKey.Value); };
            autoPilot.StartCoroutine();
            return true;
        }
        

        private int GetMinnionsWithin(float maxDistance)
        {
            return localPlayer.GetComponent<Actor>().DeployedObjects.Where(x => x?.Entity != null && x.Entity.IsAlive).Select(minnion => Vector2.Distance(new Vector2(minnion.Entity.Pos.X, minnion.Entity.Pos.Y), new Vector2(playerPosition.X, playerPosition.Y))).Count(distance => distance <= maxDistance);
        }

        private int GetMonsterWithin(float maxDistance, MonsterRarity rarity = MonsterRarity.White)
        {
            return (from monster in enemys where monster.Rarity >= rarity select Vector2.Distance(new Vector2(monster.PosNum.X, monster.PosNum.Y), new Vector2(playerPosition.X, playerPosition.Y))).Count(distance => distance <= maxDistance);
        }
        
        private int GetCorpseWithin(float maxDistance)
        {
            return corpses.Select(corpse => Vector2.Distance(new Vector2(corpse.PosNum.X, corpse.PosNum.Y), new Vector2(playerPosition.X, playerPosition.Y))).Count(distance => distance <= maxDistance);
        }

        private bool MonsterCheck(int range, int minAny, int minRare, int minUnique)
        {
            int any = 0, rare = 0, unique = 0;
            foreach (var monster in enemys)
                switch (monster.Rarity)
                {
                    case MonsterRarity.White:
                    {
                        if (Vector2.Distance(new Vector2(monster.PosNum.X, monster.PosNum.Y),
                            new Vector2(playerPosition.X, playerPosition.Y)) <= range)
                            any++;
                        break;
                    }
                    case MonsterRarity.Magic:
                    {
                        if (Vector2.Distance(new Vector2(monster.PosNum.X, monster.PosNum.Y),
                            new Vector2(playerPosition.X, playerPosition.Y)) <= range)
                            any++;
                        break;
                    }
                    case MonsterRarity.Rare:
                    {
                        if (Vector2.Distance(new Vector2(monster.PosNum.X, monster.PosNum.Y),
                            new Vector2(playerPosition.X, playerPosition.Y)) <= range)
                        {
                            any++;
                            rare++;
                        }
                        break;
                    }
                    case MonsterRarity.Unique:
                    {
                        if (Vector2.Distance(new Vector2(monster.PosNum.X, monster.PosNum.Y),
                            new Vector2(playerPosition.X, playerPosition.Y)) <= range)
                        {
                            any++;
                            rare++;
                            unique++;
                        }
                        break;
                    }
                }

            if (minUnique > 0 && unique >= minUnique) return true;

            if (minRare > 0 && rare >= minRare) return true;

            if (minAny > 0 && any >= minAny) return true;

            return minAny == 0 && minRare == 0 && minUnique == 0;
        }

        internal Vector2 GetMousePosition()
        {
            return new Vector2(GameController.IngameState.MousePosX, GameController.IngameState.MousePosY);
        }
        private int CountCorpsesAroundMouse(float maxDistance)
        {
            return corpses.Count(x => Vector2.Distance(GameController.IngameState.Camera.WorldToScreen(x.Pos), GetMousePosition()) <= maxDistance);
        }

        private int CountEnemysAroundMouse(float maxDistance)
        {
            return enemys.Count(x => Vector2.Distance(GameController.IngameState.Camera.WorldToScreen(x.Pos), GetMousePosition()) <= maxDistance);
        }

        private void UpdateBladeBlast()
        {
            var bladefall = GameController.Game.IngameState.Data.LocalPlayer.GetComponent<Actor>().ActorSkills
                .Find(x => x.InternalName == "bladefall");
            switch (bladefall.IsUsing)
            {
                case true when updateBladeBlast:
                    bladeBlastReady = true;
                    updateBladeBlast = false;
                    break;
                case false:
                    updateBladeBlast = true;
                    break;
            }
        }

        private int CountBladeBlastEnitytiesNearMouse(float maxDistance)
        {
            var bladeEntities =
                GameController.Entities.Where(x => x.IsValid && !x.IsTransitioned && x.Path.Contains("GroundBlade"));
            // Server Effect Entity from Blade Blast ?
            // If Blade Blast already active, we dont need to cast again.
            // Too Slow
            if (GameController.Entities.Any(x =>
                x.Path.Contains("ServerEffect") && x.IsValid && bladeEntities.Any(y =>
                    y.GetComponent<Positioned>()?.WorldPos == x.GetComponent<Positioned>()?.WorldPos)))
                return 0;

            return bladeEntities.Count(x => Vector2.Distance(GameController.IngameState.Camera.WorldToScreen(x.Pos), GetMousePosition()) <= maxDistance);
        }

        private int CountNonCursedEnemysAroundMouse(float maxDistance)
        {
            return enemys.Count(x => Vector2.Distance(GameController.IngameState.Camera.WorldToScreen(x.Pos), GetMousePosition()) <= maxDistance && !EntityHasCurse(x));
        }

        private bool EntityHasCurse(Entity entity)
        {
            return entity.GetComponent<Buffs>().BuffsList.Exists(x =>
                x.Name == SkillInfo.punishment.BuffName);
        }

        public bool Gcd()
        {
            return (DateTime.Now - lastTimeAny).TotalMilliseconds > Delay;
        }

        private void Quit()
        {
            try
            {
                CommandHandler.KillTcpConnectionForProcess(GameController.Window.Process.Id);
            }
            catch (Exception e)
            {
                LogError($"{e}");
            }
        }

        private Keys GetSkillInputKey(int index)
        {
            switch (index)
            {
                case 1:
                    return Settings.inputKey1.Value;
                case 3:
                    return Settings.inputKey3.Value;
                case 4:
                    return Settings.inputKey4.Value;
                case 5:
                    return Settings.inputKey5.Value;
                case 6:
                    return Settings.inputKey6.Value;
                case 7:
                    return Settings.inputKey7.Value;
                case 8:
                    return Settings.inputKey8.Value;
                case 9:
                    return Settings.inputKey9.Value;
                case 10:
                    return Settings.inputKey10.Value;
                case 11:
                    return Settings.inputKey11.Value;
                case 12:
                    return Settings.inputKey12.Value;
                default:
                    return Keys.Escape;
            }
        }

        private IEnumerator WaitForSkillsAfterAreaChange()
        {
            while (skills == null || localPlayer == null || GameController.IsLoading || !GameController.InGame)
                yield return new WaitTime(200);

            yield return new WaitTime(1000);
            SkillInfo.UpdateSkillInfo(true);
        }

        public override void AreaChange(AreaInstance area)
        {
            base.AreaChange(area);
            SkillInfo.ResetSkills();
            skills = null;
            bladeBlastReady = false;
            updateBladeBlast = true;

            var coroutine = new Coroutine(WaitForSkillsAfterAreaChange(), this);
            Core.ParallelRunner.Run(coroutine);
            
            autoPilot.AreaChange();
        }
        
        public override void DrawSettings()
        {
            //base.DrawSettings();

            // Draw Custom GUI
            if (Settings.Enable)
                ImGuiDrawSettings.DrawImGuiSettings();
        }

        private static bool HasStat(Entity monster, GameStat stat)
        {
            // Using this with GameController.EntityListWrapper.ValidEntitiesByType[EntityType.Monster].Where
            // seems to cause Nullref errors on TC Fork. Where using the Code directly in a check seems fine, must have to do with Entity Parameter.
            // Maybe someone knows why, i dont :)
            try
            {
                var value = monster?.GetComponent<Stats>()?.StatDictionary?[stat];
                    return value > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override void Render()
        {
            try
            {
                if (!Settings.Enable) return;
                SkillInfo.GetDeltaTime();
                
                try
                {
                    if (Settings.autoPilotEnabled && Settings.autoPilotGrace && buffs != null && buffs.Exists(x => x.Name == "grace_period") && Gcd())
                    {
                        Keyboard.KeyPress(Settings.autoPilotMoveKey);
                    }
                    autoPilot.Render();
                }
                catch (Exception e)
                {
                    LogError(e.ToString());
                }
                
                if (Settings.autoQuitHotkeyEnabled && (WinApi.GetAsyncKeyState(Settings.forcedAutoQuit) & 0x8000) != 0)
                {
                    LogMessage("Copilot: Panic Quit...");
                    Quit();
                }

                if (GameController?.Game?.IngameState?.Data?.LocalPlayer == null || GameController?.IngameState?.IngameUi == null )
                    return;
                var chatField = GameController?.IngameState?.IngameUi?.ChatPanel?.Children[3]?.IsVisible;
                if (chatField != null && (bool)chatField)
                    return;
                    
                localPlayer = GameController.Game.IngameState.Data.LocalPlayer;
                player = localPlayer.GetComponent<Life>();
                buffs = localPlayer.GetComponent<Buffs>().BuffsList;
                isAttacking = localPlayer.GetComponent<Actor>().isAttacking;
                isCasting = localPlayer.GetComponent<Actor>().Action.HasFlag(ActionFlags.UsingAbility);
                isMoving = localPlayer.GetComponent<Actor>().isMoving;
                skills = localPlayer.GetComponent<Actor>().ActorSkills;
                vaalSkills = localPlayer.GetComponent<Actor>().ActorVaalSkills;
                playerPosition = localPlayer.Pos;
                
                #region Auto Map Tabber

                try
                {
                    if (Settings.autoMapTabber && !Keyboard.IsKeyDown((int)Settings.inputKeyPickIt.Value))
                        if (SkillInfo.ManageCooldown(SkillInfo.autoMapTabber))
                        {
                            bool shouldBeClosed = GameController.IngameState.IngameUi.Atlas.IsVisible ||
                                                  GameController.IngameState.IngameUi.AtlasTreePanel.IsVisible ||
                                                  GameController.IngameState.IngameUi.StashElement.IsVisible ||
                                                  GameController.IngameState.IngameUi.TradeWindow.IsVisible || 
                                                  GameController.IngameState.IngameUi.ChallengesPanel.IsVisible ||
                                                  GameController.IngameState.IngameUi.CraftBench.IsVisible ||
                                                  GameController.IngameState.IngameUi.DelveWindow.IsVisible ||
                                                  GameController.IngameState.IngameUi.ExpeditionWindow.IsVisible || 
                                                  GameController.IngameState.IngameUi.BanditDialog.IsVisible ||
                                                  GameController.IngameState.IngameUi.MetamorphWindow.IsVisible ||
                                                  GameController.IngameState.IngameUi.SyndicatePanel.IsVisible || 
                                                  GameController.IngameState.IngameUi.SyndicateTree.IsVisible ||
                                                  GameController.IngameState.IngameUi.QuestRewardWindow.IsVisible ||
                                                  GameController.IngameState.IngameUi.SynthesisWindow.IsVisible ||
                                                  //GameController.IngameState.IngameUi.UltimatumPanel.IsVisible || 
                                                  GameController.IngameState.IngameUi.MapDeviceWindow.IsVisible ||
                                                  GameController.IngameState.IngameUi.SellWindow.IsVisible ||
                                                  GameController.IngameState.IngameUi.SettingsPanel.IsVisible ||
                                                  GameController.IngameState.IngameUi.InventoryPanel.IsVisible || 
                                                  //GameController.IngameState.IngameUi.NpcDialog.IsVisible ||
                                                  GameController.IngameState.IngameUi.TreePanel.IsVisible;
                           
                            
                            if (!GameController.IngameState.IngameUi.Map.SmallMiniMap.IsVisibleLocal && shouldBeClosed)
                            {
                                /*
                                LogMessage("Atlas: " + GameController.IngameState.IngameUi.Atlas.IsVisible);
                                LogMessage("AtlasTreePanel: " + GameController.IngameState.IngameUi.AtlasTreePanel.IsVisible);
                                LogMessage("StashElement: " + GameController.IngameState.IngameUi.StashElement.IsVisible);
                                LogMessage("TradeWindow: " + GameController.IngameState.IngameUi.TradeWindow.IsVisible);
                                LogMessage("ChallengesPanel: " + GameController.IngameState.IngameUi.ChallengesPanel.IsVisible);
                                LogMessage("CraftBench: " + GameController.IngameState.IngameUi.CraftBench.IsVisible);
                                LogMessage("DelveWindow: " + GameController.IngameState.IngameUi.DelveWindow.IsVisible);
                                LogMessage("ExpeditionWindow: " + GameController.IngameState.IngameUi.ExpeditionWindow.IsVisible);
                                LogMessage("BanditDialog: " + GameController.IngameState.IngameUi.BanditDialog.IsVisible);
                                LogMessage("HarvestWindow: " + GameController.IngameState.IngameUi.HarvestWindow.IsVisible);
                                LogMessage("MetamorphWindow: " + GameController.IngameState.IngameUi.MetamorphWindow.IsVisible);
                                LogMessage("SyndicatePanel: " + GameController.IngameState.IngameUi.SyndicatePanel.IsVisible);
                                LogMessage("QuestRewardWindow: " + GameController.IngameState.IngameUi.QuestRewardWindow.IsVisible);
                                LogMessage("SynthesisWindow: " + GameController.IngameState.IngameUi.SynthesisWindow.IsVisible);
                                LogMessage("UltimatumPanel: " + GameController.IngameState.IngameUi.UltimatumPanel.IsVisible);
                                LogMessage("MapDeviceWindow: " + GameController.IngameState.IngameUi.MapDeviceWindow.IsVisible);
                                LogMessage("SellWindow: " + GameController.IngameState.IngameUi.SellWindow.IsVisible);
                                LogMessage("SettingsPanel: " + GameController.IngameState.IngameUi.SettingsPanel.IsVisible);
                                LogMessage("InventoryPanel: " + GameController.IngameState.IngameUi.InventoryPanel.IsVisible);
                                LogMessage("TreePanel: " + GameController.IngameState.IngameUi.TreePanel.IsVisible);
                                */
                                Keyboard.KeyPress(Keys.Tab);
                                SkillInfo.autoMapTabber.Cooldown = 250;
                            }
                            else if (GameController.IngameState.IngameUi.Map.SmallMiniMap.IsVisibleLocal && !shouldBeClosed)
                            {
                                //LogMessage("AN");
                                Keyboard.KeyPress(Keys.Tab);
                                SkillInfo.autoMapTabber.Cooldown = 250;
                            }
                        } 
                }
                catch (Exception e)
                {
                    LogError(e.ToString());
                }

                #endregion
                if (GameController.Area.CurrentArea.IsHideout || GameController.Area.CurrentArea.IsTown ||
                    /*GameController.IngameState.IngameUi.StashElement.IsVisible ||*/ // 3.15 Null
                    GameController.IngameState.IngameUi.NpcDialog.IsVisible ||
                    GameController.IngameState.IngameUi.SellWindow.IsVisible || MenuWindow.IsOpened ||
                    !GameController.InGame || GameController.IsLoading) return;
                
                enemys = GameController.EntityListWrapper.ValidEntitiesByType[EntityType.Monster].Where(x =>
                    x != null && x.IsAlive && x.IsHostile && x.GetComponent<Life>()?.CurHP > 0 && 
                    x.GetComponent<Targetable>()?.isTargetable == true && !HasStat(x, GameStat.CannotBeDamaged) &&
                    GameController.Window.GetWindowRectangleTimeCache.Contains(
                        GameController.Game.IngameState.Camera.WorldToScreen(x.Pos))).ToList();
                if (Settings.debugMode)
                {
                    Graphics.DrawText("Enemys: " + enemys.Count, new System.Numerics.Vector2(100, 120), Color.White);
                }
                
                if (Settings.offeringsEnabled || Settings.autoZombieEnabled || Settings.generalCryEnabled)
                    corpses = GameController.Entities.Where(x =>
                        x.IsValid && !x.IsHidden && x.IsHostile && x.IsDead && x.IsTargetable &&
                        x.HasComponent<Monster>()).ToList();


                #region Auto Quit

                if (Settings.autoQuitEnabled)
                    try
                    {
                        if (player.HPPercentage < (float)Settings.hppQuit / 100||
                            player.MaxES > 0 &&
                            player.ESPercentage < (float)Settings.espQuit / 100)
                            Quit();
                    }
                    catch (Exception e)
                    {
                        LogError(e.ToString());
                    }

                if (Settings.autoQuitGuardian)
                    try
                    {
                        if (Summons.GetAnimatedGuardianHpp() < (float)Settings.guardianHpp / 100)
                            Quit();
                    }
                    catch (Exception e)
                    {
                        LogError(e.ToString());
                    }

                #endregion

                

                // Do not Cast anything while we are untouchable or Chat is Open
                if (buffs.Exists(x => x.Name == "grace_period") ||
                    /*GameController.IngameState.IngameUi.ChatBoxRoot.Parent.Parent.Parent.GetChildAtIndex(3).IsVisible || */ // 3.15 Bugged 
                    !GameController.IsForeGroundCache)
                    return;
                
                foreach (var skill in skills.Where(skill => skill.IsOnSkillBar && skill.SkillSlotIndex >= 1 && skill.SkillSlotIndex != 2 && skill.CanBeUsed || SkillInfo.summonSkeletons.Id == skill.Id))
                {
                    #region CWDT

                    if (Settings.cwdtEnabled)
                    {
                        try
                        {
                            if (SkillInfo.summonSkeletons.Id == skill.Id && SkillInfo.ManageCooldown(SkillInfo.wardFlask))
                            {
                                // Auto Ward Flask
                                var wardFlask = GameController.Game.IngameState.ServerData.PlayerInventories.FirstOrDefault(x => x.Inventory.InventType == InventoryTypeE.Flask)?.Inventory?.InventorySlotItems?.FirstOrDefault(x => x.InventoryPosition.X == 0)?.Item;
                                if (wardFlask != null && !buffs.Exists(x => x.Name == "flask_bonus_ward_not_break") &&
                                    wardFlask.GetComponent<Charges>()?.NumCharges >= wardFlask.GetComponent<Charges>()?.ChargesMax)
                                {
                                    Keyboard.KeyPress(Keys.D1);
                                    SkillInfo.wardFlask.Cooldown = 100;
                                }
                                // Auto Loop Start
                                if (buffs.Exists(x => x.Name == "flask_bonus_ward_not_break"))
                                {
                                    if (!skill.IsOnCooldown)
                                    {
                                        cwdtCounter++;
                                        if (cwdtCounter >= 3)
                                        {
                                            Keyboard.KeyPress(Keys.X);
                                            SkillInfo.wardFlask.Cooldown = 160;
                                            cwdtCounter = 0;
                                        }
                                    }
                                    else if (skill.IsOnCooldown)
                                        cwdtCounter = 0;
                                    if ((bool) GameController.Game.IngameState.ServerData.PlayerInventories
                                                 .FirstOrDefault(x => x.Inventory.InventType == InventoryTypeE.Weapon)
                                                 ?.Inventory.Items.FirstOrDefault()?.Metadata.Contains("Wand"))
                                    {
                                        Keyboard.KeyPress(Keys.X);
                                        SkillInfo.wardFlask.Cooldown = 500;
                                    }
                                }
                            }

                            if (SkillInfo.righteousFire.Id == skill.Id && SkillInfo.ManageCooldown(SkillInfo.righteousFire, skill))
                            {
                                if (buffs.Exists(x => x.Name == "flask_bonus_ward_not_break") && !buffs.Exists(x => x.Name == SkillInfo.righteousFire.BuffName))
                                {
                                    Keyboard.KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                    SkillInfo.righteousFire.Cooldown = 500;
                                }
                            }
                            // TODO
                            // Add something like Auto Aura later on, keeping in CWDT for now
                            if (SkillInfo.auraPurityOfElements.Id == skill.Id && SkillInfo.ManageCooldown(SkillInfo.auraPurityOfElements, skill))
                            {
                                if (!buffs.Exists(x => x.Name == SkillInfo.auraPurityOfElements.BuffName))
                                {
                                    Keyboard.KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                    SkillInfo.auraPurityOfElements.Cooldown = 500;
                                }
                            }
                            if (SkillInfo.auraHatred.Id == skill.Id && SkillInfo.ManageCooldown(SkillInfo.auraHatred, skill) && SkillInfo.auraHatred.IsBlessing == 0)
                            {
                                if (!buffs.Exists(x => x.Name == SkillInfo.auraHatred.BuffName))
                                {
                                    Keyboard.KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                    SkillInfo.auraHatred.Cooldown = 500;
                                }
                            }
                            if (SkillInfo.auraZealotry.Id == skill.Id && SkillInfo.ManageCooldown(SkillInfo.auraZealotry, skill) && SkillInfo.auraZealotry.IsBlessing == 0)
                            {
                                if (!buffs.Exists(x => x.Name == SkillInfo.auraZealotry.BuffName))
                                {
                                    Keyboard.KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                    SkillInfo.auraZealotry.Cooldown = 500;
                                }
                            }
                            
                        }
                        catch (Exception e)
                        {
                            LogError(e.ToString());
                        }
                    }

                    #endregion
                    
                    #region Ranged Trigger -> Mirage Archer / Frenzy

                    if (Settings.rangedTriggerEnabled)
                        try
                        {
                            skill.Stats.TryGetValue(GameStat.SkillCanOwnMirageArchers, out var mirage);
                            if (!localPlayer.Stats.TryGetValue(GameStat.VirtualMaximumPowerCharges,
                                out var maxPowerCharges))
                                maxPowerCharges = 3;
                            if (!localPlayer.Stats.TryGetValue(GameStat.VirtualMaximumFrenzyCharges,
                                out var maxFrenzyCharges))
                                maxFrenzyCharges = 3;

                            if (!skill.IsOnCooldown &&
                                CountEnemysAroundMouse(Settings.rangedTriggerMouseRange.Value) >= 1 &&
                                MonsterCheck(Settings.rangedTriggerTargetRange, 1, 0, 0))
                            {
                                if (mirage >= 1 &&
                                    !buffs.Exists(x => x.Name == "mirage_archer_visual_buff" && x.Timer > 0.5))
                                {
                                    Keyboard.KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                }
                                else if (skill.Id == SkillInfo.frenzy.Id && SkillInfo.ManageCooldown(SkillInfo.frenzy, skill) &&
                                         (!Settings.rangedTriggerPowerCharge && !buffs.Exists(x =>
                                              x.Name == "frenzy_charge" && x.Timer > 3 && x.BuffCharges == maxFrenzyCharges) ||
                                          Settings.rangedTriggerPowerCharge && !buffs.Exists(x =>
                                              x.Name == "power_charge" && x.Timer > 3 && x.BuffCharges == maxPowerCharges)))

                                {
                                    Keyboard.KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                    SkillInfo.frenzy.Cooldown = Settings.rangedTriggerCooldown;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            LogError(e.ToString());
                        }

                    #endregion

                    #region Enduring Cry

                    if (Settings.enduringCryEnabled)
                        try
                        {
                            if (skill.Id == SkillInfo.enduringCry.Id)
                                if (SkillInfo.ManageCooldown(SkillInfo.enduringCry, skill))
                                    if (MonsterCheck(Settings.enduringCryTriggerRange, Settings.enduringCryMinAny,
                                            Settings.enduringCryMinRare, Settings.enduringCryMinUnique) && 
                                        (player.HPPercentage < (float) Settings.enduringCryHealHpp / 100 ||
                                        player.ESPercentage < (float) Settings.enduringCryHealEsp / 100)
                                        || Settings.enduringCrySpam)
                                        Keyboard.KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                        }
                        catch (Exception e)
                        {
                            LogError(e.ToString());
                        }

                    #endregion

                    #region General's Cry

                    if (Settings.generalCryEnabled)
                        try
                        {
                            if (skill.Id == SkillInfo.generalCry.Id)
                                if (SkillInfo.ManageCooldown(SkillInfo.generalCry, skill))
                                    if (GetCorpseWithin(Settings.generalCryCorpseTriggerRange) >=
                                        Settings.generalCryMinCorpse && MonsterCheck(Settings.generalCryTriggerRange, 1,0,0))
                                        Keyboard.KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                        }
                        catch (Exception e)
                        {
                            LogError(e.ToString());
                        }

                    #endregion

                    #region Phase Run / WitherStep

                    if (Settings.phaserunEnabled)
                        try
                        {
                            if (skill.Id == SkillInfo.witherStep.Id)
                                if (SkillInfo.ManageCooldown(SkillInfo.witherStep, skill))
                                    if (!isAttacking && isMoving)
                                    {
                                        Keyboard.KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                        SkillInfo.phaserun.Cooldown = 250;
                                    }

                            if (skill.Id == SkillInfo.phaserun.Id)
                                if (SkillInfo.ManageCooldown(SkillInfo.phaserun, skill))
                                {
                                    if (!Settings.phaserunUseLifeTap && !isAttacking && isMoving &&
                                        !buffs.Exists(b => b.Name == SkillInfo.witherStep.BuffName) &&
                                        !buffs.Exists(b =>
                                            b.Name == SkillInfo.phaserun.BuffName && b.Timer > 0.1))
                                        Keyboard.KeyPress(GetSkillInputKey(skill.SkillSlotIndex));

                                    if (Settings.phaserunUseLifeTap && isMoving &&
                                        (!buffs.Exists(b => b.Name == "lifetap_buff" && b.Timer > 0.1) &&
                                         MonsterCheck(1000, 1, 0, 0) ||
                                         !buffs.Exists(b => b.Name == SkillInfo.witherStep.BuffName) &&
                                         !buffs.Exists(b =>
                                             b.Name == SkillInfo.phaserun.BuffName && b.Timer > 0.1)))
                                        Keyboard.KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                }
                        }
                        catch (Exception e)
                        {
                            LogError(e.ToString());
                        }

                    #endregion

                    #region Molten Shell / Steelskin / Bone Armour / Arcane Cloak

                    if (Settings.moltenShellEnabled)
                        try
                        {
                            if (skill.Id == SkillInfo.moltenShell.Id || skill.Id == SkillInfo.steelSkin.Id ||
                                skill.Id == SkillInfo.boneArmour.Id || skill.Id == SkillInfo.arcaneCloak.Id)
                                if (SkillInfo.ManageCooldown(SkillInfo.moltenShell, skill))
                                    if (MonsterCheck(Settings.moltenShellRange, Settings.moltenShellMinAny,
                                            Settings.moltenShellMinRare, Settings.moltenShellMinUnique) &&
                                        (player.HPPercentage <=
                                         (float)Settings.moltenShellHpp / 100 ||
                                         player.MaxES > 0 && player.ESPercentage <
                                         (float)Settings.moltenShellEsp / 100))
                                        Keyboard.KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                        }
                        catch (Exception e)
                        {
                            LogError(e.ToString());
                        }

                    #endregion

                    #region Aura Blessing

                    if (Settings.auraBlessingEnabled)
                        try
                        {
                            if (SkillInfo.ManageCooldown(SkillInfo.blessing, skill))
                            {
                                //guard statement to check for withering step
                                if (Settings.auraBlessingWitheringStep && buffs.Exists(b => b.Name == SkillInfo.witherStep.BuffName)) return;
                                var cachedSkill = SkillInfo.CachedAuraSkills.Find(s => s.IsBlessing > 0 && s.Id == skill.Id);
                                if (cachedSkill != null && !buffs.Exists(x => x.Name == cachedSkill.BuffName && x.Timer > 0.2))
                                    if (MonsterCheck(Settings.auraBlessingRange, Settings.auraBlessingMinAny,
                                            Settings.auraBlessingMinRare, Settings.auraBlessingMinUnique) &&
                                        (player.HPPercentage <=
                                         (float)Settings.auraBlessingHpp / 100 ||
                                         player.MaxES > 0 && player.ESPercentage <
                                         (float)Settings.auraBlessingEsp / 100))
                                            Keyboard.KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                            }
                        }
                        catch (Exception e)
                        {
                            LogError(e.ToString());
                        }

                    #endregion

                    #region Berseker

                    if (Settings.berserkEnabled)
                        try
                        {
                            if (skill.Id == SkillInfo.berserk.Id)
                                if (SkillInfo.ManageCooldown(SkillInfo.berserk, skill))
                                {
                                    skill.Stats.TryGetValue(GameStat.BerserkMinimumRage, out var minRage);
                                    if (buffs.Exists(x =>
                                            x.Name == "rage" && x.BuffCharges >= minRage &&
                                            x.BuffCharges >= Settings.berserkMinRage) &&
                                        MonsterCheck(Settings.berserkRange, Settings.berserkMinAny,
                                            Settings.berserkMinRare, Settings.berserkMinUnique))
                                    {
                                        Keyboard.KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                        SkillInfo.berserk.Cooldown = 100;
                                    }
                                }
                        }
                        catch (Exception e)
                        {
                            LogError(e.ToString());
                        }

                    #endregion

                    #region Blood Rage

                    if (Settings.bloodRageEnabled)
                        try
                        {
                            if (skill.Id == SkillInfo.bloodRage.Id)
                                if (SkillInfo.ManageCooldown(SkillInfo.bloodRage, skill))
                                    if (!buffs.Exists(b =>
                                            b.Name == SkillInfo.bloodRage.BuffName && b.Timer > 1.0) &&
                                        MonsterCheck(Settings.bloodRageRange, Settings.bloodRageMinAny,
                                            Settings.bloodRageMinRare, Settings.bloodRageMinUnique))
                                    {
                                        Keyboard.KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                        SkillInfo.bloodRage.Cooldown = 100;
                                    }
                        }
                        catch (Exception e)
                        {
                            LogError(e.ToString());
                        }

                    #endregion

                    #region Auto Summon

                    if (Settings.autoSummonEnabled)
                        try
                        {
                            if (SkillInfo.ManageCooldown(SkillInfo.autoSummon) && !isCasting && !isAttacking &&
                                GetMonsterWithin(Settings.autoGolemAvoidRange) == 0)
                            {
                                if (Settings.autoGolemEnabled)
                                {
                                    if (skill.Id == SkillInfo.chaosGolem.Id && skill.DeployedObjects.Count < Settings.autoGolemChaosMax)
                                    {
                                        Keyboard.KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                        SkillInfo.autoSummon.Cooldown = 2000;
                                    }
                                    else if (skill.Id == SkillInfo.flameGolem.Id && skill.DeployedObjects.Count < Settings.autoGolemFireMax)
                                    {
                                        Keyboard.KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                        SkillInfo.autoSummon.Cooldown = 2000;
                                    }
                                    else if (skill.Id == SkillInfo.iceGolem.Id && skill.DeployedObjects.Count < Settings.autoGolemIceMax)
                                    {
                                        Keyboard.KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                        SkillInfo.autoSummon.Cooldown = 2000;
                                    }
                                    else if (skill.Id == SkillInfo.lightningGolem.Id && skill.DeployedObjects.Count < Settings.autoGolemLightningMax)
                                    {
                                        Keyboard.KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                        SkillInfo.autoSummon.Cooldown = 2000;
                                    }
                                    else if (skill.Id == SkillInfo.stoneGolem.Id && skill.DeployedObjects.Count < Settings.autoGolemRockMax)
                                    {
                                        Keyboard.KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                        SkillInfo.autoSummon.Cooldown = 2000;
                                    }
                                    else if (skill.Id == SkillInfo.carrionGolem.Id && skill.DeployedObjects.Count < Settings.autoBoneMax)
                                    {
                                        Keyboard.KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                        SkillInfo.autoSummon.Cooldown = 2000;
                                    }
                                    else if (skill.Id == SkillInfo.ursaGolem.Id && skill.DeployedObjects.Count < Settings.autoGolemDropBearMax)
                                    {
                                        Keyboard.KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                        SkillInfo.autoSummon.Cooldown = 2000;
                                    }
                                }

                                if (Settings.autoZombieEnabled && skill.Id == SkillInfo.raiseZombie.Id)
                                {
                                    if (!skill.Stats.TryGetValue(GameStat.NumberOfZombiesAllowed, out var maxZombies))
                                        maxZombies = 3;
                                    if (skill.DeployedObjects.Count < maxZombies &&
                                        CountCorpsesAroundMouse(MouseAutoSnapRange) > 0)
                                    {
                                        Keyboard.KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                        SkillInfo.autoSummon.Cooldown = 1200;
                                    }
                                }

                                if (Settings.autoHolyRelictEnabled && skill.Id == SkillInfo.holyRelict.Id)
                                {
                                    // Seems maxRelicts is no longer available when its default value
                                    if (!skill.Stats.TryGetValue(GameStat.NumberOfRelicsAllowed, out var maxRelicts))
                                        maxRelicts = 1;
                                    
                                    if (skill.DeployedObjects.Count < maxRelicts)
                                    {
                                        Keyboard.KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                        SkillInfo.autoSummon.Cooldown = 2000;
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            LogError(e.ToString());
                        }

                    #endregion

                    #region Auto Toxic Rain Ballista

                    if (Settings.autoToxicRainBallistaEnabled)
                        try
                        {
                            if (skill.Id == SkillInfo.toxicRain.Id)
                                if (SkillInfo.ManageCooldown(SkillInfo.toxicRain, skill) && !isCasting && !isAttacking)
                                    if (MonsterCheck(Settings.autoToxicRainBallistaRange, Settings.autoToxicRainBallistaMinAny,
                                            Settings.autoToxicRainBallistaMinRare, Settings.autoToxicRainBallistaUnique) &&
                                            skill.DeployedObjects.Count < Settings.autoToxicRainBallistaMax)
                                    {
                                        Keyboard.KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                        SkillInfo.toxicRain.Cooldown = 100;
                                    }
                        }
                        catch (Exception e)
                        {
                            LogError(e.ToString());
                        }

                    #endregion

                    #region Vortex

                    if (Settings.vortexEnabled)
                        try
                        {
                            if (skill.Id == SkillInfo.vortex.Id)
                                if (SkillInfo.ManageCooldown(SkillInfo.vortex, skill))
                                    if (MonsterCheck(Settings.vortexRange, Settings.vortexMinAny,
                                            Settings.vortexMinRare, Settings.vortexMinUnique) ||
                                        Settings.vortexFrostbolt && skills.Any(x =>
                                            x.Id == SkillInfo.frostbolt.Id && x.SkillUseStage > 2))
                                        Keyboard.KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                        }
                        catch (Exception e)
                        {
                            LogError(e.ToString());
                        }

                    #endregion

                    #region Divine Ire

                    if (Settings.divineIreEnabled)
                        try
                        {
                            if ((DateTime.Now - lastStackSkill).TotalMilliseconds > 250 &&
                                (skill.Id == SkillInfo.divineIre.Id || skill.Id == SkillInfo.scourgeArror.Id ||
                                 skill.Id == SkillInfo.bladeFlurry.Id))
                                if (buffs.Exists(b =>
                                    b.Name == SkillInfo.divineIre.BuffName &&
                                    b.BuffCharges >= Settings.divineIreStacks.Value ||
                                    b.Name == SkillInfo.bladeFlurry.BuffName &&
                                    b.BuffCharges >= Settings.divineIreStacks ||
                                    b.Name == SkillInfo.scourgeArror.BuffName &&
                                    b.BuffCharges >= Settings.divineIreStacks))
                                {
                                    if (Settings.divineIreWaitForInfused)
                                        // Get delay here at some point ?
                                        if (!buffs.Exists(x =>
                                            x.Name == "storm_barrier_support_damage" && x.Timer > 1.0))
                                            return;

                                    Keyboard.KeyUp(GetSkillInputKey(skill.SkillSlotIndex));
                                    lastStackSkill = DateTime.Now;
                                    if (Settings.debugMode)
                                        LogError("Release Key Pressed: " +
                                                 GetSkillInputKey(skill.SkillSlotIndex));
                                }
                        }
                        catch (Exception e)
                        {
                            LogError(e.ToString());
                        }

                    #endregion

                    #region Doedre Effigy

                    if (Settings.doedreEffigyEnabled)
                        try
                        {
                            if (skill.Id == SkillInfo.doedreEffigy.Id)
                                if (SkillInfo.ManageCooldown(SkillInfo.doedreEffigy, skill))
                                    if (CountEnemysAroundMouse(350) > 0)
                                        Keyboard.KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                        }
                        catch (Exception e)
                        {
                            LogError(e.ToString());
                        }

                    #endregion

                    #region Offerings

                    if (Settings.offeringsEnabled)
                        try
                        {
                            if ((!Settings.offeringsUseWhileCasting && !isCasting && !isAttacking ||
                                 Settings.offeringsUseWhileCasting) &&
                                (skill.Id == SkillInfo.spiritOffering.Id || skill.Id == SkillInfo.boneOffering.Id ||
                                 skill.Id == SkillInfo.fleshOffering.Id))
                                if (SkillInfo.ManageCooldown(SkillInfo.spiritOffering, skill))
                                    if (MonsterCheck(Settings.offeringsTriggerRange, Settings.offeringsMinAny,Settings.offeringsMinRare, Settings.offeringsMinUnique) 
                                        &&!buffs.Exists(x => x.Name == "active_offering") 
                                        && CountCorpsesAroundMouse(MouseAutoSnapRange) > 0)
                                        Keyboard.KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                        }
                        catch (Exception e)
                        {
                            LogError(e.ToString());
                        }

                    #endregion

                    #region Any Vaal Skill

                    if (Settings.anyVaalEnabled)
                        try
                        {
                            if (SkillInfo.ManageCooldown(SkillInfo.vaalSkill, skill))
                                if (MonsterCheck(Settings.anyVaalTriggerRange, Settings.anyVaalMinAny,
                                    Settings.anyVaalMinRare, Settings.anyVaalMinUnique) 
                                    && vaalSkills.Exists(x => x.VaalSkillInternalName == skill.InternalName && x.CurrVaalSouls >= x.VaalMaxSouls))
                                    if (player.HPPercentage<= (float)Settings.anyVaalHpp / 100 ||
                                        player.MaxES > 0 && player.ESPercentage < (float)Settings.anyVaalEsp / 100 || 
                                        player.MPPercentage < (float)Settings.anyVaalMpp / 100)
                                        Keyboard.KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                        }
                        catch (Exception e)
                        {
                            LogError(e.ToString());
                        }

                    #endregion

                    #region Brand Recall

                    if (Settings.brandRecallEnabled)
                        try
                        {
                            if (skill.Id == SkillInfo.brandRecall.Id)
                                if (SkillInfo.ManageCooldown(SkillInfo.brandRecall, skill))
                                    if (GetMonsterWithin(Settings.brandRecallTriggerRange) >=
                                        Settings.brandRecallMinEnemys)
                                        Keyboard.KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                            // Once a Brand Skill is linked with Archemage for example, it will show incorrect stats for 1 frame IsUsing turns true, even when in down, SkillUseStage 3 etc.

                            //ActorSkill stormBrand = skills.Find(x => x.InternalName == "storm_brand");
                            //ActorSkill armageddonBrand = skills.Find(x => x.InternalName == "armageddon_brand");
                            //int activeBrands = 0;
                            //if (stormBrand != null )
                            //{
                            //    activeBrands = stormBrand.SkillUseStage - 1;
                            //}
                            //if (armageddonBrand != null && armageddonBrand.SkillUseStage - 1 > activeBrands)
                            //{
                            //    activeBrands = armageddonBrand.SkillUseStage - 1;
                            //}
                            //LogError("Brand Active: " + activeBrands.ToString());
                            //if (activeBrands >= Settings.brandRecallMinBrands)
                            //{

                            //}                                    
                        }
                        catch (Exception e)
                        {
                            LogError(e.ToString());
                        }

                    #endregion

                    #region Tempest Shield

                    if (Settings.tempestShieldEnabled)
                        try
                        {
                            if (skill.Id == SkillInfo.tempestShield.Id)
                                if (SkillInfo.ManageCooldown(SkillInfo.tempestShield, skill))
                                    if (!Settings.tempestShieldUseWhileCasting && !isCasting && !isAttacking ||
                                        Settings.tempestShieldUseWhileCasting)
                                        if (!buffs.Exists(x =>
                                                x.Name == SkillInfo.tempestShield.BuffName && x.Timer > 1.0) &&
                                            MonsterCheck(Settings.tempestShieldTriggerRange,
                                                Settings.tempestShieldMinAny, Settings.tempestShieldMinRare,
                                                Settings.tempestShieldMinUnique))
                                            Keyboard.KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                        }
                        catch (Exception e)
                        {
                            LogError(e.ToString());
                        }

                    #endregion

                    #region AutoAttack Cyclone / Nova /

                    if (Settings.autoAttackEnabled)
                        try
                        {
                            if ((DateTime.Now - autoAttackUpdate).TotalMilliseconds > 50 &&
                                (skill.Id == SkillInfo.cyclone.Id || skill.Id == SkillInfo.iceNova.Id ||
                                 skill.Id == SkillInfo.flickerStrike.Id || skill.Id == SkillInfo.sweep.Id))
                            {
                                autoAttackUpdate = DateTime.Now;
                                if (Keyboard.IsKeyDown((int)Settings.inputKeyPickIt.Value) &&
                                    Keyboard.IsKeyDown((int)GetSkillInputKey(skill.SkillSlotIndex)) ||
                                    Keyboard.IsKeyDown((int)GetSkillInputKey(skill.SkillSlotIndex)) &&
                                    !isAttacking &&
                                    autoAttackRunning > DateTime.MinValue &&
                                    (DateTime.Now - autoAttackRunning).TotalMilliseconds > 50)
                                {
                                    Keyboard.KeyUp(GetSkillInputKey(skill.SkillSlotIndex));
                                    if (Settings.debugMode.Value)
                                        LogMessage(
                                            "Copilot: Detected Key Priority Problem due to User Input, fixing.");
                                    break;
                                }

                                if ((Settings.autoAttackLeftMouseCheck.Value && !Mouse.IsMouseLeftPressed() ||
                                     !Settings.autoAttackLeftMouseCheck.Value)
                                    && (!Settings.autoAttackCurseCheck &&
                                        GetMonsterWithin(Settings.autoAttackRange) >= 1 ||
                                        Settings.autoAttackCurseCheck && enemys.Any(x =>
                                            x.Buffs.Exists(b =>
                                                b.Name.Contains("curse") || b.Name == "raider_exposure_aura"))))
                                {
                                    if (!Keyboard.IsKeyDown((int)GetSkillInputKey(skill.SkillSlotIndex)) &&
                                        !Keyboard.IsKeyDown((int)Settings.inputKeyPickIt.Value))
                                    {
                                        Keyboard.KeyDown(GetSkillInputKey(skill.SkillSlotIndex));
                                        autoAttackRunning = DateTime.Now;
                                    }
                                }
                                else if (Keyboard.IsKeyDown((int)GetSkillInputKey(skill.SkillSlotIndex)))
                                {
                                    Keyboard.KeyUp(GetSkillInputKey(skill.SkillSlotIndex));
                                    autoAttackRunning = DateTime.MinValue;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            LogError(e.ToString());
                        }

                    #endregion

                    #region Convocation

                    if (Settings.convocationEnabled)
                        try
                        {
                            if (skill.Id == SkillInfo.convocation.Id)
                                if (SkillInfo.ManageCooldown(SkillInfo.convocation, skill))
                                {
                                    if (GetMonsterWithin(Settings.convocationAvoidUniqueRange,
                                        MonsterRarity.Unique) > 0)
                                        return;
                                    if (Summons.GetLowestMinionHpp() <
                                        (float)Settings.convocationHpp / 100)
                                        Keyboard.KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                    else if (GetMonsterWithin(Settings.convocationMobRange) > 0 &&
                                             GetMinnionsWithin(Settings.convocationMinnionRange) /
                                             localPlayer.GetComponent<Actor>().DeployedObjects
                                                 .Count(x => x?.Entity != null && x.Entity.IsAlive) *
                                             100 <= Settings.convocationMinnionPct)
                                        Keyboard.KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                }
                        }
                        catch (Exception e)
                        {
                            LogError(e.ToString());
                        }

                    #endregion

                    #region Auto Curse

                    if (Settings.autoCurseEnabled)
                        try
                        {
                            if (skill.Id == SkillInfo.punishment.Id &&
                                (DateTime.Now - lastCurse).TotalMilliseconds > Settings.autoCurseCooldown)
                                if (CountNonCursedEnemysAroundMouse(Settings.autoCurseRange) >=
                                    Settings.autoCurseMinEnemys)
                                {
                                    lastCurse = DateTime.Now;
                                    Keyboard.KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                }
                        }
                        catch (Exception e)
                        {
                            LogError(e.ToString());
                        }

                    #endregion

                    #region Blade Vortex

                    if (Settings.bladeVortex)
                        try
                        {
                            if (skill.Id == SkillInfo.bladeVortex.Id)
                            {
                                skill.Stats.TryGetValue(GameStat.VirtualSupportAnticipationChargeGainIntervalMs,
                                    out var unleashCooldown);
                                skill.Stats.TryGetValue(GameStat.SkillMaxUnleashSeals,
                                    out var unleashMaxSeals);
                                if (SkillInfo.ManageCooldown(SkillInfo.bladeVortex, skill))
                                    if (GetMonsterWithin(Settings.bladeVortexRange) > 0 && !buffs.Exists(x =>
                                        x.Name == "blade_vortex_counter" && x.BuffCharges >= Settings.bladeVortexCount))
                                    {
                                        if (Settings.debugMode)
                                        {
                                            LogError("Blade Vortex Unleash Cooldown: " + unleashCooldown);
                                            LogError("Blade Vortex Unleash Max Seals: " + unleashMaxSeals);
                                        }
                                        Keyboard.KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                        SkillInfo.bladeVortex.Cooldown = unleashCooldown > 0 && unleashMaxSeals > 0
                                            ? unleashCooldown * unleashMaxSeals
                                            : 0;
                                    }
                            }
                        }
                        catch (Exception e)
                        {
                            LogError(e.ToString());
                        }

                    #endregion

                    #region Blade Blast

                    if (Settings.bladeBlast)
                        try
                        {
                            if (skill.Id == SkillInfo.bladeBlast.Id)
                            {
                                UpdateBladeBlast();
                                if (!skill.IsOnCooldown && SkillInfo.ManageCooldown(SkillInfo.bladeBlast, skill))
                                    if (!isCasting && !isAttacking &&
                                        (Settings.bladeBlastFastMode && bladeBlastReady ||
                                         !Settings.bladeBlastFastMode &&
                                         CountBladeBlastEnitytiesNearMouse(Settings.bladeBlastEntityRange) > 0))
                                    {
                                        Keyboard.KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                        bladeBlastReady = false;
                                    }
                            }
                        }
                        catch (Exception e)
                        {
                            LogError(e.ToString());
                        }
                    #endregion

                    #region Plague Bearer
                    
                    if (Settings.plagueBearer)
                        try
                        {
                            if (skill.Id == SkillInfo.plagueBearer.Id)
                            {
                                if (SkillInfo.ManageCooldown(SkillInfo.plagueBearer, skill) && GetMonsterWithin(Settings.plagueBearerRange) >= Settings.plagueBearerMinEnemys && buffs.Exists(x => x.Name == "corrosive_shroud_at_max_damage"))
                                {
                                    Keyboard.KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            LogError(e.ToString());
                        }

                    #endregion

                    /*
                    #region Spider

                    if (false)
                    {
                        if (skill.Id == SkillInfo.summonSpiders.Id && SkillInfo.ManageCooldown(SkillInfo.summonSpiders, skill))
                        {
                            var spidersSummoned = buffs.Count(x => x.Name == SkillInfo.summonSpiders.BuffName);
                            
                            if (spidersSummoned < 20 && GetCorpseWithin(30) >= 2)
                            {
                                
                            } 
                        }
                    }
                    

                    #endregion
                    */
                    #region Detonate Mines ( to be done )
/*
                    if (Settings.minesEnabled)
                    {
                        try
                        {
                            var remoteMines = localPlayer.GetComponent<Actor>().DeployedObjects.Where(x =>
                                    x.Entity != null && x.Entity.Path == "Metadata/MiscellaneousObjects/RemoteMine")
                                .ToList();

                            // Removed Logic
                            // What should a proper Detonator do and when ?
                            // Detonate Mines when they have the chance to hit a target (Range), include min. mines ?
                            // Internal delay 500-1000ms ?
                        }
                        catch (Exception e)
                        {
                            LogError(e.ToString());
                        }
                    }
                    */
                    #endregion
                }

                #region Delve Flare

                if (Settings.delveFlareEnabled)
                    try
                    {
                        if ((DateTime.Now - lastDelveFlare).TotalMilliseconds > 1000 &&
                            (player.ESPercentage < (float)Settings.delveFlareEspBelow / 100 ||
                             player.HPPercentage < (float)Settings.delveFlareHppBelow / 100) && buffs.Exists(x =>
                                x.Name == "delve_degen_buff" && x.BuffCharges >= Settings.delveFlareDebuffStacks))
                        {
                            Keyboard.KeyPress(Settings.delveFlareKey.Value);
                            lastDelveFlare = DateTime.Now;
                        }
                    }
                    catch (Exception e)
                    {
                        LogError(e.ToString());
                    }

                #endregion

                #region Custom Skill

                if (Settings.customEnabled)
                    try
                    {
                        if (Gcd() &&
                            (DateTime.Now - lastCustom).TotalMilliseconds > Settings.customCooldown.Value &&
                            MonsterCheck(Settings.customTriggerRange, Settings.customMinAny, Settings.customMinRare,
                                Settings.customMinUnique))
                            if (player.HPPercentage <= (float)Settings.customHpp / 100 ||
                                player.MaxES > 0 && player.ESPercentage <
                                (float)Settings.customEsp / 100)
                            {
                                Keyboard.KeyPress(Settings.customKey);
                                lastCustom = DateTime.Now;
                            }
                    }
                    catch (Exception e)
                    {
                        LogError(e.ToString());
                    }
                #endregion
            }
            catch (Exception e)
            {LogError(e.ToString());}
        }

        // Taken from ->
        // https://www.reddit.com/r/pathofexiledev/comments/787yq7/c_logout_app_same_method_as_lutbot/
        
        // Wont work when Private, No Touchy Touchy !!!
        // ReSharper disable once MemberCanBePrivate.Global
        [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
        public static partial class CommandHandler
        {
            public static void KillTcpConnectionForProcess(int processId)
            {
                MibTcprowOwnerPid[] table;
                const int afInet = 2;
                var buffSize = 0;
                GetExtendedTcpTable(IntPtr.Zero, ref buffSize, true, afInet, TableClass.TcpTableOwnerPidAll);
                var buffTable = Marshal.AllocHGlobal(buffSize);
                try
                {
                    var ret = GetExtendedTcpTable(buffTable, ref buffSize, true, afInet, TableClass.TcpTableOwnerPidAll);
                    if (ret != 0)
                        return;
                    var tab = (MibTcptableOwnerPid)Marshal.PtrToStructure(buffTable, typeof(MibTcptableOwnerPid));
                    var rowPtr = (IntPtr)((long)buffTable + Marshal.SizeOf(tab.dwNumEntries));
                    table = new MibTcprowOwnerPid[tab.dwNumEntries];
                    for (var i = 0; i < tab.dwNumEntries; i++)
                    {
                        var tcpRow = (MibTcprowOwnerPid)Marshal.PtrToStructure(rowPtr, typeof(MibTcprowOwnerPid));
                        table[i] = tcpRow;
                        rowPtr = (IntPtr)((long)rowPtr + Marshal.SizeOf(tcpRow));

                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(buffTable);
                }

                //Kill Path Connection
                var pathConnection = table.FirstOrDefault(t => t.owningPid == processId);
                pathConnection.state = 12;
                var ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(pathConnection));
                Marshal.StructureToPtr(pathConnection, ptr, false);
                SetTcpEntry(ptr);


            }

            [DllImport("iphlpapi.dll", SetLastError = true)]
            private static extern uint GetExtendedTcpTable(IntPtr pTcpTable, ref int dwOutBufLen, bool sort, int ipVersion, TableClass tblClass, uint reserved = 0);

            [DllImport("iphlpapi.dll")]
            private static extern int SetTcpEntry(IntPtr pTcprow);

            [StructLayout(LayoutKind.Sequential)]
            [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
            public struct MibTcprowOwnerPid
            {
                public uint state;
                public uint localAddr;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public byte[] localPort;
                public uint remoteAddr;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public byte[] remotePort;
                public uint owningPid;

            }

            [StructLayout(LayoutKind.Sequential)]
            public struct MibTcptableOwnerPid
            {
                public uint dwNumEntries;
                private readonly MibTcprowOwnerPid table;
            }

            private enum TableClass
            {
                TcpTableBasicListener,
                TcpTableBasicConnections,
                TcpTableBasicAll,
                TcpTableOwnerPidListener,
                TcpTableOwnerPidConnections,
                TcpTableOwnerPidAll,
                TcpTableOwnerModuleListener,
                TcpTableOwnerModuleConnections,
                TcpTableOwnerModuleAll
            }
        }
    }
}