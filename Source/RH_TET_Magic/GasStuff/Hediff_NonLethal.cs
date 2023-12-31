using Verse;

namespace TheEndTimes_Magic
{
    public class Hediff_NonLethal : HediffWithComps
    {
        public override float Severity
        {
            get
            {
                return base.Severity;
            }
            set
            {
                bool forceIncap = this.pawn.health.forceDowned;
                if (!(this.def is HediffDef_NonLethal def) || (double)Rand.Range(0.0f, 1f) >= (double)def.vanillaLethalityChance)
                    this.pawn.health.forceDowned = true;
                base.Severity = value;
                this.pawn.health.forceDowned = forceIncap;
            }
        }
    }
}
