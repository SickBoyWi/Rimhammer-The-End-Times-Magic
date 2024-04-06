using SickAbilityUser;
using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    public class Verb_Execute : Verb_UseAbility
    {
        protected override bool TryCastShot()
        {
            bool flag1 = false;
            Pawn theCaster = caster as Pawn;

            Pawn target = this.currentTarget.Thing as Pawn;

            AbilityActionPower abilityPower = null;
            foreach (AbilityActionPower fp in this.CasterPawn.GetComp<CompAbilityActionUser>().AbilityActionData.PowersWitchHunter)
            {
                if (fp.abilityDef.defName.Contains("Execute"))
                {
                    abilityPower = fp;
                }
            }
            
            if (abilityPower == null)
            {
                Log.Error("Someone tried to cast Execute, but didn't have the ability power.");
                return false;
            }
            
            if (target is null)
                Messages.Message("RH_TET_MessageSpellFailTargets".Translate(theCaster.Name), MessageTypeDefOf.NegativeEvent);

            Map map = theCaster.Map;

            FleckMaker.Static(theCaster.Position, map, FleckDefOf.PsycastAreaEffect, 1.2f);
            IntVec3 targetPostion = caster.Position;

            bool? attempt = TryLaunchProjectile(this.AbilityProjectileDef, target);
            this.Execute(target, theCaster, map);
            FleckMaker.Static(targetPostion, map, FleckDefOf.PsycastAreaEffect, 0.5f);

            this.PostCastShot(flag1, out flag1);
            return flag1;
        }

        private void Execute(Pawn target, Pawn caster, Map map)
        {
            CellRect cellRect = target.OccupiedRect().ExpandedBy(2);
            BodyPartRecord bodyPartRecord = target.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined, (BodyPartTagDef)null, (BodyPartRecord)null).FirstOrDefault<BodyPartRecord>((Func<BodyPartRecord, bool>)(x => x.def == BodyPartDefOf.Head));
            if (bodyPartRecord != null)
            {
                int headHealth = Mathf.Clamp((int)target.health.hediffSet.GetPartHealth(bodyPartRecord) - 1, 1, 20);

                if (ModsConfig.BiotechActive && target.genes != null && target.genes.HasGene(GeneDefOf.Deathless))
                {
                    headHealth = 99999;
                }

                // Make it so it doesn't one shot kill all animals.
                if (target.NonHumanlikeOrWildMan())
                {
                    headHealth = 18;
                }

                DamageInfo dinfo = new DamageInfo(DamageDefOf.Bullet, (float)headHealth, 999f, -1f, (Thing)caster, bodyPartRecord, (ThingDef)null, DamageInfo.SourceCategory.ThingOrUnknown, (Thing)null, false, true);
                target.TakeDamage(dinfo);
                if (!target.Dead)
                    target.Kill(new DamageInfo?(dinfo), (Hediff)null);

                // Spill blood
                List<IntVec3> tmpCellsCandidates = new List<IntVec3>();
                foreach (IntVec3 c in cellRect)
                {
                    if (c.InBounds(map) && c.Walkable(map))
                        tmpCellsCandidates.Add(c);
                }
                if (!tmpCellsCandidates.Any<IntVec3>())
                    return;
                for (int index = 0; index < tmpCellsCandidates.Count; ++index)
                    FilthMaker.TryMakeFilth(tmpCellsCandidates[index], map, ThingDefOf.Filth_Blood, 2, FilthSourceFlags.None, true);
            }
            else
            {
                Log.Warning("Could not execute. No head.");
            }
        }

        public Verb_Execute()
            : base()
        {
        }
    }
}
