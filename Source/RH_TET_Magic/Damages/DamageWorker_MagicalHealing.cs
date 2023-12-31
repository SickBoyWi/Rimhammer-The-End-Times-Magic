using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace TheEndTimes_Magic
{
    public class DamageWorker_MagicalHealing : DamageWorker
    {
        public override DamageWorker.DamageResult Apply(DamageInfo dinfo, Thing thing)
        {
            DamageWorker.DamageResult damageResult = new DamageWorker.DamageResult();
            damageResult.totalDamageDealt = 0.0f;
            Pawn pawn = thing as Pawn;
            if (pawn != null)
            {
                int num1 = 6;
                foreach (BodyPartRecord injuredPart in pawn.health.hediffSet.GetInjuredParts())
                {
                    BodyPartRecord rec = injuredPart;
                    if (num1 > 0)
                    {
                        int num2 = 2;
                        List<Hediff_Injury> resultHediffs = new List<Hediff_Injury>();
                        pawn.health.hediffSet.GetHediffs<Hediff_Injury>(ref resultHediffs, (Predicate<Hediff_Injury>)(injury => injury.Part == rec));
                        foreach (Hediff_Injury hd in resultHediffs)
                        {
                            if (num2 > 0 && hd.CanHealNaturally() && !hd.IsPermanent())
                            {
                                hd.Heal((float)((int)hd.Severity + 1));
                                --num1;
                                --num2;
                            }
                        }
                    }
                }
            }
            return damageResult;
        }
    }
}
