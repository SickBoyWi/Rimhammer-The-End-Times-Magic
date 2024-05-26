using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    [StaticConstructorOnStartup]
    public class CompShieldKhorne : ThingComp
    {
        private static readonly Material BubbleMat = MaterialPool.MatFrom("Magic/RH_TET_KhorneShield", ShaderDatabase.Transparent);
        protected int ticksToReset = -1;
        protected int lastKeepDisplayTick = -9999;
        private int lastAbsorbDamageTick = -9999;
        private int KeepDisplayingTicks = 1000;
        private float ApparelScorePerEnergyMax = 0.25f;
        protected float energy = 0f;
        private Vector3 impactAngleVect;
        private const float MaxDamagedJitterDist = 0.05f;
        private const int JitterDurationTicks = 8;

        public CompProperties_ShieldKhorne Props
        {
            get
            {
                return (CompProperties_ShieldKhorne)this.props;
            }
        }

        private float EnergyMax
        {
            get
            {
                return this.parent.GetStatValue(StatDefOf.EnergyShieldEnergyMax, true, -1);
            }
        }

        private float EnergyGainPerTick
        {
            get
            {
                return this.parent.GetStatValue(StatDefOf.EnergyShieldRechargeRate, true, -1) / 60f;
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
                if (this.parent is Pawn parent && (parent.IsCharging() || parent.IsSelfShutdown()))
                    return ShieldState.Disabled;
                CompCanBeDormant comp = this.parent.GetComp<CompCanBeDormant>();
                if (comp != null && !comp.Awake)
                    return ShieldState.Disabled;
                return this.ticksToReset <= 0 ? ShieldState.Active : ShieldState.Resetting;
            }
        }

        protected bool ShouldDisplay
        {
            get
            {
                Pawn pawnOwner = this.PawnOwner;
                return pawnOwner.Spawned && !pawnOwner.Dead && !pawnOwner.Downed && (pawnOwner.InAggroMentalState || pawnOwner.Drafted || pawnOwner.Faction.HostileTo(Faction.OfPlayer) && !pawnOwner.IsPrisoner || Find.TickManager.TicksGame < this.lastKeepDisplayTick + this.KeepDisplayingTicks || ModsConfig.BiotechActive && pawnOwner.IsColonyMech && Find.Selector.SingleSelectedThing == pawnOwner);
            }
        }

        protected Pawn PawnOwner
        {
            get
            {
                if (this.parent is Apparel parent)
                    return parent.Wearer;
                return this.parent is Pawn parent2 ? parent2 : (Pawn)null;
            }
        }

        public bool IsApparel
        {
            get
            {
                return this.parent is Apparel;
            }
        }

        private bool IsBuiltIn
        {
            get
            {
                return !this.IsApparel;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<float>(ref this.energy, "energy2", 0.0f, false);
        }

        public override IEnumerable<Gizmo> CompGetWornGizmosExtra()
        {
            CompShieldKhorne compShield = this;
            foreach (Gizmo gizmo in base.CompGetWornGizmosExtra())
                yield return gizmo;
            if (compShield.IsApparel)
            {
                foreach (Gizmo gizmo in compShield.GetGizmos())
                    yield return gizmo;
            }
            if (DebugSettings.ShowDevGizmos)
            {
                Command_Action commandAction1 = new Command_Action();
                commandAction1.defaultLabel = "DEV: Break";
                commandAction1.action = new Action(compShield.Break);
                yield return (Gizmo)commandAction1;
                if (compShield.ticksToReset > 0)
                {
                    Command_Action commandAction2 = new Command_Action();
                    commandAction2.defaultLabel = "DEV: Clear reset";
                    // ISSUE: reference to a compiler-generated method
                    commandAction2.action = new Action(compShield.Reset);
                    yield return (Gizmo)commandAction2;
                }
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
                yield return gizmo;
            if (this.IsBuiltIn)
            {
                foreach (Gizmo gizmo in this.GetGizmos())
                    yield return gizmo;
            }
        }

        private IEnumerable<Gizmo> GetGizmos()
        {
            CompShieldKhorne compShield = this;
            if ((compShield.PawnOwner.Faction == Faction.OfPlayer || compShield.parent is Pawn parent && parent.RaceProps.IsMechanoid) && Find.Selector.SingleSelectedThing == compShield.PawnOwner)
                yield return (Gizmo)new Gizmo_MagicEnergyShieldStatus()
                {
                    shield = compShield
                };
        }

        public override float CompGetSpecialApparelScoreOffset()
        {
            return this.EnergyMax * this.ApparelScorePerEnergyMax;
        }

        public override void CompTick()
        {
            base.CompTick();
            if (this.PawnOwner == null)
                this.energy = 0.0f;
            else if (this.ShieldState == ShieldState.Resetting)
            {
                --this.ticksToReset;
                if (this.ticksToReset > 0)
                    return;
                this.Reset();
            }
            else
            {
                if (this.ShieldState != ShieldState.Active)
                    return;
                this.energy += this.EnergyGainPerTick;
                if ((double)this.energy <= (double)this.EnergyMax)
                    return;
                this.energy = this.EnergyMax;
            }
        }

        public override void PostPreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
        {
            absorbed = false;
            if (this.ShieldState != ShieldState.Active)
                return;
            if (dinfo.Def == DamageDefOf.EMP)
            {
                this.energy = 0.0f;
                this.Break();
            }
            else
            {
                if (!dinfo.Def.isRanged && !dinfo.Def.isExplosive)
                    return;
                this.energy -= dinfo.Amount * this.Props.energyLossPerDamage;
                if ((double)this.energy < 0.0)
                    this.Break();
                else
                    this.AbsorbedDamage(dinfo);
                absorbed = true;
            }
        }

        public new void KeepDisplaying()
        {
            this.lastKeepDisplayTick = Find.TickManager.TicksGame;
        }

        private void AbsorbedDamage(DamageInfo dinfo)
        {
            SoundDefOf.EnergyShield_AbsorbDamage.PlayOneShot((SoundInfo)new TargetInfo(this.PawnOwner.Position, this.PawnOwner.Map, false));
            this.impactAngleVect = Vector3Utility.HorizontalVectorFromAngle(dinfo.Angle);
            Vector3 loc = this.PawnOwner.TrueCenter() + this.impactAngleVect.RotatedBy(180f) * 0.5f;
            float scale = Mathf.Min(10f, (float)(2.0 + (double)dinfo.Amount / 10.0));
            FleckMaker.Static(loc, this.PawnOwner.Map, FleckDefOf.ExplosionFlash, scale);
            int num = (int)scale;
            for (int index = 0; index < num; ++index)
                FleckMaker.ThrowDustPuff(loc, this.PawnOwner.Map, Rand.Range(0.8f, 1.2f));
            this.lastAbsorbDamageTick = Find.TickManager.TicksGame;
            this.KeepDisplaying();
        }

        private void Break()
        {
            float scale = Mathf.Lerp(this.Props.minDrawSize, this.Props.maxDrawSize, this.energy);
            EffecterDefOf.Shield_Break.SpawnAttached((Thing)this.parent, this.parent.MapHeld, scale);
            FleckMaker.Static(this.PawnOwner.TrueCenter(), this.PawnOwner.Map, FleckDefOf.ExplosionFlash, 12f);
            for (int index = 0; index < 6; ++index)
                FleckMaker.ThrowDustPuff(this.PawnOwner.TrueCenter() + Vector3Utility.HorizontalVectorFromAngle((float)Rand.Range(0, 360)) * Rand.Range(0.3f, 0.6f), this.PawnOwner.Map, Rand.Range(0.8f, 1.2f));
            this.energy = 0.0f;
            this.ticksToReset = this.Props.startingTicksToReset;
        }

        private void Reset()
        {
            if (this.PawnOwner.Spawned)
            {
                SoundDefOf.EnergyShield_Reset.PlayOneShot((SoundInfo)new TargetInfo(this.PawnOwner.Position, this.PawnOwner.Map, false));
                FleckMaker.ThrowLightningGlow(this.PawnOwner.TrueCenter(), this.PawnOwner.Map, 3f);
            }
            this.ticksToReset = -1;
            this.energy = this.Props.energyOnReset;
        }

        public override void CompDrawWornExtras()
        {
            if (!this.IsApparel)
                return;
            this.Draw();
        }

        public override void PostDraw()
        {
            base.PostDraw();
            if (!this.IsBuiltIn)
                return;
            this.Draw();
        }

        private void Draw()
        {
            if (this.ShieldState != ShieldState.Active || !this.ShouldDisplay)
                return;
            float num1 = Mathf.Lerp(this.Props.minDrawSize, this.Props.maxDrawSize, this.energy);
            Vector3 drawPos = this.PawnOwner.Drawer.DrawPos;
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
            Graphics.DrawMesh(MeshPool.plane10, matrix, CompShieldKhorne.BubbleMat, 0);
        }

        public override bool CompAllowVerbCast(Verb verb)
        {
            return !this.Props.blocksRangedWeapons || !(verb is Verb_LaunchProjectile);
        }
    }
}
