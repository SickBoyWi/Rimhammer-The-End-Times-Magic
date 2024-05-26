using RimWorld;
using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    public class PawnLeaper : PawnFlyer
    {
        private int positionLastComputedTick = -1;
        private static readonly Func<float, float> FlightCurveHeight = new Func<float, float>(GenMath.InverseParabola);
        private static readonly Func<float, float> FlightSpeed;
        private Material cachedShadowMaterial;
        private Vector3 groundPos;
        private Vector3 effectivePos;
        private float effectiveHeight;

        static PawnLeaper()
        {
            AnimationCurve animationCurve = new AnimationCurve();
            animationCurve.AddKey(0.0f, 0.0f);
            animationCurve.AddKey(0.1f, 0.15f);
            animationCurve.AddKey(1f, 1f);
            PawnLeaper.FlightSpeed = new Func<float, float>(animationCurve.Evaluate);
        }

        public override Vector3 DrawPos
        {
            get
            {
                this.RecomputePosition();
                return this.effectivePos;
            }
        }

        //protected override bool ValidateFlyer()
        //{
        //    return true;
        //}

        private void RecomputePosition()
        {
            if (this.positionLastComputedTick == this.ticksFlying)
                return;
            this.positionLastComputedTick = this.ticksFlying;
            float num = (float)this.ticksFlying / (float)this.ticksFlightTime;
            float t = PawnLeaper.FlightSpeed(num);
            this.effectiveHeight = PawnLeaper.FlightCurveHeight(t);
            this.groundPos = Vector3.Lerp(this.startVec, this.DestinationPos, t);
            Vector3 vector3_1 = new Vector3(0.0f, 0.0f, 2f);
            Vector3 vector3_2 = Altitudes.AltIncVect * this.effectiveHeight;
            double effectiveHeight = (double)this.effectiveHeight;
            Vector3 vector3_3 = vector3_1 * (float)effectiveHeight;
            this.effectivePos = this.groundPos + vector3_2 + vector3_3;
        }

        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            //this.RecomputePosition();
            this.DrawShadow(this.groundPos, this.effectiveHeight);
            if (this.CarriedThing == null || this.FlyingPawn == null)
                return;
            PawnRenderUtility.DrawCarriedThing(this.FlyingPawn, this.effectivePos, this.CarriedThing);
            //this.FlyingPawn.DrawAt(this.effectivePos, flip);
        }

        private void DrawShadow(Vector3 drawLoc, float height)
        {
            Material shadowMaterial = this.def.pawnFlyer.ShadowMaterial;
            if ((UnityEngine.Object)shadowMaterial == (UnityEngine.Object)null)
                return;
            float num = Mathf.Lerp(1f, 0.6f, height);
            Vector3 s = new Vector3(num, 1f, num);
            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetTRS(drawLoc, Quaternion.identity, s);
            Graphics.DrawMesh(MeshPool.plane10, matrix, shadowMaterial, 0);
        }

        protected override void RespawnPawn()
        {
            this.LandingEffects();
            base.RespawnPawn();
        }

        public override void Tick()
        {
            base.Tick();

            // TODO REMOVED FOR 1.4 - TEST
            //if (this.flightEffecter == null && this.def.pawnFlyer.flightEffecterDef != null)
            //{
            //    this.flightEffecter = this.def.pawnFlyer.flightEffecterDef.Spawn();
            //    this.flightEffecter.Trigger((TargetInfo)(Thing)this, TargetInfo.Invalid);
            //}
            //else
            //    this.flightEffecter?.EffectTick((TargetInfo)(Thing)this, TargetInfo.Invalid);
        }

        private void LandingEffects()
        {
            // TODO REMOVED FOR 1.4 - TEST
            //if (this.def.pawnFlyer.soundLanding != null)
            //    this.def.pawnFlyer.soundLanding.PlayOneShot((SoundInfo)new TargetInfo(this.Position, this.Map, false));
            FleckMaker.ThrowDustPuff(this.DestinationPos + Gen.RandomHorizontalVector(0.5f), this.Map, 2f);
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            base.Destroy(mode);
        }
    }
}
