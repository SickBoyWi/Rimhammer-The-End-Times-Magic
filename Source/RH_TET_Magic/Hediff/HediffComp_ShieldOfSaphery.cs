using RimWorld;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    [StaticConstructorOnStartup]
    public class HediffComp_ShieldOfSaphery : HediffComp
    {
        private static readonly Material BubbleMat = MaterialPool.MatFrom("Magic/RH_TET_HighShieldSaphery", ShaderDatabase.Transparent);

        private int ticksToReset = -1;
        private int lastKeepDisplayTick = -9999;
        private int lastAbsorbDamageTick = -9999;
        private int StartingTicksToReset = 3200;
        private float EnergyOnReset = 0.2f;
        private float EnergyLossPerDamage = 0.027f;
        private int KeepDisplayingTicks = 1000;
        private const float MinDrawSize = 1.5f;
        private const float MaxDrawSize = 1.7f;
        private const float MaxDamagedJitterDist = 0.05f;
        private const int JitterDurationTicks = 8;
        private float energy;
        private Vector3 impactAngleVect;
        private bool initialized = true;

        public float EnergyMax
        {
            get
            {
                return 1.1f;
            }
        }

        public string labelCap
        {
            get
            {
                return this.Def.LabelCap;
            }
        }

        public string label
        {
            get
            {
                return this.Def.label;
            }
        }

        private float EnergyGainPerTick
        {
            get
            {
                return 0.002166667f;
            }
        }

        public float Energy
        {
            get
            {
                return this.energy;
            }
        }

        public ShieldState ShieldState
        {
            get
            {
                return this.ticksToReset > 0 ? ShieldState.Resetting : ShieldState.Active;
            }
        }

        private bool ShouldDisplay
        {
            get
            {
                if ((this.Pawn.Dead || this.Pawn.Downed || this.Pawn.IsPrisonerOfColony && (this.Pawn.MentalStateDef == null || !this.Pawn.MentalStateDef.IsAggro)) && !this.Pawn.Faction.HostileTo(Faction.OfPlayer))
                    return Find.TickManager.TicksGame < this.lastKeepDisplayTick + this.KeepDisplayingTicks;
                return true;
            }
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look<float>(ref this.energy, "energy", 0.0f, false);
            Scribe_Values.Look<int>(ref this.ticksToReset, "ticksToReset", -1, false);
            Scribe_Values.Look<int>(ref this.lastKeepDisplayTick, "lastKeepDisplayTick", 0, false);
        }

        [DebuggerHidden]
        public IEnumerable<Gizmo> GetWornGizmos()
        {
            HediffComp_ShieldOfSaphery hediffCompShield = this;
            if (Find.Selector.SingleSelectedThing == hediffCompShield.Pawn)
                yield return (Gizmo)new Gizmo_HediffShieldSapheryStatus()
                {
                    shield = hediffCompShield
                };
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);

            if (!initialized)
            {
                initialized = true;
                Pawn.GetGizmos();
            }

            if (this.Pawn == null || this.Pawn.Dead)
            {
                this.energy = 0.0f;
                this.lastKeepDisplayTick = -1;
            }
            else if (this.ShieldState == ShieldState.Resetting)
            {
                --this.ticksToReset;
                if (this.ticksToReset > 0)
                    return;
                this.Reset();
            }
            else
            {
                //Log.Error("ShieldState:" + this.ShieldState + "," + "Energy:" + this.energy);
                if (this.ShieldState != ShieldState.Active)
                    return;
                this.energy += this.EnergyGainPerTick;
                if ((double)this.energy <= (double)this.EnergyMax)
                    return;
                this.energy = this.EnergyMax;
            }
        }

        public bool CheckPreAbsorbDamage(DamageInfo dinfo)
        {
            if (this.ShieldState != ShieldState.Active)// || (dinfo.Instigator == null || dinfo.Instigator.Position.AdjacentTo8WayOrInside(this.Pawn.Position)) && !dinfo.Def.isExplosive)
                return false;
            if (dinfo.Instigator != null)
            {
                AttachableThing instigator = dinfo.Instigator as AttachableThing;
                if (instigator != null && instigator.parent == this.Pawn)
                    return false;
            }
            this.energy -= dinfo.Amount * this.EnergyLossPerDamage;
            if ((double)this.energy < 0.0)
                this.Break();
            else
                this.AbsorbedDamage(dinfo);
            return true;
        }

        public void KeepDisplaying()
        {
            this.lastKeepDisplayTick = Find.TickManager.TicksGame;
        }

        private void AbsorbedDamage(DamageInfo dinfo)
        {
            SoundDefOf.EnergyShield_AbsorbDamage.PlayOneShot((SoundInfo)new TargetInfo(this.Pawn.Position, this.Pawn.Map, false));
            this.impactAngleVect = Vector3Utility.HorizontalVectorFromAngle(dinfo.Angle);

            Vector3 loc = this.Pawn.TrueCenter() + (this.impactAngleVect.RotatedBy(180f) * 0.5f);

            float scale = Mathf.Min(10f, (float)(2.0 + (double)dinfo.Amount / 10.0));
            FleckMaker.Static(loc, this.Pawn.Map, FleckDefOf.ExplosionFlash, scale);
            int num = (int)scale;
            for (int index = 0; index < num; ++index)
                FleckMaker.ThrowDustPuff(loc, this.Pawn.Map, Rand.Range(0.8f, 1.2f));
            this.lastAbsorbDamageTick = Find.TickManager.TicksGame;
            this.KeepDisplaying();
        }

        private void Break()
        {
            SoundDefOf.EnergyShield_Broken.PlayOneShot((SoundInfo)new TargetInfo(this.Pawn.Position, this.Pawn.Map, false));
            FleckMaker.Static(this.Pawn.TrueCenter(), this.Pawn.Map, FleckDefOf.ExplosionFlash, 12f);
            for (int index = 0; index < 6; ++index)
                FleckMaker.ThrowDustPuff(this.Pawn.TrueCenter() + Vector3Utility.HorizontalVectorFromAngle((float)Rand.Range(0, 360)) * Rand.Range(0.3f, 0.6f), this.Pawn.Map, Rand.Range(0.8f, 1.2f));
            this.energy = 0.0f;
            this.ticksToReset = this.StartingTicksToReset;
        }

        private void Reset()
        {
            if (this.Pawn.Spawned)
            {
                SoundDefOf.EnergyShield_Reset.PlayOneShot((SoundInfo)new TargetInfo(this.Pawn.Position, this.Pawn.Map, false));
                FleckMaker.ThrowLightningGlow(this.Pawn.TrueCenter(), this.Pawn.Map, 3f);
            }
            this.ticksToReset = -1;
            this.energy = this.EnergyOnReset;
        }

        public void DrawWornExtras()
        {
            if (this.ShieldState != ShieldState.Active || !this.ShouldDisplay)
                return;
            float num1 = Mathf.Lerp(1.8f, 2.1f, this.energy);
            Vector3 drawPos = this.Pawn.Drawer.DrawPos;
            drawPos.y = AltitudeLayer.MoteOverhead.AltitudeFor();
            int num2 = Find.TickManager.TicksGame - this.lastAbsorbDamageTick;
            if (num2 < 8)
            {
                float num3 = (float)((double)(8 - num2) / 8.0 * 0.0500000007450581);
                drawPos += this.impactAngleVect * num3;
                num1 -= num3;
            }
            float angle = (float)Rand.Range(0, 360);
            Vector3 s = new Vector3(num1, 1f, num1);
            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetTRS(drawPos, Quaternion.AngleAxis(angle, Vector3.up), s);
            Graphics.DrawMesh(MeshPool.plane10, matrix, HediffComp_ShieldOfSaphery.BubbleMat, 0);
        }
    }
}
