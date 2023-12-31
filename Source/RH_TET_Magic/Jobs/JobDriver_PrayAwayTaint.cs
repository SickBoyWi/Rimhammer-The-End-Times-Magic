using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;

namespace TheEndTimes_Magic
{
    public class JobDriver_PrayAwayTaint : JobDriver
    {
        private Rot4 faceDir;

        public override bool TryMakePreToilReservations(bool somethin)
        {
            return this.pawn.Reserve(this.job.targetA, this.job, 1, -1, (ReservationLayerDef)null, false);
        }

        [DebuggerHidden]
        protected override IEnumerable<Toil> MakeNewToils()
        {
            JobDriver_PrayAwayTaint driverPrayAwayTaint = this;
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.InteractionCell);
            yield return new Toil()
            {
                initAction = new Action((() =>
                    {
                        this.faceDir = !this.job.def.faceDir.IsValid ? Rot4.Random : this.job.def.faceDir;
                    })),
                tickAction = new Action((() =>
                {
                    this.pawn.rotationTracker.FaceCell(this.pawn.Position + this.faceDir.FacingCell);
                    this.pawn.GainComfortFromCellIfPossible();

                    if (this.pawn == null || this.pawn.needs == null)
                        return;

                    Hediff hediffOnPawn = null;
                    foreach (Hediff hed in this.pawn.health.hediffSet.hediffs)
                    {
                        String hedDefName = null;
                        if (hed != null && hed.def != null)
                            hedDefName = hed.def.defName;

                        if (hedDefName != null && (hedDefName.Equals("RH_TET_ChaosTaintBuildup")))
                        {
                            hediffOnPawn = hed;
                            break;
                        }
                    }

                    if (hediffOnPawn == null)
                        return;

                    if ((double)hediffOnPawn.Severity > (double).001)
                    {
                        if (Find.TickManager.TicksGame % 300 == 0)
                            FleckMaker.ThrowLightningGlow(pawn.Position.ToVector3Shifted(), pawn.Map, 1f);

                        hediffOnPawn.Severity -= 0.001f;
                    }
                    else
                        this.EndJobWith(JobCondition.Succeeded);
                })),
                defaultCompleteMode = ToilCompleteMode.Delay,
                defaultDuration = 1800
            };
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<Rot4>(ref this.faceDir, "faceDir", new Rot4(), false);
        }
    }
}
