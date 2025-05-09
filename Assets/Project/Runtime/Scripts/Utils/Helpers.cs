using UnityEngine;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityUtilities {
    public static class Helpers {
		public static WaitForSeconds GetWaitForSeconds(float seconds) {
			return WaitFor.Seconds(seconds);
		}

		public static WaitForSecondsRealtime GetWaitForSecondsRealtime(float seconds) {
			return WaitFor.SecondsRealtime(seconds);
		}

		/// <summary>
        /// Clears the console log in the Unity Editor.
        /// </summary
#if UNITY_EDITOR        
        public static void ClearConsole() {
            var assembly = Assembly.GetAssembly(typeof(SceneView));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method?.Invoke(new object(), null);
        }
#endif        
    }
}