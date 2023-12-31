using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    public static class PlayerAvoidanceGrids
    {
        private static readonly List<PlayerAvoidanceGrids.PlayerAvoidanceGrid> grids = new List<PlayerAvoidanceGrids.PlayerAvoidanceGrid>();

        public static void AddAvoidanceSource(Verse.Thing source, int pathCost)
        {
            PlayerAvoidanceGrids.AssertMap(source);
            pathCost = Mathf.Max(0, pathCost);
            PlayerAvoidanceGrids.PlayerAvoidanceGrid grid;
            if (!PlayerAvoidanceGrids.TryGetGridForMap(source.Map.uniqueID, out grid))
            {
                grid = new PlayerAvoidanceGrids.PlayerAvoidanceGrid(source.Map);
                PlayerAvoidanceGrids.grids.Add(grid);
            }
            int index = CellIndicesUtility.CellToIndex(source.Position, source.Map.Size.x);
            int pathCostInCell = PlayerAvoidanceGrids.CalculatePathCostInCell(grid, index);
            int num = Mathf.Min((int)byte.MaxValue, pathCostInCell + pathCost);
            grid.byteGrid[index] = (byte)num;
            grid.sources.Add(new PlayerAvoidanceGrids.AvoidanceSource(source.thingIDNumber, index, num - pathCostInCell));
        }

        public static void RemoveAvoidanceSource(Verse.Thing source)
        {
            PlayerAvoidanceGrids.AssertMap(source);
            PlayerAvoidanceGrids.PlayerAvoidanceGrid grid;
            if (!PlayerAvoidanceGrids.TryGetGridForMap(source.Map.uniqueID, out grid))
                return;
            int thingIdNumber = source.thingIDNumber;
            List<PlayerAvoidanceGrids.AvoidanceSource> sources = grid.sources;
            for (int index = sources.Count - 1; index >= 0; --index)
            {
                if (sources[index].thingId == thingIdNumber)
                    sources.RemoveAt(index);
            }
            if (sources.Count == 0)
            {
                PlayerAvoidanceGrids.DiscardMap(source.Map);
            }
            else
            {
                int index = CellIndicesUtility.CellToIndex(source.Position, source.Map.Size.x);
                grid.byteGrid[index] = (byte)PlayerAvoidanceGrids.CalculatePathCostInCell(grid, index);
            }
        }

        public static ByteGrid TryGetByteGridForMap(Map map)
        {
            PlayerAvoidanceGrids.PlayerAvoidanceGrid grid;
            return map == null || !PlayerAvoidanceGrids.TryGetGridForMap(map.uniqueID, out grid) ? (ByteGrid)null : grid.byteGrid;
        }

        public static bool ShouldAvoidCell(Map map, IntVec3 cell)
        {
            PlayerAvoidanceGrids.PlayerAvoidanceGrid grid;
            if (map == null || !cell.InBounds(map) || !PlayerAvoidanceGrids.TryGetGridForMap(map.uniqueID, out grid))
                return false;
            int index = CellIndicesUtility.CellToIndex(cell, map.Size.x);
            return grid.byteGrid[index] > (byte)0;
        }

        public static bool PawnShouldAvoidCell(Pawn pawn, IntVec3 cell)
        {
            return PlayerAvoidanceGrids.PawnHasPlayerAvoidanceGridKnowledge(pawn) && PlayerAvoidanceGrids.ShouldAvoidCell(pawn.Map, cell);
        }

        public static void ClearAllMaps()
        {
            PlayerAvoidanceGrids.grids.Clear();
        }

        public static void DiscardMap(Map map)
        {
            if (map == null)
                throw new ArgumentNullException(nameof(map));
            for (int index = PlayerAvoidanceGrids.grids.Count - 1; index >= 0; --index)
            {
                if (PlayerAvoidanceGrids.grids[index].mapId == map.uniqueID)
                    PlayerAvoidanceGrids.grids.RemoveAt(index);
            }
        }

        public static bool PawnHasPlayerAvoidanceGridKnowledge(Pawn p)
        {
            if (p == null)
                return false;
            Faction ofPlayer = Faction.OfPlayer;
            return p.Faction != null && !p.Faction.HostileTo(ofPlayer) || p.guest != null && p.guest.Released || p.IsPrisoner && p.HostFaction == ofPlayer || p.Faction == null && p.RaceProps.Humanlike;
        }

        private static bool TryGetGridForMap(
          int mapId,
          out PlayerAvoidanceGrids.PlayerAvoidanceGrid grid)
        {
            for (int index = 0; index < PlayerAvoidanceGrids.grids.Count; ++index)
            {
                if (PlayerAvoidanceGrids.grids[index].mapId == mapId)
                {
                    grid = PlayerAvoidanceGrids.grids[index];
                    return true;
                }
            }
            grid = new PlayerAvoidanceGrids.PlayerAvoidanceGrid();
            return false;
        }

        private static void AssertMap(Verse.Thing source)
        {
            if (source.Map == null)
                throw new ArgumentException("Source thing does not belong to a map: " + source?.ToString());
        }

        private static int CalculatePathCostInCell(
          PlayerAvoidanceGrids.PlayerAvoidanceGrid grid,
          int cellIndex)
        {
            List<PlayerAvoidanceGrids.AvoidanceSource> sources = grid.sources;
            int num = 0;
            for (int index = 0; index < sources.Count; ++index)
            {
                if (sources[index].cellIndex == cellIndex)
                    num += sources[index].addedCost;
            }
            return num;
        }

        private struct PlayerAvoidanceGrid
        {
            public readonly int mapId;
            public readonly ByteGrid byteGrid;
            public readonly List<PlayerAvoidanceGrids.AvoidanceSource> sources;

            public PlayerAvoidanceGrid(Map map)
            {
                this.mapId = map.uniqueID;
                this.byteGrid = new ByteGrid(map);
                this.sources = new List<PlayerAvoidanceGrids.AvoidanceSource>();
            }
        }

        private struct AvoidanceSource
        {
            public readonly int thingId;
            public readonly int cellIndex;
            public readonly int addedCost;

            public AvoidanceSource(int thingId, int cellIndex, int addedCost)
            {
                this.thingId = thingId;
                this.cellIndex = cellIndex;
                this.addedCost = addedCost;
            }
        }
    }
}
