using SickAbilityUser;
using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    public class Verb_TrollGuts : Verb_UseAbility
    {
        protected override bool TryCastShot()
        {
            // Miscast?
            if (MagicUtility.TryMiscast(this.caster))
                return true;

            bool flag1 = false;
            Pawn theCaster = caster as Pawn;

            this.TargetsAoE.Clear();
            this.FindTargets();

            if (this.UseAbilityProps.AbilityTargetCategory != AbilityTargetCategory.TargetAoE && this.TargetsAoE.Count > 1)
                ((List<LocalTargetInfo>)this.TargetsAoE).RemoveRange(0, this.TargetsAoE.Count - 1);

            MagicPower power = null;
            foreach (MagicPower fp in this.CasterPawn.GetComp<CompMagicUser>().MagicData.PowersGreatMaw)
            {
                if (fp.abilityDef.defName.Contains("Trollguts"))
                {
                    power = fp;
                }
            }
            
            if (power == null)
            {
                Log.Error("Someone tried to cast Troll Guts, but didn't have the magic power.");
                return false;
            }
            
            if (this.TargetsAoE.Count <= 0)
                Messages.Message("RH_TET_MessageSpellFailTargets".Translate(theCaster.Name), MessageTypeDefOf.NegativeEvent);
            
            int maxTargets = this.UseAbilityProps.TargetAoEProperties.maxTargets;
            Map map = theCaster.Map;

            int spellLevel = DetermineSpellLevel((Pawn)this.caster);
            bool regrowLimb = false;

            if (spellLevel == 4)
            {
                regrowLimb = true;
            }

            IntVec3 casterLocation = this.CasterPawn.Position;
            FleckMaker.Static(casterLocation, map, RH_TET_MagicDefOf.RH_TET_FleckWhiteEffect, 1);
            FleckMaker.Static(casterLocation, map, FleckDefOf.PsycastAreaEffect, 1f);

            for (int i = 0; i < this.TargetsAoE.Count; ++i)
            {
                if (maxTargets == 0)
                    break;
                
                LocalTargetInfo localTargetInfo = this.TargetsAoE[i];

                Pawn thing = localTargetInfo.Thing as Pawn;
                IntVec3 targetLocation = thing.Position;
                if (thing != null && !thing.Dead && thing.RaceProps.Humanlike && thing.Faction.IsPlayer && GenSight.LineOfSight(Caster.Position, thing.Position, Caster.Map, true, (Func<IntVec3, bool>)null, 0, 0))
                {
                    if (regrowLimb)
                    {
                        CompUseEffect_FixHealthConditionRH fixPawn = new CompUseEffect_FixHealthConditionRH();
                        bool? attempt = TryLaunchProjectile(this.AbilityProjectileDef, this.TargetsAoE[i]);
                        fixPawn.DoEffect(thing);
                    }
                    else
                    {
                        bool? attempt = TryLaunchProjectile(this.AbilityProjectileDef, this.TargetsAoE[i]);
                        this.Heal(thing);
                    }

                    FleckMaker.Static(targetLocation, map, RH_TET_MagicDefOf.RH_TET_FleckYellowEffect, 1);
                    FleckMaker.Static(targetLocation, map, FleckDefOf.PsycastAreaEffect, 1f);
                }
            }
            
            this.PostCastShot(flag1, out flag1);
            return flag1;
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

                        using (IEnumerator<Hediff_Injury> enumerator2 = resultHediffs.GetEnumerator())
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

        private void FindTargets()
        {
            if (this.UseAbilityProps.AbilityTargetCategory == AbilityTargetCategory.TargetAoE)
            {
                if (this.UseAbilityProps.TargetAoEProperties == null)
                    Log.Error("Tried to Cast Area of Effect Magic Power without target class", false);
                List<Thing> thingList = new List<Thing>();
                IntVec3 aoeStartPosition = ((Verb)this).caster.PositionHeld;
                if (!((TargetAoEProperties)this.UseAbilityProps.TargetAoEProperties).startsFromCaster)
                    aoeStartPosition = this.currentTarget.Cell;
                List<Thing> list;

                if (!((TargetAoEProperties)this.UseAbilityProps.TargetAoEProperties).friendlyFire)
                    list = ((Verb)this).caster.Map.listerThings.AllThings.Where<Thing>((Func<Thing, bool>)(x =>
                    {
                        if (x.Position.InHorDistOf(aoeStartPosition, (float)((TargetAoEProperties)this.UseAbilityProps.TargetAoEProperties).range) && ((System.Type)((TargetAoEProperties)this.UseAbilityProps.TargetAoEProperties).targetClass).IsAssignableFrom(x.GetType()))
                        {
                            return x.Faction != Faction.OfPlayer;
                        }

                        return false;
                    })).ToList<Thing>();

                else if (((TargetAoEProperties)this.UseAbilityProps.TargetAoEProperties).targetClass == typeof(Plant) || ((TargetAoEProperties)this.UseAbilityProps.TargetAoEProperties).targetClass == typeof(Building))
                {
                    using (List<Thing>.Enumerator enumerator = ((Verb)this).caster.Map.listerThings.AllThings.Where<Thing>((Func<Thing, bool>)(x =>
                    {
                        if (x.Position.InHorDistOf(aoeStartPosition, (float)((TargetAoEProperties)this.UseAbilityProps.TargetAoEProperties).range))
                        {
                            return ((System.Type)((TargetAoEProperties)this.UseAbilityProps.TargetAoEProperties).targetClass).IsAssignableFrom(x.GetType());
                        }

                        return false;
                    })).ToList<Thing>().GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                            ((List<LocalTargetInfo>)this.TargetsAoE).Add(new LocalTargetInfo(enumerator.Current));

                        return;
                    }
                }
                else
                {
                    thingList.Clear();
                    list = ((Verb)this).caster.Map.listerThings.AllThings.Where<Thing>((Func<Thing, bool>)(x =>
                    {
                        if (!caster.Position.InHorDistOf(x.Position, this.UseAbilityProps.range) || !x.Position.InHorDistOf(aoeStartPosition, (float)((TargetAoEProperties)this.UseAbilityProps.TargetAoEProperties).range) || !((System.Type)((TargetAoEProperties)this.UseAbilityProps.TargetAoEProperties).targetClass).IsAssignableFrom(x.GetType()))
                        {
                            return false;
                        }
                        if (!x.HostileTo(Faction.OfPlayer))
                        {
                            return (bool)((TargetAoEProperties)this.UseAbilityProps.TargetAoEProperties).friendlyFire;
                        }
                        return true;
                    })).ToList<Thing>();
                }
                int maxTargets = (int)((TargetAoEProperties)((VerbProperties_Ability)((SickAbilityUser.AbilityDef)this.UseAbilityProps.abilityDef).MainVerb).TargetAoEProperties).maxTargets;
                List<Thing> source = new List<Thing>(list.InRandomOrder<Thing>((IList<Thing>)null));
                for (int index = 0; index < maxTargets && index < source.Count<Thing>(); ++index)
                {
                    if (((VerbProperties)this.UseAbilityProps).targetParams.CanTarget(new TargetInfo(source[index])))
                        ((List<LocalTargetInfo>)this.TargetsAoE).Add(new LocalTargetInfo(source[index]));
                }

            }
            else
            {
                ((List<LocalTargetInfo>)this.TargetsAoE).Clear();
                ((List<LocalTargetInfo>)this.TargetsAoE).Add(this.currentTarget);
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

        public Verb_TrollGuts()
            : base()
        {
        }
    }
}
