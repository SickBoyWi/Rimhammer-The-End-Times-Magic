using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    public class Designator_RemoveSteamPipes : Designator
    {
        public static ThingDef RemovalMode;
        public static List<ThingDef> cachedPipeTypes;

        public Designator_RemoveSteamPipes()
        {
            this.defaultLabel = (string)"RH_TET_Magic_DesigRemovePipes".Translate();
            this.defaultDesc = (string)"RH_TET_Magic_DesigRemovePipesDesc".Translate();
            this.icon = ContentFinder<Texture2D>.Get("UI/Steam/RemoveSteamPipes", true);
            this.useMouseIcon = true;
            this.soundDragSustain = SoundDefOf.Designate_DragStandard;
            this.soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
            this.soundSucceeded = SoundDefOf.Designate_SmoothSurface;
            this.hotKey = KeyBindingDefOf.Misc1;
        }

        public override bool DragDrawMeasurements
        {
            get
            {
                return true;
            }
        }

        public override bool Visible
        {
            get
            {
                return true;
            }
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 c)
        {
            if (!c.InBounds(this.Map))
                return (AcceptanceReport)false;
            if (!DebugSettings.godMode && c.Fogged(this.Map))
                return (AcceptanceReport)false;
            return this.TopDeconstructibleInCell(c) == null ? (AcceptanceReport)false : (AcceptanceReport)true;
        }

        public override void DesignateSingleCell(IntVec3 loc)
        {
            this.DesignateThing(this.TopDeconstructibleInCell(loc));
        }

        private Thing TopDeconstructibleInCell(IntVec3 loc)
        {
            foreach (Thing t in (IEnumerable<Thing>)this.Map.thingGrid.ThingsAt(loc).OrderByDescending<Thing, AltitudeLayer>((Func<Thing, AltitudeLayer>)(t => t.def.altitudeLayer)))
            {
                if (this.CanDesignateThing(t).Accepted)
                    return t;
            }
            return (Thing)null;
        }

        public override AcceptanceReport CanDesignateThing(Thing t)
        {
            if (Designator_RemoveSteamPipes.cachedPipeTypes.NullOrEmpty())
            {
                Designator_RemoveSteamPipes.cachedPipeTypes = DefDatabase<ThingDef>.AllDefsListForReading.Where<ThingDef>((Func<ThingDef, bool>)(x => x.thingClass == typeof(Building_SteamPipe))).ToList<ThingDef>();

                if (!Designator_RemoveSteamPipes.cachedPipeTypes.NullOrEmpty())
                    Designator_RemoveSteamPipes.RemovalMode = cachedPipeTypes.First<ThingDef>();
            }

            if (!(t is Building building))
                return (AcceptanceReport)false;
            if (t.def != Designator_RemoveSteamPipes.RemovalMode)
                return (AcceptanceReport)false;
            if (!DebugSettings.godMode && building.Faction != Faction.OfPlayer)
            {
                if (building.Faction != null)
                    return (AcceptanceReport)false;
                if (!building.ClaimableBy(Faction.OfPlayer))
                    return (AcceptanceReport)false;
            }
            if (this.Map.designationManager.DesignationOn(t, DesignationDefOf.Deconstruct) != null)
                return (AcceptanceReport)false;
            if (this.Map.designationManager.DesignationOn(t, DesignationDefOf.Uninstall) != null)
                return (AcceptanceReport)false;
            else
                return (AcceptanceReport)true;
            //return (AcceptanceReport)(t.def != null && t.def.thingClass == typeof(Building_SteamPipe));
        }

        public override void DesignateThing(Thing t)
        {
            this.Map.designationManager.AddDesignation(new Designation((LocalTargetInfo)t, DesignationDefOf.Deconstruct));
        }

        public override void SelectedUpdate()
        {
            GenUI.RenderMouseoverBracket();
        }
    }
}
