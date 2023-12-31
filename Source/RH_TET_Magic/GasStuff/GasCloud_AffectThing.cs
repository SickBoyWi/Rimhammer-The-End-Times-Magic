using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    public abstract class GasCloud_AffectThing : GasCloud
    {
        protected MoteProperties_GasEffect Props;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if ((this.Props = this.def.mote as MoteProperties_GasEffect) != null)
                return;
            Log.Error("GasCloud_AffectThing needs MoteProperties_GasEffect in def " + this.def.defName);
        }

        protected override void GasTick()
        {
            base.GasTick();
            if (!this.Spawned || (uint)(this.gasTicksProcessed % this.Props.effectInterval) > 0U)
                return;
            List<Verse.Thing> thingList = this.Map.thingGrid.ThingsListAt(this.Position);
            for (int index = 0; index < thingList.Count; ++index)
            {
                Verse.Thing thing = thingList[index];
                if (thing != this)
                {
                    float num = 0.0f;
                    if (thing is Pawn pawn && !pawn.Dead && (this.Props.affectsDownedPawns || !pawn.Downed) && (this.Props.affectsFleshy && pawn.def.race.IsFlesh || this.Props.affectsMechanical && pawn.RaceProps.IsMechanoid))
                        num = 1f * this.GetImmunizingApparelMultiplier(pawn) * this.GetSensitivityStatMultiplier(pawn);
                    else if (this.Props.affectsPlants && thing is Plant || this.Props.affectsThings)
                        num = 1f;
                    float strengthMultiplier = Mathf.Clamp01(num * Mathf.Clamp01(this.Concentration / this.Props.FullAlphaConcentration));
                    if ((double)strengthMultiplier > 0.0)
                        this.ApplyGasEffect(thing, strengthMultiplier);
                }
            }
        }

        protected abstract void ApplyGasEffect(Verse.Thing thing, float strengthMultiplier);

        protected float GetSensitivityStatMultiplier(Pawn pawn)
        {
            return (double)this.Props.toxicSensitivityStatPower > 0.0 ? (float)(1.0 - (1.0 - (double)Mathf.Clamp01(pawn.GetStatValue(StatDefOf.ToxicResistance, true))) * (double)this.Props.toxicSensitivityStatPower) : 1f;
        }

        protected float GetImmunizingApparelMultiplier(Pawn pawn)
        {
            if ((double)this.Props.immunizingApparelPower > 1.40129846432482E-45 && pawn.apparel != null)
            {
                List<Apparel> wornApparel = pawn.apparel.WornApparel;
                for (int index = 0; index < wornApparel.Count; ++index)
                {
                    if (this.Props.immunizingApparelDefs.Contains(wornApparel[index].def))
                        return 1f - this.Props.immunizingApparelPower;
                }
            }
            return 1f;
        }
    }
}
