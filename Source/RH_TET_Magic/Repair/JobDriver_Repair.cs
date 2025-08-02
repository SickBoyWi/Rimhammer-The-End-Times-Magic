using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Verse;
using Verse.AI;

namespace TheEndTimes_Magic
{
    public class JobDriver_Repair : JobDriver_DoBillMagic
    {
        private readonly FieldInfo ApparelWornByCorpseInt = typeof(Apparel).GetField("wornByCorpseInt", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        private int hpPerTick;
        private float currIteration;
        private float currIterationProgress;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.hpPerTick, "hpPerTick", 1, false);
            Scribe_Values.Look<float>(ref this.currIteration, "currIteration", 1f, false);
            Scribe_Values.Look<float>(ref this.currIterationProgress, "workCycleProgress", 1f, false);
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override Toil DoBill()
        {
            Building_WorkTable tableThing = this.job.GetTarget(TargetIndex.A).Thing as Building_WorkTable;
            CompRefuelable refuelableComp = tableThing.GetComp<CompRefuelable>();
            Toil toil = new Toil();
            toil.initAction = (Action)(() =>
            {
                Verse.Thing thing = this.job.GetTarget(TargetIndex.B).Thing;
                this.job.bill.Notify_DoBillStarted(this.pawn);
                this.hpPerTick = (int)((double)thing.MaxHitPoints * .05D);
                this.currIterationProgress = this.currIteration = Math.Max(this.job.bill.recipe.workAmount, 10f);
            });
            toil.tickAction = (Action)(() =>
            {
                Verse.Thing thing = this.job.GetTarget(TargetIndex.B).Thing;
                if (thing == null || thing.Destroyed)
                    this.pawn.jobs.EndCurrentJob(JobCondition.Incompletable, true, true);
                this.currIterationProgress -= this.pawn.GetStatValue(StatDefOf.WorkToMake, true);
                tableThing.UsedThisTick();
                if (!tableThing.CurrentlyUsableForBills() || refuelableComp != null && !refuelableComp.HasFuel)
                    this.pawn.jobs.EndCurrentJob(JobCondition.Incompletable, true, true);
                if ((double)this.currIterationProgress > 0.0)
                    return;
                int val1 = thing.MaxHitPoints - thing.HitPoints;
                if (val1 > 0)
                    thing.HitPoints += Math.Min(val1, this.hpPerTick);
                float t = 0.5f;
                SkillDef workSkill = this.job.RecipeDef.workSkill;
                if (workSkill != null)
                {
                    SkillRecord skill = this.pawn.skills.GetSkill(workSkill);
                    if (skill != null)
                    {
                        t = (float)skill.Level / 20f;
                        skill.Learn(0.11f * this.job.RecipeDef.workSkillLearnFactor, false);
                    }
                }
                this.pawn.GainComfortFromCellIfPossible(1, false);
                if (thing.HitPoints == thing.MaxHitPoints)
                {
                    Apparel apparel = thing as Apparel;
                    if (apparel != null)
                        this.ApparelWornByCorpseInt.SetValue((object)apparel, (object)false);
                    this.job.bill.Notify_IterationCompleted(this.pawn, new List<Verse.Thing>()
                      {
                        thing
                      });
                    this.ReadyForNextToil();
                }
                else if (thing.HitPoints > thing.MaxHitPoints)
                {
                    thing.HitPoints = thing.MaxHitPoints;
                    this.pawn.jobs.EndCurrentJob(JobCondition.Incompletable, true, true);
                }
                this.currIterationProgress = this.currIteration;
            });
            toil.defaultCompleteMode = ToilCompleteMode.Never;
            toil.WithEffect((Func<EffecterDef>)(() => this.job.bill.recipe.effectWorking), TargetIndex.A);
            toil.PlaySustainerOrSound((Func<SoundDef>)(() => toil.actor.CurJob.bill.recipe.soundWorking));
            toil.WithProgressBar(TargetIndex.A, (Func<float>)(() =>
            {
                Verse.Thing thing = this.job.GetTarget(TargetIndex.B).Thing;
                return (float)thing.HitPoints / (float)thing.MaxHitPoints;
            }), false, 0.5f);
            toil.FailOn<Toil>((Func<bool>)(() =>
            {
                IBillGiver thing = this.job.GetTarget(TargetIndex.A).Thing as IBillGiver;
                if (this.job.bill.suspended || this.job.bill.DeletedOrDereferenced)
                    return true;
                if (thing != null)
                    return !thing.CurrentlyUsableForBills();
                return false;
            }));
            return toil;
        }
    }
}
