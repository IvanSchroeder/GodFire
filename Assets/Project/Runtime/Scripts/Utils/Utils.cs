using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Threading;

namespace UnityUtilities {
	/// <summary>
	/// Extension methods for Unity.
	/// https://github.com/lordofduct/spacepuppy-unity-framework/tree/master/SpacepuppyBase/Utils
	/// https://github.com/rocketgames/Extension-Methods-for-Unity
	/// </summary>
	public static class Utils {
		/// Constants
		/// /// <summary>
		/// Number pi.
		/// </summary>
        public const float PI = 3.14159265358979f;
		/// /// <summary>
		/// PI / 2 OR 90 deg.
		/// </summary>
        public const float PI_2 = 1.5707963267949f;
		/// /// <summary>
		/// PI / 2 OR 60 deg.
		/// </summary>
        public const float PI_3 = 1.04719755119659666667f;
		/// /// <summary>
		/// PI / 4 OR 45 deg.
		/// </summary>
        public const float PI_4 = 0.785398163397448f;
		/// /// <summary>
		/// PI / 8 OR 22.5 deg.
		/// </summary>
        public const float PI_8 = 0.392699081698724f;
		/// /// <summary>
		/// PI / 16 OR 11.25 deg.
		/// </summary>
        public const float PI_16 = 0.196349540849362f;
		/// /// <summary>
		/// 2 * PI OR 180 deg.
		/// </summary>
        public const float TWO_PI = 6.28318530717959f;
		/// /// <summary>
		/// 3 * PI_2 OR 270 deg.
		/// </summary>
        public const float THREE_PI_2 = 4.71238898038469f;
		/// /// <summary>
		/// PI / 180.
		/// </summary>
		public const float DEG_TO_RAD = 0.0174532925199433f;
		/// /// <summary>
		/// 180.0 / PI.
		/// </summary>
		public const float RAD_TO_DEG = 57.2957795130823f;
		/// /// <summary>
		/// Single float average epsilon.
		/// </summary>
        public const float EPSILON = 0.0001f;

		// this...
		// public static event Action OnGameOver;
		// is the sames as this...
		// public delegate void OnGameOver();
		// public static event OnGameOver onGameOver;

		// for smooth step lerping : floa t = time / duration (percentageComplete) => t = t * t * (3f - 2f * t);

		// private float targetValue;
		// private float valueToChange;

		// void Start() {
		//     StartCoroutine(LerpFunction(targetValue, 5));
		// }

		// IEnumerator LerpFunction(float endValue, float duration) {
		//     float time = 0f;
		//     float startValue = valueToChange;

		//     while (time < duration) {
		//         valueToChange = Mathf.Lerp(startValue, endValue, time / duration);
		//         time += Time.deltaTime;

		//         yield return null;
		//     }
		//     valueToChange = endValue;
		// }

	/// <summary>
	/// Extensions for Camera.
	/// </summary>
	#region ===== Camera =====
		public static Camera GetMainCamera() => Camera.main;
		public static Camera GetMainCamera(this Camera mainCamera) => Camera.main;
		public static Camera GetMainCamera(this Component component) => Camera.main;

		public static Vector3 ScreenToWorld(this Camera camera, Vector3 position) {
			if (camera.orthographic) position.z = camera.nearClipPlane;
			return camera.ScreenToWorldPoint(position);
		}

		public static Vector2 ScreenToWorldV2(this Camera camera, Vector3 position) {
			return camera.ScreenToWorld(position).ToVector2();
		}

		public static Vector3 WorldToScreen(this Camera camera, Vector3 position) {
			if (camera.orthographic) position.z = camera.nearClipPlane;
			return camera.WorldToScreenPoint(position);
		}

		public static Vector2 WorldToScreenV2(this Camera camera, Vector3 position) {
			return camera.WorldToScreen(position).ToVector2();
		}
	#endregion

	/// <summary>
	/// Extensions for Mouse Input.
	/// </summary>
	#region ===== Mouse =====
		public static Vector3 GetMouseWorldPosition(this Camera camera) {
			Ray ray = camera.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out RaycastHit raycastHit)) {
				return raycastHit.point;
			}

