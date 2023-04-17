using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using ExileCore.PoEMemory.MemoryObjects;
using ImGuiNET;
using SharpDX;
using Vector2 = System.Numerics.Vector2;
using Vector4 = System.Numerics.Vector4;

namespace CoPilot
{
    [SuppressMessage("Interoperability", "CA1416:Plattformkompatibilität überprüfen")]
    internal class ImGuiDrawSettings
    {
        private static Vector4 _donationColorTarget = new Vector4(0.454f, 0.031f, 0.768f, 1f);
        private static Vector4 _donationColorCurrent = new Vector4(0.454f, 0.031f, 0.768f, 1f);
        private static void SetText(string pText)
        {
            var staThread = new Thread(
                delegate()
                {
                    // Use a fully qualified name for Clipboard otherwise it
                    // will end up calling itself.
                    Clipboard.SetText(pText);
                });
            staThread.SetApartmentState(ApartmentState.STA);
            staThread.Start();
            staThread.Join();
        }

        internal static void DrawImGuiSettings()
        {
            var green = new Vector4(0.102f, 0.388f, 0.106f, 1.000f);
            var red = new Vector4(0.388f, 0.102f, 0.102f, 1.000f);

        var collapsingHeaderFlags = ImGuiTreeNodeFlags.CollapsingHeader;
            ImGui.Text("Plugin by Totalschaden. https://github.com/totalschaden/copilot");

            try
            {
                // Input Keys
                ImGui.PushStyleColor(ImGuiCol.Header, green);
                ImGui.PushID(1000);
                if (ImGui.TreeNodeEx("Input Keys", collapsingHeaderFlags))
                {
                    CoPilot.Instance.Settings.inputKey1.Value = ImGuiExtension.HotkeySelector(
                        "Use bound skill 2 Key: " + CoPilot.Instance.Settings.inputKey1.Value,
                        CoPilot.Instance.Settings.inputKey1.Value);
                    CoPilot.Instance.Settings.inputKey3.Value = ImGuiExtension.HotkeySelector(
                        "Use bound skill 4 Key: " + CoPilot.Instance.Settings.inputKey3.Value,
                        CoPilot.Instance.Settings.inputKey3.Value);
                    CoPilot.Instance.Settings.inputKey4.Value = ImGuiExtension.HotkeySelector(
                        "Use bound skill 5 Key: " + CoPilot.Instance.Settings.inputKey4.Value,
                        CoPilot.Instance.Settings.inputKey4.Value);
                    CoPilot.Instance.Settings.inputKey5.Value = ImGuiExtension.HotkeySelector(
                        "Use bound skill 6 Key: " + CoPilot.Instance.Settings.inputKey5.Value,
                        CoPilot.Instance.Settings.inputKey5.Value);
                    CoPilot.Instance.Settings.inputKey6.Value = ImGuiExtension.HotkeySelector(
                        "Use bound skill 7 Key: " + CoPilot.Instance.Settings.inputKey6.Value,
                        CoPilot.Instance.Settings.inputKey6.Value);
                    CoPilot.Instance.Settings.inputKey7.Value = ImGuiExtension.HotkeySelector(
                        "Use bound skill 8 Key: " + CoPilot.Instance.Settings.inputKey7.Value,
                        CoPilot.Instance.Settings.inputKey7.Value);
                    CoPilot.Instance.Settings.inputKey8.Value = ImGuiExtension.HotkeySelector(
                        "Use bound skill 9 Key: " + CoPilot.Instance.Settings.inputKey8.Value,
                        CoPilot.Instance.Settings.inputKey8.Value);
                    CoPilot.Instance.Settings.inputKey9.Value = ImGuiExtension.HotkeySelector(
                        "Use bound skill 10 Key: " + CoPilot.Instance.Settings.inputKey9.Value,
                        CoPilot.Instance.Settings.inputKey9.Value);
                    CoPilot.Instance.Settings.inputKey10.Value = ImGuiExtension.HotkeySelector(
                        "Use bound skill 11 Key: " + CoPilot.Instance.Settings.inputKey10.Value,
                        CoPilot.Instance.Settings.inputKey10.Value);
                    CoPilot.Instance.Settings.inputKey11.Value = ImGuiExtension.HotkeySelector(
                        "Use bound skill 12 Key: " + CoPilot.Instance.Settings.inputKey11.Value,
                        CoPilot.Instance.Settings.inputKey11.Value);
                    CoPilot.Instance.Settings.inputKey12.Value = ImGuiExtension.HotkeySelector(
                        "Use bound skill 13 Key: " + CoPilot.Instance.Settings.inputKey12.Value,
                        CoPilot.Instance.Settings.inputKey12.Value);
                    CoPilot.Instance.Settings.inputKeyPickIt.Value = ImGuiExtension.HotkeySelector(
                        "PickIt Key: " + CoPilot.Instance.Settings.inputKeyPickIt.Value,
                        CoPilot.Instance.Settings.inputKeyPickIt.Value);
                }
            }
            catch (Exception e)
            {
                CoPilot.Instance.LogError(e.ToString());
            }

            try
            {
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.Instance.Settings.confirm5 ? green : red);
                ImGui.PushID(1001);
                if (ImGui.TreeNodeEx("Important Information", collapsingHeaderFlags))
                {
                    ImGui.Text(
                        "Go to Input Keys Tab, and set them According to your Ingame Settings -> Settings -> Input -> Use bound skill X");
                    ImGui.NewLine();
                    ImGui.Text(
                        "If your a noob dont make changes to -> Input Keys <- and change your Ingame Settings to the Keys that are predefined in the Plugin!");
                    ImGui.NewLine();
                    ImGui.Text("DO NOT ASSIGN MOUSE KEYS TO -> Input Keys <- in the Plugin !!!");
                    ImGui.NewLine();
                    ImGui.Text(
                        "The Top Left and Top Right Skill Slots are EXCLUDED!!! (Ingame bound skill 1 and 3) I recommend you use these for your Mouse.");
                    ImGui.NewLine();
                    ImGui.Text(
                        "The Plugin is currently forced to use Timers for Cooldowns as there is no Proper Skill Api for Ready/Cooldown.");
                    ImGui.NewLine();
                    ImGui.Text(
                        "I STRONGLY recommend that you add 80-100ms extra delay to your Skill Settings, so a Skill wont be skipped sometimes.");
                    ImGui.NewLine();
                    ImGui.Text("Unhappy with Cooldown Slider ? Set your own Value with STRG/CTRL + Mouseclick");
                    ImGui.NewLine();
                    ImGui.Text(
                        "Using Auto Attack and Cyclone for example? If you want to Cyclone yourself, put it on Right Mouse Slot, and have Cyclone in the Slot before that one.");
                    ImGui.NewLine();


                    CoPilot.Instance.Settings.confirm1.Value = ImGuiExtension.Checkbox("I did READ the text above.",
                        CoPilot.Instance.Settings.confirm1.Value);
                    if (!CoPilot.Instance.Settings.confirm1)
                        return;
                    CoPilot.Instance.Settings.confirm2.Value = ImGuiExtension.Checkbox(
                        "I did READ and UNDERSTAND the text above.", CoPilot.Instance.Settings.confirm2.Value);
                    if (!CoPilot.Instance.Settings.confirm2)
                        return;
                    CoPilot.Instance.Settings.confirm3.Value = ImGuiExtension.Checkbox(
                        "I just READ it again and understood it.", CoPilot.Instance.Settings.confirm3.Value);
                    if (!CoPilot.Instance.Settings.confirm3)
                        return;
                    CoPilot.Instance.Settings.confirm4.Value = ImGuiExtension.Checkbox(
                        "I did everything stated above and im ready to go.", CoPilot.Instance.Settings.confirm4.Value);
                    if (!CoPilot.Instance.Settings.confirm4)
                        return;
                    CoPilot.Instance.Settings.confirm5.Value =
                        ImGuiExtension.Checkbox("Let me use the Plugin already !!!",
                            CoPilot.Instance.Settings.confirm5.Value);
                    if (!CoPilot.Instance.Settings.confirm5)
                        return;
                }
            }
            catch (Exception e)
            {
                CoPilot.Instance.LogError(e.ToString());
            }

