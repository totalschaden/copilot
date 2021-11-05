using System;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;

namespace CoPilot
{
    internal static class SkillInfo
    {
        private static DateTime _lastUpdate = DateTime.MinValue;
        private static long _lastTime;
        internal static float _deltaTime;

        // Pseudo Skills
        internal static Skill autoMapTabber = new Skill();
        internal static Skill autoSummon = new Skill();

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
        }

        public static void GetDeltaTime()
        {
            var now = DateTime.Now.Ticks;
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
            if (!CoPilot.instance.Gcd())
                return false;

            if (!actorSkill.Stats.TryGetValue(GameStat.ManaCost, out var manaCost))
                manaCost = 0;

            if (CoPilot.instance.player.CurMana >= manaCost)
                return true;
            if (!CoPilot.instance.localPlayer.Stats.TryGetValue(GameStat.VirtualEnergyShieldProtectsMana,
                out var hasEldritchBattery))
                hasEldritchBattery = 0;

            return hasEldritchBattery > 0 && CoPilot.instance.player.CurES > manaCost;
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
            foreach (var skill in CoPilot.instance.skills)
            {
                if (!skill.IsOnSkillBar)
                    continue;
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
                }
            }
        }
    }
}