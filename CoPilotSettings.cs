using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using ExileCore.PoEMemory.FilesInMemory.Atlas;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
// ReSharper disable FieldCanBeMadeReadOnly.Global
// Need non readonly to save settings.

namespace CoPilot
{
    [SuppressMessage("Interoperability", "CA1416:Plattformkompatibilität überprüfen")]
    public class CoPilotSettings : ISettings
    {
        #region Auto Map Tabber

        public ToggleNode autoMapTabber = new ToggleNode(false);

        #endregion

        public ToggleNode debugMode = new ToggleNode(false);

        #region Doedre Effigy

        public ToggleNode doedreEffigyEnabled = new ToggleNode(false);

        #endregion

        public CoPilotSettings()
        {
            Enable = new ToggleNode(false);
        }

        public ToggleNode Enable { get; set; }

        #region AutoPilot
        
        public ToggleNode autoPilotEnabled = new ToggleNode(false);
        public ToggleNode autoPilotGrace = new ToggleNode(true);
        public TextNode autoPilotLeader = new TextNode("");
        public ToggleNode autoPilotDashEnabled = new ToggleNode(false);
        public ToggleNode autoPilotCloseFollow = new ToggleNode(true);
        public HotkeyNode autoPilotDashKey = new HotkeyNode(Keys.W);
        public HotkeyNode autoPilotMoveKey = new HotkeyNode(Keys.Q);
        public HotkeyNode autoPilotToggleKey = new HotkeyNode(Keys.NumPad9);
        public ToggleNode autoPilotTakeWaypoints = new ToggleNode(true);
        public RangeNode<int> autoPilotRandomClickOffset = new RangeNode<int>(10, 1, 100);
        public RangeNode<int> autoPilotInputFrequency = new RangeNode<int>(50, 1, 100);
        public RangeNode<int> autoPilotPathfindingNodeDistance = new RangeNode<int>(200, 10, 1000);
        public RangeNode<int> autoPilotClearPathDistance = new RangeNode<int>(500, 100, 5000);

        #endregion
        
        #region Auto Quit

        public ToggleNode autoQuitEnabled = new ToggleNode(false);
        public ToggleNode autoQuitHotkeyEnabled = new ToggleNode(true);
        public HotkeyNode forcedAutoQuit = new HotkeyNode(Keys.F4);
        public RangeNode<int> hppQuit = new RangeNode<int>(35, 0, 100);
        public RangeNode<int> espQuit = new RangeNode<int>(0, 0, 100);
        public ToggleNode autoQuitGuardian = new ToggleNode(false);
        public RangeNode<int> guardianHpp = new RangeNode<int>(35, 0, 100);

        #endregion

        #region CWDT

        public ToggleNode cwdtEnabled = new ToggleNode(false);
        

        #endregion

        #region Ranged Trigger

        public ToggleNode rangedTriggerEnabled = new ToggleNode(false);
        public ToggleNode rangedTriggerPowerCharge= new ToggleNode(false);
        public RangeNode<int> rangedTriggerMouseRange = new RangeNode<int>(70, 0, 1000);
        public RangeNode<int> rangedTriggerTargetRange = new RangeNode<int>(400, 0, 1000);
        public RangeNode<int> rangedTriggerCooldown = new RangeNode<int>(400, 0, 1000);

        #endregion

        #region Convocation

        public ToggleNode convocationEnabled = new ToggleNode(false);
        public RangeNode<int> convocationHpp = new RangeNode<int>(35, 0, 100);
        public RangeNode<int> convocationMobRange = new RangeNode<int>(300, 0, 4000);
        public RangeNode<int> convocationMinnionRange = new RangeNode<int>(300, 0, 4000);
        public RangeNode<int> convocationMinnionPct = new RangeNode<int>(50, 0, 100);
        public RangeNode<int> convocationAvoidUniqueRange = new RangeNode<int>(1000, 100, 2000);

        #endregion

        #region Phaserun

        public ToggleNode phaserunEnabled = new ToggleNode(false);
        public ToggleNode phaserunUseLifeTap = new ToggleNode(false);

        #endregion

        #region Molten Shell

        public ToggleNode moltenShellEnabled = new ToggleNode(false);
        public RangeNode<int> moltenShellRange = new RangeNode<int>(550, 100, 1000);
        public RangeNode<int> moltenShellHpp = new RangeNode<int>(100, 0, 100);
        public RangeNode<int> moltenShellEsp = new RangeNode<int>(0, 0, 100);
        public RangeNode<int> moltenShellMinAny = new RangeNode<int>(1, 0, 50);
        public RangeNode<int> moltenShellMinRare = new RangeNode<int>(0, 0, 50);
        public RangeNode<int> moltenShellMinUnique = new RangeNode<int>(0, 0, 50);

        #endregion

        #region Aura Blessing

