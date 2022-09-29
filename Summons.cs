using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ExileCore.PoEMemory.Components;

namespace CoPilot
{
    [SuppressMessage("Interoperability", "CA1416:Plattformkompatibilität überprüfen")]
    internal class Summons
    {
        public static float GetLowestMinionHpp()
        {
            float hpp = 100;
            foreach (var obj in CoPilot.Instance.localPlayer.GetComponent<Actor>().DeployedObjects
                .Where(x => x?.Entity?.GetComponent<Life>() != null))
                if (obj.Entity.GetComponent<Life>().HPPercentage < hpp)
                    hpp = obj.Entity.GetComponent<Life>().HPPercentage;
            return hpp;
        }

        public static float GetAnimatedGuardianHpp()
        {
            const float hpp = 100;
            DeployedObject animatedGuardian = null;
            animatedGuardian = CoPilot.Instance.localPlayer.GetComponent<Actor>().DeployedObjects.FirstOrDefault(x =>
                x?.Entity?.GetComponent<Life>() != null && x.Entity.Path.Contains("AnimatedArmour"));
            return animatedGuardian?.Entity.GetComponent<Life>().HPPercentage ?? hpp;
        }
    }
}