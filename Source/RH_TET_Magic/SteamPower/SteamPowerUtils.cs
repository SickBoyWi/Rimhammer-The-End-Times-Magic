using RimWorld.Planet;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    /**
     * Original code for this came from Dubs Central Heating Mod, and was
     * trimmed down to suit this mod's needs.
     */
    public static class SteamPowerUtils
    {
        public static SteamPipeMapComp PipeNet(this Map map)
        {
            if (SteamPipeMapComp.loccachecomp != null && SteamPipeMapComp.loccachecomp.map.uniqueID == map.uniqueID)
                return SteamPipeMapComp.loccachecomp;
            SteamPipeMapComp steamPipeMapComp;
            SteamPipeMapComp.loccachecomp = !SteamPipeMapComp.SteamPipeComps.TryGetValue(map.uniqueID, out steamPipeMapComp) ? map.GetComponent<SteamPipeMapComp>() : steamPipeMapComp;
            return SteamPipeMapComp.loccachecomp;
        }
    }
}
