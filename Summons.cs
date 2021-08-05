using System;
using System.Linq;
using ExileCore.PoEMemory.Components;

namespace CoPilot
{
    internal class Summons
    {
        internal int boneGolem;
        internal int chaosElemental;
        internal int dropBearUniqueSummoned;
        internal int fireElemental;
        internal int holyRelict;
        internal int iceElemental;

        private DateTime lastUpdate = DateTime.Now;
        internal int lightningGolem;
        internal int rockGolem;
        internal int zombies;

        internal void UpdateSummons()
        {
            if ((DateTime.Now - lastUpdate).TotalMilliseconds < 250)
                return;
            lastUpdate = DateTime.Now;

            chaosElemental = 0;
            fireElemental = 0;
            iceElemental = 0;
            lightningGolem = 0;
            rockGolem = 0;
            boneGolem = 0;
            dropBearUniqueSummoned = 0;
            zombies = 0;
            holyRelict = 0;

            foreach (var obj in CoPilot.instance.localPlayer.GetComponent<Actor>().DeployedObjects
                .Where(x => x != null && x.Entity != null && x.Entity.IsAlive))
                if (obj.Entity.Path.Contains("ChaosElemental"))
                    chaosElemental++;
                else if (obj.Entity.Path.Contains("FireElemental"))
                    fireElemental++;
                else if (obj.Entity.Path.Contains("IceElemental"))
                    iceElemental++;
                else if (obj.Entity.Path.Contains("LightningGolem"))
                    lightningGolem++;
                else if (obj.Entity.Path.Contains("RockGolem"))
                    rockGolem++;
                else if (obj.Entity.Path.Contains("BoneGolem"))
                    boneGolem++;
                else if (obj.Entity.Path.Contains("DropBearUniqueSummoned"))
                    dropBearUniqueSummoned++;
                else if (obj.Entity.Path.Contains("RaisedZombie"))
                    zombies++;
                else if (obj.Entity.Metadata.EndsWith("HolyLivingRelic")) holyRelict++;
        }

        public static float GetLowestMinionHpp()
        {
            float hpp = 100;
            foreach (var obj in CoPilot.instance.localPlayer.GetComponent<Actor>().DeployedObjects
                .Where(x => x != null && x.Entity != null && x.Entity.GetComponent<Life>() != null))
                if (obj.Entity.GetComponent<Life>().HPPercentage < hpp)
                    hpp = obj.Entity.GetComponent<Life>().HPPercentage;
            return hpp;
        }

        public static float GetAnimatedGuardianHpp()
        {
            const float hpp = 100;
            DeployedObject animatedGuardian = null;
            animatedGuardian = CoPilot.instance.localPlayer.GetComponent<Actor>().DeployedObjects.FirstOrDefault(x =>
                x.Entity?.GetComponent<Life>() != null && x.Entity.Path.Contains("AnimatedArmour"));
            return animatedGuardian?.Entity.GetComponent<Life>().HPPercentage ?? hpp;
        }
    }
}