			return Vector3.zero;
		}

		public static Vector3Int GetMouseCellPosition(this Camera camera, GridLayout gridLayout) {
			Vector3 mousePos = camera.GetMouseWorldPosition();
			Vector3Int cellCoordinate = gridLayout.WorldToCell(mousePos);
			return cellCoordinate;
		}

		public static Vector3Int GetMouseCellPosition(this Camera camera, GridLayout gridLayout, Vector3 worldPos) => gridLayout.WorldToCell(worldPos);
		public static Vector3Int GetMouseCellPosition(this Camera camera, GridLayout gridLayout, Vector3Int worldPos) => gridLayout.WorldToCell(worldPos);
		
		public static Vector3 GetMouseWorldPosition(this Camera camera, GridLayout gridLayout, Vector3Int cellPos) => gridLayout.CellToWorld(cellPos);
		public static Vector3 GetMouseWorldPosition(this Camera camera, GridLayout gridLayout, Vector3 cellPos) => gridLayout.CellToWorld(cellPos.ToVector3Int());

	/// <summary>
	/// Extensions for Scene Management.
	/// </summary>
	#region ===== Scene Management =====
		public static void RestartScene() {
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

		public static void RestartScene(this Component component) {
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

		public static void LoadNextScene(this Component component) => SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
	#endregion

	/// <summary>
	/// Coroutines Extensions.
	/// </summary>
	#region ===== Coroutines =====
		public static Coroutine Run(this MonoBehaviour owner, ref Coroutine coroutine, IEnumerator routine) {
			owner.StartCoroutine(routine);
			return coroutine;
		}

		public static Coroutine Stop(this MonoBehaviour owner, ref Coroutine coroutine) {
			if (coroutine != null) {
				owner.StopCoroutine(coroutine);
			}
			return coroutine;
		}

		public static Coroutine Reset(this MonoBehaviour owner, ref Coroutine coroutine, IEnumerator routine) {
			coroutine = owner.Stop(ref coroutine);
			owner.Run(ref coroutine, routine);
			return coroutine;
		}

		public static IEnumerator Run(this MonoBehaviour owner, ref IEnumerator routine) {
			owner.StartCoroutine(routine);
			return routine;
		}

		public static IEnumerator Stop(this MonoBehaviour owner, ref IEnumerator routine) {
			owner.StopCoroutine(routine);
			return routine;
		}

		public static IEnumerator Reset(this MonoBehaviour owner, ref IEnumerator routine) {
			owner.Stop(ref routine);
			owner.Run(ref routine);
			return routine;
		}

		// public static CoroutineHandle RunCoroutine(this MonoBehaviour owner, IEnumerator routine) {
		// 	return new CoroutineHandle(owner, routine);
		// }

		// public static Coroutine CreateAnimationCoroutine(this MonoBehaviour owner, float duration, Action<float> changeFunction, Action onComplete = null) {
		// 	return owner.StartCoroutine(GenericAnimationRoutine(duration, changeFunction, onComplete));
		// }

		// public static IEnumerator GenericAnimationRoutine(float duration, Action<float> changeFunction, Action onComplete) {
		// 	float elapsedTime = 0f;
		// 	float progress = 0f;

		// 	while (progress <= 1f) {
		// 		changeFunction(progress);
		// 		progress = elapsedTime / duration;
		// 		elapsedTime += Time.unscaledDeltaTime;
		// 		yield return null;
		// 	}

		// 	changeFunction(1f);
		// 	onComplete?.Invoke();
		// }
	#endregion

	#region ===== Async =====
		public static CancellationToken RefreshToken(ref CancellationTokenSource tokenSource) {
			tokenSource?.Cancel();
			tokenSource?.Dispose();
			tokenSource = null;
			tokenSource = new CancellationTokenSource();
			return tokenSource.Token;
		}

		public static CancellationToken RefreshToken(this MonoBehaviour mono, ref CancellationTokenSource tokenSource) {
			tokenSource?.Cancel();
			tokenSource?.Dispose();
			tokenSource = null;
			tokenSource = new CancellationTokenSource();
			return tokenSource.Token;
		}

		public static CancellationToken RefreshToken(this CancellationTokenSource tokenSource) {
			tokenSource?.Cancel();
			tokenSource?.Dispose();
			tokenSource = null;
			tokenSource = new CancellationTokenSource();
			return tokenSource.Token;
		}

		public static CancellationTokenSource RefreshTokenSource(this CancellationTokenSource tokenSource) {
			tokenSource?.Cancel();
			tokenSource?.Dispose();
			tokenSource = null;
			tokenSource = new CancellationTokenSource();
			return tokenSource;
		}
	#endregion


	/// <summary>
	/// Array Extensions.
	/// </summary>
	#region ===== Arrays =====
		public static bool IsEmpty<T>(this IEnumerable<T> list) {
			if (list is IList) return (list as IList).Count == 0;
			else return !list.GetEnumerator().MoveNext();
		}

		/// <summary>
        /// Get how deep into the Enumerable the first instance of the object is.
        /// </summary>
        public static int Depth(this IEnumerable lst, object obj) {
            int i = 0;

            foreach(var o in lst) {
                if (object.Equals(o, obj)) return i;
                i++;
            }

            return -1;
        }

		/// <summary>
        /// Get how deep into the Enumerable the first instance of the value is.
        /// </summary>
        public static int Depth<T>(this IEnumerable<T> lst, T value) {
            int i = 0;

            foreach (var v in lst) {
                if (object.Equals(v, value)) return i;
                i++;
            }

            return -1;
        }

		public static IEnumerable<T> Like<T>(this IEnumerable lst) {
            foreach (var obj in lst) {
                if (obj is T) yield return (T)obj;
            }
        }

		public static bool Compare<T>(this IEnumerable<T> first, IEnumerable<T> second) {
            var e1 = first.GetEnumerator();
            var e2 = second.GetEnumerator();

            while (true) {
                var b1 = e1.MoveNext();
                var b2 = e2.MoveNext();
                if (!b1 && !b2) break; //reached end of list

                if (b1 && b2) {
                    if (!object.Equals(e1.Current, e2.Current)) return false;
                }
                else {
                    return false;
                }
            }

            return true;
        }

		/// <summary>
        /// Each enumerable contains the same elements, not necessarily in the same order, or of the same count. Just the same elements.
        /// </summary>
        public static bool SimilarTo<T>(this IEnumerable<T> first, IEnumerable<T> second) {
            return first.Except(second).Count() + second.Except(first).Count() == 0;
        }

		public static bool ContainsAny<T>(this IEnumerable<T> lst, params T[] objs) {
            if (objs == null) return false;
            return lst.Intersect(objs).Count() > 0;
        }

		public static bool ContainsAny<T>(this IEnumerable<T> lst, IEnumerable<T> objs) {
            return lst.Intersect(objs).Count() > 0;
		}

		// public static IEnumerable<T> Append<T>(this IEnumerable<T> lst, T obj) {
        //     var e = new LightEnumerator<T>(lst);

        //     while (e.MoveNext()) {
        //         yield return e.Current;
        //     }
        //     yield return obj;
        // }

		// public static IEnumerable<T> Append<T>(this IEnumerable<T> first, IEnumerable<T> next) {
        //     var e = new LightEnumerator<T>(first);

        //     while (e.MoveNext()) {
        //         yield return e.Current;
        //     }
        //     e = new LightEnumerator<T>(next);

        //     while (e.MoveNext()) {
        //         yield return e.Current;
        //     }
        // }

        // public static IEnumerable<T> Prepend<T>(this IEnumerable<T> lst, T obj) {
        //     yield return obj;
        //     var e = new LightEnumerator<T>(lst);

        //     while(e.MoveNext()) {
        //         yield return e.Current;
        //     }
        // }

		// public static bool Contains(this IEnumerable lst, object obj) {
        //     //foreach (var o in lst)
        //     //{
        //     //    if (Object.Equals(o, obj)) return true;
        //     //}
        //     var e = new LightEnumerator(lst);

        //     while(e.MoveNext()) {
        //         if (Object.Equals(e.Current, obj)) return true;
        //     }
        //     return false;
        // }

		// public static void AddRange<T>(this ICollection<T> lst, IEnumerable<T> elements) {
        //     //foreach (var e in elements)
        //     //{
        //     //    lst.Add(e);
        //     //}
        //     var e = new LightEnumerator<T>(elements);

        //     while(e.MoveNext()) {
        //         lst.Add(e.Current);
        //     }
        // }

		public static bool InBounds(this System.Array arr, int index) {
            return index >= 0 && index <= arr.Length - 1;
        }

		public static void Clear(this System.Array arr) {
            if (arr == null) return;
            System.Array.Clear(arr, 0, arr.Length);
        }

		public static void Copy<T>(IEnumerable<T> source, System.Array destination, int index) {
            if (source is System.Collections.ICollection) (source as System.Collections.ICollection).CopyTo(destination, index);
            else {
                int i = 0;
                foreach(var el in source) {
                    destination.SetValue(el, i + index);
                    i++;
                }
            }
        }
	#endregion

	/// <summary>
	/// Dictionary Extensions.
	/// </summary>
	#region ===== Dictionaries =====
		public static void AddRandomly<T>(this ICollection<T> toCollection, IList<T> fromList, int count) {
			while (toCollection.Count < count) {
				toCollection.Add(fromList.GetRandomElement());
			}
		}

		public static Dictionary<TKey, TValue> CloneDictionaryCloningValues<TKey, TValue>(Dictionary<TKey, TValue> original) where TValue : ICloneable {
			Dictionary<TKey, TValue> ret = new Dictionary<TKey, TValue>(original.Count, original.Comparer);

			foreach (KeyValuePair<TKey, TValue> entry in original) {
				ret.Add(entry.Key, (TValue) entry.Value.Clone());
			}

			return ret;
		}

		public class CloneableDictionary<TKey, TValue> : Dictionary<TKey, TValue> where TValue : ICloneable {
		    public IDictionary<TKey, TValue> Clone() {
		        CloneableDictionary<TKey, TValue> clone = new CloneableDictionary<TKey, TValue>();

		        foreach (KeyValuePair<TKey, TValue> pair in this) {
		            clone.Add(pair.Key, (TValue)pair.Value.Clone());
		        }

		        return clone;
		    }
		}

		public static Dictionary<int, T> ToIndexedDictionary<T>(this IEnumerable<T> list) {
			var dictionary = list.ToIndexedDictionary(t => t);
			return dictionary;
		}

		public static Dictionary<int, T> ToIndexedDictionary<S, T>(this IEnumerable<S> list, Func<S, T> valueSelector) {
			int index = -1;
			var dictionary = list.ToDictionary(t => ++index, valueSelector);
			return dictionary;
		}

		/// <summary>
		/// Method that adds the given key and value to the given dictionary only if the key is NOT present in the dictionary.
		/// This will be useful to avoid repetitive "if(!containskey()) then add" pattern of coding.
		/// </summary>
		/// <param name="dict">The given dictionary.</param>
		/// <param name="key">The given key.</param>
		/// <param name="value">The given value.</param>
		/// <returns>True if added successfully, false otherwise.</returns>
		/// <typeparam name="TKey">Refers the TKey type.</typeparam>
		/// <typeparam name="TValue">Refers the TValue type.</typeparam>
		public static bool AddIfNotExists <TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value) {
			if (dict.ContainsKey(key)) return false;

			dict.Add(key, value);
			return true;
		}

		/// <summary>
		/// Method that removes the given key and value from the given dictionary only if the key is present in the dictionary.
		/// This will be useful to avoid repetitive "if(containskey()) then remove" pattern of coding.
		/// </summary>
		/// <param name="dict">The given dictionary.</param>
		/// <param name="key">The given key.</param>
		/// <param name="value">The given value.</param>
		/// <returns>True if removed successfully, false otherwise.</returns>
		/// <typeparam name="TKey">Refers the TKey type.</typeparam>
		/// <typeparam name="TValue">Refers the TValue type.</typeparam>
		public static bool RemoveIfExists<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key) {
			if (!dict.ContainsKey(key)) return false;

			dict.Remove(key);
			return true;
		}

		/// <summary>
		/// Method that adds the given key and value to the given dictionary if the key is NOT present in the dictionary.
		/// If present, the value will be replaced with the new value.
		/// </summary>
		/// <param name="dict">The given dictionary.</param>
		/// <param name="key">The given key.</param>
		/// <param name="value">The given value.</param>
		/// <typeparam name="TKey">Refers the Key type.</typeparam>
		/// <typeparam name="TValue">Refers the Value type.</typeparam>
		public static void AddOrReplace <TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value) {
			if (dict.ContainsKey(key)) dict[key] = value;
			else dict.Add(key, value);
		}

		/// <summary>
		/// Method that adds the list of given KeyValuePair objects to the given dictionary. If a key is already present in the dictionary,
		/// then an error will be thrown.
		/// </summary>
		/// <param name="dict">The given dictionary.</param>
		/// <param name="kvpList">The list of KeyValuePair objects.</param>
		/// <typeparam name="TKey">Refers the TKey type.</typeparam>
		/// <typeparam name="TValue">Refers the TValue type.</typeparam>
		public static void AddRange <TKey, TValue>(this Dictionary<TKey, TValue> dict, List<KeyValuePair<TKey, TValue>> kvpList) {
			foreach (var kvp in kvpList) {
				dict.Add(kvp.Key, kvp.Value);
			}
		}

		/// <summary>
		/// Converts an enumeration of groupings into a Dictionary of those groupings.
		/// </summary>
		/// <typeparam name="TKey">Key type of the grouping and dictionary.</typeparam>
		/// <typeparam name="TValue">Element type of the grouping and dictionary list.</typeparam>
		/// <param name="groupings">The enumeration of groupings from a GroupBy() clause.</param>
		/// <returns>A dictionary of groupings such that the key of the dictionary is TKey type and the value is List of TValue type.</returns>
		/// <example>results = productList.GroupBy(product => product.Category).ToDictionary();</example>
		/// <remarks>http://extensionmethod.net/csharp/igrouping/todictionary-for-enumerations-of-groupings</remarks>
		public static Dictionary<TKey, List<TValue>> ToDictionary<TKey, TValue>(this IEnumerable<IGrouping<TKey, TValue>> groupings) {
			return groupings.ToDictionary(group => group.Key, group => group.ToList());
		}
	#endregion

	/// <summary>
	/// Enums Extensions.
	/// </summary>
	#region ===== Enums =====
		/// <summary>
		/// Converts a string to an enum.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="s"></param>
		/// <param name="ignoreCase">true to ignore casing in the string.</param>
		public static T ToEnum<T>(this string s, bool ignoreCase) where T : struct {
			// exit if null
			if (s.IsNullOrEmpty()) return default(T);

			Type genericType = typeof(T);

			if (!genericType.IsEnum)
				return default(T);

			try {
				return (T) Enum.Parse(genericType, s, ignoreCase);
			}

			catch (Exception) {
				// couldn't parse, so try a different way of getting the enums
				Array ary = Enum.GetValues(genericType);
				foreach (T en in ary.Cast<T>()
					.Where(en => 
						(string.Compare(en.ToString(), s, ignoreCase) == 0) ||
						(string.Compare((en as Enum).ToString(), s, ignoreCase) == 0))) {
							return en;
						}

				return default(T);
			}
		}

		/// <summary>
		/// Converts a string to an enum
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="s"></param>
		public static T ToEnum<T>(this string s) where T : struct {
			return s.ToEnum<T>(false);
		}
	#endregion

	/// <summary>
	/// File IO extensions.
	/// </summary>
	#region ===== File =====
		/// <summary>
		/// Creates a directory at <paramref name="folder"/> if it doesn't exist
		/// </summary>
		/// <param name="folder"></param>
		public static void CreateDirectoryIfNotExists(this string folder) {
			if (folder.IsNullOrEmpty()) return;

			string path = Path.GetDirectoryName(folder);
			if (path.IsNullOrEmpty()) return;

			if (! Directory.Exists(path)) Directory.CreateDirectory(path);
		}
	#endregion

	/// <summary>
	/// Generic (T) Extensions.
	/// </summary>
	#region ===== Generics =====
		/// <summary>
		/// Returns true if <paramref name="source"/> equals any of the items in <paramref name="list"/> 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="list"></param>
		/// <returns></returns>
		public static bool IsIn<T>(this T source, params T[] list) where T : class {
			// return false if the source or list are null
			// otherwise, scan the list
			return (source != null) && (! list.IsNullOrEmpty()) && (list.Contains(source));
		}

		/// <summary>
		/// Returns true if <paramref name="source"/> equals any of the items in <paramref name="list"/> 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="list"></param>
		/// <returns></returns>
		public static bool IsIn<T>(this T source, params T?[] list) where T : struct {
			// return false if the list is null
			// otherwise, scan the list
			return (!list.IsNullOrEmpty()) && (list.Contains(source));
		}

		/// <summary>
		/// Returns true if <paramref name="source"/> equals any of the items in <paramref name="list"/> 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="list"></param>
		/// <returns></returns>
		public static bool IsIn(this int source, params int[] list) {
			// return false if the list is null
			// otherwise, scan the list
			return (!list.IsNullOrEmpty()) && (list.Contains(source));
		}

		/// <summary>
		/// Returns true if <paramref name="source"/> does not equal all of the items in <paramref name="list"/> 
		/// NOTE: returns false if source is null
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="list"></param>
		/// <returns></returns>
		public static bool IsNotIn<T>(this T source, params T[] list) where T : class {
			// false if null
			if (source == null) return false;

			// return true if the list is empty
			if (list.IsNullOrEmpty()) return true;

			// otherwise, scan the list
			return (!list.Contains(source));
		}

		/// <summary>
		/// Returns true if <paramref name="source"/> does not equal all of the items in <paramref name="list"/> 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="list"></param>
		/// <returns></returns>
		public static bool IsNotIn<T>(this T source, params T?[] list) where T : struct {
			// return true if the list is empty
			if (list.IsNullOrEmpty()) return true;

			// otherwise, scan the list
			return (!list.Contains(source));
		}

		/// <summary>
		/// Returns true if <paramref name="source"/> does not equal all of the items in <paramref name="list"/> 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="list"></param>
		/// <returns></returns>
		public static bool IsNotIn(this int source, params int[] list) {
			// return true if the list is empty
			if (list.IsNullOrEmpty()) return true;

			// otherwise, scan the list
			return (!list.Contains(source));
		}

		    /// <summary>
		/// Wraps the given object into a List{T} and returns the list.
		/// </summary>
		/// <param name="tobject">The object to be wrapped.</param>
		/// <typeparam name="T">Refers the object to be returned as List{T}.</typeparam>
		/// <returns>Returns List{T}.</returns>
		public static List<T> AsList<T>(this T tobject) {
			return new List<T> { tobject };
		}

		/// <summary>
		/// Returns true if the generic T is null or default. 
		/// This will match: null for classes; null (empty) for Nullable&lt;T&gt;; zero/false/etc for other structs
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="tObj"></param>
		/// <returns></returns>
		public static bool IsTNull<T>(this T tObj) {
			return (EqualityComparer<T>.Default.Equals(tObj, default(T)));
		}
	#endregion

	/// <summary>
	/// Extensions for LINQ to XML and XElements.
	/// </summary>
	#region ===== XML =====
		/// <summary>
		/// Returns the Value of the element, or null if the element is null.
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		public static string GetValueOrNull(this XElement element) => (element != null) ? element.Value : null;

		/// <summary>
		/// Returns the Value of the element, or string.empty if the element is null.
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		public static string GetValueString(this XElement element) => (element != null) ? element.Value : string.Empty;

		/// <summary>
		/// Returns a nullable decimal.
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		public static decimal? ValueToDecimalNullable(this XElement element) => (element != null) ? element.Value.ToDecimalNull() : null;

		/// <summary>
		/// Returns a nullable int
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		public static int? ValueToIntNullable(this XElement element) => (element != null) ? element.Value.ToIntNull() : null;

		/// <summary>
		/// Returns a nullable long
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		public static long? ValueToLongNullable(this XElement element) => (element != null) ? element.Value.ToLongNull() : null;

		/// <summary>
		/// Returns a nullable float
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		public static float? ValueToFloatNullable(this XElement element) => (element != null) ? element.Value.ToFloatNull() : null;

	#endregion

	/// <summary>
	/// Handles XML serializing and deserializing Unity data.
	/// </summary>
	#region ===== Unity XML =====
		/// <summary>
		/// XML Serializes an object and returns a byte array.
		/// </summary>
		/// <param name="objToSerialize">the object to serialize</param>
		public static byte[] XMLSerialize_ToArray<T>(this T objToSerialize) where T : class {
			if (objToSerialize.IsTNull()) return null;

			// create the serialization object
			XmlSerializer xSerializer = new XmlSerializer(objToSerialize.GetType());

			// create a textwriter to hold the output
			using (MemoryStream ms = new MemoryStream()) {
				using (XmlTextWriter xtw = new XmlTextWriter(ms, Encoding.Unicode)) {
					// serialize it
					xSerializer.Serialize(xtw, objToSerialize);

					// return it
					return ((MemoryStream)xtw.BaseStream).ToArray();
				}
			}
		}

		/// <summary>
		/// XML Serializes an object and returns the serialized string.
		/// </summary>
		/// <param name="objToSerialize">the object to serialize</param>
		public static string XMLSerialize_ToString<T>(this T objToSerialize) where T : class {
			// exit if null
			if (objToSerialize.IsTNull()) return null;

			// create the serialization object
			XmlSerializer xSerializer = new XmlSerializer(objToSerialize.GetType());

			// create a textwriter to hold the output
			using (MemoryStream ms = new MemoryStream()) {
				using (XmlTextWriter xtw = new XmlTextWriter(ms, Encoding.Unicode)) {
					// serialize it
					xSerializer.Serialize(xtw, objToSerialize);

					// return it
					return UnicodeEncoding.Unicode.GetString(((MemoryStream)xtw.BaseStream).ToArray());
				}
			}
		}

		/// <summary>
		/// Deserializes an XML string.
		/// </summary>
		/// <param name="strSerial">the string to deserialize</param>
		/// <returns></returns>
		public static T XMLDeserialize_ToObject<T>(this string strSerial) where T : class {
			// skip if no string
			if (string.IsNullOrEmpty(strSerial))
				return default(T);

			using (MemoryStream ms = new MemoryStream(UnicodeEncoding.Unicode.GetBytes(strSerial))) {
				// create the serialization object
				XmlSerializer xSerializer = new XmlSerializer(typeof(T));

				// deserialize it
				return (T)xSerializer.Deserialize(ms);
			}
		}

		/// <summary>
		/// XML Deserializes a string.
		/// </summary>
		/// <param name="objSerial">the object to deserialize</param>
		/// <returns></returns>
		public static T XMLDeserialize_ToObject<T>(byte[] objSerial) where T : class {
			// skip if no object
			if (objSerial.IsNullOrEmpty()) return default(T);

			// pop the memory string
			using (MemoryStream ms = new MemoryStream(objSerial)) {
				// create the serialization object
				XmlSerializer xSerializer = new XmlSerializer(typeof(T));

				// deserialize it
				return (T)xSerializer.Deserialize(ms);
			}
		}

		/// <summary>
		/// XML Serialize the object, and save it to a file.
		/// </summary>
		/// <param name="objToSerialize"></param>
		/// <param name="path"></param>
		public static void XMLSerialize_AndSaveTo<T>(this T objToSerialize, string path) where T : class {
			// exit if null
			if ((objToSerialize.IsTNull()) || (path.IsNullOrEmpty())) return;

			// create the directory if it doesn't exist
			path.CreateDirectoryIfNotExists();

			// get a serialized on the object
			XmlSerializer serializer = new XmlSerializer(objToSerialize.GetType());

			// write to a filestream
			using (FileStream fs = new FileStream(path, FileMode.Create)) serializer.Serialize(fs, objToSerialize);
		}

		/// <summary>
		/// XML Serialize the object, and save it to the PersistentDataPath, which is a directory where your application can store user specific 
		/// data on the target computer. This is a recommended way to store files locally for a user like highscores or savegames. 
		/// </summary>
		/// <param name="objToSerialize"></param>
		/// <param name="folderName">OPTIONAL - sub folder name (ex. DataFiles\SavedGames</param>
		/// <param name="filename">the filename (ex. SavedGameData.xml)</param>
		public static void XMLSerialize_AndSaveToPersistentDataPath<T>(this T objToSerialize, string folderName, string filename) where T : class {
			// exit if null
			if ((objToSerialize.IsTNull()) || (filename.IsNullOrEmpty())) return;

			// build the path
			string path = folderName.IsNullOrEmpty() ?
				Path.Combine(Application.persistentDataPath, filename) :
				Path.Combine(Path.Combine(Application.persistentDataPath, folderName), filename);

			// create the directory if it doesn't exist
			path.CreateDirectoryIfNotExists();

			// get a serialized on the object
			XmlSerializer serializer = new XmlSerializer(objToSerialize.GetType());

			// write to a filestream
			using (FileStream fs = new FileStream(path, FileMode.Create)) serializer.Serialize(fs, objToSerialize);
		}

		/// <summary>
		/// XML Serialize the object, and save it to the DataPath, which points to your asset/project directory. This directory is typically read-only after
		/// your game has been compiled. Use only from Editor scripts.
		/// </summary>
		/// <param name="objToSerialize"></param>
		/// <param name="folderName">OPTIONAL - sub folder name (ex. DataFiles\SavedGames</param>
		/// <param name="filename">the filename (ex. SavedGameData.xml)</param>
		public static void XMLSerialize_AndSaveToDataPath<T>(this T objToSerialize, string folderName, string filename) where T : class {
			// exit if null
			if ((objToSerialize.IsTNull()) || (filename.IsNullOrEmpty())) return;

			// build the path
			string path = folderName.IsNullOrEmpty() ?
				Path.Combine(Application.persistentDataPath, filename) :
				Path.Combine(Path.Combine(Application.persistentDataPath, folderName), filename);

			// create the directory if it doesn't exist
			path.CreateDirectoryIfNotExists();

			// get a serialized on the object
			XmlSerializer serializer = new XmlSerializer(objToSerialize.GetType());

			// write to a filestream
			using (FileStream fs = new FileStream(path, FileMode.Create)) serializer.Serialize(fs, objToSerialize);
		}

		/// <summary>
		/// Load from a file and XML deserialize the object.
		/// </summary>
		/// <param name="path"></param>
		public static T XMLDeserialize_AndLoadFrom<T>(this string path) where T : class {
			// exit if null
			if (path.IsNullOrEmpty()) return null;

			// exit if the file doesn't exist
			if (!File.Exists(path)) return null;

			// get the serializer
			XmlSerializer serializer = new XmlSerializer(typeof(T));
			using (FileStream fs = new FileStream(path, FileMode.Open)) return serializer.Deserialize(fs) as T;
		}

		/// <summary>
		/// Load from a file and XML deserialize the object.
		/// </summary>
		/// <param name="folderName">OPTIONAL - sub folder name (ex. DataFiles\SavedGames</param>
		/// <param name="filename">the filename (ex. SavedGameData.xml)</param>
		public static T XMLDeserialize_AndLoadFromPersistentDataPath<T>(this string filename, string folderName) where T : class {
			// exit if null
			if (filename.IsNullOrEmpty()) return null;

			// build the path
			string path = folderName.IsNullOrEmpty() ?
				Path.Combine(Application.persistentDataPath, filename) :
				Path.Combine(Path.Combine(Application.persistentDataPath, folderName), filename);

			// load
			return path.XMLDeserialize_AndLoadFrom<T>();
		}

		/// <summary>
		/// Load from a file and XML deserialize the object.
		/// </summary>
		/// <param name="folderName">OPTIONAL - sub folder name (ex. DataFiles\SavedGames</param>
		/// <param name="filename">the filename (ex. SavedGameData.xml)</param>
		public static T XMLDeserialize_AndLoadFromDataPath<T>(this string filename, string folderName) where T : class {
			// exit if null
			if (filename.IsNullOrEmpty()) return null;

			// build the path
			string path = folderName.IsNullOrEmpty() ?
				Path.Combine(Application.dataPath, filename) :
				Path.Combine(Path.Combine(Application.dataPath, folderName), filename);

			// load
			return path.XMLDeserialize_AndLoadFrom<T>();
		}
	#endregion

	/// <summary>
	/// Extensions for Raycasting.
	/// </summary>
	#region ===== Raycast =====
		public static Vector3 GetNormal(this RaycastHit raycastHit) => raycastHit.normal;
		public static Vector3 GetPoint(this RaycastHit raycastHit) => raycastHit.point;
		public static float GetDistance(this RaycastHit raycastHit) => raycastHit.distance;
		public static Vector2 GetNormal(this RaycastHit2D raycastHit2D) => raycastHit2D.normal;
		public static Vector2 GetPoint(this RaycastHit2D raycastHit2D) => raycastHit2D.point;
		public static Vector2 GetCentroid(this RaycastHit2D raycastHit2D) => raycastHit2D.centroid;
		public static float GetDistance(this RaycastHit2D raycastHit2D) => raycastHit2D.distance;
	#endregion

	/// <summary>
	/// Extensions for Tiles.
	/// </summary>
	#region ===== Tiles =====
		public static T GetCell<T>(this Tilemap tilemap, Vector3Int coordinate) where T : TileBase {
			T tile = tilemap.GetTile<T>(coordinate);
			return tile;
		}

		public static T GetTile<T>(this Tilemap tilemap, Vector3Int cellPosition) where T : TileBase {
        	T tile = tilemap.GetTile<T>(cellPosition);
        	return tile;
    	}

		public static void SetTile<T>(this Tilemap tilemap, Vector3Int cellPosition, TileBase tile) where T : TileBase {
			tilemap.SetTile<T>(cellPosition, tile);
		}

		public static Vector3 ToWorldPosition(this Vector3Int cellCoordinate, GridLayout gridLayout) {
			Vector3 cellPosition = gridLayout.CellToWorld(cellCoordinate);
			return cellPosition;
		}

		public static Vector3Int ToCellPosition(this GameObject obj, GridLayout gridLayout) {
			Vector3 objectPos = obj.transform.position;
			Vector3Int cellCoordinate = gridLayout.WorldToCell(objectPos);
			return cellCoordinate;
		}

		public static Vector3Int ToCellPosition(this Vector3 cellPosition, GridLayout gridLayout) {
			Vector3Int cellCoordinate = gridLayout.WorldToCell(cellPosition);
			return cellCoordinate;
		}
	#endregion

	/// <summary>
	/// Audio extensions.
	/// </summary>
	#region ===== Audio =====
		public static void Play(this AudioSource src, AudioClip clip, AudioInterruptMode mode) {
            if (src == null) throw new System.ArgumentNullException("src");
            if (clip == null) throw new System.ArgumentNullException("clip");
            
            switch(mode) {
                case AudioInterruptMode.StopIfPlaying:
                    if (src.isPlaying) src.Stop();
                    break;
                case AudioInterruptMode.DoNotPlayIfPlaying:
                    if (src.isPlaying) return;
                    break;
                case AudioInterruptMode.PlayOverExisting:
                    break;
            }

            src.PlayOneShot(clip);
        }

        public static void Play(this AudioSource src, AudioClip clip, float volumeScale, AudioInterruptMode mode) {
            if (src == null) throw new System.ArgumentNullException("src");
            if (clip == null) throw new System.ArgumentNullException("clip");

            switch (mode) {
                case AudioInterruptMode.StopIfPlaying:
                    if (src.isPlaying) src.Stop();
                    break;
                case AudioInterruptMode.DoNotPlayIfPlaying:
                    if (src.isPlaying) return;
                    break;
                case AudioInterruptMode.PlayOverExisting:
                    break;
            }

            src.PlayOneShot(clip, volumeScale);
        }

		public enum AudioInterruptMode {
			StopIfPlaying,
			DoNotPlayIfPlaying,
			PlayOverExisting,
		}
	#endregion

	/// <summary>
	/// Extensions for Components.
	/// </summary>
	#region ===== Component =====
		public static bool HasComponent<T>(this GameObject gameObject) {
			return gameObject.GetComponent<T>() != null;
		}

		public static bool HasComponentInChildren<T>(this GameObject gameObject) {
			return gameObject.GetComponentInChildren<T>() != null;
		}

		public static bool HasComponentInHierarchy<T>(this GameObject gameObject) {
			return gameObject.GetComponentInHierarchy<T>() != null;
		}

		public static bool HasComponent<T>(this Component component) {
			return component.GetComponent<T>() != null;
		}

		public static bool HasComponentInChildren<T>(this Component component) {
			return component.GetComponentInChildren<T>() != null;
		}

		public static bool HasComponentInHierarchy<T>(this Component component) {
			return component.GetComponentInHierarchy<T>() != null;
		}

		public static T GetComponentInHierarchy<T>(this GameObject gameObject) {
			var candidate = gameObject.GetComponentInChildren<T>();

			return candidate == null ? gameObject.GetComponentInParent<T>() : candidate;
		}

		public static T GetComponentInHierarchy<T>(this Component component) {
			var candidate = component.GetComponentInChildren<T>();

			return candidate == null ? component.GetComponentInParent<T>() : candidate;
		}

		public static void ValidateComponent<T>(this Component obj, ref T component) where T : Component {
			if (component != null) { return; }
			obj = component.gameObject.GetComponent<T>();
		}
	#endregion

	/// <summary>
	/// Extensions for Sprite Renderers.
	/// </summary>
	#region ===== Sprite Renderer =====
		public static void SetAlpha(this SpriteRenderer spriteRenderer, float alpha) {
			Color color = spriteRenderer.color;
			color.a = alpha;
			spriteRenderer.color = color;
		}

		public static void SetSpriteColor(this SpriteRenderer spriteRenderer, Color color) {
			spriteRenderer.color = color;
		}

		public static void SetMaterialColor(this SpriteRenderer spriteRenderer, Color color) {
			spriteRenderer.material.SetColor(color.ToString(), color);
		}
	#endregion

	/// <summary>
	/// Extensions for Rigidbodies.
	/// </summary>
	#region ===== Rigidbody =====
		/// <summary>
		/// Changes the direction of a Rigidbody without changing its speed.
		/// </summary>
		/// <param name="rb">Rigidbody.</param>
		/// <param name="direction">New direction.</param>
		public static void ChangeDirection(this Rigidbody rb, Vector3 direction) {
			rb.linearVelocity = direction * rb.linearVelocity.magnitude;
		}

		/// <summary>
		/// Changes the direction of a Rigidbody2D without changing its speed.
		/// </summary>
		/// <param name="rb2D">Rigidbody.</param>
		/// <param name="direction">New direction.</param>
		public static void ChangeDirection(this Rigidbody2D rb2D, Vector2 direction) {
			rb2D.linearVelocity = direction * rb2D.linearVelocity.magnitude;
		}

		public static Rigidbody OppositeDirection(this Rigidbody rb) {
			rb.linearVelocity = rb.linearVelocity.Negate();
			return rb;
		}

		public static Rigidbody2D OppositeDirection(this Rigidbody2D rb2D) {
			rb2D.linearVelocity = rb2D.linearVelocity.Negate();
			return rb2D;
		}
	#endregion

	/// <summary>
	/// Extensions for Colliders.
	/// </summary>
	#region ===== Colliders =====
		public static void EnableCollider(this GameObject go) {
			if (go.HasComponent<Collider>()) {
				Collider col = go.GetComponent<Collider>();
				col.enabled = true;
			}
		}

		public static void DisableCollider(this GameObject go) {
			if (go.HasComponent<Collider>()) {
				Collider col = go.GetComponent<Collider>();
				col.enabled = false;
			}
		}

		public static void EnableColliders(this GameObject go) {
			if (go.HasComponentInHierarchy<Collider>()) {
				Collider[] col = go.GetComponents<Collider>();

				foreach(Collider c in col) {
					c.enabled = true;
				}
			}
		}

		public static void DisableColliders(this GameObject go) {
			if (go.HasComponentInHierarchy<Collider>()) {
				Collider[] col = go.GetComponents<Collider>();

				foreach(Collider c in col) {
					c.enabled = false;
				}
			}
		}
	#endregion

	/// <summary>
	/// Extensions for Rects.
	/// </summary>
	#region ===== Rect =====
		#region /// Resizing ///
			public static Rect SetWidth(this Rect rect, float width) {
				return new Rect(rect.x, rect.y, width, rect.height);
			}

			public static Rect SetHeight( this Rect rect, float height) {
				return new Rect(rect.x, rect.y, rect.width, height);
			}

			public static Rect SetWidthCentered(this Rect rect, float width) {
				return new Rect(rect.center.x - width * 0.5f, rect.y, width, rect.height);
			}

			public static Rect SetHeightCentered(this Rect rect, float height) {
				return new Rect(rect.x, rect.center.y - height * 0.5f, rect.width, height);
			}

			public static Rect SetSize (this Rect rect, float size) {
				return rect.SetSize( Vector2.one * size );
			}

			public static Rect SetSize(this Rect rect, float width, float height ) {
				return rect.SetSize(new Vector2(width, height));
			}

			public static Rect SetSize(this Rect rect, Vector2 size) {
				return new Rect(rect.position, size);
			}

			public static Rect SetSizeCentered(this Rect rect, float size) {
				return rect.SetSizeCentered(Vector2.one * size);
			}

			public static Rect SetSizeCentered(this Rect rect, float width, float height) {
				return rect.SetSizeCentered(new Vector2(width, height));
			}

			public static Rect SetSizeCentered(this Rect rect, Vector2 size) {
				return new Rect(rect.center - size * 0.5f, size);
			}

			public static void SetRectBorders(this RectTransform rect, float[] size) {
				rect.SetTop(size[0]);
				rect.SetLeft(size[1]);
				rect.SetRight(size[2]);
				rect.SetBottom(size[3]);
			}

			public static void SetLeft(this RectTransform rect, float left) {
				rect.offsetMin = new Vector2(left, rect.offsetMin.y);
			}

			public static void SetRight(this RectTransform rect, float right) {
				rect.offsetMax = new Vector2(-right, rect.offsetMax.y);
			}

			public static void SetTop(this RectTransform rect, float top) {
				rect.offsetMax = new Vector2(rect.offsetMax.x, -top);
			}

			public static void SetBottom(this RectTransform rect, float bottom) {
				rect.offsetMin = new Vector2(rect.offsetMin.x, bottom);
			}
		#endregion
	#endregion

	/// <summary>
	/// Extensions for Gizmos.
	/// </summary>
	#region ===== Gizmos =====
		/// <summary>
		/// Draws a wire cube with a given rotation 
		/// </summary>
		/// <param name="center"></param>
		/// <param name="size"></param>
		/// <param name="rotation"></param>
		public static void DrawWireCube(Vector3 center, Vector3 size, Quaternion rotation = default(Quaternion)) {
			var old = Gizmos.matrix;
			if (rotation.Equals(default(Quaternion)))
				rotation = Quaternion.identity;
			Gizmos.matrix = Matrix4x4.TRS(center, rotation, size);
			Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
			Gizmos.matrix = old;
		}

		public static void DrawArrow(Vector3 from, Vector3 to, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f) {
			Gizmos.DrawLine(from, to);
			var direction = to - from;
			var right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
			var left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
			Gizmos.DrawLine(to, to + right * arrowHeadLength);
			Gizmos.DrawLine(to, to + left * arrowHeadLength);
		}

		public static void DrawWireSphere(Vector3 center, float radius, Color color, Quaternion rotation = default(Quaternion)) {
			var old = Gizmos.matrix;
			if (rotation.Equals(default(Quaternion)))
				rotation = Quaternion.identity;
			Gizmos.matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
			Gizmos.DrawWireSphere(Vector3.zero, radius);
			Gizmos.matrix = old;
			Gizmos.color = color;
		}

		/// <summary>
		/// Draws a flat wire circle (up)
		/// </summary>
		/// <param name="center"></param>
		/// <param name="radius"></param>
		/// <param name="segments"></param>
		/// <param name="rotation"></param>
		public static void DrawWireCircle(Vector3 center, float radius, int segments = 20, Quaternion rotation = default(Quaternion)) {
			DrawWireArc(center,radius,360,segments,rotation);
		}

		/// <summary>
		/// Draws an arc with a rotation around the center
		/// </summary>
		/// <param name="center">center point</param>
		/// <param name="radius">radiu</param>
		/// <param name="angle">angle in degrees</param>
		/// <param name="segments">number of segments</param>
		/// <param name="rotation">rotation around the center</param>
		public static void DrawWireArc(Vector3 center, float radius, float angle, int segments = 20, Quaternion rotation = default(Quaternion)) {
			var old = Gizmos.matrix;
		
			Gizmos.matrix = Matrix4x4.TRS(center,rotation,Vector3.one);
			Vector3 from = Vector3.forward * radius;
			var step = Mathf.RoundToInt(angle / segments);
			for (int i = 0; i <= angle; i += step) {
				var to = new Vector3(radius * Mathf.Sin(i * Mathf.Deg2Rad), 0, radius * Mathf.Cos(i * Mathf.Deg2Rad));
				Gizmos.DrawLine(from, to);
				from = to;
			}

			Gizmos.matrix = old;
		}

		/// <summary>
		/// Draws an arc with a rotation around an arbitraty center of rotation
		/// </summary>
		/// <param name="center">the circle's center point</param>
		/// <param name="radius">radius</param>
		/// <param name="angle">angle in degrees</param>
		/// <param name="segments">number of segments</param>
		/// <param name="rotation">rotation around the centerOfRotation</param>
		/// <param name="centerOfRotation">center of rotation</param>
		public static void DrawWireArc(Vector3 center, float radius, float angle, int segments, Quaternion rotation, Vector3 centerOfRotation) {
			var old = Gizmos.matrix;
			if (rotation.Equals(default(Quaternion)))
				rotation = Quaternion.identity;
			Gizmos.matrix = Matrix4x4.TRS(centerOfRotation, rotation, Vector3.one);
			var deltaTranslation = centerOfRotation - center;
			Vector3 from = deltaTranslation + Vector3.forward * radius;
			var step = Mathf.RoundToInt(angle / segments);
			for (int i = 0; i <= angle; i += step)
			{
				var to = new Vector3(radius * Mathf.Sin(i * Mathf.Deg2Rad), 0, radius * Mathf.Cos(i * Mathf.Deg2Rad)) + deltaTranslation;
				Gizmos.DrawLine(from, to);
				from = to;
			}

			Gizmos.matrix = old;
		}

		/// <summary>
		/// Draws an arc with a rotation around an arbitraty center of rotation
		/// </summary>
		/// <param name="matrix">Gizmo matrix applied before drawing</param>
		/// <param name="radius">radius</param>
		/// <param name="angle">angle in degrees</param>
		/// <param name="segments">number of segments</param>
		public static void DrawWireArc(Matrix4x4 matrix, float radius, float angle, int segments) {
			var old = Gizmos.matrix;
			Gizmos.matrix = matrix;
			Vector3 from = Vector3.forward * radius;
			var step = Mathf.RoundToInt(angle / segments);
			for (int i = 0; i <= angle; i += step)
			{
				var to = new Vector3(radius * Mathf.Sin(i * Mathf.Deg2Rad), 0, radius * Mathf.Cos(i * Mathf.Deg2Rad));
				Gizmos.DrawLine(from, to);
				from = to;
			}

			Gizmos.matrix = old;
		}

		/// <summary>
		/// Draws a wire cylinder face up with a rotation around the center
		/// </summary>
		/// <param name="center"></param>
		/// <param name="radius"></param>
		/// <param name="height"></param>
		/// <param name="rotation"></param>
		public static void DrawWireCylinder(Vector3 center, float radius, float height, Quaternion rotation = default(Quaternion)) {
			var old = Gizmos.matrix;
			if (rotation.Equals(default(Quaternion)))
				rotation = Quaternion.identity;
			Gizmos.matrix = Matrix4x4.TRS(center,rotation,Vector3.one);
			var half = height / 2;
			
			//draw the 4 outer lines
			Gizmos.DrawLine( Vector3.right * radius - Vector3.up * half,  Vector3.right * radius + Vector3.up * half);
			Gizmos.DrawLine( - Vector3.right * radius - Vector3.up * half,  -Vector3.right * radius + Vector3.up * half);
			Gizmos.DrawLine( Vector3.forward * radius - Vector3.up * half,  Vector3.forward * radius + Vector3.up * half);
			Gizmos.DrawLine( - Vector3.forward * radius - Vector3.up * half,  - Vector3.forward * radius + Vector3.up * half);

			//draw the 2 cricles with the center of rotation being the center of the cylinder, not the center of the circle itself
			DrawWireArc(center + Vector3.up * half,radius,360,20,rotation, center);
			DrawWireArc(center + Vector3.down * half, radius, 360, 20, rotation, center);
			Gizmos.matrix = old;
		}

		/// <summary>
		/// Draws a wire capsule face up
		/// </summary>
		/// <param name="center"></param>
		/// <param name="radius"></param>
		/// <param name="height"></param>
		/// <param name="rotation"></param>
		public static void DrawWireCapsule(Vector3 center, float radius, float height, Quaternion rotation = default(Quaternion)) {
			if (rotation.Equals(default(Quaternion)))
				rotation = Quaternion.identity;
			var old = Gizmos.matrix;
			Gizmos.matrix = Matrix4x4.TRS(center,rotation,Vector3.one);
			var half = height / 2 - radius;
		
			//draw cylinder base
			DrawWireCylinder(center,radius,height - radius * 2,rotation);

			//draw upper cap
			//do some cool stuff with orthogonal matrices
			var mat = Matrix4x4.Translate(center + rotation * Vector3.up * half) * Matrix4x4.Rotate(rotation * Quaternion.AngleAxis(90,Vector3.forward));
			DrawWireArc(mat,radius,180,20);
			mat = Matrix4x4.Translate(center + rotation * Vector3.up * half) * Matrix4x4.Rotate(rotation * Quaternion.AngleAxis(90,Vector3.up)* Quaternion.AngleAxis(90, Vector3.forward));
			DrawWireArc(mat, radius, 180, 20);
			
			//draw lower cap
			mat = Matrix4x4.Translate(center + rotation * Vector3.down * half) * Matrix4x4.Rotate(rotation * Quaternion.AngleAxis(90, Vector3.up) * Quaternion.AngleAxis(-90, Vector3.forward));
			DrawWireArc(mat, radius, 180, 20);
			mat = Matrix4x4.Translate(center + rotation * Vector3.down * half) * Matrix4x4.Rotate(rotation * Quaternion.AngleAxis(-90, Vector3.forward));
			DrawWireArc(mat, radius, 180, 20);
		
			Gizmos.matrix = old;
		}
	#endregion

	/// <summary>
	/// Extensions for Particle Systems.
	/// </summary>
	#region ===== Particle System =====
		public static void EnableEmission(this ParticleSystem particleSystem, bool enabled) {
			var emission = particleSystem.emission;
			emission.enabled = enabled;
		}
	#endregion

	/// <summary>
	/// Extensions for numerical values.
	/// </summary>
	#region ===== Numericals =====
		public static T CoinFlip<T>(T i, T f) {
			float rand = Random.Range(0, 2);
            
			return rand.ToIntRound() < 1 ? i : f;
		}

		public static int Sign(this float f) => MathF.Sign(f);
		public static int Sign(this int i) => MathF.Sign(i);

		public static int ToIntRound(this float f) => Mathf.RoundToInt(f);
		public static int ToIntCeil(this float f) => Mathf.CeilToInt(f);
		public static int ToIntFloor(this float f) => Mathf.FloorToInt(f);

		public static float Round(this float f) => Mathf.Round(f);
		public static float RoundTo(this float f, int digits) => (float)System.Math.Round(f, digits);
		public static float Ceil(this float f) => Mathf.Ceil(f);
		public static float Floor(this float f) => Mathf.Floor(f);

		public static float Clamp(this float f, float min, float max) => Mathf.Clamp(f, min, max);
		public static int Clamp(this int i, int min, int max) => Mathf.Clamp(i, min, max);
		public static float Clamp01(this float f) => Mathf.Clamp(f, 0, 1);
		public static int Clamp01(this int i) => Mathf.Clamp(i, 0, 1);

		public static float Difference(float n1, float n2) {
			float diff = Mathf.Abs(Mathf.Max(n1, n2) - Mathf.Min(n1, n2));
			return diff;
		}

		public static int Randomize(this ref int i, int min, int max) {
			i = Random.Range(min, max);
			return i;
		}

		/// <summary>
		/// Negates (* -1) the given integer.
		/// </summary>
		/// <param name="number">The given integer.</param>
		/// <returns>The negated integer.</returns>
		public static int Negate(this int number) {
			return number * -1;
		}

		/// <summary>
		/// Strips out the sign and returns the absolute value of given integer.
		/// </summary>
		/// <param name="number">The given integer.</param>
		/// <returns>The absolute value of given integer.</returns>
		public static int AbsoluteValue(this int number) {
			return Math.Abs(number);
		}

		/// <summary>
		/// Negates (* -1) the given float.
		/// </summary>
		/// <param name="number">The given float.</param>
		/// <returns>The negated float.</returns>
		public static float Negate(this float number) {
			return number * -1;
		}

		/// <summary>
		/// Strips out the sign and returns the absolute value of given float.
		/// </summary>
		/// <param name="number">The given float.</param>
		/// <returns>The absolute value of given float.</returns>
		public static float AbsoluteValue(this float number) {
			return Math.Abs(number);
		}

		/// <summary>
		/// Negates (* -1) the given long number.
		/// </summary>
		/// <param name="number">The given long number.</param>
		/// <returns>The negated long number.</returns>
		public static long Negate(this long number) {
			return number * -1;
		}

		/// <summary>
		/// Strips out the sign and returns the absolute value of given long number.
		/// </summary>
		/// <param name="number">The given long number.</param>
		/// <returns>The absolute value of given long number.</returns>
		public static long AbsoluteValue(this long number) {
			return Math.Abs(number);
		}

		/// <summary>
        /// Returns if the given float is in between or equal to max and min.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool InRange(this float value, float max, float min = 0f) {
            if (max < min) return (value >= max && value <= min);
            else return (value >= min && value <= max);

        }
        
		/// <summary>
        /// Returns if the given int is in between or equal to max and min.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool InRange(this int value, int max, int min = 0) {
            if (max < min) return (value >= max && value <= min);
            else return (value >= min && value <= max);
        }
        
		/// <summary>
        /// Returns if the given float is in between the max and min.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool InRangeExclusive(this float value, float max, float min = 0f) {
            if (max < min) return (value > max && value < min);
            else return (value > min && value < max);
        }

		/// <summary>
        /// Returns if the given int is in between the max and min.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool InRangeExclusive(this int value, int max, int min = 0) {
            if (max < min) return (value > max && value < min);
            else return (value > min && value < max);
        }

		/// <summary>
        /// Clamp a value into a range.
        /// 
        /// If input is LT min, min returned.
        /// If input is GT max, max returned.
        /// Else input returned.
        /// </summary>
        /// <param name="input">Value to clamp</param>
        /// <param name="max">Max in range</param>
        /// <param name="min">Min in range</param>
        /// <returns>Clamped value</returns>
        /// <remarks></remarks>
        public static short Clamp(this short input, short max, short min) {
            return Math.Max(min, Math.Min(max, input));
        }
        public static short Clamp(this short input, short max) {
            return Math.Max((short)0, Math.Min(max, input));
        }

        /*public static int Clamp(this int input, int max, int min) {
            return Math.Max(min, Math.Min(max, input));
        }
        public static int Clamp(this int input, int max) {
            return Math.Max(0, Math.Min(max, input));
        }

        public static long Clamp(this long input, long max, long min) {
            return Math.Max(min, Math.Min(max, input));
        }
        public static long Clamp(this long input, long max) {
            return Math.Max(0, Math.Min(max, input));
        }

        /*public static float Clamp(this float input, float max, float min) {
            return Math.Max(min, Math.Min(max, input));
        }
        public static float Clamp(this float input, float max) {
            return Math.Max(0, Math.Min(max, input));
        }*/

		/// <summary>
        /// Test if a value is near some target value, if with in some range of 'epsilon', the target is returned.
        /// 
        /// eg:
        /// Slam(1.52,2,0.1) == 1.52
        /// Slam(1.62,2,0.1) == 1.62
        /// Slam(1.72,2,0.1) == 1.72
        /// Slam(1.82,2,0.1) == 1.82
        /// Slam(1.92,2,0.1) == 2
        /// </summary>
        /// <param name="value"></param>
        /// <param name="target"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static float Slam(this float value, float target, float epsilon) {
            if (Math.Abs(value - target) < epsilon) return target;
            else return value;
        }

        public static float Slam(this float value, float target) {
            return value.Slam(target, EPSILON);
        }

		/// <summary>
        /// Wraps a value around some significant range.
        /// 
        /// Similar to modulo, but works in a unary direction over any range (including negative values).
        /// 
        /// ex:
        /// Wrap(8,6,2) == 4
        /// Wrap(4,2,0) == 0
        /// Wrap(4,2,-2) == 0
        /// </summary>
        /// <param name="value">value to wrap</param>
        /// <param name="max">max in range</param>
        /// <param name="min">min in range</param>
        /// <returns>A value wrapped around min to max</returns>
        /// <remarks></remarks>
        public static int Wrap(this int value, int max, int min) {
            max -= min;
            if (max == 0)
                return min;
			
			value = value - max * (int)Math.Floor((double)(value - min) / max);

            return value;
        }
        public static int Wrap(this int value, int max) {
            return Wrap(value, max, 0);
        }

        public static long Wrap(long value, long max, long min) {
            max -= min;
            if (max == 0)
                return min;

            return value - max * (long)Math.Floor((double)(value - min) / max);
        }
        public static long Wrap(long value, long max) {
            return Wrap(value, max, 0);
        }

        public static float Wrap(float value, float max, float min) {
            max -= min;
            if (max == 0)
                return min;

            return value - max * (float)Math.Floor((value - min) / max);
        }
        public static float Wrap(float value, float max) {
            return Wrap(value, max, 0);
        }

		/// <summary>
        /// Set an angle with in the bounds of -PI to PI.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static float NormalizeAngle(this float angle, bool useRadians) {
            float rd = (useRadians ? PI : 180);
            return Wrap(angle, rd, -rd);
        }

		/// <summary>
        /// Closest angle from a1 to a2.
        /// Absolute value the return for exact angle.
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static float NearestAngleBetween(float a1, float a2, bool useRadians) {
            var rd = useRadians ? PI : 180f;
            var ra = Wrap(a2 - a1, rd * 2f);
            if (ra > rd) ra -= (rd * 2f);
            return ra;
        }

        /// <summary>
        /// Returns a value for dependant that is a value that is the shortest angle between dep and ind from ind.
        /// 
        /// For instance if dep=-190 degrees and ind=10 degrees then 170 degrees will be returned 
        /// since the shortest path from 10 to -190 is to rotate to 170.
        /// </summary>
        /// <param name="dep"></param>
        /// <param name="ind"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static float ShortenAngleToAnother(this float dep, float ind, bool useRadians) {
            return ind + NearestAngleBetween(ind, dep, useRadians);
        }

		/// <summary>
        /// Returns a value for dependant that is the shortest angle in the positive direction from ind.
        /// 
        /// for instance if dep=-170 degrees, and ind=10 degrees, then 190 degrees will be returned as an alternative to -170. 
        /// Since 190 is the smallest angle > 10 equal to -170.
        /// </summary>
        /// <param name="dep"></param>
        /// <param name="ind"></param>
        /// <param name="useRadians"></param>
        /// <returns></returns>
        public static float NormalizeAngleToAnother(this float dep, float ind, bool useRadians) {
            float div = useRadians ? TWO_PI : 360f;
            float v = (dep - ind) / div;
            return dep - (float)Math.Floor(v) * div;
        }

		public static int nimod(int a, int b) {
			return a - b * (int)Floor(a / b);
		}

		public static float nfmod(float a, float b) {
			return a - b * Floor(a / b);
		}

		/// <summary>
		/// Extensions for converting one data type to another
		/// </summary>
		#region /// Conversion ///
			/// /// <summary>
			/// Returns the fractional part of a float.
			/// </summary>
			/// <param name="value"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			public static float Shear(float value) {
			    return value % 1.0f;
			}

			/// <summary>
			/// Converts an int to float
			/// </summary>
			public static float ToFloat(this int value) {
				return (float)value;
			}

			/// <summary>
			/// Converts an int to a char
			/// </summary>
			public static char ToChar(this int value) {
				return Convert.ToChar(value);
			}

			/// <summary>
			/// Converts a string to an int
			/// </summary>
			/// <param name="value">value to convert</param>
			/// <param name="defaultValue">default value if could not convert</param>
			public static int ToInt(this string value, int defaultValue) {
				// exit if null
				if (string.IsNullOrEmpty(value)) return defaultValue;

				// convert
				int rVal;
				return int.TryParse(value, out rVal) ? rVal : defaultValue;
			}

			/// <summary>
			/// Converts a string to a nullable int
			/// </summary>
			/// <param name="value">value to convert</param>
			public static int? ToIntNull(this string value) {
				// exit if null
				if (string.IsNullOrEmpty(value)) return null;

				// convert
				int rVal;
				return int.TryParse(value, out rVal) ? rVal : new int?();
			}

			/// <summary>
			/// Converts a string to an long
			/// </summary>
			/// <param name="value">value to convert</param>
			/// <param name="defaultValue">default value if could not convert</param>
			public static long ToLong(this string value, long defaultValue) {
				// exit if null
				if (string.IsNullOrEmpty(value)) return defaultValue;

				// convert
				long rVal;
				return long.TryParse(value, out rVal) ? rVal : defaultValue;
			}

			/// <summary>
			/// Converts a string to a nullable long
			/// </summary>
			/// <param name="value">value to convert</param>
			public static long? ToLongNull(this string value) {
				// exit if null
				if (string.IsNullOrEmpty(value)) return null;

				// convert
				long rVal;
				return long.TryParse(value, out rVal) ? rVal : new long?();
			}

			/// <summary>
			/// Converts a string to a decimal
			/// </summary>
			/// <param name="value">value to convert</param>
			/// <param name="defaultValue">default value if could not convert</param>
			public static double ToDouble(this string value, double defaultValue) {
			    // exit if null
			    if (string.IsNullOrEmpty(value)) return defaultValue;

			    // convert
			    double rVal;
			    return double.TryParse(value, out rVal) ? rVal : defaultValue;
			}

			/// <summary>
			/// Converts a string to a decimal
			/// </summary>
			/// <param name="value">value to convert</param>
			/// <param name="defaultValue">default value if could not convert</param>
			public static decimal ToDecimal(this string value, decimal defaultValue) {
				// exit if null
				if (string.IsNullOrEmpty(value)) return defaultValue;

				// convert
				decimal rVal;
				return decimal.TryParse(value, out rVal) ? rVal : defaultValue;
			}

			/// <summary>
			/// Converts a string to a nullable decimal
			/// </summary>
			/// <param name="value">value to convert</param>
			public static decimal? ToDecimalNull(this string value) {
				// exit if null
				if (string.IsNullOrEmpty(value)) return null;

				// convert
				decimal rVal;
				return decimal.TryParse(value, out rVal) ? rVal : new decimal?();
			}

			/// <summary>
			/// Converts a string to a float
			/// </summary>
			/// <param name="value">value to convert</param>
			/// <param name="defaultValue">default value if could not convert</param>
			public static float ToFloat(this string value, float defaultValue) {
				// exit if null
				if (string.IsNullOrEmpty(value)) return defaultValue;
			
				// convert
				float rVal;
				return float.TryParse(value, out rVal) ? rVal : defaultValue;
			}

			/// <summary>
			/// Converts a string to a nullable float
			/// </summary>
			/// <param name="value">value to convert</param>
			public static float? ToFloatNull(this string value) {
				// exit if null
				if (string.IsNullOrEmpty(value)) return null;

				// convert
				float rVal;
				return float.TryParse(value, out rVal) ? rVal : new float?();
			}

			/// <summary>
			/// Converts a string to a bool
			/// </summary>
			/// <param name="value">value to convert</param>
			/// <param name="defaultValue">default value if could not convert</param>
			public static bool ToBool(this string value, bool defaultValue) {
				// exit if null
				if (string.IsNullOrEmpty(value)) return defaultValue;

				// convert
				bool rVal;
				return bool.TryParse(value, out rVal) ? rVal : defaultValue;
			}

			/// <summary>
			/// Converts a string to a nullable bool
			/// </summary>
			/// <param name="value">value to convert</param>
			public static bool? ToBoolNull(this string value) {
				// exit if null
				if (string.IsNullOrEmpty(value)) return null;

				// convert
				bool rVal;
				return bool.TryParse(value, out rVal) ? rVal : new bool?();
			}

			/// <summary>
			/// Convert radians to degrees.
			/// </summary>
			/// <param name="angle"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			public static float RadiansToDegrees(this float angle) {
			    return angle * RAD_TO_DEG;
			}

			/// <summary>
			/// Convert degrees to radians.
			/// </summary>
			/// <param name="angle"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			public static float DegreesToRadians(this float angle) {
			    return angle * DEG_TO_RAD;
			}
		#endregion
		
		/// <summary>
		/// Extensions for sizing computer terms (KB, MB, GB, etc)
		/// </summary>
		#region /// Computer Sizing ///
			/// <summary>
			/// one kilobyte
			/// </summary>
			private const int INT_OneKB = 1024;

			/// <summary>
			/// Converts to kilobyte size
			/// </summary>
			/// <param name="value"></param>
			/// <returns></returns>
			public static int KB(this int value) {
				return value * INT_OneKB;
			}

			/// <summary>
			/// Converts to megabyte size (1024^2 bytes)
			/// </summary>
			/// <param name="value"></param>
			/// <returns></returns>
			public static int MB(this int value) {
				return value * INT_OneKB * INT_OneKB;
			}

			/// <summary>
			/// Converts to gigabyte size (1024^3 bytes)
			/// </summary>
			/// <param name="value"></param>
			/// <returns></returns>
			public static int GB(this int value) {
				return value * INT_OneKB * INT_OneKB * INT_OneKB;
			}

			/// <summary>
			/// Converts to terabyte size (1024^4 bytes)
			/// </summary>
			/// <param name="value"></param>
			/// <returns></returns>
			public static int TB(this int value) {
				return value * INT_OneKB * INT_OneKB * INT_OneKB * INT_OneKB;
			}

			/// <summary>
			/// Converts to petabyte size (1024^5 bytes)
			/// </summary>
			/// <param name="value"></param>
			/// <returns></returns>
			public static int PB(this int value) {
				return value * INT_OneKB * INT_OneKB * INT_OneKB * INT_OneKB * INT_OneKB;
			}

			/// <summary>
			/// Converts to exabyte size (1024^6 bytes)
			/// </summary>
			/// <param name="value"></param>
			/// <returns></returns>
			public static int EB(this int value) {
				return value * INT_OneKB * INT_OneKB * INT_OneKB * INT_OneKB * INT_OneKB * INT_OneKB;
			}

			/// <summary>
			/// Converts to zettabyte size (1024^7 bytes)
			/// </summary>
			/// <param name="value"></param>
			/// <returns></returns>
			public static int ZB(this int value) {
				return value * INT_OneKB * INT_OneKB * INT_OneKB * INT_OneKB * INT_OneKB * INT_OneKB * INT_OneKB;
			}

			/// <summary>
			/// Converts to yottabyte size (1024^8 bytes)
			/// </summary>
			/// <param name="value"></param>
			/// <returns></returns>
			public static int YB(this int value) {
				return value * INT_OneKB * INT_OneKB * INT_OneKB * INT_OneKB * INT_OneKB * INT_OneKB * INT_OneKB * INT_OneKB;
			}
		#endregion
	#endregion

	/// <summary>
	/// Extensions for TimeSpan.
	/// </summary>
	#region ===== TimeSpan =====
	    /// <summary>
		/// Creates a timespan with <paramref name="seconds"/> seconds
		/// </summary>
		/// <param name="seconds"></param>
		/// <returns></returns>
		public static TimeSpan SecondsToTimeSpan(this int seconds) => TimeSpan.FromSeconds(seconds);

		/// <summary>
		/// Creates a timespan with <paramref name="minutes"/> minutes
		/// </summary>
		/// <param name="minutes"></param>
		/// <returns></returns>
		public static TimeSpan MinutesToTimeSpan(this int minutes) => TimeSpan.FromMinutes(minutes);

		/// <summary>
		/// Creates a timespan with <paramref name="hours"/> hours
		/// </summary>
		/// <param name="hours"></param>
		/// <returns></returns>
		public static TimeSpan HoursToTimeSpan(this int hours) => TimeSpan.FromHours(hours);
	#endregion

	/// <summary>
	/// Extensions for booleans.
	/// </summary>
	#region ===== Booleans =====
		/// <summary>
		/// Checks whether the given boolean item is true.
		/// </summary>
		/// <param name="item">Item to be checked.</param>
		/// <returns>True if the value is true, false otherwise.</returns>
		public static bool IsTrue(this bool item) {
			return item;
		}

		/// <summary>
		/// Checks whether the given boolean item is false.
		/// </summary>
		/// <param name="item">Item to be checked.</param>
		/// <returns>True if the value is false, false otherwise.</returns>
		public static bool IsFalse(this bool item) {
			return !item;
		}

		/// <summary>
		/// Checks whether the given boolean item is NOT true.
		/// </summary>
		/// <param name="item">Item to be checked.</param>
		/// <returns>True if the item is false, false otherwise.</returns>
		public static bool IsNotTrue(this bool item) {
			return !item.IsTrue();
		}

		/// <summary>
		/// Checks whether the given boolean item is NOT false.
		/// </summary>
		/// <param name="item">Item to be checked.</param>
		/// <returns>True if the value is true, false otherwise.</returns>
		public static bool IsNotFalse(this bool item) {
			return !item.IsFalse();
		}

		/// <summary>
		/// Toggles the given boolean item and returns the toggled value.
		/// </summary>
		/// <param name="item">Item to be toggled.</param>
		/// <returns>The toggled value.</returns>
		public static bool Toggle(this bool item) {
		    return !item;
		}

		/// <summary>
		/// Converts the given boolean value to integer.
		/// </summary>
		/// <param name="item">The boolean variable.</param>
		/// <returns>Returns 1 if true , 0 otherwise.</returns>
		public static int ToInt(this bool item) {
		    return item ? 1 : 0;
		}

		/// <summary>
		/// Returns the lower string representation of boolean.
		/// </summary>
		/// <param name="item">The boolean variable.</param>
		/// <returns>Returns "true" or "false".</returns>
		public static string ToLowerString(this bool item) {
		    return item.ToString().ToLower();
		}

		/// <summary>
		/// Returns the trueString or falseString based on the given boolean value.
		/// </summary>
		/// <param name="item">The boolean value.</param>
		/// <param name="trueString">Value to be returned if the condition is true.</param>
		/// <param name="falseString">Value to be returned if the condition is false.</param>
		/// <returns>Returns trueString if the given value is true otherwise falseString.</returns>
		public static string ToString(this bool item, string trueString, string falseString) {
		    return item.ToType<string>(trueString, falseString);
		}

		/// <summary>
		/// Returns the trueValue or the falseValue based on the given boolean value.
		/// </summary>
		/// <param name="item">The boolean value.</param>
		/// <param name="trueValue">Value to be returned if the condition is true.</param>
		/// <param name="falseValue">Value to be returned if the condition is false.</param>
		/// <typeparam name="T">Instance of any class.</typeparam>
		/// <returns>Returns trueValue if the given value is true otherwise falseValue.</returns>
		public static T ToType <T>(this bool item, T trueValue, T falseValue) {
		    return item ? trueValue : falseValue;
		}

		#region /// Comparables ///
			/// <summary>
			/// Returns true if the actual value is between lower and upper, Inclusive (ie, lower an upper are both allowed in the test)
			/// </summary>
			/// <typeparam name="T"></typeparam>
			/// <param name="actual">actual value to test</param>
			/// <param name="lower">inclusive lower limit</param>
			/// <param name="upper">inclusive upper limit</param>
			/// <returns></returns>
			public static bool IsBetweenInclusive<T>(this T actual, T lower, T upper) where T : IComparable<T> {
				return actual.CompareTo(lower) >= 0 && actual.CompareTo(upper) <= 0;
			}

			/// <summary>
			/// Returns true if the actual value is between lower and upper, Exclusive (ie, lower allowed in the test, upper is not allowed in the test)
			/// </summary>
			/// <typeparam name="T"></typeparam>
			/// <param name="actual">actual value to test</param>
			/// <param name="lower">inclusive lower limit</param>
			/// <param name="upper">exclusive upper limit</param>
			/// <returns></returns>
			public static bool IsBetweenExclusive<T>(this T actual, T lower, T upper) where T : IComparable<T> {
				return actual.CompareTo(lower) >= 0 && actual.CompareTo(upper) < 0;
			}
		#endregion
	#endregion
	}
}

