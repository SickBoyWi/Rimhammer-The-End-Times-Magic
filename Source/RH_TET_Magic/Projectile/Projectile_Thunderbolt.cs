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
    public class Projectile_Thunderbolt : Projectile_Ability
    {
        public Projectile_Thunderbolt()
            : base()
        {
        }

        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            base.Impact(hitThing);

            // Determine spell level.
            int spellLevel = -1;

            Pawn caster = this.launcher as Pawn;

            foreach (MagicPower mp in caster.GetComp<CompMagicUser>().MagicData.PowersHeavens)
            {
                if (mp.abilityDef.defName.Contains("Urannon"))
                {
                    spellLevel = mp.level;
                }
            }

            if (spellLevel <= 0)
            {
                foreach (FaithPower mp in caster.GetComp<CompFaithUser>().FaithData.PowersSigmar)
                {
                    if (mp.abilityDef.defName.Contains("Sigmar_Thunderbolt"))
                    {
                        spellLevel = 5;
                    }
                }

                if (spellLevel <= 0)
                {
                    return;
                }
            }

            FleckMaker.ThrowMicroSparks(caster.Position.ToVector3(), caster.Map);
            FleckMaker.Static(caster.Position, caster.Map, FleckDefOf.PsycastAreaEffect, 2f);

            IntVec3 position = this.usedTarget.CenterVector3.ToIntVec3();
            caster.Map.weatherManager.eventHandler.AddEvent((WeatherEvent)new WeatherEvent_LightningStrike(caster.Map, position));
            if (spellLevel > 2)
                caster.Map.weatherManager.eventHandler.AddEvent((WeatherEvent)new WeatherEvent_LightningStrike(caster.Map, position.RandomAdjacentCell8Way()));

            if (spellLevel == 5)
            {
                caster.Map.weatherManager.eventHandler.AddEvent((WeatherEvent)new WeatherEvent_LightningStrike(caster.Map, position.RandomAdjacentCell8Way()));
                caster.Map.weatherManager.eventHandler.AddEvent((WeatherEvent)new WeatherEvent_LightningStrike(caster.Map, position.RandomAdjacentCell8Way()));
                caster.Map.weatherManager.eventHandler.AddEvent((WeatherEvent)new WeatherEvent_LightningStrike(caster.Map, position.RandomAdjacentCell8Way()));
                caster.Map.weatherManager.eventHandler.AddEvent((WeatherEvent)new WeatherEvent_LightningStrike(caster.Map, position.RandomAdjacentCell8Way()));
            }
        }
    }
}
