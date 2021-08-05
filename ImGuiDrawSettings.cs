﻿using System;
using System.Diagnostics;
using System.Numerics;
using System.Threading;
using System.Windows.Forms;
using ImGuiNET;

namespace CoPilot
{
    internal class ImGuiDrawSettings
    {
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
                    CoPilot.instance.Settings.inputKey1.Value = ImGuiExtension.HotkeySelector(
                        "Use bound skill 2 Key: " + CoPilot.instance.Settings.inputKey1.Value,
                        CoPilot.instance.Settings.inputKey1.Value);
                    CoPilot.instance.Settings.inputKey3.Value = ImGuiExtension.HotkeySelector(
                        "Use bound skill 4 Key: " + CoPilot.instance.Settings.inputKey3.Value,
                        CoPilot.instance.Settings.inputKey3.Value);
                    CoPilot.instance.Settings.inputKey4.Value = ImGuiExtension.HotkeySelector(
                        "Use bound skill 5 Key: " + CoPilot.instance.Settings.inputKey4.Value,
                        CoPilot.instance.Settings.inputKey4.Value);
                    CoPilot.instance.Settings.inputKey5.Value = ImGuiExtension.HotkeySelector(
                        "Use bound skill 6 Key: " + CoPilot.instance.Settings.inputKey5.Value,
                        CoPilot.instance.Settings.inputKey5.Value);
                    CoPilot.instance.Settings.inputKey6.Value = ImGuiExtension.HotkeySelector(
                        "Use bound skill 7 Key: " + CoPilot.instance.Settings.inputKey6.Value,
                        CoPilot.instance.Settings.inputKey6.Value);
                    CoPilot.instance.Settings.inputKey7.Value = ImGuiExtension.HotkeySelector(
                        "Use bound skill 8 Key: " + CoPilot.instance.Settings.inputKey7.Value,
                        CoPilot.instance.Settings.inputKey7.Value);
                    CoPilot.instance.Settings.inputKey8.Value = ImGuiExtension.HotkeySelector(
                        "Use bound skill 9 Key: " + CoPilot.instance.Settings.inputKey8.Value,
                        CoPilot.instance.Settings.inputKey8.Value);
                    CoPilot.instance.Settings.inputKey9.Value = ImGuiExtension.HotkeySelector(
                        "Use bound skill 10 Key: " + CoPilot.instance.Settings.inputKey9.Value,
                        CoPilot.instance.Settings.inputKey9.Value);
                    CoPilot.instance.Settings.inputKey10.Value = ImGuiExtension.HotkeySelector(
                        "Use bound skill 11 Key: " + CoPilot.instance.Settings.inputKey10.Value,
                        CoPilot.instance.Settings.inputKey10.Value);
                    CoPilot.instance.Settings.inputKey11.Value = ImGuiExtension.HotkeySelector(
                        "Use bound skill 12 Key: " + CoPilot.instance.Settings.inputKey11.Value,
                        CoPilot.instance.Settings.inputKey11.Value);
                    CoPilot.instance.Settings.inputKey12.Value = ImGuiExtension.HotkeySelector(
                        "Use bound skill 13 Key: " + CoPilot.instance.Settings.inputKey12.Value,
                        CoPilot.instance.Settings.inputKey12.Value);
                    CoPilot.instance.Settings.inputKeyPickIt.Value = ImGuiExtension.HotkeySelector(
                        "PickIt Key: " + CoPilot.instance.Settings.inputKeyPickIt.Value,
                        CoPilot.instance.Settings.inputKeyPickIt.Value);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }

            try
            {
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.instance.Settings.confirm5 ? green : red);
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


