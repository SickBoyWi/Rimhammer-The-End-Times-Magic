using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace TheEndTimes_Magic
{
    public class JobGiver_MagicMeditation : ThinkNode_JobGiver
    {
        private MagicPoolCategory minCategory = MagicPoolCategory.Steady;

        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_MagicMeditation giverMagicMeditation = (JobGiver_MagicMeditation)base.DeepCopy(resolve);
            giverMagicMeditation.minCategory = this.minCategory;
            return (ThinkNode)giverMagicMeditation;
        }

        public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
        {
            Need_MagicPool need = pawn.needs.TryGetNeed<Need_MagicPool>();
            if (need == null)
                return ThinkResult.NoJob;
            CompMagicUser comp = pawn.TryGetComp<CompMagicUser>();
            if (comp == null || (comp.MagicData.TicksUntilMeditate > Find.TickManager.TicksAbs || need.CurCategory > MagicPoolCategory.Strong))
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
                    if (pawn.CanReserveAndReach((LocalTargetInfo) thing, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, false))
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
            IntVec3 intVec3 = JobGiver_MagicMeditation.ResolveMeditationLocation(pawn, out padResult);
            
            pawn.TryGetComp<CompMagicUser>().MagicData.TicksUntilMeditate = Find.TickManager.TicksGame + 6000;
            if (padResult != null && pawn.CanReserveAndReach(padResult, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, true))
                return new Job(DefDatabase<JobDef>.GetNamed("RH_TET_MagicMeditationJob", true), (LocalTargetInfo)padResult);
            return null;
        }
    }
}