/// <summary>
/// Percentage type.
/// </summary>
public struct Percentage {
	/// <summary>
    /// The percentage as a value
    /// </summary>
    public decimal Value { get; private set; }

    /// <summary>
    /// Returns the value as a percentage (Value / 100)
    /// </summary>
    public decimal ValueAsPercentage {
        get { return Value / 100; }
    }

	/// <summary>
    /// Init
    /// </summary>
    /// <param name="value"></param>
    public Percentage(int value) : this() {
        Value = value;
    }

	/// <summary>
    /// Init
    /// </summary>
    /// <param name="value"></param>
    public Percentage(decimal value) : this() {
        Value = value;
    }

	/// <summary>
    /// Minus (ex. 5 - 10% = 4.5).
    /// </summary>
    /// <param name="value">the decimal value</param>
    /// <param name="percentage">the percentage value</param>
    public static decimal operator -(decimal value, Percentage percentage) => value - (value * percentage);

	/// <summary>
    /// Minus (ex. 5 - 10% = 4.5).
    /// </summary>
    /// <param name="value">the int value</param>
    /// <param name="percentage">the percentage value</param>
    public static decimal operator -(int value, Percentage percentage) => value - (value * percentage);

	/// <summary>
    /// Minus (ex. 10% - 5 = -4.5).
    /// </summary>
    /// <param name="value">the decimal value</param>
    /// <param name="percentage">the percentage value</param>
    public static decimal operator -(Percentage percentage, decimal value) => (value * percentage) - value;

