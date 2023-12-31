using AbilityUser;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    public class Verb_AmberTrance : Verb_UseAbility
    {
        protected override bool TryCastShot()
        {
            // Miscast?
            if (MagicUtility.TryMiscast(this.caster))
                return true;
            
            bool flag1 = false;
            Pawn theCaster = caster as Pawn;
            
            MagicPower magicPower = null;
            foreach (MagicPower mp in this.CasterPawn.GetComp<CompMagicUser>().MagicData.PowersBeast)
            {
                if (mp.abilityDef.defName.Contains("AmberTrance"))
                {
                    magicPower = mp;
                }
            }
            
            if (magicPower == null)
            {
                Log.Error("Someone tried to cast Amber Trance, but didn't have the spell.");
                return false;
            }
            
            Pawn target = this.currentTarget.Thing as Pawn;
            Map map = theCaster.Map;

            FleckMaker.Static(caster.Position, map, RH_TET_MagicDefOf.RH_TET_FleckBrownEffect, 2f);
            FleckMaker.Static(caster.Position, map, FleckDefOf.PsycastAreaEffect, 1.5f);

            if (target != null)
            {
                flag1 = true;
                FleckMaker.Static(target.Position, map, RH_TET_MagicDefOf.RH_TET_FleckBrownEffect, 1.5f);
                FleckMaker.Static(target.Position, map, FleckDefOf.PsycastAreaEffect, 1f);
                target.health.AddHediff(RH_TET_MagicDefOf.RH_TET_AmberTrance);
            }
            
            this.PostCastShot(flag1, out flag1);
            return flag1;
        }

        public Verb_AmberTrance()
            : base()
        {
        }
    }
}
