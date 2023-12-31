using Verse;

namespace TheEndTimes_Magic
{
    public class HediffComp_SeverityRecovery : HediffComp
    {
        private static readonly string RecoveringStatusSuffix = (string)"RH_TET_Magic_HediffRecovery_status_label".Translate();
        private float lastSeenSeverity;
        private int cooldownTicksLeft;

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (this.props is HediffCompProps_SeverityRecovery props)
            {
                if (this.cooldownTicksLeft > 0)
                    --this.cooldownTicksLeft;
                if ((double)this.parent.Severity > (double)this.lastSeenSeverity + (double)props.severityIncreaseDetectionThreshold)
                    this.cooldownTicksLeft = props.cooldownAfterSeverityIncrease;
                if (this.OffCooldown)
                {
                    this.parent.Severity -= props.severityRecoveryPerTick.RandomInRange;
                    if ((double)this.parent.Severity < 0.0)
                        this.parent.Severity = 0.0f;
                }
            }
            if ((double)this.parent.Severity > (double)this.parent.def.maxSeverity)
                this.parent.Severity = this.parent.def.maxSeverity;
            this.lastSeenSeverity = this.parent.Severity;
        }

        private bool OffCooldown
        {
            get
            {
                return this.cooldownTicksLeft <= 0;
            }
        }

        public override string CompLabelInBracketsExtra
        {
            get
            {
                return this.OffCooldown ? HediffComp_SeverityRecovery.RecoveringStatusSuffix : "";
            }
        }
    }
}
