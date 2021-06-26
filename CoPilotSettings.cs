using System.Windows.Forms;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;

namespace CoPilot
{
    public class CoPilotSettings : ISettings
    {
        #region Auto Map Tabber

        public readonly ToggleNode autoMapTabber = new ToggleNode(false);

        #endregion

        public readonly ToggleNode debugMode = new ToggleNode(false);

        #region Doedre Effigy

        public readonly ToggleNode doedreEffigyEnabled = new ToggleNode(false);

        #endregion

        public CoPilotSettings()
        {
            Enable = new ToggleNode(false);
        }

        public ToggleNode Enable { get; set; }


        #region Auto Quit

        public readonly ToggleNode autoQuitEnabled = new ToggleNode(false);
        public readonly ToggleNode autoQuitHotkeyEnabled = new ToggleNode(true);
        public readonly HotkeyNode forcedAutoQuit = new HotkeyNode(Keys.F4);
        public readonly RangeNode<float> hpPctQuit = new RangeNode<float>(35f, 0f, 100f);
        public readonly RangeNode<float> esPctQuit = new RangeNode<float>(0f, 0f, 100f);
        public readonly ToggleNode autoQuitGuardian = new ToggleNode(false);
        public readonly RangeNode<float> guardianHpPct = new RangeNode<float>(35f, 0f, 100f);

        #endregion

        #region Mirage Archer

        public readonly ToggleNode mirageEnabled = new ToggleNode(false);
        public readonly RangeNode<int> mirageRange = new RangeNode<int>(70, 0, 1000);

        #endregion

        #region Convocation

        public readonly ToggleNode convocationEnabled = new ToggleNode(false);
        public readonly RangeNode<float> convocationHp = new RangeNode<float>(35f, 0f, 100f);
        public readonly RangeNode<int> convocationMobRange = new RangeNode<int>(300, 0, 4000);
        public readonly RangeNode<int> convocationMinnionRange = new RangeNode<int>(300, 0, 4000);
        public readonly RangeNode<int> convocationMinnionPct = new RangeNode<int>(50, 0, 100);
        public readonly RangeNode<int> convocationAvoidUniqueRange = new RangeNode<int>(1000, 100, 2000);

        #endregion

        #region Phaserun

        public readonly ToggleNode phaserunEnabled = new ToggleNode(false);
        public readonly ToggleNode phaserunUseLifeTap = new ToggleNode(false);

        #endregion

        #region Molten Shell

        public readonly ToggleNode moltenShellEnabled = new ToggleNode(false);
        public readonly RangeNode<int> moltenShellRange = new RangeNode<int>(550, 100, 1000);
        public readonly RangeNode<float> moltenShellHpPct = new RangeNode<float>(100, 0f, 100f);
        public readonly RangeNode<float> moltenShellEsPct = new RangeNode<float>(0f, 0f, 100f);
        public readonly RangeNode<int> moltenShellMinAny = new RangeNode<int>(1, 0, 50);
        public readonly RangeNode<int> moltenShellMinRare = new RangeNode<int>(0, 0, 50);
        public readonly RangeNode<int> moltenShellMinUnique = new RangeNode<int>(0, 0, 50);

        #endregion

        #region Enduring Cry

        public readonly ToggleNode enduringCryEnabled = new ToggleNode(false);
        public readonly RangeNode<int> warCryTriggerRange = new RangeNode<int>(550, 100, 1000);
        public readonly ToggleNode warCryKeepRage = new ToggleNode(false);
        public readonly RangeNode<int> warCryMinAny = new RangeNode<int>(1, 0, 50);
        public readonly RangeNode<int> warCryMinRare = new RangeNode<int>(0, 0, 50);
        public readonly RangeNode<int> warCryMinUnique = new RangeNode<int>(0, 0, 50);

        #endregion

        #region Divine Ire / Blade Flurry / Scourge Arrow

        public readonly ToggleNode divineIreEnabled = new ToggleNode(false);
        public readonly RangeNode<int> divineIreStacks = new RangeNode<int>(20, 1, 20);
        public readonly ToggleNode divineIreWaitForInfused = new ToggleNode(false);

        #endregion

        #region Delve Flare

        public readonly ToggleNode delveFlareEnabled = new ToggleNode(false);
        public readonly HotkeyNode delveFlareKey = new HotkeyNode(Keys.D6);
        public readonly RangeNode<int> delveFlareDebuffStacks = new RangeNode<int>(12, 1, 1000);
        public readonly RangeNode<float> delveFlareHpBelow = new RangeNode<float>(75, 0, 100);
        public readonly RangeNode<float> delveFlareEsBelow = new RangeNode<float>(0, 0, 100);

        #endregion

        #region Vortex

