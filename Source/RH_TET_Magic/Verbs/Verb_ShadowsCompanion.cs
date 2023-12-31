using AbilityUser;
using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace TheEndTimes_Magic
{
    public class Verb_ShadowsCompanion : Verb_UseAbility
    {
        protected override bool TryCastShot()
        {
            bool flag1 = false;
            Pawn casterPawn = this.CasterPawn;
            Map map = casterPawn.Map;
            bool successInd;

            if (casterPawn != null && !casterPawn.Downed)
            {
                if (((HediffSet)((Pawn_HealthTracker)casterPawn.health).hediffSet).HasHediff(RH_TET_MagicDefOf.RH_TET_ShadowsCompanion_HE, false))
                {
                    List<Hediff> resultHediffs = new List<Hediff>();
                    casterPawn.health.hediffSet.GetHediffs<Hediff>(ref resultHediffs);

                    using (IEnumerator<Hediff> enumerator = resultHediffs.GetEnumerator())
                    {
                        while (((IEnumerator)enumerator).MoveNext())
                        {
                            Hediff current = enumerator.Current;
                            if (current.def == RH_TET_MagicDefOf.RH_TET_ShadowsCompanion_HE)
                                ((Pawn_HealthTracker)casterPawn.health).RemoveHediff(current);
                        }
                    }

                    FleckMaker.Static(caster.Position, map, RH_TET_MagicDefOf.RH_TET_FleckGrayEffect, 1f);
                    FleckMaker.Static(caster.Position, map, FleckDefOf.PsycastAreaEffect, 1f);
                }
                else
                {
                    HealthUtility.AdjustSeverity(casterPawn, RH_TET_MagicDefOf.RH_TET_ShadowsCompanion_HE, (float)Mathf.RoundToInt(20f));// AFTER 20 in parens:  * ((CompMagicUser)((ThingWithComps)casterPawn).GetComp<CompMagicUser>()).arcaneDmg

                    FleckMaker.Static(caster.Position, map, RH_TET_MagicDefOf.RH_TET_FleckGrayEffect, 1f);
                    FleckMaker.Static(caster.Position, map, FleckDefOf.PsycastAreaEffect, 1f);

                    List<Pawn> allPawnsSpawned = this.CasterPawn.Map.mapPawns.AllPawnsSpawned;
                    for (int index = 0; index < allPawnsSpawned.Count; ++index)
                    {
                        if (((Thing)allPawnsSpawned[index]).Faction != null && GenHostility.HostileTo((Thing)allPawnsSpawned[index], CasterPawn.Faction) && (allPawnsSpawned[index].CurJob != null && allPawnsSpawned[index].CurJob.targetA != null && allPawnsSpawned[index].CurJob.targetA != CasterPawn))
                            ((Pawn_JobTracker)allPawnsSpawned[index].jobs).EndCurrentJob((JobCondition)5, true, true);
                    }
                }
                successInd = true;
            }
            else
                successInd = false;
            if (!successInd)
                Log.Warning("failed to TryCastShot on Shadows Companion.", false);

            //this.PostCastShot(success, out success);

            this.burstShotsLeft = 0;
            return flag1;
        }
    }
}
