using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;

namespace CoPilot
{
    [SuppressMessage("Interoperability", "CA1416:Plattformkompatibilität überprüfen")]
    internal static class SkillInfo
    {
        private static DateTime _lastUpdate = DateTime.MinValue;
        private static long _lastTime;
        internal static float _deltaTime;

        internal static List<Skill> CachedAuraSkills = new List<Skill>();

        // Pseudo Skills
        internal static Skill autoMapTabber = new Skill();
        internal static Skill autoSummon = new Skill();
        internal static Skill blessing = new Skill();

        internal static Skill vaalSkill = new Skill();

        // Skills
        internal static Skill enduringCry = new Skill();
        internal static Skill rallyingCry = new Skill();
        internal static Skill generalCry = new Skill();
        internal static Skill boneOffering = new Skill();
        internal static Skill spiritOffering = new Skill();
        internal static Skill fleshOffering = new Skill();
        internal static Skill phaserun = new Skill();
        internal static Skill moltenShell = new Skill();
        internal static Skill steelSkin = new Skill();
        internal static Skill boneArmour = new Skill();
        internal static Skill arcaneCloak = new Skill();
        internal static Skill bloodRage = new Skill();
        internal static Skill toxicRain = new Skill();
        internal static Skill chaosGolem = new Skill();
        internal static Skill flameGolem = new Skill();
        internal static Skill iceGolem = new Skill();
        internal static Skill lightningGolem = new Skill();
        internal static Skill stoneGolem = new Skill();
        internal static Skill carrionGolem = new Skill();
        internal static Skill ursaGolem = new Skill();
        internal static Skill vortex = new Skill();
        internal static Skill divineIre = new Skill();
        internal static Skill scourgeArror = new Skill();
        internal static Skill bladeFlurry = new Skill();
        internal static Skill doedreEffigy = new Skill();
        internal static Skill tempestShield = new Skill();
        internal static Skill brandRecall = new Skill();
        internal static Skill cyclone = new Skill();
        internal static Skill iceNova = new Skill();
        internal static Skill raiseZombie = new Skill();
        internal static Skill flickerStrike = new Skill();
        internal static Skill frostbolt = new Skill();
        internal static Skill convocation = new Skill();
        internal static Skill punishment = new Skill();
        internal static Skill bladeVortex = new Skill();
        internal static Skill plagueBearer = new Skill();
        internal static Skill bladeBlast = new Skill();
        internal static Skill holyRelict = new Skill();
        internal static Skill berserk = new Skill();
        internal static Skill sweep = new Skill();
        internal static Skill witherStep = new Skill();
        internal static Skill frenzy = new Skill();
        internal static Skill summonSpiders = new Skill();
        internal static Skill wardFlask = new Skill();
        internal static Skill summonSkeletons = new Skill();
        internal static Skill righteousFire = new Skill();
        internal static Skill auraPurityOfElements = new Skill();
        internal static Skill auraHatred = new Skill();
        internal static Skill auraZealotry = new Skill();
        internal static Skill auraMalevolence = new Skill();
        internal static Skill auraAnger = new Skill();
        internal static Skill auraDetermination = new Skill();
        internal static Skill auraGrace = new Skill();
        internal static Skill auraHaste = new Skill();
        internal static Skill auraPride = new Skill();
        internal static Skill auraWrath = new Skill();
        internal static Skill auraEnvy = new Skill();



