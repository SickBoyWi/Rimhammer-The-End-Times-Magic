using RimWorld;
using RimWorld.Planet;
using Verse;

namespace TheEndTimes_Magic
{
    public class HediffComp_MentalSuppression : HediffComp
    {
        public override bool CompShouldRemove
        {
            get
            {
                if (this.Pawn.SpawnedOrAnyParentSpawned)
                {
                    GameCondition_WindsOfMagic activeCondition = this.Pawn.MapHeld.gameConditionManager.GetActiveCondition<GameCondition_WindsOfMagic>();
                    if (activeCondition != null)
                        return false;
                }
                //TODO: make happen on world map?
                //else if (this.Pawn.IsCaravanMember())
                //{
                //    bool flag = true;
                //    foreach (Site site in Find.World.worldObjects.Sites)
                //    {
                //        foreach (SitePart part in site.parts)
                //        {
                //            if (part.def.Worker is SitePartWorker_ConditionCauser_PsychicSuppressor)
                //            {
                                
                //                //CompCauseGameCondition_PsychicSuppression comp = part.conditionCauser.TryGetComp<CompCauseGameCondition_PsychicSuppression>();
                //                if (comp.ConditionDef.conditionClass == typeof(GameCondition_MentalSuppression) && comp.InAoE(this.Pawn.GetCaravan().Tile))
                //                    flag = false;
                //            }
                //        }
                //    }
                //    return flag;
                //}
                return true;
            }
        }
    }
}