    /// <summary>
    /// Minus (ex. 10% - 5 = -4.5).
    /// </summary>
    /// <param name="value">the int value</param>
    /// <param name="percentage">the percentage value</param>
    public static decimal operator -(Percentage percentage, int value) => (value * percentage) - value;

    /// <summary>
    /// Subtract two percentages (ex. 10% - 8% = 2%).
    /// </summary>
    /// <param name="percentage">value 1</param>
    /// <param name="value">value 2</param>
    public static Percentage operator -(Percentage value, Percentage percentage) => new Percentage(value.Value - percentage.Value);

	/// <summary>
    /// Add (ex. 5 + 10% = 5.5).
    /// </summary>
    /// <param name="value">the decimal value</param>
    /// <param name="percentage">the percentage value</param>
    public static decimal operator +(decimal value, Percentage percentage) => value + (value * percentage);

    /// <summary>
    /// Add (ex. 5 + 10% = 5.5)
    /// </summary>
    /// <param name="value">the int value</param>
    /// <param name="percentage">the percentage value</param>
    public static decimal operator +(int value, Percentage percentage) => value + (value * percentage);

    /// <summary>
    /// Add (ex. 10% + 5 = 5.5)
    /// </summary>
    /// <param name="value">the decimal value</param>
    /// <param name="percentage">the percentage value</param>
    public static decimal operator +(Percentage percentage, decimal value) => value + (value * percentage);

