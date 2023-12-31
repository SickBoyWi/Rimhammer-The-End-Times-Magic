using HugsLib.Utils;
using System;
using System.Reflection;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    public class ValueInterpolator : IExposable
    {
        public bool finished = true;
        public float value;
        public bool respectTimeScale;
        private float elapsedTime;
        private float initialValue;
        private float targetValue;
        private float duration;
        private string curveName;
        private InterpolationCurves.Curve curve;
        private ValueInterpolator.FinishedCallback callback;

        public ValueInterpolator()
        {
        }

        public ValueInterpolator(float value = 0.0f)
        {
            this.value = value;
        }

        public ValueInterpolator StartInterpolation(
          float finalValue,
          float interpolationDuration,
          InterpolationCurves.Curve curveDelegate)
        {
            this.initialValue = this.value;
            this.elapsedTime = 0.0f;
            this.targetValue = finalValue;
            this.duration = interpolationDuration;
            this.curve = curveDelegate;
            this.finished = false;
            return this;
        }

        public ValueInterpolator StartInterpolation(
          float finalValue,
          float interpolationDuration,
          CurveType curveType)
        {
            return this.StartInterpolation(finalValue, interpolationDuration, InterpolationCurves.AllCurves[curveType]);
        }

        public void ExposeData()
        {
            Scribe_Values.Look<float>(ref this.value, "value", 0.0f, false);
            Scribe_Values.Look<bool>(ref this.finished, "finished", true, false);
            Scribe_Values.Look<bool>(ref this.respectTimeScale, "respectTimeScale", true, false);
            Scribe_Values.Look<float>(ref this.elapsedTime, "elapsedTime", 0.0f, false);
            Scribe_Values.Look<float>(ref this.initialValue, "initialValue", 0.0f, false);
            Scribe_Values.Look<float>(ref this.targetValue, "targetValue", 0.0f, false);
            Scribe_Values.Look<float>(ref this.duration, "duration", 0.0f, false);
            Scribe_Values.Look<float>(ref this.duration, "duration", 0.0f, false);
            if (Scribe.mode == LoadSaveMode.Saving)
                this.curveName = this.curve?.Method.Name;
            Scribe_Values.Look<string>(ref this.curveName, "curveName", (string)null, false);
            if (Scribe.mode != LoadSaveMode.LoadingVars || this.curveName.NullOrEmpty())
                return;
            MethodInfo method = typeof(InterpolationCurves).GetMethod(this.curveName, (BindingFlags)HugsLibUtility.AllBindingFlags);
            if (method == (MethodInfo)null)
                Log.Error("Failed to load interpolation curve: " + this.curveName);
            else
                this.curve = (InterpolationCurves.Curve)Delegate.CreateDelegate(typeof(InterpolationCurves.Curve), method, true);
        }

        public ValueInterpolator SetFinishedCallback(
          ValueInterpolator.FinishedCallback finishedCallback)
        {
            this.callback = finishedCallback;
            return this;
        }

        public float UpdateIfUnpaused()
        {
            return Find.TickManager.Paused ? this.value : this.Update();
        }

        public float Update()
        {
            if (this.finished)
                return this.value;
            float deltaTime = Time.deltaTime;
            if (this.respectTimeScale)
                deltaTime *= Find.TickManager.TickRateMultiplier;
            this.elapsedTime += deltaTime;
            if ((double)this.elapsedTime >= (double)this.duration)
            {
                this.elapsedTime = this.duration;
                this.value = this.targetValue;
                this.finished = true;
                ValueInterpolator.FinishedCallback callback = this.callback;
                if (callback != null)
                    callback(this, this.value, this.duration, this.curve);
            }
            else
                this.value = this.initialValue + this.curve(this.elapsedTime / this.duration) * (this.targetValue - this.initialValue);
            return this.value;
        }

        public static implicit operator float(ValueInterpolator v)
        {
            return v.value;
        }

        public delegate void FinishedCallback(
          ValueInterpolator interpolator,
          float finalValue,
          float interpolationDuration,
          InterpolationCurves.Curve interpolationCurve);
    }
}
