using System;
using System.Collections.Generic;
using System.Linq;

// https://github.com/adammyhre/Unity-Behaviour-Trees/blob/master/Assets/_Project/Scripts/Utilities/ListExtensions.cs

namespace UnityUtilities {
    /// <summary>
	/// Lists Extensions.
	/// </summary>
    public static class ListExtensions {
        static Random rng;
        
        /// <summary>
        /// Determines whether a collection is null or has no elements
        /// without having to enumerate the entire collection to get a count.
        ///
        /// Uses LINQ's Any() method to determine if the collection is empty,
        /// so there is some GC overhead.
        /// </summary>
        /// <param name="list">List to evaluate</param>
        public static bool IsNullOrEmpty<T>(this IList<T> list) {
            return list == null || !list.Any();
        }

        /// <summary>
        /// Creates a new list that is a copy of the original list.
        /// </summary>
        /// <param name="list">The original list to be copied.</param>
        /// <returns>A new list that is a copy of the original list.</returns>
        public static List<T> Clone<T>(this IList<T> list) {
            List<T> newList = new List<T>();
            foreach (T item in list) {
                newList.Add(item);
            }

            return newList;
        }

        /// <summary>
        /// Swaps two elements in the list at the specified indices.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="indexA">The index of the first element.</param>
        /// <param name="indexB">The index of the second element.</param>
        public static void Swap<T>(this IList<T> list, int indexA, int indexB) {
            (list[indexA], list[indexB]) = (list[indexB], list[indexA]);
        }

        // public static IList<T> Swap<T>(this IList<T> list, int a, int b) {
		// 	T temp = list[a];
		// 	list[a] = list[b];
		// 	list[b] = temp;
		// 	return list;
		// }

        /// <summary>
        /// Shuffles the elements in the list using the Durstenfeld implementation of the Fisher-Yates algorithm.
        /// This method modifies the input list in-place, ensuring each permutation is equally likely, and returns the list for method chaining.
        /// Reference: http://en.wikipedia.org/wiki/Fisher-Yates_shuffle
        /// </summary>
        /// <param name="list">The list to be shuffled.</param>
        /// <typeparam name="T">The type of the elements in the list.</typeparam>
        /// <returns>The shuffled list.</returns>
        public static IList<T> Shuffle<T>(this IList<T> list) {
            if (rng == null) rng = new Random();
            int count = list.Count;
            while (count > 1) {
                --count;
                int index = rng.Next(count + 1);
                (list[index], list[count]) = (list[count], list[index]);
            }
            return list;
        }

        /// <summary>
        /// Filters a collection based on a predicate and returns a new list
        /// containing the elements that match the specified condition.
        /// </summary>
        /// <param name="source">The collection to filter.</param>
        /// <param name="predicate">The condition that each element is tested against.</param>
        /// <returns>A new list containing elements that satisfy the predicate.</returns>
        public static IList<T> Filter<T>(this IList<T> source, Predicate<T> predicate) {
            List<T> list = new List<T>();
            foreach (T item in source) {
                if (predicate(item)) {
                    list.Add(item);
                }
            }
            return list;
        }

        public static IEnumerable<T> Flip<T>(this ICollection<T> list) {
			var flipedList = new List<T>();

			for (int i = list.Count - 1; i >= 0; i--) {
				flipedList.Add(list.ElementAt(i));
			}

			return flipedList;
		}

        public static void AscendingShuffle<T>(this IList<T> list, int iterations = 1, bool guaranteeDiscontinuity = false) {
			int n = list.Count * iterations;

			T last = list.Last();

			for (int i = 0; i < n; i++) {
				for (int j = 0; j < list.Count; j++) {
					int randomIndex = 0;

					while (randomIndex == j) {
						randomIndex = UnityEngine.Random.Range(0, list.Count);
					}

					list.Swap(j, randomIndex);
				}

				if (guaranteeDiscontinuity && list[0].NullSafeEquals(last)) {
					list.Swap(0, list.Count - 1);
				}
			}
		}

		public static void AscendingForwardShuffle<T>(this IList<T> list, int iterations = 1, bool guaranteeDiscontinuity = false) {
			int n = list.Count * iterations;

			T last = list.Last();

			for (int i = 0; i < n; i++) {
				for (int j = 0; j < list.Count - 1; j++) {
					int randomIndex = 0;

					while (randomIndex == j) {
						randomIndex = UnityEngine.Random.Range(j + 1, list.Count);
					}

					list.Swap(j, randomIndex);
				}

				if (guaranteeDiscontinuity && list[0].NullSafeEquals(last)) {
					list.Swap(0, list.Count - 1);
				}
			}
		}

		public static void DescendingShuffle<T>(this IList<T> list, int iterations = 1, bool guaranteeDiscontinuity = false) {
			int n = list.Count * iterations;

			T last = list.Last();

			for (int i = 0; i < n; i++) {
				for (int j = list.Count - 1; j >= 0; j--) {
					int randomIndex = 0;

					while (randomIndex == j) {
						randomIndex = UnityEngine.Random.Range(0, list.Count);
					}

					list.Swap(j, randomIndex);
				}

				if (guaranteeDiscontinuity && list[0].NullSafeEquals(last)) {
					list.Swap(0, list.Count - 1);
				}
			}
		}

