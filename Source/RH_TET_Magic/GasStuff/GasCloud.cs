using HugsLib;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace TheEndTimes_Magic
{
    public class GasCloud : Verse.Thing
    {
        public static readonly Dictionary<System.Type, GasCloud.TraversibilityTest> TraversibleBuildings = new Dictionary<System.Type, GasCloud.TraversibilityTest>()
        {
          {
            typeof (Building_Vent),
            (GasCloud.TraversibilityTest) ((d, g) => true)
          },
          {
            typeof (Building_Door),
            (GasCloud.TraversibilityTest) ((d, g) => ((Building_Door) d).Open)
          }
        };
        private static readonly List<GasCloud> adjacentBuffer = new List<GasCloud>(4);
        private static readonly List<IntVec3> positionBuffer = new List<IntVec3>(4);
        public Vector2 spriteScaleMultiplier = new Vector2(1f, 1f);
        public float spriteAlpha = 1f;
        private const float AlphaEasingDivider = 10f;
        private const float SpreadingAnimationDuration = 1f;
        private const float DisplacingConcentrationFraction = 0.33f;
        private const int AvoidanceGridPathCost = 10;
        private static int GlobalOffsetCounter;
        public Vector2 spriteOffset;
        public float spriteRotation;
        public int relativeZOrder;
        private MoteProperties_GasCloud gasProps;
        private string cachedMouseoverLabel;
        private float mouseoverLabelCacheTime;
        private bool needsInitialFill;
        private float interpolatedAlpha;
        private readonly ValueInterpolator interpolatedOffsetX;
        private readonly ValueInterpolator interpolatedOffsetY;
        private readonly ValueInterpolator interpolatedScale;
        private readonly ValueInterpolator interpolatedRotation;
        private float concentration;
        protected int gasTicksProcessed;

        public float Concentration
        {
            get
            {
                return this.concentration;
            }
        }

        public bool IsBlocked
        {
            get
            {
                return !this.TileIsGasTraversible(this.Position, this.Map, this);
            }
        }

        public GasCloud()
        {
            this.interpolatedOffsetX = new ValueInterpolator();
            this.interpolatedOffsetY = new ValueInterpolator();
            this.interpolatedScale = new ValueInterpolator();
            this.interpolatedRotation = new ValueInterpolator();
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.gasProps = this.def.mote as MoteProperties_GasCloud;
            this.relativeZOrder = ++GasCloud.GlobalOffsetCounter % 80;
            if (this.gasProps == null)
                throw new Exception("Missing required gas mote properties in " + this.def.defName);
            this.interpolatedScale.value = this.GetRandomGasScale();
            this.interpolatedRotation.value = this.GetRandomGasRotation();
            HugsLibController.Instance.TickDelayScheduler.ScheduleCallback((Action)(() => HugsLibController.Instance.DistributedTicker.RegisterTickability(new Action(this.GasTick), this.gasProps.GastickInterval, (Verse.Thing)this)), 1, (Verse.Thing)this, false);
            PlayerAvoidanceGrids.AddAvoidanceSource((Verse.Thing)this, 10);
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            PlayerAvoidanceGrids.RemoveAvoidanceSource((Verse.Thing)this);
            base.DeSpawn(mode);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<float>(ref this.concentration, "concentration", 0.0f, false);
            Scribe_Values.Look<int>(ref this.gasTicksProcessed, "ticks", 0, false);
        }

        public override void Draw()
        {
            if (!Find.TickManager.Paused)
                this.UpdateInterpolatedValues();
            this.spriteAlpha = this.interpolatedAlpha = this.DoAdditiveEasing(this.interpolatedAlpha, Mathf.Min(1f, this.concentration / this.gasProps.FullAlphaConcentration), 10f, Time.deltaTime);
            this.spriteOffset = new Vector2((float)this.interpolatedOffsetX, (float)this.interpolatedOffsetY);
            this.spriteScaleMultiplier = new Vector2((float)this.interpolatedScale, (float)this.interpolatedScale);
            this.spriteRotation = (float)this.interpolatedRotation;
            base.Draw();
        }

        private void UpdateInterpolatedValues()
        {
            double num1 = (double)this.interpolatedOffsetX.Update();
            double num2 = (double)this.interpolatedOffsetY.Update();
            if ((double)this.gasProps.AnimationAmplitude <= 0.0)
                return;
            double num3 = (double)this.interpolatedScale.Update();
            double num4 = (double)this.interpolatedRotation.Update();
            if (this.interpolatedOffsetX.finished)
            {
                float finalValue1 = Rand.Range(-this.gasProps.AnimationAmplitude, this.gasProps.AnimationAmplitude);
                float finalValue2 = Rand.Range(-this.gasProps.AnimationAmplitude, this.gasProps.AnimationAmplitude);
                float randomInRange = this.gasProps.AnimationPeriod.RandomInRange;
                this.interpolatedOffsetX.StartInterpolation(finalValue1, randomInRange, CurveType.CubicInOut);
                this.interpolatedOffsetY.StartInterpolation(finalValue2, randomInRange, CurveType.CubicInOut);
            }
            if (this.interpolatedScale.finished)
                this.interpolatedScale.StartInterpolation(this.GetRandomGasScale(), this.gasProps.AnimationPeriod.RandomInRange, CurveType.CubicInOut);
            if (this.interpolatedRotation.finished)
                this.interpolatedRotation.StartInterpolation(this.interpolatedRotation.value + Rand.Range(-90f, 90f) * this.gasProps.AnimationAmplitude, this.gasProps.AnimationPeriod.RandomInRange, CurveType.CubicInOut);
        }

        public override string LabelMouseover
        {
            get
            {
                if (this.cachedMouseoverLabel == null || (double)this.mouseoverLabelCacheTime < (double)Time.realtimeSinceStartup - 0.5)
                {
                    float num = Mathf.Round(Mathf.Clamp01(this.Concentration / this.gasProps.FullAlphaConcentration) * 100f);
                    this.cachedMouseoverLabel = (double)this.concentration < 1000.0 ? (string)"RH_TET_Magic_PoisonGasCloud_statusReadout_low".Translate((NamedArgument)this.LabelCap, (NamedArgument)Mathf.Round(this.concentration), (NamedArgument)num) : (string)"RH_TET_Magic_PoisonGasCloud_statusReadout_high".Translate((NamedArgument)this.LabelCap, (NamedArgument)Math.Round((double)this.concentration / 1000.0, 1), (NamedArgument)num);
                    this.mouseoverLabelCacheTime = Time.realtimeSinceStartup;
                }
                return this.cachedMouseoverLabel;
            }
        }

        public void ReceiveConcentration(float amount)
        {
            this.concentration += amount;
            if ((double)this.concentration >= 0.0)
                return;
            this.concentration = 0.0f;
        }

        public void BeginSpreadingTransition(GasCloud parentCloud, IntVec3 targetPosition)
        {
            this.interpolatedOffsetX.value = (float)(parentCloud.Position.x - targetPosition.x);
            this.interpolatedOffsetY.value = (float)(parentCloud.Position.z - targetPosition.z);
            this.interpolatedOffsetX.StartInterpolation(0.0f, 1f, CurveType.QuinticOut);
            this.interpolatedOffsetY.StartInterpolation(0.0f, 1f, CurveType.QuinticOut);
        }

        protected virtual void GasTick()
        {
            if (!this.Spawned)
                return;
            ++this.gasTicksProcessed;
            this.concentration -= this.Map.roofGrid.Roofed(this.Position) ? this.gasProps.RoofedDissipation : this.gasProps.UnroofedDissipation;
            if ((double)this.concentration <= 0.0)
            {
                this.Destroy(DestroyMode.KillFinalize);
            }
            else
            {
                if (this.gasTicksProcessed % this.gasProps.SpreadInterval == 0)
                    this.TryCreateNewNeighbors();
                if (this.IsBlocked)
                    this.ForcePushConcentrationToNeighbors();
                this.ShareConcentrationWithMinorNeighbors();
            }
        }

        private float GetRandomGasScale()
        {
            return 1f + Rand.Range(-this.gasProps.AnimationAmplitude, this.gasProps.AnimationAmplitude);
        }

        private float GetRandomGasRotation()
        {
            return Rand.Value * 360f;
        }

        private float DoAdditiveEasing(
          float currentValue,
          float targetValue,
          float easingDivider,
          float frameDeltaTime)
        {
            float num1 = (double)frameDeltaTime < 1.40129846432482E-45 ? 0.0f : 0.01666667f / frameDeltaTime;
            easingDivider *= num1;
            if ((double)easingDivider < 1.0)
                easingDivider = 1f;
            float num2 = (targetValue - currentValue) / easingDivider;
            return currentValue + num2;
        }

        private List<IntVec3> GetSpreadableAdjacentCells()
        {
            GasCloud.positionBuffer.Clear();
            for (int index1 = 0; index1 < 4; ++index1)
            {
                IntVec3 intVec3 = GenAdj.CardinalDirections[index1] + this.Position;
                if (this.TileIsGasTraversible(intVec3, this.Map, this))
                {
                    List<Verse.Thing> thingList = this.Map.thingGrid.ThingsListAtFast(intVec3);
                    bool flag = false;
                    for (int index2 = 0; index2 < thingList.Count; ++index2)
                    {
                        if (thingList[index2] is GasCloud gasCloud && (gasCloud.def == this.def || (double)gasCloud.concentration > (double)this.concentration * 0.330000013113022))
                            flag = true;
                    }
                    if (!flag)
                        GasCloud.positionBuffer.Add(intVec3);
                }
            }
            GasCloud.positionBuffer.Shuffle<IntVec3>();
            return GasCloud.positionBuffer;
        }

        private List<GasCloud> GetAdjacentGasCloudsOfSameDef()
        {
            GasCloud.adjacentBuffer.Clear();
            for (int index = 0; index < 4; ++index)
            {
                IntVec3 c = GenAdj.CardinalDirections[index] + this.Position;
                if (c.InBounds(this.Map) && c.GetFirstThing(this.Map, this.def) is GasCloud firstThing)
                    GasCloud.adjacentBuffer.Add(firstThing);
            }
            return GasCloud.adjacentBuffer;
        }

        private void ShareConcentrationWithMinorNeighbors()
        {
            List<GasCloud> gasCloudsOfSameDef = this.GetAdjacentGasCloudsOfSameDef();
            int num = 0;
            for (int index = 0; index < gasCloudsOfSameDef.Count; ++index)
            {
                GasCloud gasCloud = gasCloudsOfSameDef[index];
                if ((double)gasCloud.Concentration >= (double)this.concentration || !gasCloud.needsInitialFill && gasCloud.IsBlocked)
                    gasCloudsOfSameDef[index] = (GasCloud)null;
                else
                    ++num;
            }
            if (num <= 0)
                return;
            for (int index = 0; index < gasCloudsOfSameDef.Count; ++index)
            {
                GasCloud gasCloud = gasCloudsOfSameDef[index];
                if (gasCloud != null)
                {
                    float amount = (this.concentration - ((double)gasCloud.concentration > 0.0 ? gasCloud.Concentration : 1f)) / (float)(num + 1) * this.gasProps.SpreadAmountMultiplier;
                    gasCloud.ReceiveConcentration(amount);
                    gasCloud.needsInitialFill = false;
                    this.concentration -= amount;
                }
            }
        }

        private void ForcePushConcentrationToNeighbors()
        {
            List<GasCloud> gasCloudsOfSameDef = this.GetAdjacentGasCloudsOfSameDef();
            for (int index = 0; index < gasCloudsOfSameDef.Count; ++index)
            {
                GasCloud gasCloud = gasCloudsOfSameDef[index];
                if (!gasCloud.IsBlocked)
                {
                    float amount = this.concentration / (float)gasCloudsOfSameDef.Count;
                    gasCloud.ReceiveConcentration(amount);
                    this.concentration -= amount;
                }
            }
        }

        private void TryCreateNewNeighbors()
        {
            int num = Mathf.FloorToInt(this.concentration / this.gasProps.SpreadMinConcentration);
            if (num <= 0)
                return;
            List<IntVec3> spreadableAdjacentCells = this.GetSpreadableAdjacentCells();
            for (int index = 0; index < spreadableAdjacentCells.Count && num > 0; ++index)
            {
                IntVec3 intVec3 = spreadableAdjacentCells[index];
                GasCloud gasCloud = (GasCloud)ThingMaker.MakeThing(this.def, (ThingDef)null);
                gasCloud.needsInitialFill = true;
                gasCloud.BeginSpreadingTransition(this, intVec3);
                GenPlace.TryPlaceThing((Verse.Thing)gasCloud, intVec3, this.Map, ThingPlaceMode.Direct, (Action<Verse.Thing, int>)null, (Predicate<IntVec3>)null, new Rot4());
                --num;
            }
        }

        private bool TileIsGasTraversible(IntVec3 pos, Map map, GasCloud sourceCloud)
        {
            Thing thing1 = (Thing) map.edificeGrid.InnerArray[CellIndicesUtility.CellToIndex(pos, map.Size.x)];

            if (!pos.InBounds(map) || (thing1 != null && thing1.def != null && thing1.def.fillPercent > .8f))
                return false;
            List<Verse.Thing> thingList = map.thingGrid.ThingsListAtFast(pos);
            for (int index = 0; index < thingList.Count; ++index)
            {
                Verse.Thing thing = thingList[index];
                if (thing is Building b)
                {
                    GasCloud.TraversibilityTest traversibilityTest;
                    GasCloud.TraversibleBuildings.TryGetValue(b.GetType(), out traversibilityTest);
                    if (traversibilityTest != null && !traversibilityTest(b, sourceCloud))
                        return false;
                }
                if (thing is GasCloud gasCloud && gasCloud.def != sourceCloud.def && (double)sourceCloud.concentration < (double)gasCloud.concentration)
                    return false;
            }
            return true;
        }

        public delegate bool TraversibilityTest(Building b, GasCloud g);

    }
}
