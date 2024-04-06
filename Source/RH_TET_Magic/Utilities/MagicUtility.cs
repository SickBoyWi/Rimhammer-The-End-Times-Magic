using System;
using System.Collections.Generic;
using SickAbilityUser;
using RimWorld;
using Verse;
using AbilityDef = SickAbilityUser.AbilityDef;

namespace TheEndTimes_Magic
{
    internal static class MagicUtility
    {
        public static bool IsMagicUser(this Pawn p)
        {
            if (p != null)
            {
                TraitSet traits = p?.story?.traits;
                if (traits != null && (traits.HasTrait(RH_TET_MagicDefOf.RH_TET_LoreOfBeastsTrait) 
                    || traits.HasTrait(RH_TET_MagicDefOf.RH_TET_LoreOfTzeentchTrait) 
                    || traits.HasTrait(RH_TET_MagicDefOf.RH_TET_LoreOfShadowTrait) 
                    || traits.HasTrait(RH_TET_MagicDefOf.RH_TET_LoreOfDeathTrait) 
                    || traits.HasTrait(RH_TET_MagicDefOf.RH_TET_LoreOfWildTrait)
                    || traits.HasTrait(RH_TET_MagicDefOf.RH_TET_LoreOfFireTrait)
                    || traits.HasTrait(RH_TET_MagicDefOf.RH_TET_LoreOfHeavensTrait)
                    || traits.HasTrait(RH_TET_MagicDefOf.RH_TET_LoreOfMetalTrait)
                    || traits.HasTrait(RH_TET_MagicDefOf.RH_TET_LoreOfLightTrait)
                    || traits.HasTrait(RH_TET_MagicDefOf.RH_TET_LoreOfLifeTrait)
                    || traits.HasTrait(RH_TET_MagicDefOf.RH_TET_LoreOfNurgleTrait)
                    || traits.HasTrait(RH_TET_MagicDefOf.RH_TET_LoreOfChaosUndividedTrait)
                    || traits.HasTrait(RH_TET_MagicDefOf.RH_TET_LoreOfSlaaneshTrait)
                    || traits.HasTrait(RH_TET_MagicDefOf.RH_TET_LoreOfHighTrait)
                    || traits.HasTrait(RH_TET_MagicDefOf.RH_TET_LoreOfPlagueTrait)
                    || traits.HasTrait(RH_TET_MagicDefOf.RH_TET_LoreOfGreatMawTrait)
                    || traits.HasTrait(RH_TET_MagicDefOf.RH_TET_LoreOfWarpTrait)
                    || traits.HasTrait(RH_TET_MagicDefOf.RH_TET_LoreOfStealthTrait)
                    ))
                    return true;
            }
            return false;
        }

        internal static bool DoesPawnKnowNonLoreSpell(Pawn p, MagicAbilityDef RH_TET_AddOn_WeakenPoison_Mage)
        {
            CompMagicUser compMagicUser = ((p != null) ? p.TryGetComp<CompMagicUser>() : null);
            if (compMagicUser != null && compMagicUser.IsMagicUser)
            {
                List<MagicPower> addonPowers = compMagicUser.MagicData.PowersAddons;

                if (addonPowers != null && addonPowers.Count > 0)
                {
                    foreach (MagicPower mp in addonPowers)
                    {
                        foreach (AbilityDef ad in mp.abilityDefs)
                        {
                            if (ad != null && ad.defName.Equals(RH_TET_AddOn_WeakenPoison_Mage.defName))
                                return true;
                        }

                    }
                }
            }
            return false;
        }

        internal static bool HasReachedNonLoreSpellLimit(Pawn p)
        {
            CompMagicUser compMagicUser = ((p != null) ? p.TryGetComp<CompMagicUser>() : null);
            if (compMagicUser != null && compMagicUser.IsMagicUser)
            {
                List<MagicPower> addonPowers = compMagicUser.MagicData.PowersAddons;

                if (addonPowers != null && addonPowers.Count >= 5)
                    return true;
            }
            return false;
        }