        internal static void ResetSkills()
        {
            enduringCry = new Skill();
            generalCry = new Skill();
            rallyingCry = new Skill();
            boneOffering = new Skill();
            spiritOffering = new Skill();
            fleshOffering = new Skill();
            phaserun = new Skill();
            moltenShell = new Skill();
            steelSkin = new Skill();
            boneArmour = new Skill();
            arcaneCloak = new Skill();
            bloodRage = new Skill();
            toxicRain = new Skill();
            chaosGolem = new Skill();
            flameGolem = new Skill();
            iceGolem = new Skill();
            lightningGolem = new Skill();
            stoneGolem = new Skill();
            carrionGolem = new Skill();
            ursaGolem = new Skill();
            vortex = new Skill();
            divineIre = new Skill();
            scourgeArror = new Skill();
            bladeFlurry = new Skill();
            doedreEffigy = new Skill();
            tempestShield = new Skill();
            brandRecall = new Skill();
            cyclone = new Skill();
            iceNova = new Skill();
            raiseZombie = new Skill();
            flickerStrike = new Skill();
            frostbolt = new Skill();
            convocation = new Skill();
            punishment = new Skill();
            bladeVortex = new Skill();
            plagueBearer = new Skill();
            bladeBlast = new Skill();
            holyRelict = new Skill();
            berserk = new Skill();
            sweep = new Skill();
            witherStep = new Skill();
            frenzy = new Skill();
            summonSpiders = new Skill();
            wardFlask = new Skill();
            summonSkeletons = new Skill();
            righteousFire = new Skill();
            auraPurityOfElements = new Skill();
            auraHatred = new Skill();
            auraZealotry = new Skill();
            auraMalevolence = new Skill();
            auraAnger = new Skill();
            auraDetermination = new Skill();
            auraGrace = new Skill();
            auraHaste = new Skill();
            auraPride = new Skill();
            auraWrath = new Skill();
            auraEnvy = new Skill();
        }

        public static void GetDeltaTime()
        {
            var now = DateTime.Now.Ticks;
            // ReSharper disable once PossibleLossOfFraction
            float dT = (now - _lastTime) / 10000;
            _lastTime = now;
            _deltaTime = dT;
        }

        internal static bool ManageCooldown(Skill skill, ActorSkill actorSkill)
        {
            if (skill.Cooldown > 0)
            {
                skill.Cooldown = Helper.MoveTowards(skill.Cooldown, 0, _deltaTime);
                return false;
            }

            if (actorSkill.RemainingUses <= 0 && actorSkill.IsOnCooldown) return false;
            if (!CoPilot.Instance.Gcd())
                return false;

            if (!actorSkill.Stats.TryGetValue(GameStat.ManaCost, out var manaCost))
                manaCost = 0;

            if (CoPilot.Instance.player.CurMana >= manaCost)
                return true;
            if (!CoPilot.Instance.localPlayer.Stats.TryGetValue(GameStat.VirtualEnergyShieldProtectsMana,
                out var hasEldritchBattery))
                hasEldritchBattery = 0;

            //Include CurMana along with CurES
            return hasEldritchBattery > 0 && (CoPilot.Instance.player.CurES + CoPilot.Instance.player.CurMana) >= manaCost;
        }

        internal static bool ManageCooldown(Skill skill)
        {
            if (skill.Cooldown > 0)
            {
                skill.Cooldown = Helper.MoveTowards(skill.Cooldown, 0, _deltaTime);
                return false;
            }

            return true;
        }

        internal static void SetCooldown(Skill skill)
        {
        }

