using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    public class Need_MagicPool : Need
    {
        public int ticksUntilBaseSet = 500;
        public const float BaseGainPerTickRate = 150f;
        public const float BaseFallPerTick = 1E-05f;
        public const float ThreshVeryLow = 0.1f;
        public const float ThreshLow = 0.3f;
        public const float ThreshSatisfied = 0.5f;
        public const float ThreshHigh = 0.7f;
        public const float ThreshVeryHigh = 0.9f;
        private int lastGainTick;

        public MagicPoolCategory CurCategory
        {
            get
            {
                if ((double)this.CurLevel < 0.00999999977648258)
                    return MagicPoolCategory.Drained;
                if ((double)this.CurLevel < 0.300000011920929)
                    return MagicPoolCategory.Feeble;
                if ((double)this.CurLevel < 0.5)
                    return MagicPoolCategory.Steady;
                return (double)this.CurLevel < 0.699999988079071 ? MagicPoolCategory.Strong : MagicPoolCategory.Surging;
            }
        }

        public override int GUIChangeArrow
        {
            get
            {
                return !this.GainingNeed ? -1 : 1;
            }
        }

        public override float CurInstantLevel
        {
            get
            {
                return this.CurLevel;
            }
        }

        private bool GainingNeed
        {
            get
            {
                return Find.TickManager.TicksGame < this.lastGainTick + 10;
            }
        }

        public Need_MagicPool(Pawn pawn)
          : base(pawn)
        {
            this.lastGainTick = -999;
            this.threshPercents = new List<float>();
            this.threshPercents.Add(0.3f);
            this.threshPercents.Add(0.7f);
        }

        public override void SetInitialLevel()
        {
            this.CurLevel = 1.0f;
        }

        public void GainNeed(float amount)
        {
            amount /= 120f;
            amount *= 0.01f;
            amount = Mathf.Min(amount, 1f - this.CurLevel);
            this.curLevelInt += amount;
            this.lastGainTick = Find.TickManager.TicksGame;
        }

        public void UseMagicPower(float amount)
        {
            this.curLevelInt = Mathf.Clamp(this.curLevelInt - amount, 0.0f, 1f);
        }

        public void SetLevelToZero()
        {
            this.CurLevel = 0.0f;
        }

        public override void NeedInterval()
        {
            this.GainNeed(1f);
        }

        public override string GetTipString()
        {
            return base.GetTipString();
        }

        public override void DrawOnGUI(
          Rect rect,
          int maxThresholdMarkers = 2147483647,
          float customMargin = -1f,
          bool drawArrows = true,
          bool doTooltip = true,
          Rect? rectForTooltip = null,
          bool drawLabel = true)
        {
            if ((double)((Rect)rect).height > 70.0)
            {
                float num = (float)(((double)((Rect)rect).height - 70.0) / 2.0);
                rect.height = 70f;
                rect.y = rect.y + num;
            }
            if (Mouse.IsOver(rect))
                Widgets.DrawHighlight(rect);

            TooltipHandler.TipRegion(rect, new TipSignal((Func<string>)(() => this.GetTipString()), rect.GetHashCode()));
            float num1 = 14f;
            float num2 = num1 + 15f;
            if ((double)rect.height < 50.0)
                num1 *= Mathf.InverseLerp(0.0f, 50f, rect.height);
            Text.Font = (double)rect.height <= 55.0 ? GameFont.Tiny : GameFont.Small;
            Text.Anchor = (TextAnchor)6;
            Widgets.Label(new Rect((float)((double)rect.x + (double)num2 + (double)rect.width * 0.100000001490116), rect.y, (float)((double)rect.width - (double)num2 - (double)rect.width * 0.100000001490116), rect.height / 2f), this.LabelCap);
            Text.Anchor = (TextAnchor)0;
            Rect rect1 = new Rect(rect.x, rect.y + (rect.height / 2f), rect.width, rect.height / 2f);
            rect1 = new Rect(rect1.x + num2, rect1.y, rect1.width - num2 * 2f, rect1.height - num1);

            if (DebugSettings.ShowDevGizmos)
            {
                float lineHeight = Text.LineHeight;
                Rect rect2 = new Rect(rect1.xMax - lineHeight, rect1.y - lineHeight, lineHeight, lineHeight);
                if (Widgets.ButtonImage(rect2.ContractedBy(4f), Verse.TexButton.Plus, true))
                    this.CurLevelPercentage += 0.1f;
                if (Mouse.IsOver(rect2))
                    TooltipHandler.TipRegion(rect2, (TipSignal)"+ 10%");
                Rect rect3 = new Rect(rect2.xMin - lineHeight, rect1.y - lineHeight, lineHeight, lineHeight);
                if (Widgets.ButtonImage(rect3.ContractedBy(4f), Verse.TexButton.Minus, true))
                    this.CurLevelPercentage -= 0.1f;
                if (Mouse.IsOver(rect3))
                    TooltipHandler.TipRegion(rect3, (TipSignal)"- 10%");
            }

            Widgets.FillableBar(rect1, this.CurLevelPercentage);
            if (this.threshPercents != null)
            {
                for (int index = 0; index < this.threshPercents.Count; ++index)
                    this.DrawBarThreshold(rect1, this.threshPercents[index]);
            }
            float instantLevelPercentage = this.CurInstantLevelPercentage;
            if ((double)instantLevelPercentage >= 0.0)
                this.DrawBarInstantMarkerAt(rect1, instantLevelPercentage);
            if (!this.def.tutorHighlightTag.NullOrEmpty())
                UIHighlighter.HighlightOpportunity(rect, this.def.tutorHighlightTag);
            Text.Font = GameFont.Small;
        }

        private void DrawBarThreshold(Rect barRect, float threshPct)
        {
            float num = (double)barRect.width <= 60.0 ? 1f : 2f;
            Rect rect = new Rect((float)((double)barRect.x + (double)barRect.width * (double)threshPct - ((double)num - 1.0)), barRect.y + barRect.height / 2f, num, barRect.height / 2f);
            Texture2D texture2D1;
            if ((double)threshPct < (double)this.CurLevelPercentage)
            {
                texture2D1 = BaseContent.BlackTex;
                GUI.color = (new Color(1f, 1f, 1f, 0.9f));
            }
            else
            {
                texture2D1 = BaseContent.GreyTex;
                GUI.color = (new Color(1f, 1f, 1f, 0.5f));
            }
            Texture2D texture2D2 = texture2D1;
            GUI.DrawTexture(rect, (Texture)texture2D2);
            GUI.color = (Color.white);
        }
    }
}