        public readonly ToggleNode vortexEnabled = new ToggleNode(false);
        public readonly ToggleNode vortexFrostbolt = new ToggleNode(true);
        public readonly RangeNode<int> vortexRange = new RangeNode<int>(300, 100, 1000);
        public readonly RangeNode<int> vortexMinAny = new RangeNode<int>(1, 0, 50);
        public readonly RangeNode<int> vortexMinRare = new RangeNode<int>(0, 0, 50);
        public readonly RangeNode<int> vortexMinUnique = new RangeNode<int>(0, 0, 50);

        #endregion

        #region BloodRage

        public readonly ToggleNode bloodRageEnabled = new ToggleNode(false);
        public readonly RangeNode<int> bloodRageRange = new RangeNode<int>(300, 800, 3000);
        public readonly RangeNode<int> bloodRageMinAny = new RangeNode<int>(1, 0, 50);
        public readonly RangeNode<int> bloodRageMinRare = new RangeNode<int>(0, 0, 50);
        public readonly RangeNode<int> bloodRageMinUnique = new RangeNode<int>(0, 0, 50);

        #endregion

        #region Offerings

        public readonly ToggleNode offeringsEnabled = new ToggleNode(false);
        public readonly ToggleNode offeringsUseWhileCasting = new ToggleNode(true);
        public readonly RangeNode<int> offeringsTriggerRange = new RangeNode<int>(600, 300, 3000);
        public readonly RangeNode<int> offeringsMinAny = new RangeNode<int>(1, 0, 50);
        public readonly RangeNode<int> offeringsMinRare = new RangeNode<int>(0, 0, 50);
        public readonly RangeNode<int> offeringsMinUnique = new RangeNode<int>(0, 0, 50);

        #endregion

        #region Mines

        public readonly ToggleNode minesEnabled = new ToggleNode(false);
        public readonly RangeNode<int> minesDelay = new RangeNode<int>(1000, 250, 3000);
        public readonly RangeNode<int> minesMin = new RangeNode<int>(1, 1, 20);
        public readonly RangeNode<int> minesDetonateRange = new RangeNode<int>(50, 10, 200);

        #endregion

        #region AutoAttack Cyclone / Nova etc.

        public readonly ToggleNode autoAttackEnabled = new ToggleNode(false);
        public readonly ToggleNode autoAttackLeftMouseCheck = new ToggleNode(true);
        public readonly RangeNode<int> autoAttackRange = new RangeNode<int>(500, 100, 2000);
        public readonly ToggleNode autoAttackCurseCheck = new ToggleNode(false);

        #endregion

        #region Auto Summon

        public readonly ToggleNode autoSummonEnabled = new ToggleNode(false);
        public readonly ToggleNode autoGolemEnabled = new ToggleNode(false);
        public readonly ToggleNode autoHolyRelictEnabled = new ToggleNode(false);


        public readonly RangeNode<int> autoGolemChaosMax = new RangeNode<int>(0, 0, 15);
        public readonly RangeNode<int> autoGolemFireMax = new RangeNode<int>(0, 0, 15);
        public readonly RangeNode<int> autoGolemIceMax = new RangeNode<int>(0, 0, 15);
        public readonly RangeNode<int> autoGolemLightningMax = new RangeNode<int>(0, 0, 15);
        public readonly RangeNode<int> autoGolemRockMax = new RangeNode<int>(0, 0, 15);
        public readonly RangeNode<int> autoBoneMax = new RangeNode<int>(0, 0, 15);
        public readonly RangeNode<int> autoGolemDropBearMax = new RangeNode<int>(0, 0, 15);
        public readonly RangeNode<int> autoGolemAvoidRange = new RangeNode<int>(1000, 100, 2000);
        public readonly ToggleNode autoZombieEnabled = new ToggleNode(false);

        #endregion

        #region Auto Curse

        public readonly ToggleNode autoCurseEnabled = new ToggleNode(false);
        public readonly RangeNode<int> autoCurseRange = new RangeNode<int>(300, 100, 1000);
        public readonly RangeNode<int> autoCurseCooldown = new RangeNode<int>(1000, 100, 5000);
        public readonly RangeNode<int> autoCurseMinEnemys = new RangeNode<int>(5, 1, 20);

        #endregion

        #region Blade Vortex

        public readonly ToggleNode bladeVortex = new ToggleNode(false);
        public readonly RangeNode<int> bladeVortexRange = new RangeNode<int>(300, 100, 1000);
        public readonly RangeNode<int> bladeVortexCount = new RangeNode<int>(10, 1, 10);
        public readonly RangeNode<int> bladeVortexUnleashCount = new RangeNode<int>(3, 1, 4);

        #endregion

        #region Blade Blast

        public readonly ToggleNode bladeBlast = new ToggleNode(false);
        public readonly ToggleNode bladeBlastFastMode = new ToggleNode(false);
        public readonly RangeNode<int> bladeBlastEntityRange = new RangeNode<int>(300, 100, 1000);

        #endregion

        #region Tempest Shield

