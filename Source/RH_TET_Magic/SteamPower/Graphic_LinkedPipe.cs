using System;
using System.Linq;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    /**
     * Original code for this came from Dubs Central Heating Mod, and was
     * trimmed down to suit this mod's needs.
     */
    public class Graphic_LinkedPipe : Graphic_Linked
    {
        public PipeType mode;
        private ThingDef pipes;

        public Graphic_LinkedPipe()
        {
        }

        public Graphic_LinkedPipe(Graphic subGraphic, PipeType m)
          : base(subGraphic)
        {
            this.subGraphic = subGraphic;
            this.mode = m;
        }

        public Graphic_LinkedPipe(Graphic subGraphic)
          : base(subGraphic)
        {
            this.subGraphic = subGraphic;
        }

        public override Graphic GetColoredVersion(
          Shader newShader,
          Color newColor,
          Color newColorTwo)
        {
            Graphic_LinkedPipe graphicLinkedPipe = new Graphic_LinkedPipe(this.subGraphic.GetColoredVersion(newShader, newColor, newColorTwo), this.mode);
            graphicLinkedPipe.data = this.data;
            return (Graphic)graphicLinkedPipe;
        }

        public void PrintForGrid(SectionLayer layer, Thing thing, SteamPipeMapComp net)
        {
            IntVec3 position = thing.Position;
            if (!Settings.SteamPipesVisible || (!Settings.SteamPipesVisible ? 0 : (position.GetTerrain(thing.Map).layerable ? 1 : 0)) != 0)
                return;
            this.Print(layer, thing, 0.0f);
        }

        public override bool ShouldLinkWith(IntVec3 c, Thing parent)
        {
            if (!c.InBounds(parent.Map) || Settings.SteamPipesVisible && c.GetTerrain(parent.Map).layerable)
                return false;
            if (this.pipes == null)
            {
                foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefsListForReading.Where<ThingDef>((Func<ThingDef, bool>)(x => x.thingClass == typeof(Building_SteamPipe))))
                {
                    CompProperties_SteamPipe compProperties = thingDef.GetCompProperties<CompProperties_SteamPipe>();
                    if (compProperties != null && compProperties.mode == this.mode)
                        this.pipes = thingDef;
                }
            }
            return c.GetFirstThing(parent.Map, this.pipes) != null && parent.Map.PipeNet().ZoneAt(c, this.mode);
        }
    }
}
