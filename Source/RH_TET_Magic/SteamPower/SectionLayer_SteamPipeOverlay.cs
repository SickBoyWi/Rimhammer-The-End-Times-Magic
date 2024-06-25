using RimWorld;
using System;
using System.Linq;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    internal class SectionLayer_SteamPipeOverlay : SectionLayer
    {
        public PipeType mode;

        public SectionLayer_SteamPipeOverlay(Verse.Section section)
          : base(section)
        {
            this.mode = PipeType.Heating;
            this.relevantChangeTypes = MapMeshFlagDefOf.Buildings;
        }

        public void DrawAllTileOverlays()
        {
            if (!this.Visible)
                return;
            foreach (Map map in Find.Maps)
            {
                if (map != this.Map && map.Tile == this.Map.Tile)
                {
                    foreach (IntVec3 visibleSection in this.GetVisibleSections(map))
                    {
                        SectionLayer sl = map.mapDrawer.SectionAt(visibleSection).GetLayer(typeof(SectionLayer_SteamPipeOverlay));
                        if (sl != null)
                        {
                            if (!sl.Visible)
                                return;
                            int count = sl.subMeshes.Count;
                            for (int index = 0; index < count; ++index)
                            {
                                LayerSubMesh subMesh = sl.subMeshes[index];
                                if (subMesh.finalized && !subMesh.disabled)
                                    Graphics.DrawMesh(subMesh.mesh, Vector3.zero, Quaternion.identity, subMesh.material, 0);
                            }
                        }
                    }
                }
            }
        }

        private CellRect GetVisibleSections(Map map)
        {
            CellRect sunShadowsViewRect = this.GetSunShadowsViewRect(map, Find.CameraDriver.CurrentViewRect);
            sunShadowsViewRect.ClipInsideMap(map);
            IntVec2 intVec2_1 = this.SectionCoordsAt(sunShadowsViewRect.Min);
            IntVec2 intVec2_2 = this.SectionCoordsAt(sunShadowsViewRect.Max);
            return intVec2_2.x < intVec2_1.x || intVec2_2.z < intVec2_1.z ? CellRect.Empty : CellRect.FromLimits(intVec2_1.x, intVec2_1.z, intVec2_2.x, intVec2_2.z);
        }

        private CellRect GetSunShadowsViewRect(Map map, CellRect rect)
        {
            GenCelestial.LightInfo lightSourceInfo = GenCelestial.GetLightSourceInfo(map, GenCelestial.LightType.Shadow);
            if ((double)lightSourceInfo.vector.x < 0.0)
                rect.maxX -= Mathf.FloorToInt(lightSourceInfo.vector.x);
            else
                rect.minX -= Mathf.CeilToInt(lightSourceInfo.vector.x);
            if ((double)lightSourceInfo.vector.y < 0.0)
                rect.maxZ -= Mathf.FloorToInt(lightSourceInfo.vector.y);
            else
                rect.minZ -= Mathf.CeilToInt(lightSourceInfo.vector.y);
            return rect;
        }

        private IntVec2 SectionCoordsAt(IntVec3 loc)
        {
            return new IntVec2(Mathf.FloorToInt((float)(loc.x / 17)), Mathf.FloorToInt((float)(loc.z / 17)));
        }

        public override void DrawLayer()
        {
            // Whole map goes black without this if
            if (Designator_RemoveSteamPipes.cachedPipeTypes == null)
            {
                Designator_RemoveSteamPipes.cachedPipeTypes = DefDatabase<ThingDef>.AllDefsListForReading.Where<ThingDef>((Func<ThingDef, bool>)(x => x.thingClass == typeof(Building_SteamPipe))).ToList<ThingDef>();
                
                if (!Designator_RemoveSteamPipes.cachedPipeTypes.NullOrEmpty())
                    Designator_RemoveSteamPipes.RemovalMode = Designator_RemoveSteamPipes.cachedPipeTypes.First<ThingDef>();
            }

            if (Find.DesignatorManager.SelectedDesignator is Designator_Build selectedDesignator && selectedDesignator.PlacingDef is ThingDef placingDef && placingDef.comps.OfType<CompProperties_SteamPipe>().Any<CompProperties_SteamPipe>((Func<CompProperties_SteamPipe, bool>)(x => x.mode == this.mode)))
            {
                this.DrawAllTileOverlays();
                base.DrawLayer();
            }
            if (!(Find.DesignatorManager.SelectedDesignator is Designator_RemoveSteamPipes selectedDesignators) || Designator_RemoveSteamPipes.RemovalMode.GetCompProperties<CompProperties_SteamPipe>().mode != this.mode)
                return;
            this.DrawAllTileOverlays();
            base.DrawLayer();
        }

        public override void Regenerate()
        {
            this.ClearSubMeshes(MeshParts.All);
            if (this.Map == null || this.Map.Tile < 0 || PlumbingNet.AllTileNets(this.Map.Tile).EnumerableNullOrEmpty())
                return;
            foreach (PlumbingNet plumbingNet in PlumbingNet.AllTileNets(this.Map.Tile).Where<PlumbingNet>((Func<PlumbingNet, bool>)(x => x.MapComp.map != this.Map)))
            {
                foreach (IntVec3 cell in plumbingNet.cells)
                {
                    if (this.section.CellRect.Contains(cell) && this.Map.PipeNet().ZoneAt(cell, this.mode))
                        Printer_Plane.PrintPlane((SectionLayer)this, cell.ToVector3ShiftedWithAltitude(AltitudeLayer.MetaOverlays), Vector2.one, GraphicsCache.SteamPipeFloorConnect, 0.0f, false, (Vector2[])null, (Color32[])null, 0.01f, 0.0f);
                }
            }
            foreach (IntVec3 c in this.section.CellRect)
            {
                foreach (Thing thing in c.GetThingList(this.Map))
                {
                    CompSteamPipe comp = thing.TryGetComp<CompSteamPipe>();
                    if (comp != null && comp.mode == this.mode && (thing.Position.x == c.x && thing.Position.z == c.z))
                        comp.PrintForOverlay((SectionLayer)this, 1f);
                }
            }
            this.FinalizeMesh(MeshParts.All);
        }
    }
}