    /// <summary>
    /// Add (ex. 10% + 5 = 5.5)
    /// </summary>
    /// <param name="value">the int value</param>
    /// <param name="percentage">the percentage value</param>
    public static decimal operator +(Percentage percentage, int value) => value + (value * percentage);

    /// <summary>
    /// Add two percentages (ex. 10% + 8% = 18%)
    /// </summary>
    /// <param name="percentage">value 1</param>
    /// <param name="value">value 2</param>
    public static Percentage operator +(Percentage value, Percentage percentage) => new Percentage(value.Value + percentage.Value);

	/// <summary>
    /// Multiply (ex. 5 * 10% = 0.5)
    /// </summary>
    /// <param name="value">the decimal value</param>
    /// <param name="percentage">the percentage value</param>
    public static decimal operator *(decimal value, Percentage percentage) => value * percentage.ValueAsPercentage;

    /// <summary>
    /// Multiply (ex. 5 * 10% = 0.5)
    /// </summary>
    /// <param name="value">the int value</param>
    /// <param name="percentage">the percentage value</param>
    public static decimal operator *(int value, Percentage percentage) => value * percentage.ValueAsPercentage;

    /// <summary>
    /// Multiply (ex. 10% * 5 = 0.5)
    /// </summary>
    /// <param name="value">the decimal value</param>
    /// <param name="percentage">the percentage value</param>
    public static decimal operator *(Percentage percentage, decimal value) => value * percentage.ValueAsPercentage;

