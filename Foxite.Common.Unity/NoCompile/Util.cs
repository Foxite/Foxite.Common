// This is a compilation of helper functions.
// Go ahead and add your own stuff, and copy everything for your own use.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FoxiteUtil {
	public static class Util {
		#region Oscillate
		/// <summary>
		/// Oscillates a value, forever or for a given time. Start this as a coroutine.
		/// </summary>
		/// <param name="seconds">If null, this will continue forever until the coroutine is stopped.</param>
		/// <param name="xOffset">Value to add to the time value in the oscillation.</param>
		/// <param name="yOffset">Value to add to the result of the oscillation.</param>
		public static IEnumerator Oscillate(Action<float> setAction, float frequency = 1, float amplitude = 1, float xOffset = 0, float yOffset = 0, float? seconds = null) {
			float t = 0;
			while (!seconds.HasValue || (t < seconds)) {
				setAction(Mathf.Sin((t + xOffset) * frequency) * amplitude + yOffset);
				t += Time.deltaTime;
				yield return null;
			}
		}
		#endregion

		#region Run after
		/// <summary>
		/// Runs a function after n seconds. Start this as a coroutine.
		/// </summary>
		public static IEnumerator RunAfterSeconds(float seconds, Action lambda) {
			yield return new WaitForSeconds(seconds);
			lambda();
		}

		/// <summary>
		/// Runs a function atfer n realtime seconds, ignoring Time.timeScale. Start this as a coroutine.
		/// </summary>
		/// <param name="realSeconds"></param>
		/// <param name="lambda"></param>
		/// <returns></returns>
		public static IEnumerator RunAfterRealSeconds(float realSeconds, Action lambda) {
			yield return new WaitForSecondsRealtime(realSeconds);
			lambda();
		}

		/// <summary>
		/// Runs a function after n frames. Start this as a coroutine.
		/// </summary>
		public static IEnumerator RunAfterFrames(int frames, Action lambda) {
			for (; frames > 1; frames--) {
				yield return null;
			}
			lambda();
		}

		/// <summary>
		/// Runs a function after n seconds. Start this as a coroutine.
		/// </summary>
		public static IEnumerator RunNextFrame(Action lambda) {
			yield return null;
			lambda();
		}

		/// <summary>
		/// Given a function that returns a bool, this function will execute another function when the first function returns true. Start this as a coroutine.
		/// </summary>
		public static IEnumerator RunAfterCondition(Func<bool> condition, Action lambda) {
			yield return new WaitUntil(condition);
			lambda();
		}
		#endregion

		#region CoroutineFor
		public static IEnumerator CoroutineForFrame(Action<int> action, int start, int end, int step = 1, Action post = null) {
			for (int i = start; i < end; i += step) {
				action(i);
				yield return null;
			}
			if (post != null) {
				post();
			}
		}

		public static IEnumerator CoroutineForFrame(Action<float> action, float start, float end, float step = 1, Action post = null) {
			for (float i = start; i < end; i += step) {
				action(i);
				yield return null;
			}
			if (post != null) {
				post();
			}
		}

		public static IEnumerator CoroutineForFrameDeltaTime(Action<float> action, float start, float end, Action post = null) {
			for (float i = start; i < end; i += Time.deltaTime) {
				action(i);
				yield return null;
			}
			if (post != null) {
				post();
			}
		}
		#endregion CoroutineFor

		#region RunEvery
		public static IEnumerator RunEveryFrame(Action<int> lambda, int frames) {
			for (int i = 0; i < frames; i++) {
				lambda(i);
				yield return null;
			}
		}

		public static IEnumerator RunEverySecond(Action<float> lambda, float seconds, float step = 1) {
			for (float t = 0; t < seconds; t += step) {
				lambda(t);
				yield return new WaitForSeconds(step);
			}
		}

		public static IEnumerator RunSequenceFrame(Action[] lambdas) {
			foreach (Action lambda in lambdas) {
				lambda();
				yield return null;
			}
		}

		public static IEnumerator RunSequenceTime(Action[] lambdas, float secondInterval) {
			foreach (Action lambda in lambdas) {
				lambda();
				yield return new WaitForSeconds(secondInterval);
			}
		}

		public static IEnumerator RepeatInterval(Action lambda, float interval) {
			while (true) {
				yield return new WaitForSeconds(interval);
				lambda();
			}
		}

		public static IEnumerator RepeatFrameInterval(Action lambda, int frameInterval) {
			if (frameInterval < 1) {
				throw new ArgumentException("Non-positive number passed to RepeatFrameInterval.");
			}

			if (frameInterval == 1) {
				Debug.LogWarning("Consider using RepeatEveryFrame instead of RepeatFrameInterval(function, 1).");
			}

			while (true) {
				for (int i = 0; i < frameInterval; i++) {
					yield return null;
				}
				lambda();
			}
		}

		public static IEnumerator RepeatEveryFrame(Action lambda) {
			while (true) {
				yield return null;
				lambda();
			}
		}
		#endregion RunEvery

		#region Is sorted
		/// <summary>
		/// Checks if an array is sorted with the logic given.
		/// </summary>
		/// <param name="func">This function will be called for every item in the array, with that item as the first argument, and the item after it as the second. If the function returns false anywhere, the list is not sorted.</param>
		public static bool IsSorted<T>(T[] array, Func<T, T, bool> func) {
			for (int i = 0; i < array.Length - 1; i++) {
				if (!func(array[i], array[i + 1]))
					return false;
			}
			return true;
		}

		/// <summary>
		/// Checks if an array is sorted with the default logic for that type.
		/// </summary>
		public static bool IsSorted<T>(T[] array, bool ascending = true) where T : IComparable<T> {
			if (ascending) {
				for (int i = 0; i < array.Length - 1; i++) {
					if (array[i].CompareTo(array[i + 1]) > 0)
						return false;
				}
			} else {
				for (int i = 0; i < array.Length - 1; i++) {
					if (array[i].CompareTo(array[i + 1]) < 0)
						return false;
				}
			}
			return true;
		}
		#endregion

		#region Shuffle
		public static void Shuffle(this IList list) {
			// https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle

			System.Random rand = new System.Random();

			for (int i = list.Count; i > 1; i--) {
				// Swap the item with a random item in the list that we didn't already go past
				int swap = rand.Next(i + 1);
				object value = list[swap];
				list[swap] = list[i];
				list[i] = value;
			}
		}

		public static void Shuffle<T>(this IList<T> list) {
			// https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle

			System.Random rand = new System.Random();

			for (int i = list.Count; i > 1; i--) {
				// Swap the item with a random item in the list that we didn't already go past
				int swap = rand.Next(i + 1);
				T value = list[swap];
				list[swap] = list[i];
				list[i] = value;
			}
		}
		#endregion

		#region Update hierarchy array
		/// <summary>
		/// This method will make sure there are exactly {newItemCount} active gameObjects under the given array, by activating existing objects,
		///  and instantiating {newObjectPrefab} if necessary, deactivating existing objects.
		/// It will then call {updateFunction} for each child of the array.
		/// </summary>
		/// <param name="array">The Transform that acts as an array in the hierarchy.</param>
		/// <param name="newItemCount">The total amount of active children {array} should have.</param>
		/// <param name="newObjectPrefab">Prefab to be instantiated if no children can be activated in {array}.</param>
		/// <param name="updateFunction">Function that takes a transfrom and its index in {array}.</param>
		public static void UpdateHierarchyArray(Transform array, int newItemCount, GameObject newObjectPrefab, Action<Transform, int> updateFunction) {
			if (newItemCount < 0) {
				throw new ArgumentException("newItemCount must be above 0");
			}
			int activeChildren = 0;
			int inactiveChildren = 0;
			for (int i = 0; i < array.childCount; i++) {
				if (array.GetChild(i).gameObject.activeSelf) {
					activeChildren++;
				} else {
					inactiveChildren++;
				}
			}

			if (activeChildren > newItemCount) {
				// Deactivate children
				for (int i = 0; i < array.childCount && activeChildren > newItemCount; i++) {
					if (array.GetChild(i).gameObject.activeSelf) {
						array.GetChild(i).gameObject.SetActive(false);
						activeChildren--;
					}
				}
			} else if (activeChildren < newItemCount) {
				// Activate existing objects
				for (int i = 0; i < array.childCount && activeChildren < newItemCount; i++) {
					if (!array.GetChild(i).gameObject.activeSelf) {
						array.GetChild(i).gameObject.SetActive(true);
						activeChildren++;
					}
				}

				// Instantiate new items
				if (inactiveChildren == 0) {
					while (activeChildren < newItemCount) {
						UnityEngine.Object.Instantiate(newObjectPrefab, array);
						activeChildren++;
					}
				}
			}

			for (int i_array = 0, i_active = 0; i_array < array.childCount; i_array++) {
				if (array.GetChild(i_array).gameObject.activeSelf) {
					updateFunction(array.GetChild(i_array), i_active);
					i_active++;
				}
			}
		}
		#endregion

		#region HasComponent
		public static bool HasComponent<T>(this Component thisC, out T outC) where T : Component {
			outC = thisC.GetComponent<T>();
			return outC != null;
		}

		public static bool HasComponent<T>(this Component thisC) where T : Component {
			return thisC.GetComponent<T>();
		}

		public static bool HasComponent<T>(this GameObject thisGO, out T outC) where T : Component {
			outC = thisGO.GetComponent<T>();
			return outC != null;
		}

		public static bool HasComponent<T>(this GameObject thisC) where T : Component {
			return thisC.GetComponent<T>();
		}

		#endregion

		#region 8/4 directions
		/// <summary>
		/// Enumerates 8 coordinates adjacent to (0, 0), starting at (1, 0) and moving clockwise.
		/// </summary>
		public static IEnumerable<Vector2Int> EightDirections() {
			yield return new Vector2Int(1, 0); // :thinking:
			yield return new Vector2Int(1, -1);
			yield return new Vector2Int(0, -1);
			yield return new Vector2Int(-1, -1);
			yield return new Vector2Int(-1, 0);
			yield return new Vector2Int(-1, 1);
			yield return new Vector2Int(0, 1);
			yield return new Vector2Int(1, 1);
		}

		private static IEnumerable<Vector3Int> EightDirections3(int z = 0) {
			yield return new Vector3Int(1, 0, z); // :thinking:
			yield return new Vector3Int(1, -1, z);
			yield return new Vector3Int(0, -1, z);
			yield return new Vector3Int(-1, -1, z);
			yield return new Vector3Int(-1, 0, z);
			yield return new Vector3Int(-1, 1, z);
			yield return new Vector3Int(0, 1, z);
			yield return new Vector3Int(1, 1, z);
		}

		/// <summary>
		/// Enumerates 8 coordinates adjacent to the given Vector2Int, starting at the coordinate to the right and moving clockwise.
		/// </summary>
		public static IEnumerable<Vector2Int> EightDirections(Vector2Int pos) {
			foreach (Vector2Int adj in EightDirections()) {
				yield return pos + adj;
			}
		}

		public static IEnumerable<Vector3Int> EightDirections3(Vector3Int pos) {
			foreach (Vector3Int adj in EightDirections3()) {
				yield return pos + adj;
			}
		}

		/// <summary>
		/// Enumerates 8 coordinates adjacent to the given Vector2Int, starting at the coordinate to the right and moving clockwise.
		/// </summary>
		public static IEnumerable<Vector2Int> EightDirections(int x, int y) {
			foreach (Vector2Int yld in EightDirections(new Vector2Int(x, y))) {
				yield return yld;
			}
		}

		public static IEnumerable<Vector3Int> EightDirections3(int x, int y, int z) {
			foreach (Vector3Int yld in EightDirections3(new Vector3Int(x, y, z))) {
				yield return yld;
			}
		}

		/// <summary>
		/// Enumerates the 4 coordinates orthogonal to (0, 0), starting at (1, 0) and moving clockwise.
		/// </summary>
		public static IEnumerable<Vector2Int> FourDirections() {
			yield return new Vector2Int(1, 0);
			yield return new Vector2Int(-1, 0);
			yield return new Vector2Int(0, -1);
			yield return new Vector2Int(0, 1);
		}

		/// <summary>
		/// Enumerates the 4 coordinates orthogonal to (0, 0), starting at (1, 0) and moving clockwise.
		/// </summary>
		public static IEnumerable<Vector3Int> FourDirections3(int z = 0) {
			yield return new Vector3Int(1, 0, z);
			yield return new Vector3Int(-1, 0, z);
			yield return new Vector3Int(0, -1, z);
			yield return new Vector3Int(0, 1, z);
		}

		/// <summary>
		/// Enumerates the 4 coordinates orthogonal to the given Vector2Int, starting at the coordinate to the right and moving clockwise.
		/// </summary>
		public static IEnumerable<Vector2Int> FourDirections(Vector2Int pos) {
			foreach (Vector2Int adj in FourDirections()) {
				yield return pos + adj;
			}
		}

		/// <summary>
		/// Enumerates the first 4 of the 8 coordinates adjacent to the given Vector2Int, starting at the coordinate to the right and moving clockwise.
		/// </summary>
		public static IEnumerable<Vector2Int> FourDirections(int x, int y) {
			foreach (Vector2Int yld in FourDirections(new Vector2Int(x, y))) {
				yield return yld;
			}
		}

		public static IEnumerable<Vector3Int> FourDirections3(Vector3Int pos) {
			foreach (Vector3Int adj in FourDirections3()) {
				yield return pos + adj;
			}
		}

		public static IEnumerable<Vector3Int> FourDirections3(int x, int y, int z) {
			foreach (Vector3Int yld in FourDirections3(new Vector3Int(x, y, z))) {
				yield return yld;
			}
		}
		#endregion

		#region Vector2Int <-> Vector3
		public static Vector3Int V2toV3(Vector2Int vec, int z = 0) {
			return new Vector3Int(vec.x, vec.y, z);
		}

		public static Vector2Int V3toV2(Vector3Int vec) {
			return new Vector2Int(vec.x, vec.y);
		}

		public static Vector3 V2toV3(Vector2 vec, int z = 0) {
			return new Vector3(vec.x, vec.y, z);
		}

		public static Vector2 V3toV2(Vector3 vec) {
			return new Vector2(vec.x, vec.y);
		}
		#endregion

		#region ScreenPosToWorldPos
		/// <summary>
		/// Calculates the world position visible on the given screen position.
		/// </summary>
		/// <param name="screenPos">null to use the mouse position.</param>
		/// <returns>null if no object is visible at the position.</returns>
		public static Vector3? ScreenPosToWorldPos(Vector3? screenPos = null) {
			if (screenPos == null) {
				screenPos = Input.mousePosition;
			}

			Ray ray = Camera.main.ScreenPointToRay(screenPos.Value);
			Plane z0plane = new Plane(Vector3.back, 0);
			if (z0plane.Raycast(ray, out float dist)) {
				return ray.GetPoint(dist);
			} else {
				return null;
			}
		}
		#endregion

		#region Transform.GetChildWithTag
		public static Transform GetChildWithTag(this Transform transform, string tag) {
			for (int i = 0; i < transform.childCount; i++) {
				if (transform.GetChild(i).CompareTag(tag)) {
					return transform.GetChild(i);
				}
			}
			return null;
		}

		public static Transform[] GetChildrenWithTag(this Transform transform, string tag) {
			List<Transform> items = new List<Transform>();
			for (int i = 0; i < transform.childCount; i++) {
				if (transform.GetChild(i).CompareTag(tag)) {
					items.Add(transform.GetChild(i));
				}
			}
			return items.ToArray();
		}
		#endregion

		#region Remap
		public static float Remap(float value, float from1, float to1, float from2, float to2) {
			return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
		}
		#endregion

		#region Transform visibility
		// From https://forum.unity.com/threads/test-if-ui-element-is-visible-on-screen.276549/#post-2978773

		/// <summary>
		/// Counts the bounding box corners of the given RectTransform that are visible from the given Camera in screen space.
		/// </summary>
		/// <returns>The amount of bounding box corners that are visible from the Camera.</returns>
		/// <param name="rectTransform">Rect transform.</param>
		/// <param name="camera">Camera.</param>
		private static int CountCornersVisibleFrom(this RectTransform rectTransform, Camera camera) {
			Rect screenBounds = new Rect(0f, 0f, Screen.width, Screen.height); // Screen space bounds (assumes camera renders across the entire screen)
			Vector3[] objectCorners = new Vector3[4];
			rectTransform.GetWorldCorners(objectCorners);

			int visibleCorners = 0;
			Vector3 tempScreenSpaceCorner; // Cached
			for (var i = 0; i < objectCorners.Length; i++) // For each corner in rectTransform
			{
				tempScreenSpaceCorner = camera.WorldToScreenPoint(objectCorners[i]); // Transform world space position of corner to screen space
				if (screenBounds.Contains(tempScreenSpaceCorner)) // If the corner is inside the screen
				{
					visibleCorners++;
				}
			}
			return visibleCorners;
		}

		/// <summary>
		/// Determines if this RectTransform is fully visible from the specified camera.
		/// Works by checking if each bounding box corner of this RectTransform is inside the cameras screen space view frustrum.
		/// </summary>
		/// <returns><c>true</c> if is fully visible from the specified camera; otherwise, <c>false</c>.</returns>
		/// <param name="rectTransform">Rect transform.</param>
		/// <param name="camera">Camera.</param>
		public static bool IsFullyVisibleFrom(this RectTransform rectTransform, Camera camera) {
			return CountCornersVisibleFrom(rectTransform, camera) == 4; // True if all 4 corners are visible
		}

		/// <summary>
		/// Determines if this RectTransform is at least partially visible from the specified camera.
		/// Works by checking if any bounding box corner of this RectTransform is inside the cameras screen space view frustrum.
		/// </summary>
		/// <returns><c>true</c> if is at least partially visible from the specified camera; otherwise, <c>false</c>.</returns>
		/// <param name="rectTransform">Rect transform.</param>
		/// <param name="camera">Camera.</param>
		public static bool IsVisibleFrom(this RectTransform rectTransform, Camera camera) {
			return CountCornersVisibleFrom(rectTransform, camera) > 0; // True if any corners are visible
		}
		#endregion

		#region Bounds
		public static Vector3 GetRandomPosition(this Bounds bounds) {
			return new Vector3(
						Random.Range(bounds.min.x, bounds.max.x),
						Random.Range(bounds.min.y, bounds.max.y),
						Random.Range(bounds.min.z, bounds.max.z));
		}

		public static Vector3Int GetRandomPosition(this BoundsInt bounds) {
			return new Vector3Int(
						Random.Range(bounds.min.x, bounds.max.x),
						Random.Range(bounds.min.y, bounds.max.y),
						Random.Range(bounds.min.z, bounds.max.z));
		}
		#endregion Bounds
	}

	#region EventHandler
	public class ValueChangedEvent<T> : EventArgs {
		public readonly T oldValue;
		public readonly T newValue;

		public ValueChangedEvent(T oldValue, T newValue) {
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
	}
	#endregion

	#region Singletons
	public abstract class Singleton<T> : MonoBehaviour where T : Component {
		protected static T _instance;
		public static T Instance {
			get {
				if (_instance == null) {
					Debug.LogError("The singleton " + typeof(T).FullName + " doesn't have an instance yet!");
				}

				return _instance;
			}
		}

		protected virtual void Awake() {
			if (_instance != null) {
				Debug.LogError("Trying to create 2 instances of the " + typeof(T).FullName + " singleton! Existing Object: " + _instance.gameObject.name + " Failed object: " + gameObject.name);
				Destroy(gameObject);
				return;
			}

			_instance = this as T;
			Debug.Assert(_instance != null);
		}

		protected virtual void OnDestroy() {
			if (_instance == this)
				_instance = null;
		}
	}

	/// <summary>
	/// Due to Unity limitations, UnityEngine.Objects cannot be a lazy singleton.
	/// </summary>
	public abstract class LazySingleton<T> where T : new() {
		private static T s_Instance;
		public static T Instance {
			get {
				if (s_Instance == null) {
					s_Instance = new T();
				}

				return s_Instance;
			}
		}
	}
	#endregion
}
