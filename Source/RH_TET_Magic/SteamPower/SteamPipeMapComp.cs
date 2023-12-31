using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;

namespace TheEndTimes_Magic
{
    /**
     * Original code for this came from Dubs Central Heating Mod, and was
     * trimmed down to suit this mod's needs.
     */
    public class SteamPipeMapComp : MapComponent
    {
        public static SteamPipeMapComp loccachecomp = (SteamPipeMapComp)null;
        public static Dictionary<int, SteamPipeMapComp> SteamPipeComps = new Dictionary<int, SteamPipeMapComp>();
        private static int MasterInternetID = 0;
        public PlumbingNet[] PipeNets = new List<PlumbingNet>().ToArray();
        public List<CompSteamPipe> cachedPipes = new List<CompSteamPipe>();
        public int masterID = 1;
        public Dictionary<IntVec3, CompSteamPipe> pipeDic = new Dictionary<IntVec3, CompSteamPipe>();
        public bool MarkHeatingForDraw;
        private SteamPipeMapComp PipeCompInt;
        public int[,] PipeGrid;
        public bool[] DirtyPipe;

        public SteamPipeMapComp PipeComp
        {
            get
            {
                if (this.PipeCompInt == null)
                    this.PipeCompInt = this.map.PipeNet();
                return this.PipeCompInt;
            }
        }

        public override void MapComponentUpdate()
        {
            base.MapComponentUpdate();
            if (!this.MarkHeatingForDraw)
                return;
            foreach (PlumbingNet pipeNet in this.PipeComp.PipeNets)
            {
                foreach (HeatStore heatStore in pipeNet.HeatStores)
                    heatStore.DrawOverlay = true;
            }
            this.MarkHeatingForDraw = false;
        }

        public SteamPipeMapComp(Map map)
          : base(map)
        {
            if (SteamPipeMapComp.SteamPipeComps.ContainsKey(this.map.uniqueID))
                SteamPipeMapComp.SteamPipeComps[this.map.uniqueID] = this;
            else
                SteamPipeMapComp.SteamPipeComps.Add(this.map.uniqueID, this);
            SteamPipeMapComp.loccachecomp = (SteamPipeMapComp)null;
            int length = Enum.GetValues(typeof(PipeType)).Length;
            this.PipeGrid = new int[length, map.cellIndices.NumGridCells];
            this.DirtyPipe = new bool[length];
            for (int index = 0; index < this.DirtyPipe.Length; ++index)
                this.DirtyPipe[index] = true;
        }

        public override void MapRemoved()
        {
            SteamPipeMapComp.SteamPipeComps.Remove(this.map.uniqueID);
            base.MapRemoved();
            SteamPipeMapComp.loccachecomp = (SteamPipeMapComp)null;
        }

        public override void MapComponentTick()
        {
            int length = this.PipeNets.Length;
            for (int index = 0; index < length; ++index)
                this.PipeNets[index].Tick();
        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
            this.RegenGrids();
        }

        public override void MapGenerated()
        {
            base.MapGenerated();
            this.RegenGrids();
        }

        public void RegenGrids()
        {
            bool flag = false;
            for (int P = 0; P < this.DirtyPipe.Length; ++P)
            {
                if (this.DirtyPipe[P])
                {
                    flag = true;
                    this.RebuildPipeGrid(P);
                }
            }
            if (!flag)
                return;
            SteamPipeMapComp.RefreshInternetsOnTile(this.map.Tile);
        }

        public bool PerfectMatch(IntVec3 pos, PipeType P, int ID)
        {
            return this.PipeGrid[(int)P, this.map.cellIndices.CellToIndex(pos)] == ID;
        }

        public int IDAt(IntVec3 pos, PipeType P)
        {
            return this.PipeGrid[(int)P, this.map.cellIndices.CellToIndex(pos)];
        }

        public bool ZoneAt(IntVec3 pos, PipeType P)
        {
            return this.PipeGrid[(int)P, this.map.cellIndices.CellToIndex(pos)] > 0;
        }

        public void RegisterPipe(CompSteamPipe pipe, bool respawningAfterLoad)
        {
            if (!this.cachedPipes.Contains(pipe))
                this.cachedPipes.Add(pipe);
            this.DirtyPipeGrid(pipe.mode);
            if (respawningAfterLoad)
                return;
            this.RegenGrids();
        }

        public void DeregisterPipe(CompSteamPipe pipe)
        {
            pipe.pipeNet.PipedThings.Remove(pipe.parent);
            pipe.pipeNet.InitNet();
            if (this.cachedPipes.Contains(pipe))
                this.cachedPipes.Remove(pipe);
            foreach (IntVec3 c in pipe.parent.OccupiedRect())
                this.PipeGrid[(int)pipe.Props.mode, this.map.cellIndices.CellToIndex(c)] = -1;
            this.DirtyPipeGrid(pipe.mode);
            this.RegenGrids();
        }

        public void DirtyPipeGrid(PipeType p)
        {
            this.DirtyPipe[(int)p] = true;
        }

        public void DirtyAllPipeGrids()
        {
            for (int index = 0; index < this.DirtyPipe.Length; ++index)
                this.DirtyPipe[index] = true;
        }

