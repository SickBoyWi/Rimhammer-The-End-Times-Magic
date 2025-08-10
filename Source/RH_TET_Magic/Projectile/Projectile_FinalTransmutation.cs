using SickAbilityUser;
using RimWorld;
using System;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using KTrie;

namespace TheEndTimes_Magic
{
    public class Projectile_FinalTransmutation : Projectile_AbilityLaser
    {
        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            if (this.landed)
                return;

            int rando = RH_TET_MagicMod.random.Next(0, 10);
            Pawn pawnCaster = this.launcher as Pawn;
            int spellLevel = DetermineSpellLevel(pawnCaster);
            bool turnToGold = false;

            switch (spellLevel)
            {
                case 1:
                    if (rando < 2)
                        turnToGold = true;
                    break;
                case 2:
                    if (rando < 3)
                        turnToGold = true;
                    break;
                case 3:
                    if (rando < 4)
                        turnToGold = true;
                    break;
                case 4:
                    if (rando < 5)
                        turnToGold = true;
                    break;
                default:
                    break;
            }
            
            try
            {
                if (this.landed)
                    return;

                this.DrawAt(pawnCaster.Position.ToVector3(), false);

                Pawn target = this.intendedTarget.Pawn;
                IntVec3 pos = target.Position;

                if (target.RaceProps.Humanlike && target.RaceProps.IsFlesh && turnToGold)
                {
                    Thing sculpture = ThingMaker.MakeThing(RH_TET_MagicDefOf.RH_TET_Magic_FinalTransmutationSculpture, ThingDefOf.Gold);

                    Messages.Message("RH_TET_MessageFinalTransmutationSuccess".Translate(pawnCaster.Name, target.Name), sculpture, MessageTypeDefOf.PositiveEvent);

                    if (!target.Destroyed)
                        target.Destroy(DestroyMode.KillFinalize);
                    
                    GenSpawn.Spawn(sculpture, pos, pawnCaster.Map);

                    this.landed = true;
                }
                else
                {
                    int dam = this.DamageAmount;

                    DamageInfo damageInfo = new DamageInfo(RH_TET_MagicDefOf.RH_TET_MagicFire, dam, 0, -1, pawnCaster);
                    target.TakeDamage(damageInfo);

                    StringBuilder sb = new StringBuilder();
                    sb.Append("RH_TET_MessageFinalTransmutationSuccessNoGold".Translate());

                    if (!target.RaceProps.Humanlike || !target.RaceProps.IsFlesh)
                        sb.Append(" " + "RH_TET_MessageFinalTransmutationHumansOnly".Translate());

                    Messages.Message(sb.ToString(), pawnCaster, MessageTypeDefOf.PositiveEvent);

                    this.landed = true;
                }
            }
            catch
            {
                Messages.Message("RH_TET_MessageFinalTransmutationFail".Translate(pawnCaster.Name), pawnCaster, MessageTypeDefOf.NegativeEvent);
            }
        }
        
        public Projectile_FinalTransmutation()
            : base()
        {
        }

        private int DetermineSpellLevel(Pawn caster)
        {
            // Determine spell level.
            int spellLevel = -1;

            foreach (MagicPower mp in caster.GetComp<CompMagicUser>().MagicData.PowersMetal)
            {
                if (mp.abilityDef.defName.Contains("FinalTransmutat"))
                {
                    spellLevel = mp.level;
                }
            }

            return spellLevel;
        }
    }
}
