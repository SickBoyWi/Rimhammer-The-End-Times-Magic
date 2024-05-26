using RimWorld;
using System;
using UnityEngine;
using Verse;
using Verse.AI;

namespace TheEndTimes_Magic
{
    public class Verb_Leap : Verb
    {
        private float cachedEffectiveRange = 24.9f;

        public override float EffectiveRange
        {
            get
            {
                return this.cachedEffectiveRange;
            }
        }

        public override bool MultiSelect
        {
            get
            {
                return true;
            }
        }

        public CompRechargeable RechargeableCompSource
        {
            get
            {
                return this.DirectOwner as CompRechargeable;
            }
        }

        protected override bool TryCastShot()
        {
            Pawn casterPawn = this.CasterPawn;
            IntVec3 cell = this.currentTarget.Cell;
            Map map = casterPawn.Map;
            PawnFlyer pawnFlyer = PawnFlyer.MakeFlyer(RH_TET_MagicDefOf.RH_TET_Magic_PawnLeaper, casterPawn, cell, RH_TET_MagicDefOf.RH_TET_Magic_FlightEffect, SoundDefOf.Interact_BeatFire);
            CompRechargeable rechargableComp = this.RechargeableCompSource;
            if (rechargableComp != null)
                rechargableComp.UsedOnce();
            if (pawnFlyer == null)
                return false;
            GenSpawn.Spawn((Thing)pawnFlyer, cell, map, WipeMode.Vanish);
            return true;
        }

        public override void OrderForceTarget(LocalTargetInfo target)
        {
            Map map = this.CasterPawn.Map;
            IntVec3 cell = RCellFinder.BestOrderedGotoDestNear(target.Cell, this.CasterPawn, new Predicate<IntVec3>(AcceptableDestination));
            Job job = JobMaker.MakeJob(JobDefOf.CastJump, (LocalTargetInfo)cell);
            job.verbToUse = (Verb)this;
            if (!this.CasterPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc))
                return;
            FleckMaker.Static(cell, map, FleckDefOf.FeedbackGoto, 1f);

            bool AcceptableDestination(IntVec3 c)
            {
                return Verb_Leap.ValidJumpTarget(map, c) && this.CanHitTargetFrom(this.caster.Position, (LocalTargetInfo)c);
            }
        }

        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
        {
            return this.caster != null && this.CanHitTarget(target) && (Verb_Leap.ValidJumpTarget(this.caster.Map, target.Cell));
        }

        public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
        {
            float num = this.EffectiveRange * this.EffectiveRange;
            IntVec3 cell = targ.Cell;
            return (double)this.caster.Position.DistanceToSquared(cell) <= (double)num && GenSight.LineOfSight(root, cell, this.caster.Map, false, (Func<IntVec3, bool>)null, 0, 0);
        }

        public override void OnGUI(LocalTargetInfo target)
        {
            if (this.CanHitTarget(target) && Verb_Leap.ValidJumpTarget(this.caster.Map, target.Cell))
                base.OnGUI(target);
            else
                GenUI.DrawMouseAttachment(TexCommand.CannotShoot);
        }

        public override void DrawHighlight(LocalTargetInfo target)
        {
            if (target.IsValid && Verb_Leap.ValidJumpTarget(this.caster.Map, target.Cell))
                GenDraw.DrawTargetHighlightWithLayer(target.CenterVector3, AltitudeLayer.MetaOverlays);
            GenDraw.DrawRadiusRing(this.caster.Position, this.EffectiveRange, Color.white, (Func<IntVec3, bool>)(c => GenSight.LineOfSight(this.caster.Position, c, this.caster.Map, false, (Func<IntVec3, bool>)null, 0, 0) && Verb_Leap.ValidJumpTarget(this.caster.Map, c)));
        }

        public static bool ValidJumpTarget(Map map, IntVec3 cell)
        {
            if (!cell.IsValid || !cell.InBounds(map) || (cell.Impassable(map) || !cell.Walkable(map)) || cell.Fogged(map))
                return false;
            Building edifice = cell.GetEdifice(map);
            return edifice == null || !(edifice is Building_Door buildingDoor) || buildingDoor.Open;
        }
    }
}
