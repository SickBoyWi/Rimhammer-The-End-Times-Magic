using RimWorld;
using Verse;

namespace TheEndTimes_Magic
{
    public class CompUseEffect_GiveHeDiff : CompUseEffect
    {
        private HediffDef HeDiff
        {
            get
            {
                return this.parent.GetComp<CompHeDiffPotion>().hediff;
            }
        }

        public override void DoEffect(Pawn user)
        {
            base.DoEffect(user);
            HediffDef hediff = this.HeDiff;
            user.health.AddHediff(hediff);
            if (!PawnUtility.ShouldSendNotificationAbout(user))
                return;
            Messages.Message((string)"RH_TET_Magic_PotionHediffUsed".Translate((NamedArgument)user.LabelShort, (NamedArgument)hediff.LabelCap, user.Named("USER")), (LookTargets)(Thing)user, MessageTypeDefOf.PositiveEvent, true);
        }

        public override AcceptanceReport CanBeUsedBy(Pawn p)
        {
            return base.CanBeUsedBy(p);
        }
    }
}
