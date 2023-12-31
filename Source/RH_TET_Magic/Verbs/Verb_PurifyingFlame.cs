using AbilityUser;
using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    public class Verb_PurifyingFlame : Verb_UseAbility
    {
        protected override bool TryCastShot()
        {
            bool flag1 = false;
            Pawn theCaster = caster as Pawn;

            this.TargetsAoE.Clear();
            this.FindTargets();

            if (this.UseAbilityProps.AbilityTargetCategory != AbilityTargetCategory.TargetAoE && this.TargetsAoE.Count > 1)
                ((List<LocalTargetInfo>)this.TargetsAoE).RemoveRange(0, this.TargetsAoE.Count - 1);

            AbilityActionPower abilityPower = null;
            foreach (AbilityActionPower fp in this.CasterPawn.GetComp<CompAbilityActionUser>().AbilityActionData.PowersWitchHunter)
            {
                if (fp.abilityDef.defName.Contains("PurifyingFlame"))
                {
                    abilityPower = fp;
                }
            }
            
            if (abilityPower == null)
            {
                Log.Error("Someone tried to cast Purifying Flame, but didn't have the ability power.");
                return false;
            }
            
            if (this.TargetsAoE.Count <= 0)
                Messages.Message("RH_TET_MessageSpellFailTargets".Translate(theCaster.Name), MessageTypeDefOf.NegativeEvent);

            int maxTargets = this.UseAbilityProps.TargetAoEProperties.maxTargets;
            Map map = theCaster.Map;

            FleckMaker.Static(theCaster.Position, map, RH_TET_MagicDefOf.RH_TET_FleckYellowEffect, 2F);
            FleckMaker.Static(theCaster.Position, map, FleckDefOf.PsycastAreaEffect, 1.2f);

            for (int i = 0; i < this.TargetsAoE.Count; ++i)
            {
                if (maxTargets == 0)
                    break;
                
                LocalTargetInfo localTargetInfo = this.TargetsAoE[i];

                Pawn thing = localTargetInfo.Thing as Pawn;
                if (thing != null && !thing.Dead && thing.RaceProps.Humanlike && thing.Faction.IsPlayer && GenSight.LineOfSight(Caster.Position, thing.Position, Caster.Map, true, (Func<IntVec3, bool>)null, 0, 0))
                {
                    this.Heal(thing);
                    bool? attempt = TryLaunchProjectile(this.AbilityProjectileDef, this.TargetsAoE[i]);
                }
            }
            
            this.PostCastShot(flag1, out flag1);
            return flag1;
        }

        private void Heal(Pawn pawn)
        {
            Hediff firstHediffOfDef1 = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.ToxicBuildup, false);
            HediffDef chaosCorruption = DefDatabase<HediffDef>.GetNamedSilentFail("RH_TET_ChaosTaintBuildup");
            Hediff firstHediffOfDef2 = null;
            Hediff firstHediffOfDef3 = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.ToxGasExposure, false);
            HediffDef vfe_toxicBuildup = DefDatabase<HediffDef>.GetNamedSilentFail("VEF_ToxicBuildup");
            Hediff firstHediffOfDef4 = null;

            if (!(chaosCorruption is null))
                firstHediffOfDef2 = pawn.health.hediffSet.GetFirstHediffOfDef(chaosCorruption, false);
            if (!(vfe_toxicBuildup is null))
                firstHediffOfDef4 = pawn.health.hediffSet.GetFirstHediffOfDef(chaosCorruption, false);

            if (!(firstHediffOfDef1 is null))
            {
                pawn.health.RemoveHediff(firstHediffOfDef1);
            }
            if (!(firstHediffOfDef2 is null))
            {
                pawn.health.RemoveHediff(firstHediffOfDef2);
            }
            if (!(firstHediffOfDef3 is null))
            {
                pawn.health.RemoveHediff(firstHediffOfDef3);
            }
            if (!(firstHediffOfDef4 is null))
            {
                pawn.health.RemoveHediff(firstHediffOfDef2);
            }
        }

        private void FindTargets()
        {
            if (this.UseAbilityProps.AbilityTargetCategory == AbilityTargetCategory.TargetAoE)
            {
                if (this.UseAbilityProps.TargetAoEProperties == null)
                    Log.Error("Tried to Cast Area of Effect Ability Power without target class", false);
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
                int maxTargets = (int)((TargetAoEProperties)((VerbProperties_Ability)((AbilityUser.AbilityDef)this.UseAbilityProps.abilityDef).MainVerb).TargetAoEProperties).maxTargets;
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

        public Verb_PurifyingFlame()
            : base()
        {
        }
    }
}
