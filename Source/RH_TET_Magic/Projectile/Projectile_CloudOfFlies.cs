using AbilityUser;
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
    public class Projectile_CloudOfFlies : Projectile_AbilityBase
    {
        private Pawn casterPawn;
        private bool initialized;
        private float initialConcentration = 80000;

        public Projectile_CloudOfFlies()
            : base()
        {
        }

        private void Initialize()
        {
            this.casterPawn = this.launcher as Pawn;
        }

        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            if (!this.initialized)
            {
                this.Initialize();
            }

            // Determine spell level.
            int spellLevel = -1;

            foreach (MagicPower mp in casterPawn.GetComp<CompMagicUser>().MagicData.PowersPlague)
            {
                if (mp.abilityDef.defName.Contains("CloudOfFlies"))
                {
                    spellLevel = mp.level;
                }
            }

            if (spellLevel <= 0)
            {
                Log.Warning("No spell level for cloud of flies. Failing gracefully.");
                return;
            }
            else if (spellLevel == 2)
            {
                initialConcentration = 85000f;
            }
            else if (spellLevel == 3)
            {
                initialConcentration = 90000f;
            }
            else if (spellLevel == 4)
            {
                initialConcentration = 95000f;
            }

            base.Impact(hitThing);

            IntVec3 position = this.casterPawn.Position;

            Map map2 = this.casterPawn.Map;
            FleckMaker.Static(position, map2, RH_TET_MagicDefOf.RH_TET_FleckBrownEffect, 1f);
            FleckMaker.Static(position, map2, FleckDefOf.PsycastAreaEffect, 1f);

            if (this.usedTarget.CenterVector3 != null)
            {
                position = this.usedTarget.CenterVector3.ToIntVec3();
            }

            Thing thing = ThingMaker.MakeThing(RH_TET_MagicDefOf.RH_TET_InsectCloud, (ThingDef)null);
            GenPlace.TryPlaceThing(thing, position, map2, ThingPlaceMode.Direct, (Action<Verse.Thing, int>)null, (Predicate<IntVec3>)null, new Rot4());
            (thing as GasCloud).ReceiveConcentration(initialConcentration);

            FleckMaker.ThrowSmoke(position.ToVector3(), map2, 2f);

            SoundInfo info = SoundInfo.InMap(new TargetInfo(position, map2, false), MaintenanceType.None);
            info.pitchFactor = 1.3f;
            info.volumeFactor = 1.6f;
            SoundDefOf.Hive_Spawn.PlayOneShot(info);
        }

        public override void Tick()
        {
            Vector3 drawPos = this.DrawPos;
            drawPos.x += Rand.Range(-0.4f, 0.4f);
            drawPos.z += Rand.Range(-0.4f, 0.4f);
            base.Tick();
        }

        public override void ExposeData()
        {
            ((Projectile_AbilityBase)this).ExposeData();
            Scribe_References.Look<Pawn>(ref this.casterPawn, "casterPawn", false);
            Scribe_Values.Look<bool>(ref this.initialized, "initialized", false, false);
            Scribe_Values.Look<float>(ref this.initialConcentration, "initialConcentration", 0, false);
        }
    }
}
