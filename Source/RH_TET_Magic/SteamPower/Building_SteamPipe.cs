using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    /**
     * Original code for this came from Dubs Central Heating Mod, and was
     * trimmed down to suit this mod's needs.
     */
    public class Building_SteamPipe : Building
    {
        public CompSteamPipe pipeComp;
        private Graphic_LinkedPipe linkedPipeImage;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.pipeComp = this.GetComp<CompSteamPipe>();
        }

        public void PrintForGrid(SectionLayer layer)
        {
            if (this.pipeComp == null)
                return;
            this.CachedImg.PrintForGrid(layer, (Thing)this, this.pipeComp.mapComp.PipeComp);
        }

        public void PrintForOverlay(SectionLayer layer)
        {
            this.CachedImg.PrintForGrid(layer, (Thing)this, this.pipeComp.mapComp.PipeComp);
        }

        public override void Print(SectionLayer layer)
        {
        }

        public Graphic_LinkedPipe CachedImg
        {
            get
            {
                if (this.linkedPipeImage == null)
                    this.linkedPipeImage = !this.pipeComp.Props.stuffed ? GraphicsCache.pipeGraphic[(int)this.pipeComp.mode] : GraphicsCache.pipeGraphic[(int)this.pipeComp.mode].GetColoredVersion(GraphicsCache.pipeGraphic[(int)this.pipeComp.mode].Shader, this.DrawColor, this.DrawColorTwo) as Graphic_LinkedPipe;
                return this.linkedPipeImage;
            }
        }
    }
}