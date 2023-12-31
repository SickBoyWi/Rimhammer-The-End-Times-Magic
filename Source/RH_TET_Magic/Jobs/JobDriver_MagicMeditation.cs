using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;

namespace TheEndTimes_Magic
{
    public class JobDriver_MagicMeditation : JobDriver
    {
        private Rot4 faceDir;


        public override bool TryMakePreToilReservations(bool somethin)
        {
            return this.pawn.Reserve(this.job.targetA, this.job, 1, -1, (ReservationLayerDef)null, false);
        }

        [DebuggerHidden]
        protected override IEnumerable<Toil> MakeNewToils()
        {
            JobDriver_MagicMeditation driverMagicMeditation = this;
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
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

                    if (this.pawn.TryGetComp<CompMagicUser>() == null && this.pawn.TryGetComp<CompFaithUser>() == null)
                        return;

                    if (this.pawn.IsMagicUser())
                    { 
                        CompMagicUser comp = this.pawn.GetComp<CompMagicUser>();
                        if (Find.TickManager.TicksGame % 60 == 0)
                            ++comp.MagicUserXP;

                        if (this.pawn.IsHashIntervalTick(100))
                        { 
                            FleckMaker.ThrowFireGlow(this.pawn.Position.ToVector3Shifted(), this.pawn.Map, 1.0f);
                        }

                        Need_MagicPool need = this.pawn.needs.TryGetNeed<Need_MagicPool>();
                        if (need == null)
                            return;
                        if ((double)need.CurLevel < 0.990000009536743)
                            need.CurLevel += 0.0005f;
                        else
                            this.EndJobWith(JobCondition.Succeeded);
                    }
                    else if (this.pawn.IsFaithUser())
                    {
                        CompFaithUser comp = this.pawn.GetComp<CompFaithUser>();

                        if (this.pawn.IsHashIntervalTick(100))
                        { 
                            FleckMaker.ThrowLightningGlow(this.pawn.Position.ToVector3(), this.pawn.Map, 1.0f);
                        }

                        Need_FaithPool need = this.pawn.needs.TryGetNeed<Need_FaithPool>();
                        if (need == null)
                            return;
                        if ((double)need.CurLevel < 0.990000009536743)
                            need.CurLevel += 0.0005f;
                        else
                            this.EndJobWith(JobCondition.Succeeded);
                    }
                    else if (this.pawn.IsAbilityUser())
                    {
                        CompAbilityActionUser comp = this.pawn.GetComp<CompAbilityActionUser>();

                        if (this.pawn.IsHashIntervalTick(100))
                        {
                            FleckMaker.ThrowSmoke(this.pawn.Position.ToVector3(), this.pawn.Map, 1.0f);
                        }

                        Need_AbilityPool need = this.pawn.needs.TryGetNeed<Need_AbilityPool>();
                        if (need == null)
                            return;
                        if ((double)need.CurLevel < 0.990000009536743)
                            need.CurLevel += 0.0005f;
                        else
                            this.EndJobWith(JobCondition.Succeeded);
                    }
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
