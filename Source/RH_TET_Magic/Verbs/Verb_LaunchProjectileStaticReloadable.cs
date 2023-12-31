using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace TheEndTimes_Magic
{
    public class Verb_LaunchProjectileStaticReloadable : Verb_LaunchProjectileStatic
    {
        protected override bool TryCastShot()
        {
            bool baseResult = base.TryCastShot();
            CompRechargeable rechargableComp = this.RechargeableCompSource;
            if (baseResult)
            {
                if (rechargableComp != null)
                    rechargableComp.UsedOnce();
            }
            return baseResult;
        }

        public CompRechargeable RechargeableCompSource
        {
            get
            {
                return this.DirectOwner as CompRechargeable;
            }
        }
    }
}
