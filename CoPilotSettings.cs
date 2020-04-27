using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using ExileCore.Shared.Attributes;

using System.Windows.Forms;

namespace CoPilot
{
    public class CoPilotSettings : ISettings
    {
        public ToggleNode debugMode { get; set; }


        #region Auto Quit
        public ToggleNode autoQuitEnabled { get; set; }
        public HotkeyNode forcedAutoQuit { get; set; }
        public RangeNode<float> hpPctQuit { get; set; }
        public RangeNode<float> esPctQuit { get; set; }
        #endregion

        #region Phaserun
        public ToggleNode phaserunEnabled { get; set; }
        public RangeNode<int> phaserunDelay { get; set; }
        #endregion

        #region Molten Shell
        public ToggleNode moltenShellEnabled { get; set; }
        public RangeNode<int> moltenShellDelay { get; set; }
        public RangeNode<int> moltenShellRange { get; set; }
        #endregion

        #region War Cry
        public ToggleNode enduringCryEnabled { get; set; }
        public RangeNode<int> warCryCooldown { get; set; }
        public RangeNode<int> warCryTriggerRange { get; set; }
        public ToggleNode rallyingCryEnabled { get; set; }
        public ToggleNode warCryKeepRage { get; set; }
        #endregion

        #region Divine Ire / Blade Flurry / Scourge Arrow
        public ToggleNode divineIreEnabled { get; set; }
        public RangeNode<int> divineIreStacks { get; set; }
        public ToggleNode divineIreWaitForInfused { get; set; }
        #endregion

        #region Delve Flare
        public ToggleNode delveFlareEnabled { get; set; }
        public HotkeyNode delveFlareKey { get; set; }
        public RangeNode<int> delveFlareDebuffStacks { get; set; }
        public RangeNode<float> delveFlareHpBelow { get; set; }
        public RangeNode<float> delveFlareEsBelow { get; set; }
        #endregion

        #region Vortex
        public ToggleNode vortexEnabled { get; set; }
        public RangeNode<int> vortexDelay { get; set; }
        public RangeNode<int> vortexRange { get; set; }
        #endregion

        #region BloodRage
        public ToggleNode bloodRageEnabled { get; set; }
        public RangeNode<int> bloodRageDelay { get; set; }
        public RangeNode<int> bloodRageRange { get; set; }
        #endregion

        #region Offerings
        public ToggleNode offeringsEnabled { get; set; }
        public RangeNode<int> offeringsMinEnemys { get; set; }
        public RangeNode<int> offeringsTriggerRange { get; set; }
        #endregion

        #region Doedre Effigy
        public ToggleNode doedreEffigyEnabled { get; set; }
        public RangeNode<int> doedreEffigyDelay { get; set; }
        #endregion

        #region Flask
        public ToggleNode useSpeed4 { get; set; }
        public ToggleNode useSpeed5 { get; set; }
        public ToggleNode useSpeedMoving { get; set; }
        public ToggleNode useSpeedAttack { get; set; }

        #endregion

        #region Mines
        public ToggleNode minesEnabled { get; set; }
        public RangeNode<int> minesDelay { get; set; }
        public RangeNode<int> minesMin { get; set; }
        public RangeNode<int> minesDetonateRange { get; set; }


        #endregion

        #region AutoAttack Cyclone / Nova etc.
        public ToggleNode autoAttackEnabled { get; set; }
        public ToggleNode autoAttackLeftMouseCheck { get; set; }
        public HotkeyNode autoAttackPickItKey { get; set; }
        public RangeNode<int> autoAttackRange { get; set; }


        #endregion

        #region Auto Golem
        public ToggleNode autoGolemEnabled { get; set; }
        public RangeNode<int> autoGolemChaosMax { get; set; }
        public RangeNode<int> autoGolemFireMax { get; set; }
        public RangeNode<int> autoGolemIceMax { get; set; }
        public RangeNode<int> autoGolemLightningMax { get; set; }
        public RangeNode<int> autoGolemRockMax { get; set; }
        public RangeNode<int> autoBoneMax { get; set; }
        public RangeNode<int> autoGolemDropBearMax { get; set; }


