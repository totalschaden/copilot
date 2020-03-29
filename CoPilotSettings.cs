using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using ExileCore.Shared.Attributes;

using System.Windows.Forms;

namespace CoPilot
{
    public class CoPilotSettings : ISettings
    {
        public CoPilotSettings()
        {
            Enable = new ToggleNode(false);
            empty = new EmptyNode();
            debugMode = new ToggleNode(false);

            autoQuitEnabled = new ToggleNode(false);
            hpPctQuit = new RangeNode<float>(35f, 0f, 100f);
            esPctQuit = new RangeNode<float>(35f, 0f, 100f);
            forcedAutoQuit = Keys.F4;

            phaserunEnabled = new ToggleNode(false);
            phaserunDelay = new RangeNode<int>(4000, 100, 4100);
            phaserunKey = Keys.W;

            moltenShellEnabled = new ToggleNode(false);
            moltenShellDelay = new RangeNode<int>(4000, 100, 4100);
            moltenShellKey = Keys.T;            
            moltenShellRange = new RangeNode<int>(550, 100, 1000);

            enduringCryEnabled = new ToggleNode(false);
            enduringCryRange = new RangeNode<int>(550, 100, 1000);
            warCryDelay = new RangeNode<int>(4000, 3330, 4100);
            enduringCryKey = Keys.E;

            rallyingCryEnabled = new ToggleNode(false);
            rallyingCryKey = Keys.Q;
            rallyingCryRange = new RangeNode<int>(300, 100, 1000);

            divineIreEnabled = new ToggleNode(false);
            divineIreStacks = new RangeNode<int>(20, 1, 20);
            divineIreKey = Keys.NumPad9;
            divineIreWaitForInfused = new ToggleNode(false);

            delveFlareEnabled = new ToggleNode(false);
            delveFlareKey = Keys.D6;
            delveFlareDebuffStacks = new RangeNode<int>(12, 1, 1000);
            delveFlareHpBelow = new RangeNode<float>(75, 0, 100);
            delveFlareEsBelow = new RangeNode<float>(0, 0, 100);

            vortexEnabled = new ToggleNode(false);
            vortexDelay = new RangeNode<int>(1800, 500, 1900);
            vortexRange = new RangeNode<int>(300, 100, 1000);
            vortexKey = Keys.D8;

            bloodRageEnabled = new ToggleNode(false);
            bloodRageKey = Keys.R;
            bloodRageDelay = new RangeNode<int>(1000, 1000, 10100);
            bloodRageRange = new RangeNode<int>(300, 800, 3000);

            offeringsEnabled = new ToggleNode(false);
            offeringsKey = Keys.Q;

            doedreEffigyEnabled = new ToggleNode(false);
            doedreEffigyKey = Keys.R;
            doedreEffigyDelay = new RangeNode<int>(600, 500, 2000);

            
            useSpeed4 = new ToggleNode(false);
            useSpeed5 = new ToggleNode(false);
            useSpeedAttack = new ToggleNode(false);
            useSpeedMoving = new ToggleNode(false);

            minesEnabled = new ToggleNode(false);
            minesDelay = new RangeNode<int>(1000, 250, 3000);
            minesMin = new RangeNode<int>(1, 1, 20);
            minesDetonateRange = new RangeNode<int>(50, 10, 200);

            autoAttackEnabled = new ToggleNode(false);
            autoAttackLeftMouseCheck = new ToggleNode(true);
            autoAttackKey = new HotkeyNode(Keys.T);
            autoAttackRange = new RangeNode<int>(500, 100, 2000);



        }
        [Menu("Plugin by Totalschaden", 1)]
        public EmptyNode empty { get; set; }
        [Menu("Debug Mode", 2, 1)]
        public ToggleNode debugMode { get; set; }

        #region Auto Quit
        [Menu("Auto Quit (RUN LOADER.EXE AS ADMIN !!!", 1000)]
        public EmptyNode emptyAuto { get; set; }
        [Menu("Enable", 1001, 1000)]
        public ToggleNode autoQuitEnabled { get; set; }

