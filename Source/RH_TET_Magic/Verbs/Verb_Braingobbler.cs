using SickAbilityUser;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    public class Verb_Braingobbler : Verb_UseAbility
    {
        protected override bool TryCastShot()
        {
            // Miscast?
            if (MagicUtility.TryMiscast(this.caster))
                return true;
            
            bool flag1 = false;
            Pawn theCaster = caster as Pawn;
            
            MagicPower magicPower = null;
            int spellLevel = 0;
            foreach (MagicPower mp in this.CasterPawn.GetComp<CompMagicUser>().MagicData.PowersGreatMaw)
            {
                if (mp.abilityDef.defName.Contains("Braingobbler"))
                {
                    magicPower = mp;
                    spellLevel = mp.level;
                }
            }
            
            if (magicPower == null)
            {
                Log.Error("Someone tried to cast Braingobbler, but didn't have the spell.");
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

                // TODO - Could make certain outcomes more likely for higher spell levels.
                int rando = RH_TET_MagicMod.random.Next(0, 20);
                if (rando < 12 && target.RaceProps.Humanlike)
                { 
                    target.health.AddHediff(HediffDefOf.CatatonicBreakdown);
                }
                else
                {
                    if (target.RaceProps != null && target.RaceProps.Humanlike)
                        target.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Berserk, "terror response", true, false, false, (Pawn)null, true);
                    else
                        target.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Manhunter, "terror response", true, false, false, (Pawn)null, true);
                }
            }
            
            this.PostCastShot(flag1, out flag1);
            return flag1;
        }

        public Verb_Braingobbler()
            : base()
        {
        }
    }
}
