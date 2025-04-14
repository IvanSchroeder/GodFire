using UnityEngine;

namespace ExtensionMethods {
    public class LayerMaskHelper {
        public static bool ObjIsInLayerMask(GameObject gameObject, LayerMask layerMask) {
            if ((layerMask.value & (1 << gameObject.layer)) > 0) {
                return true;
            }

            return false;
        }

        public static LayerMask CreateLayerMask(params int[] layers) {
            LayerMask layerMask = 0;
            
            foreach (int layer in layers) {
                layerMask |= (1 << layer);
            }

            return layerMask;
        }
    }
}
