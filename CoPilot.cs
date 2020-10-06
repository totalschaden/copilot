using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using ExileCore;
using ExileCore.PoEMemory;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using ExileCore.Shared.Static;
using SharpDX;
using static CoPilot.WinApiMouse;
using Point = CoPilot.WinApiMouse.Point;
using System.Windows.Input;
using ImGuiNET;

namespace CoPilot
{
    public class CoPilot : BaseSettingsPlugin<CoPilotSettings>
    {
        internal Entity localPlayer;
        internal Life player;
        internal List<Buff> buffs;
        internal bool isAttacking;
        internal bool isCasting;
        internal bool isMoving;
        internal List<ActorSkill> skills = new List<ActorSkill>();
        internal List<ActorVaalSkill> vaalSkills = new List<ActorVaalSkill>();

        private readonly int mouseAutoSnapRange = 250;
        private DateTime lastPhaserun = new DateTime();
        private DateTime lastMoltenShell = new DateTime();
        private DateTime lastWarCry = new DateTime();
        private DateTime lastTimeAny = new DateTime();
        private DateTime lastDelveFlare = new DateTime();
        private DateTime lastVortex = new DateTime();
        private DateTime lastBloodRage = new DateTime();
        private DateTime lastdoedreEffigy = new DateTime();
        private DateTime lastStackSkill = new DateTime();
        private DateTime lastCustom = new DateTime();
        private DateTime lastOfferings = new DateTime();
        private DateTime lastAutoGolem = new DateTime();
        private DateTime lastBrandRecall = new DateTime();
        private DateTime lastTempestShield = new DateTime();
        private DateTime lastMirage = new DateTime();
        private DateTime lastConvocation = new DateTime();
        private DateTime lastCurse = new DateTime();
        private readonly int delay = 70;
        private IEnumerable<Entity> enemys;
        private IEnumerable<Entity> corpses;
        Vector3 playerPosition;

        private DateTime autoAttackRunning = new DateTime();
        private DateTime autoAttackUpdate = new DateTime();
        private Summons summons = new Summons();

        private Coroutine CoroutineWorker;
        Coroutine skillCoroutine;
        private const string coroutineKeyPress = "KeyPress";
        internal static CoPilot instance;

        private void KeyPress(Keys key, bool anyDelay = true)
        {
            if (anyDelay)
                lastTimeAny = DateTime.Now;
            CoroutineWorker = new Coroutine(KeyPressRoutine(key), this, coroutineKeyPress);
            Core.ParallelRunner.Run(CoroutineWorker);
        }

        private static IEnumerator KeyPressRoutine(Keys key)
        {
            Keyboard.KeyDown(key);
            yield return new WaitTime(20);
            Keyboard.KeyUp(key);
        }
        public override bool Initialise()
        {
            if (instance == null)
                instance = this;
            GameController.LeftPanel.WantUse(() => Settings.Enable);
            skillCoroutine = new Coroutine(WaitForSkillsAfterAreaChange(), this);
            Core.ParallelRunner.Run(skillCoroutine);
            return true;
        }
        public int GetMinnionsWithin(float maxDistance)
        {
            int count = 0;
            float maxDistanceSquare = maxDistance * maxDistance;
            foreach (var minnion in summons.minnions)
            {
                var monsterPosition = minnion.Pos;

                var xDiff = playerPosition.X - monsterPosition.X;
                var yDiff = playerPosition.Y - monsterPosition.Y;
                var monsterDistanceSquare = (xDiff * xDiff + yDiff * yDiff);

                if (monsterDistanceSquare <= maxDistanceSquare)
                {
                    count++;
                }

            }
            return count;
        }
        public int GetMonsterWithin(float maxDistance, MonsterRarity rarity = MonsterRarity.White)
        {
            int count = 0;
            float maxDistanceSquare = maxDistance * maxDistance;
            foreach (var monster in enemys)
            {
                if (monster.Rarity < rarity) { continue; }
                var monsterPosition = monster.Pos;

                var xDiff = playerPosition.X - monsterPosition.X;
                var yDiff = playerPosition.Y - monsterPosition.Y;
                var monsterDistanceSquare = (xDiff * xDiff + yDiff * yDiff);

                if (monsterDistanceSquare <= maxDistanceSquare)
                {
                    count++;
                }

            }
            return count;
        }