        public ToggleNode auraBlessingEnabled = new ToggleNode(false);
        public ToggleNode auraBlessingWitheringStep = new ToggleNode(false);
        public RangeNode<int> auraBlessingRange = new RangeNode<int>(550, 100, 1000);
        public RangeNode<int> auraBlessingHpp = new RangeNode<int>(100, 0, 100);
        public RangeNode<int> auraBlessingEsp = new RangeNode<int>(0, 0, 100);
        public RangeNode<int> auraBlessingMinAny = new RangeNode<int>(1, 0, 50);
        public RangeNode<int> auraBlessingMinRare = new RangeNode<int>(0, 0, 50);
        public RangeNode<int> auraBlessingMinUnique = new RangeNode<int>(0, 0, 50);
        public TextNode auraBlessingName = new TextNode("");
        public TextNode auraBlessing = new TextNode("");

        #endregion

        #region Enduring Cry

        public ToggleNode enduringCryEnabled = new ToggleNode(false);
        public RangeNode<int> enduringCryTriggerRange = new RangeNode<int>(550, 100, 1000);
        public ToggleNode enduringCrySpam = new ToggleNode(false);
        public RangeNode<int> enduringCryHealEsp = new RangeNode<int>(0, 1, 100);
        public RangeNode<int> enduringCryHealHpp = new RangeNode<int>(90, 1, 100);
        public RangeNode<int> enduringCryMinAny = new RangeNode<int>(1, 0, 50);
        public RangeNode<int> enduringCryMinRare = new RangeNode<int>(0, 0, 50);
        public RangeNode<int> enduringCryMinUnique = new RangeNode<int>(0, 0, 50);

        #endregion
        
        #region Generals's Cry

        public ToggleNode generalCryEnabled = new ToggleNode(false);
        public RangeNode<int> generalCryCorpseTriggerRange = new RangeNode<int>(550, 100, 1000);
        public RangeNode<int> generalCryTriggerRange = new RangeNode<int>(550, 100, 1000);
        public RangeNode<int> generalCryMinCorpse = new RangeNode<int>(1, 0, 50);

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
        public RangeNode<int> delveFlareHppBelow = new RangeNode<int>(75, 0, 100);
        public RangeNode<int> delveFlareEspBelow = new RangeNode<int>(0, 0, 100);

        #endregion

        #region Vortex

        public ToggleNode vortexEnabled = new ToggleNode(false);
        public ToggleNode vortexFrostbolt = new ToggleNode(true);
        public RangeNode<int> vortexRange = new RangeNode<int>(300, 100, 1000);
        public RangeNode<int> vortexMinAny = new RangeNode<int>(1, 0, 50);
        public RangeNode<int> vortexMinRare = new RangeNode<int>(0, 0, 50);
        public RangeNode<int> vortexMinUnique = new RangeNode<int>(0, 0, 50);

        #endregion

        #region BloodRage

        public ToggleNode bloodRageEnabled = new ToggleNode(false);
        public RangeNode<int> bloodRageRange = new RangeNode<int>(300, 800, 3000);
        public RangeNode<int> bloodRageMinAny = new RangeNode<int>(1, 0, 50);
        public RangeNode<int> bloodRageMinRare = new RangeNode<int>(0, 0, 50);
        public RangeNode<int> bloodRageMinUnique = new RangeNode<int>(0, 0, 50);

        #endregion

        #region Offerings

        public ToggleNode offeringsEnabled = new ToggleNode(false);
        public ToggleNode offeringsUseWhileCasting = new ToggleNode(true);
        public RangeNode<int> offeringsTriggerRange = new RangeNode<int>(600, 300, 3000);
        public RangeNode<int> offeringsMinAny = new RangeNode<int>(1, 0, 50);
        public RangeNode<int> offeringsMinRare = new RangeNode<int>(0, 0, 50);
        public RangeNode<int> offeringsMinUnique = new RangeNode<int>(0, 0, 50);

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
        public RangeNode<int> autoAttackRange = new RangeNode<int>(500, 100, 2000);
        public ToggleNode autoAttackCurseCheck = new ToggleNode(false);

        #endregion

        #region Auto Summon

        public ToggleNode autoSummonEnabled = new ToggleNode(false);
        public ToggleNode autoGolemEnabled = new ToggleNode(false);
        public ToggleNode autoHolyRelictEnabled = new ToggleNode(false);


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

        #region Auto Toxic Rain Ballista

        public ToggleNode autoToxicRainBallistaEnabled = new ToggleNode(false);
        public RangeNode<int> autoToxicRainBallistaRange = new RangeNode<int>(300, 800, 3000);
        public RangeNode<int> autoToxicRainBallistaMax = new RangeNode<int>(0, 0, 15);
        public RangeNode<int> autoToxicRainBallistaMinAny = new RangeNode<int>(1, 0, 50);
        public RangeNode<int> autoToxicRainBallistaMinRare = new RangeNode<int>(0, 0, 50);
        public RangeNode<int> autoToxicRainBallistaUnique = new RangeNode<int>(0, 0, 50);

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
        public RangeNode<int> bladeVortexCount = new RangeNode<int>(10, 1, 10);

        #endregion

        #region Plague Bearer
        public ToggleNode plagueBearer = new ToggleNode(false);
        public RangeNode<int> plagueBearerRange = new RangeNode<int>(300, 100, 1000);
        public RangeNode<int> plagueBearerMinEnemys = new RangeNode<int>(5, 1, 20);
        #endregion