    /// <summary>
    /// Multiply (ex. 10% * 5 = 0.5)
    /// </summary>
    /// <param name="value">the int value</param>
    /// <param name="percentage">the percentage value</param>
    public static decimal operator *(Percentage percentage, int value) => value * percentage.ValueAsPercentage;

    /// <summary>
    /// Multiply two percentages (ex. 10% * 8% = 0.8%)  (0.1 * 0.08 = 0.008)
    /// </summary>
    /// <param name="percentage">value 1</param>
    /// <param name="value">value 2</param>
    public static Percentage operator *(Percentage value, Percentage percentage) => new Percentage(value.ValueAsPercentage * percentage.ValueAsPercentage);

	/// <summary>
    /// Divide (ex. 5 / 10% = 50)
    /// </summary>
    /// <param name="value">the decimal value</param>
    /// <param name="percentage">the percentage value</param>
    public static decimal operator /(decimal value, Percentage percentage) => value / percentage.ValueAsPercentage;

    /// <summary>
    /// Divide (ex. 5 / 10% = 50)
    /// </summary>
    /// <param name="value">the int value</param>
    /// <param name="percentage">the percentage value</param>
    public static decimal operator /(int value, Percentage percentage) => value / percentage.ValueAsPercentage;

    /// <summary>
    /// Divide (ex. 10% / 5 = 0.02)
    /// </summary>
    /// <param name="value">the decimal value</param>
    /// <param name="percentage">the percentage value</param>
    public static decimal operator /(Percentage percentage, decimal value) => percentage.ValueAsPercentage / value;

    /// <summary>
    /// Divide (ex. 10% / 5 = 0.002)
    /// </summary>
    /// <param name="value">the int value</param>
    /// <param name="percentage">the percentage value</param>
    public static decimal operator /(Percentage percentage, int value) => percentage.ValueAsPercentage / value;

    /// <summary>
    /// Divide two percentages (ex. 10% / 8% = 125%)  (0.1 / 0.08 = 1.25)
    /// </summary>
    /// <param name="percentage">value 1</param>
    /// <param name="value">value 2</param>
    public static Percentage operator /(Percentage value, Percentage percentage) => new Percentage(value.ValueAsPercentage / percentage.ValueAsPercentage);
}

[Serializable]
public struct Optional<T> {
	[SerializeField] private bool enabled;
	[SerializeField] private T value;

	public Optional(T initialValue) {
		enabled = true;
		value = initialValue;
	}

	public bool Enabled => enabled;
	public T Value => value;
}
#endregion