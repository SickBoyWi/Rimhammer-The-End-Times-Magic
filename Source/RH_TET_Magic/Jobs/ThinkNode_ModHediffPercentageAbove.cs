using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace TheEndTimes_Magic
{
    public class ThinkNode_ModHediffPercentageAbove : ThinkNode_Conditional
    {
        private string hediff;
        private float threshold;

        public override ThinkNode DeepCopy(bool resolve = true)
        {
            ThinkNode_ModHediffPercentageAbove hediffPercentageAbove = (ThinkNode_ModHediffPercentageAbove)base.DeepCopy(resolve);
            hediffPercentageAbove.hediff = this.hediff;
            hediffPercentageAbove.threshold = this.threshold;
            return (ThinkNode)hediffPercentageAbove;
        }

        protected override bool Satisfied(Pawn pawn)
        {
            if (this.hediff == null || pawn == null || pawn.needs == null)
                return false;

            Hediff hediffOnPawn = null;
            foreach (Hediff hed in pawn.health.hediffSet.hediffs)
            {
                String hedDefName = null;
                if (hed != null && hed.def != null)
                    hedDefName = hed.def.defName;

                if (hedDefName != null && (hedDefName.Equals(hediff)))
                {
                    hediffOnPawn = hed;
                    break;
                }
            }

            if (hediffOnPawn == null)
                return false;

            return (double)hediffOnPawn.Severity > (double)this.threshold;
        }
    }
}
