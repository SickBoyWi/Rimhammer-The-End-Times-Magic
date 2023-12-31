using UnityEngine;
using Verse;
using RimWorld;

namespace TheEndTimes_Magic
{
    [StaticConstructorOnStartup]
    public class Gizmo_MagicEnergyShieldStatus : Gizmo
    {
        private static readonly Texture2D FullShieldBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.24f));
        private static readonly Texture2D EmptyShieldBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);
        public CompShieldKhorne shield;

        public Gizmo_MagicEnergyShieldStatus()
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
            Rect rect1 = new Rect(topLeft.x, topLeft.y, this.GetWidth(maxWidth), 75f);
            Rect rect2 = rect1.ContractedBy(6f);
            Widgets.DrawWindowBackground(rect1);
            Rect rect3 = rect2;
            rect3.height = rect1.height / 2f;
            Text.Font = GameFont.Tiny;
            Widgets.Label(rect3, this.shield.IsApparel ? this.shield.parent.LabelCap : "ShieldInbuilt".Translate().Resolve());
            Rect rect4 = rect2;
            rect4.yMin = rect2.y + rect2.height / 2f;
            float fillPercent = this.shield.Energy / Mathf.Max(1f, this.shield.parent.GetStatValue(StatDefOf.EnergyShieldEnergyMax, true, -1));
            Widgets.FillableBar(rect4, fillPercent, Gizmo_MagicEnergyShieldStatus.FullShieldBarTex, Gizmo_MagicEnergyShieldStatus.EmptyShieldBarTex, false);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect4, (this.shield.Energy * 100f).ToString("F0") + " / " + (this.shield.parent.GetStatValue(StatDefOf.EnergyShieldEnergyMax, true, -1) * 100f).ToString("F0"));
            Text.Anchor = TextAnchor.UpperLeft;
            TooltipHandler.TipRegion(rect2, (TipSignal)"ShieldPersonalTip".Translate());
            return new GizmoResult(GizmoState.Clear);
        }
    }
}
