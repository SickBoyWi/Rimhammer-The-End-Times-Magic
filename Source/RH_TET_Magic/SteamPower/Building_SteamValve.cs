using RimWorld;
using System.Text;
using Verse;

namespace TheEndTimes_Magic
{
    /**
     * Original code for this came from Dubs Central Heating Mod, and was
     * trimmed down to suit this mod's needs.
     */
    public class Building_SteamValve : Building
    {
        private CompFlickable flickableComp;

        public override Graphic Graphic
        {
            get
            {
                if (this.flickableComp == null)
                    this.flickableComp = this.GetComp<CompFlickable>();
                return this.flickableComp.CurrentGraphic;
            }
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.flickableComp = this.GetComp<CompFlickable>();
        }

        public override string GetInspectString()
        {
            if (this.ParentHolder is MinifiedThing)
                return base.GetInspectString();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(base.GetInspectString());
            if (!FlickUtility.WantsToBeOn((Thing)this))
            {
                if (stringBuilder.Length > 0)
                    stringBuilder.AppendLine();
                stringBuilder.Append((string)"RH_TET_Magic_SteamValveClosed".Translate());
            }
            return stringBuilder.ToString();
        }
    }
}
