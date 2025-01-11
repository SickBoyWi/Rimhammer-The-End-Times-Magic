using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    public class RH_TET_MagicMod : Mod
    {
        public static System.Random random = new System.Random(Guid.NewGuid().GetHashCode());
        public static bool safeToRemoveMap = true;
        private static List<Pawn> activeCasters = new List<Pawn>();

        public RH_TET_MagicMod(ModContentPack content) : base(content)
        {

        }

        public static bool GetSafeToRemoveMapNow()
        {
            return RH_TET_MagicMod.safeToRemoveMap;
        }

        public static bool SetSafeToRemoveMapNow(bool safetyInd)
        {
            RH_TET_MagicMod.safeToRemoveMap = safetyInd;
            return true;
        }

        public static bool IsActiveCasterWillAdd(Pawn p)
        {
            if (activeCasters.Contains(p))
                return true;

            activeCasters.Add(p);
            return false;
        }

        public static void RemoveActiveCaster(Pawn p)
        {
            activeCasters.Remove(p);
        }

        public static void ClearActiveCasters()
        {
            activeCasters.Clear();
        }
    }
}