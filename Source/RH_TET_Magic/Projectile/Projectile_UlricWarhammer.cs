using SickAbilityUser;
using HugsLib;
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
    public class Projectile_UlricWarhammer : Projectile_Ability
    {
        private int radius = 3;
        private readonly IntRange CollapseDelay = new IntRange(0, 120);
        private static List<Thing> tmpThingsToDestroy = new List<Thing>();

        public Projectile_UlricWarhammer()
            : base()
        {
        }

        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            base.Impact(hitThing);
            Pawn caster = this.launcher as Pawn;

            FleckMaker.ThrowMicroSparks(caster.Position.ToVector3(), caster.Map);
            FleckMaker.Static(caster.Position, caster.Map, FleckDefOf.PsycastAreaEffect, 1.5f);

            Map map = caster.Map;
            IntVec3 center = this.usedTarget.CenterVector3.ToIntVec3();

            List<IntVec3> openCells = new List<IntVec3>();
            int num = GenRadial.NumCellsInRadius(radius);

            for (int index = 0; index < num; ++index)
            {
                IntVec3 intVec3 = center + GenRadial.RadialPattern[index];
                if (intVec3.InBounds(map))
                {
                    openCells.Add(intVec3);
                }
            }
            
            bool thickRoof = false;
            for (int index1 = 0; index1 < openCells.Count; ++index1)
            {
                IntVec3 openCell = openCells[index1];

                // Destroy all plants. Destroy all buildings/structure. Destroy all items/things/corpses. Kill all pawns.
                thickRoof = false;
                if (openCell.Roofed(map))
                {
                    RoofDef roofDef = map.roofGrid.RoofAt(openCell);
                    if (roofDef != null)
                    {
                        if (roofDef.filthLeaving != null)
                        {
                            for (int index = 0; index < 3; ++index)
                                FilthMaker.TryMakeFilth(openCell, map, roofDef.filthLeaving, 1);
                        }
                        if (roofDef.isThickRoof)
                        {
                            thickRoof = true;
                            IntVec3 roofCell = openCell;
                            HugsLibController.Instance.TickDelayScheduler.ScheduleCallback((Action)(() =>
                            {
                                this.CollapseRockOnCell(roofCell, map);
                                SoundDefOf.Roof_Collapse.PlayOneShot((SoundInfo)new TargetInfo(roofCell, map, false));
                            }), this.CollapseDelay.RandomInRange, (Verse.Thing)null, false);
                        }
                        map.roofGrid.SetRoof(openCell, (RoofDef)null);
                    }
                }
                if (!thickRoof)
                {
                    Projectile_UlricWarhammer.tmpThingsToDestroy.Clear();
                    Projectile_UlricWarhammer.tmpThingsToDestroy.AddRange((IEnumerable<Thing>)openCell.GetThingList(map));
                    for (int index = 0; index < Projectile_UlricWarhammer.tmpThingsToDestroy.Count; ++index)
                    {
                        if (Projectile_UlricWarhammer.tmpThingsToDestroy[index].def.destroyable)
                        {
                            if (Projectile_UlricWarhammer.tmpThingsToDestroy[index] is Pawn)
                                Projectile_UlricWarhammer.tmpThingsToDestroy[index].Kill();
                            else
                                Projectile_UlricWarhammer.tmpThingsToDestroy[index].Destroy(DestroyMode.Vanish);
                        }
                    }

                    FleckMaker.ThrowDustPuffThick(openCell.ToVector3(), map, 3.0f, Color.white);
                }
            }
            
            DefDatabase<SoundDef>.GetNamed("Explosion_GiantBomb", true).PlayOneShot((SoundInfo)new TargetInfo(center, map, false));
        }

        private void CollapseRockOnCell(IntVec3 cell, Map map)
        {
            this.CrushThingsUnderCollapsingRock(cell, map);
            Verse.Thing thing = GenSpawn.Spawn(ThingDefOf.CollapsedRocks, cell, map, WipeMode.Vanish);
            if (!thing.def.rotatable)
                return;
            thing.Rotation = Rot4.Random;
        }

        private void CrushThingsUnderCollapsingRock(IntVec3 cell, Map map)
        {
            for (int index1 = 0; index1 < 2; ++index1)
            {
                List<Verse.Thing> thingList = cell.GetThingList(map);
                for (int index2 = thingList.Count - 1; index2 >= 0; --index2)
                {
                    Verse.Thing thing = thingList[index2];
                    Pawn pawn = thing as Pawn;
                    DamageInfo dinfo;
                    if (pawn != null)
                    {
                        BodyPartRecord brain = pawn.health.hediffSet.GetBrain();
                        dinfo = new DamageInfo(DamageDefOf.Crush, 99999f, 1f, -1f, (Verse.Thing)null, brain, (ThingDef)null, DamageInfo.SourceCategory.ThingOrUnknown, (Verse.Thing)null);
                    }
                    else
                    {
                        dinfo = new DamageInfo(DamageDefOf.Crush, 99999f, 0.0f, -1f, (Verse.Thing)null, (BodyPartRecord)null, (ThingDef)null, DamageInfo.SourceCategory.ThingOrUnknown, (Verse.Thing)null);
                        dinfo.SetBodyRegion(BodyPartHeight.Top, BodyPartDepth.Outside);
                    }
                    thing.TakeDamage(dinfo);
                    if (!thing.Destroyed && thing.def.destroyable)
                        thing.Destroy(DestroyMode.Vanish);
                }
            }
        }
    }
}
