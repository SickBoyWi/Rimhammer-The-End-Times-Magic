﻿using SickAbilityUser;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    public class Verb_CurePoison : Verb_UseAbility
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

            MagicPower magicPower = null;
            foreach (MagicPower mp in this.CasterPawn.GetComp<CompMagicUser>().MagicData.PowersAddons)
            {
                if (mp.abilityDef.defName.Contains("WeakenPoison"))
                {
                    magicPower = mp;
                }
            }
            
            if (magicPower == null)
            {
                Log.Error("Someone tried to cast Cure Poison, but didn't have the spell.");
                return false;
            }
            
            if (this.TargetsAoE.Count <= 0)
                Messages.Message("RH_TET_MessageSpellFailTargets".Translate(theCaster.Name), MessageTypeDefOf.NegativeEvent);
            
            int maxTargets = this.UseAbilityProps.TargetAoEProperties.maxTargets;
            Map map = theCaster.Map;
            IntVec3 casterLocation = theCaster.Position;
            magicPower.abilityDef.MainVerb.soundCast.PlayOneShotOnCamera();

            FleckMaker.Static(casterLocation, map, RH_TET_MagicDefOf.RH_TET_FleckGreenEffectFancy, 2f);
            FleckMaker.Static(casterLocation, map, FleckDefOf.PsycastAreaEffect, 2f);

            for (int i = 0; i < this.TargetsAoE.Count; ++i)
            {
                if (maxTargets == 0)
                    break;
                
                LocalTargetInfo localTargetInfo = this.TargetsAoE[i];
                Pawn target = localTargetInfo.Thing as Pawn;
                if (target != null)
                {
                    FleckMaker.Static(target.Position, map, RH_TET_MagicDefOf.RH_TET_FleckGreenEffectFancy, 1.5f);
                    FleckMaker.Static(target.Position, map, FleckDefOf.PsycastAreaEffect, 1f);

                    List<Hediff> hediffsToRemove = new List<Hediff>();
                    foreach (Hediff hed in target.health.hediffSet.hediffs)
                    {
                        String hedDefName = null;
                        if (hed != null && hed.def != null)
                            hedDefName = hed.def.defName;

                        if (hedDefName != null && (hedDefName.Contains("Toxic") || hedDefName.Contains("Poison") || hedDefName.Contains("Taint")))
                            hediffsToRemove.Add(hed);
                    }

                    foreach (Hediff hediff in hediffsToRemove)
                    { 
                        target.health.RemoveHediff(hediff);
                        Messages.Message("RH_TET_MessageCuredOf".Translate(target.Name, hediff.def.label), MessageTypeDefOf.PositiveEvent);
                    }

                    maxTargets--;
                    flag1 = true;
                    continue;
                }
            }

            this.PostCastShot(flag1, out flag1);
            return flag1;
        }

        private void FindTargets()
        {
            if (this.UseAbilityProps.AbilityTargetCategory == AbilityTargetCategory.TargetAoE)
            {
                if (this.UseAbilityProps.TargetAoEProperties == null)
                    Log.Error("Tried to Cast Area of Effect Spell without target class");
                List<Thing> thingList = new List<Thing>();
                IntVec3 aoeStartPosition = ((Verb)this).caster.PositionHeld;
                if (!((TargetAoEProperties)this.UseAbilityProps.TargetAoEProperties).startsFromCaster)
                    aoeStartPosition = this.currentTarget.Cell;
                List<Thing> list;

                if (!((TargetAoEProperties)this.UseAbilityProps.TargetAoEProperties).friendlyFire)
                    list = ((Verb)this).caster.Map.listerThings.AllThings.Where<Thing>((Func<Thing, bool>)(x =>
                    {
                        if (x.Position.InHorDistOf(aoeStartPosition, (float)((TargetAoEProperties)this.UseAbilityProps.TargetAoEProperties).range) && ((System.Type)((TargetAoEProperties)this.UseAbilityProps.TargetAoEProperties).targetClass).IsAssignableFrom(x.GetType()))
                            return x.Faction != Faction.OfPlayer;
                        return false;
                    })).ToList<Thing>();

                else if (((TargetAoEProperties)this.UseAbilityProps.TargetAoEProperties).targetClass == typeof(Plant) || ((TargetAoEProperties)this.UseAbilityProps.TargetAoEProperties).targetClass == typeof(Building))
                {
                    using (List<Thing>.Enumerator enumerator = ((Verb)this).caster.Map.listerThings.AllThings.Where<Thing>((Func<Thing, bool>)(x =>
                    {
                        if (x.Position.InHorDistOf(aoeStartPosition, (float)((TargetAoEProperties)this.UseAbilityProps.TargetAoEProperties).range))
                            return ((System.Type)((TargetAoEProperties)this.UseAbilityProps.TargetAoEProperties).targetClass).IsAssignableFrom(x.GetType());
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
                        if (!x.Position.InHorDistOf(aoeStartPosition, (float)((TargetAoEProperties)this.UseAbilityProps.TargetAoEProperties).range) || !((System.Type)((TargetAoEProperties)this.UseAbilityProps.TargetAoEProperties).targetClass).IsAssignableFrom(x.GetType()))
                            return false;
                        if (!x.HostileTo(Faction.OfPlayer))
                        {
                            Pawn p = x as Pawn;
                            if(p != null)
                            {
                                foreach (Hediff hed in p.health.hediffSet.hediffs)
                                {
                                    String hedDefName = null;
                                    if (hed != null && hed.def != null)
                                    {
                                        hedDefName = hed.def.defName;
                                    }

                                    if (hedDefName != null && (hedDefName.Contains("Toxic") || hedDefName.Contains("Poison") || hedDefName.Contains("Taint")))
                                    {
                                        return (bool)((TargetAoEProperties)this.UseAbilityProps.TargetAoEProperties).friendlyFire;
                                    }
                                }
                            }
                        }
                        return false;
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

        public Verb_CurePoison()
            : base()
        {
        }
    }
}
