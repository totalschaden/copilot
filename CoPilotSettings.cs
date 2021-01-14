using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using ExileCore.Shared.Attributes;

using System.Windows.Forms;

namespace CoPilot
{
    public class CoPilotSettings : ISettings
    {
        public ToggleNode debugMode = new ToggleNode(false);


        #region Auto Quit
        public ToggleNode autoQuitEnabled = new ToggleNode(false);
        public ToggleNode autoQuitHotkeyEnabled = new ToggleNode(true);
        public HotkeyNode forcedAutoQuit = new HotkeyNode(Keys.F4);
        public RangeNode<float> hpPctQuit = new RangeNode<float>(35f, 0f, 100f);
        public RangeNode<float> esPctQuit = new RangeNode<float>(0f, 0f, 100f);
        public ToggleNode autoQuitGuardian = new ToggleNode(false);
        public RangeNode<float> guardianHpPct = new RangeNode<float>(35f, 0f, 100f);

        #endregion

        #region Mirage Archer
        public ToggleNode mirageEnabled = new ToggleNode(false);
        public RangeNode<int> mirageRange = new RangeNode<int>(70, 0, 1000);
        #endregion

        #region Convocation
        public ToggleNode convocationEnabled = new ToggleNode(false);
        public RangeNode<float> convocationHp = new RangeNode<float>(35f, 0f, 100f);
        public RangeNode<int> convocationMobRange = new RangeNode<int>(300, 0, 4000);
        public RangeNode<int> convocationMinnionRange = new RangeNode<int>(300, 0, 4000);
        public RangeNode<int> convocationMinnionPct = new RangeNode<int>(50, 0, 100);
        public RangeNode<int> convocationCooldown = new RangeNode<int>(3100, 100, 3100);
        public RangeNode<int> convocationAvoidUniqueRange = new RangeNode<int>(1000, 100, 2000);
        #endregion

        #region Phaserun
        public ToggleNode phaserunEnabled = new ToggleNode(false);
        public RangeNode<int> phaserunDelay = new RangeNode<int>(4000, 100, 4100);
        #endregion

        #region Molten Shell
        public ToggleNode moltenShellEnabled = new ToggleNode(false);
        public RangeNode<int> moltenShellDelay = new RangeNode<int>(4000, 100, 4100);
        public RangeNode<int> moltenShellRange = new RangeNode<int>(550, 100, 1000);
        #endregion

        #region War Cry
        public ToggleNode enduringCryEnabled = new ToggleNode(false);
        public RangeNode<int> warCryCooldown = new RangeNode<int>(4000, 3330, 4100);
        public RangeNode<int> warCryTriggerRange = new RangeNode<int>(550, 100, 1000);
        public ToggleNode rallyingCryEnabled = new ToggleNode(false);
        public ToggleNode warCryKeepRage = new ToggleNode(false);
        #endregion

        #region Divine Ire / Blade Flurry / Scourge Arrow
        public ToggleNode divineIreEnabled = new ToggleNode(false);
        public RangeNode<int> divineIreStacks = new RangeNode<int>(20, 1, 20);
        public ToggleNode divineIreWaitForInfused = new ToggleNode(false);
        #endregion

        #region Delve Flare
        public ToggleNode delveFlareEnabled = new ToggleNode(false);
        public HotkeyNode delveFlareKey = new HotkeyNode(Keys.D6);
        public RangeNode<int> delveFlareDebuffStacks = new RangeNode<int>(12, 1, 1000);
        public RangeNode<float> delveFlareHpBelow = new RangeNode<float>(75, 0, 100);
        public RangeNode<float> delveFlareEsBelow = new RangeNode<float>(0, 0, 100);
        #endregion

        #region Vortex
        public ToggleNode vortexEnabled = new ToggleNode(false);
        public ToggleNode vortexFrostbolt = new ToggleNode(true);
        public RangeNode<int> vortexDelay = new RangeNode<int>(1800, 500, 1900);
        public RangeNode<int> vortexRange = new RangeNode<int>(300, 100, 1000);

        #endregion

        #region BloodRage
        public ToggleNode bloodRageEnabled = new ToggleNode(false);
        public RangeNode<int> bloodRageDelay = new RangeNode<int>(1000, 1000, 10100);
        public RangeNode<int> bloodRageRange = new RangeNode<int>(300, 800, 3000);
        #endregion

        #region Offerings
        public ToggleNode offeringsEnabled = new ToggleNode(false);
        public ToggleNode offeringsUseWhileCasting = new ToggleNode(true);
        public RangeNode<int> offeringsMinEnemys = new RangeNode<int>(0, 0, 20);
        public RangeNode<int> offeringsTriggerRange = new RangeNode<int>(600, 300, 3000);
        #endregion

        #region Doedre Effigy
        public ToggleNode doedreEffigyEnabled = new ToggleNode(false);
        public RangeNode<int> doedreEffigyDelay = new RangeNode<int>(600, 500, 2000);
        #endregion

        #region Mines
        public ToggleNode minesEnabled = new ToggleNode(false);
        public RangeNode<int> minesDelay = new RangeNode<int>(1000, 250, 3000);
        public RangeNode<int> minesMin = new RangeNode<int>(1, 1, 20);
        public RangeNode<int> minesDetonateRange = new RangeNode<int>(50, 10, 200);


        #endregion

        #region AutoAttack Cyclone / Nova etc.
        public ToggleNode autoAttackEnabled = new ToggleNode(false);
        public ToggleNode autoAttackLeftMouseCheck = new ToggleNode(true);
        public HotkeyNode autoAttackPickItKey = new HotkeyNode(Keys.Space);
        public RangeNode<int> autoAttackRange = new RangeNode<int>(500, 100, 2000);


