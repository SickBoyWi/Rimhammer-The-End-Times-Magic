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
    public class Projectile_Comet : Projectile_Ability
    {
        public Projectile_Comet()
            : base()
        {
        }

        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            base.Impact(hitThing);

            // Determine spell level.
            int spellLevel = -1;

            Pawn caster = this.launcher as Pawn;
            Color useColor = Color.blue;

            foreach (MagicPower mp in caster.GetComp<CompMagicUser>().MagicData.PowersHeavens)
            {
                if (mp.abilityDef.defName.Contains("Cassandora"))
                {
                    spellLevel = mp.level;
                }
            }

            if (spellLevel <= 0)
            {
                foreach (FaithPower mp in caster.GetComp<CompFaithUser>().FaithData.PowersSigmar)
                {
                    if (mp.abilityDef.defName.Contains("Sigmar_Comet"))
                    {
                        useColor = Color.yellow;
                        spellLevel = 5;
                    }
                }

                if (spellLevel <= 0)
                {
                    return;
                }
            }

            FleckMaker.ThrowTornadoDustPuff(caster.Position.ToVector3(), caster.Map, 3.0f, useColor);
            FleckMaker.Static(caster.Position, caster.Map, FleckDefOf.PsycastAreaEffect, 2.2f);
            IntVec3 position = this.usedTarget.CenterVector3.ToIntVec3();
            
            List<Thing> thingList = ThingSetMakerDefOf.Meteorite.root.Generate();
            SkyfallerMaker.SpawnSkyfaller(ThingDefOf.MeteoriteIncoming, (IEnumerable<Thing>)thingList, position, caster.Map);
            if (spellLevel > 3)
            {
                CellRect cr = CellRect.CenteredOn(position, 5);
                List<Thing> thingList2 = ThingSetMakerDefOf.Meteorite.root.Generate();
                SkyfallerMaker.SpawnSkyfaller(ThingDefOf.MeteoriteIncoming, (IEnumerable<Thing>)thingList2, cr.RandomCell, caster.Map);
            }
            if (spellLevel == 5)
            {
                CellRect cr = CellRect.CenteredOn(position, 5);
                List<Thing> thingList2 = ThingSetMakerDefOf.Meteorite.root.Generate();
                SkyfallerMaker.SpawnSkyfaller(ThingDefOf.MeteoriteIncoming, (IEnumerable<Thing>)thingList2, cr.RandomCell, caster.Map);
            }
        }
    }
}
