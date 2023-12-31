using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    /**
     * Original code for this came from Dubs Central Heating Mod, and was
     * trimmed down to suit this mod's needs.
     */
    [StaticConstructorOnStartup]
    public static class GraphicsCache
    {
        public static Material MissingPipes = MaterialPool.MatFrom("UI/Buildings/PipeMissing", ShaderDatabase.MetaOverlay);
        public static Material PowerOff = MaterialPool.MatFrom("UI/Buildings/PowerOff", ShaderDatabase.MetaOverlay);
        public static Material SteamPipeFloorConnect = MaterialPool.MatFrom("UI/Icons/LifeStage/Adult", ShaderDatabase.MetaOverlay, Color.white);
        public static Graphic_LinkedPipeOverlay[] pipeOverlayGraphic = new Graphic_LinkedPipeOverlay[1]
        {
            new Graphic_LinkedPipeOverlay(GraphicDatabase.Get<Graphic_Single>("Things/Building/Steam/SteamPipesOverlay_atlas", ShaderDatabase.MetaOverlay, Vector2.one, Color.cyan), PipeType.Heating)
        };
        public static Graphic_LinkedPipe[] pipeGraphic = new Graphic_LinkedPipe[1]
        {
            new Graphic_LinkedPipe(GraphicDatabase.Get<Graphic_Single>("Things/Building/Steam/SteamPipes_atlas", ShaderDatabase.Transparent), PipeType.Heating)
        };

        public static Mesh MeshFor(Vector2 size, Rot4 rot)
        {
            return !(rot == Rot4.West) && !(rot == Rot4.East) ? MeshPool.GridPlane(size) : MeshPool.GridPlaneFlip(size);
        }
    }
}
