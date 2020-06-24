using ExileCore;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoPilot
{
    internal class ImGuiDrawSettings 
    {
        internal static void DrawImGuiSettings()
        {
            System.Numerics.Vector4 green = new System.Numerics.Vector4(0.102f, 0.388f, 0.106f, 1.000f);
            System.Numerics.Vector4 red = new System.Numerics.Vector4(0.388f, 0.102f, 0.102f, 1.000f);

            ImGuiTreeNodeFlags collapsingHeaderFlags = ImGuiTreeNodeFlags.CollapsingHeader;
            ImGui.Text("Plugin by Totalschaden. https://github.com/totalschaden/copilot");

            try
            {
                // Input Keys
                ImGui.PushStyleColor(ImGuiCol.Header, green);
                ImGui.PushID(1000);
                if (ImGui.TreeNodeEx("Input Keys", collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.InputKey1.Value = ImGuiExtension.HotkeySelector("Use bound skill 2 Key: " + CoPilot.instance.Settings.InputKey1.Value, CoPilot.instance.Settings.InputKey1.Value);
                    CoPilot.instance.Settings.InputKey3.Value = ImGuiExtension.HotkeySelector("Use bound skill 4 Key: " + CoPilot.instance.Settings.InputKey3.Value, CoPilot.instance.Settings.InputKey3.Value);
                    CoPilot.instance.Settings.InputKey4.Value = ImGuiExtension.HotkeySelector("Use bound skill 5 Key: " + CoPilot.instance.Settings.InputKey4.Value, CoPilot.instance.Settings.InputKey4.Value);
                    CoPilot.instance.Settings.InputKey5.Value = ImGuiExtension.HotkeySelector("Use bound skill 6 Key: " + CoPilot.instance.Settings.InputKey5.Value, CoPilot.instance.Settings.InputKey5.Value);
                    CoPilot.instance.Settings.InputKey6.Value = ImGuiExtension.HotkeySelector("Use bound skill 7 Key: " + CoPilot.instance.Settings.InputKey6.Value, CoPilot.instance.Settings.InputKey6.Value);
                    CoPilot.instance.Settings.InputKey7.Value = ImGuiExtension.HotkeySelector("Use bound skill 8 Key: " + CoPilot.instance.Settings.InputKey7.Value, CoPilot.instance.Settings.InputKey7.Value);
                    CoPilot.instance.Settings.InputKey8.Value = ImGuiExtension.HotkeySelector("Use bound skill 9 Key: " + CoPilot.instance.Settings.InputKey8.Value, CoPilot.instance.Settings.InputKey8.Value);
                    CoPilot.instance.Settings.InputKey9.Value = ImGuiExtension.HotkeySelector("Use bound skill 10 Key: " + CoPilot.instance.Settings.InputKey9.Value, CoPilot.instance.Settings.InputKey9.Value);
                    CoPilot.instance.Settings.InputKey10.Value = ImGuiExtension.HotkeySelector("Use bound skill 11 Key: " + CoPilot.instance.Settings.InputKey10.Value, CoPilot.instance.Settings.InputKey10.Value);
                    CoPilot.instance.Settings.InputKey11.Value = ImGuiExtension.HotkeySelector("Use bound skill 12 Key: " + CoPilot.instance.Settings.InputKey11.Value, CoPilot.instance.Settings.InputKey11.Value);
                    CoPilot.instance.Settings.InputKey12.Value = ImGuiExtension.HotkeySelector("Use bound skill 13 Key: " + CoPilot.instance.Settings.InputKey12.Value, CoPilot.instance.Settings.InputKey12.Value);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }
            try
            {
                if (CoPilot.instance.Settings.confirm5)
                    ImGui.PushStyleColor(ImGuiCol.Header, green);
                else
                    ImGui.PushStyleColor(ImGuiCol.Header, red);
                ImGui.PushID(1001);
                if (ImGui.TreeNodeEx("Importand Informations", collapsingHeaderFlags))
                {
                    ImGui.Text("Go to Input Keys Tab, and set them According to your Ingame Settings -> Settings -> Input -> Use bound skill X");
                    ImGui.NewLine();
                    ImGui.Text("If your a noob dont make changes to -> Input Keys <- and change your Ingame Settings to the Keys that are predefined in the Plugin!");
                    ImGui.NewLine();
                    ImGui.Text("DO NOT ASSIGN MOUSE KEYS TO -> Input Keys <- in the Plugin !!!");
                    ImGui.NewLine();
                    ImGui.Text("The Top Left and Top Right Skill Slots are EXCLUDED!!! (Ingame bound skill 1 and 3) I recommend you use these for your Mouse.");
                    ImGui.NewLine();
                    ImGui.Text("The Plugin is currently forced to use Timers for Cooldowns as there is no Proper Skill Api for Ready/Cooldown.");
                    ImGui.NewLine();
                    ImGui.Text("I STRONGLY recommend that you add 80-100ms extra delay to your Skill Settings, so a Skill wont be skipped sometimes.");
                    ImGui.NewLine();


                    CoPilot.instance.Settings.confirm1.Value = ImGuiExtension.Checkbox("I did READ the text above.", CoPilot.instance.Settings.confirm1.Value);
                    if (!CoPilot.instance.Settings.confirm1)
                        return;
                    CoPilot.instance.Settings.confirm2.Value = ImGuiExtension.Checkbox("I did READ and UNDERSTAND the text above.", CoPilot.instance.Settings.confirm2.Value);
                    if (!CoPilot.instance.Settings.confirm2)
                        return;
                    CoPilot.instance.Settings.confirm3.Value = ImGuiExtension.Checkbox("I just READ it again and understood it.", CoPilot.instance.Settings.confirm3.Value);
                    if (!CoPilot.instance.Settings.confirm3)
                        return;
                    CoPilot.instance.Settings.confirm4.Value = ImGuiExtension.Checkbox("I did everything stated above and im ready to go.", CoPilot.instance.Settings.confirm4.Value);
                    if (!CoPilot.instance.Settings.confirm4)
                        return;
                    CoPilot.instance.Settings.confirm5.Value = ImGuiExtension.Checkbox("Let me use the Plugin already !!!", CoPilot.instance.Settings.confirm5.Value);
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
                ImGui.PushStyleColor(ImGuiCol.Header, new System.Numerics.Vector4(0.454f, 0.031f, 0.768f, 1.000f));
                ImGui.PushID(99999);
                if (ImGui.TreeNodeEx("Donation", collapsingHeaderFlags))
                {
                    ImGui.Text("I was asked to add a way to donate, as im not using any online payment services the only way would be Amazon.de Wishlist.");
                    ImGui.NewLine();
                    ImGui.Text("Thanks to anyone who is considering this.");
                    if(ImGui.Button("Open Wishlist"))
                    {
                        System.Diagnostics.Process.Start("https://www.amazon.de/hz/wishlist/ls/MZ543BDBC6PJ?ref_=wl_share");
                    }
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }

            try
            {
                // Auto Attack
                if (CoPilot.instance.Settings.autoAttackEnabled)
                    ImGui.PushStyleColor(ImGuiCol.Header, green);
                else
                    ImGui.PushStyleColor(ImGuiCol.Header, red);
                ImGui.PushID(1);
                if (ImGui.TreeNodeEx("Auto Attack with Cyclone / Nova / Flicker", collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.autoAttackEnabled.Value = ImGuiExtension.Checkbox("Enabled", CoPilot.instance.Settings.autoAttackEnabled.Value);
                    CoPilot.instance.Settings.autoAttackLeftMouseCheck.Value = ImGuiExtension.Checkbox("Pause on Left Mouse Pressed", CoPilot.instance.Settings.autoAttackLeftMouseCheck.Value);
                    CoPilot.instance.Settings.autoAttackPickItKey.Value = ImGuiExtension.HotkeySelector("PickIt Key: " + CoPilot.instance.Settings.autoAttackPickItKey.Value, CoPilot.instance.Settings.autoAttackPickItKey.Value);
                    CoPilot.instance.Settings.autoAttackRange.Value = ImGuiExtension.IntSlider("Range", CoPilot.instance.Settings.autoAttackRange);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }


            try
            {
                // Auto Golem
                if (CoPilot.instance.Settings.autoSummonEnabled)
                    ImGui.PushStyleColor(ImGuiCol.Header, green);
                else
                    ImGui.PushStyleColor(ImGuiCol.Header, red);
                ImGui.PushID(2);
                if (ImGui.TreeNodeEx("Auto Summon", collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.autoSummonEnabled.Value = ImGuiExtension.Checkbox("Auto Summons", CoPilot.instance.Settings.autoSummonEnabled.Value);
                    CoPilot.instance.Settings.autoGolemEnabled.Value = ImGuiExtension.Checkbox("Auto Golem", CoPilot.instance.Settings.autoGolemEnabled.Value);
                    CoPilot.instance.Settings.autoGolemChaosMax.Value = ImGuiExtension.IntSlider("Chaos Golems max.", CoPilot.instance.Settings.autoGolemChaosMax);
                    CoPilot.instance.Settings.autoGolemFireMax.Value = ImGuiExtension.IntSlider("Flame Golems max.", CoPilot.instance.Settings.autoGolemFireMax);
                    CoPilot.instance.Settings.autoGolemIceMax.Value = ImGuiExtension.IntSlider("Ice Golems max.", CoPilot.instance.Settings.autoGolemIceMax);
                    CoPilot.instance.Settings.autoGolemLightningMax.Value = ImGuiExtension.IntSlider("Lightning Golems max.", CoPilot.instance.Settings.autoGolemLightningMax);
                    CoPilot.instance.Settings.autoGolemRockMax.Value = ImGuiExtension.IntSlider("Stone Golems max.", CoPilot.instance.Settings.autoGolemRockMax);
                    CoPilot.instance.Settings.autoBoneMax.Value = ImGuiExtension.IntSlider("Carrion Golems max.", CoPilot.instance.Settings.autoBoneMax);
                    CoPilot.instance.Settings.autoGolemDropBearMax.Value = ImGuiExtension.IntSlider("Beastial Ursa Max.", CoPilot.instance.Settings.autoGolemDropBearMax);
                    CoPilot.instance.Settings.autoZombieEnabled.Value = ImGuiExtension.Checkbox("Auto Zombies", CoPilot.instance.Settings.autoZombieEnabled.Value);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }


            try
            {
                // Auto Quit
                if (CoPilot.instance.Settings.autoQuitEnabled)
                    ImGui.PushStyleColor(ImGuiCol.Header, green);
                else
                    ImGui.PushStyleColor(ImGuiCol.Header, red);
                ImGui.PushID(3);
                if (ImGui.TreeNodeEx("Auto Quit", collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.autoQuitEnabled.Value = ImGuiExtension.Checkbox("Enabled", CoPilot.instance.Settings.autoQuitEnabled.Value);
                    CoPilot.instance.Settings.hpPctQuit.Value = ImGuiExtension.FloatSlider("HP%", CoPilot.instance.Settings.hpPctQuit);
                    CoPilot.instance.Settings.esPctQuit.Value = ImGuiExtension.FloatSlider("ES%", CoPilot.instance.Settings.esPctQuit);
                    CoPilot.instance.Settings.forcedAutoQuit.Value = ImGuiExtension.HotkeySelector("Force Quit Key: " + CoPilot.instance.Settings.forcedAutoQuit.Value, CoPilot.instance.Settings.forcedAutoQuit.Value);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }


            try
            {
                // Blood Rage
                if (CoPilot.instance.Settings.bloodRageEnabled)
                    ImGui.PushStyleColor(ImGuiCol.Header, green);
                else
                    ImGui.PushStyleColor(ImGuiCol.Header, red);
                ImGui.PushID(4);
                if (ImGui.TreeNodeEx("Blood Rage", collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.bloodRageEnabled.Value = ImGuiExtension.Checkbox("Enabled", CoPilot.instance.Settings.bloodRageEnabled.Value);
                    CoPilot.instance.Settings.bloodRageDelay.Value = ImGuiExtension.IntSlider("Delay", CoPilot.instance.Settings.bloodRageDelay);
                    CoPilot.instance.Settings.bloodRageRange.Value = ImGuiExtension.IntSlider("Range", CoPilot.instance.Settings.bloodRageRange);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }


            try
            {
                if (CoPilot.instance.Settings.delveFlareEnabled)
                    ImGui.PushStyleColor(ImGuiCol.Header, green);
                else
                    ImGui.PushStyleColor(ImGuiCol.Header, red);
                ImGui.PushID(5);
                if (ImGui.TreeNodeEx("Delve Flare", collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.delveFlareEnabled.Value = ImGuiExtension.Checkbox("Enabled", CoPilot.instance.Settings.delveFlareEnabled);
                    CoPilot.instance.Settings.delveFlareKey.Value = ImGuiExtension.HotkeySelector("Key: " + CoPilot.instance.Settings.delveFlareKey.Value, CoPilot.instance.Settings.delveFlareKey.Value);
                    CoPilot.instance.Settings.delveFlareDebuffStacks.Value = ImGuiExtension.IntSlider("min. Debuff Stacks", CoPilot.instance.Settings.delveFlareDebuffStacks);
                    CoPilot.instance.Settings.delveFlareHpBelow.Value = ImGuiExtension.FloatSlider("HP%", CoPilot.instance.Settings.delveFlareHpBelow);
                    CoPilot.instance.Settings.delveFlareEsBelow.Value = ImGuiExtension.FloatSlider("ES%", CoPilot.instance.Settings.delveFlareEsBelow);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }


            try
            {
                // Doedre Effigy
                if (CoPilot.instance.Settings.doedreEffigyEnabled)
                    ImGui.PushStyleColor(ImGuiCol.Header, green);
                else
                    ImGui.PushStyleColor(ImGuiCol.Header, red);
                ImGui.PushID(6);
                if (ImGui.TreeNodeEx("Doedre Effigy", collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.doedreEffigyEnabled.Value = ImGuiExtension.Checkbox("Enabled", CoPilot.instance.Settings.doedreEffigyEnabled.Value);
                    CoPilot.instance.Settings.doedreEffigyDelay.Value = ImGuiExtension.IntSlider("min. Debuff Stacks", CoPilot.instance.Settings.doedreEffigyDelay);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }


            try
            {
                if (CoPilot.instance.Settings.divineIreEnabled)
                    ImGui.PushStyleColor(ImGuiCol.Header, green);
                else
                    ImGui.PushStyleColor(ImGuiCol.Header, red);
                ImGui.PushID(7);
                if (ImGui.TreeNodeEx("Divine Ire / Blade Flurry / Scourge Arrow", collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.divineIreEnabled.Value = ImGuiExtension.Checkbox("Enabled", CoPilot.instance.Settings.divineIreEnabled.Value);
                    CoPilot.instance.Settings.divineIreStacks.Value = ImGuiExtension.IntSlider("Stacks", CoPilot.instance.Settings.divineIreStacks);
                    CoPilot.instance.Settings.divineIreWaitForInfused.Value = ImGuiExtension.Checkbox("Wait for Infused Channeling Support", CoPilot.instance.Settings.divineIreWaitForInfused.Value);


                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }


            try
            {
                // Warcrys
                if (CoPilot.instance.Settings.enduringCryEnabled || CoPilot.instance.Settings.rallyingCryEnabled)
                    ImGui.PushStyleColor(ImGuiCol.Header, green);
                else
                    ImGui.PushStyleColor(ImGuiCol.Header, red);
                ImGui.PushID(8);
                if (ImGui.TreeNodeEx("Enduring Cry / Rallying Cry", collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.warCryCooldown.Value = ImGuiExtension.IntSlider("Cooldown", CoPilot.instance.Settings.warCryCooldown);
                    CoPilot.instance.Settings.warCryKeepRage.Value = ImGuiExtension.Checkbox("Keep Rage Up", CoPilot.instance.Settings.warCryKeepRage.Value);
                    CoPilot.instance.Settings.enduringCryEnabled.Value = ImGuiExtension.Checkbox("Enduring Cry Enabled", CoPilot.instance.Settings.enduringCryEnabled.Value);
                    CoPilot.instance.Settings.warCryTriggerRange.Value = ImGuiExtension.IntSlider("Range", CoPilot.instance.Settings.warCryTriggerRange);
                    CoPilot.instance.Settings.rallyingCryEnabled.Value = ImGuiExtension.Checkbox("Rallying Cry Enabled", CoPilot.instance.Settings.rallyingCryEnabled.Value);

                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }


            try
            {
                // Molten Shell / Steelskin / Bone Armour / Arcane Cloak
                if (CoPilot.instance.Settings.moltenShellEnabled)
                    ImGui.PushStyleColor(ImGuiCol.Header, green);
                else
                    ImGui.PushStyleColor(ImGuiCol.Header, red);
                ImGui.PushID(9);
                if (ImGui.TreeNodeEx("Molten Shell / Steelskin / Bone Armour / Arcane Cloak", collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.moltenShellEnabled.Value = ImGuiExtension.Checkbox("Enabled", CoPilot.instance.Settings.moltenShellEnabled.Value);
                    CoPilot.instance.Settings.moltenShellDelay.Value = ImGuiExtension.IntSlider("Cooldown", CoPilot.instance.Settings.moltenShellDelay);
                    CoPilot.instance.Settings.moltenShellRange.Value = ImGuiExtension.IntSlider("Range", CoPilot.instance.Settings.moltenShellRange);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }


            try
            {
                // Offerings
                if (CoPilot.instance.Settings.offeringsEnabled)
                    ImGui.PushStyleColor(ImGuiCol.Header, green);
                else
                    ImGui.PushStyleColor(ImGuiCol.Header, red);
                ImGui.PushID(10);
                if (ImGui.TreeNodeEx("Offerings (This will get you stuck in Animation for your Casttime !)", collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.offeringsEnabled.Value = ImGuiExtension.Checkbox("Enabled", CoPilot.instance.Settings.offeringsEnabled.Value);
                    CoPilot.instance.Settings.offeringsUseWhileCasting.Value = ImGuiExtension.Checkbox("Use while Casting/Attacking", CoPilot.instance.Settings.offeringsUseWhileCasting.Value);
                    CoPilot.instance.Settings.offeringsMinEnemys.Value = ImGuiExtension.IntSlider("min. Enemys", CoPilot.instance.Settings.offeringsMinEnemys);
                    CoPilot.instance.Settings.offeringsTriggerRange.Value = ImGuiExtension.IntSlider("Trigger Range", CoPilot.instance.Settings.offeringsTriggerRange);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }


            try
            {
                // Phaserun
                if (CoPilot.instance.Settings.phaserunEnabled)
                    ImGui.PushStyleColor(ImGuiCol.Header, green);
                else
                    ImGui.PushStyleColor(ImGuiCol.Header, red);
                ImGui.PushID(11);
                if (ImGui.TreeNodeEx("Phaserun", collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.phaserunEnabled.Value = ImGuiExtension.Checkbox("Enabled", CoPilot.instance.Settings.phaserunEnabled.Value);
                    CoPilot.instance.Settings.phaserunDelay.Value = ImGuiExtension.IntSlider("Cooldown", CoPilot.instance.Settings.phaserunDelay);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }




            try
            {
                // Speed Flasks
                if (CoPilot.instance.Settings.useSpeed4 || CoPilot.instance.Settings.useSpeed5)
                    ImGui.PushStyleColor(ImGuiCol.Header, green);
                else
                    ImGui.PushStyleColor(ImGuiCol.Header, red);
                ImGui.PushID(12);
                if (ImGui.TreeNodeEx("Speed Flask", collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.useSpeed4.Value = ImGuiExtension.Checkbox("Flask 4 Enabled", CoPilot.instance.Settings.useSpeed4.Value);
                    CoPilot.instance.Settings.useSpeed5.Value = ImGuiExtension.Checkbox("Flask 5 Enabled", CoPilot.instance.Settings.useSpeed5.Value);
                    CoPilot.instance.Settings.useSpeedMoving.Value = ImGuiExtension.Checkbox("Use when Moving", CoPilot.instance.Settings.useSpeedMoving.Value);
                    CoPilot.instance.Settings.useSpeedAttack.Value = ImGuiExtension.Checkbox("Use when Attacking", CoPilot.instance.Settings.useSpeedAttack.Value);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }


            try
            {
                // Vortex
                if (CoPilot.instance.Settings.vortexEnabled)
                    ImGui.PushStyleColor(ImGuiCol.Header, green);
                else
                    ImGui.PushStyleColor(ImGuiCol.Header, red);
                ImGui.PushID(13);
                if (ImGui.TreeNodeEx("Vortex", collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.vortexEnabled.Value = ImGuiExtension.Checkbox("Enabled", CoPilot.instance.Settings.vortexEnabled.Value);
                    CoPilot.instance.Settings.vortexDelay.Value = ImGuiExtension.IntSlider("Cooldown", CoPilot.instance.Settings.vortexDelay);
                    CoPilot.instance.Settings.vortexRange.Value = ImGuiExtension.IntSlider("Range", CoPilot.instance.Settings.vortexRange);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }

            try
            {
                if (CoPilot.instance.Settings.anyVaalEnabled)
                    ImGui.PushStyleColor(ImGuiCol.Header, green);
                else
                    ImGui.PushStyleColor(ImGuiCol.Header, red);
                ImGui.PushID(14);
                if (ImGui.TreeNodeEx("Any Vaal Skill", collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.anyVaalEnabled.Value = ImGuiExtension.Checkbox("Enabled", CoPilot.instance.Settings.anyVaalEnabled.Value);
                    CoPilot.instance.Settings.anyVaalMinEnemys.Value = ImGuiExtension.IntSlider("min. Enemys in Trigger Range", CoPilot.instance.Settings.anyVaalMinEnemys);
                    CoPilot.instance.Settings.anyVaalTriggerRange.Value = ImGuiExtension.IntSlider("Trigger Range", CoPilot.instance.Settings.anyVaalTriggerRange);
                    CoPilot.instance.Settings.anyVaalHpPct.Value = ImGuiExtension.FloatSlider("HP%", CoPilot.instance.Settings.anyVaalHpPct);
                    CoPilot.instance.Settings.anyVaalEsPct.Value = ImGuiExtension.FloatSlider("ES%", CoPilot.instance.Settings.anyVaalEsPct);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }

            try
            {
                if (CoPilot.instance.Settings.customEnabled)
                    ImGui.PushStyleColor(ImGuiCol.Header, green);
                else
                    ImGui.PushStyleColor(ImGuiCol.Header, red);
                ImGui.PushID(15);
                if (ImGui.TreeNodeEx("Custom Skill (Use any Skill not Supported here.)", collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.customEnabled.Value = ImGuiExtension.Checkbox("Enabled", CoPilot.instance.Settings.customEnabled.Value);
                    CoPilot.instance.Settings.customKey.Value = ImGuiExtension.HotkeySelector("Key: " + CoPilot.instance.Settings.customKey.Value, CoPilot.instance.Settings.customKey);
                    CoPilot.instance.Settings.customCooldown.Value = ImGuiExtension.IntSlider("Cooldown", CoPilot.instance.Settings.customCooldown);
                    CoPilot.instance.Settings.customMinEnemys.Value = ImGuiExtension.IntSlider("min. Enemys in Trigger Range", CoPilot.instance.Settings.customMinEnemys);
                    CoPilot.instance.Settings.customTriggerRange.Value = ImGuiExtension.IntSlider("Trigger Range", CoPilot.instance.Settings.customTriggerRange);
                    CoPilot.instance.Settings.customHpPct.Value = ImGuiExtension.FloatSlider("HP%", CoPilot.instance.Settings.customHpPct);
                    CoPilot.instance.Settings.customEsPct.Value = ImGuiExtension.FloatSlider("ES%", CoPilot.instance.Settings.customEsPct);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }

            try
            {
                if (CoPilot.instance.Settings.brandRecallEnabled)
                    ImGui.PushStyleColor(ImGuiCol.Header, green);
                else
                    ImGui.PushStyleColor(ImGuiCol.Header, red);
                ImGui.PushID(16);
                if (ImGui.TreeNodeEx("Brand Recall", collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.brandRecallEnabled.Value = ImGuiExtension.Checkbox("Enabled", CoPilot.instance.Settings.brandRecallEnabled.Value);
                    CoPilot.instance.Settings.brandRecallCooldown.Value = ImGuiExtension.IntSlider("Cooldown", CoPilot.instance.Settings.brandRecallCooldown);
                    CoPilot.instance.Settings.brandRecallMinEnemys.Value = ImGuiExtension.IntSlider("min. Enemys in Trigger Range", CoPilot.instance.Settings.brandRecallMinEnemys);
                    CoPilot.instance.Settings.brandRecallTriggerRange.Value = ImGuiExtension.IntSlider("Trigger Range", CoPilot.instance.Settings.brandRecallTriggerRange);
                    CoPilot.instance.Settings.brandRecallMinBrands.Value = ImGuiExtension.IntSlider("min. Brands Summoned", CoPilot.instance.Settings.brandRecallMinBrands);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }

            try
            {
                if (CoPilot.instance.Settings.tempestShieldEnabled)
                    ImGui.PushStyleColor(ImGuiCol.Header, green);
                else
                    ImGui.PushStyleColor(ImGuiCol.Header, red);
                ImGui.PushID(17);
                if (ImGui.TreeNodeEx("Tempest Shield", collapsingHeaderFlags))
                {
                    CoPilot.instance.Settings.tempestShieldEnabled.Value = ImGuiExtension.Checkbox("Enabled", CoPilot.instance.Settings.tempestShieldEnabled.Value);
                    CoPilot.instance.Settings.tempestShieldUseWhileCasting.Value = ImGuiExtension.Checkbox("Use while Casting/Attacking", CoPilot.instance.Settings.tempestShieldUseWhileCasting.Value);
                    CoPilot.instance.Settings.tempestShieldMinEnemys.Value = ImGuiExtension.IntSlider("min. Enemys in Trigger Range", CoPilot.instance.Settings.tempestShieldMinEnemys);
                    CoPilot.instance.Settings.tempestShieldTriggerRange.Value = ImGuiExtension.IntSlider("Trigger Range", CoPilot.instance.Settings.tempestShieldTriggerRange);
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError(e.ToString());
            }



            try
            {
                if (CoPilot.instance.Settings.minesEnabled)
                    ImGui.PushStyleColor(ImGuiCol.Header, green);
                else
                    ImGui.PushStyleColor(ImGuiCol.Header, red);
                ImGui.PushID(99999);
                if (ImGui.TreeNodeEx("Mines (Not Implemented)", collapsingHeaderFlags))
                {

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
