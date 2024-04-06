using SickAbilityUser;
using RimWorld;
using System;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    public class Projectile_EnstableMind : Projectile_AbilityBase
    {
        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            Map map = this.Map;
            base.Impact(hitThing);
            ThingDef def = this.def;
            Pawn launcher = this.launcher as Pawn;
            Pawn theTarget = hitThing as Pawn;
            
            IntVec3 casterLocation = launcher.Position;
            IntVec3 targetLocation = theTarget.Position;
            
            if (!theTarget.Destroyed && !theTarget.Dead && theTarget != null && theTarget.mindState != null && theTarget.mindState.mentalStateHandler != null
                && theTarget.mindState.mentalStateHandler.CurStateDef != null
                && theTarget.mindState.mentalStateHandler.CurStateDef.label != null)
            {
                String label = theTarget.mindState.mentalStateHandler.CurStateDef.label;

                FleckMaker.Static(casterLocation, launcher.Map, RH_TET_MagicDefOf.RH_TET_FleckGoldEffect, 1);
                FleckMaker.Static(casterLocation, launcher.Map, FleckDefOf.PsycastAreaEffect, 1.1f);
                theTarget.mindState.mentalStateHandler.Reset();
                Messages.Message("RH_TET_ClearMentalState".Translate(theTarget.Name, label), MessageTypeDefOf.PositiveEvent);
                FleckMaker.Static(targetLocation, launcher.Map, RH_TET_MagicDefOf.RH_TET_FleckGoldEffect, 1);
                FleckMaker.Static(targetLocation, launcher.Map, FleckDefOf.PsycastAreaEffect, 1.1f);
            }
            else
            {
                // Catatonic breakdown.
                Hediff firstHediffOfDef = theTarget.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.CatatonicBreakdown, false);
                if (firstHediffOfDef != null)
                {
                    FleckMaker.Static(casterLocation, launcher.Map, RH_TET_MagicDefOf.RH_TET_FleckGoldEffect, 1);
                    FleckMaker.Static(casterLocation, launcher.Map, FleckDefOf.PsycastAreaEffect, 1.1f);
                    theTarget.health.RemoveHediff(firstHediffOfDef);
                    Messages.Message("RH_TET_ClearMentalState".Translate(theTarget.Name, firstHediffOfDef.Label), MessageTypeDefOf.PositiveEvent);
                    FleckMaker.Static(targetLocation, launcher.Map, RH_TET_MagicDefOf.RH_TET_FleckGoldEffect, 1);
                    FleckMaker.Static(targetLocation, launcher.Map, FleckDefOf.PsycastAreaEffect, 1.1f);
                }
                else
                { 
                    Messages.Message("RH_TET_ClearMentalStateFail".Translate(launcher.Name, theTarget.Name), MessageTypeDefOf.NegativeEvent);
                }
            }
        }

        public Projectile_EnstableMind()
            : base()
        {
        }
    }
}