        public void RebuildPipeGrid(int P)
        {
            this.DirtyPipe[P] = false;
            for (int index = 0; index < this.PipeGrid.GetLength(1); ++index)
                this.PipeGrid[P, index] = -1;
            List<PlumbingNet> list1 = ((IEnumerable<PlumbingNet>)this.PipeNets).Where<PlumbingNet>((Func<PlumbingNet, bool>)(x => x.NetType != P)).ToList<PlumbingNet>();
            List<CompSteamPipe> list2 = this.cachedPipes.Where<CompSteamPipe>((Func<CompSteamPipe, bool>)(x => x.mode == (PipeType)P)).ToList<CompSteamPipe>();
            foreach (CompSteamPipe compPipe in list2)
                compPipe.GridID = -1;
            this.pipeDic.Clear();
            foreach (CompSteamPipe compPipe in list2.Where<CompSteamPipe>((Func<CompSteamPipe, bool>)(x => !x.closed)))
            {
                foreach (IntVec3 key in compPipe.parent.OccupiedRect())
                {
                    try
                    {
                        this.pipeDic.Add(key, compPipe);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(string.Format("More than 1 pipe comp in the same cell at {0}, things with pipes cannot overlap!\n", (object)key) + ex?.ToString());
                    }
                }
            }
            for (CompSteamPipe compPipe = this.pipeDic.Values.FirstOrDefault<CompSteamPipe>((Func<CompSteamPipe, bool>)(k => k.mode == (PipeType)P && !k.closed && k.GridID == -1)); compPipe != null; compPipe = this.pipeDic.Values.FirstOrDefault<CompSteamPipe>((Func<CompSteamPipe, bool>)(k => k.mode == (PipeType)P && !k.closed && k.GridID == -1)))
            {
                PlumbingNet newNet = new PlumbingNet()
                {
                    MapComp = this,
                    NetID = this.masterID,
                    NetType = P
                };
                list1.Add(newNet);
                this.map.floodFiller.FloodFill(compPipe.parent.Position, new Predicate<IntVec3>(PassCheck), new Action<IntVec3>(Processor), int.MaxValue, false, (IEnumerable<IntVec3>)null);
                ++this.masterID;

                bool PassCheck(IntVec3 c)
                {
                    CompSteamPipe compPipe2 = this.pipeDic.TryGetValue<IntVec3, CompSteamPipe>(c, (CompSteamPipe)null);
                    if (compPipe2 == null)
                        return false;
                    compPipe2.GridID = this.masterID;
                    compPipe2.pipeNetRef = newNet;
                    if (!newNet.PipedThings.Contains(compPipe2.parent))
                        newNet.PipedThings.Add(compPipe2.parent);
                    newNet.cells.Add(c);
                    this.PipeGrid[P, this.map.cellIndices.CellToIndex(c)] = this.masterID;
                    return true;
                }
            }
            this.PipeNets = list1.ToArray();
            for (int index = 0; index < this.PipeNets.Length; ++index)
                this.PipeNets[index].InitNet();

            void Processor(IntVec3 c)
            {
            }
        }

        public static void RefreshInternetsOnTile(int tile)
        {
            if (Find.Maps.Count<Map>((Func<Map, bool>)(x => x.Tile == tile)) < 2)
                return;
            List<PlumbingNet> list = PlumbingNet.AllTileNets(tile).ToList<PlumbingNet>();
            foreach (PlumbingNet plumbingNet in list)
            {
                plumbingNet.IP = -1;
                plumbingNet.slave = false;
            }
            for (PlumbingNet net = list.FirstOrDefault<PlumbingNet>((Func<PlumbingNet, bool>)(x => x.IP == -1)); net != null; net = list.FirstOrDefault<PlumbingNet>((Func<PlumbingNet, bool>)(x => x.IP == -1)))
            {
                ++SteamPipeMapComp.MasterInternetID;
                net.IP = SteamPipeMapComp.MasterInternetID;
                net.slave = true;
                foreach (PlumbingNet plumbingNet1 in list)
                {
                    PlumbingNet overnet = plumbingNet1;
                    if (overnet != net && net.cells.Any<IntVec3>((Func<IntVec3, bool>)(cell => cell.InBounds(overnet.PipeComp.map) && overnet.PipeComp.PerfectMatch(cell, (PipeType)net.NetType, overnet.NetID))))
                    {
                        if (overnet.IP != -1)
                        {
                            foreach (PlumbingNet plumbingNet2 in list.Where<PlumbingNet>((Func<PlumbingNet, bool>)(x => x.IP == overnet.IP)))
                                plumbingNet2.IP = net.IP;
                        }
                        overnet.IP = net.IP;
                    }
                }
            }
            foreach (IGrouping<int, PlumbingNet> source in list.GroupBy<PlumbingNet, int>((Func<PlumbingNet, int>)(x => x.IP)))
            {
                PlumbingNet plumbingNet1 = source.First<PlumbingNet>();
                plumbingNet1.slave = false;
                foreach (PlumbingNet plumbingNet2 in (IEnumerable<PlumbingNet>)source)
                {
                    foreach (ThingWithComps pipedThing in plumbingNet2.PipedThings)
                        plumbingNet1.PipedThings.Add(pipedThing);
                }
                plumbingNet1.InitNet();
                foreach (PlumbingNet plumbingNet2 in (IEnumerable<PlumbingNet>)source)
                {
                    foreach (ThingWithComps pipedThing in plumbingNet1.PipedThings)
                        plumbingNet2.PipedThings.Add(pipedThing);
                    plumbingNet2.InitNet();
                }
            }
        }
    }
}
