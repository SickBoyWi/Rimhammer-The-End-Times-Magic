using AbilityUser;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    public class FaithCardUtility
    {
        public static Vector2 faithCardSize = new Vector2(395f, 536f);
        public static float ButtonSize = 40f;
        public static float FaithButtonSize = 46f;
        public static float FaithButtonPointSize = 24f;
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

        public static Vector2 FaithCardSize
        {
            get
            {
                return FaithCardUtility.faithCardSize;
            }
        }

        public static void AdjustForLanguage()
        {
            if (FaithCardUtility.adjustedForLanguage)
                return;
            FaithCardUtility.adjustedForLanguage = true;
            if (LanguageDatabase.activeLanguage != LanguageDatabase.AllLoadedLanguages.FirstOrDefault<LoadedLanguage>((Func<LoadedLanguage, bool>)(x => x.folderName == "German")))
                return;
            FaithCardUtility.SkillsColumnDivider = 170f;
            FaithCardUtility.SkillsTextWidth = 170f;
        }

        public static void DrawFaithCard(Rect rect, Pawn pawn)
        {
            FaithCardUtility.AdjustForLanguage();
            GUI.BeginGroup(rect);
            CompFaithUser comp1 = pawn.GetComp<CompFaithUser>();

            if (comp1 != null)
            {
                    Trait trait = FindFaithTrait(comp1);

                    float x1 = (float)Text.CalcSize(trait.Label).x;
                    Rect rect1 = new Rect(3.0f, rect.y, rect.width, FaithCardUtility.HeaderSize);
                    Text.Font = GameFont.Medium;
                    Widgets.Label(rect1, trait.Label.CapitalizeFirst());

                    Widgets.DrawLineHorizontal(rect.x - 10f, rect1.yMax, rect.width - 15f);

                    Rect inRect1 = new Rect(3.0f, rect1.yMax + FaithCardUtility.Padding, rect.width, (FaithCardUtility.SkillsColumnHeight + (FaithCardUtility.TextSize * 2)));
                    Text.Font = GameFont.Small;
                    Widgets.Label(inRect1, "RH_TET_FaithPawnInfo".Translate());

                    Rect rect9 = new Rect(3.0f, inRect1.yMax + (FaithCardUtility.SectionOffset * 4f), rect.width, FaithCardUtility.HeaderSize);
                    Text.Font = GameFont.Medium;
                    Widgets.Label(rect9, "RH_TET_Prayers".Translate().CapitalizeFirst());
                    Text.Font = GameFont.Small;
                    Widgets.DrawLineHorizontal(rect.x - 10f, rect9.yMax, rect.width - 15f);

                    Rect rect10 = new Rect(rect.x, rect9.yMax + FaithCardUtility.SectionOffset, rect9.width, FaithCardUtility.PowersColumnHeight);
                    Rect inRect3 = new Rect(rect10.x, rect10.y, FaithCardUtility.PowersColumnWidth, FaithCardUtility.PowersColumnHeight);
                    Rect inRect4 = new Rect(4.0f, rect10.y, FaithCardUtility.PowersColumnWidth, FaithCardUtility.PowersColumnHeight);
                    Rect inRect5 = new Rect(4.0f, rect10.y, FaithCardUtility.PowersColumnWidth, FaithCardUtility.PowersColumnHeight);
                    
                    if (MagicUtility.GetFaithType(pawn).Equals(FaithType.Sigmar))
                        FaithCardUtility.PowersGUIHandler(inRect4, pawn.GetComp<CompFaithUser>(), pawn.GetComp<CompFaithUser>().FaithData.PowersSigmar, TexButton.RH_TET_MagicPoint);
                    else if (MagicUtility.GetFaithType(pawn).Equals(FaithType.Shallya))
                        FaithCardUtility.PowersGUIHandler(inRect4, pawn.GetComp<CompFaithUser>(), pawn.GetComp<CompFaithUser>().FaithData.PowersShallya, TexButton.RH_TET_MagicPoint);
                    else if (MagicUtility.GetFaithType(pawn).Equals(FaithType.Ulric))
                        FaithCardUtility.PowersGUIHandler(inRect4, pawn.GetComp<CompFaithUser>(), pawn.GetComp<CompFaithUser>().FaithData.PowersUlric, TexButton.RH_TET_MagicPoint);
                    else
                        Log.Error("No valid faith found for faithful. WTF?");
            }
            GUI.EndGroup();
        }

        private static Trait FindFaithTrait(CompFaithUser comp1)
        {
            Trait faithTrait = null;

            TraitSet compTraits = comp1.AbilityUser.story.traits;
            foreach (Trait trait in compTraits.allTraits)
            {
                if (trait.def.defName.Equals("RH_TET_SigmarTrait")
                        || trait.def.defName.Equals("RH_TET_ShallyaTrait")
                        || trait.def.defName.Equals("RH_TET_UlricTrait")
                        )
                {
                    faithTrait = trait;
                    break;
                }
            }

            return faithTrait;
        }

        public static void InfoPaneSensitive(Rect inRect, CompFaithUser comp)
        {
            Rect rect1 = new Rect(inRect.x, inRect.y, inRect.width * 0.7f, FaithCardUtility.TextSize);
            Text.Font = GameFont.Small;
            Widgets.Label(rect1, "RH_TET_FaithSensitiveMessage".Translate((object)comp.AbilityUser.LabelShort));
            Text.Font = GameFont.Small;
            Rect rect2 = new Rect(inRect.x, rect1.yMax, inRect.width, FaithCardUtility.TextSize);
        }

        public static void PowersGUIHandler(
          Rect inRect,
          CompFaithUser comp,
          List<FaithPower> powers,
          Texture2D pointTexture)
        {
            float y = inRect.y;
            foreach (FaithPower faithPower in powers)
            {
                FaithPower power = faithPower;
                Rect rect1 = new Rect(inRect.x, y, FaithCardUtility.FaithButtonSize, FaithCardUtility.FaithButtonSize);
                TooltipHandler.TipRegion(rect1, (Func<string>)(() => ((Def)power.abilityDef).label + "\n\n" + ((Def)power.abilityDef).description + "\n\n" + "RH_TET_FaithCheckBoxesForMoreInfo".Translate()), 398462);
            
                Widgets.DrawTextureFitted(rect1, (Texture)power.Icon, 1f);

                for (int index = 0; index < 1; ++index)
                {
                    float num1 = FaithCardUtility.FaithButtonSize + 1f;
                    if (index != 0)
                        num1 += FaithCardUtility.FaithButtonPointSize * (float)index;
                    float num2 = y + FaithCardUtility.FaithButtonSize / 3f;
                    Rect rect2 = new Rect(inRect.x + num1, num2, FaithCardUtility.FaithButtonPointSize, FaithCardUtility.FaithButtonPointSize);

                    if (power.level > index)
                        Widgets.DrawTextureFitted(rect2, (Texture)pointTexture, 1f);
                    else
                        Widgets.DrawTextureFitted(rect2, (Texture)TexButton.RH_TET_MagicPointOff, 1f);

                    FaithAbilityDef powerDef = power.GetAbilityDef(index) as FaithAbilityDef;
                    if (powerDef != null)
                    {
                        TooltipHandler.TipRegion(rect2, (Func<string>)(() => powerDef.GetDescription() + "\n" + comp.PostAbilityVerbCompDesc((VerbProperties_Ability)powerDef.MainVerb) + "\n" + powerDef.GetPointDesc() + "\n" + powerDef.GetFaithPoolCostDesc()), 398462);
                    }
                }

                y += FaithCardUtility.FaithButtonSize + 1f;
            }
        }
    }
}