        #endregion

        #region Any Vaal Skill
        public ToggleNode anyVaalEnabled { get; set; }
        public RangeNode<int> anyVaalTriggerRange { get; set; }
        public RangeNode<int> anyVaalMinEnemys { get; set; }
        public RangeNode<float> anyVaalHpPct { get; set; }
        public RangeNode<float> anyVaalEsPct { get; set; }
        #endregion

        #region Custom Skill
        public ToggleNode customEnabled { get; set; }
        public RangeNode<int> customTriggerRange { get; set; }
        public RangeNode<int> customMinEnemys { get; set; }
        public RangeNode<float> customHpPct { get; set; }
        public RangeNode<float> customEsPct { get; set; }
        public RangeNode<int> customCooldown { get; set; }
        public HotkeyNode customKey { get; set; }
        #endregion

        #region Brand Recall
        public ToggleNode brandRecallEnabled { get; set; }
        public RangeNode<int> brandRecallTriggerRange { get; set; }
        public RangeNode<int> brandRecallMinEnemys { get; set; }
        public RangeNode<int> brandRecallCooldown { get; set; }
        public RangeNode<int> brandRecallMinBrands { get; set; }
        #endregion

        #region Input Keys
        [Menu("boundSkill2:")]
        public HotkeyNode InputKey1 { get; set; }

        [Menu("boundSkill4:")]
        public HotkeyNode InputKey3 { get; set; }

        [Menu("boundSkill5:")]
        public HotkeyNode InputKey4 { get; set; }

        [Menu("boundSkill6:")]
        public HotkeyNode InputKey5 { get; set; }

        [Menu("boundSkill7:")]
        public HotkeyNode InputKey6 { get; set; }

        [Menu("boundSkill8:")]
        public HotkeyNode InputKey7 { get; set; }

        [Menu("boundSkill9:")]
        public HotkeyNode InputKey8 { get; set; }

        [Menu("boundSkill10:")]
        public HotkeyNode InputKey9 { get; set; }

        [Menu("boundSkill11:")]
        public HotkeyNode InputKey10 { get; set; }

        [Menu("boundSkill12:")]
        public HotkeyNode InputKey11 { get; set; }

        [Menu("boundSkill13:")]
        public HotkeyNode InputKey12 { get; set; }
        #endregion

        #region Confirm
        public ToggleNode confirm1 { get; set; }
        public ToggleNode confirm2 { get; set; }
        public ToggleNode confirm3 { get; set; }
        public ToggleNode confirm4 { get; set; }
        public ToggleNode confirm5 { get; set; }
        #endregion

