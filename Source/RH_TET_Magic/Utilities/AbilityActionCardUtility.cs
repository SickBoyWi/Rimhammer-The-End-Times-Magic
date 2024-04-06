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
    public class AbilityActionCardUtility
    {
        public static Vector2 abilityActionCardSize = new Vector2(395f, 536f);
        public static float ButtonSize = 40f;
        public static float AbilityActionButtonSize = 46f;
        public static float AbilityActionButtonPointSize = 24f;
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

        public static Vector2 AbilityActionCardSize
        {
            get
            {
                return AbilityActionCardUtility.abilityActionCardSize;
            }
        }

        public static void AdjustForLanguage()
        {
            if (AbilityActionCardUtility.adjustedForLanguage)
                return;
            AbilityActionCardUtility.adjustedForLanguage = true;
            if (LanguageDatabase.activeLanguage != LanguageDatabase.AllLoadedLanguages.FirstOrDefault<LoadedLanguage>((Func<LoadedLanguage, bool>)(x => x.folderName == "German")))
                return;
            AbilityActionCardUtility.SkillsColumnDivider = 170f;
            AbilityActionCardUtility.SkillsTextWidth = 170f;
        }

        public static void DrawAbilityCard(Rect rect, Pawn pawn)
        {
            AbilityActionCardUtility.AdjustForLanguage();
            GUI.BeginGroup(rect);
            CompAbilityActionUser comp1 = pawn.GetComp<CompAbilityActionUser>();

            if (comp1 != null)
            {
                    Trait trait = FindAbilityActionTrait(comp1);

                    float x1 = (float)Text.CalcSize(trait.Label).x;
                    Rect rect1 = new Rect(3.0f, rect.y, rect.width, AbilityActionCardUtility.HeaderSize);
                    Text.Font = GameFont.Medium;
                    Widgets.Label(rect1, trait.Label.CapitalizeFirst());

                    Widgets.DrawLineHorizontal(rect.x - 10f, rect1.yMax, rect.width - 15f);

                    Rect inRect1 = new Rect(3.0f, rect1.yMax + AbilityActionCardUtility.Padding, rect.width, (AbilityActionCardUtility.SkillsColumnHeight + (AbilityActionCardUtility.TextSize * 2)));
                    Text.Font = GameFont.Small;
                    Widgets.Label(inRect1, "RH_TET_AbilityPawnInfo".Translate());

                    Rect rect9 = new Rect(3.0f, inRect1.yMax + (AbilityActionCardUtility.SectionOffset * 4f), rect.width, AbilityActionCardUtility.HeaderSize);
                    Text.Font = GameFont.Medium;
                    Widgets.Label(rect9, "RH_TET_Abilities".Translate().CapitalizeFirst());
                    Text.Font = GameFont.Small;
                    Widgets.DrawLineHorizontal(rect.x - 10f, rect9.yMax, rect.width - 15f);

                    Rect rect10 = new Rect(rect.x, rect9.yMax + AbilityActionCardUtility.SectionOffset, rect9.width, AbilityActionCardUtility.PowersColumnHeight);
                    Rect inRect3 = new Rect(rect10.x, rect10.y, AbilityActionCardUtility.PowersColumnWidth, AbilityActionCardUtility.PowersColumnHeight);
                    Rect inRect4 = new Rect(4.0f, rect10.y, AbilityActionCardUtility.PowersColumnWidth, AbilityActionCardUtility.PowersColumnHeight);
                    Rect inRect5 = new Rect(4.0f, rect10.y, AbilityActionCardUtility.PowersColumnWidth, AbilityActionCardUtility.PowersColumnHeight);
                    
                    if (MagicUtility.GetAbilityActionType(pawn).Equals(AbilityActionType.WitchHunter))
                        AbilityActionCardUtility.PowersGUIHandler(inRect4, pawn.GetComp<CompAbilityActionUser>(), pawn.GetComp<CompAbilityActionUser>().AbilityActionData.PowersWitchHunter, TexButton.RH_TET_MagicPoint);
                    //else if (MagicUtility.GetFaithType(pawn).Equals(FaithType.Shallya))
                    //    FaithCardUtility.PowersGUIHandler(inRect4, pawn.GetComp<CompFaithUser>(), pawn.GetComp<CompFaithUser>().FaithData.PowersShallya, TexButton.RH_TET_MagicPoint);
                    //else if (MagicUtility.GetFaithType(pawn).Equals(FaithType.Ulric))
                    //    FaithCardUtility.PowersGUIHandler(inRect4, pawn.GetComp<CompFaithUser>(), pawn.GetComp<CompFaithUser>().FaithData.PowersUlric, TexButton.RH_TET_MagicPoint);
                    else
                        Log.Error("No valid ability found for ability action user. WTF?");
            }
            GUI.EndGroup();
        }

        private static Trait FindAbilityActionTrait(CompAbilityActionUser comp1)
        {
            Trait abilTrait = null;

            TraitSet compTraits = comp1.Pawn.story.traits;
            foreach (Trait trait in compTraits.allTraits)
            {
                if (trait.def.defName.Equals("RH_TET_WitchHunterTrait")
                        //|| trait.def.defName.Equals("RH_TET_ShallyaTrait")
                        //|| trait.def.defName.Equals("RH_TET_UlricTrait")
                        )
                {
                    abilTrait = trait;
                    break;
                }
            }

            return abilTrait;
        }

        public static void InfoPaneSensitive(Rect inRect, CompFaithUser comp)
        {
            Rect rect1 = new Rect(inRect.x, inRect.y, inRect.width * 0.7f, AbilityActionCardUtility.TextSize);
            Text.Font = GameFont.Small;
            Widgets.Label(rect1, "RH_TET_AbilitySensitiveMessage".Translate((object)comp.AbilityUser.LabelShort));
            Text.Font = GameFont.Small;
            Rect rect2 = new Rect(inRect.x, rect1.yMax, inRect.width, AbilityActionCardUtility.TextSize);
        }

        public static void PowersGUIHandler(
          Rect inRect,
          CompAbilityActionUser comp,
          List<AbilityActionPower> powers,
          Texture2D pointTexture)
        {
            float y = inRect.y;
            foreach (AbilityActionPower abilPower in powers)
            {
                AbilityActionPower power = abilPower;
                Rect rect1 = new Rect(inRect.x, y, AbilityActionCardUtility.AbilityActionButtonSize, AbilityActionCardUtility.AbilityActionButtonSize);
                TooltipHandler.TipRegion(rect1, (Func<string>)(() => ((Def)power.abilityDef).label + "\n\n" + ((Def)power.abilityDef).description + "\n\n" + "RH_TET_AbilityCheckBoxesForMoreInfo".Translate()), 398462);
            
                Widgets.DrawTextureFitted(rect1, (Texture)power.Icon, 1f);

                for (int index = 0; index < 1; ++index)
                {
                    float num1 = AbilityActionCardUtility.AbilityActionButtonSize + 1f;
                    if (index != 0)
                        num1 += AbilityActionCardUtility.AbilityActionButtonPointSize * (float)index;
                    float num2 = y + AbilityActionCardUtility.AbilityActionButtonSize / 3f;
                    Rect rect2 = new Rect(inRect.x + num1, num2, AbilityActionCardUtility.AbilityActionButtonPointSize, AbilityActionCardUtility.AbilityActionButtonPointSize);

                    if (power.level > index)
                        Widgets.DrawTextureFitted(rect2, (Texture)pointTexture, 1f);
                    else
                        Widgets.DrawTextureFitted(rect2, (Texture)TexButton.RH_TET_MagicPointOff, 1f);

                    AbilityActionAbilityDef powerDef = power.GetAbilityDef(index) as AbilityActionAbilityDef;
                    if (powerDef != null)
                    {
                        TooltipHandler.TipRegion(rect2, (Func<string>)(() => powerDef.GetDescription() + "\n" + comp.PostAbilityVerbCompDesc((VerbProperties_Ability)powerDef.MainVerb) + "\n" + powerDef.GetPointDesc() + "\n" + powerDef.GetAbilityPoolCostDesc()), 398462);
                    }
                }

                y += AbilityActionCardUtility.AbilityActionButtonSize + 1f;
            }
        }
    }
}
