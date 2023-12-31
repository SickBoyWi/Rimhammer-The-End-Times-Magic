using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace TheEndTimes_Magic
{
    public abstract class JobDriver_DoBillMagic : Verse.AI.JobDriver_DoBill
    {
        protected override IEnumerable<Toil> MakeNewToils()
        {
            JobDriver_DoBillMagic f = this;
            f.AddEndCondition(new Func<JobCondition>(f.AddEndConditionOne));
            f.FailOnBurningImmobile<JobDriver_DoBillMagic>(TargetIndex.A);
            f.FailOn<JobDriver_DoBillMagic>(new Func<bool>(f.FailOnOne));
            yield return Toils_Reserve.Reserve(TargetIndex.A, 1, -1, (ReservationLayerDef)null);
            yield return Toils_Reserve.ReserveQueue(TargetIndex.B, 1, -1, (ReservationLayerDef)null);
            yield return Toils_JobTransforms.ExtractNextTargetFromQueue(TargetIndex.B, true);
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.Touch);
            yield return Toils_Haul.StartCarryThing(TargetIndex.B, false, false, false);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
            yield return Toils_JobTransforms.SetTargetToIngredientPlaceCell(TargetIndex.A, TargetIndex.B, TargetIndex.C);
            yield return Toils_Haul.PlaceHauledThingInCell(TargetIndex.A, (Toil)null, false, false);
            yield return Toils_Reserve.Reserve(TargetIndex.B, 1, -1, (ReservationLayerDef)null);
            yield return f.DoBill();
            yield return f.Store();
            yield return Toils_Reserve.Reserve(TargetIndex.C, 1, -1, (ReservationLayerDef)null);
            yield return Toils_Haul.CarryHauledThingToCell(TargetIndex.C);
            yield return Toils_Haul.PlaceHauledThingInCell(TargetIndex.C, (Toil)null, false, false);
            yield return Toils_Reserve.Release(TargetIndex.B);
            yield return Toils_Reserve.Release(TargetIndex.C);
            yield return Toils_Reserve.Release(TargetIndex.A);
        }

        private JobCondition AddEndConditionOne()
        {
            Thing thing = this.GetActor().jobs.curJob.GetTarget(TargetIndex.A).Thing;
            return thing is Building && !thing.Spawned ? JobCondition.Incompletable : JobCondition.Ongoing;
        }

        private bool FailOnOne()
        {
            IBillGiver thing = this.job.GetTarget(TargetIndex.A).Thing as IBillGiver;
            return thing != null && (this.job.bill.DeletedOrDereferenced || !thing.CurrentlyUsableForBills());
        }

        protected abstract Toil DoBill();

        private Toil Store()
        {
            return new Toil()
            {
                initAction = (Action)(() =>
                {
                    Verse.Thing resultingThing = this.job.GetTarget(TargetIndex.B).Thing;
                    if (this.job.bill.GetStoreMode() != BillStoreModeDefOf.DropOnFloor)
                    {
                        IntVec3 foundCell = IntVec3.Invalid;
                        if (this.job.bill.GetStoreMode() == BillStoreModeDefOf.BestStockpile)
                            StoreUtility.TryFindBestBetterStoreCellFor(resultingThing, this.pawn, this.pawn.Map, StoragePriority.Unstored, this.pawn.Faction, out foundCell, true);
                        else if (this.job.bill.GetStoreMode() == BillStoreModeDefOf.SpecificStockpile)
                            StoreUtility.TryFindBestBetterStoreCellForIn(resultingThing, this.pawn, this.pawn.Map, StoragePriority.Unstored, this.pawn.Faction, this.job.bill.GetStoreZone().slotGroup, out foundCell, true);
                        else
                            Log.ErrorOnce("Unknown store mode", 9158246, false);
                        if (foundCell.IsValid)
                        {
                            this.pawn.carryTracker.TryStartCarry(resultingThing, resultingThing.stackCount, true);
                            this.job.SetTarget(TargetIndex.C, (LocalTargetInfo)foundCell);
                            this.job.count = 99999;
                            return;
                        }
                    }
                    this.pawn.carryTracker.TryStartCarry(resultingThing, resultingThing.stackCount, true);
                    this.pawn.carryTracker.TryDropCarriedThing(this.pawn.Position, ThingPlaceMode.Near, out resultingThing, (Action<Verse.Thing, int>)null);
                    this.pawn.jobs.EndCurrentJob(JobCondition.Succeeded, true, true);
                })
            };
        }
    }
}
