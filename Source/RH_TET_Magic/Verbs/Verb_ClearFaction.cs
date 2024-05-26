using SickAbilityUser;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace TheEndTimes_Magic
{
    public class Verb_ClearFaction : Verb_UseAbility
    {
        public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
        {
            return targ.IsValid && targ.CenterVector3.InBounds(this.CasterPawn.Map) && !targ.Cell.Fogged(this.CasterPawn.Map) && (double)(root - targ.Cell).LengthHorizontal < (double)(this.verbProps.range); 
        }

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
            foreach (MagicPower mp in this.CasterPawn.GetComp<CompMagicUser>().MagicData.PowersShadow)
            {
                if (mp.abilityDef.defName.Contains("MindSlip"))
                {
                    magicPower = mp;
                }
            }

            if (magicPower == null)
            {
                Log.Error("Someone tried to cast Mind Slip, but didn't have the spell.");
                return false;
            }

            Map map = caster.Map;
            FleckMaker.Static(caster.Position, map, RH_TET_MagicDefOf.RH_TET_FleckGrayEffect, 2f);
            FleckMaker.Static(caster.Position, map, FleckDefOf.PsycastAreaEffect, 2f);

            if (this.TargetsAoE.Count <= 0)
                Messages.Message("RH_TET_MessageSpellFailTargets".Translate(theCaster.Name), MessageTypeDefOf.NegativeEvent);

            int targetsHit = 0;

            // Clear faction
            for (int i = 0; i < this.TargetsAoE.Count; ++i)
            {
                LocalTargetInfo localTargetInfo = this.TargetsAoE[i];

                // Do not target pawns that are already members of the caster's faction.
                if (localTargetInfo.Thing.Faction != this.CasterPawn.Faction)
                {
                    localTargetInfo = ((List<LocalTargetInfo>)this.TargetsAoE)[i];
                    Pawn thing = localTargetInfo.Thing as Pawn;
                    
                    // If it's humanlike.
                    if (thing.kindDef.RaceProps.Humanlike)
                    { 
                        if (thing.guest != null)
                                thing.guest.SetGuestStatus((Faction)null);
                        
                        thing.SetFaction((Faction)null, (Pawn)null);
                        targetsHit++;
                        FleckMaker.Static(thing.Position, map, RH_TET_MagicDefOf.RH_TET_FleckGrayEffect, 1.5f);
                        FleckMaker.Static(thing.Position, map, FleckDefOf.PsycastAreaEffect, 1.2f);
                    }
                }
            }

            if (targetsHit <= 0)
                Messages.Message("RH_TET_MessageSpellFailTargets".Translate(theCaster.Name), MessageTypeDefOf.NegativeEvent);

            this.PostCastShot(flag1, out flag1);
            return flag1;
        }

        private void FindTargets()
        {
            if (this.UseAbilityProps.AbilityTargetCategory == AbilityTargetCategory.TargetAoE)
            {
                if (this.UseAbilityProps.TargetAoEProperties == null)
                    Log.Error("Tried to Cast AoE-Ability without defining a target class");
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

        public Verb_ClearFaction()
            : base()
        {
        }
    }
}