        public int CountCorpsesAroundMouse(float maxDistance)
        {
            int count = 0;
            float maxDistanceSquare = maxDistance * maxDistance;
            foreach (var corpse in corpses)
            {
                var monsterPosition = corpse.Pos;
                var screenPosition = GameController.IngameState.Camera.WorldToScreen(monsterPosition);
                var cursorPosition = MouseTools.GetCursorPosition();

                var xDiff = screenPosition.X - cursorPosition.X;
                var yDiff = screenPosition.Y - cursorPosition.Y;
                var monsterDistanceSquare = (xDiff * xDiff + yDiff * yDiff);

                if (monsterDistanceSquare <= maxDistanceSquare)
                {
                    count++;
                }
            }
            //LogMessage("Total Corpses: " + corpses.Count().ToString() + " Valid Corpses Counted: " + count.ToString());
            return count;
        }
        public int CountEnemysAroundMouse(float maxDistance)
        {
            int count = 0;
            float maxDistanceSquare = maxDistance * maxDistance;
            foreach (var enemy in enemys)
            {
                var monsterPosition = enemy.Pos;
                var screenPosition = GameController.IngameState.Camera.WorldToScreen(monsterPosition);
                var cursorPosition = MouseTools.GetCursorPosition();

                var xDiff = screenPosition.X - cursorPosition.X;
                var yDiff = screenPosition.Y - cursorPosition.Y;
                var monsterDistanceSquare = (xDiff * xDiff + yDiff * yDiff);

                if (monsterDistanceSquare <= maxDistanceSquare)
                {
                    count++;
                }
            }
            //LogMessage("Total Enemys: " + enemy.Count().ToString() + " Valid Enemys Counted: " + count.ToString());
            return count;
        }

        public int CountNonCursedEnemysAroundMouse(float maxDistance)
        {
            int count = 0;
            float maxDistanceSquare = maxDistance * maxDistance;
            foreach (var enemy in enemys)
            {
                var monsterPosition = enemy.Pos;
                var screenPosition = GameController.IngameState.Camera.WorldToScreen(monsterPosition);
                var cursorPosition = MouseTools.GetCursorPosition();

                var xDiff = screenPosition.X - cursorPosition.X;
                var yDiff = screenPosition.Y - cursorPosition.Y;
                var monsterDistanceSquare = (xDiff * xDiff + yDiff * yDiff);
                if (monsterDistanceSquare <= maxDistanceSquare && !EntityHasCurse(enemy))
                {
                    if (enemy.Rarity == MonsterRarity.Unique)
                        return 0;
                    count++;
                }
            }
            //LogMessage("Total Enemys: " + enemy.Count().ToString() + " Valid Enemys Counted: " + count.ToString());
            return count;
        }

        public bool EntityHasCurse(Entity entity)
        {
            if (entity.GetComponent<Life>().Buffs.Exists(x => 
            x.Name == SkillInfo.punishment.BuffName))
            {
                return true;
            }
            return false;
        }

        // Taken from ->
        // https://www.reddit.com/r/pathofexiledev/comments/787yq7/c_logout_app_same_method_as_lutbot/
        public static partial class CommandHandler
        {
            public static void KillTCPConnectionForProcess(int ProcessId)
            {
                MibTcprowOwnerPid[] table;
                var afInet = 2;
                var buffSize = 0;
                var ret = GetExtendedTcpTable(IntPtr.Zero, ref buffSize, true, afInet, TcpTableClass.TcpTableOwnerPidAll);
                var buffTable = Marshal.AllocHGlobal(buffSize);
                try
                {
                    ret = GetExtendedTcpTable(buffTable, ref buffSize, true, afInet, TcpTableClass.TcpTableOwnerPidAll);
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
                var PathConnection = table.FirstOrDefault(t => t.owningPid == ProcessId);
                PathConnection.state = 12;
                var ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(PathConnection));
                Marshal.StructureToPtr(PathConnection, ptr, false);
                SetTcpEntry(ptr);


            }

            [DllImport("iphlpapi.dll", SetLastError = true)]
            private static extern uint GetExtendedTcpTable(IntPtr pTcpTable, ref int dwOutBufLen, bool sort, int ipVersion, TcpTableClass tblClass, uint reserved = 0);

            [DllImport("iphlpapi.dll")]
            private static extern int SetTcpEntry(IntPtr pTcprow);

            [StructLayout(LayoutKind.Sequential)]
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

            private enum TcpTableClass
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

        public bool GCD()
        {
            if ((DateTime.Now - lastTimeAny).TotalMilliseconds > delay)
                return true;
            else
                return false;
        }

