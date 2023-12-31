using AbilityUser;
using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    [StaticConstructorOnStartup]
    public class Projectile_ClearSky : Projectile_Ability
    {
        public Projectile_ClearSky()
            : base()
        {
        }

        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            base.Impact(hitThing);
            
            IntVec3 position = this.Caster.Position;
            Map map = this.Caster.Map;
            
            FleckMaker.ThrowSmoke(position.ToVector3(), map, 2f);
            FleckMaker.ThrowMicroSparks(position.ToVector3(), map);
            FleckMaker.Static(position, map, FleckDefOf.PsycastAreaEffect, 3f);

            List<GameCondition> conditions = map.gameConditionManager.ActiveConditions;
            List<GameConditionDef> affectedConditions = new List<GameConditionDef>();
            affectedConditions.Add(GameConditionDefOf.ColdSnap);
            affectedConditions.Add(GameConditionDefOf.Eclipse);
            affectedConditions.Add(GameConditionDefOf.EMIField);
            affectedConditions.Add(GameConditionDefOf.HeatWave);
            affectedConditions.Add(GameConditionDefOf.Flashstorm);
            affectedConditions.Add(GameConditionDefOf.HeatWave);
            affectedConditions.Add(GameConditionDefOf.PsychicDrone);
            affectedConditions.Add(GameConditionDefOf.PsychicSuppression);
            affectedConditions.Add(GameConditionDefOf.SolarFlare);
            affectedConditions.Add(GameConditionDefOf.ToxicFallout);
            affectedConditions.Add(GameConditionDefOf.ToxicSpewer);
            affectedConditions.Add(GameConditionDefOf.VolcanicWinter);
            affectedConditions.Add(GameConditionDefOf.WeatherController);

            GameConditionDef chaosStorm = DefDatabase<GameConditionDef>.GetNamed("RH_TET_ChaosStorm", false);
            if (chaosStorm != null)
                affectedConditions.Add(chaosStorm);

            bool foundCondition = false;
            String condLabel = null;

            foreach (GameCondition cond in conditions)
            {
                if (affectedConditions.Contains(cond.def))
                {
                    // Not permanent or forced.
                    if (!cond.Permanent)
                    {
                        condLabel = cond.def.label;
                        cond.Duration = 1;
                        foundCondition = true;
                        break;
                    }
                }
            }
            
            if (!foundCondition)
            {
                Messages.Message("RH_TET_MessageClearSkyFail".Translate(this.Caster.Name), MessageTypeDefOf.NegativeEvent);
            }
            else
            {
                Messages.Message("RH_TET_MessageClearSkySuccess".Translate(this.Caster.Name, condLabel), MessageTypeDefOf.PositiveEvent);
            }
        }
    }
}
