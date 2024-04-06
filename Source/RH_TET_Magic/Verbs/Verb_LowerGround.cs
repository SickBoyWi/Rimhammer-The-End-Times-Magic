using SickAbilityUser;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    public class Verb_LowerGround : Verb_UseAbility
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
                if (mp.abilityDef.defName.Contains("LowerGround"))
                {
                    magicPower = mp;
                }
            }

            if (magicPower == null)
            {
                Log.Error("Someone tried to cast Lower Ground, but didn't have the spell.");
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
                Verb_LowerGround.AffectCell(map, cell);
            }

            this.PostCastShot(flag1, out flag1);
            return flag1;
        }

        private static TerrainDef GetTerrainToLowerTo(Map map, TerrainDef terrainDef)
        {
            TerrainDef retDef = null;

            if (terrainDef.Equals(TerrainDefOf.WaterDeep) || terrainDef.Equals(TerrainDefOf.WaterMovingChestDeep) || terrainDef.Equals(TerrainDefOf.WaterOceanDeep))
            {
                return retDef;
            }
            else if (terrainDef.Equals(TerrainDefOf.WaterMovingShallow))
            {
                retDef = map.Biome == BiomeDefOf.SeaIce ? TerrainDefOf.Ice : TerrainDefOf.WaterMovingChestDeep;
                return retDef;
            }
            else if (terrainDef.Equals(TerrainDefOf.WaterShallow))
            {
                retDef = map.Biome == BiomeDefOf.SeaIce ? TerrainDefOf.Ice : TerrainDefOf.WaterDeep;
                return retDef;
            }
            else if (terrainDef.Equals(TerrainDefOf.WaterOceanShallow))
            {
                retDef = map.Biome == BiomeDefOf.SeaIce ? TerrainDefOf.Ice : TerrainDefOf.WaterOceanDeep;
                return retDef;
            }
            else if (terrainDef.IsSoil || terrainDef.Equals(TerrainDefOf.FungalGravel) || terrainDef.Equals(TerrainDefOf.Sand) || terrainDef.Equals(DefDatabase<TerrainDef>.GetNamed("SoftSand")) || terrainDef.defName.Contains("_Smooth"))
            {
                retDef = map.Biome == BiomeDefOf.SeaIce ? TerrainDefOf.Ice : TerrainDefOf.WaterShallow;
                return retDef;
            }
            else if (terrainDef.layerable)
            {
                return retDef;
            }
            else
            {
                retDef = map.Biome == BiomeDefOf.SeaIce ? TerrainDefOf.Ice : TerrainDefOf.WaterShallow;
                return retDef;
            }
        }

        public override float HighlightFieldRadiusAroundTarget(out bool needLOSToCenter)
        {
            needLOSToCenter = true;
            return RANGE;
        }

        public static void AffectCell(Map map, IntVec3 c)
        {

            TerrainDef terrainDef = c.GetTerrain(map);

            // Remove buildings or edifices at the location.
            List<Thing> thingList = map.thingGrid.ThingsListAt(c);
            List<Thing> destroyTheseThings = new List<Thing>();
            foreach (Thing t in thingList)
            {
                if (t.def.IsBuildingArtificial)
                {
                    destroyTheseThings.Add(t);
                }
                else if (t.def.IsEdifice())
                {
                    destroyTheseThings.Add(t);
                }
                else if (t.def.plant != null)
                {
                    destroyTheseThings.Add(t);
                }
            }

            foreach(Thing t in destroyTheseThings)
            {
                if (t.def.IsBuildingArtificial)
                {
                    t.Destroy(DestroyMode.Deconstruct);
                }
                else if (t.def.IsEdifice())
                {
                    t.Destroy(DestroyMode.KillFinalize);
                }
                else if (t.def.plant != null)
                {
                    t.Destroy(DestroyMode.Vanish);
                }
            }

            if (terrainDef.layerable) // Is carpet, tile, concrete, road, or other player constructed.
            {
                map.terrainGrid.RemoveTopLayer(c, true);

                terrainDef = c.GetTerrain(map);
            }

            TerrainDef terrainToLowerTo = Verb_LowerGround.GetTerrainToLowerTo(map, terrainDef);

            if (terrainToLowerTo != null)
                map.terrainGrid.SetTerrain(c, terrainToLowerTo);

        }

        public Verb_LowerGround()
            : base()
        {
        }
    }
}
