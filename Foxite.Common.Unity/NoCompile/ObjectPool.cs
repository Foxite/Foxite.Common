using System;
using System.Runtime.Serialization;
using UnityEngine;

namespace FoxiteUtil {
	/// <summary>
	/// A pool of objects that stores items for instantaneous deployment into the scene.
	/// 
	/// They are kept inactive as children of this GameObject, and are activated by this class and then initialized by themselves (usually with arguments given by the requesting MonoBehaviour).
	/// </summary>
	public class ObjectPool : MonoBehaviour {
		[SerializeField]
		private PooledObject m_ItemPrefab;

		private void Start() {
			int activeObjects = 0;
			foreach (PooledObject object_ in transform.GetComponentsInChildren<PooledObject>(true)) {
				if (object_.gameObject.activeSelf) {
					activeObjects++;
				}
			}
			if (activeObjects != 0) {
				Debug.LogWarning("ObjectPool " + name + " has " + activeObjects + " active objects in it. Pooled objects should usually be inactive when starting.");
			}
		}

		/// <summary>
		/// Puts an item back into the pool for later reuse.
		/// </summary>
		public virtual void ReturnToPool(PooledObject theObject) {
			theObject.transform.parent = transform;
			theObject.OnReturnToPool(this);
			theObject.gameObject.SetActive(false);
		}

		/// <summary>
		/// Takes an item from the pool and returns it.
		/// </summary>
		/// <param name="params_">Parameters for activating the object. Refer to the pooled object's documentation for details.</param>
		public virtual PooledObject TakeFromPool(float xPos, float yPos, float zPos, params object[] params_) {
			PooledObject theObject = transform.GetComponentInChildren<PooledObject>(true);

			if (theObject == null) {
				if (m_ItemPrefab == null) {
					throw new PoolEmptyException("ObjectPool " + name + " was empty and no prefab was supplied to instantiate.");
				} else {
					Debug.LogWarning("ObjectPool " + name + " was empty and a new item has been initialized.");
					theObject = Instantiate(m_ItemPrefab);
				}
			}
			theObject.gameObject.SetActive(true);
			theObject.transform.parent = null;
			theObject.OnTakeFromPool(this, xPos, yPos, zPos, params_);
			return theObject;
		}

		/// <summary>
		/// Takes an item from the pool and returns it.
		/// </summary>
		/// <param name="params_">Parameters for activating the object. Refer to the pooled object's documentation for details.</param>
		public virtual PooledObject TakeFromPool(Vector3 position, params object[] params_) {
			return TakeFromPool(position.x, position.y, position.z, params_);
		}
	}

	/// <summary>
	/// Object contained in an ObjectPool.
	/// The PooledObject should take care of initializing itself. The ObjectPool will take care of activating and deactivating the PooledObject.
	/// </summary>
	public abstract class PooledObject : MonoBehaviour {
		public abstract void OnTakeFromPool(ObjectPool pool, float xPos, float yPos, float zPos, params object[] params_);
		public abstract void OnReturnToPool(ObjectPool pool);
	}

	/// <summary>
	/// Exception thrown when a pool cannot supply a new item because it is empty and it cannot make a new item.
	/// </summary>
	public class PoolEmptyException : Exception {
		public PoolEmptyException() : base() { }
		public PoolEmptyException(string message) : base(message) { }
		public PoolEmptyException(string message, Exception innerException) : base(message, innerException) { }
		protected PoolEmptyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
