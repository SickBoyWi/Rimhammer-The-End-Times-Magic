using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    public class Thing_MagicWeaponWithLight : ThingWithComps
    {
        //public const int updatePeriodInTicks = 10;
        //public int nextUpdateTick;
        public Thing light;
        public bool lightIsOn = true;
        public Thing_MagicWeaponWithLight.LightMode lightMode;
        public ThingDef lightDef;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (respawningAfterLoad)
                return;
        }

        public override void Tick()
        {
            base.Tick();
            // TODO REMOVED LIGHTS FOR 1.4
            //this.RefreshLightState();
        }

        public void RefreshLightState()
        {
            if (this.ComputeLightState())
                this.SwitchOnLight();
            else
                this.SwitchOffLight();
        }

        public bool ComputeLightState()
        {
            if (this.Destroyed)
            {
                return false;
            }
            return true;
        }

        public void SwitchOnLight()
        {

            // See if it is held by a pawn. If not, ignore error.
            Pawn holder = null;
            if (this.ParentHolder != null && this.ParentHolder is Pawn_EquipmentTracker)
            {
                try
                {
                    holder = (this.ParentHolder as Pawn_EquipmentTracker).pawn;
                }
                catch
                {
                    //Log.Error("1");
                    this.lightIsOn = false;
                }

                if (holder == null)
                {
                    try
                    {
                        holder = (this.ParentHolder as Pawn_ApparelTracker).pawn;
                    }
                    catch
                    {
                        //Log.Error("2");
                        this.lightIsOn = false;
                    }
                }
            }

            // Don't let it glow unless it's being held.
            if (holder == null)
            {
                if (!this.light.DestroyedOrNull())
                    this.SwitchOffLight();
                return;
            }

            IntVec3 intVec3;
            try
            {
                intVec3 = holder.DrawPos.ToIntVec3();

                if (!this.light.DestroyedOrNull() && intVec3 != this.light.Position)
                    this.SwitchOffLight();

                Map map = this.Map;
                if (map == null)
                {
                    map = holder.Map;
                }

                //Log.Error("LIGHTDEF:" + (lightDef == null ? "NULL": lightDef.defName));
                // TODO : THIS MAY ONLY ALLOW ONE PER MAP, TEST!!
                if (this.light.DestroyedOrNull() && intVec3.GetFirstThing(map, lightDef) == null)
                    this.light = GenSpawn.Spawn(lightDef, intVec3, map, WipeMode.Vanish);

                this.lightIsOn = true;
            }
            catch
            {
                this.lightIsOn = false;
            }
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            base.Destroy(mode);

            if (!this.light.DestroyedOrNull())
            {
                this.light.Destroy(DestroyMode.Vanish);
                this.light = (Thing)null;
            }
        }

        public void SwitchOffLight()
        {
            try
            { 
                if (!this.light.DestroyedOrNull())
                {
                    this.light.Destroy(DestroyMode.Vanish);
                    this.light = (Thing)null;
                }
                this.lightIsOn = false;
            }
            catch
            {
                this.lightIsOn = false;
            }
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            IEnumerable<Gizmo> gizmos = base.GetGizmos();
            return gizmos;
        }

        public void SwitchLigthMode()
        {
            this.RefreshLightState();
        }

        public enum LightMode
        {
            Automatic
        }
    }
}