        #region Blade Blast

        public ToggleNode bladeBlast = new ToggleNode(false);
        public ToggleNode bladeBlastFastMode = new ToggleNode(false);
        public RangeNode<int> bladeBlastEntityRange = new RangeNode<int>(300, 100, 1000);

        #endregion

        #region Tempest Shield

        public ToggleNode tempestShieldEnabled = new ToggleNode(false);
        public RangeNode<int> tempestShieldTriggerRange = new RangeNode<int>(1200, 100, 1200);
        public RangeNode<int> tempestShieldMinAny = new RangeNode<int>(1, 0, 50);
        public RangeNode<int> tempestShieldMinRare = new RangeNode<int>(0, 0, 50);
        public RangeNode<int> tempestShieldMinUnique = new RangeNode<int>(0, 0, 50);
        public ToggleNode tempestShieldUseWhileCasting = new ToggleNode(true);

        #endregion

        #region Vaal Skills

        public ToggleNode anyVaalEnabled = new ToggleNode(false);
        public RangeNode<int> anyVaalHpp = new RangeNode<int>(35, 0, 100);
        public RangeNode<int> anyVaalEsp = new RangeNode<int>(0, 0, 100);
        public RangeNode<int> anyVaalMpp = new RangeNode<int>(0, 0, 100);
        public RangeNode<int> anyVaalTriggerRange = new RangeNode<int>(500, 100, 2000);
        public RangeNode<int> anyVaalMinAny = new RangeNode<int>(1, 0, 50);
        public RangeNode<int> anyVaalMinRare = new RangeNode<int>(0, 0, 50);
        public RangeNode<int> anyVaalMinUnique = new RangeNode<int>(0, 0, 50);
        
        public ToggleNode VaalClarityEnabled = new ToggleNode(false);
        public RangeNode<int> VaalClarityManaPct = new RangeNode<int>(0, 0, 100);
        
        #endregion

        #region Custom Skill

        public ToggleNode customEnabled = new ToggleNode(false);
        public RangeNode<int> customTriggerRange = new RangeNode<int>(500, 100, 2000);
        public RangeNode<int> customMinAny = new RangeNode<int>(1, 0, 50);
        public RangeNode<int> customMinRare = new RangeNode<int>(0, 0, 50);
        public RangeNode<int> customMinUnique = new RangeNode<int>(0, 0, 50);
        public RangeNode<int> customHpp = new RangeNode<int>(100, 0, 100);
        public RangeNode<int> customEsp = new RangeNode<int>(0, 0, 100);
        public RangeNode<int> customCooldown = new RangeNode<int>(4000, 1000, 10000);
        public HotkeyNode customKey = new HotkeyNode(Keys.T);

        #endregion

        #region Brand Recall

        public ToggleNode brandRecallEnabled = new ToggleNode(false);
        public RangeNode<int> brandRecallTriggerRange = new RangeNode<int>(500, 100, 2000);
        public RangeNode<int> brandRecallMinEnemys = new RangeNode<int>(1, 0, 50);
        public RangeNode<int> brandRecallMinBrands = new RangeNode<int>(0, 0, 10);

        #endregion

        #region Berserk

        public ToggleNode berserkEnabled = new ToggleNode(false);
        public RangeNode<int> berserkMinRage = new RangeNode<int>(0, 0, 50);
        public RangeNode<int> berserkRange = new RangeNode<int>(500, 100, 1000);
        public RangeNode<int> berserkMinAny = new RangeNode<int>(1, 0, 50);
        public RangeNode<int> berserkMinRare = new RangeNode<int>(0, 0, 50);
        public RangeNode<int> berserkMinUnique = new RangeNode<int>(0, 0, 50);

        #endregion

        #region Input Keys

        public HotkeyNode inputKey1 = new HotkeyNode(Keys.Z);
        public HotkeyNode inputKey3 = new HotkeyNode(Keys.Q);
        public HotkeyNode inputKey4 = new HotkeyNode(Keys.W);
        public HotkeyNode inputKey5 = new HotkeyNode(Keys.E);
        public HotkeyNode inputKey6 = new HotkeyNode(Keys.R);
        public HotkeyNode inputKey7 = new HotkeyNode(Keys.T);
        public HotkeyNode inputKey8 = new HotkeyNode(Keys.NumPad1);
        public HotkeyNode inputKey9 = new HotkeyNode(Keys.NumPad2);
        public HotkeyNode inputKey10 = new HotkeyNode(Keys.NumPad3);
        public HotkeyNode inputKey11 = new HotkeyNode(Keys.NumPad4);
        public HotkeyNode inputKey12 = new HotkeyNode(Keys.NumPad5);
        public HotkeyNode inputKeyPickIt = new HotkeyNode(Keys.Space);

        #endregion

        #region Confirm

        public ToggleNode confirm1 = new ToggleNode(false);
        public ToggleNode confirm2 = new ToggleNode(false);
        public ToggleNode confirm3 = new ToggleNode(false);
        public ToggleNode confirm4 = new ToggleNode(false);
        public ToggleNode confirm5 = new ToggleNode(false);

        #endregion
    }
}