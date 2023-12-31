using AbilityUser;
using RimWorld;
using System;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    public class Projectile_IlluminateTheEdifice : Projectile_AbilityBase
    {
        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            Map map = this.Map;
            base.Impact(hitThing);
            ThingDef def = this.def;
            IntVec3 pos = this.Position;
            Pawn launcher = this.launcher as Pawn;
            IntVec3 casterLocation = launcher.Position;

            FleckMaker.Static(casterLocation, map, RH_TET_MagicDefOf.RH_TET_FleckWhiteLightEffect, 1);

            Thing light = ThingMaker.MakeThing(RH_TET_MagicDefOf.RH_TET_IlluminateLight);
            GenSpawn.Spawn(light, pos, map);

            FleckMaker.Static(casterLocation, map, RH_TET_MagicDefOf.RH_TET_FleckWhiteEffect, .2f);
            FleckMaker.Static(casterLocation, map, FleckDefOf.PsycastAreaEffect, .5f);
        }

        public Projectile_IlluminateTheEdifice()
            : base()
        {
        }
    }
}