        [Menu("Hotkey:", 1002, 1000)]
        public HotkeyNode forcedAutoQuit { get; set; }
        [Menu("%Life Quit", 1003, 1000)]
        public RangeNode<float> hpPctQuit { get; set; }

        [Menu("%ES Quit", 1004, 1000)]
        public RangeNode<float> esPctQuit { get; set; }
        #endregion

        #region Phaserun
        [Menu("Phase Run", 3)]
        public EmptyNode emptyPhase { get; set; }
        [Menu("Enable", 4, 3)]
        public ToggleNode phaserunEnabled { get; set; }

        [Menu("Key:", 5, 3)]
        public HotkeyNode phaserunKey { get; set; }

        [Menu("Cooldown", 6, 3)]
        public RangeNode<int> phaserunDelay { get; set; }
        #endregion

        #region Molten Shell
        [Menu("Molten Shell / Bone Armour / Steelskin", 10)]
        public EmptyNode emptyMolten { get; set; }
        [Menu("Enable", 11, 10)]
        public ToggleNode moltenShellEnabled { get; set; }

        [Menu("Key:", 12, 10)]
        public HotkeyNode moltenShellKey { get; set; }

        [Menu("Cooldown", 13, 10)]
        public RangeNode<int> moltenShellDelay { get; set; }

        [Menu("Trigger Range", 14, 10)]
        public RangeNode<int> moltenShellRange { get; set; }
        #endregion

        #region War Cry
        [Menu("Enduring Cry", 20)]
        public EmptyNode emptyEnduring { get; set; }
        [Menu("Enable", 21, 20)]
        public ToggleNode enduringCryEnabled { get; set; }

        [Menu("Cooldown", 22, 20)]
        public RangeNode<int> warCryDelay { get; set; }

        [Menu("Key:", 23, 20)]
        public HotkeyNode enduringCryKey { get; set; }

        [Menu("Trigger Range", 24, 20)]
        public RangeNode<int> enduringCryRange { get; set; }

        [Menu("Rallying Cry", 25)]
        public EmptyNode emptyRally { get; set; }
        [Menu("Enable", 26, 25)]
        public ToggleNode rallyingCryEnabled { get; set; }

        [Menu("Key:", 27, 25)]
        public HotkeyNode rallyingCryKey { get; set; }

        [Menu("Trigger Range", 28, 25)]
        public RangeNode<int> rallyingCryRange { get; set; }
        #endregion

        #region Divine Ire / Blade Flurry / Scourge Arrow
        [Menu("Divine Ire / Blade Flurry / Scourge Arrow (Mouse might have stuck Issue, use Keyboard)", 30)]
        public EmptyNode emptyIre { get; set; }
        [Menu("Enable", 31, 30)]
        public ToggleNode divineIreEnabled { get; set; }

        [Menu("Key:", 33, 30)]
        public HotkeyNode divineIreKey { get; set; }

        [Menu("Stacks", 34, 30)]
        public RangeNode<int> divineIreStacks { get; set; }
        [Menu("Wait for Infused Channelling Support", 35, 30)]
        public ToggleNode divineIreWaitForInfused { get; set; }
        #endregion

        #region Delve Flare
        [Menu("Delve Flare", 40)]
        public EmptyNode emptyFlare { get; set; }
        [Menu("Enable", 41, 40)]
        public ToggleNode delveFlareEnabled { get; set; }

        [Menu("Key:", 42, 40)]
        public HotkeyNode delveFlareKey { get; set; }

        [Menu("Delve Debuff Stacks", 43, 40)]
        public RangeNode<int> delveFlareDebuffStacks { get; set; }

        [Menu("min. Hp%", 44, 40)]
        public RangeNode<float> delveFlareHpBelow { get; set; }

        [Menu("min. Es%", 45, 40)]
        public RangeNode<float> delveFlareEsBelow { get; set; }
        #endregion

