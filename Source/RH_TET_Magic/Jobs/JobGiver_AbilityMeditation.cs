//using RimWorld;
//using System;
//using System.Collections.Generic;
//using Verse;
//using Verse.AI;

// TODO - Keep and consider. Abilities will be added at some point.
//namespace TheEndTimes_Magic
//{
//    public class JobGiver_AbilityMeditation : ThinkNode_JobGiver
//    {
//        private AbilityPoolCategory minCategory = AbilityPoolCategory.Steady;

//        public override ThinkNode DeepCopy(bool resolve = true)
//        {
//            JobGiver_AbilityMeditation giverAbilityMeditation = (JobGiver_AbilityMeditation)base.DeepCopy(resolve);
//            giverAbilityMeditation.minCategory = this.minCategory;
//            return (ThinkNode)giverAbilityMeditation;
//        }

//        //public override float GetPriority(Pawn pawn)
//        //{
//        //    Need_MagicPool need = pawn.needs.TryGetNeed<Need_MagicPool>();
//        //    return need == null || pawn.TryGetComp<CompMagicUser>().MagicData.TicksUntilMeditate > Find.TickManager.TicksAbs || (need.CurCategory > MagicPoolCategory.Steady || (double)need.CurLevelPercentage >= 0.5) ? 0.0f : 9.5f;
//        //}

//        public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
//        {
//            Need_AbilityPool need = pawn.needs.TryGetNeed<Need_AbilityPool>();
//            if (need == null)
//                return ThinkResult.NoJob;
//            CompAbilityActionUser comp = pawn.TryGetComp<CompAbilityActionUser>();
//            if (comp == null || (comp.AbilityActionData.TicksUntilMeditate > Find.TickManager.TicksAbs || need.CurCategory > AbilityPoolCategory.Strong))
//                return ThinkResult.NoJob;
//            return base.TryIssueJobPackage(pawn, jobParams);
//        }

//        public static IntVec3 ResolveMeditationLocation(Pawn pawn, out Thing padResult)
//        {
//            IntVec3 intVec3 = CellFinder.RandomClosewalkCellNearNotForbidden(pawn, 4);
//            float num = 0.0f;
//            padResult = (Thing)null;
//            List<Thing> all = pawn.Map.listerThings.AllThings.FindAll((Predicate<Thing>)(t =>
//            {
//                if (t.Spawned)
//                    return t is Building_MagicMeditationSpot;
//                return false;
//            }));
//            if (all != null && all.Count > 0)
//            {
//                foreach (Thing thing in all)
//                {
//                    if (pawn.CanReserveAndReach((LocalTargetInfo) thing, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, false))
//                    { 
//                        if ((double)num == 0.0)
//                        {
//                            intVec3 = thing.PositionHeld;
//                            num = (float)pawn.PositionHeld.DistanceToSquared(thing.PositionHeld);
//                            padResult = thing;
//                        }
//                        float squared = (float)pawn.PositionHeld.DistanceToSquared(thing.PositionHeld);
//                        if ((double)squared < (double)num)
//                        {
//                            num = squared;
//                            intVec3 = thing.PositionHeld;
//                            padResult = thing;
//                        }
//                    }
//                }
//            }
//            return intVec3;
//        }

//        protected override Job TryGiveJob(Pawn pawn)
//        {
//            Thing padResult = (Thing)null;
//            IntVec3 intVec3 = JobGiver_AbilityMeditation.ResolveMeditationLocation(pawn, out padResult);
            
//            pawn.TryGetComp<CompAbilityActionUser>().AbilityActionData.TicksUntilMeditate = Find.TickManager.TicksGame + 6000;
//            if (padResult != null && pawn.CanReserveAndReach(padResult, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, true))
//                return new Job(DefDatabase<JobDef>.GetNamed("RH_TET_AbilityMeditationJob", true), (LocalTargetInfo)padResult);
//            return null;
//        }
//    }
//}