        #endregion

        #region Auto Summon
        public ToggleNode autoSummonEnabled = new ToggleNode(false);
        public ToggleNode autoGolemEnabled = new ToggleNode(false);
        

        public RangeNode<int> autoGolemChaosMax = new RangeNode<int>(0, 0, 15);
        public RangeNode<int> autoGolemFireMax = new RangeNode<int>(0, 0, 15);
        public RangeNode<int> autoGolemIceMax = new RangeNode<int>(0, 0, 15);
        public RangeNode<int> autoGolemLightningMax = new RangeNode<int>(0, 0, 15);
        public RangeNode<int> autoGolemRockMax = new RangeNode<int>(0, 0, 15);
        public RangeNode<int> autoBoneMax = new RangeNode<int>(0, 0, 15);
        public RangeNode<int> autoGolemDropBearMax = new RangeNode<int>(0, 0, 15);
        public RangeNode<int> autoGolemAvoidRange = new RangeNode<int>(1000, 100, 2000);
        public ToggleNode autoZombieEnabled = new ToggleNode(false);
        #endregion

        #region Auto Curse
        public ToggleNode autoCurseEnabled = new ToggleNode(false);
        public RangeNode<int> autoCurseRange = new RangeNode<int>(300, 100, 1000);
        public RangeNode<int> autoCurseCooldown = new RangeNode<int>(1000, 100, 5000);
        public RangeNode<int> autoCurseMinEnemys = new RangeNode<int>(5, 1, 20);
        #endregion

        #region Blade Vortex
        public ToggleNode bladeVortex = new ToggleNode(false);
        public RangeNode<int> bladeVortexRange = new RangeNode<int>(300, 100, 1000);
        public RangeNode<int> bladeVortexCooldown = new RangeNode<int>(2750, 100, 5000);
        #endregion

        #region Blade Blast
        public ToggleNode bladeBlast = new ToggleNode(false);
        public RangeNode<int> bladeBlastEntityRange = new RangeNode<int>(300, 100, 1000);
        public RangeNode<int> bladeBlastCooldown = new RangeNode<int>(500, 100, 5000);
        #endregion

        #region Tempest Shield
        public ToggleNode tempestShieldEnabled = new ToggleNode(false);
        public RangeNode<int> tempestShieldTriggerRange = new RangeNode<int>(1200, 100, 1200);
        public RangeNode<int> tempestShieldMinEnemys = new RangeNode<int>(1, 0, 50);

        public ToggleNode tempestShieldUseWhileCasting = new ToggleNode(true);
        #endregion

        #region Any Vaal Skill
        public ToggleNode anyVaalEnabled = new ToggleNode(false);
        public RangeNode<int> anyVaalTriggerRange = new RangeNode<int>(500, 100, 2000);
        public RangeNode<int> anyVaalMinEnemys = new RangeNode<int>(0, 0, 50);
        public RangeNode<float> anyVaalHpPct = new RangeNode<float>(35f, 0f, 100f);
        public RangeNode<float> anyVaalEsPct = new RangeNode<float>(0f, 0f, 100f);
        #endregion

        #region Custom Skill
        public ToggleNode customEnabled = new ToggleNode(false);
        public RangeNode<int> customTriggerRange = new RangeNode<int>(500, 100, 2000);
        public RangeNode<int> customMinEnemys = new RangeNode<int>(1, 0, 50);
        public RangeNode<float> customHpPct = new RangeNode<float>(100, 0f, 100f);
        public RangeNode<float> customEsPct = new RangeNode<float>(0f, 0f, 100f);
        public RangeNode<int> customCooldown = new RangeNode<int>(4000, 1000, 10000);
        public HotkeyNode customKey = new HotkeyNode(Keys.T);
        #endregion

        #region Brand Recall
        public ToggleNode brandRecallEnabled = new ToggleNode(false);
        public RangeNode<int> brandRecallTriggerRange = new RangeNode<int>(500, 100, 2000);
        public RangeNode<int> brandRecallMinEnemys = new RangeNode<int>(1, 0, 50);
        public RangeNode<int> brandRecallCooldown = new RangeNode<int>(3100, 1000, 10000);
        public RangeNode<int> brandRecallMinBrands = new RangeNode<int>(0, 0, 10);
        #endregion

        #region Input Keys
        public HotkeyNode InputKey1 = new HotkeyNode(Keys.Z);
        public HotkeyNode InputKey3 = new HotkeyNode(Keys.Q);
        public HotkeyNode InputKey4 = new HotkeyNode(Keys.W);
        public HotkeyNode InputKey5 = new HotkeyNode(Keys.E);
        public HotkeyNode InputKey6 = new HotkeyNode(Keys.R);
        public HotkeyNode InputKey7 = new HotkeyNode(Keys.T);
        public HotkeyNode InputKey8 = new HotkeyNode(Keys.NumPad1);
        public HotkeyNode InputKey9 = new HotkeyNode(Keys.NumPad2);
        public HotkeyNode InputKey10 = new HotkeyNode(Keys.NumPad3);
        public HotkeyNode InputKey11 = new HotkeyNode(Keys.NumPad4);
        public HotkeyNode InputKey12 = new HotkeyNode(Keys.NumPad5);
        #endregion

        #region Confirm
        public ToggleNode confirm1 = new ToggleNode(false);
        public ToggleNode confirm2 = new ToggleNode(false);
        public ToggleNode confirm3 = new ToggleNode(false);
        public ToggleNode confirm4 = new ToggleNode(false);
        public ToggleNode confirm5 = new ToggleNode(false);
        #endregion

        public ToggleNode Enable { get; set; }
        public CoPilotSettings()
        {
            Enable = new ToggleNode(false);
        }
    }
}
