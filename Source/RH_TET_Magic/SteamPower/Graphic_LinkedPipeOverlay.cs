using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    /**
     * Original code for this came from Dubs Central Heating Mod, and was
     * trimmed down to suit this mod's needs.
     */
    public class Graphic_LinkedPipeOverlay : Graphic_Linked
    {
        public PipeType mode;

        public Graphic_LinkedPipeOverlay()
        {
        }

        public Graphic_LinkedPipeOverlay(Graphic subGraphic)
          : base(subGraphic)
        {
        }

        public Graphic_LinkedPipeOverlay(Graphic subGraphic, PipeType m)
          : base(subGraphic)
        {
            this.mode = m;
        }

        public override bool ShouldLinkWith(IntVec3 c, Thing parent)
        {
            return c.InBounds(parent.Map) && parent.Map.PipeNet().ZoneAt(c, this.mode);
        }

        public override void Print(SectionLayer layer, Thing parent, float extraRotation)
        {
            foreach (IntVec3 cell in parent.OccupiedRect())
            {
                Vector3 shiftedWithAltitude = cell.ToVector3ShiftedWithAltitude(AltitudeLayer.MapDataOverlay);
                Printer_Plane.PrintPlane(layer, shiftedWithAltitude, Vector2.one, this.LinkedDrawMatFrom(parent, cell), 0.0f, false, (Vector2[])null, (Color32[])null, 0.01f, 0.0f);
            }
        }
    }
}
