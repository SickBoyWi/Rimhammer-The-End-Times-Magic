using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
	public class PlaceWorker_RadiusTwelve : PlaceWorker
	{
		public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
		{
			Map currentMap = Find.CurrentMap;
			GenDraw.DrawRadiusRing(center, 12f);
		}
	}
}