            if (!CoPilot.Instance.Settings.confirm5)
                return;

            try
            {
                // Donation
                // ReSharper disable CompareOfFloatsByEqualityOperator
                if (_donationColorCurrent.X == _donationColorTarget.X &&
                    _donationColorCurrent.Y == _donationColorTarget.Y &&
                    _donationColorCurrent.Z == _donationColorTarget.Z)
                    // ReSharper restore CompareOfFloatsByEqualityOperator
                {
                    _donationColorTarget = new Vector4(Helper.random.NextFloat(0, 1), Helper.random.NextFloat(0, 1),
                        Helper.random.NextFloat(0, 1), 1f);
                }
                else
                {
                    var deltaTime = SkillInfo._deltaTime / 1000;
                    
                    _donationColorCurrent.X = Helper.MoveTowards(_donationColorCurrent.X, _donationColorTarget.X, deltaTime);
                    _donationColorCurrent.Y = Helper.MoveTowards(_donationColorCurrent.Y, _donationColorTarget.Y, deltaTime);
                    _donationColorCurrent.Z = Helper.MoveTowards(_donationColorCurrent.Z, _donationColorTarget.Z, deltaTime);
                }
                ImGui.PushStyleColor(ImGuiCol.Header, _donationColorCurrent);
                ImGui.PushID(99999);
                if (ImGui.TreeNodeEx("Donation - Send some Snacks nom nom", collapsingHeaderFlags))
                {
                    ImGui.Text("Thanks to anyone who is considering this.");
                    if (ImGui.Button("Copy Amazon.de Wishlist URL")) SetText("https://www.amazon.de/hz/wishlist/ls/MZ543BDBC6PJ?ref_=wl_share");
                    if (ImGui.Button("Copy BTC Adress")) SetText("bc1qwjpdf9q3n94e88m3z398udjagach5u56txwpkh");
                    if (ImGui.Button("Copy ETH Adress")) SetText("0x78Af12D08B32f816dB9788C5Cf3122693143ed78");
                    if (ImGui.Button("Copy LTC Adress")) SetText("LXCoWiLS5ZKEzHb7yTpJ7AxJrU9QLhCyHR");
                    CoPilot.Instance.Settings.debugMode.Value = ImGuiExtension.Checkbox("Turn on Debug Mode",
                        CoPilot.Instance.Settings.debugMode.Value);
                }
            }
            catch (Exception e)
            {
                CoPilot.Instance.LogError(e.ToString());
            }

            try
            {
                // Auto Pilot
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.Instance.Settings.autoPilotEnabled ? green : red);
                ImGui.PushID(0);
                if (ImGui.TreeNodeEx("Auto Pilot", collapsingHeaderFlags))
                {
                    CoPilot.Instance.Settings.autoPilotEnabled.Value =
                        ImGuiExtension.Checkbox("Enabled", CoPilot.Instance.Settings.autoPilotEnabled.Value);
                    CoPilot.Instance.Settings.autoPilotGrace.Value =
                        ImGuiExtension.Checkbox("Remove Grace Period", CoPilot.Instance.Settings.autoPilotGrace.Value);
                    CoPilot.Instance.Settings.autoPilotLeader = ImGuiExtension.InputText("Leader Name: ", CoPilot.Instance.Settings.autoPilotLeader, 60, ImGuiInputTextFlags.None);
                    if (string.IsNullOrWhiteSpace(CoPilot.Instance.Settings.autoPilotLeader.Value))
                    {
                        // Show error message or set a default value
                        CoPilot.Instance.Settings.autoPilotLeader.Value = "DefaultLeader";
                    }
                    else
                    {
                        // Remove any invalid characters from the input
                        CoPilot.Instance.Settings.autoPilotLeader.Value = new string(CoPilot.Instance.Settings.autoPilotLeader.Value.Where(c => char.IsLetterOrDigit(c) || c == '_').ToArray());
                    }
                    CoPilot.Instance.Settings.autoPilotDashEnabled.Value = ImGuiExtension.Checkbox(
                        "Dash Enabled", CoPilot.Instance.Settings.autoPilotDashEnabled.Value);
                    CoPilot.Instance.Settings.autoPilotCloseFollow.Value = ImGuiExtension.Checkbox(
                        "Close Follow", CoPilot.Instance.Settings.autoPilotCloseFollow.Value);
                    CoPilot.Instance.Settings.autoPilotDashKey.Value = ImGuiExtension.HotkeySelector(
                        "Dash Key: " + CoPilot.Instance.Settings.autoPilotDashKey.Value, CoPilot.Instance.Settings.autoPilotDashKey);
                    CoPilot.Instance.Settings.autoPilotMoveKey.Value = ImGuiExtension.HotkeySelector(
                        "Move Key: " + CoPilot.Instance.Settings.autoPilotMoveKey.Value, CoPilot.Instance.Settings.autoPilotMoveKey);
                    CoPilot.Instance.Settings.autoPilotToggleKey.Value = ImGuiExtension.HotkeySelector(
                        "Toggle Key: " + CoPilot.Instance.Settings.autoPilotToggleKey.Value, CoPilot.Instance.Settings.autoPilotToggleKey);
                    CoPilot.Instance.Settings.autoPilotTakeWaypoints.Value = ImGuiExtension.Checkbox(
                        "Take Waypoints", CoPilot.Instance.Settings.autoPilotTakeWaypoints.Value);
                    /*
                    CoPilot.instance.Settings.autoPilotRandomClickOffset.Value =
                        ImGuiExtension.IntSlider("Random Click Offset", CoPilot.instance.Settings.autoPilotRandomClickOffset);
                    */
                    CoPilot.Instance.Settings.autoPilotInputFrequency.Value =
                        ImGuiExtension.IntSlider("Input Freq.", CoPilot.Instance.Settings.autoPilotInputFrequency);
                    CoPilot.Instance.Settings.autoPilotPathfindingNodeDistance.Value =
                        ImGuiExtension.IntSlider("Keep within Distance", CoPilot.Instance.Settings.autoPilotPathfindingNodeDistance);
                    CoPilot.Instance.Settings.autoPilotClearPathDistance.Value =
                        ImGuiExtension.IntSlider("Transition Distance", CoPilot.Instance.Settings.autoPilotClearPathDistance);
                }
            }
            catch (Exception e)
            {
                CoPilot.Instance.LogError(e.ToString());
            }
            
