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
    public class Projectile_Trollguts : Projectile_AbilityLaser
    {
        public override void Impact_Override(Thing hitThing)
        {
            Pawn thing = hitThing as Pawn;
            if (thing != null && !thing.Dead && thing.RaceProps.Humanlike && thing.Faction.IsPlayer && GenSight.LineOfSight(Caster.Position, hitThing.Position, Caster.Map, true, (Func<IntVec3, bool>)null, 0, 0))
            {
                int rando = RH_TET_MagicMod.random.Next(0, 10);
                Pawn pawnCaster = this.launcher as Pawn;
                int spellLevel = DetermineSpellLevel(pawnCaster);
                bool regrowLimb = false;

                switch (spellLevel)
                {
                    case 4:
                        if (rando < 6)
                            regrowLimb = true;
                        break;
                    default:
                        break;
                }

                if (regrowLimb)
                {
                    IntVec3 casterLocation = launcher.Position;
                    IntVec3 targetLocation = thing.Position;
                    Map map = launcher.Map;

                    FleckMaker.Static(casterLocation, map, RH_TET_MagicDefOf.RH_TET_FleckWhiteEffect, 1);
                    FleckMaker.Static(casterLocation, map, FleckDefOf.PsycastAreaEffect, 1f);

                    CompUseEffect_FixHealthConditionRH fixPawn = new CompUseEffect_FixHealthConditionRH();
                    fixPawn.DoEffect(thing);

                    FleckMaker.Static(targetLocation, map, RH_TET_MagicDefOf.RH_TET_FleckYellowEffect, 1);
                    FleckMaker.Static(targetLocation, map, FleckDefOf.PsycastAreaEffect, 1f);
                }
                else
                    this.Heal(thing);
            }
        }

        private void Heal(Pawn pawn)
        {
            int num1 = RH_TET_MagicMod.random.Next(2, 5);
            using (IEnumerator<BodyPartRecord> enumerator1 = pawn.health.hediffSet.GetInjuredParts().GetEnumerator())
            {
                while (enumerator1.MoveNext())
                {
                    BodyPartRecord rec = enumerator1.Current;
                    if (num1 > 0)
                    {
                        int num2 = RH_TET_MagicMod.random.Next(2, 5);

                        List<Hediff_Injury> resultHediffs = new List<Hediff_Injury>();
                        pawn.health.hediffSet.GetHediffs<Hediff_Injury>(ref resultHediffs, (Predicate<Hediff_Injury>)((injury => injury.Part == rec)));
                        IEnumerator<Hediff_Injury> enumerator2 = resultHediffs.GetEnumerator();

                        using (enumerator2)
                        {
                            while (((IEnumerator)enumerator2).MoveNext())
                            {
                                Hediff_Injury current = enumerator2.Current;
                                if (num2 > 0 && (HediffUtility.CanHealNaturally(current) && !HediffUtility.IsPermanent(current)))
                                {
                                    if (Rand.Chance(0.8f))
                                    {
                                        current.Heal((float)RH_TET_MagicMod.random.Next(10, 21));
                                        IntVec3 position = pawn.Position;
                                        FleckMaker.Static(pawn.Position, pawn.Map, RH_TET_MagicDefOf.RH_TET_FleckWhiteEffect, 1.5f);
                                        FleckMaker.Static(pawn.Position, pawn.Map, FleckDefOf.PsycastAreaEffect, 1.5f);
                                    }
                                    --num1;
                                    --num2;
                                }
                            }
                        }
                    }
                }
            }
        }

        private int DetermineSpellLevel(Pawn caster)
        {
            // Determine spell level.
            int spellLevel = -1;

            foreach (MagicPower mp in caster.GetComp<CompMagicUser>().MagicData.PowersGreatMaw)
            {
                if (mp.abilityDef.defName.Contains("Trollguts"))
                {
                    spellLevel = mp.level;
                }
            }

            return spellLevel;
        }

        public Projectile_Trollguts()
            : base()
        {
        }
    }
}
