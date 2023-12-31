using System;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    [StaticConstructorOnStartup]
    internal class Gizmo_HediffMagicShieldStatus : Gizmo
    {
        private static readonly Texture2D FullShieldBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.24f));
        private static readonly Texture2D EmptyShieldBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);
        public HediffComp_MagicShield shield;

        public override float GetWidth(float maxWidth)
        {
            return 140f;
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            Rect overRect = new Rect((float)topLeft.x, (float)topLeft.y, this.GetWidth(maxWidth), 75f);
            Find.WindowStack.ImmediateWindow(984688, overRect, WindowLayer.GameUI,
                    (Action)(() =>
                    {
                        Rect rect1;
                        Rect rect2 = rect1 = overRect.AtZero().ContractedBy(6f);
                        rect2.height = (overRect.height / 2f);
                        Text.Font = GameFont.Tiny;
                        Widgets.Label(rect2, this.shield.labelCap);
                        Rect rect3 = rect1;
                        rect3.yMin = (overRect.height / 2f);
                        float fillPercent = this.shield.Energy / Mathf.Max(1f, this.shield.EnergyMax);
                        Widgets.FillableBar(rect3, fillPercent, Gizmo_HediffMagicShieldStatus.FullShieldBarTex, Gizmo_HediffMagicShieldStatus.EmptyShieldBarTex, false);
                        Text.Font = GameFont.Small;
                        Text.Anchor = (TextAnchor)4;
                        Rect rect4 = rect3;
                        float num = this.shield.Energy * 100f;
                        string str1 = num.ToString("F0");
                        num = this.shield.EnergyMax * 100f;
                        string str2 = num.ToString("F0");
                        string label = str1 + " / " + str2;
                        Widgets.Label(rect4, label);
                        Text.Anchor = (TextAnchor)0;
        }), true, false, 1f);

      return new GizmoResult(GizmoState.Clear);
    }
    }
}
