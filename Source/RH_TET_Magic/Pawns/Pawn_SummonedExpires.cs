using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    public class Pawn_SummonedExpires : Pawn
    {
        private int ticksToDestroy = 1800;
        public bool validSummoning = false;
        private Pawn original = (Pawn)null;
        private List<float> bodypartDamage = new List<float>();
        private List<DamageDef> bodypartDamageType = new List<DamageDef>();
        private List<Hediff_Injury> injuries = new List<Hediff_Injury>();
        private Effecter effecter;
        private bool initialized;
        private bool temporary;
        private int ticksLeft;
        private CompMagicUser compSummoner;
        private Pawn spawner;

        public Pawn Original
        {
            get
            {
                return this.original;
            }
            set
            {
                this.original = value;
            }
        }

        public Pawn Spawner
        {
            get
            {
                return this.spawner;
            }
            set
            {
                this.spawner = value;
            }
        }

        public CompMagicUser CompSummoner
        {
            get
            {
                CompMagicUser comp = this.spawner.GetComp<CompMagicUser>();
                return comp;
            }
        }

        public bool Temporary
        {
            get
            {
                return this.temporary;
            }
            set
            {
                this.temporary = value;
            }
        }

        public int TicksToDestroy
        {
            get
            {
                return this.ticksToDestroy;
            }
            set
            {
                this.ticksToDestroy = value;
            }
        }

        public int TicksLeft
        {
            get
            {
                return this.ticksLeft;
            }
            set
            {
                this.ticksLeft = value;
            }
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            this.ticksLeft = this.ticksToDestroy;
            base.SpawnSetup(map, respawningAfterLoad);
        }

        public virtual void PostSummonSetup()
        {
            if (this.validSummoning)
                return;
            this.Destroy(DestroyMode.Vanish);
        }

        public void CheckPawnState()
        {
            try
            {
                if (this.Downed && !this.Destroyed && this != null && this.Faction == Faction.OfPlayer)
                {
                    FleckMaker.ThrowSmoke(this.Position.ToVector3(), this.Map, 1f);
                    FleckMaker.ThrowHeatGlow(this.Position, this.Map, 1f);
                    FleckMaker.Static(this.Position, this.Map, FleckDefOf.PsycastAreaEffect, 1.2f);
                    if (this.CompSummoner != null)
                    {
                        this.CompSummoner.summonedPawns.Remove(this);
                    }
                    this.Destroy(DestroyMode.Vanish);
                }
            }
            catch
            {
                Log.Message("RH_TET_PawnStateFail".Translate() + this.def.defName);
                this.Destroy(DestroyMode.Vanish);
            }
        }

        protected override void Tick()
        {
            base.Tick();
            if (Find.TickManager.TicksGame % 10 != 0)
                return;
            if (!this.initialized)
            {
                this.initialized = true;
                this.PostSummonSetup();
            }
            if (this.temporary)
            {
                this.ticksLeft -= 10;
                if (this.ticksLeft <= 0)
                {
                    this.Destroy(DestroyMode.Vanish);
                }
                this.CheckPawnState();
                if (this.Spawned)
                {
                    if (this.effecter == null)
                    {
                        this.effecter = EffecterDefOf.ProgressBar.Spawn();
                    }
                    else
                    {
                        LocalTargetInfo localTargetInfo = (LocalTargetInfo)((Thing)this);
                        if (this.Spawned)
                            this.effecter.EffectTick((TargetInfo)((Thing)this), TargetInfo.Invalid);
                        MoteProgressBar mote = ((SubEffecter_ProgressBar)this.effecter.children[0]).mote;
                        if (mote != null)
                        {
                            float num = (float)(1.0 - (double)(this.TicksToDestroy - this.ticksLeft) / (double)this.TicksToDestroy);
                            mote.progress = Mathf.Clamp01(num);
                            mote.offsetZ = -0.5f;
                        }
                    }
                }
            }
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            if (this.effecter != null)
                this.effecter.Cleanup();
            base.DeSpawn(mode);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref this.temporary, "temporary", false, false);
            Scribe_Values.Look<bool>(ref this.validSummoning, "validSummoning", true, false);
            Scribe_Values.Look<int>(ref this.ticksLeft, "ticksLeft", 0, false);
            Scribe_Values.Look<int>(ref this.ticksToDestroy, "ticksToDestroy", 1800, false);
            Scribe_Values.Look<CompMagicUser>(ref this.compSummoner, "compSummoner", (CompMagicUser)null, false);
            Scribe_References.Look<Pawn>(ref this.spawner, "spawner", false);
            Scribe_References.Look<Pawn>(ref this.original, "original", false);
        }
    }
}
