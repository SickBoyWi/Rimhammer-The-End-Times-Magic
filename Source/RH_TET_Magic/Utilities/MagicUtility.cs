using System;
using System.Collections.Generic;
using SickAbilityUser;
using RimWorld;
using Verse;
using AbilityDef = SickAbilityUser.AbilityDef;
using UnityEngine;

namespace TheEndTimes_Magic
{
    [StaticConstructorOnStartup]
    public static class MagicUtility
    {
        public static readonly Texture2D beastsIcon = ContentFinder<Texture2D>.Get("Icons/beasts", true);
        public static readonly Texture2D deathIcon = ContentFinder<Texture2D>.Get("Icons/death", true);
        public static readonly Texture2D fireIcon = ContentFinder<Texture2D>.Get("Icons/fire", true);
        public static readonly Texture2D heavensIcon = ContentFinder<Texture2D>.Get("Icons/heavens", true);
        public static readonly Texture2D lifeIcon = ContentFinder<Texture2D>.Get("Icons/life", true);
        public static readonly Texture2D lightIcon = ContentFinder<Texture2D>.Get("Icons/light", true);
        public static readonly Texture2D metalIcon = ContentFinder<Texture2D>.Get("Icons/metal", true);
        public static readonly Texture2D shadowIcon = ContentFinder<Texture2D>.Get("Icons/shadow", true);

        public static readonly Texture2D chaosIcon = ContentFinder<Texture2D>.Get("Icons/chaos", true);
        public static readonly Texture2D highIcon = ContentFinder<Texture2D>.Get("Icons/high", true);
        public static readonly Texture2D mawIcon = ContentFinder<Texture2D>.Get("Icons/maw", true);
        public static readonly Texture2D nurgleIcon = ContentFinder<Texture2D>.Get("Icons/nurgle", true);
        public static readonly Texture2D plagueIcon = ContentFinder<Texture2D>.Get("Icons/plague", true);
        public static readonly Texture2D slaaneshIcon = ContentFinder<Texture2D>.Get("Icons/slaanesh", true);
        public static readonly Texture2D stealthIcon = ContentFinder<Texture2D>.Get("Icons/stealth", true);
        public static readonly Texture2D tzeentchIcon = ContentFinder<Texture2D>.Get("Icons/tzeentch", true);
        public static readonly Texture2D warpIcon = ContentFinder<Texture2D>.Get("Icons/warp", true);
        public static readonly Texture2D wildIcon = ContentFinder<Texture2D>.Get("Icons/wild", true);

        public static readonly Texture2D shallyaIcon = ContentFinder<Texture2D>.Get("Icons/shallya", true);
        public static readonly Texture2D sigmarIcon = ContentFinder<Texture2D>.Get("Icons/sigmar", true);
        public static readonly Texture2D ulricIcon = ContentFinder<Texture2D>.Get("Icons/ulric", true);

        public static readonly Texture2D witchhunterIcon = ContentFinder<Texture2D>.Get("Icons/witchhunter", true);

         static MagicUtility()
        {
        }

        public static List<TraitDef> AllAbilityTraits = new List<TraitDef>
        {
            RH_TET_MagicDefOf.RH_TET_SigmarTrait,
            RH_TET_MagicDefOf.RH_TET_ShallyaTrait,
            RH_TET_MagicDefOf.RH_TET_UlricTrait,
            RH_TET_MagicDefOf.RH_TET_LoreOfBeastsTrait,
            RH_TET_MagicDefOf.RH_TET_LoreOfWildTrait,
            RH_TET_MagicDefOf.RH_TET_LoreOfTzeentchTrait,
            RH_TET_MagicDefOf.RH_TET_LoreOfShadowTrait,
            RH_TET_MagicDefOf.RH_TET_LoreOfDeathTrait,
            RH_TET_MagicDefOf.RH_TET_LoreOfFireTrait,
            RH_TET_MagicDefOf.RH_TET_LoreOfHeavensTrait,
            RH_TET_MagicDefOf.RH_TET_LoreOfMetalTrait,
            RH_TET_MagicDefOf.RH_TET_LoreOfLightTrait,
            RH_TET_MagicDefOf.RH_TET_LoreOfLifeTrait,
            RH_TET_MagicDefOf.RH_TET_LoreOfNurgleTrait,
            RH_TET_MagicDefOf.RH_TET_LoreOfChaosUndividedTrait,
            RH_TET_MagicDefOf.RH_TET_LoreOfSlaaneshTrait,
            RH_TET_MagicDefOf.RH_TET_LoreOfHighTrait,
            RH_TET_MagicDefOf.RH_TET_LoreOfPlagueTrait,
            RH_TET_MagicDefOf.RH_TET_LoreOfGreatMawTrait,
            RH_TET_MagicDefOf.RH_TET_LoreOfWarpTrait,
            RH_TET_MagicDefOf.RH_TET_LoreOfStealthTrait,
            RH_TET_MagicDefOf.RH_TET_WitchHunterTrait
        };

