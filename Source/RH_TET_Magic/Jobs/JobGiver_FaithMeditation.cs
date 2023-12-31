using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace TheEndTimes_Magic
{
    public class JobGiver_FaithMeditation : ThinkNode_JobGiver
    {
        private FaithPoolCategory minCategory = FaithPoolCategory.Steady;

        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_FaithMeditation giverMeditation = (JobGiver_FaithMeditation)base.DeepCopy(resolve);
            giverMeditation.minCategory = this.minCategory;
            return (ThinkNode)giverMeditation;
        }

        public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
        {
            Need_FaithPool need = pawn.needs.TryGetNeed<Need_FaithPool>();
            if (need == null)
                return ThinkResult.NoJob;
            CompFaithUser comp = pawn.TryGetComp<CompFaithUser>();
            if (comp == null || (comp.FaithData.TicksUntilMeditate > Find.TickManager.TicksAbs || need.CurCategory > FaithPoolCategory.Strong))
                return ThinkResult.NoJob;
            return base.TryIssueJobPackage(pawn, jobParams);
        }

        public static IntVec3 ResolveMeditationLocation(Pawn pawn, out Thing padResult)
        {
            IntVec3 intVec3 = CellFinder.RandomClosewalkCellNearNotForbidden(pawn, 4);
            float num = 0.0f;
            padResult = (Thing)null;
            List<Thing> all = pawn.Map.listerThings.AllThings.FindAll((Predicate<Thing>)(t =>
            {
                if (t.Spawned)
                    return t is Building_MagicMeditationSpot;
                return false;
            }));
            if (all != null && all.Count > 0)
            {
                foreach (Thing thing in all)
                {
                    if (pawn.CanReserveAndReach((LocalTargetInfo)thing, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, false))
                    {
                        if ((double)num == 0.0)
                        {
                            intVec3 = thing.PositionHeld;
                            num = (float)pawn.PositionHeld.DistanceToSquared(thing.PositionHeld);
                            padResult = thing;
                        }
                        float squared = (float)pawn.PositionHeld.DistanceToSquared(thing.PositionHeld);
                        if ((double)squared < (double)num)
                        {
                            num = squared;
                            intVec3 = thing.PositionHeld;
                            padResult = thing;
                        }
                    }
                }
            }
            return intVec3;
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            Thing padResult = (Thing)null;
            IntVec3 intVec3 = JobGiver_FaithMeditation.ResolveMeditationLocation(pawn, out padResult);
            
            pawn.TryGetComp<CompFaithUser>().FaithData.TicksUntilMeditate = Find.TickManager.TicksGame + 6000;
            if (padResult != null && pawn.CanReserveAndReach(padResult, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, true))
                return new Job(DefDatabase<JobDef>.GetNamed("RH_TET_FaithMeditationJob", true), (LocalTargetInfo)padResult);
            return null;
        }
    }
}
