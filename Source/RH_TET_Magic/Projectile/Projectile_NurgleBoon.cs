using AbilityUser;
using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    public class Projectile_NurgleBoon : Projectile_AbilityBase
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

            MoteMaker.MakeStaticMote(casterLocation, map, RH_TET_MagicDefOf.RH_TET_Mote_Afflictions, 1.4f);
            FleckMaker.Static(casterLocation, map, RH_TET_MagicDefOf.RH_TET_FleckGreenEffect, .2f);
            FleckMaker.Static(casterLocation, map, FleckDefOf.PsycastAreaEffect, 1.1f);


            Pawn pawn = hitThing as Pawn;
            if (pawn == null)
                return;

            using (IEnumerator<BodyPartRecord> enumerator1 = pawn.health.hediffSet.GetInjuredParts().GetEnumerator())
            {
                int num2 = RH_TET_MagicMod.random.Next(2, 5);

                List<Hediff> resultHediffs = new List<Hediff>();
                pawn.health.hediffSet.GetHediffs<Hediff>(ref resultHediffs, (Predicate<Hediff>)(hediffC => (!(hediffC is Hediff_Injury) && !(hediffC is Hediff_MissingPart) && (hediffC.Visible && hediffC.def.PossibleToDevelopImmunityNaturally()) && !hediffC.FullyImmune())));
                //List<Hediff> resultHediffsOrdered = (List < Hediff > )resultHediffs.OrderByDescending(h => h.Severity).GetEnumerator();

                using (IEnumerator<Hediff> enumerator2 = resultHediffs.OrderByDescending(h => h.Severity).GetEnumerator())
                {
                    while (((IEnumerator)enumerator2).MoveNext())
                    {
                        Hediff current = enumerator2.Current;

                        pawn.health.RemoveHediff(current);

                        IntVec3 position = pawn.Position;

                        FleckMaker.Static(targetLocation, map, RH_TET_MagicDefOf.RH_TET_FleckGreenEffect, .2f);
                        MoteMaker.MakeStaticMote(targetLocation, map, RH_TET_MagicDefOf.RH_TET_Mote_Afflictions, 1f);
                        FleckMaker.Static(targetLocation, map, FleckDefOf.PsycastAreaEffect, 1.1f);
                        break;
                    }
                }
            }
        }

        public Projectile_NurgleBoon()
            : base()
        {
        }
    }
}
