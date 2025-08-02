using SickAbilityUser;
using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace TheEndTimes_Magic
{
    [StaticConstructorOnStartup]
    internal class Projectile_Transport : Projectile_AbilityBase, IExposable
    {
        private int dissipationTicks;
        private int age;
        private bool setUp = true;
        private Thing transporterPlatform = null;
        private Thing transporterPod = null;

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            if (this.age < this.dissipationTicks)
                return;
            Messages.Message((string)"RH_TET_Magic_TransportCollapseFinal".Translate(), MessageTypeDefOf.SilentInput, true);
            base.Destroy(mode);
        }

        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            Map map = this.Map;
            GenClamor.DoClamor((Thing)this, 2.1f, ClamorDefOf.Impact);
            CompMagicUser comp = this.Caster.GetComp<CompMagicUser>();

            int spellLevel = -1;

            foreach (MagicPower mp in this.Caster.GetComp<CompMagicUser>().MagicData.PowersAddons)
            {
                if (mp.abilityDef.defName.Contains("RH_TET_AddOn_Transport"))
                {
                    spellLevel = mp.level;
                }
            }

            CellRect cellRect = CellRect.CenteredOn(this.Position, 1);
            cellRect.ClipInsideMap(map);
            IntVec3 centerCell = cellRect.CenterCell;

            if (this.setUp)
            {
                this.dissipationTicks = Mathf.RoundToInt((float)(10000 + (spellLevel * 5000))); // 2500 per hour
                IntVec3 c1 = centerCell;
                ++centerCell.x;
                IntVec3 c2 = centerCell;
                ++centerCell.z;
                IntVec3 c3 = centerCell;
                centerCell = cellRect.CenterCell;
                if (c1.IsValid && c1.Standable(map) && (c2.IsValid && c2.Standable(map)) && (c3.IsValid && c3.Standable(map)))
                {
                    SpawnThings spawnables = new SpawnThings();
                    IntVec3 intVec3_1 = centerCell;
                    ++centerCell.x;
                    spawnables.def = spellLevel != 2 ? (spellLevel != 3 ? (spellLevel != 4 ? ThingDef.Named("RH_TET_Magic_TransportPlatform_I") : ThingDef.Named("RH_TET_Magic_TransportPlatform_IV")) : ThingDef.Named("RH_TET_Magic_TransportPlatform_III")) : ThingDef.Named("RH_TET_Magic_TransportPlatform_II");
                    spawnables.spawnCount = 1;
                    try
                    {
                        this.SingleSpawnLoop(spawnables, intVec3_1, map);
                        Building firstBuilding = intVec3_1.GetFirstBuilding(map);
                        this.transporterPlatform = firstBuilding;
                        int countToFullyRefuel = firstBuilding.TryGetComp<CompRefuelable>().GetFuelCountToFullyRefuel();
                        firstBuilding.TryGetComp<CompRefuelable>().Refuel((float)countToFullyRefuel);
                    }
                    catch
                    {
                        Log.Message("Tried to create a transporter, but failed due to unknown error - recovering and ending attempt.");
                        this.TransportDissipate(intVec3_1, map);
                        if (this.Caster != null)
                        {
                            comp.MagicPool.CurLevel += .5F;
                        }
                        this.age = this.dissipationTicks;
                        return;
                    }
                    spawnables.def = spellLevel != 2 ? (spellLevel != 3 ? (spellLevel != 4 ? ThingDef.Named("RH_TET_Magic_TransportPod_I") : ThingDef.Named("RH_TET_Magic_TransportPod_IV")) : ThingDef.Named("RH_TET_Magic_TransportPod_III")) : ThingDef.Named("RH_TET_Magic_TransportPod_II");
                    spawnables.spawnCount = 1;
                    IntVec3 intVec3_2 = centerCell;
                    ++centerCell.z;
                    try
                    {
                        this.SingleSpawnLoop(spawnables, intVec3_2, map);
                        Building firstBuilding = intVec3_2.GetFirstBuilding(map);
                        this.transporterPod = firstBuilding;
                    }
                    catch
                    {
                        Log.Message("Tried to create a transporter pod, but failed due to unknown error - recovering and ending attempt.");
                        this.TransportDissipate(intVec3_2, map);
                        if (this.Caster != null)
                        {
                            comp.MagicPool.CurLevel += .5F;
                        }
                        this.age = this.dissipationTicks;
                        return;
                    }
                    Messages.Message((string)"RH_TET_Magic_TransporterDissipationTime".Translate((NamedArgument)((this.dissipationTicks - this.age) / 60).ToString()), MessageTypeDefOf.NeutralEvent, true);
                    this.setUp = false;
                }
                else
                {
                    Messages.Message((string)"RH_TET_Magic_InvalidTransportlLocation".Translate(), MessageTypeDefOf.RejectInput, true);
                    comp.MagicPool.GainNeed(.5F);
                    this.dissipationTicks = 0;
                }
            }
            if (this.age < this.dissipationTicks)
            {
                if ((double)this.age == (double)this.dissipationTicks * 0.5)
                    Messages.Message((string)"RH_TET_Magic_TransporterDissipationTime".Translate((NamedArgument)((this.dissipationTicks - this.age) / 60).ToString()), MessageTypeDefOf.NeutralEvent, true);
                if ((double)this.age == (double)this.dissipationTicks * 0.75)
                    Messages.Message((string)"RH_TET_Magic_TransporterDissipationTime".Translate((NamedArgument)((this.dissipationTicks - this.age) / 60).ToString()), MessageTypeDefOf.NeutralEvent, true);
                if ((double)this.age == (double)this.dissipationTicks * 0.95)
                    Messages.Message((string)"RH_TET_Magic_TransporterDissipationTime".Translate((NamedArgument)((this.dissipationTicks - this.age) / 60).ToString()), MessageTypeDefOf.NeutralEvent, true);
            }
            else
                this.TransportDissipate(centerCell, map);

            ((Thing)this).Destroy(DestroyMode.Vanish);
        }

        protected void TransportDissipate(IntVec3 pos, Map map)
        {
            FleckMaker.ThrowMicroSparks(pos.ToVector3(), map);
            FleckMaker.Static(pos, map, FleckDefOf.PsycastAreaEffect, 3f);
            FleckMaker.ThrowSmoke(pos.ToVector3(), map, 2f);

            ThingDef motePuff = DefDatabase<ThingDef>.GetNamedSilentFail("Mote_ExtinguisherPuff");
            Projectile_Transport.Explosion(pos, map, 1, RimWorld.DamageDefOf.Bomb, this.Caster, (SoundDef)null, def, this.equipmentDef, motePuff, 0.4f, 1, false, (ThingDef)null, 0.0f, 1);

            if (this.transporterPlatform != null && !this.transporterPlatform.Destroyed)
            {
                this.transporterPlatform.Destroy();

                DamageInfo damInfo = new DamageInfo(RH_TET_MagicDefOf.RH_TET_MagicalBlunt, 1000, 1.0f, -1, this.Caster);
                this.transporterPlatform.TakeDamage(damInfo);
            }
            if (this.transporterPod != null && !this.transporterPod.Destroyed)
                this.transporterPod.Destroy();
        }

        public static void Explosion(
          IntVec3 center,
          Map map,
          float radius,
          DamageDef damType,
          Thing instigator,
          SoundDef explosionSound = null,
          ThingDef projectile = null,
          ThingDef source = null,
          ThingDef postExplosionSpawnThingDef = null,
          float postExplosionSpawnChance = 0.0f,
          int postExplosionSpawnThingCount = 1,
          bool applyDamageToExplosionCellsNeighbors = false,
          ThingDef preExplosionSpawnThingDef = null,
          float preExplosionSpawnChance = 0.0f,
          int preExplosionSpawnThingCount = 1)
        {
            if (map == null)
            {
                Log.Warning("You can't do an explosion on a null map.");
            }
            else
            {
                Verse.Explosion explosion = (Verse.Explosion)GenSpawn.Spawn(ThingDefOf.Explosion, center, map, WipeMode.Vanish);
                explosion.damageFalloff = true;
                explosion.chanceToStartFire = 0.0f;
                explosion.Position = center;
                explosion.radius = radius;
                explosion.damType = damType;
                explosion.instigator = instigator;
                explosion.damAmount = 30;
                explosion.weapon = source;
                explosion.chanceToStartFire = 0.0f;
                explosion.preExplosionSpawnThingDef = preExplosionSpawnThingDef;
                explosion.preExplosionSpawnChance = preExplosionSpawnChance;
                explosion.preExplosionSpawnThingCount = preExplosionSpawnThingCount;
                explosion.postExplosionSpawnThingDef = postExplosionSpawnThingDef;
                explosion.postExplosionSpawnChance = postExplosionSpawnChance;
                explosion.postExplosionSpawnThingCount = postExplosionSpawnThingCount;
                explosion.applyDamageToExplosionCellsNeighbors = applyDamageToExplosionCellsNeighbors;
                explosion.StartExplosion(explosionSound, (List<Thing>)null);
            }
        }

        public void SingleSpawnLoop(SpawnThings spawnables, IntVec3 position, Map map)
        {
            if (spawnables.def == null)
                return;
            Faction faction = this.Caster.Faction;
            ThingDef def = (ThingDef)spawnables.def;
            ThingDef stuff = (ThingDef)null;
            if (def.MadeFromStuff)
                stuff = ThingDefOf.WoodLog;
            Thing newThing = ThingMaker.MakeThing(def, stuff);
            if (newThing.def.defName != "RH_TET_Skaven_Warpstone")
                newThing.SetFaction(faction, (Pawn)null);
            GenSpawn.Spawn(newThing, position, map, Rot4.North, WipeMode.Vanish, false);
        }

        protected override void Tick()
        {
            base.Tick();
            ++this.age;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref this.setUp, "setUp", false, false);
            Scribe_Values.Look<int>(ref this.age, "age", -1, false);
            Scribe_Values.Look<int>(ref this.dissipationTicks, "dissipationTicks", 3600, false);
        }

        public Projectile_Transport()
            : base()
        {
        }

        public Pawn SpawnPawn(
          Pawn caster,
          SpawnThings spawnables,
          Faction faction,
          IntVec3 position,
          int duration,
          Map map)
        {
            Pawn newPawn = PawnGenerator.GeneratePawn((PawnKindDef)spawnables.kindDef, faction);
            GenSpawn.Spawn((Thing)newPawn, position, map, WipeMode.Vanish);
            if (newPawn.Faction != null && newPawn.Faction != Faction.OfPlayer)
            {
                Lord lord = (Lord)null;
                if (newPawn.Map.mapPawns.SpawnedPawnsInFaction(faction).Any<Pawn>((Predicate<Pawn>)(p => p != newPawn)))
                {
                    Predicate<Thing> validator = (Predicate<Thing>)(p => p != newPawn && ((Pawn)p).GetLord() != null);
                    lord = ((Pawn)GenClosest.ClosestThing_Global(newPawn.Position, (IEnumerable)newPawn.Map.mapPawns.SpawnedPawnsInFaction(faction), 99999f, validator, (Func<Thing, float>)null)).GetLord();
                }
                if (lord == null)
                {
                    LordJob_DefendPoint lordJobDefendPoint = new LordJob_DefendPoint(newPawn.Position, new float?(), new float?(), false, true);
                    lord = LordMaker.MakeNewLord(faction, (LordJob)lordJobDefendPoint, newPawn.Map, (IEnumerable<Pawn>)null);
                }
                else
                {
                    try
                    {
                        newPawn.mindState.duty = new PawnDuty(DutyDefOf.Defend);
                    }
                    catch
                    {
                        Log.Message("Couldn't assign a dute to a summoned object.");
                    }
                }
                lord.AddPawn((Pawn)newPawn);
            }
            return newPawn;
        }
    }
}
