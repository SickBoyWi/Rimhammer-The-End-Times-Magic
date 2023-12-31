using RimWorld;
using Verse;

namespace TheEndTimes_Magic
{
    public class CompGeothermalBoiler : CompBoiler
    {
        private Building_SteamGeyser geyser;

        public override void CompTick()
        {
            base.CompTick();
            if (this.geyser == null)
                this.geyser = (Building_SteamGeyser)this.parent.Map.thingGrid.ThingAt(this.parent.Position, ThingDefOf.SteamGeyser);
            if (this.geyser == null)
                return;
            this.geyser.harvester = (Building)this.parent;
        }
    }
}