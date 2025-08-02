using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using Verse;
using RimWorld;
using UnityEngine;
using Verse.AI;
using Verse.AI.Group;
using TheEndTimes_Magic;
using System.Reflection;
using RimWorld.Planet;
using SickAbilityUser;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    [StaticConstructorOnStartup]
    static class HarmonyPatches
    {
        //public static PawnKindDef SpaceRefugee;

        static HarmonyPatches()
        {
            Harmony harmony = new Harmony("rimworld.theendtimes.magic");

            harmony.Patch(AccessTools.Method(typeof(SkillRecord), "Learn", null, null), null, 
                new HarmonyMethod(typeof(HarmonyPatches), "Learn_PostFix", null), null);
            harmony.Patch(AccessTools.Method(typeof(PawnRenderUtility), "DrawEquipmentAndApparelExtras", null, null), null,
                new HarmonyMethod(typeof(HarmonyPatches), "DrawEquipment_PostFix", null), null);
            harmony.Patch(AccessTools.Method(typeof(Pawn_ApparelTracker), "GetGizmos", null, null), null,
                new HarmonyMethod(typeof(HarmonyPatches), "Pawn_ApparelTracker_GetGizmos_PostFix", null), null);
            harmony.Patch(AccessTools.DeclaredPropertyGetter(typeof(Pawn), "HealthScale"), null,
                new HarmonyMethod(typeof(HarmonyPatches), "Pawn_HealthScale_Getter_PostFix", null), null);

            harmony.Patch(original: AccessTools.Method(
                    type: typeof(Pawn_HealthTracker),
                    name: "PreApplyDamage"),
                    prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(Pawn_PreApplyDamage_PreFix)),
                    postfix: null);
            harmony.Patch(original: AccessTools.Method(
                    type: typeof(MapParent),
                    name: "CheckRemoveMapNow"),
                    prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(MapParent_CheckRemoveMapNow_PreFix)),
                    postfix: null);
            harmony.Patch(original: AccessTools.Method(
                    type: typeof(AttackTargetFinder),
                    name: "CanSee"),
                    prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(AttackTargetFinder_CanSee_PreFix)),
                    postfix: null);
            harmony.Patch(original: AccessTools.Method(
                    type: typeof(AttackTargetFinder),
                    name: "CanReach"),
                    prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(AttackTargetFinder_CanReach_PreFix)),
                    postfix: null);
            harmony.Patch(AccessTools.Method(typeof(CompRottable), "PostIngested", null, null), null,
                new HarmonyMethod(typeof(HarmonyPatches), "CompRottable_PostIngested", null), null);
            harmony.Patch(AccessTools.Method(typeof(Verb_UseAbility), "TryLaunchProjectile", null, null), null,
                new HarmonyMethod(typeof(HarmonyPatches), "Verb_UseAbility_TryLaunchProjectile", null), null);
            harmony.Patch(AccessTools.Method(typeof(PawnAbility), "PostAbilityAttempt", null, null), null,
                new HarmonyMethod(typeof(HarmonyPatches), "Verb_PawnAbility_PostAbilityAttempt_PostFix", null), null);
            harmony.Patch(AccessTools.Method(typeof(ColonistBarColonistDrawer), "DrawIcons", null, null), null,
                new HarmonyMethod(typeof(HarmonyPatches), "ColonistBarColonistDrawer_DrawIcons_PostFix", null), null);
            harmony.Patch((MethodBase)AccessTools.Method(typeof(Map), 
                    "MapUpdate", 
                    (System.Type[])null, 
                    (System.Type[])null), 
                    new HarmonyMethod(typeof(HarmonyPatches), 
                    "GridRegen_Prefix", 
                    (System.Type[])null), 
                    (HarmonyMethod)null, 
                    (HarmonyMethod)null, 
                    (HarmonyMethod)null);
            harmony.Patch((MethodBase)AccessTools.Method(typeof(BuildableDef), 
                    "ForceAllowPlaceOver", 
                    (System.Type[])null, 
                    (System.Type[])null), 
                    (HarmonyMethod)null, 
                    new HarmonyMethod(typeof(HarmonyPatches), 
                    "ForceAllowPlaceOver_Postfix", 
                    (System.Type[])null), 
                    (HarmonyMethod)null, 
                    (HarmonyMethod)null);
            harmony.Patch((MethodBase)AccessTools.Method(typeof(VerbProperties),
                    "GetForceMissFactorFor",
                    (System.Type[])null,
                    (System.Type[])null),
                    new HarmonyMethod(typeof(HarmonyPatches),
                    "GetForceMissFactorFor_Prefix",
                    (System.Type[])null),
                    (HarmonyMethod)null,
                    (HarmonyMethod)null,
                    (HarmonyMethod)null);
            harmony.Patch(original: AccessTools.Method(
                    type: typeof(Need_Rest),
                    name: "NeedInterval"),
                    prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(Need_Rest_NeedInterval_PreFix)),
                    postfix: null);



            //harmony.Patch(original: AccessTools.Method(
            //        type: typeof(Verb),
            //        name: "DrawHighlight"),
            //        prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(TEST_PreFix)),
            //        postfix: null);
        }

        //public static bool TEST_PreFix(Verb __instance, LocalTargetInfo target)
        //{
        //    Log.Error("JEH1-verbprops:" + (__instance.verbProps is null));
        //    Log.Error("JEH1-caster:" + (__instance.caster is null));
        //    __instance.verbProps.DrawRadiusRing(__instance.caster.Position);
        //    Log.Error("JEH2");
        //    if (!target.IsValid)
        //        return false;
        //    Log.Error("JEH3");
        //    GenDraw.DrawTargetHighlight(target);
        //    Log.Error("JEH4");
        //    //__instance.DrawHighlightFieldRadiusAroundTarget(target);

        //    Log.Error("JEH5");
        //    return false;
        //}

        [HarmonyPatch(typeof(ColonistBarColonistDrawer), "DrawIcons", null)]
        public static void ColonistBarColonistDrawer_DrawIcons_PostFix(
              ColonistBarColonistDrawer __instance,
              ref Rect rect,
              Pawn colonist)
        {
            if (colonist.Dead || !Settings.ShowLoreIconInColonistBar)
                return;

            TraitDef abilityTrait = MagicUtility.GetFirstAbilityTrait(colonist);

            if (abilityTrait == null)
                return;

            IconUtilityData icu = MagicUtility.GetIconInfoForTrait(abilityTrait);

            //var traitIconValue = ColonistBarColonistDrawerCache.GetOrCreate(
            //    colonist.ThingID,
            //    () =>
            //    {
            //        if (colonist.health.hediffSet.HasHediff(TorannMagicDefOf.TM_UndeadHD))
            //        {
            //            return new TraitIconMap.TraitIconValue(TM_RenderQueue.necroMarkMat, TM_MatPool.Icon_Undead, "TM_Icon_Undead");
            //        }
            //            // Early exit condition
            //            if (!ModOptions.Settings.Instance.showClassIconOnColonistBar || colonist.story == null)
            //        {
            //            return null;
            //        }
            //            //Custom Classes loaded at startup                        

            //            for (int i = 0; i < colonist.story.traits.allTraits.Count; i++)
            //        {
            //            TraitDef trait = colonist.story.traits.allTraits[i].def;
            //            if (TraitIconMap.ContainsKey(trait))
            //            {
            //                return TraitIconMap.Get(trait);
            //            }
            //        }
            //        return null;
            //    }, 5);

            // Skip rendering if the pawn has no ability trait.
            //if (traitIconValue == null) return;

            // Otherwise render away!
            float num = 20f * Find.ColonistBar.Scale * Settings.Instance.classIconSize;
            Vector2 vector = new Vector2(rect.x + 1f, rect.yMin + 1f);
            rect = new Rect(vector.x, vector.y, num, num);
            GUI.DrawTexture(rect, icu.Texture);
            TooltipHandler.TipRegion(rect, icu.ToolTip);
            vector.x += num;
        }

        public static bool Need_Rest_NeedInterval_PreFix(Need_Rest __instance, ref float ___lastRestEffectiveness, ref Pawn ___pawn)
        {
            // If pawn has ability need, and they are resting, then increase ability need.
            Need_AbilityPool need = ___pawn.needs?.TryGetNeed<Need_AbilityPool>();
            if (need != null && __instance.Resting)//!__instance.IsFrozen)
            {
                float num = ___lastRestEffectiveness * ___pawn.GetStatValue(StatDefOf.RestRateMultiplier, true, -1);
                if ((double)num > 0.0)
                    need.CurLevel += 0.008714286f * num;
            }

            return true;
        }

        public static bool GetForceMissFactorFor_Prefix(VerbProperties __instance, ref float __result, Thing equipment, Pawn caster)
        {
            if (equipment == null)
            {
                __result = 1.0F;
                return false;
            }
            else
                return true;
        }

        public static void GridRegen_Prefix(Map __instance)
        {
            __instance.PipeNet().RegenGrids();
        }

        private static void ForceAllowPlaceOver_Postfix(
          BuildableDef __instance,
          BuildableDef other,
          ref bool __result)
        {
            if (__result || !(other is ThingDef thingDef) || (!(thingDef.entityDefToBuild is ThingDef entityDefToBuild) || !(entityDefToBuild.thingClass == typeof(Building_SteamPipe))))
                return;
            __result = true;
        }

        private static void Verb_UseAbility_TryLaunchProjectile(Verb_UseAbility __instance, ThingDef projectileDef, LocalTargetInfo launchTarget)
        {
            if (__instance.Ability != null && __instance.Ability.Def != null)
            {
                SickAbilityUser.AbilityDef theDef = __instance.Ability.Def;
                if (theDef.defName.Contains("FirePiercingBolts")
                    || theDef.defName.Contains("MawBonecrusher")
                    || theDef.defName.Contains("HeavensAzureBlades")
                    || theDef.defName.Contains("LightBurningGaze")
                    || theDef.defName.Contains("SlaaneshPainPleasure")
                    || theDef.defName.Contains("TzeentchBolt")
                    || theDef.defName.Contains("TzeentchPinkFire")
                    || theDef.defName.Contains("WarpLightning"))
                {
                    __instance.Ability.CooldownTicksLeft = __instance.Ability.MaxCastingTicks;
                }
            }
        }

        private static void Verb_PawnAbility_PostAbilityAttempt_PostFix(PawnAbility __instance)
        {
            if (__instance != null && __instance.Def != null)
            {
                if (__instance.Pawn != null)
                    RH_TET_MagicMod.RemoveActiveCaster(__instance.Pawn);
            }
        }

        private static void CompRottable_PostIngested(CompRottable __instance, Pawn ingester)
        {
            if (__instance.parent != null && __instance.parent.def != null)
            { 
                ThingDef thingDef = __instance.parent.def;

                if (thingDef.defName.Equals("RH_TET_JabberTounge") && !MagicUtility.IsMagicUser(ingester))
                {            // 1/20 chance of random magic trait.
                    int rand = RH_TET_MagicMod.random.Next(0, 20);

                    if (rand == 0)
                    {
                        // Success! Healing increased.
                        List<TraitDef> traits = new List<TraitDef>();
                        traits.Add(RH_TET_MagicDefOf.RH_TET_LoreOfChaosUndividedTrait);
                        traits.Add(RH_TET_MagicDefOf.RH_TET_LoreOfSlaaneshTrait);
                        traits.Add(RH_TET_MagicDefOf.RH_TET_LoreOfNurgleTrait);
                        traits.Add(RH_TET_MagicDefOf.RH_TET_LoreOfTzeentchTrait);
                        traits.Add(RH_TET_MagicDefOf.RH_TET_LoreOfWarpTrait);
                        traits.Add(RH_TET_MagicDefOf.RH_TET_LoreOfPlagueTrait);

                        TraitDef randTrait = traits.RandomElement();

                        Trait trait = new Trait(randTrait, 1);
                        ingester.story.traits.GainTrait(trait);

                        Messages.Message("RH_TET_GainedMagicTrait".Translate(ingester.Name, randTrait.label), (LookTargets)((Thing)ingester), MessageTypeDefOf.PositiveEvent, true);
                    }
                    else
                    {
                        // Fail! 
                        Messages.Message("RH_TET_DidntGainMagicTrait".Translate(ingester.Name), (LookTargets)((Thing)ingester), MessageTypeDefOf.NeutralEvent, true);
                    }

                    // Add chaos corruption.
                    HediffDef chaosCorruption = DefDatabase<HediffDef>.GetNamedSilentFail("RH_TET_ChaosTaintBuildup");
                    int corruptionAmt = 39;
                    if (chaosCorruption == null)
                    {
                        chaosCorruption = HediffDefOf.ToxicBuildup;
                        corruptionAmt = 64;
                    }

                    ingester.health.AddHediff(chaosCorruption);
                    ingester.health.hediffSet.GetFirstHediffOfDef(chaosCorruption).Severity = (float)(RH_TET_MagicMod.random.Next(20, corruptionAmt) * .01f);

                    Messages.Message("RH_TET_ChaosCorruptionToxic".Translate(ingester.Name, chaosCorruption.label), (LookTargets)((Thing)ingester), MessageTypeDefOf.NegativeEvent, true);
                }
                else if (thingDef.defName.Equals("RH_TET_TrollHeart"))
                {
                    // 50/50 chance of healing boost.
                    int rand = RH_TET_MagicMod.random.Next(0, 2);

                    if (rand == 0)
                    {
                        // Success! Healing increased.
                        ingester.health.AddHediff(RH_TET_MagicDefOf.RH_TET_TrollHeartEffect);

                        Messages.Message("RH_TET_GainedTrollBoost".Translate(ingester.Name), (LookTargets)((Thing)ingester), MessageTypeDefOf.PositiveEvent, true);
                    }
                    else
                    {
                        // Fail! 
                        Messages.Message("RH_TET_DidntGainTrollBoost".Translate(ingester.Name), (LookTargets)((Thing)ingester), MessageTypeDefOf.NeutralEvent, true);
                    }
                }
            }
        }

        public static bool AttackTargetFinder_CanReach_PreFix(Thing target, ref bool __result)
        {
            if (target is Pawn)
            {
                Pawn pawn = target as Pawn;
                if (pawn.health.hediffSet.HasHediff(RH_TET_MagicDefOf.RH_TET_ShadowsCompanion_HE, false))
                {
                    __result = false;
                    return false;
                }
            }
            return true;
        }

        public static bool AttackTargetFinder_CanSee_PreFix(Thing target, ref bool __result)
        {
            if (target is Pawn)
            {
                Pawn pawn = target as Pawn;
                if (pawn.health.hediffSet.HasHediff(RH_TET_MagicDefOf.RH_TET_ShadowsCompanion_HE, false))
                {
                    __result = false;
                    return false;
                }
            }
            return true;
        }

        public static float GetBaseValueFor(StatRequest request, StatDef stat)
        {
            float defaultBaseValue = stat.defaultBaseValue;
            if (request.StatBases != null)
            {
                for (int index = 0; index < request.StatBases.Count; ++index)
                {
                    if (request.StatBases[index].stat == stat)
                    {
                        defaultBaseValue = request.StatBases[index].value;
                        break;
                    }
                }
            }
            return defaultBaseValue;
        }

        public static bool MapParent_CheckRemoveMapNow_PreFix()
        {
            // If a living player pawn is flying or not properly spawned for some other similar reason,
            // don't let the call to the vanilla function happen.
            // TODO - Something is not working here. 20220428
            return RH_TET_MagicMod.GetSafeToRemoveMapNow();
        }

        private static void Pawn_HealthScale_Getter_PostFix(ref float __result, Pawn __instance)
        {
            if (__instance != null && __instance.apparel != null && __instance.apparel.WornApparel != null && __instance.apparel.WornApparelCount > 0)
            {
                List<Apparel> apparel = __instance.apparel.WornApparel;
                for (int index = 0; index < apparel.Count; ++index)
                {
                    if (apparel[index] != null && apparel[index].def.defName.Equals("RH_TET_Magic_HelmetRegeneration"))
                    {
                        __result *= 10;
                        return;
                    }
                }
            }
        }

        // TODO: Cleanup
        // I believe it's safe to remove this one. It was used when these weapons were light sources also, which really 
        // slagged the game down dramatically (and was therefore removed). 
        //harmony.Patch(AccessTools.Method(typeof(Pawn_EquipmentTracker), "EquipmentTrackerTick", null, null), null,
        //        new HarmonyMethod(typeof(HarmonyPatches), "Pawn_EquipmentTracker_EquipmentTrackerTick_PostFix", null), null);
        //private static void Pawn_EquipmentTracker_EquipmentTrackerTick_PostFix(Pawn_EquipmentTracker __instance)
        //{
        //    List<ThingWithComps> equipmentListForReading = __instance.AllEquipmentListForReading;
        //    for (int index = 0; index < equipmentListForReading.Count; ++index)
        //    {
        //        if (equipmentListForReading[index].def.defName.Equals("RH_TET_MeleeWeapon_KhorneAxe")
        //            || equipmentListForReading[index].def.defName.Equals("RH_TET_MeleeWeapon_NurgleStar")
        //            || equipmentListForReading[index].def.defName.Equals("RH_TET_MeleeWeapon_SlaaneshWhip")
        //            || equipmentListForReading[index].def.defName.Equals("RH_TET_MeleeWeapon_TzeentchStaff")
        //            || equipmentListForReading[index].def.defName.Equals("RH_TET_MeleeWeapon_SigmarHammer")
        //            || equipmentListForReading[index].def.defName.Equals("RH_TET_MeleeWeapon_UlricHammer"))
        //        {
        //            equipmentListForReading[index].Tick();
        //        }
        //    }
        //}

        private static bool Pawn_PreApplyDamage_PreFix(Pawn __instance, ref DamageInfo dinfo, out bool absorbed)
        {
            Traverse.Create((object)__instance);
            Pawn pawn = (Pawn)typeof(Pawn_HealthTracker).GetField(nameof(pawn), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField).GetValue((object)__instance);

            absorbed = false;

            if (pawn.health.hediffSet.HasHediff(RH_TET_MagicDefOf.RH_TET_FireCloak, false)
                    || pawn.health.hediffSet.HasHediff(RH_TET_MagicDefOf.RH_TET_MagicShield, false)
                    || pawn.health.hediffSet.HasHediff(RH_TET_MagicDefOf.RH_TET_GlitteringRobe, false)
                    || pawn.health.hediffSet.HasHediff(RH_TET_MagicDefOf.RH_TET_ArmorOfDarkness, false))
            {
                Thing instigator = dinfo.Instigator;

                if (instigator != null && dinfo.Weapon != null && dinfo.Weapon.IsRangedWeapon)
                {
                    HediffDef hediffDef = null;
                    Hediff hediff = null;

                    if (pawn.health.hediffSet.HasHediff(RH_TET_MagicDefOf.RH_TET_FireCloak, false))
                    {
                        hediffDef = RH_TET_MagicDefOf.RH_TET_FireCloak;
                        hediff = pawn.health.hediffSet.hediffs.FirstOrDefault<Hediff>((Func<Hediff, bool>)(h => h.def == hediffDef));

                        HediffComp_FireCloak hediffComp = hediff.TryGetComp<HediffComp_FireCloak>();
                        absorbed = hediffComp.CheckPreAbsorbDamage(dinfo);
                        if (absorbed)
                            dinfo.SetAmount(0.0f);
                    }
                    if (dinfo.Amount > 0.0f && pawn.health.hediffSet.HasHediff(RH_TET_MagicDefOf.RH_TET_MagicShield, false))
                    {
                        hediffDef = RH_TET_MagicDefOf.RH_TET_MagicShield;
                        hediff = pawn.health.hediffSet.hediffs.FirstOrDefault<Hediff>((Func<Hediff, bool>)(h => h.def == hediffDef));

                        HediffComp_MagicShield hediffComp = hediff.TryGetComp<HediffComp_MagicShield>();
                        absorbed = hediffComp.CheckPreAbsorbDamage(dinfo);
                        if (absorbed)
                            dinfo.SetAmount(0.0f);
                    }
                    if (dinfo.Amount > 0.0f && pawn.health.hediffSet.HasHediff(RH_TET_MagicDefOf.RH_TET_GlitteringRobe, false))
                    {
                        hediffDef = RH_TET_MagicDefOf.RH_TET_GlitteringRobe;
                        hediff = pawn.health.hediffSet.hediffs.FirstOrDefault<Hediff>((Func<Hediff, bool>)(h => h.def == hediffDef));

                        HediffComp_GlitteringRobe hediffComp = hediff.TryGetComp<HediffComp_GlitteringRobe>();
                        absorbed = hediffComp.CheckPreAbsorbDamage(dinfo);
                        if (absorbed)
                            dinfo.SetAmount(0.0f);
                    }
                    if (dinfo.Amount > 0.0f && pawn.health.hediffSet.HasHediff(RH_TET_MagicDefOf.RH_TET_ArmorOfDarkness, false))
                    {
                        hediffDef = RH_TET_MagicDefOf.RH_TET_ArmorOfDarkness;
                        hediff = pawn.health.hediffSet.hediffs.FirstOrDefault<Hediff>((Func<Hediff, bool>)(h => h.def == hediffDef));

                        HediffComp_ArmorOfDarkness hediffComp = hediff.TryGetComp<HediffComp_ArmorOfDarkness>();
                        absorbed = hediffComp.CheckPreAbsorbDamage(dinfo);
                        if (absorbed)
                            dinfo.SetAmount(0.0f);
                    }

                    if (absorbed)
                    {
                        FleckMaker.ThrowLightningGlow(pawn.Position.ToVector3Shifted(), pawn.Map, 3f);
                        return false;
                    }
                }
            }
            else if (pawn.health.hediffSet.HasHediff(RH_TET_MagicDefOf.RH_TET_SigmarShield, false)
                    || pawn.health.hediffSet.HasHediff(RH_TET_MagicDefOf.RH_TET_ShieldSaphery, false))
            {
                Thing instigator = dinfo.Instigator;

                if (instigator != null)
                {
                    HediffDef hediffDef = null;
                    Hediff hediff = null;
                    if (dinfo.Amount > 0.0f && pawn.health.hediffSet.HasHediff(RH_TET_MagicDefOf.RH_TET_SigmarShield, false))
                    {
                        hediffDef = RH_TET_MagicDefOf.RH_TET_SigmarShield;
                        hediff = pawn.health.hediffSet.hediffs.FirstOrDefault<Hediff>((Func<Hediff, bool>)(h => h.def == hediffDef));

                        HediffComp_SigmarShield hediffComp = hediff.TryGetComp<HediffComp_SigmarShield>();
                        absorbed = hediffComp.CheckPreAbsorbDamage(dinfo);
                        if (absorbed)
                            dinfo.SetAmount(0.0f);
                    }
                    else if (dinfo.Amount > 0.0f && pawn.health.hediffSet.HasHediff(RH_TET_MagicDefOf.RH_TET_ShieldSaphery, false))
                    {
                        hediffDef = RH_TET_MagicDefOf.RH_TET_ShieldSaphery;
                        hediff = pawn.health.hediffSet.hediffs.FirstOrDefault<Hediff>((Func<Hediff, bool>)(h => h.def == hediffDef));

                        HediffComp_ShieldOfSaphery hediffComp = hediff.TryGetComp<HediffComp_ShieldOfSaphery>();
                        absorbed = hediffComp.CheckPreAbsorbDamage(dinfo);
                        if (absorbed)
                            dinfo.SetAmount(0.0f);
                    }

                    if (absorbed)
                    {
                        FleckMaker.ThrowLightningGlow(pawn.Position.ToVector3Shifted(), pawn.Map, 3f);
                        FleckMaker.ThrowMicroSparks(pawn.Position.ToVector3Shifted(), pawn.Map);
                        return false;
                    }
                }
            }
            return true;
        }

        public static void Pawn_ApparelTracker_GetGizmos_PostFix(Pawn_ApparelTracker __instance, ref IEnumerable<Gizmo> __result)
        {
            Pawn pawn = __instance.pawn;

            if (pawn.health.hediffSet.HasHediff(RH_TET_MagicDefOf.RH_TET_FireCloak, false)
                     || pawn.health.hediffSet.HasHediff(RH_TET_MagicDefOf.RH_TET_MagicShield, false)
                     || pawn.health.hediffSet.HasHediff(RH_TET_MagicDefOf.RH_TET_GlitteringRobe, false)
                     || pawn.health.hediffSet.HasHediff(RH_TET_MagicDefOf.RH_TET_SigmarShield, false)
                     || pawn.health.hediffSet.HasHediff(RH_TET_MagicDefOf.RH_TET_ShieldSaphery, false)
                     || pawn.health.hediffSet.HasHediff(RH_TET_MagicDefOf.RH_TET_ArmorOfDarkness, false))
            {
                HediffDef hediffDef = null;
                Hediff hediff = null;

                if (pawn.health.hediffSet.HasHediff(RH_TET_MagicDefOf.RH_TET_FireCloak, false))
                {
                    hediffDef = RH_TET_MagicDefOf.RH_TET_FireCloak;
                    hediff = pawn.health.hediffSet.hediffs.FirstOrDefault<Hediff>((Func<Hediff, bool>)(h => h.def == hediffDef));

                    HediffComp_FireCloak hediffComp = hediff.TryGetComp<HediffComp_FireCloak>();

                    __result = __result.Concat(hediffComp.GetWornGizmos());
                }
                if (pawn.health.hediffSet.HasHediff(RH_TET_MagicDefOf.RH_TET_MagicShield, false))
                {
                    hediffDef = RH_TET_MagicDefOf.RH_TET_MagicShield;
                    hediff = pawn.health.hediffSet.hediffs.FirstOrDefault<Hediff>((Func<Hediff, bool>)(h => h.def == hediffDef));

                    HediffComp_MagicShield hediffComp = hediff.TryGetComp<HediffComp_MagicShield>();

                    __result = __result.Concat(hediffComp.GetWornGizmos());
                }
                if (pawn.health.hediffSet.HasHediff(RH_TET_MagicDefOf.RH_TET_GlitteringRobe, false))
                {
                    hediffDef = RH_TET_MagicDefOf.RH_TET_GlitteringRobe;
                    hediff = pawn.health.hediffSet.hediffs.FirstOrDefault<Hediff>((Func<Hediff, bool>)(h => h.def == hediffDef));

                    HediffComp_GlitteringRobe hediffComp = hediff.TryGetComp<HediffComp_GlitteringRobe>();

                    __result = __result.Concat(hediffComp.GetWornGizmos());
                }
                if (pawn.health.hediffSet.HasHediff(RH_TET_MagicDefOf.RH_TET_SigmarShield, false))
                {
                    hediffDef = RH_TET_MagicDefOf.RH_TET_SigmarShield;
                    hediff = pawn.health.hediffSet.hediffs.FirstOrDefault<Hediff>((Func<Hediff, bool>)(h => h.def == hediffDef));

                    HediffComp_SigmarShield hediffComp = hediff.TryGetComp<HediffComp_SigmarShield>();

                    __result = __result.Concat(hediffComp.GetWornGizmos());
                }
                if (pawn.health.hediffSet.HasHediff(RH_TET_MagicDefOf.RH_TET_ShieldSaphery, false))
                {
                    hediffDef = RH_TET_MagicDefOf.RH_TET_ShieldSaphery;
                    hediff = pawn.health.hediffSet.hediffs.FirstOrDefault<Hediff>((Func<Hediff, bool>)(h => h.def == hediffDef));

                    HediffComp_ShieldOfSaphery hediffComp = hediff.TryGetComp<HediffComp_ShieldOfSaphery>();

                    __result = __result.Concat(hediffComp.GetWornGizmos());
                }
                if (pawn.health.hediffSet.HasHediff(RH_TET_MagicDefOf.RH_TET_ArmorOfDarkness, false))
                {
                    hediffDef = RH_TET_MagicDefOf.RH_TET_ArmorOfDarkness;
                    hediff = pawn.health.hediffSet.hediffs.FirstOrDefault<Hediff>((Func<Hediff, bool>)(h => h.def == hediffDef));

                    HediffComp_ArmorOfDarkness hediffComp = hediff.TryGetComp<HediffComp_ArmorOfDarkness>();

                    __result = __result.Concat(hediffComp.GetWornGizmos());
                }
            }
        }

        public static void DrawEquipment_PostFix(Pawn pawn, Vector3 drawPos, Rot4 facing, PawnRenderFlags flags)
        {
            if (pawn.health?.hediffSet?.hediffs == null || pawn.health.hediffSet.hediffs.Count <= 0)
                return;
            Hediff hd = pawn.health.hediffSet.hediffs.FirstOrDefault<Hediff>((Func<Hediff, bool>)(x => x.TryGetComp<HediffComp_MagicShield>() != null));
            (hd != null ? hd.TryGetComp<HediffComp_MagicShield>() : (HediffComp_MagicShield)null)?.DrawWornExtras();
            Hediff hd2 = pawn.health.hediffSet.hediffs.FirstOrDefault<Hediff>((Func<Hediff, bool>)(x => x.TryGetComp<HediffComp_FireCloak>() != null));
            (hd2 != null ? hd2.TryGetComp<HediffComp_FireCloak>() : (HediffComp_FireCloak)null)?.DrawWornExtras();
            Hediff hd3 = pawn.health.hediffSet.hediffs.FirstOrDefault<Hediff>((Func<Hediff, bool>)(x => x.TryGetComp<HediffComp_GlitteringRobe>() != null));
            (hd3 != null ? hd3.TryGetComp<HediffComp_GlitteringRobe>() : (HediffComp_GlitteringRobe)null)?.DrawWornExtras();
            Hediff hd4 = pawn.health.hediffSet.hediffs.FirstOrDefault<Hediff>((Func<Hediff, bool>)(x => x.TryGetComp<HediffComp_SigmarShield>() != null));
            (hd4 != null ? hd4.TryGetComp<HediffComp_SigmarShield>() : (HediffComp_SigmarShield)null)?.DrawWornExtras();
            Hediff hd5 = pawn.health.hediffSet.hediffs.FirstOrDefault<Hediff>((Func<Hediff, bool>)(x => x.TryGetComp<HediffComp_ShieldOfSaphery>() != null));
            (hd5 != null ? hd5.TryGetComp<HediffComp_ShieldOfSaphery>() : (HediffComp_ShieldOfSaphery)null)?.DrawWornExtras();
            Hediff hd6 = pawn.health.hediffSet.hediffs.FirstOrDefault<Hediff>((Func<Hediff, bool>)(x => x.TryGetComp<HediffComp_ArmorOfDarkness>() != null));
            (hd6 != null ? hd6.TryGetComp<HediffComp_ArmorOfDarkness>() : (HediffComp_ArmorOfDarkness)null)?.DrawWornExtras();
        }

        public static void Learn_PostFix(SkillRecord __instance, float xp, bool direct = false, bool ignoreLearnRate = false)
        {
            Pawn pawn = (Pawn)AccessTools.Field(typeof(SkillRecord), "pawn").GetValue((object)__instance);
            if ((double)xp <= 0.0)
                return;
            CompMagicUser compMagicUser = ((pawn != null) ? pawn.TryGetComp<CompMagicUser>() : null);

            if (compMagicUser == null || !compMagicUser.IsMagicUser)
                return;
            
            int ticksGame = Find.TickManager.TicksGame;
            int? ticksUntilXpGain = compMagicUser?.MagicData?.TicksUntilXPGain;
            int valueOrDefault = ticksUntilXpGain.GetValueOrDefault();

            if (!(ticksGame > valueOrDefault & ticksUntilXpGain.HasValue))
                return;

            int num = 250;
            if (__instance.def == SkillDefOf.Intellectual)
                num += 50;

            compMagicUser.MagicData.TicksUntilXPGain = Find.TickManager.TicksGame + num;
            ++compMagicUser.MagicUserXP;
        }
    }
}