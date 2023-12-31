using RimWorld;
using Verse;

namespace TheEndTimes_Magic
{
    public class CompTargetEffect_EndMentalState : CompTargetEffect
    {
        public override void DoEffectOn(Pawn user, Thing target)
        {
            Pawn pawn = null;
            try
            {
                pawn = (Pawn)target;
                if (pawn.Dead)
                    return;
            }
            catch
            {
                return;
            }
            
            if (pawn != null && pawn.mindState != null && pawn.mindState.mentalStateHandler != null
                && pawn.mindState.mentalStateHandler.CurStateDef != null
                && pawn.mindState.mentalStateHandler.CurStateDef.label != null)
            {
                Messages.Message("RH_TET_ClearMentalState".Translate(pawn.Name, pawn.mindState.mentalStateHandler.CurStateDef.label) + " "
                        + "RH_TET_Praise".Translate() + " " + "RH_TET_Nurgle".Translate() + "!", MessageTypeDefOf.PositiveEvent);
            
                pawn.mindState.mentalStateHandler.Reset();
            }
            else
            {
                HediffDef giveThemThis = HediffDefOf.Plague;

                try
                { 
                    pawn.health.AddHediff(giveThemThis);
                }
                catch
                {
                    // They must already have it. Ignore.
                }

                Messages.Message("RH_TET_MisusedNurgleCharm".Translate("RH_TET_Nurgle".Translate(), giveThemThis), MessageTypeDefOf.NegativeHealthEvent);
            }
        }
    }
}