                    CoPilot.instance.Settings.confirm1.Value = ImGuiExtension.Checkbox("I did READ the text above.",
                        CoPilot.instance.Settings.confirm1.Value);
                    if (!CoPilot.instance.Settings.confirm1)
                        return;
                    CoPilot.instance.Settings.confirm2.Value = ImGuiExtension.Checkbox(
                        "I did READ and UNDERSTAND the text above.", CoPilot.instance.Settings.confirm2.Value);
                    if (!CoPilot.instance.Settings.confirm2)
                        return;
                    CoPilot.instance.Settings.confirm3.Value = ImGuiExtension.Checkbox(
                        "I just READ it again and understood it.", CoPilot.instance.Settings.confirm3.Value);
                    if (!CoPilot.instance.Settings.confirm3)
                        return;
                    CoPilot.instance.Settings.confirm4.Value = ImGuiExtension.Checkbox(
                        "I did everything stated above and im ready to go.", CoPilot.instance.Settings.confirm4.Value);
                    if (!CoPilot.instance.Settings.confirm4)
                        return;
                    CoPilot.instance.Settings.confirm5.Value =
                        ImGuiExtension.Checkbox("Let me use the Plugin already !!!",
                            CoPilot.instance.Settings.confirm5.Value);
                    if (!CoPilot.instance.Settings.confirm5)
                        return;
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }

            if (!CoPilot.instance.Settings.confirm5)
                return;

            try
            {
                // Donation
                ImGui.PushStyleColor(ImGuiCol.Header, new Vector4(0.454f, 0.031f, 0.768f, 1.000f));
                ImGui.PushID(99999);
                if (ImGui.TreeNodeEx("Donation", collapsingHeaderFlags))
                {
                    ImGui.Text("Thanks to anyone who is considering this.");
                    if (ImGui.Button("Open Amazon.de Wishlist"))
                        Process.Start("https://www.amazon.de/hz/wishlist/ls/MZ543BDBC6PJ?ref_=wl_share");
                    if (ImGui.Button("Copy BTC Adress")) SetText("bc1qwjpdf9q3n94e88m3z398udjagach5u56txwpkh");
                    if (ImGui.Button("Copy ETH Adress")) SetText("0x78Af12D08B32f816dB9788C5Cf3122693143ed78");
                    CoPilot.instance.Settings.debugMode.Value = ImGuiExtension.Checkbox("Turn on Debug Mode",
                        CoPilot.instance.Settings.debugMode.Value);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }

            try
            {
                // Auto Attack
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.instance.Settings.autoAttackEnabled ? green : red);
                ImGui.PushID(1);
                if (ImGui.TreeNodeEx("Auto Attack with Cyclone / Nova / Flicker / Sweep", collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.autoAttackEnabled.Value =
                        ImGuiExtension.Checkbox("Enabled", CoPilot.instance.Settings.autoAttackEnabled.Value);
                    CoPilot.instance.Settings.autoAttackLeftMouseCheck.Value = ImGuiExtension.Checkbox(
                        "Pause on Left Mouse Pressed", CoPilot.instance.Settings.autoAttackLeftMouseCheck.Value);
                    CoPilot.instance.Settings.autoAttackRange.Value =
                        ImGuiExtension.IntSlider("Range", CoPilot.instance.Settings.autoAttackRange);
                    CoPilot.instance.Settings.autoAttackCurseCheck.Value = ImGuiExtension.Checkbox(
                        "Line of Sight Check with Curse / RaiderExposeAura, Ignores Range Setting!",
                        CoPilot.instance.Settings.autoAttackCurseCheck.Value);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }


            try
            {
                // Auto Golem
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.instance.Settings.autoSummonEnabled ? green : red);
                ImGui.PushID(2);
                if (ImGui.TreeNodeEx("Auto Summon", collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.autoSummonEnabled.Value = ImGuiExtension.Checkbox("Auto Summons",
                        CoPilot.instance.Settings.autoSummonEnabled.Value);
                    CoPilot.instance.Settings.autoGolemEnabled.Value = ImGuiExtension.Checkbox("Auto Golem",
                        CoPilot.instance.Settings.autoGolemEnabled.Value);
                    CoPilot.instance.Settings.autoHolyRelictEnabled.Value = ImGuiExtension.Checkbox("Auto Holy Relict",
                        CoPilot.instance.Settings.autoHolyRelictEnabled.Value);
                    CoPilot.instance.Settings.autoZombieEnabled.Value = ImGuiExtension.Checkbox("Auto Zombies",
                        CoPilot.instance.Settings.autoZombieEnabled.Value);
                    CoPilot.instance.Settings.autoGolemAvoidRange.Value =
                        ImGuiExtension.IntSlider("Do not use when Enemys within",
                            CoPilot.instance.Settings.autoGolemAvoidRange);
                    CoPilot.instance.Settings.autoGolemChaosMax.Value = ImGuiExtension.IntSlider("Chaos Golems max.",
                        CoPilot.instance.Settings.autoGolemChaosMax);
                    CoPilot.instance.Settings.autoGolemFireMax.Value = ImGuiExtension.IntSlider("Flame Golems max.",
                        CoPilot.instance.Settings.autoGolemFireMax);
                    CoPilot.instance.Settings.autoGolemIceMax.Value = ImGuiExtension.IntSlider("Ice Golems max.",
                        CoPilot.instance.Settings.autoGolemIceMax);
                    CoPilot.instance.Settings.autoGolemLightningMax.Value =
                        ImGuiExtension.IntSlider("Lightning Golems max.",
                            CoPilot.instance.Settings.autoGolemLightningMax);
                    CoPilot.instance.Settings.autoGolemRockMax.Value = ImGuiExtension.IntSlider("Stone Golems max.",
                        CoPilot.instance.Settings.autoGolemRockMax);
                    CoPilot.instance.Settings.autoBoneMax.Value = ImGuiExtension.IntSlider("Carrion Golems max.",
                        CoPilot.instance.Settings.autoBoneMax);
                    CoPilot.instance.Settings.autoGolemDropBearMax.Value =
                        ImGuiExtension.IntSlider("Beastial Ursa Max.", CoPilot.instance.Settings.autoGolemDropBearMax);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }


            try
            {
                // Auto Quit
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.instance.Settings.autoQuitEnabled ? green : red);
                ImGui.PushID(3);
                if (ImGui.TreeNodeEx("Auto Quit (This requires HUD started as Admin !)", collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.autoQuitEnabled.Value =
                        ImGuiExtension.Checkbox("Enabled", CoPilot.instance.Settings.autoQuitEnabled.Value);
                    CoPilot.instance.Settings.hppQuit.Value =
                        ImGuiExtension.IntSlider("HP%", CoPilot.instance.Settings.hppQuit);
                    CoPilot.instance.Settings.espQuit.Value =
                        ImGuiExtension.IntSlider("ES%", CoPilot.instance.Settings.espQuit);
                    CoPilot.instance.Settings.autoQuitGuardian.Value = ImGuiExtension.Checkbox("Guardian Auto Quit",
                        CoPilot.instance.Settings.autoQuitGuardian.Value);
                    CoPilot.instance.Settings.guardianHpp.Value =
                        ImGuiExtension.IntSlider("Guardian HP%", CoPilot.instance.Settings.guardianHpp);
                    CoPilot.instance.Settings.autoQuitHotkeyEnabled.Value = ImGuiExtension.Checkbox("Hotkey Enabled",
                        CoPilot.instance.Settings.autoQuitHotkeyEnabled.Value);
                    CoPilot.instance.Settings.forcedAutoQuit.Value = ImGuiExtension.HotkeySelector(
                        "Force Quit Hotkey: " + CoPilot.instance.Settings.forcedAutoQuit.Value,
                        CoPilot.instance.Settings.forcedAutoQuit.Value);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }


            try
            {
                // Blood Rage
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.instance.Settings.bloodRageEnabled ? green : red);
                ImGui.PushID(4);
                if (ImGui.TreeNodeEx("Blood Rage", collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.bloodRageEnabled.Value =
                        ImGuiExtension.Checkbox("Enabled", CoPilot.instance.Settings.bloodRageEnabled.Value);
                    CoPilot.instance.Settings.bloodRageRange.Value =
                        ImGuiExtension.IntSlider("Range", CoPilot.instance.Settings.bloodRageRange);
                    CoPilot.instance.Settings.bloodRageMinAny.Value =
                        ImGuiExtension.IntSlider("min Enemy Any", CoPilot.instance.Settings.bloodRageMinAny);
                    CoPilot.instance.Settings.bloodRageMinRare.Value =
                        ImGuiExtension.IntSlider("min Enemy Rare", CoPilot.instance.Settings.bloodRageMinRare);
                    CoPilot.instance.Settings.bloodRageMinUnique.Value = ImGuiExtension.IntSlider("min Enemy Unique",
                        CoPilot.instance.Settings.bloodRageMinUnique);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }


            try
            {
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.instance.Settings.delveFlareEnabled ? green : red);
                ImGui.PushID(5);
                if (ImGui.TreeNodeEx("Delve Flare", collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.delveFlareEnabled.Value =
                        ImGuiExtension.Checkbox("Enabled", CoPilot.instance.Settings.delveFlareEnabled);
                    CoPilot.instance.Settings.delveFlareKey.Value = ImGuiExtension.HotkeySelector(
                        "Key: " + CoPilot.instance.Settings.delveFlareKey.Value,
                        CoPilot.instance.Settings.delveFlareKey.Value);
                    CoPilot.instance.Settings.delveFlareDebuffStacks.Value =
                        ImGuiExtension.IntSlider("min. Debuff Stacks",
                            CoPilot.instance.Settings.delveFlareDebuffStacks);
                    CoPilot.instance.Settings.delveFlareHppBelow.Value =
                        ImGuiExtension.IntSlider("HP%", CoPilot.instance.Settings.delveFlareHppBelow);
                    CoPilot.instance.Settings.delveFlareEspBelow.Value =
                        ImGuiExtension.IntSlider("ES%", CoPilot.instance.Settings.delveFlareEspBelow);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }


            try
            {
                // Doedre Effigy
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.instance.Settings.doedreEffigyEnabled ? green : red);
                ImGui.PushID(6);
                if (ImGui.TreeNodeEx("Doedre Effigy", collapsingHeaderFlags))
                    CoPilot.instance.Settings.doedreEffigyEnabled.Value = ImGuiExtension.Checkbox("Enabled",
                        CoPilot.instance.Settings.doedreEffigyEnabled.Value);
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }


            try
            {
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.instance.Settings.divineIreEnabled ? green : red);
                ImGui.PushID(7);
                if (ImGui.TreeNodeEx("Divine Ire / Blade Flurry / Scourge Arrow (Do NOT bind on Mousekeys!!!)",
                    collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.divineIreEnabled.Value =
                        ImGuiExtension.Checkbox("Enabled", CoPilot.instance.Settings.divineIreEnabled.Value);
                    CoPilot.instance.Settings.divineIreStacks.Value =
                        ImGuiExtension.IntSlider("Stacks", CoPilot.instance.Settings.divineIreStacks);
                    CoPilot.instance.Settings.divineIreWaitForInfused.Value = ImGuiExtension.Checkbox(
                        "Wait for Infused Channeling Support", CoPilot.instance.Settings.divineIreWaitForInfused.Value);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }


            try
            {
                // Enduring Cry
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.instance.Settings.enduringCryEnabled ? green : red);
                ImGui.PushID(8);
                if (ImGui.TreeNodeEx("Enduring Cry", collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.enduringCryEnabled.Value = ImGuiExtension.Checkbox("Enabled",
                        CoPilot.instance.Settings.enduringCryEnabled.Value);
                    CoPilot.instance.Settings.enduringCryTriggerRange.Value = ImGuiExtension.IntSlider("Enemy within Range",
                        CoPilot.instance.Settings.enduringCryTriggerRange);
                    CoPilot.instance.Settings.enduringCryHealHpp.Value = ImGuiExtension.IntSlider("Heal HP%",
                        CoPilot.instance.Settings.enduringCryHealHpp);
                    CoPilot.instance.Settings.enduringCrySpam.Value = ImGuiExtension.Checkbox("Spam Warcry",
                        CoPilot.instance.Settings.enduringCrySpam.Value);
                    CoPilot.instance.Settings.enduringCryMinAny.Value =
                        ImGuiExtension.IntSlider("min Enemy Any", CoPilot.instance.Settings.enduringCryMinAny);
                    CoPilot.instance.Settings.enduringCryMinRare.Value =
                        ImGuiExtension.IntSlider("min Enemy Rare", CoPilot.instance.Settings.enduringCryMinRare);
                    CoPilot.instance.Settings.enduringCryMinUnique.Value = ImGuiExtension.IntSlider("min Enemy Unique",
                        CoPilot.instance.Settings.enduringCryMinUnique);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }


            try
            {
                // Molten Shell / Steelskin / Bone Armour / Arcane Cloak
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.instance.Settings.moltenShellEnabled ? green : red);
                ImGui.PushID(9);
                if (ImGui.TreeNodeEx("Molten Shell / Steelskin / Bone Armour / Arcane Cloak", collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.moltenShellEnabled.Value = ImGuiExtension.Checkbox("Enabled",
                        CoPilot.instance.Settings.moltenShellEnabled.Value);
                    CoPilot.instance.Settings.moltenShellHpp.Value =
                        ImGuiExtension.IntSlider("HP%", CoPilot.instance.Settings.moltenShellHpp);
                    CoPilot.instance.Settings.moltenShellEsp.Value =
                        ImGuiExtension.IntSlider("ES%", CoPilot.instance.Settings.moltenShellEsp);
                    CoPilot.instance.Settings.moltenShellRange.Value =
                        ImGuiExtension.IntSlider("Range", CoPilot.instance.Settings.moltenShellRange);
                    CoPilot.instance.Settings.moltenShellMinAny.Value =
                        ImGuiExtension.IntSlider("min Enemy Any", CoPilot.instance.Settings.moltenShellMinAny);
                    CoPilot.instance.Settings.moltenShellMinRare.Value = ImGuiExtension.IntSlider("min Enemy Rare",
                        CoPilot.instance.Settings.moltenShellMinRare);
                    CoPilot.instance.Settings.moltenShellMinUnique.Value = ImGuiExtension.IntSlider("min Enemy Unique",
                        CoPilot.instance.Settings.moltenShellMinUnique);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }


            try
            {
                // Offerings
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.instance.Settings.offeringsEnabled ? green : red);
                ImGui.PushID(10);
                if (ImGui.TreeNodeEx("Offerings (This will get you stuck in Animation for your Casttime !)",
                    collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.offeringsEnabled.Value =
                        ImGuiExtension.Checkbox("Enabled", CoPilot.instance.Settings.offeringsEnabled.Value);
                    CoPilot.instance.Settings.offeringsUseWhileCasting.Value = ImGuiExtension.Checkbox(
                        "Use while Casting/Attacking", CoPilot.instance.Settings.offeringsUseWhileCasting.Value);
                    CoPilot.instance.Settings.offeringsTriggerRange.Value = ImGuiExtension.IntSlider("Trigger Range",
                        CoPilot.instance.Settings.offeringsTriggerRange);
                    CoPilot.instance.Settings.offeringsMinAny.Value =
                        ImGuiExtension.IntSlider("min Enemy Any", CoPilot.instance.Settings.offeringsMinAny);
                    CoPilot.instance.Settings.offeringsMinRare.Value =
                        ImGuiExtension.IntSlider("min Enemy Rare", CoPilot.instance.Settings.offeringsMinRare);
                    CoPilot.instance.Settings.offeringsMinUnique.Value = ImGuiExtension.IntSlider("min Enemy Unique",
                        CoPilot.instance.Settings.offeringsMinUnique);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }


            try
            {
                // Phaserun
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.instance.Settings.phaserunEnabled ? green : red);
                ImGui.PushID(11);
                if (ImGui.TreeNodeEx("Phaserun / WitherStep", collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.phaserunEnabled.Value =
                        ImGuiExtension.Checkbox("Enabled", CoPilot.instance.Settings.phaserunEnabled.Value);
                    CoPilot.instance.Settings.phaserunUseLifeTap.Value = ImGuiExtension.Checkbox("LifeTap Mode",
                        CoPilot.instance.Settings.phaserunUseLifeTap.Value);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }

            try
            {
                // Vortex
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.instance.Settings.vortexEnabled ? green : red);
                ImGui.PushID(13);
                if (ImGui.TreeNodeEx("Vortex", collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.vortexEnabled.Value =
                        ImGuiExtension.Checkbox("Enabled", CoPilot.instance.Settings.vortexEnabled.Value);
                    CoPilot.instance.Settings.vortexFrostbolt.Value = ImGuiExtension.Checkbox("When Frostbolt's flying",
                        CoPilot.instance.Settings.vortexFrostbolt.Value);
                    CoPilot.instance.Settings.vortexRange.Value =
                        ImGuiExtension.IntSlider("Range", CoPilot.instance.Settings.vortexRange);
                    CoPilot.instance.Settings.vortexMinAny.Value =
                        ImGuiExtension.IntSlider("min Enemy Any", CoPilot.instance.Settings.vortexMinAny);
                    CoPilot.instance.Settings.vortexMinRare.Value =
                        ImGuiExtension.IntSlider("min Enemy Rare", CoPilot.instance.Settings.vortexMinRare);
                    CoPilot.instance.Settings.vortexMinUnique.Value = ImGuiExtension.IntSlider("min Enemy Unique",
                        CoPilot.instance.Settings.vortexMinUnique);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }

            try
            {
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.instance.Settings.anyVaalEnabled || CoPilot.instance.Settings.VaalClarityEnabled ? green : red);
                ImGui.PushID(14);
                if (ImGui.TreeNodeEx("Vaal Skills", collapsingHeaderFlags))
                {
                    ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.instance.Settings.anyVaalEnabled ? green : red);
                    ImGui.PushID(014);
                    ImGui.BeginChild(14, new Vector2(0,280));
                    if (ImGui.TreeNodeEx("Any", collapsingHeaderFlags))
                    {
                        CoPilot.instance.Settings.anyVaalEnabled.Value =
                            ImGuiExtension.Checkbox("Enabled", CoPilot.instance.Settings.anyVaalEnabled.Value);
                        CoPilot.instance.Settings.anyVaalTriggerRange.Value = ImGuiExtension.IntSlider("Trigger Range",
                            CoPilot.instance.Settings.anyVaalTriggerRange);
                        CoPilot.instance.Settings.anyVaalHpp.Value =
                            ImGuiExtension.IntSlider("HP%", CoPilot.instance.Settings.anyVaalHpp);
                        CoPilot.instance.Settings.anyVaalEsp.Value =
                            ImGuiExtension.IntSlider("ES%", CoPilot.instance.Settings.anyVaalEsp);
                        CoPilot.instance.Settings.anyVaalMpp.Value =
                            ImGuiExtension.IntSlider("MP%", CoPilot.instance.Settings.anyVaalMpp);
                        CoPilot.instance.Settings.anyVaalMinAny.Value =
                            ImGuiExtension.IntSlider("min Enemy Any", CoPilot.instance.Settings.anyVaalMinAny);
                        CoPilot.instance.Settings.anyVaalMinRare.Value =
                            ImGuiExtension.IntSlider("min Enemy Rare", CoPilot.instance.Settings.anyVaalMinRare);
                        CoPilot.instance.Settings.anyVaalMinUnique.Value = ImGuiExtension.IntSlider("min Enemy Unique",
                            CoPilot.instance.Settings.anyVaalMinUnique);
                    }
                    
                    ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.instance.Settings.VaalClarityEnabled ? green : red);
                    ImGui.PushID(0014);
                    if (ImGui.TreeNodeEx("Vaal Clarity Test not active !", collapsingHeaderFlags))
                    {
                        CoPilot.instance.Settings.VaalClarityEnabled.Value =
                            ImGuiExtension.Checkbox("Enabled", CoPilot.instance.Settings.VaalClarityEnabled.Value);
                        CoPilot.instance.Settings.VaalClarityManaPct.Value =
                            ImGuiExtension.IntSlider("MP%", CoPilot.instance.Settings.VaalClarityManaPct);
                    }
                    ImGui.EndChild();
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }

            try
            {
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.instance.Settings.customEnabled ? green : red);
                ImGui.PushID(15);
                if (ImGui.TreeNodeEx("Custom Skill (Use any Skill not Supported here.)", collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.customEnabled.Value =
                        ImGuiExtension.Checkbox("Enabled", CoPilot.instance.Settings.customEnabled.Value);
                    CoPilot.instance.Settings.customKey.Value = ImGuiExtension.HotkeySelector(
                        "Key: " + CoPilot.instance.Settings.customKey.Value, CoPilot.instance.Settings.customKey);
                    CoPilot.instance.Settings.customCooldown.Value =
                        ImGuiExtension.IntSlider("Cooldown", CoPilot.instance.Settings.customCooldown);
                    CoPilot.instance.Settings.customTriggerRange.Value = ImGuiExtension.IntSlider("Trigger Range",
                        CoPilot.instance.Settings.customTriggerRange);
                    CoPilot.instance.Settings.customHpp.Value =
                        ImGuiExtension.IntSlider("HP%", CoPilot.instance.Settings.customHpp);
                    CoPilot.instance.Settings.customEsp.Value =
                        ImGuiExtension.IntSlider("ES%", CoPilot.instance.Settings.customEsp);
                    CoPilot.instance.Settings.customMinAny.Value =
                        ImGuiExtension.IntSlider("min Enemy Any", CoPilot.instance.Settings.customMinAny);
                    CoPilot.instance.Settings.customMinRare.Value =
                        ImGuiExtension.IntSlider("min Enemy Rare", CoPilot.instance.Settings.customMinRare);
                    CoPilot.instance.Settings.customMinUnique.Value = ImGuiExtension.IntSlider("min Enemy Unique",
                        CoPilot.instance.Settings.customMinUnique);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }

            try
            {
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.instance.Settings.brandRecallEnabled ? green : red);
                ImGui.PushID(16);
                if (ImGui.TreeNodeEx("Brand Recall", collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.brandRecallEnabled.Value = ImGuiExtension.Checkbox("Enabled",
                        CoPilot.instance.Settings.brandRecallEnabled.Value);
                    CoPilot.instance.Settings.brandRecallMinEnemys.Value =
                        ImGuiExtension.IntSlider("min. Enemys in Trigger Range",
                            CoPilot.instance.Settings.brandRecallMinEnemys);
                    CoPilot.instance.Settings.brandRecallTriggerRange.Value = ImGuiExtension.IntSlider("Trigger Range",
                        CoPilot.instance.Settings.brandRecallTriggerRange);
                    CoPilot.instance.Settings.brandRecallMinBrands.Value =
                        ImGuiExtension.IntSlider("min. Brands Summoned",
                            CoPilot.instance.Settings.brandRecallMinBrands);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }

            try
            {
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.instance.Settings.tempestShieldEnabled ? green : red);
                ImGui.PushID(17);
                if (ImGui.TreeNodeEx("Tempest Shield", collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.tempestShieldEnabled.Value = ImGuiExtension.Checkbox("Enabled",
                        CoPilot.instance.Settings.tempestShieldEnabled.Value);
                    CoPilot.instance.Settings.tempestShieldUseWhileCasting.Value = ImGuiExtension.Checkbox(
                        "Use while Casting/Attacking", CoPilot.instance.Settings.tempestShieldUseWhileCasting.Value);
                    CoPilot.instance.Settings.tempestShieldTriggerRange.Value =
                        ImGuiExtension.IntSlider("Trigger Range", CoPilot.instance.Settings.tempestShieldTriggerRange);
                    CoPilot.instance.Settings.tempestShieldMinAny.Value =
                        ImGuiExtension.IntSlider("min Enemy Any", CoPilot.instance.Settings.anyVaalMinAny);
                    CoPilot.instance.Settings.tempestShieldMinRare.Value =
                        ImGuiExtension.IntSlider("min Enemy Rare", CoPilot.instance.Settings.anyVaalMinRare);
                    CoPilot.instance.Settings.tempestShieldMinUnique.Value =
                        ImGuiExtension.IntSlider("min Enemy Unique", CoPilot.instance.Settings.anyVaalMinUnique);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }

            try
            {
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.instance.Settings.convocationEnabled ? green : red);
                ImGui.PushID(18);
                if (ImGui.TreeNodeEx("Convocation", collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.convocationEnabled.Value = ImGuiExtension.Checkbox("Enabled",
                        CoPilot.instance.Settings.convocationEnabled.Value);
                    CoPilot.instance.Settings.convocationAvoidUniqueRange.Value = ImGuiExtension.IntSlider(
                        "Do not use when Unique Enemy within", CoPilot.instance.Settings.convocationAvoidUniqueRange);
                    CoPilot.instance.Settings.guardianHpp.Value =
                        ImGuiExtension.IntSlider("Summon HP% below", CoPilot.instance.Settings.guardianHpp);
                    CoPilot.instance.Settings.convocationMobRange.Value = ImGuiExtension.IntSlider("Mob Trigger Range",
                        CoPilot.instance.Settings.convocationMobRange);
                    CoPilot.instance.Settings.convocationMinnionRange.Value = ImGuiExtension.IntSlider("Minnion Range",
                        CoPilot.instance.Settings.convocationMinnionRange);
                    CoPilot.instance.Settings.convocationMinnionPct.Value =
                        ImGuiExtension.IntSlider("min. % Minnions in Range",
                            CoPilot.instance.Settings.convocationMinnionPct);

                    ImGui.Text(
                        "This will Summon your Minnions when min% minnions are not within Minnion Range, and there is an enemys within Mob Trigger Range.");
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }

            try
            {
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.instance.Settings.rangedTriggerEnabled ? green : red);
                ImGui.PushID(19);
                if (ImGui.TreeNodeEx("Ranged Trigger -> Mirage Archer / Frenzy(Power/Frenzy Charges)", collapsingHeaderFlags))
                {
                    ImGui.Text(
                        "Currently only check if an enemy is near mouse, recommend < 75 range when only firing 1 arrow.");
                    ImGui.Text("Works alot better with Volley/GMP, Increase Range by try&error");
                    CoPilot.instance.Settings.rangedTriggerEnabled.Value =
                        ImGuiExtension.Checkbox("Enabled", CoPilot.instance.Settings.rangedTriggerEnabled.Value);
                    CoPilot.instance.Settings.rangedTriggerPowerCharge.Value =
                        ImGuiExtension.Checkbox("Frenzy: Power Charges Instead", CoPilot.instance.Settings.rangedTriggerPowerCharge.Value);
                    CoPilot.instance.Settings.rangedTriggerMouseRange.Value = ImGuiExtension.IntSlider("Enemy Range to Mouse",
                        CoPilot.instance.Settings.rangedTriggerMouseRange);
                    CoPilot.instance.Settings.rangedTriggerTargetRange.Value = ImGuiExtension.IntSlider("Enemy Range to Player",
                        CoPilot.instance.Settings.rangedTriggerTargetRange);
                    CoPilot.instance.Settings.rangedTriggerCooldown.Value = ImGuiExtension.IntSlider("Cooldown between attacks",
                        CoPilot.instance.Settings.rangedTriggerCooldown);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }

            try
            {
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.instance.Settings.autoCurseEnabled ? green : red);
                ImGui.PushID(20);
                if (ImGui.TreeNodeEx("Auto Curse (Testing: Only Punishment atm)", collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.autoCurseEnabled.Value =
                        ImGuiExtension.Checkbox("Enabled", CoPilot.instance.Settings.autoCurseEnabled.Value);
                    CoPilot.instance.Settings.autoCurseCooldown.Value =
                        ImGuiExtension.IntSlider("Cooldown", CoPilot.instance.Settings.autoCurseCooldown);
                    CoPilot.instance.Settings.autoCurseRange.Value = ImGuiExtension.IntSlider("Enemy Range near Mouse",
                        CoPilot.instance.Settings.autoCurseRange);
                    CoPilot.instance.Settings.autoCurseMinEnemys.Value =
                        ImGuiExtension.IntSlider("min. Enemys not Cursed",
                            CoPilot.instance.Settings.autoCurseMinEnemys);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }

            try
            {
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.instance.Settings.bladeVortex ? green : red);
                ImGui.PushID(21);
                if (ImGui.TreeNodeEx("Blade Vortex", collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.bladeVortex.Value =
                        ImGuiExtension.Checkbox("Enabled", CoPilot.instance.Settings.bladeVortex.Value);
                    CoPilot.instance.Settings.bladeVortexRange.Value =
                        ImGuiExtension.IntSlider("Enemy Range", CoPilot.instance.Settings.bladeVortexRange);
                    CoPilot.instance.Settings.bladeVortexCount.Value =
                        ImGuiExtension.IntSlider("Blade Count", CoPilot.instance.Settings.bladeVortexCount);
                    CoPilot.instance.Settings.bladeVortexUnleashCount.Value =
                        ImGuiExtension.IntSlider("If Unleash, wait for Charges",
                            CoPilot.instance.Settings.bladeVortexUnleashCount);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }

            try
            {
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.instance.Settings.bladeBlast ? green : red);
                ImGui.PushID(22);
                if (ImGui.TreeNodeEx("Blade Blast", collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.bladeBlast.Value =
                        ImGuiExtension.Checkbox("Enabled", CoPilot.instance.Settings.bladeBlast.Value);
                    CoPilot.instance.Settings.bladeBlastFastMode.Value = ImGuiExtension.Checkbox("Fast Mode",
                        CoPilot.instance.Settings.bladeBlastFastMode.Value);
                    CoPilot.instance.Settings.bladeBlastEntityRange.Value =
                        ImGuiExtension.IntSlider("Entity Mouse Range", CoPilot.instance.Settings.bladeBlastEntityRange);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }

            try
            {
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.instance.Settings.autoMapTabber ? green : red);
                ImGui.PushID(23);
                if (ImGui.TreeNodeEx("Auto Map Tabber", collapsingHeaderFlags))
                    CoPilot.instance.Settings.autoMapTabber.Value =
                        ImGuiExtension.Checkbox("Enabled", CoPilot.instance.Settings.autoMapTabber.Value);
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }

            try
            {
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.instance.Settings.berserkEnabled ? green : red);
                ImGui.PushID(24);
                if (ImGui.TreeNodeEx("Berserk", collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.berserkEnabled.Value =
                        ImGuiExtension.Checkbox("Enabled", CoPilot.instance.Settings.berserkEnabled.Value);
                    CoPilot.instance.Settings.berserkRange.Value =
                        ImGuiExtension.IntSlider("Mob Trigger Range", CoPilot.instance.Settings.berserkRange);
                    CoPilot.instance.Settings.berserkMinAny.Value =
                        ImGuiExtension.IntSlider("min Enemy Any", CoPilot.instance.Settings.berserkMinAny);
                    CoPilot.instance.Settings.berserkMinRare.Value =
                        ImGuiExtension.IntSlider("min Enemy Rare", CoPilot.instance.Settings.berserkMinRare);
                    CoPilot.instance.Settings.berserkMinUnique.Value = ImGuiExtension.IntSlider("min Enemy Unique",
                        CoPilot.instance.Settings.berserkMinUnique);
                    CoPilot.instance.Settings.berserkMinRage.Value = ImGuiExtension.IntSlider(
                        "min Rage. Even 0 will be > min set in Skill.", CoPilot.instance.Settings.berserkMinRage);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }
            
            try
            {
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.instance.Settings.generalCryEnabled ? green : red);
                ImGui.PushID(25);
                if (ImGui.TreeNodeEx("General's Cry", collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.generalCryEnabled.Value =
                        ImGuiExtension.Checkbox("Enabled", CoPilot.instance.Settings.generalCryEnabled.Value);
                    CoPilot.instance.Settings.generalCryTriggerRange.Value =
                        ImGuiExtension.IntSlider("Corpse Trigger Range", CoPilot.instance.Settings.generalCryTriggerRange);
                    CoPilot.instance.Settings.generalCryMinCorpse.Value =
                        ImGuiExtension.IntSlider("min Corpse", CoPilot.instance.Settings.generalCryMinCorpse);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }


            try
            {
                ImGui.PushStyleColor(ImGuiCol.Header, CoPilot.instance.Settings.minesEnabled ? green : red);
                ImGui.PushID(99999);
                if (ImGui.TreeNodeEx("Mines (Not Implemented)", collapsingHeaderFlags))
                {
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }

            try
            {
                if (CoPilot.instance.Settings.plagueBearer)
                    ImGui.PushStyleColor(ImGuiCol.Header, green);
                else
                    ImGui.PushStyleColor(ImGuiCol.Header, red);
                ImGui.PushID(23);
                if (ImGui.TreeNodeEx("Plague Bearer", collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.plagueBearer.Value = ImGuiExtension.Checkbox("Enabled", CoPilot.instance.Settings.plagueBearer.Value);
                    CoPilot.instance.Settings.plagueBearerRange.Value = ImGuiExtension.IntSlider("Enemy Range", CoPilot.instance.Settings.plagueBearerRange);
                    CoPilot.instance.Settings.plagueBearerMinEnemys.Value = ImGuiExtension.IntSlider("Minimum enemy Count", CoPilot.instance.Settings.plagueBearerMinEnemys);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }


            ImGui.End();
        }
    }
}