        internal static void UpdateSkillInfo(bool force = false)
        {
            if (!force && (DateTime.Now - _lastUpdate).TotalMilliseconds < 10000)
                return;
            _lastUpdate = DateTime.Now;
            CachedAuraSkills = new List<Skill>();
            foreach (var skill in CoPilot.Instance.skills)
            {
                switch (skill.InternalName)
                {
                    case "enduring_cry":
                        enduringCry.Id = skill.Id;
                        break;
                    case "spiritual_cry":
                        generalCry.Id = skill.Id;
                        break;
                    case "inspiring_cry":
                        rallyingCry.Id = skill.Id;
                        rallyingCry.BuffName = "inspiring_cry";
                        break;
                    case "bone_offering":
                        boneOffering.Id = skill.Id;
                        boneOffering.BuffName = "active_offering";
                        break;
                    case "spirit_offering":
                        spiritOffering.Id = skill.Id;
                        spiritOffering.BuffName = "active_offering";
                        break;
                    case "flesh_offering":
                        fleshOffering.Id = skill.Id;
                        fleshOffering.BuffName = "active_offering";
                        break;
                    case "new_phase_run":
                        phaserun.Id = skill.Id;
                        phaserun.BuffName = "new_phase_run";
                        break;
                    case "molten_shell_barrier":
                        moltenShell.Id = skill.Id;
                        moltenShell.BuffName = "fire_shield";
                        break;
                    case "steelskin":
                        steelSkin.Id = skill.Id;
                        steelSkin.BuffName = "quick_guard";
                        break;
                    case "bone_armour":
                        boneArmour.Id = skill.Id;
                        boneArmour.BuffName = "bone_armour";
                        break;
                    case "arcane_cloak":
                        arcaneCloak.Id = skill.Id;
                        arcaneCloak.BuffName = "arcane_cloak";
                        break;
                    case "blood_rage":
                        bloodRage.Id = skill.Id;
                        bloodRage.BuffName = "blood_rage";
                        break;
                    case "rain_of_spores":
                        toxicRain.Id = skill.Id;
                        break;
                    case "summon_chaos_elemental":
                        chaosGolem.Id = skill.Id;
                        break;
                    case "summon_fire_elemental":
                        flameGolem.Id = skill.Id;
                        break;
                    case "summon_ice_elemental":
                        iceGolem.Id = skill.Id;
                        break;
                    case "summon_lightning_golem":
                        lightningGolem.Id = skill.Id;
                        break;
                    case "summon_rock_golem":
                        stoneGolem.Id = skill.Id;
                        break;
                    case "summon_bone_golem":
                        carrionGolem.Id = skill.Id;
                        break;
                    case "summon_beastial_ursa":
                        ursaGolem.Id = skill.Id;
                        break;
                    case "frost_bolt_nova":
                        vortex.Id = skill.Id;
                        break;
                    case "divine_tempest":
                        divineIre.Id = skill.Id;
                        divineIre.BuffName = "divine_tempest_stage";
                        break;
                    case "virulent_arrow":
                        scourgeArror.Id = skill.Id;
                        scourgeArror.BuffName = "virulent_arrow_counter";
                        break;
                    case "charged_attack_channel":
                        bladeFlurry.Id = skill.Id;
                        bladeFlurry.BuffName = "charged_attack";
                        break;
                    case "curse_pillar":
                        doedreEffigy.Id = skill.Id;
                        break;
                    case "tempest_shield":
                        tempestShield.Id = skill.Id;
                        tempestShield.BuffName = "lightning_shield";
                        break;
                    case "sigil_recall":
                        brandRecall.Id = skill.Id;
                        break;
                    case "cyclone_channelled":
                        cyclone.Id = skill.Id;
                        break;
                    case "ice_nova":
                        iceNova.Id = skill.Id;
                        break;
                    case "raise_zombie":
                        raiseZombie.Id = skill.Id;
                        break;
                    case "flicker_strike":
                        flickerStrike.Id = skill.Id;
                        break;
                    case "frost_bolt":
                        frostbolt.Id = skill.Id;
                        break;
                    case "convocation":
                        convocation.Id = skill.Id;
                        break;
                    case "new_punishment":
                        punishment.Id = skill.Id;
                        punishment.BuffName = "curse_newpunishment";
                        break;
                    case "new_new_blade_vortex":
                        bladeVortex.Id = skill.Id;
                        bladeVortex.BuffName = "new_new_blade_vortex";
                        break;
                    case "blade_burst":
                        bladeBlast.Id = skill.Id;
                        break;
                    case "summon_relic":
                        holyRelict.Id = skill.Id;
                        break;
                    case "berserk":
                        berserk.Id = skill.Id;
                        berserk.BuffName = "berserk";
                        break;
                    case "sweep":
                        sweep.Id = skill.Id;
                        break;
                    case "slither":
                        witherStep.Id = skill.Id;
                        witherStep.BuffName = "slither";
                        break;
                    case "frenzy":
                        frenzy.Id = skill.Id;
                        break;
                    case "corrosive_shroud":
                        plagueBearer.Id = skill.Id;
                        plagueBearer.BuffName = "corrosive_shroud";
                        break;
                    case "triggered_summon_spider":
                        summonSpiders.Id = skill.Id;
                        summonSpiders.BuffName = "summoned_spider_buff";
                        break;
                    case "summon_skeletons":
                        summonSkeletons.Id = skill.Id;
                        break;
                    case "righteous_fire":
                        righteousFire.Id = skill.Id;
                        righteousFire.BuffName = "righteous_fire_aura";
                        break;
                    case "purity":
                        auraPurityOfElements.Id = skill.Id;
                        auraPurityOfElements.BuffName = "player_aura_resists";
                        if (skill.Stats.TryGetValue(GameStat.SkillIsBlessingSkill, out var isBlessing))
                        {
                            if (isBlessing > 0)
                                auraPurityOfElements.IsBlessing = 1;
                            CachedAuraSkills.Add(auraPurityOfElements);
                        }
                        break;
                    case "hatred":
                        auraHatred.Id = skill.Id;
                        auraHatred.BuffName = "player_aura_cold_damage";
                        if (skill.Stats.TryGetValue(GameStat.SkillIsBlessingSkill, out isBlessing))
                        {
                            if (isBlessing > 0)
                                auraHatred.IsBlessing = 1;
                            CachedAuraSkills.Add(auraHatred);
                        }
                        break;
                    case "spell_damage_aura":
                        auraZealotry.Id = skill.Id;
                        auraZealotry.BuffName = "player_aura_spell_damage";
                        if (skill.Stats.TryGetValue(GameStat.SkillIsBlessingSkill, out isBlessing))
                        {
                            if (isBlessing > 0)
                                auraZealotry.IsBlessing = 1;
                            CachedAuraSkills.Add(auraZealotry);
                        }
                        break;
                    case "damage_over_time_aura":
                        auraMalevolence.Id = skill.Id;
                        auraMalevolence.BuffName = "player_aura_damage_over_time";
                        if (skill.Stats.TryGetValue(GameStat.SkillIsBlessingSkill, out isBlessing))
                        {
                            if (isBlessing > 0)
                                auraMalevolence.IsBlessing = 1;
                            CachedAuraSkills.Add(auraMalevolence);
                        }
                        break;
                    case "anger":
                        auraAnger.Id = skill.Id;
                        auraAnger.BuffName = "player_aura_fire_damage";
                        if (skill.Stats.TryGetValue(GameStat.SkillIsBlessingSkill, out isBlessing))
                        {
                            if (isBlessing > 0)
                                auraAnger.IsBlessing = 1;
                            CachedAuraSkills.Add(auraAnger);
                        }
                        break;
                    case "determination":
                        auraDetermination.Id = skill.Id;
                        auraDetermination.BuffName = "player_aura_armour";
                        if (skill.Stats.TryGetValue(GameStat.SkillIsBlessingSkill, out isBlessing))
                        {
                            if (isBlessing > 0)
                                auraDetermination.IsBlessing = 1;
                            CachedAuraSkills.Add(auraDetermination);
                        }
                        break;
                    case "grace":
                        auraGrace.Id = skill.Id;
                        auraGrace.BuffName = "player_aura_evasion";
                        if (skill.Stats.TryGetValue(GameStat.SkillIsBlessingSkill, out isBlessing))
                        {
                            if (isBlessing > 0)
                                auraGrace.IsBlessing = 1;
                            CachedAuraSkills.Add(auraGrace);
                        }
                        break;
                    case "haste":
                        auraHaste.Id = skill.Id;
                        auraHaste.BuffName = "player_aura_speed";
                        if (skill.Stats.TryGetValue(GameStat.SkillIsBlessingSkill, out isBlessing))
                        {
                            if (isBlessing > 0)
                                auraHaste.IsBlessing = 1;
                            CachedAuraSkills.Add(auraHaste);
                        }
                        break;
                    case "physical_damage_aura":
                        auraPride.Id = skill.Id;
                        auraPride.BuffName = "player_physical_damage_aura";
                        if (skill.Stats.TryGetValue(GameStat.SkillIsBlessingSkill, out isBlessing))
                        {
                            if (isBlessing > 0)
                                auraPride.IsBlessing = 1;
                            CachedAuraSkills.Add(auraPride);
                        }
                        break;
                    case "wrath":
                        auraWrath.Id = skill.Id;
                        auraWrath.BuffName = "player_aura_lightning_damage";
                        if (skill.Stats.TryGetValue(GameStat.SkillIsBlessingSkill, out isBlessing))
                        {
                            if (isBlessing > 0)
                                auraWrath.IsBlessing = 1;
                            CachedAuraSkills.Add(auraWrath);
                        }
                        break;
                    case "envy":
                        auraEnvy.Id = skill.Id;
                        auraEnvy.BuffName = "envy_chaos_damage";
                        if (skill.Stats.TryGetValue(GameStat.SkillIsBlessingSkill, out isBlessing))
                        {
                            if (isBlessing > 0)
                                auraEnvy.IsBlessing = 1;
                            CachedAuraSkills.Add(auraEnvy);
                        }
                        break;

                }
            }
        }
    }
}
