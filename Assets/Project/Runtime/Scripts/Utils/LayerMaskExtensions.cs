using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtilities {
    /// <summary>
	/// Layers extensions.
	/// </summary>
    public static class LayerMaskExtensions {
        private static List<int> LayerNumbersList;
        private static List<string> LayerNamesList;
        private static long lastUpdateTick;
        
        /// <summary>
        /// Checks if the given layer number is contained in the LayerMask.
        /// </summary>
        /// <param name="mask">The LayerMask to check.</param>
        /// <param name="layerNumber">The layer number to check if it is contained in the LayerMask.</param>
        /// <returns>True if the layer number is contained in the LayerMask, otherwise false.</returns>
        public static bool Contains(this LayerMask mask, int layerNumber) {
            return mask == (mask | (1 << layerNumber));
        }

		public static bool HasLayer(this LayerMask layerMask, int layer) {
			if (layerMask == (layerMask | (1 << layer))) {
				return true;
			}

			return false;
		}

		public static bool[] HasLayers(this LayerMask layerMask) {
			var hasLayers = new bool[32];

			for (int i = 0; i < 32; i++) {
				if (layerMask == (layerMask | (1 << i))) {
					hasLayers[i] = true;
				}
			}

			return hasLayers;
		}

		public static int AddLayerToLayerMask(this LayerMask layerMask, int layer) => layerMask |= 1 << layer;
		public static int RemoveLayerFromLayerMask(this LayerMask layerMask, int layer) => layerMask &= ~(1 << layer);
		public static int AddLayerToLayerMaskInt(this int layerMask, int layer) => layerMask |= 1 << layer;
		public static int RemoveLayerFromLayerMaskInt(this int layerMask, int layer) => layerMask &= ~(1 << layer);
		public static int AddLayerToCullingMask(this Camera camera, int layer) => camera.cullingMask |= 1 << layer;
		public static int RemoveLayerFromCullingMask(this Camera camera, int layer) => camera.cullingMask &= ~(1 << layer);

		public static LayerMask Inverse(this LayerMask mask) => ~mask;
		public static LayerMask Inverse(this int mask) => ~mask;

		private static void TestUpdateLayers() {
            if (LayerNumbersList == null || (DateTime.UtcNow.Ticks - lastUpdateTick > 10000000L && Event.current.type == EventType.Layout)) {
                lastUpdateTick = DateTime.UtcNow.Ticks;
                if (LayerNumbersList == null) {
                    LayerNumbersList = new List<int>();
                    LayerNamesList = new List<string>();
                }
                else {
                    LayerNumbersList.Clear();
                    LayerNamesList.Clear();
                }

                for (int i = 0; i < 32; i++) {
                    string layerName = LayerMask.LayerToName(i);

                    if (layerName != "") {
                        LayerNumbersList.Add(i);
                        LayerNamesList.Add(layerName);
                    }
                }
            }
        }

		public static string[] GetLayerNames() {
            TestUpdateLayers();
            return LayerNamesList.ToArray();
        }

        public static string[] GetAllLayerNames() {
            TestUpdateLayers();
            string[] names = new string[32];

            for (int i = 0; i < 32; i++) {
                if (LayerNumbersList.Contains(i)) names[i] = LayerMask.LayerToName(i);
                else names[i] = "Layer " + i.ToString();
            }

            return names;
        }

		public static bool Intersects(this LayerMask mask, int layer) => (mask.value & (1 << layer)) != 0;
		public static bool Intersects(this LayerMask mask, LayerMask layers) => (mask.value & (1 << layers)) != 0;
        public static bool Intersects(this LayerMask mask, GameObject go) => (mask.value & (1 << go.layer)) != 0;
        public static bool Intersects(this LayerMask mask, Component c) => (mask.value & (1 << c.gameObject.layer)) != 0;
    }
}