using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityUtilities;

// https://github.com/adammyhre/Unity-Utils/blob/master/UnityUtils/Scripts/Extensions/Vector3Extensions.cs
// https://github.com/adammyhre/Unity-Utils/blob/master/UnityUtils/Scripts/Extensions/Vector2Extensions.cs

namespace UnityUtilities {
    /// <summary>
	/// Extensions for Vectors.
	/// </summary>
    public static class VectorExtensions {
        #region /// Conversion ///
			public static Vector3 ToInt(this Vector3 v3) => new Vector3(v3.x.ToIntRound(), v3.y.ToIntRound(), v3.z.ToIntRound());
			public static Vector3 Floor(this Vector3 v3) => new Vector3(v3.x.Floor(), v3.y.Floor(), v3.z.Floor());
			public static Vector3 Ceil(this Vector3 v3) => new Vector3(v3.x.Ceil(), v3.y.Ceil(), v3.z.Ceil());
			public static Vector2 ToVector2(this Vector3 v3) => new Vector2(v3.x, v3.y);
			public static Vector3Int ToVector3Int(this Vector3 v3) => new Vector3Int(v3.x.ToIntRound(), v3.y.ToIntRound(), v3.z.ToIntRound());
			public static Vector3Int ToVector3IntFloor(this Vector3 v3) => new Vector3Int(v3.x.ToIntFloor(), v3.y.ToIntFloor(), v3.z.ToIntFloor());
			public static Vector3Int ToVector3IntCeil(this Vector3 v3) => new Vector3Int(v3.x.ToIntCeil(), v3.y.ToIntCeil(), v3.z.ToIntCeil());
			public static Vector2Int ToVector2Int(this Vector3 v3) => new Vector2Int(v3.x.ToIntRound(), v3.y.ToIntRound());
			public static Vector2Int ToVector2IntFloor(this Vector3 v3) => new Vector2Int(v3.x.ToIntFloor(), v3.y.ToIntFloor());
			public static Vector2Int ToVector2IntCeil(this Vector3 v3) => new Vector2Int(v3.x.ToIntCeil(), v3.y.ToIntCeil());

			public static Vector3 ToVector3(this Vector3Int v3Int) => new Vector3(v3Int.x.ToFloat(), v3Int.y.ToFloat(), v3Int.z.ToFloat());
			public static Vector2 ToVector2(this Vector3Int v3Int) => new Vector2(v3Int.x.ToFloat(), v3Int.y.ToFloat());
			public static Vector2Int ToVector2Int(this Vector3Int v3Int) => new Vector2Int(v3Int.x, v3Int.y);
			
			public static Vector2 ToInt(this Vector2 v2) => new Vector2(v2.x.ToIntRound(), v2.y.ToIntRound());
			public static Vector2 Floor(this Vector2 v2) => new Vector2(v2.x.Floor(), v2.y.Floor());
			public static Vector2 Ceil(this Vector2 v2) => new Vector2(v2.x.Ceil(), v2.y.Ceil());
			public static Vector3 ToVector3(this Vector2 v2, float z = 0f) => new Vector3(v2.x, v2.y, z);
			public static Vector3Int ToVector3Int(this Vector2 v2, int z = 0) => new Vector3Int(v2.x.ToIntRound(), v2.y.ToIntRound(), z);
			public static Vector3Int ToVector3IntFloor(this Vector2 v2, int z = 0) => new Vector3Int(v2.x.ToIntFloor(), v2.y.ToIntFloor(), z);
			public static Vector3Int ToVector3IntCeil(this Vector2 v2, int z = 0) => new Vector3Int(v2.x.ToIntCeil(), v2.y.ToIntCeil(), z);
			public static Vector2Int ToVector2Int(this Vector2 v2) => new Vector2Int(v2.x.ToIntRound(), v2.y.ToIntRound());
			public static Vector2Int ToVector2IntFloor(this Vector2 v2) => new Vector2Int(v2.x.ToIntFloor(), v2.y.ToIntFloor());
			public static Vector2Int ToVector2IntCeil(this Vector2 v2) => new Vector2Int(v2.x.ToIntCeil(), v2.y.ToIntCeil());

			public static Vector3 ToVector3(this Vector2Int v2Int, float z = 0f) => new Vector3(v2Int.x.ToFloat(), v2Int.y.ToFloat(), z);
			public static Vector2 ToVector2(this Vector2Int v2Int) => new Vector2(v2Int.x.ToFloat(), v2Int.y.ToFloat());
			public static Vector3Int ToVector3Int(this Vector2Int v2Int, int z = 0) => new Vector3Int(v2Int.x, v2Int.y, z);
		#endregion

