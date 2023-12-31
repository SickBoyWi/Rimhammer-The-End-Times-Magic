using System.Collections.Generic;
using UnityEngine;

namespace TheEndTimes_Magic
{
    public static class InterpolationCurves
    {
        public static readonly Dictionary<CurveType, InterpolationCurves.Curve> AllCurves = new Dictionary<CurveType, InterpolationCurves.Curve>()
    {
      {
        CurveType.Linear,
        new InterpolationCurves.Curve(InterpolationCurves.Linear)
      },
      {
        CurveType.QuadraticIn,
        new InterpolationCurves.Curve(InterpolationCurves.Quadratic.In)
      },
      {
        CurveType.QuadraticOut,
        new InterpolationCurves.Curve(InterpolationCurves.Quadratic.Out)
      },
      {
        CurveType.QuadraticInOut,
        new InterpolationCurves.Curve(InterpolationCurves.Quadratic.InOut)
      },
      {
        CurveType.CubicIn,
        new InterpolationCurves.Curve(InterpolationCurves.Cubic.In)
      },
      {
        CurveType.CubicOut,
        new InterpolationCurves.Curve(InterpolationCurves.Cubic.Out)
      },
      {
        CurveType.CubicInOut,
        new InterpolationCurves.Curve(InterpolationCurves.Cubic.InOut)
      },
      {
        CurveType.QuarticIn,
        new InterpolationCurves.Curve(InterpolationCurves.Quartic.In)
      },
      {
        CurveType.QuarticOut,
        new InterpolationCurves.Curve(InterpolationCurves.Quartic.Out)
      },
      {
        CurveType.QuarticInOut,
        new InterpolationCurves.Curve(InterpolationCurves.Quartic.InOut)
      },
      {
        CurveType.QuinticIn,
        new InterpolationCurves.Curve(InterpolationCurves.Quintic.In)
      },
      {
        CurveType.QuinticOut,
        new InterpolationCurves.Curve(InterpolationCurves.Quintic.Out)
      },
      {
        CurveType.QuinticInOut,
        new InterpolationCurves.Curve(InterpolationCurves.Quintic.InOut)
      },
      {
        CurveType.SinusoidalIn,
        new InterpolationCurves.Curve(InterpolationCurves.Sinusoidal.In)
      },
      {
        CurveType.SinusoidalOut,
        new InterpolationCurves.Curve(InterpolationCurves.Sinusoidal.Out)
      },
      {
        CurveType.SinusoidalInOut,
        new InterpolationCurves.Curve(InterpolationCurves.Sinusoidal.InOut)
      },
      {
        CurveType.ExponentialIn,
        new InterpolationCurves.Curve(InterpolationCurves.Exponential.In)
      },
      {
        CurveType.ExponentialOut,
        new InterpolationCurves.Curve(InterpolationCurves.Exponential.Out)
      },
      {
        CurveType.ExponentialInOut,
        new InterpolationCurves.Curve(InterpolationCurves.Exponential.InOut)
      },
      {
        CurveType.CircularIn,
        new InterpolationCurves.Curve(InterpolationCurves.Circular.In)
      },
      {
        CurveType.CircularOut,
        new InterpolationCurves.Curve(InterpolationCurves.Circular.Out)
      },
      {
        CurveType.CircularInOut,
        new InterpolationCurves.Curve(InterpolationCurves.Circular.InOut)
      },
      {
        CurveType.ElasticIn,
        new InterpolationCurves.Curve(InterpolationCurves.Elastic.In)
      },
      {
        CurveType.ElasticOut,
        new InterpolationCurves.Curve(InterpolationCurves.Elastic.Out)
      },
      {
        CurveType.ElasticInOut,
        new InterpolationCurves.Curve(InterpolationCurves.Elastic.InOut)
      },
      {
        CurveType.BackIn,
        new InterpolationCurves.Curve(InterpolationCurves.Back.In)
      },
      {
        CurveType.BackOut,
        new InterpolationCurves.Curve(InterpolationCurves.Back.Out)
      },
      {
        CurveType.BackInOut,
        new InterpolationCurves.Curve(InterpolationCurves.Back.InOut)
      },
      {
        CurveType.BounceIn,
        new InterpolationCurves.Curve(InterpolationCurves.Bounce.In)
      },
      {
        CurveType.BounceOut,
        new InterpolationCurves.Curve(InterpolationCurves.Bounce.Out)
      },
      {
        CurveType.BounceInOut,
        new InterpolationCurves.Curve(InterpolationCurves.Bounce.InOut)
      }
    };

        public static float Linear(float t)
        {
            return t;
        }

        public delegate float Curve(float time);

        public class Quadratic
        {
            public static float In(float t)
            {
                return t * t;
            }

            public static float Out(float t)
            {
                return t * (2f - t);
            }

            public static float InOut(float t)
            {
                return (double)(t *= 2f) < 1.0 ? 0.5f * t * t : (float)(-0.5 * ((double)--t * ((double)t - 2.0) - 1.0));
            }
        }

        public class Cubic
        {
            public static float In(float t)
            {
                return t * t * t;
            }

            public static float Out(float t)
            {
                return (float)(1.0 + (double)--t * (double)t * (double)t);
            }

            public static float InOut(float t)
            {
                return (double)(t *= 2f) < 1.0 ? 0.5f * t * t * t : (float)(0.5 * ((double)(t -= 2f) * (double)t * (double)t + 2.0));
            }
        }

        public class Quartic
        {
            public static float In(float t)
            {
                return t * t * t * t;
            }

            public static float Out(float t)
            {
                return (float)(1.0 - (double)--t * (double)t * (double)t * (double)t);
            }

