using RimWorld;
using System.Linq;
using Verse;

namespace TheEndTimes_Magic
{
    /**
     * Original code for this came from Dubs Central Heating Mod, and was
     * trimmed down to suit this mod's needs.
     */
    public class CompSteamPipe : ThingComp
    {
        public CompFlickable flicker;
        public CompRefuelable fuel;
        public SteamPipeMapComp mapComp;
        public PlumbingNet pipeNetRef;
        public CompPowerTrader power;

        public PlumbingNet pipeNet
        {
            get
            {
                if (this.pipeNetRef == null)
                    this.mapComp.PipeComp.RegenGrids();
                return this.pipeNetRef;
            }
        }

        public bool closed
        {
            get
            {
                return this.flicker != null && !this.flicker.SwitchIsOn;
            }
        }

        public int GridID { get; set; } = -1;

        public PipeType mode
        {
            get
            {
                return this.Props.mode;
            }
        }

        public CompProperties_SteamPipe Props
        {
            get
            {
                return (CompProperties_SteamPipe)this.props;
            }
        }

        public override void ReceiveCompSignal(string signal)
        {
            base.ReceiveCompSignal(signal);
            if (!(signal == "FlickedOn") && !(signal == "FlickedOff"))
                return;
            this.mapComp.PipeComp.DirtyPipeGrid(this.mode);
            this.mapComp.PipeComp.RegenGrids();
        }

        public override void PostDeSpawn(Map map)
        {
            map.PipeNet().DeregisterPipe(this);
            base.PostDeSpawn(map);
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (this.parent is Building_SteamValve)
                this.flicker = this.parent.GetComp<CompFlickable>();
            this.fuel = this.parent.GetComp<CompRefuelable>();
            if (this.fuel != null && this.fuel.Props.fuelFilter.AllowedDefCount > 0)
                this.fuel = (CompRefuelable)null;
            this.power = this.parent.GetComp<CompPowerTrader>();
            this.mapComp = this.parent.Map.GetComponent<SteamPipeMapComp>();
            foreach (IntVec3 c in this.parent.OccupiedRect())
            {
                foreach (Thing thing in c.GetThingList(this.parent.Map).ToList<Thing>())
                {
                    if (thing != this.parent && thing.TryGetComp<CompSteamPipe>() != null && thing.TryGetComp<CompSteamPipe>().mode == this.mode)
                        thing.Destroy(DestroyMode.Vanish);
                }
            }
            this.parent.Map.mapDrawer.MapMeshDirty(this.parent.Position, MapMeshFlagDefOf.PowerGrid);
            this.mapComp.PipeComp.RegisterPipe(this, respawningAfterLoad);
        }

        public void PrintForOverlay(SectionLayer layer, float alpha = 1f)
        {
            if (this.closed)
                return;
            GraphicsCache.pipeOverlayGraphic[(int)this.mode].Print(layer, (Thing)this.parent, 0.0f);
        }
    }
}
