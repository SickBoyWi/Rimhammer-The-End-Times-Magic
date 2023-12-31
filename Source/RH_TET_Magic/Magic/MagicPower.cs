using AbilityUser;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    public class MagicPower : IExposable
    {
        public int ticksUntilNextCast = -1;
        public List<AbilityDef> abilityDefs;
        public int level;

        public AbilityDef GetAbilityDef(int index)
        {
            AbilityDef abilityDef = (AbilityDef)null;
            if (this.abilityDefs != null && this.abilityDefs.Count > 0)
            {
                abilityDef = this.abilityDefs[0];
                if (index > -1 && index < this.abilityDefs.Count)
                    abilityDef = this.abilityDefs[index];
                else if (index >= this.abilityDefs.Count)
                    abilityDef = this.abilityDefs[this.abilityDefs.Count - 1];
            }
            return abilityDef;
        }

        public AbilityDef abilityDef
        {
            get
            {
                AbilityDef abilityDef = (AbilityDef)null;
                if (this.abilityDefs != null && this.abilityDefs.Count > 0)
                {
                    abilityDef = this.abilityDefs[0];
                    int index = this.level - 1;
                    if (index > -1 && index < this.abilityDefs.Count)
                        abilityDef = this.abilityDefs[index];
                    else if (index >= this.abilityDefs.Count)
                        abilityDef = this.abilityDefs[this.abilityDefs.Count - 1];
                }
                return abilityDef;
            }
        }

        public AbilityDef nextLevelAbilityDef
        {
            get
            {
                AbilityDef abilityDef = (AbilityDef)null;
                if (this.abilityDefs != null && this.abilityDefs.Count > 0)
                {
                    abilityDef = this.abilityDefs[0];
                    int level = this.level;
                    if (level > -1 && level <= this.abilityDefs.Count)
                        abilityDef = this.abilityDefs[level];
                    else if (level >= this.abilityDefs.Count)
                        abilityDef = this.abilityDefs[this.abilityDefs.Count - 1];
                }
                return abilityDef;
            }
        }

        public AbilityDef HasAbilityDef(AbilityDef defToFind)
        {
            return ((IEnumerable<AbilityDef>)this.abilityDefs).FirstOrDefault<AbilityDef>((Func<AbilityDef, bool>)(x => x == defToFind));
        }

        public MagicPower()
        {
        }

        public Texture2D Icon
        {
            get
            {
                return (Texture2D)this.abilityDef.uiIcon;
            }
        }

        public MagicPower(List<AbilityDef> newAbilityDefs)
        {
            this.level = 0;
            this.abilityDefs = newAbilityDefs;
        }

        public void ExposeData()
        {
            Scribe_Values.Look<int>(ref this.level, "level", 0, false);
            Scribe_Values.Look<int>(ref this.ticksUntilNextCast, "ticksUntilNextCast", -1, false);
            Scribe_Collections.Look<AbilityDef>(ref this.abilityDefs, "abilityDefs", LookMode.Def, (object[])null);
        }
    }
}
