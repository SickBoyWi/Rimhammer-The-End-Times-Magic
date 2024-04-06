using SickAbilityUser;
using RimWorld;
using System;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    public class Projectile_ChaosVeilOfCorruption : Projectile_AbilityBase
    {
        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            Map map = this.Map;
            base.Impact(hitThing);
            ThingDef def = ((Thing)this).def;
            GenExplosion.DoExplosion(this.Position, 
                map, 
                ((Thing)this).def.projectile.explosionRadius, 
                RimWorld.DamageDefOf.Smoke, 
                this.launcher, 
                Mathf.RoundToInt((float)Rand.Range(((Thing)this).def.projectile.GetDamageAmount(1f, (StringBuilder)null) / 2, ((Thing)this).def.projectile.GetDamageAmount(1f, (StringBuilder)null)) * 1), 
                0.0f, 
                SoundDef.Named("InfernoCannon_Fire"), 
                def, 
                this.equipmentDef, 
                (Thing)null, 
                null, 
                1.0f, 
                1,
                new GasType?(GasType.BlindSmoke),
                false, 
                (ThingDef)null, 
                0.0f, 
                1,
                0.0f, 
                true);
            CellRect cellRect = CellRect.CenteredOn(this.Position, 10);
            cellRect.ClipInsideMap(map);
            Pawn launcher = this.launcher as Pawn;
            CompMagicUser comp = launcher.GetComp<CompMagicUser>();
        }

        public void Explosion(
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
          GasType? postExplosionGasType = null,
          bool applyDamageToExplosionCellsNeighbors = false,
          ThingDef preExplosionSpawnThingDef = null,
          float preExplosionSpawnChance = 0.0f,
          int preExplosionSpawnThingCount = 1)
        {
            if (map == null)
            {
                Log.Warning("Tried to do explosion on null map.", false);
            }
            else
            {
                Explosion explosion = (Explosion)GenSpawn.Spawn(ThingDefOf.Explosion, center, map, WipeMode.Vanish);
                explosion.Position = center;
                explosion.radius = radius;
                explosion.damType = damType;
                explosion.instigator = instigator;
                explosion.damAmount = projectile != null ? projectile.projectile.GetDamageAmount(1f) : 2;
                explosion.weapon = source;
                explosion.preExplosionSpawnThingDef = preExplosionSpawnThingDef;
                explosion.preExplosionSpawnChance = preExplosionSpawnChance;
                explosion.preExplosionSpawnThingCount = preExplosionSpawnThingCount;
                explosion.postExplosionSpawnThingDef = postExplosionSpawnThingDef;
                explosion.postExplosionSpawnChance = postExplosionSpawnChance;
                explosion.postExplosionSpawnThingCount = postExplosionSpawnThingCount;
                explosion.postExplosionGasType = postExplosionGasType;
                explosion.applyDamageToExplosionCellsNeighbors = applyDamageToExplosionCellsNeighbors;
                explosion.damageFalloff = true;
                explosion.chanceToStartFire = 0.05f;
                explosion.StartExplosion(explosionSound, null);
            }
        }

        public override void Tick()
        {
            Vector3 drawPos = this.DrawPos;
            drawPos.x += Rand.Range(-0.4f, 0.4f);
            drawPos.z += Rand.Range(-0.4f, 0.4f);
            base.Tick();
        }

        public Projectile_ChaosVeilOfCorruption()
            : base()
        {
        }
    }
}