        public static bool IsFaithUser(this Pawn p)
        {
            if (p != null)
            {
                TraitSet traits = p?.story?.traits;
                if (traits != null && (traits.HasTrait(RH_TET_MagicDefOf.RH_TET_SigmarTrait)
                    || traits.HasTrait(RH_TET_MagicDefOf.RH_TET_ShallyaTrait)
                    || traits.HasTrait(RH_TET_MagicDefOf.RH_TET_UlricTrait)
                    ))
                    return true;
            }
            return false;
        }

        public static bool IsFaithUserSigmar(this Pawn p)
        {
            if (p != null)
            {
                TraitSet traits = p?.story?.traits;
                if (traits != null && traits.HasTrait(RH_TET_MagicDefOf.RH_TET_SigmarTrait))
                    return true;
            }
            return false;
        }

        public static bool IsFaithUserShallya(this Pawn p)
        {
            if (p != null)
            {
                TraitSet traits = p?.story?.traits;
                if (traits != null && traits.HasTrait(RH_TET_MagicDefOf.RH_TET_ShallyaTrait))
                    return true;
            }
            return false;
        }

        public static bool IsWitchhunter(this Pawn p)
        {
            if (p != null)
            {
                TraitSet traits = p?.story?.traits;
                if (traits != null && traits.HasTrait(RH_TET_MagicDefOf.RH_TET_WitchHunterTrait))
                    return true;
            }
            return false;
        }

        public static bool IsFaithUserUlric(this Pawn p)
        {
            if (p != null)
            {
                TraitSet traits = p?.story?.traits;
                if (traits != null && traits.HasTrait(RH_TET_MagicDefOf.RH_TET_UlricTrait))
                    return true;
            }
            return false;
        }

        public static bool IsAbilityUser(this Pawn p)
        {
            if (p != null)
            {
                TraitSet traits = p?.story?.traits;
                if (traits != null && (traits.HasTrait(RH_TET_MagicDefOf.RH_TET_WitchHunterTrait)
                    //|| traits.HasTrait(RH_TET_MagicDefOf.RH_TET_ShallyaTrait)
                    //|| traits.HasTrait(RH_TET_MagicDefOf.RH_TET_UlricTrait)
                    ))
                    return true;
            }
            return false;
        }

