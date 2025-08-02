using LudeonTK;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    // TODO jeh this class doesnt work. It's a WIP.
    [StaticConstructorOnStartup]
    public class MineInSpawner : ThingWithComps
    {
        private static MaterialPropertyBlock matPropertyBlock = new MaterialPropertyBlock();
        [TweakValue("Gameplay", 0.0f, 1f)]
        private static float DustMoteSpawnMTB = 0.2f;
        [TweakValue("Gameplay", 0.0f, 1f)]
        private static float FilthSpawnMTB = 0.3f;
        [TweakValue("Gameplay", 0.0f, 10f)]
        private static float FilthSpawnRadius = 3f;
        private static readonly Material TunnelMaterial = MaterialPool.MatFrom("Things/Filth/Grainy/GrainyA", ShaderDatabase.Transparent);
        private static List<ThingDef> filthTypes = new List<ThingDef>();
        public bool spawnHive = true;
        private readonly FloatRange ResultSpawnDelay = new FloatRange(26f, 30f);
        private int secondarySpawnTick;
        //public float insectsPoints;
        //public bool spawnedByInfestationThingComp;
        private Sustainer sustainer;

        public static void ResetStaticData()
        {
            MineInSpawner.filthTypes.Clear();
            MineInSpawner.filthTypes.Add(ThingDefOf.Filth_Dirt);
            MineInSpawner.filthTypes.Add(ThingDefOf.Filth_Dirt);
            MineInSpawner.filthTypes.Add(ThingDefOf.Filth_Dirt);
            MineInSpawner.filthTypes.Add(ThingDefOf.Filth_RubbleRock);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.secondarySpawnTick, "secondarySpawnTicks", 0, false);
            Scribe_Values.Look<bool>(ref this.spawnHive, "spawnMineIn", true, false);
            //Scribe_Values.Look<float>(ref this.insectsPoints, "insectsPoints", 0.0f, false);
            //Scribe_Values.Look<bool>(ref this.spawnedByInfestationThingComp, "spawnedByMineInThingComp", false, false);
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (!respawningAfterLoad)
                this.secondarySpawnTick = Find.TickManager.TicksGame + this.ResultSpawnDelay.RandomInRange.SecondsToTicks();
            this.CreateSustainer();
        }

        protected override void Tick()
        {
            if (!this.Spawned)
                return;
            this.sustainer.Maintain();
            Vector3 vector3Shifted = this.Position.ToVector3Shifted();
            IntVec3 result;
            if (Rand.MTBEventOccurs(MineInSpawner.FilthSpawnMTB, 1f, 1.TicksToSeconds()) && CellFinder.TryFindRandomReachableCellNearPosition(this.Position, this.Position, this.Map, MineInSpawner.FilthSpawnRadius, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false, false, false), (Predicate<IntVec3>)null, (Predicate<Region>)null, out result, 999999))
                FilthMaker.TryMakeFilth(result, this.Map, MineInSpawner.filthTypes.RandomElement<ThingDef>(), 1, FilthSourceFlags.None);
            if (Rand.MTBEventOccurs(MineInSpawner.DustMoteSpawnMTB, 1f, 1.TicksToSeconds()))
                FleckMaker.ThrowDustPuffThick(new Vector3(vector3Shifted.x, 0.0f, vector3Shifted.z)
                {
                    y = AltitudeLayer.MoteOverhead.AltitudeFor()
                }, this.Map, Rand.Range(1.5f, 3f), new Color(1f, 1f, 1f, 2.5f));
            if (this.secondarySpawnTick > Find.TickManager.TicksGame)
                return;
            this.sustainer.End();
            Map map = this.Map;
            IntVec3 position = this.Position;
            this.Destroy(DestroyMode.Vanish);
            //this.Spawn(map, position);
        }

        //protected virtual void Spawn(Map map, IntVec3 loc)
        //{
        //    if (this.spawnHive)
        //    {
        //        Hive hive = (Hive)GenSpawn.Spawn(ThingMaker.MakeThing(ThingDefOf.Hive, (ThingDef)null), loc, map, WipeMode.Vanish);
        //        hive.SetFaction(Faction.OfInsects, (Pawn)null);
        //        hive.questTags = this.questTags;
        //        foreach (CompSpawner comp in hive.GetComps<CompSpawner>())
        //        {
        //            if (comp.PropsSpawner.thingToSpawn == ThingDefOf.InsectJelly)
        //            {
        //                comp.TryDoSpawn();
        //                break;
        //            }
        //        }
        //    }
        //    if ((double)this.insectsPoints <= 0.0)
        //        return;
        //    this.insectsPoints = Mathf.Max(this.insectsPoints, Hive.spawnablePawnKinds.Min<PawnKindDef>((Func<PawnKindDef, float>)(x => x.combatPower)));
        //    float pointsLeft = this.insectsPoints;
        //    List<Pawn> list = new List<Pawn>();
        //    int num = 0;
        //    PawnKindDef result;
        //    for (; (double)pointsLeft > 0.0; pointsLeft -= result.combatPower)
        //    {
        //        ++num;
        //        if (num > 1000)
        //        {
        //            Log.Error("Too many iterations.");
        //            break;
        //        }
        //        if (Hive.spawnablePawnKinds.Where<PawnKindDef>((Func<PawnKindDef, bool>)(x => (double)x.combatPower <= (double)pointsLeft)).TryRandomElement<PawnKindDef>(out result))
        //        {
        //            Pawn pawn = PawnGenerator.GeneratePawn(result, Faction.OfInsects);
        //            GenSpawn.Spawn((Thing)pawn, CellFinder.RandomClosewalkCellNear(loc, map, 2, (Predicate<IntVec3>)null), map, WipeMode.Vanish);
        //            pawn.mindState.spawnedByInfestationThingComp = this.spawnedByInfestationThingComp;
        //            list.Add(pawn);
        //        }
        //        else
        //            break;
        //    }
        //    if (!list.Any<Pawn>())
        //        return;
        //    LordMaker.MakeNewLord(Faction.OfInsects, (LordJob)new LordJob_AssaultColony(Faction.OfInsects, true, false, false, false, true, false, false), map, (IEnumerable<Pawn>)list);
        //}

        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            Rand.PushState();
            Rand.Seed = this.thingIDNumber;
            for (int index = 0; index < 6; ++index)
                this.DrawDustPart(Rand.Range(0.0f, 360f), (float)((double)Rand.Range(0.9f, 1.1f) * (double)Rand.Sign * 4.0), Rand.Range(1f, 1.5f));
            Rand.PopState();
        }

        private void DrawDustPart(float initialAngle, float speedMultiplier, float scale)
        {
            float seconds = (Find.TickManager.TicksGame - this.secondarySpawnTick).TicksToSeconds();
            Vector3 shiftedWithAltitude = this.Position.ToVector3ShiftedWithAltitude(AltitudeLayer.Filth);
            shiftedWithAltitude.y += 0.04054054f * Rand.Range(0.0f, 1f);
            Color color = new Color(0.4705882f, 0.3843137f, 0.3254902f, 0.7f);
            MineInSpawner.matPropertyBlock.SetColor(ShaderPropertyIDs.Color, color);
            Matrix4x4 matrix = Matrix4x4.TRS(shiftedWithAltitude, Quaternion.Euler(0.0f, initialAngle + speedMultiplier * seconds, 0.0f), Vector3.one * scale);
            Graphics.DrawMesh(MeshPool.plane10, matrix, MineInSpawner.TunnelMaterial, 0, (Camera)null, 0, MineInSpawner.matPropertyBlock);
        }

        private void CreateSustainer()
        {
            LongEventHandler.ExecuteWhenFinished((Action)(() => this.sustainer = SoundDefOf.Tunnel.TrySpawnSustainer(SoundInfo.InMap((TargetInfo)(Thing)this, MaintenanceType.PerTick))));
        }
    }
}
