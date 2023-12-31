using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    /**
     * Original code for this came from Dubs Central Heating Mod, and was
     * trimmed down to suit this mod's needs.
     */
    public class PlumbingNet
    {
        public HashSet<ThingWithComps> PipedThings = new HashSet<ThingWithComps>();
        public HashSet<IntVec3> cells = new HashSet<IntVec3>();
        public int IP = -1;
        public List<CompThermostat> thermostats = new List<CompThermostat>();
        public List<CompBoiler> Boilers = new List<CompBoiler>();
        public List<CompRadiator> Radiators = new List<CompRadiator>();
        public List<HeatStore> HeatStores = new List<HeatStore>();
        public List<HeatStore> HotWaterTanks = new List<HeatStore>();
        public List<CompSteamPipe> Pipes = new List<CompSteamPipe>();
        public List<CompSteamUser> SteamUsers = new List<CompSteamUser>();
        public int NetType;
        public int NetID;
        public bool slave;
        public float HeatingCap;
        public float CoolingCap;
        public float BoilerCapacitySum;
        public float HeatStoreCapacitySum;
        public float OutdoorUnitCapacitySum;
        public float IndoorUnitCapacitySum;
        public SteamPipeMapComp PipeComp;
        public SteamPipeMapComp MapComp;
        public bool dirty;

        public static IEnumerable<PlumbingNet> AllTileNets(int Tile)
        {
            if (SteamPipeMapComp.SteamPipeComps == null)
            {
                Log.Warning("Null SteamPipeComps - correcting");
                SteamPipeMapComp.SteamPipeComps = new Dictionary<int, SteamPipeMapComp>();
            }
            foreach (KeyValuePair<int, SteamPipeMapComp> steamPipeComp in SteamPipeMapComp.SteamPipeComps)
            {
                if (steamPipeComp.Value?.map != null && (steamPipeComp.Value?.PipeNets != null && steamPipeComp.Value.map.Tile == Tile))
                {
                    PlumbingNet[] plumbingNetArray = steamPipeComp.Value.PipeNets;
                    for (int index = 0; index < plumbingNetArray.Length; ++index)
                    {
                        PlumbingNet plumbingNet = plumbingNetArray[index];
                        if (plumbingNet != null)
                            yield return plumbingNet;
                    }
                    plumbingNetArray = (PlumbingNet[])null;
                }
            }
        }

        public void InitNet()
        {
            this.Pipes.Clear();
            this.Boilers.Clear();
            this.Radiators.Clear();
            this.HeatStores.Clear();
            this.thermostats.Clear();
            this.HotWaterTanks.Clear();
            this.SteamUsers.Clear();
            foreach (ThingWithComps pipedThing in this.PipedThings)
            {
                if (pipedThing is Building_SteamPipe)
                    this.Pipes.AddRange(pipedThing.GetComps<CompSteamPipe>());
                foreach (ThingComp allComp in pipedThing.AllComps)
                {
                    if (allComp is CompBoiler compBoiler)
                        this.Boilers.Add(compBoiler);
                    if (allComp is CompRadiator compRadiator)
                        this.Radiators.Add(compRadiator);
                    if (allComp is HeatStore heatStore)
                    {
                        this.HeatStores.Add(heatStore);
                    }
                    if (allComp is CompThermostat compThermostat)
                        this.thermostats.Add(compThermostat);
                    if (allComp is CompSteamUser compSteamUser)
                        this.SteamUsers.Add(compSteamUser);
                }
            }
        }

        public bool SteamPresent()
        {
            return !this.HotWaterTanks.NullOrEmpty<HeatStore>() && (!this.HotWaterTanks.Any<HeatStore>((Predicate<HeatStore>)(x => x.ThermostatControlled)) || this.HotWaterTanks.Any<HeatStore>((Predicate<HeatStore>)(x => x.ThermostatControlled && x.LowCap)));
        }

        public bool ThermostatPresent()
        {
            return this.thermostats.NullOrEmpty<CompThermostat>() && this.HotWaterTanks.NullOrEmpty<HeatStore>() || this.thermostats.NullOrEmpty<CompThermostat>() && !this.HotWaterTanks.Any<HeatStore>((Predicate<HeatStore>)(x => x.ThermostatControlled)) || this.thermostats.Any<CompThermostat>((Predicate<CompThermostat>)(x => x.requestHeating));
        }

        public void Tick()
        {
            if (this.dirty)
            {
                this.dirty = false;
                this.InitNet();
            }
            if (this.slave)
                return;
            this.TickHeating();
            this.TickAircon();
        }

        private void TickAircon()
        {
            this.CoolingCap = 0.0f;
            if ((double)this.IndoorUnitCapacitySum <= 0.0)
                return;
            this.CoolingCap = Mathf.Clamp01(1f - (this.IndoorUnitCapacitySum - this.OutdoorUnitCapacitySum) / this.IndoorUnitCapacitySum);
        }

        private void TickHeating()
        {
            this.BoilerCapacitySum = this.Boilers.Sum<CompBoiler>((Func<CompBoiler, float>)(x => x.Capacity));
            this.HeatStoreCapacitySum = this.HeatStores.Sum<HeatStore>((Func<HeatStore, float>)(x => x.GetStoreCapacity));
            this.HeatingCap = 0.0f;
            if ((double)this.HeatStoreCapacitySum <= 0.0)
                return;
            this.HeatingCap = Mathf.Clamp01(1f - (this.HeatStoreCapacitySum - this.BoilerCapacitySum) / this.HeatStoreCapacitySum);
        }
    }
}