        internal static void MakeGenericCastingMoteMagic(Pawn pawn)
        {
            if (pawn != null)
            { 
                MagicLoreType lore = MagicUtility.GetMagicLoreType(pawn);

                switch (lore)
                {
                    case MagicLoreType.Beasts:
                        FleckMaker.Static(pawn.Position, pawn.Map, RH_TET_MagicDefOf.RH_TET_FleckBrownEffect, 2f);
                        FleckMaker.Static(pawn.Position, pawn.Map, FleckDefOf.PsycastAreaEffect, 1.4f);
                        break;
                    case MagicLoreType.Death:
                        FleckMaker.Static(pawn.Position, pawn.Map, RH_TET_MagicDefOf.RH_TET_FleckPurpleEffect, 2f);
                        FleckMaker.Static(pawn.Position, pawn.Map, FleckDefOf.PsycastAreaEffect, 1.4f);
                        break;
                    case MagicLoreType.Fire:
                        FleckMaker.Static(pawn.Position, pawn.Map, RH_TET_MagicDefOf.RH_TET_FleckRedEffect, 2f);
                        FleckMaker.Static(pawn.Position, pawn.Map, FleckDefOf.PsycastAreaEffect, 1.4f);
                        break;
                    case MagicLoreType.Heavens:
                        FleckMaker.Static(pawn.Position, pawn.Map, RH_TET_MagicDefOf.RH_TET_FleckBlueEffect, 2f);
                        FleckMaker.Static(pawn.Position, pawn.Map, FleckDefOf.PsycastAreaEffect, 1.4f);
                        break;
                    case MagicLoreType.High:
                        FleckMaker.Static(pawn.Position, pawn.Map, RH_TET_MagicDefOf.RH_TET_FleckBlueEffect, 1f);
                        FleckMaker.Static(pawn.Position, pawn.Map, RH_TET_MagicDefOf.RH_TET_FleckWhiteEffect, 2f);
                        FleckMaker.Static(pawn.Position, pawn.Map, FleckDefOf.PsycastAreaEffect, 1.6f);
                        break;
                    case MagicLoreType.Light:
                        FleckMaker.Static(pawn.Position, pawn.Map, RH_TET_MagicDefOf.RH_TET_FleckWhiteEffect, 2f);
                        FleckMaker.Static(pawn.Position, pawn.Map, FleckDefOf.PsycastAreaEffect, 1.4f);
                        break;
                    case MagicLoreType.Life:
                        FleckMaker.Static(pawn.Position, pawn.Map, RH_TET_MagicDefOf.RH_TET_FleckGreenEffect, 2f);
                        FleckMaker.Static(pawn.Position, pawn.Map, FleckDefOf.PsycastAreaEffect, 1.4f);
                        break;
                    case MagicLoreType.GreatMaw:
                        FleckMaker.Static(pawn.Position, pawn.Map, RH_TET_MagicDefOf.RH_TET_FleckRedEffect, 2f);
                        FleckMaker.Static(pawn.Position, pawn.Map, RH_TET_MagicDefOf.RH_TET_FleckBrownEffect, .5f);
                        FleckMaker.Static(pawn.Position, pawn.Map, FleckDefOf.PsycastAreaEffect, 2f);
                        break;
                    case MagicLoreType.Tzeentch:
                        FleckMaker.Static(pawn.Position, pawn.Map, RH_TET_MagicDefOf.RH_TET_FleckPurpleEffect, 1f);
                        FleckMaker.Static(pawn.Position, pawn.Map, RH_TET_MagicDefOf.RH_TET_FleckRedEffect, 2f);
                        FleckMaker.Static(pawn.Position, pawn.Map, RH_TET_MagicDefOf.RH_TET_FleckBlueEffect, .5f);
                        FleckMaker.Static(pawn.Position, pawn.Map, FleckDefOf.PsycastAreaEffect, 2f);
                        break;
                    case MagicLoreType.Metal:
                        FleckMaker.Static(pawn.Position, pawn.Map, RH_TET_MagicDefOf.RH_TET_FleckYellowEffect, 2f);
                        FleckMaker.Static(pawn.Position, pawn.Map, FleckDefOf.PsycastAreaEffect, 1.4f);
                        break;
                    case MagicLoreType.Shadow:
                    case MagicLoreType.Stealth:
                        FleckMaker.Static(pawn.Position, pawn.Map, RH_TET_MagicDefOf.RH_TET_FleckGrayEffect, 2f);
                        FleckMaker.Static(pawn.Position, pawn.Map, FleckDefOf.PsycastAreaEffect, 1.4f);
                        break;
                    case MagicLoreType.Wild:
                        FleckMaker.Static(pawn.Position, pawn.Map, RH_TET_MagicDefOf.RH_TET_FleckBrownEffect, 2f);
                        FleckMaker.Static(pawn.Position, pawn.Map, FleckDefOf.PsycastAreaEffect, 1.4f);
                        break;
                    case MagicLoreType.Plague:
                    case MagicLoreType.Nurgle:
                        FleckMaker.Static(pawn.Position, pawn.Map, RH_TET_MagicDefOf.RH_TET_FleckGreenEffect, 2f);
                        FleckMaker.Static(pawn.Position, pawn.Map, RH_TET_MagicDefOf.RH_TET_FleckBrownEffect, 2f);
                        FleckMaker.Static(pawn.Position, pawn.Map, FleckDefOf.PsycastAreaEffect, 1.4f);
                        break;
                    case MagicLoreType.ChaosUndivided:
                        FleckMaker.Static(pawn.Position, pawn.Map, RH_TET_MagicDefOf.RH_TET_Fleck_ChaosStar, 2f);
                        FleckMaker.Static(pawn.Position, pawn.Map, RH_TET_MagicDefOf.RH_TET_FleckBlueEffect, .5f);
                        FleckMaker.Static(pawn.Position, pawn.Map, RH_TET_MagicDefOf.RH_TET_FleckRedEffect, 2f);
                        FleckMaker.Static(pawn.Position, pawn.Map, FleckDefOf.PsycastAreaEffect, 1.4f);
                        break;
                    case MagicLoreType.Slaanesh:
                        FleckMaker.Static(pawn.Position, pawn.Map, RH_TET_MagicDefOf.RH_TET_FleckPinkEffect, 2f);
                        FleckMaker.Static(pawn.Position, pawn.Map, FleckDefOf.PsycastAreaEffect, 1.4f);
                        break;
                    case MagicLoreType.Warp:
                        FleckMaker.Static(pawn.Position, pawn.Map, RH_TET_MagicDefOf.RH_TET_FleckGreenEffect, 2f);
                        FleckMaker.Static(pawn.Position, pawn.Map, RH_TET_MagicDefOf.RH_TET_FleckGreenEffectFancy, 1f);
                        FleckMaker.Static(pawn.Position, pawn.Map, FleckDefOf.PsycastAreaEffect, 1.4f);
                        break;
                    default:
                        break;
                }
            }
        }
        internal static void MakeGenericCastingMoteFaith(Pawn pawn)
        {
            if (pawn != null)
            {
                FaithType lore = MagicUtility.GetFaithType(pawn);

                switch (lore)
                {
                    case FaithType.Sigmar:
                        FleckMaker.Static(pawn.Position, pawn.Map, RH_TET_MagicDefOf.RH_TET_FleckYellowEffect, 2.5f);
                        FleckMaker.Static(pawn.Position, pawn.Map, FleckDefOf.PsycastAreaEffect, 2.0f);
                        break;
                    case FaithType.Ulric:
                        FleckMaker.Static(pawn.Position, pawn.Map, RH_TET_MagicDefOf.RH_TET_FleckBlueEffect, 2.5f);
                        FleckMaker.Static(pawn.Position, pawn.Map, FleckDefOf.PsycastAreaEffect, 2.0f);
                        break;
                    case FaithType.Shallya:
                        FleckMaker.Static(pawn.Position, pawn.Map, RH_TET_MagicDefOf.RH_TET_FleckWhiteEffect, 2.5f);
                        FleckMaker.Static(pawn.Position, pawn.Map, FleckDefOf.PsycastAreaEffect, 2.0f);
                        break;
                    default:
                        break;
                }
            }
        }
        internal static void MakeGenericCastingMoteAbility(Pawn pawn)
        {
            if (pawn != null)
            {
                AbilityActionType abilityType = MagicUtility.GetAbilityActionType(pawn);

                switch (abilityType)
                {
                    case AbilityActionType.WitchHunter:
                        FleckMaker.Static(pawn.Position, pawn.Map, RH_TET_MagicDefOf.RH_TET_FleckYellowEffect, 2.5f);
                        FleckMaker.Static(pawn.Position, pawn.Map, FleckDefOf.PsycastAreaEffect, 2.0f);
                        break;
                    default:
                        break;
                }
            }
        }

