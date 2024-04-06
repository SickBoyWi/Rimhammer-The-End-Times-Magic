using SickAbilityUser;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    public class MagicCardUtility
    {
        public static Vector2 magicCardSize = new Vector2(450f, 500f);
        public static float ButtonSize = 40f;
        public static float MagicButtonSize = 46f;
        public static float MagicButtonPointSize = 24f;
        public static float HeaderSize = 32f;
        public static float TextSize = 22f;
        public static float Padding = 3f;
        public static float SpacingOffset = 12f;
        public static float SectionOffset = 8f;
        public static float ColumnSize = 245f;
        public static float SkillsColumnHeight = 113f;
        public static float SkillsColumnDivider = 114f;
        public static float SkillsTextWidth = 138f;
        public static float SkillsBoxSize = 18f;
        public static float PowersColumnHeight = 195f;
        public static float PowersColumnWidth = 123f;
        public static bool adjustedForLanguage = false;

        public static Vector2 MagicCardSize
        {
            get
            {
                return MagicCardUtility.magicCardSize;
            }
        }

        public static void AdjustForLanguage()
        {
            if (MagicCardUtility.adjustedForLanguage)
                return;
            MagicCardUtility.adjustedForLanguage = true;
            if (LanguageDatabase.activeLanguage != LanguageDatabase.AllLoadedLanguages.FirstOrDefault<LoadedLanguage>((Func<LoadedLanguage, bool>)(x => x.folderName == "German")))
                return;
            MagicCardUtility.SkillsColumnDivider = 170f;
            MagicCardUtility.SkillsTextWidth = 170f;
        }

        public static void DrawMagicCard(Rect rect, Pawn pawn)
        {
            MagicCardUtility.AdjustForLanguage();
            GUI.BeginGroup(rect);
            CompMagicUser comp1 = pawn.GetComp<CompMagicUser>();

            if (comp1 != null)
            {
                if (comp1.MagicUserLevel > 0)
                {
                    Trait magicTrait = FindMagicTrait(comp1);

                    float x1 = (float)Text.CalcSize(magicTrait.Label).x;
                    Rect rect1 = new Rect(3.0f, rect.y, rect.width, MagicCardUtility.HeaderSize);
                    Text.Font = GameFont.Medium;
                    Widgets.Label(rect1, magicTrait.Label.CapitalizeFirst());
                    Text.Font = GameFont.Small;
                    Widgets.DrawLineHorizontal(rect.x - 10f, rect1.yMax, rect.width - 15f);
                    
                    Rect inRect1 = new Rect(3.0f, rect1.yMax + MagicCardUtility.Padding, MagicCardUtility.SkillsColumnDivider * 2, MagicCardUtility.SkillsColumnHeight);
                    Rect rectX = new Rect(inRect1.x, inRect1.y, inRect1.width * 0.7f, MagicCardUtility.TextSize);
                    Text.Font = GameFont.Small;
                    Widgets.Label(rectX, "RH_TET_Level".Translate().CapitalizeFirst() + " " + comp1.MagicUserLevel.ToString());

                    Text.Font = GameFont.Small;
                    if (DebugSettings.godMode)
                    {
                        Rect rect2 = new Rect(rectX.x + (MagicCardUtility.Padding * 15), rectX.y, rectX.width * 2f, MagicCardUtility.TextSize);
                        if (Widgets.ButtonText(rect2, "+ magic lvl", true, false, true))
                            comp1.LevelUp(true);
                        if (comp1.MagicUserLevel > 0 && Widgets.ButtonText(new Rect(rect2.x, rect2.yMax + 1f, rect2.width, MagicCardUtility.TextSize), "reset magic", true, false, true))
                            comp1.ResetPowers();
                        if (comp1.MagicUserLevel > 0 && Widgets.ButtonText(new Rect(rect2.x, rect2.yMax + MagicCardUtility.TextSize + 1f, rect2.width, MagicCardUtility.TextSize), "+ XP", true, false, true))
                            comp1.IncreaseExperience(50);
                    }
                    Rect rect3 = new Rect(3.0f, rectX.yMax + MagicCardUtility.SectionOffset, rect.width, MagicCardUtility.TextSize);
                    Text.Font = GameFont.Tiny;
                    Widgets.Label(rect3, comp1.MagicData.AbilityPoints.ToString() + " " + "RH_TET_PointsAvail".Translate());
                    Text.Font = GameFont.Small;
                    MagicCardUtility.DrawLevelBar(new Rect(rect3.x, rect3.yMax, rect.width - 10f, MagicCardUtility.HeaderSize * 0.6f), comp1, false);
                    
                    Rect rect9 = new Rect(3.0f, rect3.yMax + (MagicCardUtility.SectionOffset * 4f), rect.width, MagicCardUtility.HeaderSize);
                    Text.Font = GameFont.Medium;
                    Widgets.Label(rect9, "RH_TET_Spells".Translate().CapitalizeFirst());
                    Text.Font = GameFont.Small;
                    Widgets.DrawLineHorizontal(rect.x - 10f, rect9.yMax, ((rect.width)/2) - 20f);
                    Rect inRect4 = new Rect(13.0f, rect9.yMax + MagicCardUtility.SectionOffset, MagicCardUtility.PowersColumnWidth, MagicCardUtility.PowersColumnHeight);
                    Rect inRect5 = new Rect((rect.width / 2) + 4f, rect9.yMax + MagicCardUtility.SectionOffset, MagicCardUtility.PowersColumnWidth, MagicCardUtility.PowersColumnHeight);

                    // Determine Lore.
                    if (MagicUtility.GetMagicLoreType(pawn).Equals(MagicLoreType.Beasts))
                        MagicCardUtility.PowersGUIHandler(inRect4, pawn.GetComp<CompMagicUser>(), pawn.GetComp<CompMagicUser>().MagicData.PowersBeast, TexButton.RH_TET_MagicPoint);
                    else if (MagicUtility.GetMagicLoreType(pawn).Equals(MagicLoreType.Tzeentch))
                        MagicCardUtility.PowersGUIHandler(inRect4, pawn.GetComp<CompMagicUser>(), pawn.GetComp<CompMagicUser>().MagicData.PowersTzeentch, TexButton.RH_TET_MagicPoint);
                    else if (MagicUtility.GetMagicLoreType(pawn).Equals(MagicLoreType.Death))
                        MagicCardUtility.PowersGUIHandler(inRect4, pawn.GetComp<CompMagicUser>(), pawn.GetComp<CompMagicUser>().MagicData.PowersDeath, TexButton.RH_TET_MagicPoint);
                    else if (MagicUtility.GetMagicLoreType(pawn).Equals(MagicLoreType.Shadow))
                        MagicCardUtility.PowersGUIHandler(inRect4, pawn.GetComp<CompMagicUser>(), pawn.GetComp<CompMagicUser>().MagicData.PowersShadow, TexButton.RH_TET_MagicPoint);
                    else if (MagicUtility.GetMagicLoreType(pawn).Equals(MagicLoreType.Wild))
                        MagicCardUtility.PowersGUIHandler(inRect4, pawn.GetComp<CompMagicUser>(), pawn.GetComp<CompMagicUser>().MagicData.PowersWild, TexButton.RH_TET_MagicPoint);
                    else if (MagicUtility.GetMagicLoreType(pawn).Equals(MagicLoreType.Fire))
                        MagicCardUtility.PowersGUIHandler(inRect4, pawn.GetComp<CompMagicUser>(), pawn.GetComp<CompMagicUser>().MagicData.PowersFire, TexButton.RH_TET_MagicPoint);
                    else if (MagicUtility.GetMagicLoreType(pawn).Equals(MagicLoreType.Heavens))
                        MagicCardUtility.PowersGUIHandler(inRect4, pawn.GetComp<CompMagicUser>(), pawn.GetComp<CompMagicUser>().MagicData.PowersHeavens, TexButton.RH_TET_MagicPoint);
                    else if (MagicUtility.GetMagicLoreType(pawn).Equals(MagicLoreType.Metal))
                        MagicCardUtility.PowersGUIHandler(inRect4, pawn.GetComp<CompMagicUser>(), pawn.GetComp<CompMagicUser>().MagicData.PowersMetal, TexButton.RH_TET_MagicPoint);
                    else if (MagicUtility.GetMagicLoreType(pawn).Equals(MagicLoreType.Light))
                        MagicCardUtility.PowersGUIHandler(inRect4, pawn.GetComp<CompMagicUser>(), pawn.GetComp<CompMagicUser>().MagicData.PowersLight, TexButton.RH_TET_MagicPoint);
                    else if (MagicUtility.GetMagicLoreType(pawn).Equals(MagicLoreType.Life))
                        MagicCardUtility.PowersGUIHandler(inRect4, pawn.GetComp<CompMagicUser>(), pawn.GetComp<CompMagicUser>().MagicData.PowersLife, TexButton.RH_TET_MagicPoint);
                    else if (MagicUtility.GetMagicLoreType(pawn).Equals(MagicLoreType.Nurgle))
                        MagicCardUtility.PowersGUIHandler(inRect4, pawn.GetComp<CompMagicUser>(), pawn.GetComp<CompMagicUser>().MagicData.PowersNurgle, TexButton.RH_TET_MagicPoint);
                    else if (MagicUtility.GetMagicLoreType(pawn).Equals(MagicLoreType.ChaosUndivided))
                        MagicCardUtility.PowersGUIHandler(inRect4, pawn.GetComp<CompMagicUser>(), pawn.GetComp<CompMagicUser>().MagicData.PowersChaos, TexButton.RH_TET_MagicPoint);
                    else if (MagicUtility.GetMagicLoreType(pawn).Equals(MagicLoreType.Slaanesh))
                        MagicCardUtility.PowersGUIHandler(inRect4, pawn.GetComp<CompMagicUser>(), pawn.GetComp<CompMagicUser>().MagicData.PowersSlaanesh, TexButton.RH_TET_MagicPoint);
                    else if (MagicUtility.GetMagicLoreType(pawn).Equals(MagicLoreType.High))
                        MagicCardUtility.PowersGUIHandler(inRect4, pawn.GetComp<CompMagicUser>(), pawn.GetComp<CompMagicUser>().MagicData.PowersHigh, TexButton.RH_TET_MagicPoint);
                    else if (MagicUtility.GetMagicLoreType(pawn).Equals(MagicLoreType.Plague))
                        MagicCardUtility.PowersGUIHandler(inRect4, pawn.GetComp<CompMagicUser>(), pawn.GetComp<CompMagicUser>().MagicData.PowersPlague, TexButton.RH_TET_MagicPoint);
                    else if (MagicUtility.GetMagicLoreType(pawn).Equals(MagicLoreType.GreatMaw))
                        MagicCardUtility.PowersGUIHandler(inRect4, pawn.GetComp<CompMagicUser>(), pawn.GetComp<CompMagicUser>().MagicData.PowersGreatMaw, TexButton.RH_TET_MagicPoint);
                    else if (MagicUtility.GetMagicLoreType(pawn).Equals(MagicLoreType.Warp))
                        MagicCardUtility.PowersGUIHandler(inRect4, pawn.GetComp<CompMagicUser>(), pawn.GetComp<CompMagicUser>().MagicData.PowersWarp, TexButton.RH_TET_MagicPoint);
                    else if (MagicUtility.GetMagicLoreType(pawn).Equals(MagicLoreType.Stealth))
                        MagicCardUtility.PowersGUIHandler(inRect4, pawn.GetComp<CompMagicUser>(), pawn.GetComp<CompMagicUser>().MagicData.PowersStealth, TexButton.RH_TET_MagicPoint);
                    else
                        Log.Error("No valid lore found for wizard. WTF?");

                    MagicCardUtility.PowersGUIHandler(inRect5, pawn.GetComp<CompMagicUser>(), pawn.GetComp<CompMagicUser>().MagicData.PowersAddons, TexButton.RH_TET_MagicPoint, true, rect9);
                }
                else
                {
                    MagicCardUtility.InfoPaneSensitive(new Rect(rect.x, rect.y, rect.width, rect.height), pawn.GetComp<CompMagicUser>());
                }
            }
            GUI.EndGroup();
        }

        private static Trait FindMagicTrait(CompMagicUser comp1)
        {
            Trait magicTrait = null;

            TraitSet compTraits = comp1.AbilityUser.story.traits;
            foreach (Trait trait in compTraits.allTraits)
            {
                if (trait.def.defName.Equals("RH_TET_LoreOfTzeentchTrait")
                        || trait.def.defName.Equals("RH_TET_LoreOfWildTrait")
                        || trait.def.defName.Equals("RH_TET_LoreOfShadowTrait")
                        || trait.def.defName.Equals("RH_TET_LoreOfDeathTrait")
                        || trait.def.defName.Equals("RH_TET_LoreOfBeastsTrait")
                        || trait.def.defName.Equals("RH_TET_LoreOfFireTrait")
                        || trait.def.defName.Equals("RH_TET_LoreOfHeavensTrait")
                        || trait.def.defName.Equals("RH_TET_LoreOfMetalTrait")
                        || trait.def.defName.Equals("RH_TET_LoreOfLightTrait")
                        || trait.def.defName.Equals("RH_TET_LoreOfLifeTrait")
                        || trait.def.defName.Equals("RH_TET_LoreOfNurgleTrait")
                        || trait.def.defName.Equals("RH_TET_LoreOfChaosUndividedTrait")
                        || trait.def.defName.Equals("RH_TET_LoreOfSlaaneshTrait")
                        || trait.def.defName.Equals("RH_TET_LoreOfHighTrait")
                        || trait.def.defName.Equals("RH_TET_LoreOfPlagueTrait")
                        || trait.def.defName.Equals("RH_TET_LoreOfGreatMawTrait")
                        || trait.def.defName.Equals("RH_TET_LoreOfWarpTrait")
                        || trait.def.defName.Equals("RH_TET_LoreOfStealthTrait"))
                {
                    magicTrait = trait;
                    break;
                }
            }

            return magicTrait;
        }

        public static string MagicXPTipString(CompMagicUser compMagic, bool sensitive)
        {
            if (!sensitive)
                return compMagic.MagicUserXP.ToString() + " / " + compMagic.MagicUserXPTillNextLevel.ToString() + "\n" + "RH_TET_MagicXPDesc".Translate();
            return "RH_TET_MagicSensitiveDesc".Translate((object)compMagic.AbilityUser.LabelShort);
        }

        public static void InfoPaneSensitive(Rect inRect, CompMagicUser compMagic)
        {
            Rect rect1 = new Rect(inRect.x, inRect.y, inRect.width * 0.7f, MagicCardUtility.TextSize);
            Text.Font = GameFont.Small;
            Widgets.Label(rect1, "RH_TET_MagicSensitiveMessage".Translate((object)compMagic.AbilityUser.LabelShort));
            Text.Font = GameFont.Small;
            if (DebugSettings.godMode && Widgets.ButtonText(new Rect(rect1.xMax, inRect.y, inRect.width * 0.3f, MagicCardUtility.TextSize), "+", true, false, true))
                compMagic.LevelUp(true);
            Rect rect2 = new Rect(inRect.x, rect1.yMax, inRect.width, MagicCardUtility.TextSize);
            MagicCardUtility.DrawLevelBar(new Rect(rect2.x, rect2.yMax + 3f, inRect.width - 10f, MagicCardUtility.HeaderSize * 0.6f), compMagic, true);
        }

        public static void DrawLevelBar(Rect rect, CompMagicUser compMagic, bool sensitive = false)
        {
            if ((double)rect.height > 70.0)
            {
                float num = (float)(((double)rect.height - 70.0) / 2.0);
                rect.height = 70f;
                ref Rect local = ref rect;
                local.y = local.y + num;
            }
            float num1 = 14f;
            if ((double)rect.height < 50.0)
                num1 *= Mathf.InverseLerp(0.0f, 50f, rect.height);
            Text.Anchor = (TextAnchor)0;
            Rect rect1 = new Rect(rect.x, rect.y + rect.height / 2f, rect.width, rect.height);
            rect1 = new Rect(rect1.x, rect1.y, rect1.width, rect1.height - num1);
            if (Mouse.IsOver(rect1))
                Widgets.DrawHighlight(rect);
            TooltipHandler.TipRegion(rect1, new TipSignal((Func<string>)(() => MagicCardUtility.MagicXPTipString(compMagic, sensitive)), rect.GetHashCode()));
            Widgets.FillableBar(rect1, compMagic.XPTillNextLevelPercent, (Texture2D)AccessTools.Field(typeof(Widgets), "BarFullTexHor").GetValue((object)null), BaseContent.GreyTex, false);
        }

        public static void PowersGUIHandler(
          Rect inRect,
          CompMagicUser compMagic,
          List<MagicPower> magicPowers,
          Texture2D pointTexture,
          bool addOn = false,
          Rect rectComp = new Rect())
        {
            if (magicPowers == null || magicPowers.Count <= 0)
                return;

            float y = inRect.y;

            if (addOn)
            {
                Rect rect9 = new Rect(inRect.x - 10f, rectComp.y, rectComp.width, MagicCardUtility.HeaderSize);
                Text.Font = GameFont.Medium;
                Widgets.Label(rect9, "RH_TET_AddOnSpells".Translate().CapitalizeFirst());
                Text.Font = GameFont.Small;
                Widgets.DrawLineHorizontal(inRect.x - 10f, rectComp.yMax, ((rectComp.width) / 2) - 20f);
            }

            foreach (MagicPower magicPower in magicPowers)
            {
                MagicPower power = magicPower;
                Rect rect1 = new Rect(inRect.x - 10f, y, MagicCardUtility.MagicButtonSize, MagicCardUtility.MagicButtonSize);
                TooltipHandler.TipRegion(rect1, (Func<string>)(() => ((Def)power.abilityDef).label + "\n\n" + ((Def)power.abilityDef).description + "\n\n" + "RH_TET_CheckBoxesForMoreInfo".Translate()), 398462);
            
                // Ability Image.
                // Makes sure that once they're max level, they won't appear to be upgradable. 
                if (compMagic.MagicData.AbilityPoints == 0 || power.level >= 4)
                {
                    Widgets.DrawTextureFitted(rect1, (Texture)power.Icon, 1f);
                }
                else if (Widgets.ButtonImage(rect1, power.Icon) && compMagic.AbilityUser.Faction == Faction.OfPlayer)
                {
                    MagicAbilityDef nextLevelAbilityDef = power.nextLevelAbilityDef as MagicAbilityDef;
                
                    if (compMagic.MagicData.AbilityPoints < nextLevelAbilityDef.nextMagicLevelPointsRequired)
                    {
                        Messages.Message("RH_TET_MagicPointsRequired".Translate((object)nextLevelAbilityDef.nextMagicLevelPointsRequired), MessageTypeDefOf.RejectInput, true);
                        break;
                    }
                    compMagic.LevelUpPower(power);
                    compMagic.MagicData.AbilityPoints -= nextLevelAbilityDef.abilityPoints;
                }

                //Actual level pawn has images, active or no.
                int saveIndex = -1;
                bool cooldownActive = false;
                for (int index = 0; index < 4; ++index)
                {
                    float num1 = MagicCardUtility.MagicButtonSize + 1f;
                    if (index != 0)
                        num1 += MagicCardUtility.MagicButtonPointSize * (float)index;
                    float num2 = y + MagicCardUtility.MagicButtonSize / 3f;
                    Rect rect2 = new Rect(inRect.x + num1, num2, MagicCardUtility.MagicButtonPointSize, MagicCardUtility.MagicButtonPointSize);

                    if (power.level > index)
                    { 
                        Widgets.DrawTextureFitted(rect2, (Texture)pointTexture, 1f);
                        
                        if (!cooldownActive)
                        {
                            foreach (PawnAbility pa in compMagic.AbilityData.AllPowers)
                            {
                                if (pa.Def.defName.Equals(power.abilityDef.defName))
                                {
                                    if (pa.CooldownTicksLeft > 1)
                                    {
                                        cooldownActive = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                        Widgets.DrawTextureFitted(rect2, (Texture)TexButton.RH_TET_MagicPointOff, 1f);

                    MagicAbilityDef powerDef = power.GetAbilityDef(index) as MagicAbilityDef;
                    if (powerDef != null)
                    {
                        TooltipHandler.TipRegion(rect2, (Func<string>)(() => powerDef.GetDescription() + "\n" + compMagic.PostAbilityVerbCompDesc((VerbProperties_Ability)powerDef.MainVerb) + "\n" + powerDef.GetPointDesc() + "\n" + powerDef.GetMagicPoolCostDesc()), 398462);
                    }
                    saveIndex = index;
                }

                if (cooldownActive)
                {
                    float num1 = MagicCardUtility.MagicButtonSize + 1f;
                    num1 += MagicCardUtility.MagicButtonPointSize * (float)(saveIndex + 1);
                    float num2 = y + MagicCardUtility.MagicButtonSize / 3f;
                    Rect rect2 = new Rect(inRect.x + num1, num2, MagicCardUtility.MagicButtonPointSize, MagicCardUtility.MagicButtonPointSize);

                    if (compMagic.MagicData.AbilityPoints > 0)
                    { 
                        if (Widgets.ButtonImage(rect2, TexButton.RH_TET_MagicPointRenew) && compMagic.Pawn.Faction == Faction.OfPlayer)
                        {
                            foreach (PawnAbility pa in compMagic.AbilityData.AllPowers)
                            {
                                if (pa.Def.defName.Equals(power.abilityDef.defName))
                                {
                                    pa.CooldownTicksLeft = 0;
                                    cooldownActive = false;
                                    compMagic.MagicData.AbilityPoints = compMagic.MagicData.AbilityPoints - 1;
                                }
                            }
                        }

                        TooltipHandler.TipRegion(rect2, (Func<string>)(() => "RH_TET_AbilityRenew".Translate()), 398772);
                    }
                    else
                    {
                        Widgets.DrawTextureFitted(rect2, (Texture)TexButton.RH_TET_MagicPointRenew, 1f, new Vector2((float)TexButton.RH_TET_MagicPointRenew.width, (float)TexButton.RH_TET_MagicPointRenew.height), new Rect(0.0f, 0.0f, 1f, 1f), 0.0f, TexUI.GrayscaleGUI);
                        TooltipHandler.TipRegion(rect2, (Func<string>)(() => "RH_TET_AbilityRenewNoPoints".Translate()), 398772);
                    }
                }

                y += MagicCardUtility.MagicButtonSize + 1f;
            }
        }
    }
}
