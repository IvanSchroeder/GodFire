using UnityEngine;
using System.Linq;
using UnityUtilities;

namespace UnityUtilities {
    public static class GameObjectExtensions {
        /// <summary>
        /// This method is used to hide the GameObject in the Hierarchy view.
        /// </summary>
        /// <param name="gameObject"></param>
        public static void HideInHierarchy(this GameObject gameObject) {
            gameObject.hideFlags = HideFlags.HideInHierarchy;
        }

        /// <summary>
        /// Gets a component of the given type attached to the GameObject. If that type of component does not exist, it adds one.
        /// </summary>
        /// <remarks>
        /// This method is useful when you don't know if a GameObject has a specific type of component,
        /// but you want to work with that component regardless. Instead of checking and adding the component manually,
        /// you can use this method to do both operations in one line.
        /// </remarks>
        /// <typeparam name="T">The type of the component to get or add.</typeparam>
        /// <param name="gameObject">The GameObject to get the component from or add the component to.</param>
        /// <returns>The existing component of the given type, or a new one if no such component exists.</returns>    
        public static T GetOrAdd<T>(this GameObject gameObject) where T : Component {
            T component = gameObject.GetComponent<T>();
            if (!component) component = gameObject.AddComponent<T>();

            return component;
        }

        /// <summary>
        /// Returns the object itself if it exists, null otherwise.
        /// </summary>
        /// <remarks>
        /// This method helps differentiate between a null reference and a destroyed Unity object. Unity's "== null" check
        /// can incorrectly return true for destroyed objects, leading to misleading behaviour. The OrNull method use
        /// Unity's "null check", and if the object has been marked for destruction, it ensures an actual null reference is returned,
        /// aiding in correctly chaining operations and preventing NullReferenceExceptions.
        /// </remarks>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="obj">The object being checked.</param>
        /// <returns>The object itself if it exists and not destroyed, null otherwise.</returns>
        public static T OrNull<T>(this T obj) where T : Object => obj ? obj : null;

        /// <summary>
        /// Destroys all children of the game object
        /// </summary>
        /// <param name="gameObject">GameObject whose children are to be destroyed.</param>
        public static void DestroyChildren(this GameObject gameObject) {
            gameObject.transform.DestroyChildren();
        }

        /// <summary>
        /// Immediately destroys all children of the given GameObject.
        /// </summary>
        /// <param name="gameObject">GameObject whose children are to be destroyed.</param>
        public static void DestroyChildrenImmediate(this GameObject gameObject) {
            gameObject.transform.DestroyChildrenImmediate();
        }

        /// <summary>
        /// Enables all child GameObjects associated with the given GameObject.
        /// </summary>
        /// <param name="gameObject">GameObject whose child GameObjects are to be enabled.</param>
        public static void EnableChildren(this GameObject gameObject) {
            gameObject.transform.EnableChildren();
        }

        /// <summary>
        /// Disables all child GameObjects associated with the given GameObject.
        /// </summary>
        /// <param name="gameObject">GameObject whose child GameObjects are to be disabled.</param>
        public static void DisableChildren(this GameObject gameObject) {
            gameObject.transform.DisableChildren();
        }

        /// <summary>
        /// Resets the GameObject's transform's position, rotation, and scale to their default values.
        /// </summary>
        /// <param name="gameObject">GameObject whose transformation is to be reset.</param>
        public static void ResetTransformation(this GameObject gameObject) {
            gameObject.transform.Reset();
        }

        /// <summary>
        /// Returns the hierarchical path in the Unity scene hierarchy for this GameObject.
        /// </summary>
        /// <param name="gameObject">The GameObject to get the path for.</param>
        /// <returns>A string representing the full hierarchical path of this GameObject in the Unity scene.
        /// This is a '/'-separated string where each part is the name of a parent, starting from the root parent and ending
        /// with the name of the specified GameObjects parent.</returns>
        public static string Path(this GameObject gameObject) {
            return "/" + string.Join("/",
                gameObject.GetComponentsInParent<Transform>().Select(t => t.name).Reverse().ToArray());
        }

        /// <summary>
        /// Returns the full hierarchical path in the Unity scene hierarchy for this GameObject.
        /// </summary>
        /// <param name="gameObject">The GameObject to get the path for.</param>
        /// <returns>A string representing the full hierarchical path of this GameObject in the Unity scene.
        /// This is a '/'-separated string where each part is the name of a parent, starting from the root parent and ending
        /// with the name of the specified GameObject itself.</returns>
        public static string PathFull(this GameObject gameObject) {
            return gameObject.Path() + "/" + gameObject.name;
        }

        /// <summary>
        /// Recursively sets the provided layer for this GameObject and all of its descendants in the Unity scene hierarchy.
        /// </summary>
        /// <param name="gameObject">The GameObject to set layers for.</param>
        /// <param name="layer">The layer number to set for GameObject and all of its descendants.</param>
        public static void SetLayersRecursively(this GameObject gameObject, int layer) {
            gameObject.layer = layer;
            gameObject.transform.ForEveryChild(child => child.gameObject.SetLayersRecursively(layer));
        }
        