        #region /// Default Vectors ///
			public static Vector3 NaNVector3 { get { return new Vector3(float.NaN, float.NaN, float.NaN); } }
			public static Vector3 PosInfVector3 { get { return new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity); } }
			public static Vector3 NegInfVector3 { get { return new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity); } }

			public static Vector2 NaNVector2 { get { return new Vector2(float.NaN, float.NaN); } }
			public static Vector2 PosInfVector2 { get { return new Vector2(float.PositiveInfinity, float.PositiveInfinity); } }
			public static Vector2 NegInfVector2 { get { return new Vector2(float.NegativeInfinity, float.NegativeInfinity); } }

			public static Vector3Int NaNVector3Int { get { return new Vector3Int(float.NaN.ToIntRound(), float.NaN.ToIntRound(), float.NaN.ToIntRound()); } }
			public static Vector3Int PosInfVector3Int { get { return new Vector3Int(float.PositiveInfinity.ToIntRound(), float.PositiveInfinity.ToIntRound(), float.PositiveInfinity.ToIntRound()); } }
			public static Vector3Int NegInfVector3Int { get { return new Vector3Int(float.NegativeInfinity.ToIntRound(), float.NegativeInfinity.ToIntRound(), float.NegativeInfinity.ToIntRound()); } }
			
			public static Vector2Int NaNVector2Int { get { return new Vector2Int(float.NaN.ToIntRound(), float.NaN.ToIntRound()); } }
			public static Vector2Int PosInfVector2Int { get { return new Vector2Int(float.PositiveInfinity.ToIntRound(), float.PositiveInfinity.ToIntRound()); } }
			public static Vector2Int NegInfVector2Int { get { return new Vector2Int(float.NegativeInfinity.ToIntRound(), float.NegativeInfinity.ToIntRound()); } }
		#endregion

		#region /// To Boolean ///
			public static bool IsNan(Vector3 v3) {
				return float.IsNaN(v3.sqrMagnitude);
			}

			public static bool IsNan(Vector2 v2) {
				return float.IsNaN(v2.sqrMagnitude);
			}

			/// <summary>
			/// Returns a Boolean indicating whether the current Vector3 is in a given range from another Vector3
			/// </summary>
			/// <param name="current">The current Vector3 position</param>
			/// <param name="target">The Vector3 position to compare against</param>
			/// <param name="range">The range value to compare against</param>
			/// <returns>True if the current Vector3 is in the given range from the target Vector3, false otherwise</returns>
			public static bool InRangeOf(this Vector3 current, Vector3 target, float range) {
                return current.SqrDistance(target) <= range * range;
			}

			/// <summary>
			/// Returns a Boolean indicating whether the current Vector2 is in a given range from another Vector2
			/// </summary>
			/// <param name="current">The current Vector2 position</param>
			/// <param name="target">The Vector2 position to compare against</param>
			/// <param name="range">The range value to compare against</param>
			/// <returns>True if the current Vector2 is in the given range from the target Vector2, false otherwise</returns>
			public static bool InRangeOf(this Vector2 current, Vector2 target, float range) {
                return current.SqrDistance(target) <= range * range;
			}
		#endregion

		#region /// Setters ///	
			// ex. transform.position.With(x: valueX, y: valueY);
			public static Vector3 With(this Vector3 v3, float? x = null, float? y = null, float? z = null) => new Vector3(x ?? v3.x, y ?? v3.y, z ?? v3.z);
			public static Vector2 With(this Vector2 v2, float? x = null, float? y = null) => new Vector2(x ?? v2.x, y ?? v2.y);
			public static Vector3Int With(this Vector3Int v3Int, int? x = null, int? y = null, int? z = null) => new Vector3Int(x ?? v3Int.x, y ?? v3Int.y, z ?? v3Int.z);
			public static Vector2Int With(this Vector2Int v2Int, int? x = null, int? y = null) => new Vector2Int(x ?? v2Int.x, y ?? v2Int.y);

			public static Vector3 Add(this Vector3 v3, float x = 0, float y = 0, float z = 0) => new Vector3(x: v3.x + x, y: v3.y + y, z: v3.z + z);
			public static Vector2 Add(this Vector2 v2, float x = 0, float y = 0) => new Vector2(x: v2.x + x, y: v2.y + y);
			public static Vector3Int Add(this Vector3Int v3Int, int x = 0, int y = 0, int z = 0) => new Vector3Int(x: v3Int.x + x, y: v3Int.y + y, z: v3Int.z + z);
			public static Vector2Int Add(this Vector2Int v2Int, int x = 0, int y = 0) => new Vector2Int(x: v2Int.x + x, y: v2Int.y + y);

			public static Vector3 Multiply(this Vector3 v3, float n = 1) => v3 * n;
			public static Vector2 Multiply(this Vector2 v2, float n = 1) => v2 * n;
			public static Vector3Int Multiply(this Vector3Int v3Int, int n = 1) => v3Int * n;
			public static Vector2Int Multiply(this Vector2Int v2Int, int n = 1) => v2Int * n;

			public static Vector3 Multiply(this Vector3 v3, float x = 1, float y = 1, float z = 1) => new Vector3(x: v3.x * x, y: v3.y * y, z: v3.z * z);
			public static Vector2 Multiply(this Vector2 v2, float x = 1, float y = 1) => new Vector2(x: v2.x * x, y: v2.y * y);
			public static Vector3Int Multiply(this Vector3Int v3Int, int x = 1, int y = 1, int z = 1) => new Vector3Int(x: v3Int.x * x, y: v3Int.y * y, z: v3Int.z * z);
			public static Vector2Int Multiply(this Vector2Int v2Int, int x = 1, int y = 1) => new Vector2Int(x: v2Int.x * x, y: v2Int.y * y);

			public static Vector3 MultiplyBy(this Vector3 v3, Vector3 vMult) => v3.With(x: v3.x * vMult.x, y: v3.y * vMult.y, z: v3.z * vMult.z);
			public static Vector2 MultiplyBy(this Vector2 v2, Vector2 vMult) => v2.With(x: v2.x * vMult.x, y: v2.y * vMult.y);
			public static Vector3Int MultiplyBy(this Vector3Int v3Int, Vector3Int vMult) => v3Int.With(x: v3Int.x * vMult.x, y: v3Int.y * vMult.y, z: v3Int.z * vMult.z);
			public static Vector2Int MultiplyBy(this Vector2Int v2Int, Vector2Int vMult) => v2Int.With(x: v2Int.x * vMult.x, y: v2Int.y * vMult.y);

			public static Vector3 DivideBy(this Vector3 v3, Vector3 vDiv) => v3.With(x: vDiv.x != 0 ? v3.x / vDiv.x : v3.x, vDiv.y != 0 ? v3.y / vDiv.y : v3.y, vDiv.z != 0 ? v3.z / vDiv.z : v3.z);
			public static Vector2 DivideBy(this Vector2 v2, Vector2 vDiv) => v2.With(vDiv.x != 0 ? v2.x / vDiv.x : v2.x, vDiv.y != 0 ? v2.y / vDiv.y : v2.y);
			public static Vector3Int DivideBy(this Vector3Int v3Int, Vector3Int vDiv) => v3Int.With(x: vDiv.x != 0 ? v3Int.x / vDiv.x : v3Int.x, vDiv.y != 0 ? v3Int.y / vDiv.y : v3Int.y, vDiv.z != 0 ? v3Int.z / vDiv.z : v3Int.z);
			public static Vector2Int DivideBy(this Vector2Int v2Int, Vector2Int vDiv) => v2Int.With(x: vDiv.x != 0 ? v2Int.x / vDiv.x : v2Int.x, y: vDiv.y != 0 ? v2Int.y / vDiv.y : v2Int.y);
			
			public static Vector3 FlattenX(this Vector3 v3) => v3.With(x: 0);
			public static Vector3 FlattenY(this Vector3 v3) => v3.With(y: 0);
			public static Vector3 FlattenZ(this Vector3 v3) => v3.With(z: 0);
			public static Vector3 Flatten(this Vector3 v3) => v3.With(x: 0, y: 0, z: 0);

			public static Vector2 FlattenX(this Vector2 v2) => v2.With(x: 0);
			public static Vector2 FlattenY(this Vector2 v2) => v2.With(y: 0);
			public static Vector2 Flatten(this Vector2 v2) => v2.With(x: 0, y: 0);

			public static Vector3Int FlattenX(this Vector3Int v3Int) => v3Int.With(x: 0);
			public static Vector3Int FlattenY(this Vector3Int v3Int) => v3Int.With(y: 0);
			public static Vector3Int FlattenZ(this Vector3Int v3Int) => v3Int.With(z: 0);
			public static Vector3Int Flatten(this Vector3Int v3Int) => v3Int.With(x: 0, y: 0, z: 0);

			public static Vector2Int FlattenX(this Vector2Int v2Int) => v2Int.With(x: 0);
			public static Vector2Int FlattenY(this Vector2Int v2Int) => v2Int.With(y: 0);
			public static Vector2Int Flatten(this Vector2Int v2Int) => v2Int.With(x: 0, y: 0);

			public static Vector3 Negate(this Vector3 v3) => v3.Multiply(-1);
			public static Vector2 Negate(this Vector2 v2) => v2.Multiply(-1);
			public static Vector3Int Negate(this Vector3Int v3Int) => v3Int.Multiply(-1);
			public static Vector2Int Negate(this Vector2Int v2Int) => v2Int.Multiply(-1);

			public static Vector3 Abs(this Vector3 v3) => v3.With(Mathf.Abs(v3.x), Mathf.Abs(v3.y), Mathf.Abs(v3.z));
			public static Vector2 Abs(this Vector2 v2) => v2.With(Mathf.Abs(v2.x), Mathf.Abs(v2.y));
			public static Vector3Int Abs(this Vector3Int v3Int) => v3Int.With(Mathf.Abs(v3Int.x), Mathf.Abs(v3Int.y), Mathf.Abs(v3Int.z));
			public static Vector2Int Abs(this Vector2Int v2Int) => v2Int.With(Mathf.Abs(v2Int.x), Mathf.Abs(v2Int.y));

			public static Vector3 NegAbs(this Vector3 v3) => v3.Abs().Negate();
			public static Vector2 NegAbs(this Vector2 v2) => v2.Abs().Negate();
			public static Vector3Int NegAbs(this Vector3Int v3Int) => v3Int.Abs().Negate();
			public static Vector2Int NegAbs(this Vector2Int v2Int) => v2Int.Abs().Negate();

			// public static Vector3 SwapXY(this Vector3 v3) => v3.SetXY(v3.y, v3.x);
			// public static Vector3 SwapXZ(this Vector3 v3) => v3.SetXY(v3.z, v3.x);
			// public static Vector3 SwapYZ(this Vector3 v3) => v3.SetXY(v3.z, v3.y);
			// public static Vector2 SwapXY(this Vector2 v2) => v2.SetXY(v2.y, v2.x);
			// public static Vector3Int SwapXY(this Vector3Int v3Int) => v3Int.SetXY(v3Int.y, v3Int.x);
			// public static Vector3Int SwapXZ(this Vector3Int v3Int) => v3Int.SetXZ(v3Int.z, v3Int.x);
			// public static Vector3Int SwapYZ(this Vector3Int v3Int) => v3Int.SetYZ(v3Int.z, v3Int.y);
			// public static Vector2Int SwapXY(this Vector2Int v2Int) => v2Int.SetXY(v2Int.y, v2Int.x);
		#endregion

		#region /// Getters ///
			public static float GetMinScalar(this Vector3 v3) => Mathf.Min(v3.x, v3.y, v3.z);
			public static float GetMaxScalar(this Vector3 v3) => Mathf.Max(v3.x, v3.y, v3.z);
			public static float GetMinScalar(this Vector2 v2) => Mathf.Min(v2.x, v2.y);
			public static float GetMaxScalar(this Vector2 v2) => Mathf.Max(v2.x, v2.y);
			public static int GetMinScalar(this Vector2Int v2Int) => Mathf.Min(v2Int.x, v2Int.y);
			public static int GetMaxScalar(this Vector2Int v2Int) => Mathf.Max(v2Int.x, v2Int.y);
			public static int GetMinScalar(this Vector3Int v3Int) => Mathf.Min(v3Int.x, v3Int.y, v3Int.z);
			public static int GetMaxScalar(this Vector3Int v3Int) => Mathf.Max(v3Int.x, v3Int.y, v3Int.z);

			public static Vector3 Midpoint(this Vector3 v3) => v3.Multiply(0.5f);
			public static Vector2 Midpoint(this Vector2 v2) => v2.Multiply(0.5f);

			public static Vector2 Average(Vector2 a, Vector2 b) => (a + b) / 2f;
			public static Vector2 Average(Vector2 a, Vector2 b, Vector2 c) => (a + b + c) / 3f;
			public static Vector2 Average(Vector2 a, Vector2 b, Vector2 c, Vector2 d) => (a + b + c + d) / 4f;
			public static Vector2 Average(params Vector2[] values) {
				if (values == null || values.Length == 0) return Vector3.zero;

				Vector2 v = Vector2.zero;
				for (int i = 0; i < values.Length; i++)
				{
					v += values[i];
				}
				return v / values.Length;
			}
			public static Vector2 ListAverage(this IList<Vector2> values) {
				if (values == null || values.Count == 0) return Vector3.zero;

				Vector2 v = Vector2.zero;
				for (int i = 0; i < values.Count; i++)
				{
					v += values[i];
				}
				return v / values.Count;
			}
			public static Vector2 Average(this IList<Transform> list) {
				if (list == null || list.Count == 0) return Vector3.zero;

				Vector2 v = Vector2.zero;
				for (int i = 0; i < list.Count; i++) {
                    v += list.GetElement(i).position.ToVector2();
				}
				return v / list.Count;
			}

			/// <summary>
			/// Finds the Vector3 closest to the given Vector3.
			/// </summary>
			/// <param name="position">Original Vector3.</param>
			/// <param name="otherPositions">Others Vector3.</param>
			/// <returns>Closest Vector3.</returns>
			public static Vector3 GetClosestTo(this Vector3 v3, IEnumerable<Vector3> vectors3List) {
				var closestVector3 = Vector3.zero;
				var shortestDistance = Mathf.Infinity;

				foreach (var vector3 in vectors3List) {
					var distance = (v3 - vector3).sqrMagnitude;

					if (distance < shortestDistance) {
						closestVector3 = vector3;
						shortestDistance = distance;
					}
				}

				return closestVector3;
			}

			/// <summary>
			/// Finds the Vector2 closest to the given Vector2.
			/// </summary>
			/// <param name="position">Original Vector2.</param>
			/// <param name="otherPositions">Others Vector2.</param>
			/// <returns>Closest Vector2.</returns>
			public static Vector2 GetClosestTo(this Vector2 v2, IEnumerable<Vector2> vectors2List) {
				var closestVector2 = Vector3.zero;
				var shortestDistance = Mathf.Infinity;

				foreach (var vector2 in vectors2List) {
					var distance = (v2 - vector2).sqrMagnitude;

					if (distance < shortestDistance) {
						closestVector2 = vector2;
						shortestDistance = distance;
					}
				}

				return closestVector2;
			}

			/// <summary>
			/// Finds the Vector3Int closest to the given Vector3Int.
			/// </summary>
			/// <param name="position">Original Vector3Int.</param>
			/// <param name="otherPositions">Others Vector3Int.</param>
			/// <returns>Closest Vector3Int.</returns>
			public static Vector3Int GetClosestTo(this Vector3Int v3Int, IEnumerable<Vector3Int> vectors3IntList) {
				var closestVector3Int = Vector3Int.zero;
				var shortestDistance = Mathf.Infinity;

				foreach (var vector3Int in vectors3IntList) {
					var distance = (v3Int - vector3Int).sqrMagnitude;

					if (distance < shortestDistance) {
						closestVector3Int = v3Int;
						shortestDistance = distance;
					}
				}

				return closestVector3Int;
			}

			/// <summary>
			/// Finds the Vector2Int closest to the given Vector2Int.
			/// </summary>
			/// <param name="position">Original Vector2Int.</param>
			/// <param name="otherPositions">Others Vector2Int.</param>
			/// <returns>Closest Vector2Int.</returns>
			public static Vector2Int GetClosestTo(this Vector2Int v2Int, IEnumerable<Vector2Int> vectors2IntList) {
				var closestVector2Int = Vector2Int.zero;
				var shortestDistance = Mathf.Infinity;

				foreach (var vector2Int in vectors2IntList) {
					var distance = (v2Int - vector2Int).sqrMagnitude;

					if (distance < shortestDistance) {
						closestVector2Int = v2Int;
						shortestDistance = distance;
					}
				}

				return closestVector2Int;
			}

			/// <summary>
			/// Finds the Vector3 farthest from the given Vector3.
			/// </summary>
			/// <param name="position">Original Vector3.</param>
			/// <param name="otherPositions">Others Vector3.</param>
			/// <returns>Farthest Vector3.</returns>
			public static Vector3 GetFarthestFrom(this Vector3 v3, IEnumerable<Vector3> vectors3List) {
				var farthestVector3 = Vector3.zero;
				var farthestDistance = 0f;

				foreach (var vector3 in vectors3List) {
					var distance = (v3 + vector3).sqrMagnitude;

					if (distance > farthestDistance) {
						farthestVector3 = v3;
						farthestDistance = distance;
					}
				}

				return farthestVector3;
			}

			/// <summary>
			/// Finds the Vector2 farthest from the given Vector2.
			/// </summary>
			/// <param name="position">Original Vector2.</param>
			/// <param name="otherPositions">Others Vector2.</param>
			/// <returns>Farthest Vector2.</returns>
			public static Vector2 GetFarthestFrom(this Vector2 v2, IEnumerable<Vector2> vectors2List) {
				var farthestVector2 = Vector2.zero;
				var farthestDistance = 0f;

				foreach (var vector2 in vectors2List) {
					var distance = (v2 + vector2).sqrMagnitude;

					if (distance > farthestDistance) {
						farthestVector2 = v2;
						farthestDistance = distance;
					}
				}

				return farthestVector2;
			}

			/// <summary>
			/// Finds the Vector2Int farthest from the given Vector2Int.
			/// </summary>
			/// <param name="position">Original Vector2Int.</param>
			/// <param name="otherPositions">Others Vector2Int.</param>
			/// <returns>Farthest Vector2Int.</returns>
			public static Vector3Int GetFarthestFrom(this Vector3Int v3Int, IEnumerable<Vector3Int> vectors3IntList) {
				var farthestVector3Int = Vector3Int.zero;
				var farthestDistance = 0f;

				foreach (var vector3Int in vectors3IntList) {
					var distance = (v3Int + vector3Int).sqrMagnitude;

					if (distance > farthestDistance) {
						farthestVector3Int = v3Int;
						farthestDistance = distance;
					}
				}

				return farthestVector3Int;
			}

			/// <summary>
			/// Finds the Vector2Int farthest from the given Vector2Int.
			/// </summary>
			/// <param name="position">Original Vector2Int.</param>
			/// <param name="otherPositions">Others Vector2Int.</param>
			/// <returns>Farthest Vector2Int.</returns>
			public static Vector2Int GetFarthestFrom(this Vector2Int v2Int, IEnumerable<Vector2Int> vectors2IntList) {
				var farthestVector2Int = Vector2Int.zero;
				var farthestDistance = 0f;

				foreach (var vector2Int in vectors2IntList) {
					var distance = (v2Int + vector2Int).sqrMagnitude;

					if (distance > farthestDistance) {
						farthestVector2Int = v2Int;
						farthestDistance = distance;
					}
				}

				return farthestVector2Int;
			}

			/// <summary>
			/// Converts a Vector2 to a Vector3 with a y value of 0.
			/// </summary>
			/// <param name="v2">The Vector2 to convert.</param>
			/// <returns>A Vector3 with the x and z values of the Vector2 and a y value of 0.</returns>
			// public static Vector3 ToVector3(this Vector2 v2) {
			// 	return new Vector3(v2.x, 0, v2.y);
			// }

			/// <summary>
			/// Adds a random offset to the components of a <see cref="Vector3"/> within the specified range.
			/// </summary>
			/// <param name="vector">The original vector to which the random offset will be applied.</param>
			/// <param name="range">The maximum absolute value of random offsets that can be added 
			/// or subtracted to/from each component of the vector.</param>
			/// <returns>A new <see cref="Vector3"/> with random offsets applied to its X, Y, and Z components.
			/// Each offset is in the range [-<paramref name="range"/>, <paramref name="range"/>].</returns>
			public static Vector3 RandomOffset(this Vector3 vector, float range) {
				return vector + new Vector3(
					Random.Range(-range, range),
					Random.Range(-range, range),
					Random.Range(-range, range)
				);
			}

			/// <summary>
			/// Adds a random offset to the components of a <see cref="Vector2"/> within the specified range.
			/// </summary>
			/// <param name="vector">The original vector to which the random offset will be applied.</param>
			/// <param name="range">The maximum absolute value of random offsets that can be added 
			/// or subtracted to/from each component of the vector.</param>
			/// <returns>A new <see cref="Vector2"/> with random offsets applied to its X and Y components.
			/// Each offset is in the range [-<paramref name="range"/>, <paramref name="range"/>].</returns>
			public static Vector2 RandomOffset(this Vector2 vector, float range) {
				return vector + new Vector2(
					Random.Range(-range, range),
					Random.Range(-range, range)
				);
			}

			/// <summary>
			/// Adds a random offset to the components of a <see cref="Vector3Int"/> within the specified range.
			/// </summary>
			/// <param name="vector">The original vector to which the random offset will be applied.</param>
			/// <param name="range">The maximum absolute value of random offsets that can be added 
			/// or subtracted to/from each component of the vector.</param>
			/// <returns>A new <see cref="Vector3Int"/> with random offsets applied to its X, Y, and Z components.
			/// Each offset is in the range [-<paramref name="range"/>, <paramref name="range"/>].</returns>
			public static Vector3Int RandomOffset(this Vector3Int vector, int range) {
				return vector + new Vector3Int(
					Random.Range(-range, range + 1),
					Random.Range(-range, range + 1),
					Random.Range(-range, range + 1)
				);
			}

			/// <summary>
			/// Adds a random offset to the components of a <see cref="Vector2Int"/> within the specified range.
			/// </summary>
			/// <param name="vector">The original vector to which the random offset will be applied.</param>
			/// <param name="range">The maximum absolute value of random offsets that can be added 
			/// or subtracted to/from each component of the vector.</param>
			/// <returns>A new <see cref="Vector2Int"/> with random offsets applied to its X and Y components.
			/// Each offset is in the range [-<paramref name="range"/>, <paramref name="range"/>].</returns>
			public static Vector2Int RandomOffset(this Vector2Int vector, int range) {
				return vector + new Vector2Int(
					Random.Range(-range, range + 1),
					Random.Range(-range, range + 1)
				);
			}

			/// <summary>
			/// Computes a random point in an annulus (a ring-shaped area) based on minimum and 
			/// maximum radius values around a central Vector3 point (origin).
			/// </summary>
			/// <param name="origin">The center Vector3 point of the annulus.</param>
			/// <param name="minRadius">Minimum radius of the annulus.</param>
			/// <param name="maxRadius">Maximum radius of the annulus.</param>
			/// <returns>A random Vector3 point within the specified annulus.</returns>
			public static Vector3 RandomPointInAnnulus(this Vector3 origin, float minRadius, float maxRadius) {
				float angle = Random.value * Mathf.PI * 2f;
				Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
		
				// Squaring and then square-rooting radii to ensure uniform distribution within the annulus
				float minRadiusSquared = minRadius * minRadius;
				float maxRadiusSquared = maxRadius * maxRadius;
				float distance = Mathf.Sqrt(Random.value * (maxRadiusSquared - minRadiusSquared) + minRadiusSquared);
		
				// Converting the 2D direction vector to a 3D position vector
				Vector3 position = new Vector3(direction.x, 0, direction.y) * distance;
				return origin + position;
			}

			/// <summary>
			/// Computes a random point in an annulus (a ring-shaped area) based on minimum and 
			/// maximum radius values around a central Vector2 point (origin).
			/// </summary>
			/// <param name="origin">The center Vector2 point of the annulus.</param>
			/// <param name="minRadius">Minimum radius of the annulus.</param>
			/// <param name="maxRadius">Maximum radius of the annulus.</param>
			/// <returns>A random Vector2 point within the specified annulus.</returns>
			public static Vector2 RandomPointInAnnulus(this Vector2 origin, float minRadius, float maxRadius) {
				float angle = Random.value * Mathf.PI * 2f;
				Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
		
				// Squaring and then square-rooting radii to ensure uniform distribution within the annulus
				float minRadiusSquared = minRadius * minRadius;
				float maxRadiusSquared = maxRadius * maxRadius;
				float distance = Mathf.Sqrt(Random.value * (maxRadiusSquared - minRadiusSquared) + minRadiusSquared);
		
				// Calculate the position vector
				Vector2 position = direction * distance;
				return origin + position;
			}
		#endregion

        #region /// General ///
			public static Vector3 Normalize(this Vector3 v3) => v3.normalized;
			public static Vector2 Normalize(this Vector2 v2) => v2.normalized;

			// public static Vector3 Clamp(this Vector3 v3, Vector3 min, Vector3 max) => v3.With(x: v3.x.Clamp(min.x, max.x), y: v3.y.Clamp(min.y, max.y), z: v3.z.Clamp(min.z, max.z));
			// public static Vector2 Clamp(this Vector2 v2, Vector2 min, Vector2 max) => v2.With(x: v2.x.Clamp(min.x, max.x), y: v2.y.Clamp(min.y, max.y));
			// public static Vector3Int Clamp(this Vector3Int v3Int, Vector3Int min, Vector3Int max) => v3Int.With(x: v3Int.x.Clamp(min.x, max.x), y: v3Int.y.Clamp(min.y, max.y), z: v3Int.z.Clamp(min.z, max.z));
			// public static Vector2Int Clamp(this Vector2Int v2Int, Vector2Int min, Vector2Int max) => v2Int.With(x: v2Int.x.Clamp(min.x, max.x), y: v2Int.y.Clamp(min.y, max.y));

			public static Vector3 Clamp(this Vector3 v3, Vector3 min, Vector3 max) => new Vector3(x: v3.x.Clamp(min.x, max.x), y: v3.y.Clamp(min.y, max.y), z: v3.z.Clamp(min.z, max.z));
			public static Vector2 Clamp(this Vector2 v2, Vector2 min, Vector2 max) => new Vector2(x: v2.x.Clamp(min.x, max.x), y: v2.y.Clamp(min.y, max.y));
			public static Vector3Int Clamp(this Vector3Int v3Int, Vector3Int min, Vector3Int max) => new Vector3Int(x: v3Int.x.Clamp(min.x, max.x), y: v3Int.y.Clamp(min.y, max.y), z: v3Int.z.Clamp(min.z, max.z));
			public static Vector2Int Clamp(this Vector2Int v2Int, Vector2Int min, Vector2Int max) => new Vector2Int(x: v2Int.x.Clamp(min.x, max.x), y: v2Int.y.Clamp(min.y, max.y));

			public static Vector3 Clamp(this Vector3 v3, float xMin, float yMin, float zMin, float xMax, float yMax, float zMax) => v3.With(x: v3.x.Clamp(xMin, xMax), y: v3.y.Clamp(yMin, yMax), z: v3.z.Clamp(zMin, zMax));
			public static Vector2 Clamp(this Vector2 v2, float xMin, float yMin, float xMax, float yMax) => v2.With(x: v2.x.Clamp(xMin, xMax), y: v2.y.Clamp(yMin, yMax));
			public static Vector3Int Clamp(this Vector3Int v3Int, int xMin, int yMin, int zMin, int xMax, int yMax, int zMax) => v3Int.With(x: v3Int.x.Clamp(xMin, xMax), y: v3Int.y.Clamp(yMin, yMax), z: v3Int.z.Clamp(zMin, zMax));
			public static Vector2Int Clamp(this Vector2Int v2Int, int xMin, int yMin, int xMax, int yMax) => v2Int.With(x: v2Int.x.Clamp(xMin, xMax), y: v2Int.y.Clamp(yMin, yMax));

			public static float LerpTo(this float a, float b, float t) => (b - a) * t + a;
			public static float SpeedLerpTo(this float a, float b, float speed, float dt) {
				var v = b - a;
				var dv = speed * dt;
				if (dv > v)
					return b;
				else
					return a + v * dv;
			}

			/// <summary>
			/// /// Unity's Vector3.Lerp clamps between 0->1, this allows a true lerp of all ranges.
			/// </summary>
			/// <param name="a"></param>
			/// <param name="b"></param>
			/// <param name="t"></param>
			/// <returns></returns>
			public static Vector3 LerpTo(this Vector3 a, Vector3 b, float t) => (b - a) * t + a;

			/// <summary>
			/// Moves from a to b at some speed dependent of a delta time with out passing b.
			/// </summary>
			/// <param name="a"></param>
			/// <param name="b"></param>
			/// <param name="speed"></param>
			/// <param name="dt"></param>
			/// <returns></returns>
			public static Vector3 SpeedLerpTo(this Vector3 a, Vector3 b, float speed, float dt) {
				var v = b - a;
				var dv = speed * dt;
				if (dv > v.magnitude)
					return b;
				else
					return a + v.normalized * dv;
			}

			/// <summary>
			/// Unity's Vector2.Lerp clamps between 0->1, this allows a true lerp of all ranges.
			/// </summary>
			/// <param name="a"></param>
			/// <param name="b"></param>
			/// <param name="t"></param>
			/// <returns></returns>
			public static Vector2 LerpTo(this Vector2 a, Vector2 b, float t) => (b - a) * t + a;

			/// <summary>
			/// Moves from a to b at some speed dependent of a delta time with out passing b.
			/// </summary>
			/// <param name="a"></param>
			/// <param name="b"></param>
			/// <param name="speed"></param>
			/// <param name="dt"></param>
			/// <returns></returns>
			public static Vector2 SpeedLerpTo(this Vector2 a, Vector2 b, float speed, float dt) {
				var v = b - a;
				var dv = speed * dt;
				if (dv > v.magnitude)
					return b;
				else
					return a + v.normalized * dv;
			}
		#endregion

		#region /// Distance ///
			public static float SqrDistance(this Vector3 from, Vector3 to) => (to - from).sqrMagnitude;
			public static float SqrDistance(this Vector2 from, Vector2 to) => (to - from).sqrMagnitude;
			public static int SqrDistance(this Vector3Int from, Vector3Int to) => (to - from).sqrMagnitude;
			public static int SqrDistance(this Vector2Int from, Vector2Int to) => (to - from).sqrMagnitude;
		#endregion

		#region /// To String ///
			public static string Stringify(this Vector3 v3) {
				return v3.x.ToString() + "," + v3.y.ToString() + "," + v3.z.ToString();
			}

			public static string Stringify(this Vector2 v2) {
				return v2.x.ToString() + "," + v2.y.ToString();
			}
		#endregion

		#region /// Angles ///
			/// <summary>
			/// Get Vector2 from angle.
			/// </summary>
			public static Vector2 AngleFloatToVector2(this float a, bool useRadians = false, bool yDominant = false) {
				float angle = a;
				if (!useRadians) angle *= Utils.DEG_TO_RAD;
				if (yDominant) return new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
				else return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
			}

			public static Vector2 AngleIntToVector2(this int a, bool useRadians = false, bool yDominant = false) {
				int angle = a;
				if (!useRadians) angle = (angle * Utils.DEG_TO_RAD).ToIntRound();
				if (yDominant) return new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
				else return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
			}

			/// <summary>
			/// Get the float angle in degrees off the forward defined by x.
			/// </summary>
			/// <param name="v"></param>
			/// <returns></returns>
			public static float AngleFloat(this Vector2 v) {
				return Mathf.Atan2(v.normalized.y, v.normalized.x) * Utils.RAD_TO_DEG;
			}

			/// <summary>
			/// Get the int angle in degrees off the forward defined by x.
			/// </summary>
			/// <param name="v"></param>
			/// <returns></returns>
			public static int AngleInt(this Vector2 v) {
				return (Mathf.Atan2(v.normalized.y, v.normalized.x) * Utils.RAD_TO_DEG).ToIntRound();
			}

			/// <summary>
			/// Get the float angle in degrees off the forward defined by x.
			/// Due to float error the dot / mag can sometimes be ever so slightly over 1, which can cause NaN in acos.
			/// </summary>
			/// <param name="v"></param>
			/// <returns>Mathf.Acos(Vector2.Dot(a, b) / (a.magnitude * b.magnitude)) * MathUtil.RAD_TO_DEG;</returns>
			public static float AngleBetween(Vector2 a, Vector2 b) {
				// // Due to float error the dot / mag can sometimes be ever so slightly over 1, which can cause NaN in acos.
				//return Mathf.Acos(Vector2.Dot(a, b) / (a.magnitude * b.magnitude)) * MathUtil.RAD_TO_DEG;
				double d = (double)Vector2.Dot(a, b) / ((double)a.magnitude * (double)b.magnitude);
				if (d >= 1d) return 0f;
				else if (d <= -1d) return 180f;
				return (float)System.Math.Acos(d) * Utils.RAD_TO_DEG;
			}
			
			/// <summary>
			/// Angle in degrees off some axis in the counter-clockwise direction. Think of like 'Angle' or 'Atan2' where you get to control
			/// which axis as opposed to only measuring off of <1,0>.
			/// </summary>
			/// <param name="v"></param>
			/// <returns></returns>
			public static float AngleOff(this Vector2 v, Vector2 axis) {
				if (axis.sqrMagnitude < 0.0001f) return float.NaN;
				axis.Normalize();
				var tang = new Vector2(-axis.y, axis.x);
				return AngleBetween(v, axis) * Mathf.Sign(Vector2.Dot(v, tang));
			}

			/// <summary>
			/// Rotate Vector2 counter-clockwise by 'a'.
			/// </summary>
			/// <param name="v"></param>
			/// <param name="a"></param>
			/// <returns></returns>
			public static Vector2 RotateBy(this Vector2 v, float a, bool bUseRadians = false) {
				if (!bUseRadians) a *= Utils.DEG_TO_RAD;
				var ca = System.Math.Cos(a);
				var sa = System.Math.Sin(a);
				var rx = v.x * ca - v.y * sa;

				return new Vector2((float)rx, (float)(v.x * sa + v.y * ca));
			}

			/// <summary>
			/// Rotates a vector toward another. Magnitude of the from vector is maintained.
			/// </summary>
			/// <param name="from"></param>
			/// <param name="to"></param>
			/// <param name="a"></param>
			/// <param name="bUseRadians"></param>
			/// <returns></returns>
			public static Vector2 RotateToward(this Vector2 from, Vector2 to, float a, bool bUseRadians = false) {
				if (!bUseRadians) a *= Utils.DEG_TO_RAD;
				var a1 = Mathf.Atan2(from.y, from.x);
				var a2 = Mathf.Atan2(to.y, to.x);
				a2 = Utils.ShortenAngleToAnother(a2, a1, true);
				var ra = (a2 - a1 >= 0f) ? a1 + a : a1 - a;
				var l = from.magnitude;
				return new Vector2(Mathf.Cos(ra) * l, Mathf.Sin(ra) * l);
			}

			public static Vector2 RotateTowardClamped(this Vector2 from, Vector2 to, float a, bool bUseRadians = false) {
				if (!bUseRadians) a *= Utils.DEG_TO_RAD;
				var a1 = Mathf.Atan2(from.y, from.x);
				var a2 = Mathf.Atan2(to.y, to.x);
				a2 = Utils.ShortenAngleToAnother(a2, a1, true);

				var da = a2 - a1;
				var ra = a1 + Mathf.Clamp(Mathf.Abs(a), 0f, Mathf.Abs(da)) * Mathf.Sign(da);

				var l = from.magnitude;
				return new Vector2(Mathf.Cos(ra) * l, Mathf.Sin(ra) * l);
			}
		#endregion
    }
}