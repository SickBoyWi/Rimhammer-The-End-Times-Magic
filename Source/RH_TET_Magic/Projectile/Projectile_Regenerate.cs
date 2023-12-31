using AbilityUser;
using RimWorld;
using System;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    public class Projectile_Regenerate : Projectile_AbilityBase
    {
        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            base.Impact(hitThing);
            ThingDef def = this.def;
            Pawn launcher = this.launcher as Pawn;
            Pawn theTarget = hitThing as Pawn;
            
            IntVec3 casterLocation = launcher.Position;
            IntVec3 targetLocation = theTarget.Position;
            Map map = launcher.Map;

            FleckMaker.Static(casterLocation, map, RH_TET_MagicDefOf.RH_TET_FleckGreenEffectFancy, 1);
            FleckMaker.Static(casterLocation, map, RH_TET_MagicDefOf.RH_TET_FleckGreenEffect, .2f);
            FleckMaker.Static(casterLocation, map, FleckDefOf.PsycastAreaEffect, 1.1f);

            CompUseEffect_FixWorstHealthConditionRH fixPawn = new CompUseEffect_FixWorstHealthConditionRH();
            fixPawn.DoEffect(theTarget);

            FleckMaker.Static(targetLocation, map, RH_TET_MagicDefOf.RH_TET_FleckGreenEffect, .2f);
            FleckMaker.Static(targetLocation, map, RH_TET_MagicDefOf.RH_TET_FleckGreenEffectFancy, 1);
            FleckMaker.Static(targetLocation, map, FleckDefOf.PsycastAreaEffect, 1.1f);
        }

        public Projectile_Regenerate()
            : base()
        {
        }
    }
}
