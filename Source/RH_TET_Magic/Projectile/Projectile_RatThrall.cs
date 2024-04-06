using SickAbilityUser;
using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    [StaticConstructorOnStartup]
    public class Projectile_RatThrall : Projectile_AbilityBase
    {
        private int age;
        private bool initialized;
        private bool summoning;
        private bool summoningComplete;
        private bool destroyed;
        private CompMagicUser comp;
        private List<Pawn> summonedPawns;
        private Pawn casterPawn;
        private List<IntVec3> summoningArea;
        private int pawnSpawnCountMin = 1;
        private int pawnSpawnCountMax = 2;

        public Projectile_RatThrall()
            : base()
        {
        }

        public override void Tick()
        {
            base.Tick();
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            base.Destroy(mode);
        }

        private void Initialize()
        {
            this.casterPawn = this.launcher as Pawn;
            this.comp = this.casterPawn.GetComp<CompMagicUser>();
            this.summoningArea = new List<IntVec3>();
            this.summoningArea.Clear();
            this.summoningArea = GenRadial.RadialCellsAround(this.usedTarget.CenterVector3.ToIntVec3(), 5f, false).ToList<IntVec3>();
            this.age = 0;
            this.summoning = true;
            this.initialized = true;
            this.summonedPawns = new List<Pawn>();
        }

        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            if (!this.initialized)
            {
                this.Initialize();
                FleckMaker.ThrowSmoke(this.summoningArea[0].ToVector3Shifted(), this.Map, 2f);
            }

            if (this.summoningComplete)
            {
                this.Destroy();
                return;
            }

            // Determine spell level.
            int spellLevel = -1;

            foreach (MagicPower mp in casterPawn.GetComp<CompMagicUser>().MagicData.PowersWarp)
            {
                if (mp.abilityDef.defName.Contains("RatThrall"))
                {
                    spellLevel = mp.level;
                }
            }

            if (spellLevel <= 0)
            {
                Log.Warning("No spell level for rat thrall spell. Failing gracefully.");
                return;
            }
            else if (spellLevel == 2)
            {
                pawnSpawnCountMin = 2;
                pawnSpawnCountMax = 3;
            }
            else if (spellLevel == 3)
            {
                pawnSpawnCountMin = 3;
                pawnSpawnCountMax = 4;
            }
            else if (spellLevel == 4)
            {
                pawnSpawnCountMin = 4;
                pawnSpawnCountMax = 5;
            }

            int pawnSpawnCount = RH_TET_MagicMod.random.Next(pawnSpawnCountMin, pawnSpawnCountMax + 1);

            base.Impact(hitThing);

            IntVec3 position = this.casterPawn.Position;

            Map map2 = this.casterPawn.Map;
            FleckMaker.Static(position, map2, FleckDefOf.PsycastAreaEffect, 1f);

            if (this.usedTarget.CenterVector3 != null)
            {
                position = this.usedTarget.CenterVector3.ToIntVec3();
            }

            this.SpawnLoop(position, map2, pawnSpawnCount);
            FleckMaker.ThrowSmoke(position.ToVector3(), map2, 2f);
            FleckMaker.ThrowMicroSparks(position.ToVector3(), map2);
            FleckMaker.ThrowHeatGlow(position, map2, 2f);

            SoundInfo info = SoundInfo.InMap(new TargetInfo(position, map2, false), MaintenanceType.None);
            info.pitchFactor = 1.3f;
            info.volumeFactor = 1.6f;
            SoundDef.Named("Pawn_Boomrat_Angry").PlayOneShot(info);

            this.summoningComplete = true;
            this.summoning = false;
        }

        public void SpawnLoop(IntVec3 position, Map map, int pawnSpawnCount)
        {
            Faction faction = this.launcher.Faction;

            for (int i = 0; i < pawnSpawnCount; i++)
            { 
                float biologicalAge = (float)RH_TET_MagicMod.random.Next(3, 6);

                PawnGenerationRequest pawnRequest = new PawnGenerationRequest(RH_TET_MagicDefOf.RH_TET_Rat_Medium, faction, PawnGenerationContext.PlayerStarter, -1,
                    true, false, false,
                    false, true, 0f,
                    false, true, false,
                    false, false, false,
                    false, false, false,
                    0.0f, 0.0f, null,
                    0.0f, null, null,
                    null, null, null,
                    biologicalAge, biologicalAge);

                Pawn newPawn = (Pawn)PawnGenerator.GeneratePawn(pawnRequest);

                try
                {
                    this.summonedPawns.Add(newPawn);

                    // Fully train.
                    int trainableCount = 0;
                    List<TrainableDef> trainableDefsInListOrder = TrainableUtility.TrainableDefsInListOrder;
                    for (int j = 0; j < trainableDefsInListOrder.Count; ++j)
                    {
                        bool visible = false;
                        AcceptanceReport ar = newPawn.training.CanAssignToTrain(trainableDefsInListOrder[j], out visible);
                        if (ar.Accepted)
                        {
                            trainableCount++;
                            newPawn.training.SetWantedRecursive(trainableDefsInListOrder[j], true);
                        }
                    }

                    switch (trainableCount)
                    {
                        case 1:
                            trainableCount = 0;
                            break;
                        case 2:
                            trainableCount = 3;
                            break;
                        case 3:
                            trainableCount = 5;
                            break;
                        case 4:
                            trainableCount = 7;
                            break;
                        case 5:
                            trainableCount = 14;
                            break;
                        default:
                            break;
                    }

                    if (trainableCount > 0)
                    {
                        for (int k = 0; k < trainableCount; k++)
                        {
                            TrainableDef train = newPawn.training.NextTrainableToTrain();
                            newPawn.training.Train(train, casterPawn, false);
                        }
                    }

                    RelationsUtility.TryDevelopBondRelation(casterPawn, newPawn, 1f);

                    GenSpawn.Spawn((Thing)newPawn, position, map, WipeMode.Vanish);
                }
                catch
                {
                    this.Destroy(DestroyMode.Vanish);
                }
            }
        }

        public Vector3 GetVector(IntVec3 center, IntVec3 objectPos)
        {
            Vector3 vector3 = (objectPos - center).ToVector3();
            float magnitude = vector3.magnitude;
            return vector3 / magnitude;
        }

        public override void ExposeData()
        {
            ((Projectile_AbilityBase)this).ExposeData();
            Scribe_Values.Look<int>(ref this.age, "age", 0, false);
            Scribe_Values.Look<bool>(ref this.initialized, "initialized", false, false);
            Scribe_Values.Look<bool>(ref this.summoning, "summoning", false, false);
            Scribe_Values.Look<bool>(ref this.summoningComplete, "summoningComplete", false, false);
            Scribe_Values.Look<bool>(ref this.destroyed, "destroyed", false, false);
            Scribe_Collections.Look<Pawn>(ref this.summonedPawns, "summonedPawns", LookMode.Reference, (object[])Array.Empty<object>());
            Scribe_Collections.Look<IntVec3>(ref this.summoningArea, "summoningArea", LookMode.Value);
            Scribe_References.Look<Pawn>(ref this.casterPawn, "casterPawn", false);
        }
    }
}