        public readonly ToggleNode tempestShieldEnabled = new ToggleNode(false);
        public readonly RangeNode<int> tempestShieldTriggerRange = new RangeNode<int>(1200, 100, 1200);
        public readonly RangeNode<int> tempestShieldMinAny = new RangeNode<int>(1, 0, 50);
        public readonly RangeNode<int> tempestShieldMinRare = new RangeNode<int>(0, 0, 50);
        public readonly RangeNode<int> tempestShieldMinUnique = new RangeNode<int>(0, 0, 50);
        public readonly ToggleNode tempestShieldUseWhileCasting = new ToggleNode(true);

        #endregion

        #region Any Vaal Skill

        public readonly ToggleNode anyVaalEnabled = new ToggleNode(false);
        public readonly RangeNode<float> anyVaalHpPct = new RangeNode<float>(35f, 0f, 100f);
        public readonly RangeNode<float> anyVaalEsPct = new RangeNode<float>(0f, 0f, 100f);
        public readonly RangeNode<int> anyVaalTriggerRange = new RangeNode<int>(500, 100, 2000);
        public readonly RangeNode<int> anyVaalMinAny = new RangeNode<int>(1, 0, 50);
        public readonly RangeNode<int> anyVaalMinRare = new RangeNode<int>(0, 0, 50);
        public readonly RangeNode<int> anyVaalMinUnique = new RangeNode<int>(0, 0, 50);

        #endregion

        #region Custom Skill

        public readonly ToggleNode customEnabled = new ToggleNode(false);
        public readonly RangeNode<int> customTriggerRange = new RangeNode<int>(500, 100, 2000);
        public readonly RangeNode<int> customMinAny = new RangeNode<int>(1, 0, 50);
        public readonly RangeNode<int> customMinRare = new RangeNode<int>(0, 0, 50);
        public readonly RangeNode<int> customMinUnique = new RangeNode<int>(0, 0, 50);
        public readonly RangeNode<float> customHpPct = new RangeNode<float>(100, 0f, 100f);
        public readonly RangeNode<float> customEsPct = new RangeNode<float>(0f, 0f, 100f);
        public readonly RangeNode<int> customCooldown = new RangeNode<int>(4000, 1000, 10000);
        public readonly HotkeyNode customKey = new HotkeyNode(Keys.T);

        #endregion

        #region Brand Recall

        public readonly ToggleNode brandRecallEnabled = new ToggleNode(false);
        public readonly RangeNode<int> brandRecallTriggerRange = new RangeNode<int>(500, 100, 2000);
        public readonly RangeNode<int> brandRecallMinEnemys = new RangeNode<int>(1, 0, 50);
        public readonly RangeNode<int> brandRecallMinBrands = new RangeNode<int>(0, 0, 10);

        #endregion

        #region Berserk

        public readonly ToggleNode berserkEnabled = new ToggleNode(false);
        public readonly RangeNode<int> berserkMinRage = new RangeNode<int>(0, 0, 50);
        public readonly RangeNode<int> berserkRange = new RangeNode<int>(500, 100, 1000);
        public readonly RangeNode<int> berserkMinAny = new RangeNode<int>(1, 0, 50);
        public readonly RangeNode<int> berserkMinRare = new RangeNode<int>(0, 0, 50);
        public readonly RangeNode<int> berserkMinUnique = new RangeNode<int>(0, 0, 50);

        #endregion

        #region Input Keys

        public readonly HotkeyNode inputKey1 = new HotkeyNode(Keys.Z);
        public readonly HotkeyNode inputKey3 = new HotkeyNode(Keys.Q);
        public readonly HotkeyNode inputKey4 = new HotkeyNode(Keys.W);
        public readonly HotkeyNode inputKey5 = new HotkeyNode(Keys.E);
        public readonly HotkeyNode inputKey6 = new HotkeyNode(Keys.R);
        public readonly HotkeyNode inputKey7 = new HotkeyNode(Keys.T);
        public readonly HotkeyNode inputKey8 = new HotkeyNode(Keys.NumPad1);
        public readonly HotkeyNode inputKey9 = new HotkeyNode(Keys.NumPad2);
        public readonly HotkeyNode inputKey10 = new HotkeyNode(Keys.NumPad3);
        public readonly HotkeyNode inputKey11 = new HotkeyNode(Keys.NumPad4);
        public readonly HotkeyNode inputKey12 = new HotkeyNode(Keys.NumPad5);
        public readonly HotkeyNode inputKeyPickIt = new HotkeyNode(Keys.Space);

        #endregion

        #region Confirm

        public readonly ToggleNode confirm1 = new ToggleNode(false);
        public readonly ToggleNode confirm2 = new ToggleNode(false);
        public readonly ToggleNode confirm3 = new ToggleNode(false);
        public readonly ToggleNode confirm4 = new ToggleNode(false);
        public readonly ToggleNode confirm5 = new ToggleNode(false);

        #endregion
    }
}