        #region Vortex
        [Menu("Vortex", 50)]
        public EmptyNode emptyVortex { get; set; }
        [Menu("Enable", 51, 50)]
        public ToggleNode vortexEnabled { get; set; }

        [Menu("Cooldown", 52, 50)]
        public RangeNode<int> vortexDelay { get; set; }

        [Menu("Key:", 53, 50)]
        public HotkeyNode vortexKey { get; set; }

        [Menu("Trigger Range", 54, 50)]
        public RangeNode<int> vortexRange { get; set; }
        #endregion

        #region BloodRage
        [Menu("Blood Rage", 60)]
        public EmptyNode emptyBlood { get; set; }
        [Menu("Enable", 61, 60)]
        public ToggleNode bloodRageEnabled { get; set; }

        [Menu("Cooldown", 62, 60)]
        public RangeNode<int> bloodRageDelay { get; set; }

        [Menu("Key:", 63, 60)]
        public HotkeyNode bloodRageKey { get; set; }

        [Menu("Trigger Range", 64, 60)]
        public RangeNode<int> bloodRageRange { get; set; }
        #endregion

        #region Offerings
        [Menu("Offerings (This will get you stuck in Animation for your Casttime !", 70)]
        public EmptyNode emptyOffering { get; set; }
        [Menu("Enable", 71, 70)]
        public ToggleNode offeringsEnabled { get; set; }

        [Menu("Key:", 73, 70)]
        public HotkeyNode offeringsKey { get; set; }
        #endregion

        #region Doedre Effigy
        [Menu("Use Doedre's Effigy", 80)]
        public EmptyNode emptyeffigy { get; set; }
        [Menu("Enable", 81, 80)]
        public ToggleNode doedreEffigyEnabled { get; set; }

        [Menu("Key:", 83, 80)]
        public HotkeyNode doedreEffigyKey { get; set; }

        [Menu("Cooldown", 84, 80)]
        public RangeNode<int> doedreEffigyDelay { get; set; }
        #endregion

        #region Flask
        
        [Menu("Speed Flask", 90)]
        public EmptyNode emptySpeed { get; set; }
        [Menu("Enable Slot 4", 91, 90)]
        public ToggleNode useSpeed4 { get; set; }
        [Menu("Enable Slot 5", 92, 90)]
        public ToggleNode useSpeed5 { get; set; }
        [Menu("Use when Moving", 93, 90)]
        public ToggleNode useSpeedMoving { get; set; }
        [Menu("Use when Attacking", 94, 90)]
        public ToggleNode useSpeedAttack { get; set; }

        #endregion

        #region Mines

        [Menu("Detonate Mines || This is not Done yet and will not work", 100)]
        public EmptyNode emptyMines { get; set; }
        [Menu("Enable", 101, 100)]
        public ToggleNode minesEnabled { get; set; }
        [Menu("Cooldown", 102, 100)]
        public RangeNode<int> minesDelay { get; set; }
        [Menu("min. Mines", 103, 100)]
        public RangeNode<int> minesMin { get; set; }
        [Menu("Detonate Range", 103, 100)]
        public RangeNode<int> minesDetonateRange { get; set; }


        #endregion

        #region AutoAttack Cyclone / Nova etc.

        [Menu("Auto Attack (Cyclone/Nova/etc.)", 110)]
        public EmptyNode autoAttack { get; set; }

        [Menu("Enable", 111, 110)]
        public ToggleNode autoAttackEnabled { get; set; }

        [Menu("Dont use on Left Mouse Pressed", 112, 110)]
        public ToggleNode autoAttackLeftMouseCheck { get; set; }

        [Menu("Key: (Dont use Mousebuttons)", 113, 110)]
        public HotkeyNode autoAttackKey { get; set; }

        [Menu("Trigger Range", 114, 110)]
        public RangeNode<int> autoAttackRange { get; set; }


        #endregion


        public ToggleNode Enable { get; set; }
    }
}
