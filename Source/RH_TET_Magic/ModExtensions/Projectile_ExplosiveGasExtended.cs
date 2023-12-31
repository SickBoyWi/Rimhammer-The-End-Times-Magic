using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace TheEndTimes_Magic
{
    public class Projectile_ExplosiveGasExtended : Projectile_Explosive
    {
        private ProjectileCloundExtension extensionProps => (def.GetModExtension<ProjectileCloundExtension>() ?? null);

        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            var usedTargetInfo = hitThing ?? new TargetInfo(usedTarget.Cell, Map);
            base.Impact(hitThing);

            Map mapLocal = this.Map;
            if (mapLocal is null)
                mapLocal = this.launcher.Map;

            if (Rand.Chance(extensionProps.spawnChance))
            { 
                Thing thing = ThingMaker.MakeThing(extensionProps.spawnThingDef, (ThingDef)null);
                GenPlace.TryPlaceThing(thing, this.Position, mapLocal, ThingPlaceMode.Direct, (Action<Verse.Thing, int>)null, (Predicate<IntVec3>)null, new Rot4());
                (thing as GasCloud).ReceiveConcentration(extensionProps.spawnThingCount);
            }
        }
    }
}