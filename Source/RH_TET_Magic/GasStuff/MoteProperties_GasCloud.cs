using Verse;

namespace TheEndTimes_Magic
{
    public class MoteProperties_GasCloud : MoteProperties
    {
        public int GastickInterval = 1;
        public int SpreadInterval = 1;
        public float SpreadAmountMultiplier = 1f;
        public float SpreadMinConcentration = 1f;
        public float FullAlphaConcentration = 1f;
        public float RoofedDissipation;
        public float UnroofedDissipation;
        public float AnimationAmplitude;
        public FloatRange AnimationPeriod;

        public virtual void PostLoad()
        {
            this.Assert(this.GastickInterval > 0, "GastickInterval must be greater than zero");
            this.Assert(this.SpreadInterval > 0, "SpreadInterval must be greater than zero");
            this.Assert((double)this.SpreadAmountMultiplier >= 0.0 && (double)this.SpreadAmountMultiplier <= 1.0, "SpreadAmountMultiplier must be between 0 and 1");
            this.Assert((double)this.SpreadMinConcentration > 0.0, "SpreadMinConcentration must be greater than zero");
            this.Assert((double)this.FullAlphaConcentration > 0.0, "FullAlphaConcentration must be greater than zero");
            this.Assert((double)this.RoofedDissipation >= 0.0, "RoofedDissipation must be at least zero");
            this.Assert((double)this.UnroofedDissipation >= 0.0, "RoofedDissipation must be at least zero");
            this.Assert((double)this.AnimationAmplitude >= 0.0, "AnimationAmplitude must be at least zero");
        }

        protected void Assert(bool check, string errorMessage)
        {
            if (check)
                return;
            Log.Error("[RemoteTech] Invalid data in " + this.GetType().Name + " definition: " + errorMessage, false);
        }
    }
}
