using System.Collections.Generic;
using UnityEngine;

namespace UnityUtilities {
    public static class WaitFor {
		static readonly WaitForFixedUpdate fixedUpdate = new WaitForFixedUpdate();
		public static WaitForFixedUpdate FixedUpdate => fixedUpdate;

		static readonly WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();
		public static WaitForEndOfFrame EndOfFrame => endOfFrame;

		static readonly Dictionary<float, WaitForSeconds> WaitForSecondsDict = new (100, new FloatComparer());
		public static WaitForSeconds Seconds(float seconds) {
			if (seconds < 1f / Application.targetFrameRate) return null;

			if (!WaitForSecondsDict.TryGetValue(seconds, out var forSeconds)) {
				forSeconds = new WaitForSeconds(seconds);
				WaitForSecondsDict[seconds] = forSeconds;
			}

			return forSeconds;
		}
		
		static readonly Dictionary<float, WaitForSecondsRealtime> WaitForSecondsRealtimeDict = new (100, new FloatComparer());
		public static WaitForSecondsRealtime SecondsRealtime(float seconds) {
			if (seconds < 1f / Application.targetFrameRate) return null;

			if (!WaitForSecondsRealtimeDict.TryGetValue(seconds, out var forSecondsReal)) {
				forSecondsReal = new WaitForSecondsRealtime(seconds);
				WaitForSecondsRealtimeDict[seconds] = forSecondsReal;
			}

			return forSecondsReal;
		}

		class FloatComparer : IEqualityComparer<float> {
			public bool Equals(float x, float y) => Mathf.Abs(x - y) <= Mathf.Epsilon;
			public int GetHashCode(float obj) => obj.GetHashCode();
		}
	}
}