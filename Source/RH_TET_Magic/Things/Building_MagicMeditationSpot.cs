using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace TheEndTimes_Magic
{
    public class Building_MagicMeditationSpot : Building
    {
        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(
          Pawn selPawn)
        {
            Building_MagicMeditationSpot magicMeditationSpot = this;
            
            foreach (FloatMenuOption floatMenuOption in base.GetFloatMenuOptions(selPawn))
                yield return floatMenuOption;
            
            if (selPawn.RaceProps.Humanlike && !selPawn.Drafted && magicMeditationSpot.Faction == Faction.OfPlayer)
            {
                CompMagicUser compMagic = selPawn.TryGetComp<CompMagicUser>();
                CompFaithUser compFaith = selPawn.TryGetComp<CompFaithUser>();

                Action action = (Action)(() =>
                {
                    if (!selPawn.CanReserveAndReach((LocalTargetInfo)((Thing)this), PathEndMode.ClosestTouch, Danger.Deadly, 1, -1, (ReservationLayerDef)null, false))
                        return;
                    compMagic.MagicData.TicksUntilMeditate = Find.TickManager.TicksGame + 6000;
                    selPawn.jobs.TryTakeOrderedJob(new Job(DefDatabase<JobDef>.GetNamed("RH_TET_MagicMeditationJob", true), (LocalTargetInfo)((Thing)this)), JobTag.Misc);
                    selPawn.mindState.ResetLastDisturbanceTick();
                });
                if (!selPawn.CanReserve((LocalTargetInfo)((Thing)magicMeditationSpot), 1, -1, (ReservationLayerDef)null, false))
                    yield return new FloatMenuOption("RH_TET_MagicMeditate".Translate() + " (" + "Reserved".Translate() + ")", (Action)null, MenuOptionPriority.Default, null, (Thing)null, 0.0f, (Func<Rect, bool>)null, (WorldObject)null);
                else if ((compMagic == null || (compMagic != null && compMagic.MagicUserLevel < 1)) && (compFaith == null || (compFaith != null && !compFaith.IsFaithUser)))//TODO: compFaith.IsFaithUser will fail if pawns ever get faith abilities from a route other than the trait.
                    yield return new FloatMenuOption("RH_TET_MagicMeditate".Translate() + " (" + "RH_TET_MagicMeditate_MagicUsersOnly".Translate() + ")", (Action)null, MenuOptionPriority.Default, null, (Thing)null, 0.0f, (Func<Rect, bool>)null, (WorldObject)null);
                else if ((compMagic != null && compMagic.MagicData.TicksUntilMeditate > Find.TickManager.TicksGame) && (compFaith != null && compFaith.FaithData.TicksUntilMeditate > Find.TickManager.TicksGame))
                    yield return new FloatMenuOption("RH_TET_MagicMeditate".Translate() + " (" + "RH_TET_MagicMeditate_NeedRest".Translate() + ")", (Action)null, MenuOptionPriority.Default, null, (Thing)null, 0.0f, (Func<Rect, bool>)null, (WorldObject)null);
                else
                    yield return new FloatMenuOption("RH_TET_MagicMeditate".Translate(), action, MenuOptionPriority.Default, null, (Thing)null, 0.0f, (Func<Rect, bool>)null, (WorldObject)null);
            }
        }
    }
}
