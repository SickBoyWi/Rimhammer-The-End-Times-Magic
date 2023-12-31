using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    [StaticConstructorOnStartup]
    public class Command_Rechargeable : Command_VerbTarget
    {
        private readonly CompRechargeable comp;
        public Color? overrideColor;
        private static readonly Texture2D cooldownBarTex = SolidColorMaterials.NewSolidColorTexture((Color)new Color32((byte)9, (byte)203, (byte)4, (byte)64));

        public Command_Rechargeable(CompRechargeable comp)
        {
            this.comp = comp;
        }

        public override string TopRightLabel
        {
            get
            {
                return "";
            }
        }

        public override Color IconDrawColor
        {
            get
            {
                return this.overrideColor ?? base.IconDrawColor;
            }
        }

        public override void GizmoUpdateOnMouseover()
        {
            this.verb.DrawHighlight(LocalTargetInfo.Invalid);
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            Rect rect = new Rect(topLeft.x, topLeft.y, this.GetWidth(maxWidth), 75f);
            GizmoResult gizmoResult = base.GizmoOnGUI(topLeft, maxWidth, parms);
            if (this.comp.TicksUntilNextUse > 0)
            {
                float f = Mathf.InverseLerp((float)this.comp.Props.baseRechargeTicks, 0.0f, (float)this.comp.TicksUntilNextUse);
                Widgets.FillableBar(rect, Mathf.Clamp01(f), Command_Rechargeable.cooldownBarTex, (Texture2D)null, false);
                if (this.comp.TicksUntilNextUse > 0)
                {
                    Text.Font = GameFont.Tiny;
                    Text.Anchor = TextAnchor.UpperCenter;
                    Widgets.Label(rect, f.ToStringPercent("F0"));
                    Text.Anchor = TextAnchor.UpperLeft;
                }
            }
            return gizmoResult.State == GizmoState.Interacted && this.comp.CanBeUsed ? gizmoResult : new GizmoResult(gizmoResult.State);
        }

        public override bool GroupsWith(Gizmo other)
        {
            return base.GroupsWith(other) && other is Command_Rechargeable commandRechargeable && this.comp.parent.def == commandRechargeable.comp.parent.def;
        }
    }
}
