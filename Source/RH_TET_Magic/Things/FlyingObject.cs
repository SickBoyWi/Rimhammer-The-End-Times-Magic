using RimWorld;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    public class FlyingObject : ThingWithComps
    {
        protected float speed = 30f;
        protected Vector3 origin;
        protected Vector3 destination;
        protected int ticksToImpact;
        protected Thing launcher;
        protected Thing usedTarget;
        protected Thing flyingThing;
        public DamageInfo? impactDamage;
        public Map map;

        protected int StartingTicksToImpact
        {
            get
            {
                Vector3 vector3 = this.origin - this.destination;
                int num = Mathf.RoundToInt(vector3.magnitude / (this.speed / 100f));
                if (num < 1)
                    num = 1;
                return num;
            }
        }

        protected IntVec3 DestinationCell
        {
            get
            {
                return new IntVec3(this.destination);
            }
        }

        public virtual Vector3 ExactPosition
        {
            get
            {
                return ((this.origin + ((this.destination - this.origin) * (float)(1.0 - (double)this.ticksToImpact / (double)this.StartingTicksToImpact))) + (Vector3.up * this.def.Altitude));
            }
        }

        public virtual Quaternion ExactRotation
        {
            get
            {
                return Quaternion.LookRotation((this.destination - this.origin));
            }
        }

        public override Vector3 DrawPos
        {
            get
            {
                return this.ExactPosition;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<Vector3>(ref this.origin, "origin", new Vector3(), false);
            Scribe_Values.Look<Vector3>(ref this.destination, "destination", new Vector3(), false);
            Scribe_Values.Look<int>(ref this.ticksToImpact, "ticksToImpact", 0, false);
            Scribe_References.Look<Thing>(ref this.usedTarget, "usedTarget", false);
            Scribe_References.Look<Thing>(ref this.launcher, "launcher", false);
            Scribe_References.Look<Thing>(ref this.flyingThing, "flyingThing", false);
        }

        public void Launch(
          Thing launcher,
          LocalTargetInfo targ,
          Thing flyingThing,
          DamageInfo? impactDamage)
        {
            this.Launch(launcher, this.Position.ToVector3Shifted(), targ, flyingThing, impactDamage);
        }

        public void Launch(Thing launcher, LocalTargetInfo targ, Thing flyingThing)
        {
            this.Launch(launcher, this.Position.ToVector3Shifted(), targ, flyingThing, new DamageInfo?());
        }

        public void Launch(
          Thing launcher,
          Vector3 origin,
          LocalTargetInfo targ,
          Thing flyingThing,
          DamageInfo? newDamageInfo = null)
        {
            this.flyingThing = flyingThing;
            this.map = flyingThing.Map;
            FleckMaker.Static(flyingThing.Position, flyingThing.Map, RH_TET_MagicDefOf.RH_TET_FleckGrayEffect, 2f);
            if (flyingThing.Spawned)
                flyingThing.DeSpawn(DestroyMode.Vanish);
            this.launcher = launcher;
            this.origin = origin;
            if (newDamageInfo != null)
                this.impactDamage = newDamageInfo;
            else
            {
                DamageInfo dinfo = new DamageInfo(DamageDefOf.Stab, 25, .25f, -1f, (Thing)this, (BodyPartRecord)null, (ThingDef)null, DamageInfo.SourceCategory.ThingOrUnknown, (Thing)null, true, true);
                this.impactDamage = dinfo;
            }
            if (targ.Thing != null)
                this.usedTarget = targ.Thing;
            this.destination = (targ.Cell.ToVector3Shifted() + new Vector3(Rand.Range(-0.3f, 0.3f), 0.0f, Rand.Range(-0.3f, 0.3f)));
            this.ticksToImpact = this.StartingTicksToImpact;
        }

        public override void Tick()
        {
            base.Tick();
            Vector3 exactPosition = this.ExactPosition;
            --this.ticksToImpact;
            if (!this.ExactPosition.InBounds(this.Map))
            {
                ++this.ticksToImpact;
                this.Position = this.ExactPosition.ToIntVec3();
                this.Destroy(DestroyMode.Vanish);
            }
            else
            {
                this.Position = this.ExactPosition.ToIntVec3();
                if (this.ticksToImpact > 0)
                    return;
                if (this.DestinationCell.InBounds(this.Map))
                    this.Position = this.DestinationCell;
                this.ImpactSomething();
            }
        }

        public override void Draw()
        {
            if (this.flyingThing == null)
                return;
            if (this.flyingThing is Pawn)
            {
                Vector3 drawPos = this.DrawPos;
                if (!this.DrawPos.ToIntVec3().IsValid)
                    return;
                (this.flyingThing as Pawn).Drawer.DrawAt(this.DrawPos);
            }
            else
                Graphics.DrawMesh(MeshPool.plane10, this.DrawPos, this.ExactRotation, this.flyingThing.def.DrawMatSingle, 0);
            this.Comps_PostDraw();
        }

        private void ImpactSomething()
        {
            if (this.usedTarget != null)
            {
                Pawn usedTarget = this.usedTarget as Pawn;
                if (usedTarget != null && usedTarget.GetPosture() != PawnPosture.Standing && ((double)(this.origin - this.destination).MagnitudeHorizontalSquared() >= 20.25 && (double)Rand.Value > 0.200000002980232))
                    this.Impact((Thing)null);
                else
                    this.Impact(this.usedTarget);
            }
            else
                this.Impact((Thing)null);
        }

        protected virtual void Impact(Thing hitThing)
        {
            GenSpawn.Spawn(this.flyingThing, this.Position, map, WipeMode.Vanish);
            if (this.impactDamage.HasValue)
            {
                for (int index = 0; index < 3; ++index)
                    this.flyingThing.TakeDamage(this.impactDamage.Value);
            }
            this.Destroy(DestroyMode.Vanish);
        }
    }
}
