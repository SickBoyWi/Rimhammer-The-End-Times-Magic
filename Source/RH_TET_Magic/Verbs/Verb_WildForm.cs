using AbilityUser;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    public class Verb_WildForm : Verb_UseAbility
    {
        private bool validTarg;

        protected override bool TryCastShot()
        {
            // Miscast?
            if (MagicUtility.TryMiscast(this.caster))
                return true;
            
            bool flag1 = false;
            Pawn theCaster = caster as Pawn;
            
            MagicPower magicPower = null;
            int spellLevel = -1;
            foreach (MagicPower mp in this.CasterPawn.GetComp<CompMagicUser>().MagicData.PowersBeast)
            {
                if (mp.abilityDef.defName.Contains("Wildform"))
                {
                    magicPower = mp;
                    spellLevel = mp.level;
                }
            }
            
            if (magicPower == null)
            {
                Log.Error("Someone tried to cast Wysson's Wildform, but didn't have the spell.");
                return false;
            }
            
            Pawn target = this.currentTarget.Thing as Pawn;
            Map map = theCaster.Map;

            FleckMaker.Static(caster.Position, map, RH_TET_MagicDefOf.RH_TET_FleckBrownEffect, 2f);
            FleckMaker.Static(caster.Position, map, FleckDefOf.PsycastAreaEffect, 2f);

            if (target != null)
            {
                flag1 = true;
                FleckMaker.Static(target.Position, map, RH_TET_MagicDefOf.RH_TET_FleckBrownEffect, 1.5f);
                FleckMaker.Static(target.Position, map, FleckDefOf.PsycastAreaEffect, .5f);

                switch (spellLevel)
                {
                    case 1:
                        target.health.AddHediff(RH_TET_MagicDefOf.RH_TET_BeastsWyssonsWildformI);
                        break;
                    case 2:
                        target.health.AddHediff(RH_TET_MagicDefOf.RH_TET_BeastsWyssonsWildformII);
                        break;
                    case 3:
                        target.health.AddHediff(RH_TET_MagicDefOf.RH_TET_BeastsWyssonsWildformIII);
                        break;
                    case 4:
                        target.health.AddHediff(RH_TET_MagicDefOf.RH_TET_BeastsWyssonsWildformIV);
                        break;
                    default:
                        break;
                }
            }
            
            this.PostCastShot(flag1, out flag1);
            return flag1;
        }

        public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
        {
            this.validTarg = targ.IsValid && targ.CenterVector3.InBounds(this.CasterPawn.Map) && !targ.Cell.Fogged(this.CasterPawn.Map) && targ.Cell.Walkable(this.CasterPawn.Map) && (double)(root - targ.Cell).LengthHorizontal < (double)(this.verbProps.range);
            return this.validTarg;
        }

        public Verb_WildForm()
            : base()
        {
        }
    }
}
