using SickAbilityUser;
using RimWorld;
using System;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    public class Projectile_DirectDamagePawn : Projectile_AbilityBase
    {
        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            Map map = Caster.Map;
            base.Impact(hitThing);
            ThingDef def = this.def;
            Pawn launcher = this.launcher as Pawn;
            Pawn theTarget = hitThing as Pawn;

            DamageDef damage = this.def.projectile.damageDef;
            DamageInfo damageInfo = new DamageInfo(damage, this.def.projectile.GetDamageAmount(null), 0);
            FleckMaker.ThrowMicroSparks(destination, map);
            FleckMaker.Static(destination, map, FleckDefOf.ShotHit_Dirt, 1f);
            theTarget.TakeDamage(damageInfo);

            //this.ApplyHediffsAndMentalStates(theTarget);
        }

        public Projectile_DirectDamagePawn()
            : base()
        {
        }
    }
}
