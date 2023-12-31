using RimWorld;
using System.Collections.Generic;
using Verse;

namespace TheEndTimes_Magic
{
    public class CompProperties_Rechargeable : CompProperties
    {
        public int baseRechargeTicks = 2500;
        public bool displayGizmoWhileUndrafted = true;
        public bool displayGizmoWhileDrafted = true;
        public string chargeNoun = "charge";
        public KeyBindingDef hotKey;

        public NamedArgument ChargeNounArgument
        {
            get
            {
                return this.chargeNoun.Named("CHARGENOUN");
            }
        }

        public CompProperties_Rechargeable()
        {
            this.compClass = typeof(CompRechargeable);
        }

        public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
        {
            foreach (string configError in base.ConfigErrors(parentDef))
                yield return configError;
        }

        public override IEnumerable<StatDrawEntry> SpecialDisplayStats(
          StatRequest req)
        {
            foreach (StatDrawEntry specialDisplayStat in base.SpecialDisplayStats(req))
                yield return specialDisplayStat;
        }
    }
}
