using RimWorld;
using RimWorld.Planet;
using Verse;

namespace TheEndTimes_Magic
{
    public class GasCloud_HediffGiver : GasCloud_AffectThing
    {
        private const int IncapGoodwillPenalty = 20;
        private const int KillGoodwillPenalty = 50;

        protected override void ApplyGasEffect(Verse.Thing thing, float strengthMultiplier)
        {
            if (!(thing is Pawn pawn))
                return;
            float sevOffset = this.Props.hediffSeverityPerGastick.RandomInRange * strengthMultiplier;
            bool downed = pawn.Downed;
            HealthUtility.AdjustSeverity(pawn, this.Props.hediffDef, sevOffset);
            if (pawn.Faction != null && pawn.Faction != Faction.OfPlayer)
            {
                if (pawn.Dead)
                    this.ApplyGoodwillPenalty(pawn.Faction, KillGoodwillPenalty);
                else if (!downed && pawn.Downed)
                    this.ApplyGoodwillPenalty(pawn.Faction, IncapGoodwillPenalty);
            }
        }

        private void ApplyGoodwillPenalty(Faction targetFaction, int goodwillLoss)
        {
            Faction ofPlayer = Faction.OfPlayer;
            int num = targetFaction.GoodwillWith(ofPlayer);
            targetFaction.TryAffectGoodwillWith(ofPlayer, -goodwillLoss, true, true, (HistoryEventDef)null, new GlobalTargetInfo?());
        }
    }
}
