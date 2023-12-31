using AbilityUser;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    public class Verb_CurseOfHornedRat : Verb_UseAbility
    {
        protected override bool TryCastShot()
        {
            //if (!ModsConfig.IsActive("SickBoyWi.TheEndTimes.Skaven"))
            //{
            //    Messages.Message("Curse of the Horned Rat requires the Rimhammer Skaven mod be active, since it uses some pawn kinds from that mod.", MessageTypeDefOf.NegativeEvent);
            //    return false;
            //}

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
            foreach (MagicPower mp in this.CasterPawn.GetComp<CompMagicUser>().MagicData.PowersWarp)
            {
                if (mp.abilityDef.defName.Contains("CurseHornedRat"))
                {
                    magicPower = mp;
                }
            }
            
            if (magicPower == null)
            {
                Log.Error("Someone tried to cast Curse of the Horned Rat, but didn't have the spell.");
                return false;
            }
            
            if (this.TargetsAoE.Count <= 0)
                Messages.Message("RH_TET_MessageSpellFailTargets".Translate(theCaster.Name), MessageTypeDefOf.NegativeEvent);
            
            int maxTargets = this.UseAbilityProps.TargetAoEProperties.maxTargets;
            Map map = theCaster.Map;
            
            magicPower.abilityDef.MainVerb.soundCast.PlayOneShotOnCamera();
            
            for (int i = 0; i < this.TargetsAoE.Count; ++i)
            {
                if (maxTargets == 0)
                    break;
                
                LocalTargetInfo localTargetInfo = this.TargetsAoE[i];
                Pawn target = localTargetInfo.Thing as Pawn;
                if (target != null && target.RaceProps.Humanlike && target.kindDef != null && !target.kindDef.defName.Contains("Skaven"))
                {
                    float pawnCombatLevel = target.kindDef.combatPower;

                    FleckMaker.Static(target.Position, target.Map, RH_TET_MagicDefOf.RH_TET_FleckGreenEffect, .5f);
                    FleckMaker.Static(target.Position, target.Map, FleckDefOf.PsycastAreaEffect, .5f);

                    TryLaunchProjectile(verbProps.defaultProjectile, TargetsAoE[i]);
                    IntVec3 targetPosition = target.PositionHeld;

                    // Get new pawntype based on combat power of old pawn. 
                    PawnKindDef newPawnKind = GetNewPawnKindDef(MagicUtility.IsMagicUser(target), pawnCombatLevel);

                    // Generate new pawn. Use age, and maybe injuries on new pawn that old pawn had. Consider age of dawi wouldn't be appropriate.
                    int age = RH_TET_MagicMod.random.Next(20, 40);
                    PawnGenerationRequest request = new PawnGenerationRequest(newPawnKind, null, PawnGenerationContext.NonPlayer, -1,
                        true, false, false,
                        false, false, 0.00f, 
                        true, true, true, 
                        true, false, false, 
                        false, false, false,
                        0.0f, 0.0f, null, 
                        0.0f, null, null, 
                        null, null, null, 
                        age, age);

                    //public PawnGenerationRequest(
                    //PawnKindDef kind, Faction faction = null, PawnGenerationContext context = PawnGenerationContext.NonPlayer, int tile = -1, 
                    //bool forceGenerateNewPawn = false, bool newborn = false, bool allowDead = false, 
                    //bool allowDowned = false, bool canGeneratePawnRelations = true, bool mustBeCapableOfViolence = false, 
                    //float colonistRelationChanceFactor = 1, bool forceAddFreeWarmLayerIfNeeded = false, bool allowGay = true, 
                    //bool allowFood = true, bool allowAddictions = true, bool inhabitant = false, 
                    //bool certainlyBeenInCryptosleep = false, bool forceRedressWorldPawnIfFormerColonist = false, bool worldPawnFactionDoesntMatter = false, 
                    //float biocodeWeaponChance = 0, Pawn extraPawnForExtraRelationChance = null, float relationWithExtraPawnChanceFactor = 1, 
                    //Predicate<Pawn> validatorPreGear = null, Predicate<Pawn> validatorPostGear = null, IEnumerable<TraitDef> forcedTraits = null, 
                    //IEnumerable<TraitDef> prohibitedTraits = null, float? minChanceToRedressWorldPawn = null, float? fixedBiologicalAge = null, 
                    //float? fixedChronologicalAge = null, Gender? fixedGender = null, float? fixedMelanin = null, 
                    //string fixedLastName = null, string fixedBirthName = null, RoyalTitleDef fixedTitle = null);

                    Pawn newPawn = PawnGenerator.GeneratePawn(request);

                    // Kill target.
                    DamageInfo dinfo = new DamageInfo(RH_TET_MagicDefOf.RH_TET_MagicalInjury, 99999f, 999f, -1f, caster, target.health.hediffSet.GetBrain(), (ThingDef)null, DamageInfo.SourceCategory.Collapse, (Thing)null);
                    target.TakeDamage(dinfo);
                    if (!target.Dead)
                        target.Kill(new DamageInfo?(dinfo), (Hediff)null);

                    // Find target corpse and destroy.
                    target.Corpse.Destroy(DestroyMode.KillFinalize);

                    // Set new pawn faction.
                    newPawn.SetFaction(caster.Faction);

                    // Spawn new pawn.
                    GenSpawn.Spawn((Thing)newPawn, targetPosition, map, Rot4.Random, WipeMode.Vanish, false);

                    continue;
                }
            }

            this.PostCastShot(flag1, out flag1);
            return flag1;
        }

        private PawnKindDef GetNewPawnKindDef(bool isMagicUser, float pawnCombatLevel)
        {
            String pawnKindDefString = null;

            if (!ModsConfig.IsActive("SickBoyWi.TheEndTimes.Skaven"))
            {
                pawnKindDefString = "RH_TET_Skaven_WizardStandard";
                PawnKindDef retValNoSkaven = null;

                Log.Warning("Rimhammer Skaven mod not active. Selecting random pawnkind from player faction to create.");

                if (pawnKindDefString != null)
                    retValNoSkaven = caster.Faction.RandomPawnKind();

                return retValNoSkaven;
            }
                
            if (isMagicUser)
            {
                pawnKindDefString = "RH_TET_Skaven_WizardStandard";
            }
            else if (pawnCombatLevel < 35)
            {
                pawnKindDefString = "RH_TET_Skaven_PlayerSlave";
            }
            else if (pawnCombatLevel >= 35 && pawnCombatLevel < 48)
            {
                pawnKindDefString = "RH_TET_Skaven_PlayerSlave";
            }
            else if (pawnCombatLevel >= 48 && pawnCombatLevel < 60)
            {
                pawnKindDefString = "RH_TET_Skaven_ClanPlayer";
            }
            else if (pawnCombatLevel >= 60 && pawnCombatLevel < 90)
            {
                pawnKindDefString = "RH_TET_Skaven_ClanPlayer";
            }
            else if (pawnCombatLevel >= 90 && pawnCombatLevel <= 130)
            {
                pawnKindDefString = "RH_TET_Skaven_StormPlayer";
            }
            else if (pawnCombatLevel > 130)
            {
                pawnKindDefString = "RH_TET_Skaven_StormPlayer";
            }

            PawnKindDef retVal = null;
            
            if (pawnKindDefString != null)
                retVal = DefDatabase<PawnKindDef>.GetNamedSilentFail(pawnKindDefString);

            return retVal;
        }

        private void FindTargets()
        {
            if (this.UseAbilityProps.AbilityTargetCategory == AbilityTargetCategory.TargetAoE)
            {
                if (this.UseAbilityProps.TargetAoEProperties == null)
                    Log.Error("Tried to Cast Area of Effect Spell without target class", false);
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

        public Verb_CurseOfHornedRat()
            : base()
        {
        }
    }
}