        public void Quit()
        {
            try
            {
                CommandHandler.KillTCPConnectionForProcess(GameController.Window.Process.Id);
            }
            catch (Exception e)
            {
                LogError($"{e}");
            }

        }
        public Keys GetSkillInputKey(int index)
        {
            if (index == 1)
                return Settings.InputKey1.Value;
            else if (index == 3)
                return Settings.InputKey3.Value;
            else if (index == 4)
                return Settings.InputKey4.Value;
            else if (index == 5)
                return Settings.InputKey5.Value;
            else if (index == 6)
                return Settings.InputKey6.Value;
            else if (index == 7)
                return Settings.InputKey7.Value;
            else if (index == 8)
                return Settings.InputKey8.Value;
            else if (index == 9)
                return Settings.InputKey9.Value;
            else if (index == 10)
                return Settings.InputKey10.Value;
            else if (index == 11)
                return Settings.InputKey11.Value;
            else if (index == 12)
                return Settings.InputKey12.Value;
            else return Keys.Escape;
        }
        private IEnumerator WaitForSkillsAfterAreaChange()
        {
            while (skills == null || localPlayer == null || GameController.IsLoading || !GameController.InGame)
            {
                yield return new WaitTime(200);
            }
            yield return new WaitTime(1000);
            SkillInfo.UpdateSkillInfo(true);
        }
        public override void AreaChange(AreaInstance area)
        {
            base.AreaChange(area);
            SkillInfo.ResetSkills();
            Coroutine skillCoroutine = new Coroutine(WaitForSkillsAfterAreaChange(), this);
            Core.ParallelRunner.Run(skillCoroutine);

        }
        public override void DrawSettings()
        {
            //base.DrawSettings();

            // Draw Custom GUI
            if (Settings.Enable)
                ImGuiDrawSettings.DrawImGuiSettings();
        }
        public override void Render()
        {
            if (Settings.Enable)
            {
                if (Settings.autoQuitHotkeyEnabled && (WinApi.GetAsyncKeyState(Settings.forcedAutoQuit) & 0x8000) != 0)
                {
                    LogMessage("Copilot: Panic Quit...");
                    Quit();
                }


                if (!GameController.Area.CurrentArea.IsHideout && !GameController.Area.CurrentArea.IsTown && !GameController.IngameState.IngameUi.StashElement.IsVisible && 
                    !GameController.IngameState.IngameUi.NpcDialog.IsVisible && !GameController.IngameState.IngameUi.SellWindow.IsVisible)
                {

                    localPlayer = GameController.Game.IngameState.Data.LocalPlayer;
                    player = localPlayer.GetComponent<Life>();
                    buffs = player.Buffs;
                    isAttacking = localPlayer.GetComponent<Actor>().isAttacking;
                    isCasting = localPlayer.GetComponent<Actor>().Action.HasFlag(ActionFlags.UsingAbility);
                    isMoving = localPlayer.GetComponent<Actor>().isMoving;
                    skills = localPlayer.GetComponent<Actor>().ActorSkills;
                    vaalSkills = localPlayer.GetComponent<Actor>().ActorVaalSkills;

                    playerPosition = GameController.Player.Pos;
                    // Bugged in 3.11 Hud, Monsters shown as isDead true for whatever reason.
                    //enemys = GameController.Entities.Where(x => x.IsValid && x.IsHostile && !x.IsHidden && !x.IsDead && x.IsAlive && x.IsTargetable && x.GetComponent<Monster>() != null && x.GetComponent<Life>().CurHP > 0 && !x.Buffs.Exists(b => b.Name == "hidden_monster_disable_minions"));
                    // Use thif for now.
                    enemys = GameController.Entities.Where(x => x.IsValid && x.IsHostile && !x.IsHidden && x.IsTargetable && x.GetComponent<Monster>() != null && x.GetComponent<Life>().CurHP > 0 && !x.Buffs.Exists(b => b.Name == "hidden_monster_disable_minions"));
                    if (Settings.offeringsEnabled || Settings.autoZombieEnabled)
                        corpses = GameController.Entities.Where(x => x.IsValid && !x.IsHidden && x.IsHostile && x.IsDead && x.IsTargetable && x.GetComponent<Monster>() != null);
                    if (Settings.autoGolemEnabled) { }
                        summons.UpdateSummons();

                    // Option for Cyclone on destroyable stuff ?
                    // Chest isTargetable && !isOpen && isHostile

                    #region Auto Quit
                    if (Settings.autoQuitEnabled)
                    {
                        try
                        {
                            if ((Math.Round(player.HPPercentage, 3) * 100 < Settings.hpPctQuit.Value || player.MaxES > 0 && (Math.Round(player.ESPercentage, 3) * 100 < Settings.esPctQuit.Value)))
                            {
                                Quit();
                            }
                        }
                        catch (Exception e)
                        {
                            LogError(e.ToString());
                        }
                    }
                    if (Settings.autoQuitGuardian)
                    {
                        try
                        {
                            if (Math.Round(summons.GetAnimatedGuardianHpp()) * 100 < Settings.guardianHpPct.Value)
                            {
                                Quit();
                            }
                        }
                        catch (Exception e)
                        {
                            LogError(e.ToString());
                        }
                    }
                    #endregion


                    // Do not Cast anything while we are untouchable or Chat is Open
                    if (buffs.Exists(x => x.Name == "grace_period") || GameController.IngameState.IngameUi.ChatBox.Parent.Parent.Parent.GetChildAtIndex(3).IsVisible)
                        return;


                    // Still waiting for proper Skill.cooldown / Skill.isReady to add to the Loop.
                    // Currently thats unanavailable in API.
                    foreach (var skill in skills)
                    {
                        if (!GCD())
                            break;
                        skill.Stats.TryGetValue(GameStat.ManaCost, out int manaCost);
                        if (!skill.IsOnSkillBar || skill.SkillSlotIndex < 1 || skill.SkillSlotIndex == 2 || player.CurMana < manaCost)
                            continue;

                        #region Mirage Archer
                        if (Settings.mirageEnabled)
                        {
                            try
                            {
                                skill.Stats.TryGetValue(GameStat.NumberOfMirageArchersAllowed, out int mirage);
                                if ((DateTime.Now - lastMirage).TotalMilliseconds > 500 && mirage >= 1 && CountEnemysAroundMouse(Settings.mirageRange.Value) > 0 &&
                                !buffs.Exists(x => x.Name == "mirage_archer_visual_buff" && x.Timer > 0.5))
                                {
                                    KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                    lastMirage = DateTime.Now;
                                }
                            }
                            catch (Exception e)
                            {
                                LogError(e.ToString());
                            }
                        }
                        #endregion

                        #region Enduring Cry / Rallying Cry
                        if (Settings.enduringCryEnabled || Settings.rallyingCryEnabled)
                        {
                            try
                            {
                                if ((DateTime.Now - lastWarCry).TotalMilliseconds > Settings.warCryCooldown.Value && skill.Id == SkillInfo.enduringCry.Id || skill.Id == SkillInfo.rallyingCry.Id)
                                {
                                    if (Settings.enduringCryEnabled && Settings.rallyingCryEnabled && skills.Exists(x => x.Id == SkillInfo.enduringCry.Id) && skills.Exists(x => x.Id == SkillInfo.rallyingCry.Id))
                                    {
                                        if (GetMonsterWithin(Settings.warCryTriggerRange) >= 1 || player.HPPercentage < 0.90f || (Settings.warCryKeepRage &&
                                            ((DateTime.Now - lastWarCry).TotalMilliseconds > 3800 || !buffs.Exists(b => b.Name == "rage" && b.Charges >= 50))))
                                        {
                                            if (!buffs.Exists(x => x.Name == "endurance_charge" && x.Timer * 1000 > Settings.warCryCooldown.Value + 100) ||
                                                buffs.Exists(x => x.Name == "inspiring_cry" && x.Timer * 1000 > Settings.warCryCooldown.Value + 100))
                                            {
                                                if (skill.Id == SkillInfo.rallyingCry.Id)
                                                    continue;
                                                if (skill.Id == SkillInfo.enduringCry.Id)
                                                {
                                                    KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                                    lastWarCry = DateTime.Now;
                                                }
                                            }
                                            else if (skill.Id == SkillInfo.rallyingCry.Id)
                                            {
                                                KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                                lastWarCry = DateTime.Now;
                                            }

                                        }
                                    }
                                    else if (Settings.enduringCryEnabled || Settings.rallyingCryEnabled)
                                    {
                                        if (GetMonsterWithin(Settings.warCryTriggerRange) >= 1 || player.HPPercentage < 0.90f || (Settings.warCryKeepRage &&
                                            ((DateTime.Now - lastWarCry).TotalMilliseconds > 3800 || !buffs.Exists(b => b.Name == "rage" && b.Charges >= 50))))
                                        {
                                            if (Settings.enduringCryEnabled && skill.Id == SkillInfo.enduringCry.Id)
                                            {
                                                KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                                lastWarCry = DateTime.Now;
                                            }
                                            else if (Settings.rallyingCryEnabled && skill.Id == SkillInfo.rallyingCry.Id)
                                            {
                                                KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                                lastWarCry = DateTime.Now;
                                            }

                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                LogError(e.ToString());
                            }
                        }
                        #endregion

                        #region Phase Run
                        if (Settings.phaserunEnabled)
                        {
                            try
                            {
                                if ((DateTime.Now - lastPhaserun).TotalMilliseconds > Settings.phaserunDelay.Value && skill.Id == SkillInfo.phaserun.Id)
                                {
                                    if (!isAttacking && isMoving && (!buffs.Exists(b => b.Name == SkillInfo.phaserun.BuffName && b.Timer < 0.1)))
                                    {
                                        KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                        lastPhaserun = DateTime.Now;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                LogError(e.ToString());
                            }
                        }
                        #endregion

                        #region Molten Shell / Steelskin / Bone Armour / Arcane Cloak
                        if (Settings.moltenShellEnabled)
                        {
                            try
                            {
                                // Cooldown reset starts on Buff expire
                                if (buffs.Exists(x => x.Name == SkillInfo.moltenShell.BuffName) || buffs.Exists(x => x.Name == SkillInfo.steelSkin.BuffName) || buffs.Exists(x => x.Name == SkillInfo.boneArmour.BuffName) 
                                    || buffs.Exists(x => x.Name == SkillInfo.arcaneCloak.BuffName))
                                {
                                    lastMoltenShell = DateTime.MaxValue;
                                }
                                else
                                {
                                    if (lastMoltenShell == DateTime.MaxValue)
                                    {
                                        lastMoltenShell = DateTime.Now;
                                    }
                                }
                                if ((DateTime.Now - lastMoltenShell).TotalMilliseconds > Settings.moltenShellDelay.Value &&
                                    (skill.Id == SkillInfo.moltenShell.Id || skill.Id == SkillInfo.steelSkin.Id || skill.Id == SkillInfo.boneArmour.Id || skill.Id == SkillInfo.arcaneCloak.Id))
                                {
                                    if ((GetMonsterWithin(Settings.moltenShellRange) >= 1))
                                    {
                                        KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                        lastMoltenShell = DateTime.MaxValue;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                LogError(e.ToString());
                            }
                        }
                        #endregion

                        #region Blood Rage
                        if (Settings.bloodRageEnabled)
                        {
                            try
                            {
                                if ((DateTime.Now - lastBloodRage).TotalMilliseconds > Settings.bloodRageDelay.Value && skill.Id == SkillInfo.bloodRage.Id)
                                {
                                    if (!buffs.Exists(b => b.Name == SkillInfo.bloodRage.BuffName && b.Timer > 1.0) && (GetMonsterWithin(Settings.bloodRageRange) >= 1))
                                    {
                                        KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                        lastBloodRage = DateTime.Now;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                LogError(e.ToString());
                            }
                        }
                        #endregion

                        #region Auto Summon
                        if (Settings.autoSummonEnabled)
                        {
                            try
                            {
                                if (Settings.autoGolemEnabled &&
                                    (summons.chaosElemental < Settings.autoGolemChaosMax || 
                                    summons.fireElemental < Settings.autoGolemFireMax || 
                                    summons.iceElemental < Settings.autoGolemIceMax || 
                                    summons.lightningGolem < Settings.autoGolemLightningMax || 
                                    summons.rockGolem < Settings.autoGolemRockMax || 
                                    summons.boneGolem < Settings.autoBoneMax || 
                                    summons.dropBearUniqueSummoned < Settings.autoGolemDropBearMax)
                                    && (DateTime.Now - lastAutoGolem).TotalMilliseconds > 1200 && !isCasting && !isAttacking && GetMonsterWithin(Settings.autoGolemAvoidRange) == 0)
                                {
                                    if (skill.Id == SkillInfo.chaosGolem.Id && summons.chaosElemental < Settings.autoGolemChaosMax)
                                    {
                                        KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                        lastAutoGolem = DateTime.Now;
                                    }
                                    else if (skill.Id == SkillInfo.flameGolem.Id && summons.fireElemental < Settings.autoGolemFireMax)
                                    {
                                        KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                        lastAutoGolem = DateTime.Now;
                                    }
                                    else if (skill.Id == SkillInfo.iceGolem.Id && summons.iceElemental < Settings.autoGolemIceMax)
                                    {
                                        KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                        lastAutoGolem = DateTime.Now;
                                    }
                                    else if (skill.Id == SkillInfo.lightningGolem.Id && summons.lightningGolem < Settings.autoGolemLightningMax)
                                    {
                                        KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                        lastAutoGolem = DateTime.Now;
                                    }
                                    else if (skill.Id == SkillInfo.stoneGolem.Id && summons.rockGolem < Settings.autoGolemRockMax)
                                    {
                                        KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                        lastAutoGolem = DateTime.Now;
                                    }
                                    else if (skill.Id == SkillInfo.carrionGolem.Id && summons.boneGolem < Settings.autoBoneMax)
                                    {
                                        KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                        lastAutoGolem = DateTime.Now;
                                    }
                                    else if (skill.Id == SkillInfo.ursaGolem.Id && summons.dropBearUniqueSummoned < Settings.autoGolemDropBearMax)
                                    {
                                        KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                        lastAutoGolem = DateTime.Now;
                                    }
                                }
                                if (Settings.autoZombieEnabled && skill.Id == SkillInfo.raiseZombie.Id)
                                {
                                    skill.Stats.TryGetValue(GameStat.NumberOfZombiesAllowed, out int maxZombies);
                                    if (summons.zombies < maxZombies && !isCasting && !isAttacking && GetMonsterWithin(600) == 0
                                    && CountCorpsesAroundMouse(mouseAutoSnapRange) > 0 && (DateTime.Now - lastAutoGolem).TotalMilliseconds > 1200)
                                    {
                                        KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                        lastAutoGolem = DateTime.Now;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                LogError(e.ToString());
                            }
                        }
                        #endregion

                        #region Vortex
                        if (Settings.vortexEnabled)
                        {
                            try
                            {
                                if ((DateTime.Now - lastVortex).TotalMilliseconds > Settings.vortexDelay.Value && skill.Id == SkillInfo.vortex.Id)
                                {
                                    if (GetMonsterWithin(Settings.vortexRange) >= 1 || (Settings.vortexFrostbolt && skills.Any(x => x.Id == SkillInfo.frostbolt.Id && x.SkillUseStage > 2) ))
                                    {
                                        KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                        lastVortex = DateTime.Now;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                LogError(e.ToString());
                            }
                        }
                        #endregion

                        #region Divine Ire
                        if (Settings.divineIreEnabled)
                        {
                            try
                            {
                                if ((DateTime.Now - lastStackSkill).TotalMilliseconds > 250 &&
                                    (skill.Id == SkillInfo.divineIre.Id || skill.Id == SkillInfo.scourgeArror.Id || skill.Id == SkillInfo.bladeFlurry.Id))
                                {
                                    if (buffs.Exists(b => (b.Name == SkillInfo.divineIre.BuffName && b.Charges >= Settings.divineIreStacks.Value) ||
                                    (b.Name == SkillInfo.bladeFlurry.BuffName && b.Charges >= Settings.divineIreStacks) ||
                                    (b.Name == SkillInfo.scourgeArror.BuffName && b.Charges >= Settings.divineIreStacks)))
                                    {

                                        if (Settings.divineIreWaitForInfused)
                                        {
                                            // Get delay here at some point ?
                                            if (!buffs.Exists(x => x.Name == "storm_barrier_support_damage" && x.Timer > 1.0))
                                            {
                                                return;
                                            }
                                        }
                                        Keyboard.KeyUp(GetSkillInputKey(skill.SkillSlotIndex));
                                        lastStackSkill = DateTime.Now;
                                        if (Settings.debugMode)
                                            LogError("Release Key Pressed: " + GetSkillInputKey(skill.SkillSlotIndex).ToString());
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                LogError(e.ToString());
                            }
                        }
                        #endregion

                        #region Doedre Effigy
                        if (Settings.doedreEffigyEnabled)
                        {
                            try
                            {
                                if ((DateTime.Now - lastdoedreEffigy).TotalMilliseconds > Settings.doedreEffigyDelay.Value && skill.Id == SkillInfo.doedreEffigy.Id)
                                {
                                    if (CountEnemysAroundMouse(350) > 0)
                                    {
                                        KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                        lastdoedreEffigy = DateTime.Now;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                LogError(e.ToString());
                            }
                        }
                        #endregion

                        #region Offerings
                        if (Settings.offeringsEnabled)
                        {
                            try
                            {
                                if (((!Settings.offeringsUseWhileCasting && !isCasting && !isAttacking) || Settings.offeringsUseWhileCasting) && (DateTime.Now - lastOfferings).TotalMilliseconds > 500 &&
                                    (skill.Id == SkillInfo.spiritOffering.Id || skill.Id == SkillInfo.boneOffering.Id || skill.Id == SkillInfo.fleshOffering.Id))
                                {
                                    if (GetMonsterWithin(Settings.offeringsTriggerRange) >= Settings.offeringsMinEnemys && !buffs.Exists(x => x.Name == "active_offering" ) && CountCorpsesAroundMouse(mouseAutoSnapRange) > 0)
                                    {
                                        KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                        lastOfferings = DateTime.Now;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                LogError(e.ToString());
                            }
                        }
                        #endregion

                        #region Any Vaal Skill
                        if (Settings.anyVaalEnabled)
                        {
                            try
                            {
                                if (skill.InternalName.Contains("vaal"))
                                {
                                    if (GetMonsterWithin(Settings.anyVaalTriggerRange) >= Settings.anyVaalMinEnemys && vaalSkills.Exists(x => x.VaalSkillInternalName == skill.InternalName && x.CurrVaalSouls >= x.VaalSoulsPerUse))
                                    {
                                        if ((Math.Round(player.HPPercentage, 3) * 100 <= Settings.anyVaalHpPct.Value || player.MaxES > 0 && (Math.Round(player.ESPercentage, 3) * 100 < Settings.anyVaalEsPct.Value)))
                                        {
                                            KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                LogError(e.ToString());
                            }
                        }
                        #endregion

                        #region Brand Recall
                        if (Settings.brandRecallEnabled)
                        {
                            try
                            {
                                if ((DateTime.Now - lastBrandRecall).TotalMilliseconds > Settings.brandRecallCooldown && skill.Id == SkillInfo.brandRecall.Id)
                                {
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
                                        if (GetMonsterWithin(Settings.brandRecallTriggerRange) >= Settings.brandRecallMinEnemys)
                                        {
                                            KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                            lastBrandRecall = DateTime.Now;
                                        }
                                    //}                                    
                                }   
                            }
                        catch (Exception e)
                            {
                                LogError(e.ToString());
                            }
                        }
                        #endregion

                        #region Tempest Shield
                        if (Settings.tempestShieldEnabled)
                        {
                            try
                            {
                                if (((!Settings.tempestShieldUseWhileCasting && !isCasting && !isAttacking) || Settings.tempestShieldUseWhileCasting) && (DateTime.Now - lastTempestShield).TotalMilliseconds > 1200 && skill.Id == SkillInfo.tempestShield.Id)
                                {
                                    if (!buffs.Exists(x => x.Name == SkillInfo.tempestShield.BuffName && x.Timer > 1.0) && GetMonsterWithin(Settings.tempestShieldTriggerRange) >= Settings.tempestShieldMinEnemys)
                                    {
                                        KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                        lastTempestShield = DateTime.Now;
                                    }                                   
                                }
                            }
                            catch (Exception e)
                            {
                                LogError(e.ToString());
                            }
                        }
                        #endregion

                        #region AutoAttack Cyclone / Nova / 
                        if (Settings.autoAttackEnabled)
                        {
                            try
                            {
                                if ((DateTime.Now - autoAttackUpdate).TotalMilliseconds > 50 && 
                                    (skill.Id == SkillInfo.cyclone.Id || skill.Id == SkillInfo.iceNova.Id || skill.Id == SkillInfo.flickerStrike.Id))
                                {
                                    autoAttackUpdate = DateTime.Now;
                                    if ((Settings.autoAttackLeftMouseCheck.Value && !MouseTools.IsMouseLeftPressed() || !Settings.autoAttackLeftMouseCheck.Value) &&
                                        GetMonsterWithin(Settings.autoAttackRange) >= 1)
                                    {
                                        if ((Keyboard.IsKeyDown((int)Settings.autoAttackPickItKey.Value) && Keyboard.IsKeyDown((int)GetSkillInputKey(skill.SkillSlotIndex)) || !isCasting && !isAttacking && autoAttackRunning > DateTime.MinValue && (DateTime.Now - autoAttackRunning).TotalMilliseconds > 100 && Keyboard.IsKeyDown((int)GetSkillInputKey(skill.SkillSlotIndex))))
                                        {
                                            Keyboard.KeyUp(GetSkillInputKey(skill.SkillSlotIndex));
                                            if (Settings.debugMode.Value)
                                                LogMessage("Copilot: Detected Key Priority Problem due to User Input, fixing.");
                                        }
                                        if (!Keyboard.IsKeyDown((int)GetSkillInputKey(skill.SkillSlotIndex)) && !Keyboard.IsKeyDown((int)Settings.autoAttackPickItKey.Value))
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
                        }
                        #endregion

                        #region Convocation
                        if (Settings.convocationEnabled)
                        {
                            try
                            {
                                if (skill.Id == SkillInfo.convocation.Id && (DateTime.Now - lastConvocation).TotalMilliseconds > Settings.convocationCooldown)
                                {
                                    if (GetMonsterWithin(Settings.convocationAvoidUniqueRange, MonsterRarity.Unique) > 0)
                                        return;
                                    if (Math.Round(summons.GetLowestMinionHpp()) * 100 < Settings.convocationHp.Value)
                                    {
                                        lastConvocation = DateTime.Now;
                                        KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                    }           
                                    else if (GetMonsterWithin(Settings.convocationMobRange) > 0 && (GetMinnionsWithin(Settings.convocationMinnionRange) / summons.minnions.Count) * 100 <= Settings.convocationMinnionPct)
                                    {
                                        lastConvocation = DateTime.Now;
                                        KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                LogError(e.ToString());
                            }
                        }
                        #endregion

                        #region Auto Curse
                        if (Settings.autoCurseEnabled)
                        {
                            try
                            {
                                if (skill.Id == SkillInfo.punishment.Id && (DateTime.Now - lastCurse).TotalMilliseconds > Settings.autoCurseCooldown)
                                {
                                    if (CountNonCursedEnemysAroundMouse(Settings.autoCurseRange) >= Settings.autoCurseMinEnemys)
                                    {
                                        lastCurse = DateTime.Now;
                                        KeyPress(GetSkillInputKey(skill.SkillSlotIndex));
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                LogError(e.ToString());
                            }
                        }
                        #endregion

                        #region Detonate Mines ( to be done )
                        if (Settings.minesEnabled)
                        {
                            try
                            {
                                var remoteMines = localPlayer.GetComponent<Actor>().DeployedObjects.Where(x => x.Entity != null && x.Entity.Path == "Metadata/MiscellaneousObjects/RemoteMine").ToList();

                                // Removed Logic
                                // What should a proper Detonator do and when ?
                                // Detonate Mines when they have the chance to hit a target (Range), include min. mines ?
                                // Internal delay 500-1000ms ?
                                // Removing/Filter enemys that are not "deployed" yet / invunerale from enemys list ? 
                            }
                            catch (Exception e)
                            {
                                LogError(e.ToString());
                            }
                        }
                        #endregion
                    }

                    #region Delve Flare
                    if (Settings.delveFlareEnabled)
                    {
                        try
                        {
                            if ((DateTime.Now - lastDelveFlare).TotalMilliseconds > 1000 && (player.ESPercentage < (Settings.delveFlareEsBelow / 100) || player.HPPercentage < (Settings.delveFlareHpBelow / 100)) && buffs.Exists(x => x.Name == "delve_degen_buff" && x.Charges >= Settings.delveFlareDebuffStacks))
                            {
                                KeyPress(Settings.delveFlareKey.Value);
                                lastDelveFlare = DateTime.Now;
                            }
                        }
                        catch (Exception e)
                        {
                            LogError(e.ToString());
                        }
                    }
                    #endregion

                    #region Custom Skill
                    if (Settings.customEnabled)
                    {
                        try
                        {
                            if (GCD() && (DateTime.Now - lastCustom).TotalMilliseconds > Settings.customCooldown.Value && GetMonsterWithin(Settings.customTriggerRange) >= Settings.customMinEnemys)
                            {
                                if ((Math.Round(player.HPPercentage, 3) * 100 <= Settings.customHpPct.Value || player.MaxES > 0 && (Math.Round(player.ESPercentage, 3) * 100 < Settings.customEsPct.Value)))
                                {
                                    KeyPress(Settings.customKey);
                                    lastCustom = DateTime.Now;
                                }

                            }
                        }
                        catch (Exception e)
                        {
                            LogError(e.ToString());
                        }
                    }
                    #endregion
                }
            }
        }
    }
    internal static class MouseTools
    {
        public static bool IsMouseLeftPressed()
        {
            if (Control.MouseButtons == MouseButtons.Left)
                return true;
            else
                return false;
        }
        public static void MouseLeftClickEvent()
        {
            MouseEvent(MouseEventFlags.LeftUp);
            Thread.Sleep(10);
            MouseEvent(MouseEventFlags.LeftDown);
        }

        public static void MouseRightClickEvent()
        {
            MouseEvent(MouseEventFlags.RightUp);
            Thread.Sleep(10);
            MouseEvent(MouseEventFlags.RightDown);
        }

        public static Point GetCursorPosition()
        {
            Point currentMousePoint;
            return GetCursorPos(out currentMousePoint) ? new Point(currentMousePoint.X, currentMousePoint.Y) : new Point(0, 0);
        }

        private static void MouseEvent(MouseEventFlags value)
        {
            var position = GetCursorPosition();

            mouse_event
                ((int)value,
                    position.X,
                    position.Y,
                    0,
                    0)
                ;
        }
    }
    public class Keyboard
    {
        [DllImport("user32.dll")]
        private static extern uint keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        private const int KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const int KEYEVENTF_KEYUP = 0x0002;

        private const int ACTION_DELAY = 15;


        [DllImport("user32.dll")]
        public static extern bool BlockInput(bool fBlockIt);

        public static void KeyDown(Keys key)
        {
            keybd_event((byte)key, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
        }

        public static void KeyUp(Keys key)
        {
            keybd_event((byte)key, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0); //0x7F
        }

        [DllImport("USER32.dll")]
        private static extern short GetKeyState(int nVirtKey);

        public static bool IsKeyDown(int nVirtKey)
        {
            return GetKeyState(nVirtKey) < 0;
        }
    }
    public static class WinApiMouse
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(out Point lpMousePoint);

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        #region Structs/Enums

        [Flags]
        public enum MouseEventFlags
        {
            LeftDown = 0x00000002,
            LeftUp = 0x00000004,
            MiddleDown = 0x00000020,
            MiddleUp = 0x00000040,
            Move = 0x00000001,
            Absolute = 0x00008000,
            RightDown = 0x00000008,
            RightUp = 0x00000010
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public int X;
            public int Y;

            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }

        }

        #endregion
    }
}





