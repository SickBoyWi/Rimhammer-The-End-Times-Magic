using SickAbilityUser;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    public class Verb_BurningBlood : Verb_UseAbility
    {
        protected override bool TryCastShot()
        {
            // Miscast?
            if (MagicUtility.TryMiscast(this.caster))
                return true;
            
            bool flag1 = false;
            Pawn theCaster = caster as Pawn;
            
            MagicPower magicPower = null;
            int spellLevel = -1;
            foreach (MagicPower mp in this.CasterPawn.GetComp<CompMagicUser>().MagicData.PowersChaos)
            {
                if (mp.abilityDef.defName.Contains("BurningBlood"))
                {
                    magicPower = mp;
                    spellLevel = mp.level;
                }
            }
            
            if (magicPower == null || spellLevel < 0)
            {
                Log.Error("Someone tried to cast Burning Blood, but didn't have the spell.");
                return false;
            }

            Pawn target = this.currentTarget.Thing as Pawn;
            Map map = theCaster.Map;

            FleckMaker.Static(caster.Position, map, RH_TET_MagicDefOf.RH_TET_Fleck_ChaosStar, 2f);
            FleckMaker.Static(caster.Position, map, RH_TET_MagicDefOf.RH_TET_FleckRedEffect, 2f);
            FleckMaker.Static(caster.Position, map, FleckDefOf.PsycastAreaEffect, 1.5f);

            if (target != null)
            {
                flag1 = true;
                FleckMaker.Static(target.Position, map, RH_TET_MagicDefOf.RH_TET_Fleck_ChaosStar, 1f);
                FleckMaker.Static(target.Position, map, RH_TET_MagicDefOf.RH_TET_FleckRedEffect, 1.5f);
                FleckMaker.Static(target.Position, map, FleckDefOf.PsycastAreaEffect, 1f);

                DamageDef damage = this.verbProps.defaultProjectile.projectile.damageDef;
                float damageAmt = this.verbProps.defaultProjectile.projectile.GetDamageAmount(1f);

                switch (spellLevel)
                {
                    case 1:
                        damageAmt -= 30f;
                        break;
                    case 2:
                        damageAmt -= 20f;
                        break;
                    case 3:
                        damageAmt -= 10f;
                        break;
                    default:
                        break;
                }

                float actualDamPerIteration = 5f;

                while (true)
                { 
                    DamageInfo damageInfo = new DamageInfo(damage, actualDamPerIteration, 1.0f);
                    target.TakeDamage(damageInfo);

                    damageAmt -= actualDamPerIteration;
                    if (damageAmt <= 0)
                    {
                        FleckMaker.Static(target.Position, map, RH_TET_MagicDefOf.RH_TET_Fleck_ChaosStar, 1f);
                        FleckMaker.Static(target.Position, map, RH_TET_MagicDefOf.RH_TET_FleckRedEffect, 1.5f);
                        FleckMaker.Static(target.Position, map, FleckDefOf.PsycastAreaEffect, 1f);

                        break;
                    }
                }
            }
            
            this.PostCastShot(flag1, out flag1);
            return flag1;
        }

        public Verb_BurningBlood()
            : base()
        {
        }
    }
}
