using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using RimWorld.Planet;
using RimWorld;

/**
 * Original code from Jecrell's Cult Mod - Tweaked to fit a Rimhammer Beastmen Use by SickBoyWI.
 * */
namespace TheEndTimes_Magic
{
    public class DarkGod : Thing
    {
        private const float MODIFIER_HUMANLIKE = 0.5f;
        private const float MODIFIER_ANIMAL = 0.2f;
        private const float DIVIDER_HUMANLIKE = 300f;
        private const float DIVIDER_FOOD = 700f;

        private bool hostileToPlayer = false;
        private float favor = 0f;
        private float lastFavor = 0f;
        public float LastFavor => lastFavor;
        public int humanlikesSacrificedCount = 0;

        public DarkGodDef Def => def as DarkGodDef;
        public override string Label => Def.label;
        public override string LabelCap => Def.label.CapitalizeFirst();

        public float PlayerFavor
        {
            get { return this.favor; }
        }

        public DarkGod()
            : base()
        {
        }

        public DarkGod(ThingDef newDef)
            : base()
        {
            DarkGodDef currentDef = newDef as DarkGodDef;
            if (currentDef != null)
            {
                this.def = newDef;
            }
            return;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref this.hostileToPlayer, "hostileToPlayer", false, false);
            Scribe_Values.Look<float>(ref this.favor, "favor", 0f, false);
            Scribe_Values.Look<float>(ref this.lastFavor, "lastFavor", 0f, false);
            Scribe_Values.Look<int>(ref this.humanlikesSacrificedCount, "humanlikesSacrificedCount", 0, false);
        }

        public override void Tick()
        {
            base.Tick();
        }

        public void ConsumeBodyOffering(Thing thing)
        {
            thing.Destroy();
            AffectFavor(1F);
        }

        public void ConsumeOfferings(Pawn eater, Thing offering)
        {
            lastFavor = favor;

            float nutrition = offering.stackCount * FoodUtility.GetNutrition(eater, offering, offering.def);

            // Round to 2 decimal places.
            nutrition = (float)Math.Round(nutrition * 100f) / 100f;

            if (nutrition > 0f && offering.stackCount > 0)
            {
                AffectFavor(nutrition);
            }

            if (!offering.Destroyed)
                offering.Destroy(DestroyMode.Vanish);
        }

        public void ReceiveSacrifice(Pawn sacrifice)
        {
            if (sacrifice.RaceProps.Animal)
            {
                lastFavor = favor;

                DarkGodDef currentDef = this.def as DarkGodDef;
                float value = currentDef.SacredNumber;
                AffectFavor(value);

                // Rewards must be handled by caller.
            }
            else
            {
                // Rewards must be handled by caller.
            }
        }

        // Debug function.
        public void ResetFavor()
        {
            this.favor = 0f;
        }

        public void AffectFavor(float favorChange)
        {
            float newFavor = (float)Math.Round(favorChange, 2);
            float value = this.favor + newFavor;
            favor = value;
        }

        public string Info()
        {
            StringBuilder s = new StringBuilder();
            s.AppendLine("RH_TET_Box_Titles".Translate() + ": " + Def.Titles);
            s.AppendLine();
            s.AppendLine("RH_TET_Box_Domains".Translate() + ": " + Def.Domains);
            s.AppendLine();
            s.AppendLine("RH_TET_Box_Description".Translate() + ": ");
            s.AppendLine(Def.DescriptionLong);
            return s.ToString();
        }

        public string GetInfoText()
        {
            string text = this.def.LabelCap;
            string text2 = text;
            text = string.Concat(new string[]
            {
                text2,
                "\n",
                "ColonyGoodwill".Translate(),
                ": ",
                this.PlayerFavor.ToString("###0")
            });
            if (hostileToPlayer)
            {
                text = text + "\n" + "Hostile".Translate();
            }
            else
            {
                text = text + "\n" + "Neutral".Translate();
            }
            return text;
        }

        public string DebugString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            return stringBuilder.ToString();
        }

        public void DebugDraw()
        {
        }
    }
}