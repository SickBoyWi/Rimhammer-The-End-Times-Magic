using SickAbilityUser;
using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    [StaticConstructorOnStartup]
    public class Projectile_FlameStorm : Projectile_Ability
    {
        private Pawn casterPawn;
        private bool initialized;
        private FlameTornado flameTornado;
        private bool destroyed = false;
        private bool castingComplete;

        public Projectile_FlameStorm()
            : base()
        {
        }

        protected override void Tick()
        {
            base.Tick();
            if (this.casterPawn != null && (this.casterPawn.Destroyed || this.casterPawn.Dead))
            {
                flameTornado.Destroy(DestroyMode.Vanish);
                this.Destroy(DestroyMode.Vanish);
            }
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            base.Destroy(mode);
            destroyed = true;
        }

        private void Initialize()
        {
            this.casterPawn = this.launcher as Pawn;
            this.initialized = true;
            this.castingComplete = false;
        }

        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            IntVec3 position = this.Position;

            base.Impact(hitThing);

            if (!this.initialized)
            {
                this.Initialize();
                FleckMaker.ThrowSmoke(Caster.Position.ToVector3(), this.Map, 2f);
            }

            if (this.castingComplete)
            {
                return;
            }
            
            Map map2 = Caster.Map;
            
            FleckMaker.ThrowSmoke(position.ToVector3(), map2, 2f);
            FleckMaker.ThrowHeatGlow(position, map2, 2f);
            FleckMaker.Static(position, map2, FleckDefOf.PsycastAreaEffect, 2f);
            FleckMaker.ThrowMicroSparks(position.ToVector3(), map2);

            flameTornado = (FlameTornado)GenSpawn.Spawn(RH_TET_MagicDefOf.RH_TET_FlameTornado, position, this.casterPawn.Map);
            
            this.castingComplete = true;
        }

        public override void ExposeData()
        {
            ((Projectile_AbilityBase)this).ExposeData();
            Scribe_Values.Look<bool>(ref this.initialized, "initialized", false, false);
            Scribe_Values.Look<bool>(ref this.destroyed, "destroyed", false, false);
            Scribe_Values.Look<bool>(ref this.castingComplete, "castingComplete", false, false);
            Scribe_References.Look<FlameTornado>(ref this.flameTornado, "flameTornado", false);
            Scribe_References.Look<Pawn>(ref this.casterPawn, "casterPawn", false);
        }
    }
}
