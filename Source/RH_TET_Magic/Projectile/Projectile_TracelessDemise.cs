using SickAbilityUser;
using RimWorld;
using System;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    public class Projectile_TracelessDemise : Projectile_AbilityBase
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

            FleckMaker.Static(targetLocation, map, RH_TET_MagicDefOf.RH_TET_FleckGreenEffect, .2f);
            FleckMaker.Static(targetLocation, map, RH_TET_MagicDefOf.RH_TET_FleckGreenEffectFancy, 1);
            FleckMaker.Static(targetLocation, map, FleckDefOf.PsycastAreaEffect, 1.1f);
            
            // Kill target.
            DamageInfo dinfo = new DamageInfo(RH_TET_MagicDefOf.RH_TET_MagicalInjury, 99999f, 999f, -1f, launcher, theTarget.health.hediffSet.GetBrain(), (ThingDef)null, DamageInfo.SourceCategory.Collapse, (Thing)null);
            theTarget.TakeDamage(dinfo);
            if (!theTarget.Dead)
                theTarget.Kill(new DamageInfo?(dinfo), (Hediff)null);

            // Find target corpse and destroy.
            theTarget.Corpse.Destroy(DestroyMode.KillFinalize);
        }

        public override void Tick()
        {
            Vector3 drawPos = this.DrawPos;
            drawPos.x += Rand.Range(-0.4f, 0.4f);
            drawPos.z += Rand.Range(-0.4f, 0.4f);
            base.Tick();
        }

        public Projectile_TracelessDemise()
            : base()
        {
        }
    }
}
