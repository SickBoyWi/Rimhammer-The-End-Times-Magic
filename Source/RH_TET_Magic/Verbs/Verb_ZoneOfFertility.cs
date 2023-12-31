using AbilityUser;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    public class Verb_ZoneOfFertility : Verb_UseAbility
    {
        public const float RANGE = 3;

        protected override bool TryCastShot()
        {
            // Miscast?
            if (MagicUtility.TryMiscast(this.caster))
                return true;
            
            bool flag1 = false;
            Pawn theCaster = caster as Pawn;

            MagicPower magicPower = null;
            foreach (MagicPower mp in this.CasterPawn.GetComp<CompMagicUser>().MagicData.PowersAddons)
            {
                if (mp.abilityDef.defName.Contains("ZoneOfFertility"))
                {
                    magicPower = mp;
                }
            }

            if (magicPower == null)
            {
                Log.Error("Someone tried to cast Zone of Fertility, but didn't have the spell.");
                return false;
            }

            Map map = theCaster.Map;
            LocalTargetInfo target = CurrentTarget;
            IntVec3 targetCell = target.Cell;

            FleckMaker.Static(theCaster.Position, map, RH_TET_MagicDefOf.RH_TET_FleckGreenEffect, 2F);
            FleckMaker.Static(theCaster.Position, map, RH_TET_MagicDefOf.RH_TET_FleckGoldEffect, 1F);
            FleckMaker.Static(theCaster.Position, map, FleckDefOf.PsycastAreaEffect, 1.2f);

            float range = RANGE;

            IEnumerable<IntVec3> areaAround = GenRadial.RadialCellsAround(targetCell, range, true);
            FleckMaker.Static(targetCell, map, RH_TET_MagicDefOf.RH_TET_FleckGreenEffect, 4F);
            FleckMaker.Static(targetCell, map, RH_TET_MagicDefOf.RH_TET_FleckGoldEffect, 2F);
            FleckMaker.Static(targetCell, map, FleckDefOf.PsycastAreaEffect, 2f);

            foreach (IntVec3 cell in areaAround)
            {
                Verb_ZoneOfFertility.AffectCell(map, cell);
            }

            this.PostCastShot(flag1, out flag1);
            return flag1;
        }

        private static TerrainDef GetTerrainToChangeTo(Map map, TerrainDef terrainDef)
        {
            TerrainDef retDef = null;

            // JEH Exceptions
            if (terrainDef.Equals(TerrainDefOf.SoilRich))
                retDef = null;
            else if (terrainDef.IsSoil || terrainDef.Equals(TerrainDefOf.Sand))
                retDef = TerrainDefOf.SoilRich;
            else if (terrainDef.fertility > .25F)
                retDef = TerrainDefOf.SoilRich;

            return retDef;
        }

        public override float HighlightFieldRadiusAroundTarget(out bool needLOSToCenter)
        {
            needLOSToCenter = true;
            return RANGE;
        }

        public static void AffectCell(Map map, IntVec3 c)
        {
            TerrainDef terrain = c.GetTerrain(map);
            TerrainDef terrainToChangeTo1 = Verb_ZoneOfFertility.GetTerrainToChangeTo(map, terrain);

            if (terrainToChangeTo1 != null)
                map.terrainGrid.SetTerrain(c, terrainToChangeTo1);

            TerrainDef terrainDef = map.terrainGrid.UnderTerrainAt(c);
            if (terrainDef == null)
                return;

            TerrainDef terrainToChangeTo2 = Verb_ZoneOfFertility.GetTerrainToChangeTo(map, terrainDef);
            if (terrainToChangeTo2 == null)
                return;
            map.terrainGrid.SetUnderTerrain(c, terrainToChangeTo2);
        }

        public Verb_ZoneOfFertility()
            : base()
        {
        }
    }
}