        /// <summary>
        /// Activates the GameObject associated with the MonoBehaviour and returns the instance.
        /// </summary>
        /// <typeparam name="T">The type of the MonoBehaviour.</typeparam>
        /// <param name="obj">The MonoBehaviour whose GameObject will be activated.</param>
        /// <returns>The instance of the MonoBehaviour.</returns>
        public static T SetActive<T>(this T obj) where T : MonoBehaviour {
            obj.gameObject.SetActive(true);
            return obj;
        }

        /// <summary>
        /// Deactivates the GameObject associated with the MonoBehaviour and returns the instance.
        /// </summary>
        /// <typeparam name="T">The type of the MonoBehaviour.</typeparam>
        /// <param name="obj">The MonoBehaviour whose GameObject will be deactivated.</param>
        /// <returns>The instance of the MonoBehaviour.</returns>
        public static T SetInactive<T>(this T obj) where T : MonoBehaviour {
            obj.gameObject.SetActive(false);
            return obj;
        }

        public static bool NullSafeEquals(this object a, object other) {
			return (a == null && other == null) || a.Equals( other );
		}

		public static void Destroy(this GameObject obj, float? inSeconds = null) {
            Object.Destroy(obj, inSeconds ?? 0f);
		}

		public static int GetCollisionMask(this GameObject gameObject, int layer = -1) {
			if (layer == -1) {
				layer = gameObject.layer;
			}

			int mask = 0;
			for (int i = 0; i < 32; i++) {
				mask |= (Physics.GetIgnoreLayerCollision(layer, i) ? 0:1) << i;
			}

			return mask;
		}

        public static bool IntersectsLayerMask(this GameObject obj, int layerMask) {
            if (obj == null) return false;
            return ((1 << obj.layer) & layerMask) != 0;
        }

        /// <summary>
	    /// Object extensions.
	    /// https://github.com/lordofduct/spacepuppy-unity-framework/blob/master/SpacepuppyBase/Utils/ObjUtil.cs
	    /// </summary>
    #region ===== Objects =====
        /// <summary>
		/// Checks whether the given object is of {T}.
		/// </summary>
		/// <param name="obj">The object to be checked.</param>
		/// <typeparam name="T">Refers the target data type.</typeparam>
		/// <returns>True if the given object is of type T, false otherwise.</returns>
		public static bool IsA<T>(this object obj) => obj is T;

		/// <summary>
		/// Checks whether the given object is NOT of type T.
		/// </summary>
		/// <param name="obj">The object to be checked.</param>
		/// <typeparam name="T">Refers the target data type.</typeparam>
		/// <returns>True if the given object is NOT of type T, false otherwise.</returns>
		public static bool IsNotA<T>(this object obj) => obj.IsA<T>().Toggle();

		/// <summary>
		/// Tries to cast the given object to type T
		/// </summary>
		/// <param name="obj">The object to be casted.</param>
		/// <typeparam name="T">Refers target data type.</typeparam>
		/// <returns>Returns the casted objects. Null if casting fails.</returns>
		public static T As<T>(this object obj) where T : class => obj as T;

		/// <summary>
		/// Checks whether the given object is Null.
		/// </summary>
		/// <param name="obj">The object to be checked.</param>
		/// <returns>True if the object is Null, false otherwise.</returns>
		public static bool IsNull<T>(this T obj) where T : Object => obj ? false : true;
		public static bool IsNull(this object obj) => obj != null ? false : true;

		/// <summary>
		/// Checks whether the given object is NOT Null.
		/// </summary>
		/// <param name="obj">The object to be checked.</param>
		/// <returns>True if the object is NOT Null, false otherwise.</returns>
		public static bool IsNotNull<T>(this T obj) where T : Object => obj ? true : false;
        public static bool IsNotNull(this object obj) => obj != null ? true : false;

		/// <summary>
		/// Makes a copy from the object.
		/// Doesn't copy the reference memory, only data.
		/// </summary>
		/// <typeparam name="T">Type of the return object.</typeparam>
		/// <param name="item">Object to be copied.</param>
		/// <returns>Returns the copied object.</returns>
		public static T Clone<T>(this object item) where T : class => (item != null) ? item.XMLSerialize_ToString().XMLDeserialize_ToObject<T>() : default(T);
    #endregion

    /// <summary>
	/// Extensions for Prefabs.
	/// </summary>
	#region ===== Prefabs =====
		public static GameObject Create(GameObject prefab) => Object.Instantiate(prefab);
        public static GameObject Create(GameObject prefab, Vector3 pos, Quaternion rot) => Object.Instantiate(prefab, pos, rot);
        public static GameObject Create(GameObject prefab, Transform parent = null) {
            if (parent == null) return Create(prefab);
            //NOTE - this appears to work, thanks to help from @Polymorphik
            bool isActive = prefab.activeSelf;
            prefab.SetActive(false);
            var result = Object.Instantiate(prefab, parent.position, parent.rotation);
            result.transform.parent = parent;
            result.SetActive(isActive);
            prefab.SetActive(isActive);
            return result;
        }
		public static GameObject Create(GameObject prefab, Vector3 pos, Quaternion rot, Transform parent) {
            if (parent == null) return Create(prefab, pos, rot);
            //NOTE - this appears to work, thanks to help from @Polymorphik
            bool isActive = prefab.activeSelf;
            prefab.SetActive(false);
            var result = Object.Instantiate(prefab, pos, rot);
            result.transform.parent = parent;
            result.SetActive(isActive);
            prefab.SetActive(isActive);
            return result;
        }
	#endregion
    }
}