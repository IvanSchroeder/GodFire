using System;
using UnityEngine;
#if ENABLED_UNITY_MATHEMATICS
using Unity.Mathematics;
#endif

// https://github.com/adammyhre/Unity-Utils/blob/master/UnityUtils/Scripts/Extensions/NumberExtensions.cs

namespace UnityUtilities {
    /// <summary>
	/// Numbers Extensions.
	/// </summary>
    public static class NumberExtensions {
        public static float PowerOf(this int i, int p) => Mathf.Pow(i, p);
        public static float PowerOf(this float i, float p) => Mathf.Pow(i, p);

        public static float PercentageOf(this int part, int whole) {
            if (whole == 0) return 0; // Handling division by zero
            return (float) part / whole;
        }

        public static bool Approx(this float f1, float f2) => Mathf.Approximately(f1, f2);
        public static bool IsOdd(this int i) => i % 2 == 1;
        public static bool IsEven(this int i) => i % 2 == 0;

        public static int AtLeast(this int value, int min) => Mathf.Max(value, min);
        public static int AtMost(this int value, int max) => Mathf.Min(value, max);

#if ENABLED_UNITY_MATHEMATICS
        public static half AtLeast(this half value, half max) => MathfExtension.Max(value, max);
        public static half AtMost(this half value, half max) => MathfExtension.Min(value, max);
#endif

        public static float AtLeast(this float value, float min) => Mathf.Max(value, min);
        public static float AtMost(this float value, float max) => Mathf.Min(value, max);

        public static double AtLeast(this double value, double min) => MathfExtensions.Max(value, min);
        public static double AtMost(this double value, double min) => MathfExtensions.Min(value, min);

        /// <summary>
        /// Returns a random float from min (inclusive) to max (exclusive)
        /// </summary>
        /// <param name="random">The given random instance</param>
        /// <param name="min">The inclusive minimum bound</param>
        /// <param name="max">The exclusive maximum bound.  Must be greater than min</param>
        public static float NextFloat(this System.Random random, float min, float max) {
            if (max <= min)
                throw new ArgumentOutOfRangeException("max", "max must be > min!");

            float range = (float)(max - min);

            float floatRand;
            do
            {
                byte[] buf = new byte[4];
                random.NextBytes(buf);
                floatRand = (float)BitConverter.ToSingle(buf, 0);
            } while (floatRand > float.MaxValue - ((float.MaxValue % range) + 1) % range);

            return (float)(floatRand % range) + min;
        }

        /// <summary>
        /// Returns a random float from 0 (inclusive) to max (exclusive)
        /// </summary>
        /// <param name="random">The given random instance</param>
        /// <param name="max">The exclusive maximum bound.  Must be greater than 0</param>
        public static float NextFloat(this System.Random random, float max) {
            return random.NextFloat(0, max);
        }

        /// <summary>
        /// Returns a random float over all possible values of float (except float.MaxValue, similar to random.Next())
        /// </summary>
        /// <param name="random">The given random instance</param>
        public static float NextFloat(this System.Random random) {
            return random.NextFloat(float.MinValue, float.MaxValue);
        }

        /// <summary>
        /// Returns a random long from min (inclusive) to max (exclusive)
        /// </summary>
        /// <param name="random">The given random instance</param>
        /// <param name="min">The inclusive minimum bound</param>
        /// <param name="max">The exclusive maximum bound.  Must be greater than min</param>
        public static long NextLong(this System.Random random, long min, long max) {
            if (max <= min)
                throw new ArgumentOutOfRangeException("max", "max must be > min!");

            //Working with ulong so that modulo works correctly with values > long.MaxValue
            ulong uRange = (ulong)(max - min);

            //Prevent a modolo bias; see https://stackoverflow.com/a/10984975/238419
            //for more information.
            //In the worst case, the expected number of calls is 2 (though usually it's
            //much closer to 1) so this loop doesn't really hurt performance at all.
            ulong ulongRand;
            do
            {
                byte[] buf = new byte[8];
                random.NextBytes(buf);
                ulongRand = (ulong)BitConverter.ToInt64(buf, 0);
            } while (ulongRand > ulong.MaxValue - ((ulong.MaxValue % uRange) + 1) % uRange);

            return (long)(ulongRand % uRange) + min;
        }

        /// <summary>
        /// Returns a random long from 0 (inclusive) to max (exclusive)
        /// </summary>
        /// <param name="random">The given random instance</param>
        /// <param name="max">The exclusive maximum bound.  Must be greater than 0</param>
        public static long NextLong(this System.Random random, long max) {
            return random.NextLong(0, max);
        }

        /// <summary>
        /// Returns a random long over all possible values of long (except long.MaxValue, similar to random.Next())
        /// </summary>
        /// <param name="random">The given random instance</param>
        public static long NextLong(this System.Random random) {
            return random.NextLong(long.MinValue, long.MaxValue);
        }
    }
}