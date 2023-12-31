using System;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    /**
     * Original code for this came from Dubs Central Heating Mod, and was
     * trimmed down to suit this mod's needs.
     */
    [StaticConstructorOnStartup]
    internal class Gizmo_BoilerStatus : Gizmo
    {
        private static readonly Texture2D FullBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.35f, 0.35f, 0.2f));
        private static readonly Texture2D EmptyBarTex = SolidColorMaterials.NewSolidColorTexture(Color.black);
        private static readonly Texture2D TargetLevelArrow = ContentFinder<Texture2D>.Get("UI/Misc/BarInstantMarkerRotated", true);
        public CompBoiler boiler;

        public Gizmo_BoilerStatus()
        {
            this.Order = -100f;
        }

        public override float GetWidth(float maxWidth)
        {
            return 140f;
        }

        public override GizmoResult GizmoOnGUI(
          Vector2 topLeft,
          float maxWidth,
          GizmoRenderParms parms)
        {
            Rect overRect = new Rect(topLeft.x, topLeft.y, this.GetWidth(maxWidth), 75f);
            Find.WindowStack.ImmediateWindow(1523289473, overRect, WindowLayer.GameUI, (Action)(() =>
            {
                Rect rect1;
                Rect rect2 = rect1 = overRect.AtZero().ContractedBy(6f);
                rect2.height = overRect.height / 2f;
                Text.Font = GameFont.Tiny;
                Widgets.Label(rect2, this.boiler.Props.GizmoLabel);
                Rect rect3 = rect1;
                rect3.yMin = overRect.height / 2f;
                float fillPercent = (float)this.boiler.powerMode / (float)this.boiler.Props.PowerModes;
                Widgets.FillableBar(rect3, fillPercent, Gizmo_BoilerStatus.FullBarTex, Gizmo_BoilerStatus.EmptyBarTex, false);
                float num = (float)this.boiler.powerMode / (float)this.boiler.Props.PowerModes;
                GUI.DrawTexture(new Rect((float)((double)rect3.x + (double)num * (double)rect3.width - (double)Gizmo_BoilerStatus.TargetLevelArrow.width * 0.5 / 2.0), rect3.y - (float)Gizmo_BoilerStatus.TargetLevelArrow.height * 0.5f, (float)Gizmo_BoilerStatus.TargetLevelArrow.width * 0.5f, (float)Gizmo_BoilerStatus.TargetLevelArrow.height * 0.5f), (Texture)Gizmo_BoilerStatus.TargetLevelArrow);
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(rect3, this.boiler.powerMode.ToString("F0") + " / " + this.boiler.Props.PowerModes.ToString("F0"));
                Text.Anchor = TextAnchor.UpperLeft;
            }), true, false, 1f, (Action)null);
            return new GizmoResult(GizmoState.Clear);
        }
    }
}