            public static float InOut(float t)
            {
                return (double)(t *= 2f) < 1.0 ? 0.5f * t * t * t * t : (float)(-0.5 * ((double)(t -= 2f) * (double)t * (double)t * (double)t - 2.0));
            }
        }

        public class Quintic
        {
            public static float In(float t)
            {
                return t * t * t * t * t;
            }

            public static float Out(float t)
            {
                return (float)(1.0 + (double)--t * (double)t * (double)t * (double)t * (double)t);
            }

            public static float InOut(float t)
            {
                return (double)(t *= 2f) < 1.0 ? 0.5f * t * t * t * t * t : (float)(0.5 * ((double)(t -= 2f) * (double)t * (double)t * (double)t * (double)t + 2.0));
            }
        }

        public class Sinusoidal
        {
            public static float In(float t)
            {
                return 1f - Mathf.Cos((float)((double)t * 3.14159274101257 / 2.0));
            }

            public static float Out(float t)
            {
                return Mathf.Sin((float)((double)t * 3.14159274101257 / 2.0));
            }

            public static float InOut(float t)
            {
                return (float)(0.5 * (1.0 - (double)Mathf.Cos(3.141593f * t)));
            }
        }

        public class Exponential
        {
            public static float In(float t)
            {
                return (double)t == 0.0 ? 0.0f : Mathf.Pow(1024f, t - 1f);
            }

            public static float Out(float t)
            {
                return (double)t == 1.0 ? 1f : 1f - Mathf.Pow(2f, -10f * t);
            }

            public static float InOut(float t)
            {
                if ((double)t == 0.0)
                    return 0.0f;
                if ((double)t == 1.0)
                    return 1f;
                return (double)(t *= 2f) < 1.0 ? 0.5f * Mathf.Pow(1024f, t - 1f) : (float)(0.5 * (-(double)Mathf.Pow(2f, (float)(-10.0 * ((double)t - 1.0))) + 2.0));
            }
        }

        public class Circular
        {
            public static float In(float t)
            {
                return 1f - Mathf.Sqrt((float)(1.0 - (double)t * (double)t));
            }

            public static float Out(float t)
            {
                return Mathf.Sqrt((float)(1.0 - (double)--t * (double)t));
            }

            public static float InOut(float t)
            {
                return (double)(t *= 2f) < 1.0 ? (float)(-0.5 * ((double)Mathf.Sqrt((float)(1.0 - (double)t * (double)t)) - 1.0)) : (float)(0.5 * ((double)Mathf.Sqrt((float)(1.0 - (double)(t -= 2f) * (double)t)) + 1.0));
            }
        }

        public class Elastic
        {
            public static float In(float t)
            {
                if ((double)t == 0.0)
                    return 0.0f;
                return (double)t == 1.0 ? 1f : -Mathf.Pow(2f, 10f * --t) * Mathf.Sin((float)(((double)t - 0.100000001490116) * 6.28318548202515 / 0.400000005960464));
            }

            public static float Out(float t)
            {
                if ((double)t == 0.0)
                    return 0.0f;
                return (double)t == 1.0 ? 1f : (float)((double)Mathf.Pow(2f, -10f * t) * (double)Mathf.Sin((float)(((double)t - 0.100000001490116) * 6.28318548202515 / 0.400000005960464)) + 1.0);
            }

            public static float InOut(float t)
            {
                return (double)(t *= 2f) < 1.0 ? -0.5f * Mathf.Pow(2f, 10f * --t) * Mathf.Sin((float)(((double)t - 0.100000001490116) * 6.28318548202515 / 0.400000005960464)) : (float)((double)Mathf.Pow(2f, -10f * --t) * (double)Mathf.Sin((float)(((double)t - 0.100000001490116) * 6.28318548202515 / 0.400000005960464)) * 0.5 + 1.0);
            }
        }

        public class Back
        {
            private static float s = 1.70158f;
            private static float s2 = 2.594909f;

            public static float In(float t)
            {
                return (float)((double)t * (double)t * (((double)InterpolationCurves.Back.s + 1.0) * (double)t - (double)InterpolationCurves.Back.s));
            }

            public static float Out(float t)
            {
                return (float)((double)--t * (double)t * (((double)InterpolationCurves.Back.s + 1.0) * (double)t + (double)InterpolationCurves.Back.s) + 1.0);
            }

            public static float InOut(float t)
            {
                return (double)(t *= 2f) < 1.0 ? (float)(0.5 * ((double)t * (double)t * (((double)InterpolationCurves.Back.s2 + 1.0) * (double)t - (double)InterpolationCurves.Back.s2))) : (float)(0.5 * ((double)(t -= 2f) * (double)t * (((double)InterpolationCurves.Back.s2 + 1.0) * (double)t + (double)InterpolationCurves.Back.s2) + 2.0));
            }
        }

        public class Bounce
        {
            public static float In(float t)
            {
                return 1f - InterpolationCurves.Bounce.Out(1f - t);
            }

            public static float Out(float t)
            {
                if ((double)t < 0.363636374473572)
                    return 121f / 16f * t * t;
                if ((double)t < 0.727272748947144)
                    return (float)(121.0 / 16.0 * (double)(t -= 0.5454546f) * (double)t + 0.75);
                return (double)t < 0.909090936183929 ? (float)(121.0 / 16.0 * (double)(t -= 0.8181818f) * (double)t + 15.0 / 16.0) : (float)(121.0 / 16.0 * (double)(t -= 0.9545454f) * (double)t + 63.0 / 64.0);
            }

            public static float InOut(float t)
            {
                return (double)t < 0.5 ? InterpolationCurves.Bounce.In(t * 2f) * 0.5f : (float)((double)InterpolationCurves.Bounce.Out((float)((double)t * 2.0 - 1.0)) * 0.5 + 0.5);
            }
        }
    }
}
