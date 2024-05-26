using SickAbilityUser;
using RimWorld;
using System;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    public class Projectile_DirectDamagePawnLaser : Projectile_AbilityLaser
    {
        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            Pawn pawnCaster = this.launcher as Pawn;

            this.DrawAt(pawnCaster.Position.ToVector3(), false);

            Pawn target = this.intendedTarget.Pawn;
            IntVec3 pos = target.Position;

            int damageAmt = this.def.projectile.GetDamageAmount(1f);
            DamageDef damage = this.def.projectile.damageDef;
            DamageInfo damageInfo = new DamageInfo(damage, damageAmt, 0, -1, pawnCaster);
            FleckMaker.ThrowMicroSparks(destination, Map);
            FleckMaker.Static(destination, Map, FleckDefOf.ShotHit_Dirt, 1f);
            target.TakeDamage(damageInfo);
        }
        
        public Projectile_DirectDamagePawnLaser()
            : base()
        {
        }
    }
}
