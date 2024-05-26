using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace TheEndTimes_Magic
{
    public class JobDriver_DoBillMagicBench : JobDriver
    {
        public float workLeft;
        public int billStartTick;
        public int ticksSpentDoingRecipeWork;
        public const PathEndMode GotoIngredientPathEndMode = PathEndMode.ClosestTouch;
        public const TargetIndex BillGiverInd = TargetIndex.A;
        public const TargetIndex IngredientInd = TargetIndex.B;
        public const TargetIndex IngredientPlaceCellInd = TargetIndex.C;

        public override string GetReport()
        {
            if (this.job.RecipeDef != null)
                return this.ReportStringProcessed(this.job.RecipeDef.jobString);
            return base.GetReport();
        }

        public IBillGiver BillGiver
        {
            get
            {
                IBillGiver thing = this.job.GetTarget(TargetIndex.A).Thing as IBillGiver;
                if (thing != null)
                    return thing;
                throw new InvalidOperationException("DoBill on non-Billgiver.");
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<float>(ref this.workLeft, "workLeft", 0.0f, false);
            Scribe_Values.Look<int>(ref this.billStartTick, "billStartTick", 0, false);
            Scribe_Values.Look<int>(ref this.ticksSpentDoingRecipeWork, "ticksSpentDoingRecipeWork", 0, false);
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (!this.pawn.Reserve(this.job.GetTarget(TargetIndex.A), this.job, 1, -1, (ReservationLayerDef)null, errorOnFailed))
                return false;
            this.pawn.ReserveAsManyAsPossible(this.job.GetTargetQueue(TargetIndex.B), this.job, 1, -1, (ReservationLayerDef)null);
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            JobDriver_DoBillMagicBench f = this;

            Thing thing = this.GetActor().jobs.curJob.GetTarget(TargetIndex.A).Thing;
            base.AddEndCondition(() => (thing is Building && !thing.Spawned) ? JobCondition.Incompletable : JobCondition.Ongoing);
            f.FailOnBurningImmobile<JobDriver_DoBillMagicBench>(TargetIndex.A);
            f.FailOn<JobDriver_DoBillMagicBench>(() => (this.job.GetTarget(TargetIndex.A).Thing is IBillGiver thing1 && (this.job.bill.DeletedOrDereferenced || !thing1.CurrentlyUsableForBills())));
            Toil gotoBillGiver = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
            yield return new Toil()
            {
                initAction = new Action(delegate ()
                {
                    if (this.job.targetQueueB == null || this.job.targetQueueB.Count != 1 || !(this.job.targetQueueB[0].Thing is UnfinishedThing thing2))
                        return;
                    thing2.BoundBill = (Bill_ProductionWithUft)this.job.bill;
                })
            };
            yield return Toils_Jump.JumpIf(gotoBillGiver, new Func<bool>(() => this.job.GetTargetQueue(TargetIndex.B).NullOrEmpty<LocalTargetInfo>()));
            Toil extract = Toils_JobTransforms.ExtractNextTargetFromQueue(TargetIndex.B, true);
            yield return extract;
            Toil getToHaulTarget = Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden<Toil>(TargetIndex.B).FailOnSomeonePhysicallyInteracting<Toil>(TargetIndex.B);
            yield return getToHaulTarget;
            yield return Toils_Haul.StartCarryThing(TargetIndex.B, true, false, true);
            yield return JobDriver_DoBillMagicBench.JumpToCollectNextIntoHandsForBill(getToHaulTarget, TargetIndex.B);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell).FailOnDestroyedOrNull<Toil>(TargetIndex.B);
            Toil findPlaceTarget = Toils_JobTransforms.SetTargetToIngredientPlaceCell(TargetIndex.A, TargetIndex.B, TargetIndex.C);
            yield return findPlaceTarget;
            yield return Toils_RecipeMagicBench.PlaceHauledThingInCell(TargetIndex.C, findPlaceTarget, false, false);
            yield return Toils_Jump.JumpIfHaveTargetInQueue(TargetIndex.B, extract);
            extract = (Toil)null;
            getToHaulTarget = (Toil)null;
            findPlaceTarget = (Toil)null;
            yield return gotoBillGiver;
            yield return Toils_RecipeMagicBench.MakeUnfinishedThingIfNeeded();
            yield return Toils_RecipeMagicBench.DoRecipeWork().FailOnDespawnedNullOrForbiddenPlacedThings(TargetIndex.A).FailOnCannotTouch<Toil>(TargetIndex.A, PathEndMode.InteractionCell);
            yield return Toils_RecipeMagicBench.FinishRecipeAndStartStoringProduct();
            if (!f.job.RecipeDef.products.NullOrEmpty<ThingDefCountClass>() || !f.job.RecipeDef.specialProducts.NullOrEmpty<SpecialProductType>())
            {
                JobDriver_DoBillMagicBench jobDriverDoBill = f;
                yield return Toils_Reserve.Reserve(TargetIndex.B, 1, -1, (ReservationLayerDef)null);
                findPlaceTarget = Toils_Haul.CarryHauledThingToCell(TargetIndex.B);
                yield return findPlaceTarget;
                yield return Toils_Haul.PlaceHauledThingInCell(TargetIndex.B, findPlaceTarget, true, true);
                Toil recount = new Toil();
                recount.initAction = (Action)(() =>
                {
                    Bill_Production bill = recount.actor.jobs.curJob.bill as Bill_Production;
                    if (bill == null || bill.repeatMode != BillRepeatModeDefOf.TargetCount)
                        return;
                    jobDriverDoBill.Map.resourceCounter.UpdateResourceCounts();
                });
                yield return recount;
                findPlaceTarget = (Toil)null;
            }
        }

        private static Toil JumpToCollectNextIntoHandsForBill(
          Toil gotoGetTargetToil,
          TargetIndex ind)
        {
            Toil toil = new Toil();
            toil.initAction = (Action)(() =>
            {
                Pawn actor = toil.actor;
                if (actor.carryTracker.CarriedThing == null)
                {
                    Log.Error("JumpToAlsoCollectTargetInQueue run on " + (object)actor + " who is not carrying something.");
                }
                else
                {
                    if (actor.carryTracker.Full)
                        return;
                    Job curJob = actor.jobs.curJob;
                    List<LocalTargetInfo> targetQueue = curJob.GetTargetQueue(ind);
                    if (targetQueue.NullOrEmpty<LocalTargetInfo>())
                        return;
                    for (int index = 0; index < targetQueue.Count; ++index)
                    {
                        if (GenAI.CanUseItemForWork(actor, targetQueue[index].Thing) && targetQueue[index].Thing.CanStackWith(actor.carryTracker.CarriedThing) && (double)(actor.Position - targetQueue[index].Thing.Position).LengthHorizontalSquared <= 64.0)
                        {
                            int num1 = actor.carryTracker.CarriedThing == null ? 0 : actor.carryTracker.CarriedThing.stackCount;
                            int num2 = Mathf.Min(Mathf.Min(curJob.countQueue[index], targetQueue[index].Thing.def.stackLimit - num1), actor.carryTracker.AvailableStackSpace(targetQueue[index].Thing.def));
                            if (num2 > 0)
                            {
                                curJob.count = num2;
                                curJob.SetTarget(ind, (LocalTargetInfo)targetQueue[index].Thing);
                                curJob.countQueue[index] -= num2;
                                if (curJob.countQueue[index] <= 0)
                                {
                                    curJob.countQueue.RemoveAt(index);
                                    targetQueue.RemoveAt(index);
                                }
                                actor.jobs.curDriver.JumpToToil(gotoGetTargetToil);
                                break;
                            }
                        }
                    }
                }
            });
            return toil;
        }
    }
}