		public static void DescendingReverseShuffle<T>(this IList<T> list, int iterations = 1, bool guaranteeDiscontinuity = false) {
			int n = list.Count * iterations;

			T last = list.Last();

			for (int i = 0; i < n; i++) {
				for (int j = list.Count - 1; j >= 1; j--) {
					int randomIndex = 0;

					while (randomIndex == j) {
						randomIndex = UnityEngine.Random.Range(j - 1, list.Count);
					}

					list.Swap(j, randomIndex);
				}

				if (guaranteeDiscontinuity && list[0].NullSafeEquals(last)) {
					list.Swap(0, list.Count - 1);
				}
			}
		}

        public static int GetFirstIndex<T>(this IList<T> list) => 0;
		public static int GetLastIndex<T>(this IList<T> list) => list.Count - 1;

		public static int GetRandomIndex<T>(this IList<T> list) {
			int randomIndex = 0;
            if (rng == null) rng = new Random();

			if (list.Count >= 2) {
				// randomIndex = UnityEngine.Random.Range(0, list.Count);
				randomIndex = rng.Next(list.Count);
			}

			return randomIndex;
		}

		public static T GetElement<T>(this IList<T> list, int i) => list.ElementAt(i);
		public static T GetFirstElement<T>(this IList<T> list) => list[list.GetFirstIndex()];
		public static T GetLastElement<T>(this IList<T> list) => list[list.GetLastIndex()];
		public static T GetRandomElement<T>(this IList<T> list) => list[list.GetRandomIndex()];

		public static List<T> GetTrueRandomElements<T>(this IList<T> inputList, int count, bool throwArgumentOutOfRangeException = true) {
			if (throwArgumentOutOfRangeException && count > inputList.Count) throw new ArgumentOutOfRangeException();

			var outputList = new List<T>(count);
			outputList.AddRandomly(inputList, count);
			return outputList;
		}

		public static List<T> GetDistinctRandomElements<T>(this IList<T> inputList, int count) {
			if (count > inputList.Count) {
				throw new ArgumentOutOfRangeException();
			}

			List<T> outputList = new List<T>();

			if (count == inputList.Count) {
				outputList = new List<T>(inputList);
				return outputList;
			}

			var sourceDictionary = inputList.ToIndexedDictionary();

			if (count > inputList.Count / 2) {
				while (sourceDictionary.Count > count) {
					sourceDictionary.Remove(inputList.GetRandomIndex());
				}

				outputList = sourceDictionary.Select(kvp => kvp.Value).ToList();
				return outputList;
			}

			var randomDictionary = new Dictionary<int, T>(count);

			while (randomDictionary.Count < count) {
				int key = inputList.GetRandomIndex();

				if (!randomDictionary.ContainsKey(key)) {
					randomDictionary.Add(key, sourceDictionary[key]);
				}
			}

			outputList = randomDictionary.Select(kvp => kvp.Value).ToList();
			return outputList;
		}

		public static IList<T> UnifyLists<T>(this IList<T> outputList, IList<T>[] listsArray) {
			outputList = new List<T>();

			foreach (List<T> inputList in listsArray) {
				outputList = outputList.Union(inputList).ToList();
			}

			return outputList;
		}

		/// <summary>
		/// Returns true if the array is null or empty
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="data"></param>
		/// <returns></returns>
		public static bool IsNullOrEmpty<T>(this T[] data) {
			return ((data == null) || (data.Length == 0));
		}

		/// <summary>
		/// Returns true if the list is null or empty
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="data"></param>
		/// <returns></returns>
		public static bool IsNullOrEmpty<T>(this List<T> data) {
			return ((data == null) || (data.Count == 0));
		}

		/// <summary>
		/// Returns true if the dictionary is null or empty
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <param name="data"></param>
		/// <returns></returns>
		public static bool IsNullOrEmpty<T1, T2>(this Dictionary<T1,T2> data) {
			return ((data == null) || (data.Count == 0));
		}

		/// <summary>
		/// Removes items from a collection based on the condition you provide. This is useful if a query gives 
		/// you some duplicates that you can't seem to get rid of. Some Linq2Sql queries are an example of this. 
		/// Use this method afterward to strip things you know are in the list multiple times
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="Predicate"></param>
		/// <remarks>http://extensionmethod.net/csharp/icollection-t/removeduplicates</remarks>
		/// <returns></returns>
		public static IEnumerable<T> RemoveDuplicates<T>(this ICollection<T> list, Func<T, int> Predicate) {
			var dict = new Dictionary<int, T>();

			foreach (var item in list) {
				if (!dict.ContainsKey(Predicate(item))) {
					dict.Add(Predicate(item), item);
				}
			}

			return dict.Values.AsEnumerable();
		}
    }
}