            try
            {
                // Auto Attack
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.Instance.Settings.autoAttackEnabled ? green : red);
                ImGui.PushID(1);
                if (ImGui.TreeNodeEx("Auto Attack with Cyclone / Nova / Flicker / Sweep", collapsingHeaderFlags))
                {
                    CoPilot.Instance.Settings.autoAttackEnabled.Value =
                        ImGuiExtension.Checkbox("Enabled", CoPilot.Instance.Settings.autoAttackEnabled.Value);
                    CoPilot.Instance.Settings.autoAttackLeftMouseCheck.Value = ImGuiExtension.Checkbox(
                        "Pause on Left Mouse Pressed", CoPilot.Instance.Settings.autoAttackLeftMouseCheck.Value);
                    CoPilot.Instance.Settings.autoAttackRange.Value =
                        ImGuiExtension.IntSlider("Range", CoPilot.Instance.Settings.autoAttackRange);
                    CoPilot.Instance.Settings.autoAttackCurseCheck.Value = ImGuiExtension.Checkbox(
                        "Line of Sight Check with Curse / RaiderExposeAura, Ignores Range Setting!",
                        CoPilot.Instance.Settings.autoAttackCurseCheck.Value);
                }
            }
            catch (Exception e)
            {
                CoPilot.Instance.LogError(e.ToString());
            }


            try
            {
                // Auto Golem
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.Instance.Settings.autoSummonEnabled ? green : red);
                ImGui.PushID(2);
                if (ImGui.TreeNodeEx("Auto Summon", collapsingHeaderFlags))
                {
                    CoPilot.Instance.Settings.autoSummonEnabled.Value = ImGuiExtension.Checkbox("Auto Summons",
                        CoPilot.Instance.Settings.autoSummonEnabled.Value);
                    CoPilot.Instance.Settings.autoGolemEnabled.Value = ImGuiExtension.Checkbox("Auto Golem",
                        CoPilot.Instance.Settings.autoGolemEnabled.Value);
                    CoPilot.Instance.Settings.autoHolyRelictEnabled.Value = ImGuiExtension.Checkbox("Auto Holy Relict",
                        CoPilot.Instance.Settings.autoHolyRelictEnabled.Value);
                    CoPilot.Instance.Settings.autoZombieEnabled.Value = ImGuiExtension.Checkbox("Auto Zombies",
                        CoPilot.Instance.Settings.autoZombieEnabled.Value);
                    CoPilot.Instance.Settings.autoGolemAvoidRange.Value =
                        ImGuiExtension.IntSlider("Do not use when Enemys within",
                            CoPilot.Instance.Settings.autoGolemAvoidRange);
                    CoPilot.Instance.Settings.autoGolemChaosMax.Value = ImGuiExtension.IntSlider("Chaos Golems max.",
                        CoPilot.Instance.Settings.autoGolemChaosMax);
                    CoPilot.Instance.Settings.autoGolemFireMax.Value = ImGuiExtension.IntSlider("Flame Golems max.",
                        CoPilot.Instance.Settings.autoGolemFireMax);
                    CoPilot.Instance.Settings.autoGolemIceMax.Value = ImGuiExtension.IntSlider("Ice Golems max.",
                        CoPilot.Instance.Settings.autoGolemIceMax);
                    CoPilot.Instance.Settings.autoGolemLightningMax.Value =
                        ImGuiExtension.IntSlider("Lightning Golems max.",
                            CoPilot.Instance.Settings.autoGolemLightningMax);
                    CoPilot.Instance.Settings.autoGolemRockMax.Value = ImGuiExtension.IntSlider("Stone Golems max.",
                        CoPilot.Instance.Settings.autoGolemRockMax);
                    CoPilot.Instance.Settings.autoBoneMax.Value = ImGuiExtension.IntSlider("Carrion Golems max.",
                        CoPilot.Instance.Settings.autoBoneMax);
                    CoPilot.Instance.Settings.autoGolemDropBearMax.Value =
                        ImGuiExtension.IntSlider("Beastial Ursa Max.", CoPilot.Instance.Settings.autoGolemDropBearMax);
                }
            }
            catch (Exception e)
            {
                CoPilot.Instance.LogError(e.ToString());
            }


            try
            {
                // Auto Quit
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.Instance.Settings.autoQuitEnabled ? green : red);
                ImGui.PushID(3);
                if (ImGui.TreeNodeEx("Auto Quit (This requires HUD started as Admin !)", collapsingHeaderFlags))
                {
                    CoPilot.Instance.Settings.autoQuitEnabled.Value =
                        ImGuiExtension.Checkbox("Enabled", CoPilot.Instance.Settings.autoQuitEnabled.Value);
                    CoPilot.Instance.Settings.hppQuit.Value =
                        ImGuiExtension.IntSlider("HP%", CoPilot.Instance.Settings.hppQuit);
                    CoPilot.Instance.Settings.espQuit.Value =
                        ImGuiExtension.IntSlider("ES%", CoPilot.Instance.Settings.espQuit);
                    CoPilot.Instance.Settings.autoQuitGuardian.Value = ImGuiExtension.Checkbox("Guardian Auto Quit",
                        CoPilot.Instance.Settings.autoQuitGuardian.Value);
                    CoPilot.Instance.Settings.guardianHpp.Value =
                        ImGuiExtension.IntSlider("Guardian HP%", CoPilot.Instance.Settings.guardianHpp);
                    CoPilot.Instance.Settings.autoQuitHotkeyEnabled.Value = ImGuiExtension.Checkbox("Hotkey Enabled",
                        CoPilot.Instance.Settings.autoQuitHotkeyEnabled.Value);
                    CoPilot.Instance.Settings.forcedAutoQuit.Value = ImGuiExtension.HotkeySelector(
                        "Force Quit Hotkey: " + CoPilot.Instance.Settings.forcedAutoQuit.Value,
                        CoPilot.Instance.Settings.forcedAutoQuit.Value);
                }
            }
            catch (Exception e)
            {
                CoPilot.Instance.LogError(e.ToString());
            }


            try
            {
                // Blood Rage
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.Instance.Settings.bloodRageEnabled ? green : red);
                ImGui.PushID(4);
                if (ImGui.TreeNodeEx("Blood Rage", collapsingHeaderFlags))
                {
                    CoPilot.Instance.Settings.bloodRageEnabled.Value =
                        ImGuiExtension.Checkbox("Enabled", CoPilot.Instance.Settings.bloodRageEnabled.Value);
                    CoPilot.Instance.Settings.bloodRageRange.Value =
                        ImGuiExtension.IntSlider("Range", CoPilot.Instance.Settings.bloodRageRange);
                    CoPilot.Instance.Settings.bloodRageMinAny.Value =
                        ImGuiExtension.IntSlider("min Enemy Any", CoPilot.Instance.Settings.bloodRageMinAny);
                    CoPilot.Instance.Settings.bloodRageMinRare.Value =
                        ImGuiExtension.IntSlider("min Enemy Rare", CoPilot.Instance.Settings.bloodRageMinRare);
                    CoPilot.Instance.Settings.bloodRageMinUnique.Value = ImGuiExtension.IntSlider("min Enemy Unique",
                        CoPilot.Instance.Settings.bloodRageMinUnique);
                }
            }
            catch (Exception e)
            {
                CoPilot.Instance.LogError(e.ToString());
            }


            try
            {
                // Auto Toxic Rain Ballista
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.Instance.Settings.autoToxicRainBallistaEnabled ? green : red);
                ImGui.PushID(4);
                if (ImGui.TreeNodeEx("Auto Toxic Rain Ballista", collapsingHeaderFlags))
                {
                    CoPilot.Instance.Settings.autoToxicRainBallistaEnabled.Value =
                        ImGuiExtension.Checkbox("Enabled", CoPilot.Instance.Settings.autoToxicRainBallistaEnabled.Value);
                    CoPilot.Instance.Settings.autoToxicRainBallistaRange.Value =
                        ImGuiExtension.IntSlider("Range", CoPilot.Instance.Settings.autoToxicRainBallistaRange);
                    CoPilot.Instance.Settings.autoToxicRainBallistaMax.Value = ImGuiExtension.IntSlider("Toxic Rain Ballistas max.",
                        CoPilot.Instance.Settings.autoToxicRainBallistaMax);
                    CoPilot.Instance.Settings.autoToxicRainBallistaMinAny.Value =
                        ImGuiExtension.IntSlider("min Enemy Any", CoPilot.Instance.Settings.autoToxicRainBallistaMinAny);
                    CoPilot.Instance.Settings.autoToxicRainBallistaMinRare.Value =
                        ImGuiExtension.IntSlider("min Enemy Rare", CoPilot.Instance.Settings.autoToxicRainBallistaMinRare);
                    CoPilot.Instance.Settings.autoToxicRainBallistaUnique.Value = ImGuiExtension.IntSlider("min Enemy Unique",
                        CoPilot.Instance.Settings.autoToxicRainBallistaUnique);
                }
            }
            catch (Exception e)
            {
                CoPilot.Instance.LogError(e.ToString());
            }


            try
            {
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.Instance.Settings.delveFlareEnabled ? green : red);
                ImGui.PushID(5);
                if (ImGui.TreeNodeEx("Delve Flare", collapsingHeaderFlags))
                {
                    CoPilot.Instance.Settings.delveFlareEnabled.Value =
                        ImGuiExtension.Checkbox("Enabled", CoPilot.Instance.Settings.delveFlareEnabled);
                    CoPilot.Instance.Settings.delveFlareKey.Value = ImGuiExtension.HotkeySelector(
                        "Key: " + CoPilot.Instance.Settings.delveFlareKey.Value,
                        CoPilot.Instance.Settings.delveFlareKey.Value);
                    CoPilot.Instance.Settings.delveFlareDebuffStacks.Value =
                        ImGuiExtension.IntSlider("min. Debuff Stacks",
                            CoPilot.Instance.Settings.delveFlareDebuffStacks);
                    CoPilot.Instance.Settings.delveFlareHppBelow.Value =
                        ImGuiExtension.IntSlider("HP%", CoPilot.Instance.Settings.delveFlareHppBelow);
                    CoPilot.Instance.Settings.delveFlareEspBelow.Value =
                        ImGuiExtension.IntSlider("ES%", CoPilot.Instance.Settings.delveFlareEspBelow);
                }
            }
            catch (Exception e)
            {
                CoPilot.Instance.LogError(e.ToString());
            }


            try
            {
                // Doedre Effigy
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.Instance.Settings.doedreEffigyEnabled ? green : red);
                ImGui.PushID(6);
                if (ImGui.TreeNodeEx("Doedre Effigy", collapsingHeaderFlags))
                    CoPilot.Instance.Settings.doedreEffigyEnabled.Value = ImGuiExtension.Checkbox("Enabled",
                        CoPilot.Instance.Settings.doedreEffigyEnabled.Value);
            }
            catch (Exception e)
            {
                CoPilot.Instance.LogError(e.ToString());
            }


            try
            {
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.Instance.Settings.divineIreEnabled ? green : red);
                ImGui.PushID(7);
                if (ImGui.TreeNodeEx("Divine Ire / Blade Flurry / Scourge Arrow (Do NOT bind on Mousekeys!!!)",
                    collapsingHeaderFlags))
                {
                    CoPilot.Instance.Settings.divineIreEnabled.Value =
                        ImGuiExtension.Checkbox("Enabled", CoPilot.Instance.Settings.divineIreEnabled.Value);
                    CoPilot.Instance.Settings.divineIreStacks.Value =
                        ImGuiExtension.IntSlider("Stacks", CoPilot.Instance.Settings.divineIreStacks);
                    CoPilot.Instance.Settings.divineIreWaitForInfused.Value = ImGuiExtension.Checkbox(
                        "Wait for Infused Channeling Support", CoPilot.Instance.Settings.divineIreWaitForInfused.Value);
                }
            }
            catch (Exception e)
            {
                CoPilot.Instance.LogError(e.ToString());
            }


            try
            {
                // Enduring Cry
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.Instance.Settings.enduringCryEnabled ? green : red);
                ImGui.PushID(8);
                if (ImGui.TreeNodeEx("Enduring Cry", collapsingHeaderFlags))
                {
                    CoPilot.Instance.Settings.enduringCryEnabled.Value = ImGuiExtension.Checkbox("Enabled",
                        CoPilot.Instance.Settings.enduringCryEnabled.Value);
                    CoPilot.Instance.Settings.enduringCryTriggerRange.Value = ImGuiExtension.IntSlider("Enemy within Range",
                        CoPilot.Instance.Settings.enduringCryTriggerRange);
                    CoPilot.Instance.Settings.enduringCryHealHpp.Value = ImGuiExtension.IntSlider("Heal HP%",
                        CoPilot.Instance.Settings.enduringCryHealHpp);
                    CoPilot.Instance.Settings.enduringCryHealEsp.Value = ImGuiExtension.IntSlider("Heal ES%",
                        CoPilot.Instance.Settings.enduringCryHealEsp);
                    CoPilot.Instance.Settings.enduringCrySpam.Value = ImGuiExtension.Checkbox("Spam Warcry",
                        CoPilot.Instance.Settings.enduringCrySpam.Value);
                    CoPilot.Instance.Settings.enduringCryMinAny.Value =
                        ImGuiExtension.IntSlider("min Enemy Any", CoPilot.Instance.Settings.enduringCryMinAny);
                    CoPilot.Instance.Settings.enduringCryMinRare.Value =
                        ImGuiExtension.IntSlider("min Enemy Rare", CoPilot.Instance.Settings.enduringCryMinRare);
                    CoPilot.Instance.Settings.enduringCryMinUnique.Value = ImGuiExtension.IntSlider("min Enemy Unique",
                        CoPilot.Instance.Settings.enduringCryMinUnique);
                }
            }
            catch (Exception e)
            {
                CoPilot.Instance.LogError(e.ToString());
            }


            try
            {
                // Molten Shell / Steelskin / Bone Armour / Arcane Cloak
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.Instance.Settings.moltenShellEnabled ? green : red);
                ImGui.PushID(9);
                if (ImGui.TreeNodeEx("Molten Shell / Steelskin / Bone Armour / Arcane Cloak", collapsingHeaderFlags))
                {
                    CoPilot.Instance.Settings.moltenShellEnabled.Value = ImGuiExtension.Checkbox("Enabled",
                        CoPilot.Instance.Settings.moltenShellEnabled.Value);
                    CoPilot.Instance.Settings.moltenShellHpp.Value =
                        ImGuiExtension.IntSlider("HP%", CoPilot.Instance.Settings.moltenShellHpp);
                    CoPilot.Instance.Settings.moltenShellEsp.Value =
                        ImGuiExtension.IntSlider("ES%", CoPilot.Instance.Settings.moltenShellEsp);
                    CoPilot.Instance.Settings.moltenShellRange.Value =
                        ImGuiExtension.IntSlider("Range", CoPilot.Instance.Settings.moltenShellRange);
                    CoPilot.Instance.Settings.moltenShellMinAny.Value =
                        ImGuiExtension.IntSlider("min Enemy Any", CoPilot.Instance.Settings.moltenShellMinAny);
                    CoPilot.Instance.Settings.moltenShellMinRare.Value = ImGuiExtension.IntSlider("min Enemy Rare",
                        CoPilot.Instance.Settings.moltenShellMinRare);
                    CoPilot.Instance.Settings.moltenShellMinUnique.Value = ImGuiExtension.IntSlider("min Enemy Unique",
                        CoPilot.Instance.Settings.moltenShellMinUnique);
                }
            }
            catch (Exception e)
            {
                CoPilot.Instance.LogError(e.ToString());
            }
            
            
            try
            {
                // Aura Blessing
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.Instance.Settings.auraBlessingEnabled ? green : red);
                ImGui.PushID(9);
                if (ImGui.TreeNodeEx("Aura Blessing", collapsingHeaderFlags))
                {
                    CoPilot.Instance.Settings.auraBlessingEnabled.Value = ImGuiExtension.Checkbox("Enabled",
                        CoPilot.Instance.Settings.auraBlessingEnabled.Value);
                    CoPilot.Instance.Settings.auraBlessingWitheringStep.Value = 
                        ImGuiExtension.Checkbox("Do Not Override Withering Step", CoPilot.Instance.Settings.auraBlessingWitheringStep.Value);
                    CoPilot.Instance.Settings.auraBlessingHpp.Value =
                        ImGuiExtension.IntSlider("HP%", CoPilot.Instance.Settings.auraBlessingHpp);
                    CoPilot.Instance.Settings.auraBlessingEsp.Value =
                        ImGuiExtension.IntSlider("ES%", CoPilot.Instance.Settings.auraBlessingEsp);
                    CoPilot.Instance.Settings.auraBlessingRange.Value =
                        ImGuiExtension.IntSlider("Range", CoPilot.Instance.Settings.auraBlessingRange);
                    CoPilot.Instance.Settings.auraBlessingMinAny.Value =
                        ImGuiExtension.IntSlider("min Enemy Any", CoPilot.Instance.Settings.auraBlessingMinAny);
                    CoPilot.Instance.Settings.auraBlessingMinRare.Value = ImGuiExtension.IntSlider("min Enemy Rare",
                        CoPilot.Instance.Settings.auraBlessingMinRare);
                    CoPilot.Instance.Settings.auraBlessingMinUnique.Value = ImGuiExtension.IntSlider("min Enemy Unique",
                        CoPilot.Instance.Settings.auraBlessingMinUnique);
                }
            }
            catch (Exception e)
            {
                CoPilot.Instance.LogError(e.ToString());
            }


            try
            {
                // Offerings
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.Instance.Settings.offeringsEnabled ? green : red);
                ImGui.PushID(10);
                if (ImGui.TreeNodeEx("Offerings (This will get you stuck in Animation for your Casttime !)",
                    collapsingHeaderFlags))
                {
                    CoPilot.Instance.Settings.offeringsEnabled.Value =
                        ImGuiExtension.Checkbox("Enabled", CoPilot.Instance.Settings.offeringsEnabled.Value);
                    CoPilot.Instance.Settings.offeringsUseWhileCasting.Value = ImGuiExtension.Checkbox(
                        "Use while Casting/Attacking", CoPilot.Instance.Settings.offeringsUseWhileCasting.Value);
                    CoPilot.Instance.Settings.offeringsTriggerRange.Value = ImGuiExtension.IntSlider("Trigger Range",
                        CoPilot.Instance.Settings.offeringsTriggerRange);
                    CoPilot.Instance.Settings.offeringsMinAny.Value =
                        ImGuiExtension.IntSlider("min Enemy Any", CoPilot.Instance.Settings.offeringsMinAny);
                    CoPilot.Instance.Settings.offeringsMinRare.Value =
                        ImGuiExtension.IntSlider("min Enemy Rare", CoPilot.Instance.Settings.offeringsMinRare);
                    CoPilot.Instance.Settings.offeringsMinUnique.Value = ImGuiExtension.IntSlider("min Enemy Unique",
                        CoPilot.Instance.Settings.offeringsMinUnique);
                }
            }
            catch (Exception e)
            {
                CoPilot.Instance.LogError(e.ToString());
            }


            try
            {
                // Phaserun
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.Instance.Settings.phaserunEnabled ? green : red);
                ImGui.PushID(11);
                if (ImGui.TreeNodeEx("Phaserun / WitherStep", collapsingHeaderFlags))
                {
                    CoPilot.Instance.Settings.phaserunEnabled.Value =
                        ImGuiExtension.Checkbox("Enabled", CoPilot.Instance.Settings.phaserunEnabled.Value);
                    CoPilot.Instance.Settings.phaserunUseLifeTap.Value = ImGuiExtension.Checkbox("LifeTap Mode",
                        CoPilot.Instance.Settings.phaserunUseLifeTap.Value);
                }
            }
            catch (Exception e)
            {
                CoPilot.Instance.LogError(e.ToString());
            }

            try
            {
                // Vortex
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.Instance.Settings.vortexEnabled ? green : red);
                ImGui.PushID(13);
                if (ImGui.TreeNodeEx("Vortex", collapsingHeaderFlags))
                {
                    CoPilot.Instance.Settings.vortexEnabled.Value =
                        ImGuiExtension.Checkbox("Enabled", CoPilot.Instance.Settings.vortexEnabled.Value);
                    CoPilot.Instance.Settings.vortexFrostbolt.Value = ImGuiExtension.Checkbox("When Frostbolt's flying",
                        CoPilot.Instance.Settings.vortexFrostbolt.Value);
                    CoPilot.Instance.Settings.vortexRange.Value =
                        ImGuiExtension.IntSlider("Range", CoPilot.Instance.Settings.vortexRange);
                    CoPilot.Instance.Settings.vortexMinAny.Value =
                        ImGuiExtension.IntSlider("min Enemy Any", CoPilot.Instance.Settings.vortexMinAny);
                    CoPilot.Instance.Settings.vortexMinRare.Value =
                        ImGuiExtension.IntSlider("min Enemy Rare", CoPilot.Instance.Settings.vortexMinRare);
                    CoPilot.Instance.Settings.vortexMinUnique.Value = ImGuiExtension.IntSlider("min Enemy Unique",
                        CoPilot.Instance.Settings.vortexMinUnique);
                }
            }
            catch (Exception e)
            {
                CoPilot.Instance.LogError(e.ToString());
            }

            try
            {
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.Instance.Settings.anyVaalEnabled || CoPilot.Instance.Settings.VaalClarityEnabled ? green : red);
                ImGui.PushID(14);
                if (ImGui.TreeNodeEx("Vaal Skills", collapsingHeaderFlags))
                {
                    ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.Instance.Settings.anyVaalEnabled ? green : red);
                    ImGui.PushID(014);
                    ImGui.BeginChild(14, new Vector2(0,280));
                    if (ImGui.TreeNodeEx("Any", collapsingHeaderFlags))
                    {
                        CoPilot.Instance.Settings.anyVaalEnabled.Value =
                            ImGuiExtension.Checkbox("Enabled", CoPilot.Instance.Settings.anyVaalEnabled.Value);
                        CoPilot.Instance.Settings.anyVaalTriggerRange.Value = ImGuiExtension.IntSlider("Trigger Range",
                            CoPilot.Instance.Settings.anyVaalTriggerRange);
                        CoPilot.Instance.Settings.anyVaalHpp.Value =
                            ImGuiExtension.IntSlider("HP%", CoPilot.Instance.Settings.anyVaalHpp);
                        CoPilot.Instance.Settings.anyVaalEsp.Value =
                            ImGuiExtension.IntSlider("ES%", CoPilot.Instance.Settings.anyVaalEsp);
                        CoPilot.Instance.Settings.anyVaalMpp.Value =
                            ImGuiExtension.IntSlider("MP%", CoPilot.Instance.Settings.anyVaalMpp);
                        CoPilot.Instance.Settings.anyVaalMinAny.Value =
                            ImGuiExtension.IntSlider("min Enemy Any", CoPilot.Instance.Settings.anyVaalMinAny);
                        CoPilot.Instance.Settings.anyVaalMinRare.Value =
                            ImGuiExtension.IntSlider("min Enemy Rare", CoPilot.Instance.Settings.anyVaalMinRare);
                        CoPilot.Instance.Settings.anyVaalMinUnique.Value = ImGuiExtension.IntSlider("min Enemy Unique",
                            CoPilot.Instance.Settings.anyVaalMinUnique);
                    }
                    
                    ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.Instance.Settings.VaalClarityEnabled ? green : red);
                    ImGui.PushID(0014);
                    if (ImGui.TreeNodeEx("Vaal Clarity Test not active !", collapsingHeaderFlags))
                    {
                        CoPilot.Instance.Settings.VaalClarityEnabled.Value =
                            ImGuiExtension.Checkbox("Enabled", CoPilot.Instance.Settings.VaalClarityEnabled.Value);
                        CoPilot.Instance.Settings.VaalClarityManaPct.Value =
                            ImGuiExtension.IntSlider("MP%", CoPilot.Instance.Settings.VaalClarityManaPct);
                    }
                    ImGui.EndChild();
                }
            }
            catch (Exception e)
            {
                CoPilot.Instance.LogError(e.ToString());
            }

            try
            {
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.Instance.Settings.customEnabled ? green : red);
                ImGui.PushID(15);
                if (ImGui.TreeNodeEx("Custom Skill (Use any Skill not Supported here.)", collapsingHeaderFlags))
                {
                    CoPilot.Instance.Settings.customEnabled.Value =
                        ImGuiExtension.Checkbox("Enabled", CoPilot.Instance.Settings.customEnabled.Value);
                    CoPilot.Instance.Settings.customKey.Value = ImGuiExtension.HotkeySelector(
                        "Key: " + CoPilot.Instance.Settings.customKey.Value, CoPilot.Instance.Settings.customKey);
                    CoPilot.Instance.Settings.customCooldown.Value =
                        ImGuiExtension.IntSlider("Cooldown", CoPilot.Instance.Settings.customCooldown);
                    CoPilot.Instance.Settings.customTriggerRange.Value = ImGuiExtension.IntSlider("Trigger Range",
                        CoPilot.Instance.Settings.customTriggerRange);
                    CoPilot.Instance.Settings.customHpp.Value =
                        ImGuiExtension.IntSlider("HP%", CoPilot.Instance.Settings.customHpp);
                    CoPilot.Instance.Settings.customEsp.Value =
                        ImGuiExtension.IntSlider("ES%", CoPilot.Instance.Settings.customEsp);
                    CoPilot.Instance.Settings.customMinAny.Value =
                        ImGuiExtension.IntSlider("min Enemy Any", CoPilot.Instance.Settings.customMinAny);
                    CoPilot.Instance.Settings.customMinRare.Value =
                        ImGuiExtension.IntSlider("min Enemy Rare", CoPilot.Instance.Settings.customMinRare);
                    CoPilot.Instance.Settings.customMinUnique.Value = ImGuiExtension.IntSlider("min Enemy Unique",
                        CoPilot.Instance.Settings.customMinUnique);
                }
            }
            catch (Exception e)
            {
                CoPilot.Instance.LogError(e.ToString());
            }

            try
            {
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.Instance.Settings.brandRecallEnabled ? green : red);
                ImGui.PushID(16);
                if (ImGui.TreeNodeEx("Brand Recall", collapsingHeaderFlags))
                {
                    CoPilot.Instance.Settings.brandRecallEnabled.Value = ImGuiExtension.Checkbox("Enabled",
                        CoPilot.Instance.Settings.brandRecallEnabled.Value);
                    CoPilot.Instance.Settings.brandRecallMinEnemys.Value =
                        ImGuiExtension.IntSlider("min. Enemys in Trigger Range",
                            CoPilot.Instance.Settings.brandRecallMinEnemys);
                    CoPilot.Instance.Settings.brandRecallTriggerRange.Value = ImGuiExtension.IntSlider("Trigger Range",
                        CoPilot.Instance.Settings.brandRecallTriggerRange);
                    CoPilot.Instance.Settings.brandRecallMinBrands.Value =
                        ImGuiExtension.IntSlider("min. Brands Summoned",
                            CoPilot.Instance.Settings.brandRecallMinBrands);
                }
            }
            catch (Exception e)
            {
                CoPilot.Instance.LogError(e.ToString());
            }

            try
            {
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.Instance.Settings.tempestShieldEnabled ? green : red);
                ImGui.PushID(17);
                if (ImGui.TreeNodeEx("Tempest Shield", collapsingHeaderFlags))
                {
                    CoPilot.Instance.Settings.tempestShieldEnabled.Value = ImGuiExtension.Checkbox("Enabled",
                        CoPilot.Instance.Settings.tempestShieldEnabled.Value);
                    CoPilot.Instance.Settings.tempestShieldUseWhileCasting.Value = ImGuiExtension.Checkbox(
                        "Use while Casting/Attacking", CoPilot.Instance.Settings.tempestShieldUseWhileCasting.Value);
                    CoPilot.Instance.Settings.tempestShieldTriggerRange.Value =
                        ImGuiExtension.IntSlider("Trigger Range", CoPilot.Instance.Settings.tempestShieldTriggerRange);
                    CoPilot.Instance.Settings.tempestShieldMinAny.Value =
                        ImGuiExtension.IntSlider("min Enemy Any", CoPilot.Instance.Settings.tempestShieldMinAny);
                    CoPilot.Instance.Settings.tempestShieldMinRare.Value =
                        ImGuiExtension.IntSlider("min Enemy Rare", CoPilot.Instance.Settings.tempestShieldMinRare);
                    CoPilot.Instance.Settings.tempestShieldMinUnique.Value =
                        ImGuiExtension.IntSlider("min Enemy Unique", CoPilot.Instance.Settings.tempestShieldMinUnique);
                }
            }
            catch (Exception e)
            {
                CoPilot.Instance.LogError(e.ToString());
            }

            try
            {
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.Instance.Settings.convocationEnabled ? green : red);
                ImGui.PushID(18);
                if (ImGui.TreeNodeEx("Convocation", collapsingHeaderFlags))
                {
                    CoPilot.Instance.Settings.convocationEnabled.Value = ImGuiExtension.Checkbox("Enabled",
                        CoPilot.Instance.Settings.convocationEnabled.Value);
                    CoPilot.Instance.Settings.convocationAvoidUniqueRange.Value = ImGuiExtension.IntSlider(
                        "Do not use when Unique Enemy within", CoPilot.Instance.Settings.convocationAvoidUniqueRange);
                    CoPilot.Instance.Settings.guardianHpp.Value =
                        ImGuiExtension.IntSlider("Summon HP% below", CoPilot.Instance.Settings.guardianHpp);
                    CoPilot.Instance.Settings.convocationMobRange.Value = ImGuiExtension.IntSlider("Mob Trigger Range",
                        CoPilot.Instance.Settings.convocationMobRange);
                    CoPilot.Instance.Settings.convocationMinnionRange.Value = ImGuiExtension.IntSlider("Minnion Range",
                        CoPilot.Instance.Settings.convocationMinnionRange);
                    CoPilot.Instance.Settings.convocationMinnionPct.Value =
                        ImGuiExtension.IntSlider("min. % Minnions in Range",
                            CoPilot.Instance.Settings.convocationMinnionPct);

                    ImGui.Text(
                        "This will Summon your Minnions when min% minnions are not within Minnion Range, and there is an enemys within Mob Trigger Range.");
                }
            }
            catch (Exception e)
            {
                CoPilot.Instance.LogError(e.ToString());
            }

            try
            {
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.Instance.Settings.rangedTriggerEnabled ? green : red);
                ImGui.PushID(19);
                if (ImGui.TreeNodeEx("Ranged Trigger -> Mirage Archer / Frenzy(Power/Frenzy Charges)", collapsingHeaderFlags))
                {
                    ImGui.Text(
                        "Currently only check if an enemy is near mouse, recommend < 75 range when only firing 1 arrow.");
                    ImGui.Text("Works alot better with Volley/GMP, Increase Range by try&error");
                    CoPilot.Instance.Settings.rangedTriggerEnabled.Value =
                        ImGuiExtension.Checkbox("Enabled", CoPilot.Instance.Settings.rangedTriggerEnabled.Value);
                    CoPilot.Instance.Settings.rangedTriggerPowerCharge.Value =
                        ImGuiExtension.Checkbox("Frenzy: Power Charges Instead", CoPilot.Instance.Settings.rangedTriggerPowerCharge.Value);
                    CoPilot.Instance.Settings.rangedTriggerMouseRange.Value = ImGuiExtension.IntSlider("Enemy Range to Mouse",
                        CoPilot.Instance.Settings.rangedTriggerMouseRange);
                    CoPilot.Instance.Settings.rangedTriggerTargetRange.Value = ImGuiExtension.IntSlider("Enemy Range to Player",
                        CoPilot.Instance.Settings.rangedTriggerTargetRange);
                    CoPilot.Instance.Settings.rangedTriggerCooldown.Value = ImGuiExtension.IntSlider("Cooldown between attacks",
                        CoPilot.Instance.Settings.rangedTriggerCooldown);
                }
            }
            catch (Exception e)
            {
                CoPilot.Instance.LogError(e.ToString());
            }

            try
            {
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.Instance.Settings.autoCurseEnabled ? green : red);
                ImGui.PushID(20);
                if (ImGui.TreeNodeEx("Auto Curse (Testing: Only Punishment atm)", collapsingHeaderFlags))
                {
                    CoPilot.Instance.Settings.autoCurseEnabled.Value =
                        ImGuiExtension.Checkbox("Enabled", CoPilot.Instance.Settings.autoCurseEnabled.Value);
                    CoPilot.Instance.Settings.autoCurseCooldown.Value =
                        ImGuiExtension.IntSlider("Cooldown", CoPilot.Instance.Settings.autoCurseCooldown);
                    CoPilot.Instance.Settings.autoCurseRange.Value = ImGuiExtension.IntSlider("Enemy Range near Mouse",
                        CoPilot.Instance.Settings.autoCurseRange);
                    CoPilot.Instance.Settings.autoCurseMinEnemys.Value =
                        ImGuiExtension.IntSlider("min. Enemys not Cursed",
                            CoPilot.Instance.Settings.autoCurseMinEnemys);
                }
            }
            catch (Exception e)
            {
                CoPilot.Instance.LogError(e.ToString());
            }

            try
            {
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.Instance.Settings.bladeVortex ? green : red);
                ImGui.PushID(21);
                if (ImGui.TreeNodeEx("Blade Vortex", collapsingHeaderFlags))
                {
                    CoPilot.Instance.Settings.bladeVortex.Value =
                        ImGuiExtension.Checkbox("Enabled", CoPilot.Instance.Settings.bladeVortex.Value);
                    CoPilot.Instance.Settings.bladeVortexRange.Value =
                        ImGuiExtension.IntSlider("Enemy Range", CoPilot.Instance.Settings.bladeVortexRange);
                    CoPilot.Instance.Settings.bladeVortexCount.Value =
                        ImGuiExtension.IntSlider("Blade Count", CoPilot.Instance.Settings.bladeVortexCount);
                }
            }
            catch (Exception e)
            {
                CoPilot.Instance.LogError(e.ToString());
            }

            try
            {
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.Instance.Settings.bladeBlast ? green : red);
                ImGui.PushID(22);
                if (ImGui.TreeNodeEx("Blade Blast", collapsingHeaderFlags))
                {
                    CoPilot.Instance.Settings.bladeBlast.Value =
                        ImGuiExtension.Checkbox("Enabled", CoPilot.Instance.Settings.bladeBlast.Value);
                    CoPilot.Instance.Settings.bladeBlastFastMode.Value = ImGuiExtension.Checkbox("Fast Mode",
                        CoPilot.Instance.Settings.bladeBlastFastMode.Value);
                    CoPilot.Instance.Settings.bladeBlastEntityRange.Value =
                        ImGuiExtension.IntSlider("Entity Mouse Range", CoPilot.Instance.Settings.bladeBlastEntityRange);
                }
            }
            catch (Exception e)
            {
                CoPilot.Instance.LogError(e.ToString());
            }

            try
            {
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.Instance.Settings.autoMapTabber ? green : red);
                ImGui.PushID(23);
                if (ImGui.TreeNodeEx("Auto Map Tabber", collapsingHeaderFlags))
                    CoPilot.Instance.Settings.autoMapTabber.Value =
                        ImGuiExtension.Checkbox("Enabled", CoPilot.Instance.Settings.autoMapTabber.Value);
            }
            catch (Exception e)
            {
                CoPilot.Instance.LogError(e.ToString());
            }

            try
            {
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.Instance.Settings.berserkEnabled ? green : red);
                ImGui.PushID(24);
                if (ImGui.TreeNodeEx("Berserk", collapsingHeaderFlags))
                {
                    CoPilot.Instance.Settings.berserkEnabled.Value =
                        ImGuiExtension.Checkbox("Enabled", CoPilot.Instance.Settings.berserkEnabled.Value);
                    CoPilot.Instance.Settings.berserkRange.Value =
                        ImGuiExtension.IntSlider("Mob Trigger Range", CoPilot.Instance.Settings.berserkRange);
                    CoPilot.Instance.Settings.berserkMinAny.Value =
                        ImGuiExtension.IntSlider("min Enemy Any", CoPilot.Instance.Settings.berserkMinAny);
                    CoPilot.Instance.Settings.berserkMinRare.Value =
                        ImGuiExtension.IntSlider("min Enemy Rare", CoPilot.Instance.Settings.berserkMinRare);
                    CoPilot.Instance.Settings.berserkMinUnique.Value = ImGuiExtension.IntSlider("min Enemy Unique",
                        CoPilot.Instance.Settings.berserkMinUnique);
                    CoPilot.Instance.Settings.berserkMinRage.Value = ImGuiExtension.IntSlider(
                        "min Rage. Even 0 will be > min set in Skill.", CoPilot.Instance.Settings.berserkMinRage);
                }
            }
            catch (Exception e)
            {
                CoPilot.Instance.LogError(e.ToString());
            }
            
            try
            {
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.Instance.Settings.generalCryEnabled ? green : red);
                ImGui.PushID(25);
                if (ImGui.TreeNodeEx("General's Cry", collapsingHeaderFlags))
                {
                    CoPilot.Instance.Settings.generalCryEnabled.Value =
                        ImGuiExtension.Checkbox("Enabled", CoPilot.Instance.Settings.generalCryEnabled.Value);
                    CoPilot.Instance.Settings.generalCryCorpseTriggerRange.Value =
                        ImGuiExtension.IntSlider("Corpse Trigger Range", CoPilot.Instance.Settings.generalCryCorpseTriggerRange);
                    CoPilot.Instance.Settings.generalCryTriggerRange.Value =
                        ImGuiExtension.IntSlider("min 1 Enemy Range", CoPilot.Instance.Settings.generalCryTriggerRange);
                    CoPilot.Instance.Settings.generalCryMinCorpse.Value =
                        ImGuiExtension.IntSlider("min Corpse", CoPilot.Instance.Settings.generalCryMinCorpse);
                }
            }
            catch (Exception e)
            {
                CoPilot.Instance.LogError(e.ToString());
            }

            try
            {
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.Instance.Settings.plagueBearer ? green : red);
                ImGui.PushID(26);
                if (ImGui.TreeNodeEx("Plague Bearer", collapsingHeaderFlags))
                {
                    CoPilot.Instance.Settings.plagueBearer.Value = ImGuiExtension.Checkbox("Enabled", CoPilot.Instance.Settings.plagueBearer.Value);
                    CoPilot.Instance.Settings.plagueBearerRange.Value = ImGuiExtension.IntSlider("Enemy Range", CoPilot.Instance.Settings.plagueBearerRange);
                    CoPilot.Instance.Settings.plagueBearerMinEnemys.Value = ImGuiExtension.IntSlider("Min Enemy Any", CoPilot.Instance.Settings.plagueBearerMinEnemys);
                }
            }
            catch (Exception e)
            {
                CoPilot.Instance.LogError(e.ToString());
            }
            
            try
            {
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.Instance.Settings.cwdtEnabled ? green : red);
                ImGui.PushID(27);
                if (ImGui.TreeNodeEx("CWDT Loop Helper", collapsingHeaderFlags))
                {
                    CoPilot.Instance.Settings.cwdtEnabled.Value = ImGuiExtension.Checkbox("Enabled", CoPilot.Instance.Settings.cwdtEnabled.Value);
                }
            }
            catch (Exception e)
            {
                CoPilot.Instance.LogError(e.ToString());
            }

            try
            {
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.Instance.Settings.minesEnabled ? green : red);
                ImGui.PushID(99999);
                if (ImGui.TreeNodeEx("Mines (Not Implemented)", collapsingHeaderFlags))
                {
                }
            }
            catch (Exception e)
            {
                CoPilot.Instance.LogError(e.ToString());
            }

            ImGui.End();
        }
    }
}