        public static Need_MagicPool GetMagicPool(this Pawn pawn)
        {
            return pawn?.GetComp<CompMagicUser>()?.MagicPool;
        }

        public static Need_FaithPool GetFaithPool(this Pawn pawn)
        {
            return pawn?.GetComp<CompFaithUser>()?.FaithPool;
        }

        public static Need_AbilityPool GetAbilityActionPool(this Pawn pawn)
        {
            return pawn?.GetComp<CompAbilityActionUser>()?.AbilityPool;
        }

        public static bool TryMiscast(Thing caster)
        {
            if (!Settings.UseMiscasts)
                return false;

            Pawn casterPawn = caster as Pawn;
            CompMagicUser compMagicUser = ((casterPawn != null) ? casterPawn.TryGetComp<CompMagicUser>() : null);
            if (compMagicUser == null || !compMagicUser.IsMagicUser)
            {
                Log.Error("Attempted miscast try on non-magic user.");
                return false;
            }

            // This sets a 1 in 100 chance of miscast.
            int randomCap = 100;
            // This makes more experienced magic users less likely to miscast.
            randomCap += compMagicUser.MagicUserLevel * 2;
            
            if (RH_TET_MagicMod.random.Next(0, randomCap) == 0)
            //if (true) Always miscast. TESTING.
            {
                // If carrying WizardStaff or BrayStaff, disregard miscast on a 90% chance.
                if (casterPawn.equipment.Primary != null 
                    && (casterPawn.equipment.Primary.def.defName.Contains("WizardStaff") || casterPawn.equipment.Primary.def.defName.Contains("TzeentchStaff") || casterPawn.equipment.Primary.def.defName.Contains("BrayStaff")))
                {
                    randomCap = 10;
                    if (RH_TET_MagicMod.random.Next(0, randomCap) == 0)
                    {
                        return false;
                    }
                }
                
                GenExplosion.DoExplosion(
                    casterPawn.Position, casterPawn.Map, 1.0f, 
                    RH_TET_MagicDefOf.RH_TET_MagicFire, null, RH_TET_MagicMod.random.Next(25, 80), 
                    0, RH_TET_MagicDefOf.RH_TET_Magic_Miscast, null, 
                    null, null, null, 
                    0.0f, 1, new GasType?(GasType.BlindSmoke),
                    true);
                casterPawn.needs.TryGetNeed<Need_MagicPool>()?.SetLevelToZero();

                Messages.Message("RH_TET_MessageMiscast".Translate(casterPawn.Name), casterPawn, MessageTypeDefOf.NegativeHealthEvent, false);
                
                return true;
            }

            return false;
        }

