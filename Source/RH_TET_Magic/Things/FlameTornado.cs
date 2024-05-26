using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Noise;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    [StaticConstructorOnStartup]
    public class FlameTornado : ThingWithComps
    {
        private static MaterialPropertyBlock matPropertyBlock = new MaterialPropertyBlock();
        private static readonly IntRange DurationTicks = new IntRange(2700, 10080);
        private static readonly Material TornadoMaterial = MaterialPool.MatFrom("Magic/RH_TET_FireFlameTornado", ShaderDatabase.Transparent, MapMaterialRenderQueues.Tornado);
        private static readonly FloatRange PartsDistanceFromCenter = new FloatRange(1f, 10f);
        private static readonly float ZOffsetBias = -4f * FlameTornado.PartsDistanceFromCenter.min;
        private static List<Thing> tmpThings = new List<Thing>();
        private int leftFadeOutTicks = -1;
        private int ticksLeftToDisappear = -1;
        private List<IntVec3> removedRoofsTmp = new List<IntVec3>();
        private Vector2 realPosition;
        private float direction;
        private int spawnTick;
        private Sustainer sustainer;
        private static ModuleBase directionNoise;
        private const float Wind = 5f;
        private const int CloseDamageIntervalTicks = 15;
        private const int RoofDestructionIntervalTicks = 20;
        private const float FarDamageMTBTicks = 15f;
        private const float CloseDamageRadius = 4.2f;
        private const float FarDamageRadius = 10f;
        private const float BaseDamage = 30f;
        private const int SpawnMoteEveryTicks = 4;
        private const float DownedPawnDamageFactor = 0.2f;
        private const float AnimalPawnDamageFactor = 0.75f;
        private const float BuildingDamageFactor = 0.8f;
        private const float PlantDamageFactor = 1.7f;
        private const float ItemDamageFactor = 0.68f;
        private const float CellsPerSecond = 1.7f;
        private const float DirectionChangeSpeed = 0.78f;
        private const float DirectionNoiseFrequency = 0.002f;
        private const float TornadoAnimationSpeed = 25f;
        private const float ThreeDimensionalEffectStrength = 4f;
        private const int FadeInTicks = 120;
        private const int FadeOutTicks = 120;
        private const float MaxMidOffset = 2f;

        private float FadeInOutFactor
        {
            get
            {
                return Mathf.Min(Mathf.Clamp01((float)(Find.TickManager.TicksGame - this.spawnTick) / 120f), this.leftFadeOutTicks < 0 ? 1f : Mathf.Min((float)this.leftFadeOutTicks / 120f, 1f));
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<Vector2>(ref this.realPosition, "realPosition", new Vector2(), false);
            Scribe_Values.Look<float>(ref this.direction, "direction", 0.0f, false);
            Scribe_Values.Look<int>(ref this.spawnTick, "spawnTick", 0, false);
            Scribe_Values.Look<int>(ref this.leftFadeOutTicks, "leftFadeOutTicks", 0, false);
            Scribe_Values.Look<int>(ref this.ticksLeftToDisappear, "ticksLeftToDisappear", 0, false);
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                Vector3 vector3Shifted = this.Position.ToVector3Shifted();
                this.realPosition = new Vector2(vector3Shifted.x, vector3Shifted.z);
                this.direction = Rand.Range(0.0f, 360f);
                this.spawnTick = Find.TickManager.TicksGame;
                this.leftFadeOutTicks = -1;
                this.ticksLeftToDisappear = FlameTornado.DurationTicks.RandomInRange;
            }
            this.CreateSustainer();
        }

        public override void Tick()
        {
            if (!this.Spawned)
                return;
            if (this.sustainer == null)
            {
                Log.Error("Tornado sustainer is null.");
                this.CreateSustainer();
            }
            this.sustainer.Maintain();
            this.UpdateSustainerVolume();
            this.GetComp<CompWindSource>().wind = 5f * this.FadeInOutFactor;
            if (this.leftFadeOutTicks > 0)
            {
                --this.leftFadeOutTicks;
                if (this.leftFadeOutTicks != 0)
                    return;
                this.Destroy(DestroyMode.Vanish);
            }
            else
            {
                if (FlameTornado.directionNoise == null)
                    FlameTornado.directionNoise = (ModuleBase)new Perlin(1.0 / 500.0, 2.0, 0.5, 4, 1948573612, Verse.Noise.QualityMode.Medium);
                this.direction += (float)FlameTornado.directionNoise.GetValue((double)Find.TickManager.TicksAbs, (double)(this.thingIDNumber % 500) * 1000.0, 0.0) * 0.78f;
                this.realPosition = this.realPosition.Moved(this.direction, 0.02833333f);
                IntVec3 intVec3 = new Vector3(this.realPosition.x, 0.0f, this.realPosition.y).ToIntVec3();
                if (intVec3.InBounds(this.Map))
                {
                    this.Position = intVec3;
                    if (this.IsHashIntervalTick(15))
                        this.DamageCloseThings();
                    if (Rand.MTBEventOccurs(15f, 1f, 1f))
                        this.DamageFarThings();
                    if (this.IsHashIntervalTick(20))
                        this.DestroyRoofs();
                    if (this.ticksLeftToDisappear > 0)
                    {
                        --this.ticksLeftToDisappear;
                        if (this.ticksLeftToDisappear == 0)
                        {
                            this.leftFadeOutTicks = 120;
                            Messages.Message((string)"MessageTornadoDissipated".Translate(), (LookTargets)new TargetInfo(this.Position, this.Map, false), MessageTypeDefOf.PositiveEvent, true);
                        }
                    }
                    if (!this.IsHashIntervalTick(4) || this.CellImmuneToDamage(this.Position))
                        return;
                    float num = Rand.Range(0.6f, 1f);

                    // Start a fire, if possible.
                    IntVec3 intVec3Loc = (new Vector3(this.realPosition.x, 0.0f, this.realPosition.y)
                    {
                        y = AltitudeLayer.MoteOverhead.AltitudeFor()
                    } + Vector3Utility.RandomHorizontalOffset(1.5f)).ToIntVec3();
                    if (!intVec3Loc.InBounds(this.Map))
                        return;
                    FireUtility.TryStartFireIn(intVec3Loc, this.Map, Rand.Range(0.1f, 0.925f), this);

                    FleckMaker.ThrowTornadoDustPuff(new Vector3(this.realPosition.x, 0.0f, this.realPosition.y)
                    {
                        y = AltitudeLayer.MoteOverhead.AltitudeFor()
                    } + Vector3Utility.RandomHorizontalOffset(1.5f), this.Map, Rand.Range(1.5f, 3f), new Color(num, num, num));
                }
                else
                {
                    this.leftFadeOutTicks = 120;
                    Messages.Message((string)"MessageTornadoLeftMap".Translate(), (LookTargets)new TargetInfo(this.Position, this.Map, false), MessageTypeDefOf.PositiveEvent, true);
                }
            }
        }

        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            Rand.PushState();
            Rand.Seed = this.thingIDNumber;
            for (int index = 0; index < 180; ++index)
                this.DrawTornadoPart(FlameTornado.PartsDistanceFromCenter.RandomInRange, Rand.Range(0.0f, 360f), Rand.Range(0.9f, 1.1f), Rand.Range(0.52f, 0.88f));
            Rand.PopState();
        }

        private void DrawTornadoPart(
          float distanceFromCenter,
          float initialAngle,
          float speedMultiplier,
          float colorMultiplier)
        {
            int ticksGame = Find.TickManager.TicksGame;
            float num1 = 1f / distanceFromCenter;
            float num2 = 25f * speedMultiplier * num1;
            float num3 = (float)(((double)initialAngle + (double)ticksGame * (double)num2) % 360.0);
            Vector2 vector2 = this.realPosition.Moved(num3, this.AdjustedDistanceFromCenter(distanceFromCenter));
            vector2.y += distanceFromCenter * 4f;
            vector2.y += FlameTornado.ZOffsetBias;
            Vector3 vector3_1 = new Vector3(vector2.x, AltitudeLayer.Weather.AltitudeFor() + 0.04545455f * Rand.Range(0.0f, 1f), vector2.y);
            float num4 = distanceFromCenter * 3f;
            float num5 = 1f;
            if ((double)num3 > 270.0)
                num5 = GenMath.LerpDouble(270f, 360f, 0.0f, 1f, num3);
            else if ((double)num3 > 180.0)
                num5 = GenMath.LerpDouble(180f, 270f, 1f, 0.0f, num3);
            float num6 = Mathf.Min(distanceFromCenter / (FlameTornado.PartsDistanceFromCenter.max + 2f), 1f);
            float num7 = Mathf.InverseLerp(0.18f, 0.4f, num6);
            Vector3 vector3_2 = new Vector3(Mathf.Sin((float)ticksGame / 1000f + (float)(this.thingIDNumber * 10)) * 2f, 0.0f, 0.0f) * num7;
            Vector3 pos = vector3_1 + vector3_2;
            float a = Mathf.Max(1f - num6, 0.0f) * num5 * this.FadeInOutFactor;
            Color color = new Color(colorMultiplier, colorMultiplier, colorMultiplier, a);
            FlameTornado.matPropertyBlock.SetColor(ShaderPropertyIDs.Color, color);
            Quaternion q = Quaternion.Euler(0.0f, num3, 0.0f);
            Vector3 s = new Vector3(num4, 1f, num4);
            Matrix4x4 matrix = Matrix4x4.TRS(pos, q, s);
            Graphics.DrawMesh(MeshPool.plane10, matrix, FlameTornado.TornadoMaterial, 0, (Camera)null, 0, FlameTornado.matPropertyBlock);
        }

        private float AdjustedDistanceFromCenter(float distanceFromCenter)
        {
            float num1 = Mathf.Min(distanceFromCenter / 8f, 1f);
            float num2 = num1 * num1;
            return distanceFromCenter * num2;
        }

        private void UpdateSustainerVolume()
        {
            this.sustainer.info.volumeFactor = this.FadeInOutFactor;
        }

        private void CreateSustainer()
        {
            LongEventHandler.ExecuteWhenFinished((Action)(() =>
            {
                this.sustainer = SoundDefOf.Tornado.TrySpawnSustainer(SoundInfo.InMap((TargetInfo)((Thing)this), MaintenanceType.PerTick));
                this.UpdateSustainerVolume();
            }));
        }

        private void DamageCloseThings()
        {
            int num = GenRadial.NumCellsInRadius(4.2f);
            for (int index = 0; index < num; ++index)
            {
                IntVec3 intVec3 = this.Position + GenRadial.RadialPattern[index];
                if (intVec3.InBounds(this.Map) && !this.CellImmuneToDamage(intVec3))
                {
                    Pawn firstPawn = intVec3.GetFirstPawn(this.Map);
                    if (firstPawn == null || !firstPawn.Downed || !Rand.Bool)
                    {
                        float damageFactor = GenMath.LerpDouble(0.0f, 4.2f, 1f, 0.2f, intVec3.DistanceTo(this.Position));
                        this.DoDamage(intVec3, damageFactor);
                    }
                }
            }
        }

        private void DamageFarThings()
        {
            IntVec3 c = GenRadial.RadialCellsAround(this.Position, 10f, true).Where<IntVec3>((Func<IntVec3, bool>)(x => x.InBounds(this.Map))).RandomElement<IntVec3>();
            if (this.CellImmuneToDamage(c))
                return;
            this.DoDamage(c, 0.5f);
        }

        private void DestroyRoofs()
        {
            this.removedRoofsTmp.Clear();
            foreach (IntVec3 c in GenRadial.RadialCellsAround(this.Position, 4.2f, true).Where<IntVec3>((Func<IntVec3, bool>)(x => x.InBounds(this.Map))))
            {
                if (!this.CellImmuneToDamage(c) && c.Roofed(this.Map))
                {
                    RoofDef roof = c.GetRoof(this.Map);
                    if (!roof.isThickRoof && !roof.isNatural)
                    {
                        RoofCollapserImmediate.DropRoofInCells(c, this.Map, (List<Thing>)null);
                        this.removedRoofsTmp.Add(c);
                    }
                }
            }
            if (this.removedRoofsTmp.Count <= 0)
                return;
            RoofCollapseCellsFinder.CheckCollapseFlyingRoofs(this.removedRoofsTmp, this.Map, true, false);
        }

        private bool CellImmuneToDamage(IntVec3 c)
        {
            if (c.Roofed(this.Map) && c.GetRoof(this.Map).isThickRoof)
                return true;
            Building edifice = c.GetEdifice(this.Map);
            return edifice != null && edifice.def.category == ThingCategory.Building && (edifice.def.building.isNaturalRock || edifice.def == ThingDefOf.Wall && edifice.Faction == null);
        }

        private void DoDamage(IntVec3 c, float damageFactor)
        {
            FlameTornado.tmpThings.Clear();
            FlameTornado.tmpThings.AddRange((IEnumerable<Thing>)c.GetThingList(this.Map));
            Vector3 vector3Shifted = c.ToVector3Shifted();
            float angle = (float)(-(double)this.realPosition.AngleTo(new Vector2(vector3Shifted.x, vector3Shifted.z)) + 180.0);
            for (int index = 0; index < FlameTornado.tmpThings.Count; ++index)
            {
                BattleLogEntry_DamageTaken entryDamageTaken = (BattleLogEntry_DamageTaken)null;
                switch (FlameTornado.tmpThings[index].def.category)
                {
                    case ThingCategory.Pawn:
                        Pawn tmpThing = (Pawn)FlameTornado.tmpThings[index];
                        entryDamageTaken = new BattleLogEntry_DamageTaken(tmpThing, RulePackDefOf.DamageEvent_Tornado, (Pawn)null);
                        Find.BattleLog.Add((LogEntry)entryDamageTaken);
                        if ((double)tmpThing.RaceProps.baseHealthScale < 1.0)
                            damageFactor *= tmpThing.RaceProps.baseHealthScale;
                        if (tmpThing.RaceProps.Animal)
                            damageFactor *= 0.75f;
                        if (tmpThing.Downed)
                        {
                            damageFactor *= 0.2f;
                            break;
                        }
                        break;
                    case ThingCategory.Item:
                        damageFactor *= 0.68f;
                        break;
                    case ThingCategory.Building:
                        damageFactor *= 0.8f;
                        break;
                    case ThingCategory.Plant:
                        damageFactor *= 1.7f;
                        break;
                }
                int num = Mathf.Max(GenMath.RoundRandom(30f * damageFactor), 1);
                FlameTornado.tmpThings[index].TakeDamage(new DamageInfo(DamageDefOf.TornadoScratch, (float)num, 0.0f, angle, (Thing)this, (BodyPartRecord)null, (ThingDef)null, DamageInfo.SourceCategory.ThingOrUnknown, (Thing)null)).AssociateWithLog((LogEntry_DamageResult)entryDamageTaken);
            }
            FlameTornado.tmpThings.Clear();
        }
    }
}
