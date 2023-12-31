using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using HugsLib;

namespace TheEndTimes_Magic
{
	public class Building_CleaningStatue : Building
	{
		public IEnumerable<IntVec3> cleaningAreaCells
		{
			get
			{
				Room room = this.GetRoom();
				if (room != null)
					return room.Cells;
				return GenRadial.RadialCellsAround(base.Position, 12f, true);
			}
		}

		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);

			HugsLibController.Instance.DistributedTicker.RegisterTickability(CleanTick, 5000, this);
		}

		public void CleanTick()
		{
			FleckMaker.ThrowLightningGlow(this.Position.ToVector3Shifted(), this.Map, 1f);
			foreach (IntVec3 cellToClean in cleaningAreaCells)
			{
				FilthMaker.RemoveAllFilth(cellToClean, this.Map);
			}
		}
	}
}