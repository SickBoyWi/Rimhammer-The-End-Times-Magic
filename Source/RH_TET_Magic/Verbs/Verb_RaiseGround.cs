using SickAbilityUser;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    public class Verb_RaiseGround : Verb_UseAbility
    {
        public const float RANGE = 4;

        protected override bool TryCastShot()
        {
            // Miscast?
            if (MagicUtility.TryMiscast(this.caster))
                return true;
            
            bool flag1 = false;
            Pawn theCaster = caster as Pawn;

            //this.TargetsAoE.Clear();

            //if (this.UseAbilityProps.AbilityTargetCategory != AbilityTargetCategory.TargetAoE && this.TargetsAoE.Count > 1)
            //    ((List<LocalTargetInfo>)this.TargetsAoE).RemoveRange(0, this.TargetsAoE.Count - 1);

            MagicPower magicPower = null;
            foreach (MagicPower mp in this.CasterPawn.GetComp<CompMagicUser>().MagicData.PowersAddons)
            {
                if (mp.abilityDef.defName.Contains("RaiseGround"))
                {
                    magicPower = mp;
                }
            }

            if (magicPower == null)
            {
                Log.Error("Someone tried to cast Raise Ground, but didn't have the spell.");
                return false;
            }

            //if (this.TargetsAoE.Count <= 0)
            //Messages.Message("RH_TET_MessageSpellFailTargets".Translate(theCaster.Name), MessageTypeDefOf.NegativeEvent);

            //int maxTargets = this.UseAbilityProps.TargetAoEProperties.maxTargets;
            Map map = theCaster.Map;
            LocalTargetInfo target = CurrentTarget;
            IntVec3 targetCell = target.Cell;

            FleckMaker.Static(theCaster.Position, map, RH_TET_MagicDefOf.RH_TET_FleckGoldEffect, 2F);
            FleckMaker.Static(theCaster.Position, map, RH_TET_MagicDefOf.RH_TET_FleckBlueEffect, 1F);
            FleckMaker.Static(theCaster.Position, map, FleckDefOf.PsycastAreaEffect, 1.2f);

            //float range = (float)((TargetAoEProperties)this.UseAbilityProps.TargetAoEProperties).range;
            float range = RANGE;

            IEnumerable<IntVec3> areaAround = GenRadial.RadialCellsAround(targetCell, range, true);
            FleckMaker.Static(targetCell, map, RH_TET_MagicDefOf.RH_TET_FleckGoldEffect, 4F);
            FleckMaker.Static(targetCell, map, RH_TET_MagicDefOf.RH_TET_FleckBlueEffect, 2F);
            FleckMaker.Static(targetCell, map, FleckDefOf.PsycastAreaEffect, 2f);

            foreach (IntVec3 cell in areaAround)
            {
                Verb_RaiseGround.AffectCell(map, cell);
            }

            this.PostCastShot(flag1, out flag1);
            return flag1;
        }

        private static TerrainDef GetTerrainToDryTo(Map map, TerrainDef terrainDef)
        {
            TerrainDef retDef = null;

            // JEH Exceptions
            if (terrainDef.Equals(TerrainDefOf.SoilRich))
                retDef = TerrainDefOf.SoilRich;

            retDef = map.Biome == BiomeDefOf.SeaIce ? TerrainDefOf.Ice : terrainDef.driesTo;

            if (retDef != null)
                return retDef;

            // JEH Exceptions
            if (terrainDef.Equals(TerrainDefOf.SoilRich))
                retDef = TerrainDefOf.SoilRich;
            else if (terrainDef.Equals(TerrainDefOf.WaterDeep))
                retDef = TerrainDefOf.WaterShallow;
            else if (terrainDef.Equals(TerrainDefOf.WaterMovingChestDeep))
                retDef = TerrainDefOf.WaterMovingShallow;
            else if (terrainDef.Equals(TerrainDefOf.WaterOceanDeep))
                retDef = TerrainDefOf.WaterOceanShallow;
            else if (terrainDef.Equals(TerrainDefOf.WaterMovingShallow))
                retDef = TerrainDefOf.SoilRich;
            else if (terrainDef.Equals(TerrainDefOf.WaterOceanShallow))
                retDef = DefDatabase<TerrainDef>.GetNamed("SoftSand"); //TerrainDefOf.SoftSand;

            return retDef;
        }

        public override float HighlightFieldRadiusAroundTarget(out bool needLOSToCenter)
        {
            needLOSToCenter = true;
            return RANGE;
            //var targetAoEProperties = UseAbilityProps.abilityDef.MainVerb.TargetAoEProperties;
            //return targetAoEProperties?.showRangeOnSelect ?? false
            //    ? targetAoEProperties.range
            //    : verbProps.defaultProjectile?.projectile?.explosionRadius ?? 1;
        }

        public static void AffectCell(Map map, IntVec3 c)
        {
            TerrainDef terrain = c.GetTerrain(map);
            TerrainDef terrainToDryTo1 = Verb_RaiseGround.GetTerrainToDryTo(map, terrain);

            if (terrainToDryTo1 != null)
                map.terrainGrid.SetTerrain(c, terrainToDryTo1);

            TerrainDef terrainDef = map.terrainGrid.UnderTerrainAt(c);

            if (terrainDef == null)
                return;
            TerrainDef terrainToDryTo2 = Verb_RaiseGround.GetTerrainToDryTo(map, terrainDef);

            if (terrainToDryTo2 == null)
                return;

            map.terrainGrid.SetUnderTerrain(c, terrainToDryTo2);
        }

        public Verb_RaiseGround()
            : base()
        {
        }
    }
}
