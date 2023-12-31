using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace TheEndTimes_Magic
{
    public class IncidentWorker_MagicTransportPodCrash : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map target = (Map)parms.target;
            List<Thing> things = new List<Thing>();
            things.Add(GenerateNewPawnRefugeeReplacement());

            IntVec3 intVec3 = DropCellFinder.RandomDropSpot(target);
            Pawn pawn = ThingUtility.FindPawn(things);
            if (pawn.Faction != null)
                pawn.SetFaction(null);
            pawn.guest.getRescuedThoughtOnUndownedBecauseOfPlayer = true;
            HealthUtility.DamageUntilDowned(pawn, true);
            TaggedString title = "RH_TET_LetterLabelMagicTransportPodCrash".Translate();
            TaggedString taggedString = "RH_TET_MagicTransportPodCrash".Translate(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN", true) + "\n\n";
            TaggedString text = taggedString;// + "RefugeePodCrash_Factionless".Translate(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN", true);

            this.SendStandardLetter(title, text, LetterDefOf.NeutralEvent, parms, (LookTargets)new TargetInfo(intVec3, target, false), (NamedArgument[])Array.Empty<NamedArgument>());
            ActiveDropPodInfo info = new ActiveDropPodInfo();
            info.innerContainer.TryAddRangeOrTransfer((IEnumerable<Thing>)things, true, false);
            info.openDelay = 180;
            info.leaveSlag = true;
            MagicTransportPodUtility.MakeMagicPodAt(intVec3, target, info);
            return true;
        }

        private Pawn GenerateNewPawnRefugeeReplacement()
        {
            PawnKindDef genPawnKind = PawnKindDefOf.Villager;
            int minAge = 22;
            int maxAge = 55;
            bool dawiModActive = ModLister.HasActiveModWithName("Rimhammer - The End Times - Dwarfs");
            bool empireModActive = ModLister.HasActiveModWithName("Rimhammer - The End Times - Empire");
            
            if (dawiModActive || empireModActive)
            {
                int rand = RH_TET_MagicMod.random.Next(0, 10);

                try
                {
                    if (rand < 3 && dawiModActive)
                    {
                        genPawnKind = PawnKindDef.Named("RH_TET_DwarfRanged");
                        minAge = 80;
                        maxAge = 120;
                    }
                    else if (rand < 5 && dawiModActive)
                    {
                        genPawnKind = PawnKindDef.Named("RH_TET_DwarfSlayer");
                        minAge = 80;
                        maxAge = 120;
                    }
                    else if (rand < 8 && empireModActive)
                    {
                        genPawnKind = PawnKindDef.Named("RH_TET_EmpireVillager");
                    }
                    else if (empireModActive)
                    {
                        genPawnKind = PawnKindDef.Named("RH_TET_Empire_WizardScenario");
                    }
                }
                catch
                {
                    genPawnKind = PawnKindDefOf.Villager;
                }
            }

            int age = RH_TET_MagicMod.random.Next(minAge, maxAge);
            PawnGenerationRequest request = new PawnGenerationRequest(genPawnKind, null, PawnGenerationContext.NonPlayer, -1,
                true, false, false,
                false, true, .05f,
                false, true, true,
                false, false, false,
                false, false, false,
                0.0f, 0.0f, null, 
                0.0f, null, null, 
                null, null, null, 
                age, age);

            return PawnGenerator.GeneratePawn(request);
        }
    }
}
