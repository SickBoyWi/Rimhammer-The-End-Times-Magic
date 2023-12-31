using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace TheEndTimes_Magic
{
    // TODO - This is being added. Not actually live for people yet.
    public class JobGiver_PrayAwayTaint : ThinkNode_JobGiver
    {
        string hediff = "RH_TET_ChaosTaintBuildup";

        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_PrayAwayTaint giverPrayAwayTaint = (JobGiver_PrayAwayTaint)base.DeepCopy(resolve);
            return (ThinkNode)giverPrayAwayTaint;
        }

        public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
        {
            //Log.Error("1:" + pawn.Name.ToString());

            if (this.hediff == null || pawn == null || pawn.needs == null)
                return ThinkResult.NoJob;

            //Log.Error("2");

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
            //Log.Error("3");

            if (hediffOnPawn == null)
                return ThinkResult.NoJob;

            //Log.Error("4:" + (hediffOnPawn.Severity));
            if ((double)hediffOnPawn.Severity < (double).05)
                return ThinkResult.NoJob;

            //Log.Error("5");
            return base.TryIssueJobPackage(pawn, jobParams);
        }

        public static IntVec3 ResolvePrayerLocation(Pawn pawn, out Thing padResult)
        {
            IntVec3 intVec3 = CellFinder.RandomClosewalkCellNearNotForbidden(pawn, 4);
            float num = 0.0f;
            padResult = (Thing)null;
            List<Thing> all = pawn.Map.listerThings.AllThings.FindAll((Predicate<Thing>)(t =>
            {
                if (t.Spawned && t.def != null && t.def.defName != null)
                    return (t.def.defName.Equals("RH_TET_Magic_FaithIdol_Sigmar") || t.def.defName.Equals("RH_TET_Magic_FaithIdol_Shallya") || t.def.defName.Equals("RH_TET_Magic_FaithIdol_Ulricr"));
                return false;
            }));
            if (all != null && all.Count > 0)
            {
                foreach (Thing thing in all)
                {
                    if (pawn.CanReserveAndReach((LocalTargetInfo) thing, PathEndMode.InteractionCell, Danger.Some, 1, -1, null, false))
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
            //Log.Error("A:" + pawn.Name.ToString());

            Thing idol = (Thing)null;
            IntVec3 intVec3 = JobGiver_PrayAwayTaint.ResolvePrayerLocation(pawn, out idol);
            
            if (idol != null && pawn.CanReserveAndReach(idol, PathEndMode.OnCell, Danger.Some, 1, -1, null, true))
                return new Job(DefDatabase<JobDef>.GetNamed("RH_TET_Magic_PrayAwayTaint_Job", true), (LocalTargetInfo)idol);
            return null;
        }
    }
}
