using SickAbilityUser;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    public class Verb_DrainLife : Verb_UseAbility
    {
        protected override bool TryCastShot()
        {
            // Miscast?
            if (MagicUtility.TryMiscast(this.caster))
                return true;
            
            bool flag1 = false;
            int spellLevel = -1;
            Pawn theCaster = caster as Pawn;

            foreach (MagicPower mp in this.CasterPawn.GetComp<CompMagicUser>().MagicData.PowersLife)
            {
                if (mp.abilityDef.defName.Contains("DrainLife"))
                {
                    spellLevel = mp.level;
                }
            }
            
            if (spellLevel <= 0)
                return false;

            this.TargetsAoE.Clear();
            this.FindTargets();

            if (this.UseAbilityProps.AbilityTargetCategory != AbilityTargetCategory.TargetAoE && this.TargetsAoE.Count > 1)
                ((List<LocalTargetInfo>)this.TargetsAoE).RemoveRange(0, this.TargetsAoE.Count - 1);

            MagicPower magicPower = null;
            foreach (MagicPower mp in this.CasterPawn.GetComp<CompMagicUser>().MagicData.PowersLife)
            {
                if (mp.abilityDef.defName.Contains("DrainLife"))
                {
                    magicPower = mp;
                }
            }
            
            if (magicPower == null)
            {
                Log.Error("Someone tried to cast Drain Life but didn't have the spell.");
                return false;
            }
            
            if (this.TargetsAoE.Count <= 0)
                Messages.Message("RH_TET_MessageSpellFailTargets".Translate(theCaster.Name), MessageTypeDefOf.NegativeEvent);

            for (int i = 0; i < this.TargetsAoE.Count; ++i)
            {
                LocalTargetInfo localTargetInfo = this.TargetsAoE[i];
                Pawn thing = localTargetInfo.Thing as Pawn;
                
                DamageInfo dinfo;
                
                DamageDef damageDef = RH_TET_MagicDefOf.RH_TET_MagicalInjury;
                
                this.TryLaunchProjectile(this.verbProps.defaultProjectile, thing);

                int num1 = magicPower.abilityDef.MainVerb.defaultProjectile.projectile.GetDamageAmount(null);
                
                float num2 = 999f;
                BodyPartRecord randomBodyPart = thing.health.hediffSet.GetRandomNotMissingPart(damageDef);
                
                DamageDef def = damageDef;
                
                double num3 = (double)num1;
                double num4 = (double)num2;

                dinfo = new DamageInfo(def, (float)num3, (float)num4, -1f, (Thing)null, randomBodyPart, (ThingDef)null, DamageInfo.SourceCategory.ThingOrUnknown, (Thing)null);
                
                thing.TakeDamage(dinfo);
                RH_TET_MagicDefOf.RH_TET_Magic_SoundWhoosh.PlayOneShotOnCamera(theCaster.Map);
                FleckMaker.ThrowMicroSparks(thing.DrawPos, thing.Map);
                
                Hediff_Injury result;

                List<Hediff_Injury> resultHediffs = new List<Hediff_Injury>();
                theCaster.health.hediffSet.GetHediffs<Hediff_Injury>(ref resultHediffs, (Predicate<Hediff_Injury>)(x =>
                {
                    if (x.CanHealNaturally())
                        return true;
                    return false;
                }));
                IEnumerator<Hediff_Injury> enumerator2 = resultHediffs.GetEnumerator();

                if (resultHediffs.TryRandomElement<Hediff_Injury>(out result))
                {
                    result.Heal((float)num1);
                    string text = "RH_TET_MessageDrainLifeSpellSuccess".Translate(theCaster.Name, thing.Name).CapitalizeFirst();
                    Messages.Message(text, MessageTypeDefOf.PositiveEvent);
                    FleckMaker.ThrowDustPuffThick(thing.DrawPos, thing.Map, 2.0f, UnityEngine.Color.magenta);
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
                            return (bool)((TargetAoEProperties)this.UseAbilityProps.TargetAoEProperties).friendlyFire;
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

        public Verb_DrainLife()
            : base()
        {
        }
    }
}