        public static CompMagicUser GetMagicUser(Pawn pawn)
        {
            return pawn?.GetComp<CompMagicUser>() ?? (CompMagicUser)null;
        }

        public static CompFaithUser GetFaithUser(Pawn pawn)
        {
            return pawn?.GetComp<CompFaithUser>() ?? (CompFaithUser)null;
        }

        public static CompAbilityActionUser GetAbilityActionUser(Pawn pawn)
        {
            return pawn?.GetComp<CompAbilityActionUser>() ?? (CompAbilityActionUser)null;
        }

        public static MagicLoreType GetMagicLoreType(Pawn pawn)
        {
            CompMagicUser comp = pawn?.GetComp<CompMagicUser>();
            if (comp != null)
                return comp.MagicLoreType;
            return MagicLoreType.None;
        }

        public static FaithType GetFaithType(Pawn pawn)
        {
            CompFaithUser comp = pawn?.GetComp<CompFaithUser>();
            if (comp != null)
                return comp.FaithType;
            return FaithType.None;
        }

        public static AbilityActionType GetAbilityActionType(Pawn pawn)
        {
            CompAbilityActionUser comp = pawn?.GetComp<CompAbilityActionUser>();
            if (comp != null)
                return comp.AbilityActionType;
            return AbilityActionType.None;
        }

        public static bool HasFaithAbility(CompFaithUser comp, AbilityDef abilityDef)
        {
            if (comp != null && comp.AbilityData != null && abilityDef != null)
            {
                foreach (PawnAbility ability in comp.AbilityData.Powers)
                {
                    FaithAbility faithAbility = ability as FaithAbility;
                    if (faithAbility != null)
                    {
                        if (faithAbility.Def.defName.Equals(abilityDef.defName))
                            return true;
                    }
                }
            }

            return false;
        }

        public static bool HasMagicAbility(CompMagicUser comp, AbilityDef abilityDef)
        {
            if (comp != null && comp.AbilityData != null && abilityDef != null)
            {
                foreach (PawnAbility ability in comp.AbilityData.Powers)
                {
                    MagicAbility magicAbility = ability as MagicAbility;
                    if (magicAbility != null)
                    {
                        if (magicAbility.Def.defName.Equals(abilityDef.defName))
                            return true;
                    }
                }
            }

            return false;
        }

        public static bool HasAbilityActionAbility(CompAbilityActionUser comp, AbilityDef abilityDef)
        {
            if (comp != null && comp.AbilityData != null && abilityDef != null)
            {
                foreach (PawnAbility ability in comp.AbilityData.Powers)
                {
                    AbilityActionAbility actionAbil = ability as AbilityActionAbility;
                    if (actionAbil != null)
                    {
                        if (actionAbil.Def.defName.Equals(abilityDef.defName))
                            return true;
                    }
                }
            }

            return false;
        }
    }
}