        public static TraitDef GetFirstAbilityTrait (this Pawn p)
        {
            TraitSet traits = p?.story?.traits;

            for (int i = 0; i < AllAbilityTraits.Count; i++)
            {
                if (traits.HasTrait(AllAbilityTraits[i]))
                    return AllAbilityTraits[i];
            }

            return null;
        }

        public static IconUtilityData GetIconInfoForTrait(TraitDef abilityTrait)
        {
            IconUtilityData icu;

            if (abilityTrait == RH_TET_MagicDefOf.RH_TET_LoreOfBeastsTrait)
            {
                icu = new IconUtilityData();
                icu.Texture = MagicUtility.beastsIcon;
                icu.ToolTip = "RH_TET_Magic_LoreOfBeasts".Translate();
                return icu;
            }
            else if (abilityTrait == RH_TET_MagicDefOf.RH_TET_LoreOfChaosUndividedTrait)
            {
                icu = new IconUtilityData();
                icu.Texture = MagicUtility.chaosIcon;
                icu.ToolTip = "RH_TET_LoreOfChaos".Translate();
                return icu;
            }
            else if (abilityTrait == RH_TET_MagicDefOf.RH_TET_LoreOfDeathTrait)
            {
                icu = new IconUtilityData();
                icu.Texture = MagicUtility.deathIcon;
                icu.ToolTip = "RH_TET_LoreOfDeath".Translate();
                return icu;
            }
            else if (abilityTrait == RH_TET_MagicDefOf.RH_TET_LoreOfFireTrait)
            {
                icu = new IconUtilityData();
                icu.Texture = MagicUtility.fireIcon;
                icu.ToolTip = "RH_TET_LoreOfFire".Translate();
                return icu;
            }
            else if (abilityTrait == RH_TET_MagicDefOf.RH_TET_LoreOfHeavensTrait)
            {
                icu = new IconUtilityData();
                icu.Texture = MagicUtility.heavensIcon;
                icu.ToolTip = "RH_TET_LoreOfHeavens".Translate();
                return icu;
            }
            else if (abilityTrait == RH_TET_MagicDefOf.RH_TET_LoreOfHighTrait)
            {
                icu = new IconUtilityData();
                icu.Texture = MagicUtility.highIcon;
                icu.ToolTip = "RH_TET_LoreOfHigh".Translate();
                return icu;
            }
            else if (abilityTrait == RH_TET_MagicDefOf.RH_TET_LoreOfLifeTrait)
            {
                icu = new IconUtilityData();
                icu.Texture = MagicUtility.lifeIcon;
                icu.ToolTip = "RH_TET_LoreOfLife".Translate();
                return icu;
            }
            else if (abilityTrait == RH_TET_MagicDefOf.RH_TET_LoreOfLightTrait)
            {
                icu = new IconUtilityData();
                icu.Texture = MagicUtility.lightIcon;
                icu.ToolTip = "RH_TET_LoreOfLight".Translate();
                return icu;
            }
            else if (abilityTrait == RH_TET_MagicDefOf.RH_TET_LoreOfGreatMawTrait)
            {
                icu = new IconUtilityData();
                icu.Texture = MagicUtility.mawIcon;
                icu.ToolTip = "RH_TET_LoreOfMaw".Translate();
                return icu;
            }
            else if (abilityTrait == RH_TET_MagicDefOf.RH_TET_LoreOfMetalTrait)
            {
                icu = new IconUtilityData();
                icu.Texture = MagicUtility.metalIcon;
                icu.ToolTip = "RH_TET_LoreOfMetal".Translate();
                return icu;
            }
            else if (abilityTrait == RH_TET_MagicDefOf.RH_TET_LoreOfNurgleTrait)
            {
                icu = new IconUtilityData();
                icu.Texture = MagicUtility.nurgleIcon;
                icu.ToolTip = "RH_TET_LoreOfNurgle".Translate();
                return icu;
            }
            else if (abilityTrait == RH_TET_MagicDefOf.RH_TET_LoreOfPlagueTrait)
            {
                icu = new IconUtilityData();
                icu.Texture = MagicUtility.plagueIcon;
                icu.ToolTip = "RH_TET_LoreOfPlague".Translate();
                return icu;
            }
            else if (abilityTrait == RH_TET_MagicDefOf.RH_TET_LoreOfShadowTrait)
            {
                icu = new IconUtilityData();
                icu.Texture = MagicUtility.shadowIcon;
                icu.ToolTip = "RH_TET_LoreOfShadow".Translate();
                return icu;
            }
            else if (abilityTrait == RH_TET_MagicDefOf.RH_TET_LoreOfSlaaneshTrait)
            {
                icu = new IconUtilityData();
                icu.Texture = MagicUtility.slaaneshIcon;
                icu.ToolTip = "RH_TET_LoreOfSlaanesh".Translate();
                return icu;
            }
            else if (abilityTrait == RH_TET_MagicDefOf.RH_TET_LoreOfStealthTrait)
            {
                icu = new IconUtilityData();
                icu.Texture = MagicUtility.stealthIcon;
                icu.ToolTip = "RH_TET_LoreOfStealth".Translate();
                return icu;
            }
            else if (abilityTrait == RH_TET_MagicDefOf.RH_TET_LoreOfTzeentchTrait)
            {
                icu = new IconUtilityData();
                icu.Texture = MagicUtility.tzeentchIcon;
                icu.ToolTip = "RH_TET_LoreOfTzeentch".Translate();
                return icu;
            }
            else if (abilityTrait == RH_TET_MagicDefOf.RH_TET_LoreOfWarpTrait)
            {
                icu = new IconUtilityData();
                icu.Texture = MagicUtility.warpIcon;
                icu.ToolTip = "RH_TET_LoreOfWarp".Translate();
                return icu;
            }
            else if (abilityTrait == RH_TET_MagicDefOf.RH_TET_LoreOfWildTrait)
            {
                icu = new IconUtilityData();
                icu.Texture = MagicUtility.wildIcon;
                icu.ToolTip = "RH_TET_LoreOfWild".Translate();
                return icu;
            }
            // end magic, start faith
            else if (abilityTrait == RH_TET_MagicDefOf.RH_TET_ShallyaTrait)
            {
                icu = new IconUtilityData();
                icu.Texture = MagicUtility.shallyaIcon;
                icu.ToolTip = "RH_TET_FaithOfShallya".Translate();
                return icu;
            }
            else if (abilityTrait == RH_TET_MagicDefOf.RH_TET_SigmarTrait)
            {
                icu = new IconUtilityData();
                icu.Texture = MagicUtility.sigmarIcon;
                icu.ToolTip = "RH_TET_FaithOfSigmar".Translate();
                return icu;
            }
            else if (abilityTrait == RH_TET_MagicDefOf.RH_TET_UlricTrait)
            {
                icu = new IconUtilityData();
                icu.Texture = MagicUtility.ulricIcon;
                icu.ToolTip = "RH_TET_FaithOfUlric".Translate();
                return icu;
            }
            // end faith, start abilities
            else if (abilityTrait == RH_TET_MagicDefOf.RH_TET_WitchHunterTrait)
            {
                icu = new IconUtilityData();
                icu.Texture = MagicUtility.witchhunterIcon;
                icu.ToolTip = "RH_TET_AbilityOfWitchhunter".Translate();
                return icu;
            }

            return null;
        }

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
                    new float?(), new int(),
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
