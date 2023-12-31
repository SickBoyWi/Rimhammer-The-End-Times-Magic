using RimWorld;
using Verse;
using Verse.AI;

/**
 * Original code from Star Wars Psionics mod.
 * */
namespace TheEndTimes_Magic
{
    public class ThinkNode_ModNeedPercentageAbove : ThinkNode_Conditional
    {
        private NeedDef need;
        private float threshold;

        public override ThinkNode DeepCopy(bool resolve = true)
        {
            ThinkNode_ModNeedPercentageAbove needPercentageAbove = (ThinkNode_ModNeedPercentageAbove)base.DeepCopy(resolve);
            needPercentageAbove.need = this.need;
            needPercentageAbove.threshold = this.threshold;
            return (ThinkNode)needPercentageAbove;
        }

        protected override bool Satisfied(Pawn pawn)
        {
            if (this.need == null || pawn == null || (pawn.needs == null || pawn.needs.TryGetNeed(this.need) == null))
                return false;
            
            return (double)pawn.needs.TryGetNeed(this.need).CurLevelPercentage > (double)this.threshold;
        }
    }
}