        public ToggleNode Enable { get; set; }
        public CoPilotSettings()
        {
            Enable = new ToggleNode(false);
            debugMode = new ToggleNode(false);

            autoQuitEnabled = new ToggleNode(false);
            hpPctQuit = new RangeNode<float>(35f, 0f, 100f);
            esPctQuit = new RangeNode<float>(0f, 0f, 100f);
            forcedAutoQuit = Keys.F4;

            phaserunEnabled = new ToggleNode(false);
            phaserunDelay = new RangeNode<int>(4000, 100, 4100);

            moltenShellEnabled = new ToggleNode(false);
            moltenShellDelay = new RangeNode<int>(4000, 100, 4100);           
            moltenShellRange = new RangeNode<int>(550, 100, 1000);

            enduringCryEnabled = new ToggleNode(false);
            warCryTriggerRange = new RangeNode<int>(550, 100, 1000);
            warCryCooldown = new RangeNode<int>(4000, 3330, 4100);
            warCryKeepRage = new ToggleNode(false);
            rallyingCryEnabled = new ToggleNode(false);

            divineIreEnabled = new ToggleNode(false);
            divineIreStacks = new RangeNode<int>(20, 1, 20);
            divineIreWaitForInfused = new ToggleNode(false);

            delveFlareEnabled = new ToggleNode(false);
            delveFlareKey = Keys.D6;
            delveFlareDebuffStacks = new RangeNode<int>(12, 1, 1000);
            delveFlareHpBelow = new RangeNode<float>(75, 0, 100);
            delveFlareEsBelow = new RangeNode<float>(0, 0, 100);

            vortexEnabled = new ToggleNode(false);
            vortexDelay = new RangeNode<int>(1800, 500, 1900);
            vortexRange = new RangeNode<int>(300, 100, 1000);

            bloodRageEnabled = new ToggleNode(false);
            bloodRageDelay = new RangeNode<int>(1000, 1000, 10100);
            bloodRageRange = new RangeNode<int>(300, 800, 3000);

            offeringsEnabled = new ToggleNode(false);
            offeringsMinEnemys = new RangeNode<int>(0, 0, 20);
            offeringsTriggerRange = new RangeNode<int>(600, 300, 3000);

            doedreEffigyEnabled = new ToggleNode(false);
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
            autoAttackPickItKey = new HotkeyNode(Keys.Space);
            autoAttackRange = new RangeNode<int>(500, 100, 2000);

            autoGolemEnabled = new ToggleNode(false);
            autoGolemChaosMax = new RangeNode<int>(0, 0, 15);
            autoGolemFireMax = new RangeNode<int>(0, 0, 15);
            autoGolemIceMax = new RangeNode<int>(0, 0, 15);
            autoGolemLightningMax = new RangeNode<int>(0, 0, 15);
            autoGolemRockMax = new RangeNode<int>(0, 0, 15);
            autoBoneMax = new RangeNode<int>(0, 0, 15);
            autoGolemDropBearMax = new RangeNode<int>(0, 0, 15);

            anyVaalEnabled = new ToggleNode(false);
            anyVaalTriggerRange = new RangeNode<int>(500, 100, 2000);
            anyVaalMinEnemys = new RangeNode<int>(0, 0, 50);
            anyVaalHpPct = new RangeNode<float>(35f, 0f, 100f);
            anyVaalEsPct = new RangeNode<float>(0f, 0f, 100f);

            customEnabled = new ToggleNode(false);
            customTriggerRange = new RangeNode<int>(500, 100, 2000);
            customMinEnemys = new RangeNode<int>(1, 0, 50);
            customHpPct = new RangeNode<float>(100, 0f, 100f);
            customEsPct = new RangeNode<float>(0f, 0f, 100f);
            customCooldown = new RangeNode<int>(4000, 1000, 10000);
            customKey = new HotkeyNode(Keys.T);

            brandRecallEnabled = new ToggleNode(false);
            brandRecallTriggerRange = new RangeNode<int>(500, 100, 2000);
            brandRecallMinEnemys = new RangeNode<int>(1, 0, 50);
            brandRecallCooldown = new RangeNode<int>(3100, 1000, 10000);
            brandRecallMinBrands = new RangeNode<int>(0, 0, 10);

            InputKey1 = new HotkeyNode(Keys.Z);
            InputKey3 = new HotkeyNode(Keys.Q);
            InputKey4 = new HotkeyNode(Keys.W);
            InputKey5 = new HotkeyNode(Keys.E);
            InputKey6 = new HotkeyNode(Keys.R);
            InputKey7 = new HotkeyNode(Keys.T);
            InputKey8 = new HotkeyNode(Keys.NumPad1);
            InputKey9 = new HotkeyNode(Keys.NumPad2);
            InputKey10 = new HotkeyNode(Keys.NumPad3);
            InputKey11 = new HotkeyNode(Keys.NumPad4);
            InputKey12= new HotkeyNode(Keys.NumPad5);

            confirm1 = new ToggleNode(false);
            confirm2 = new ToggleNode(false);
            confirm3 = new ToggleNode(false);
            confirm4 = new ToggleNode(false);
            confirm5 = new ToggleNode(false);


        }
    }
}
