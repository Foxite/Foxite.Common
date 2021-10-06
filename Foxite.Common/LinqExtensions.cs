using System.Collections;
using System.Collections.Generic;

namespace System.Linq {
	/// <summary>
	/// Extension methods for <see cref="IEnumerable{T}"/> and some of its derivates.
	/// </summary>
	public static class LinqExtensions {
		/// <summary>
		/// Returns the index of the first item in the list that returns true when passed into <paramref name="predicate"/>.
		/// </summary>
		public static int IndexOf<T>(this IList<T> list, Func<T, bool> predicate) {
			for (int i = 0; i < list.Count; i++) {
				if (predicate(list[i])) {
					return i;
				}
			}
			return -1;
		}

		/// <summary>
		/// Puts all the items from <paramref name="source"/> that return true when passed into <paramref name="predicate"/> into <paramref name="left"/>,
		/// and all the other ones into <paramref name="right"/>.
		/// </summary>
		public static void Partition<T>(this IEnumerable<T> source, out IList<T> left, out IList<T> right, Predicate<T> predicate) {
			left = new List<T>();
			right = new List<T>();

			foreach (T item in source) {
				if (predicate(item)) {
					left.Add(item);
				} else {
					right.Add(item);
				}
			}
		}

		/// <summary>
		/// Returns an enumeration which starts with <paramref name="prepend"/> and then enumerates <paramref name="source"/>.
		/// </summary>
		public static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, T prepend) {
			yield return prepend;

			foreach (T item in source) {
				yield return item;
			}
		}

		/// <summary>
		/// Returns an IReadOnlyCollection wrapping <paramref name="source"/>, which applies <paramref name="selector"/> when enumerating objects.
		/// </summary>
		public static IReadOnlyCollection<TSelect> CollectionSelect<TCollection, TSelect>(this IReadOnlyCollection<TCollection> source, Func<TCollection, TSelect> selector) =>
			new SelectedReadOnlyCollection<TCollection, TSelect>(source, selector);

		private class SelectedReadOnlyCollection<TCollection, TSelect> : IReadOnlyCollection<TSelect> {
			private readonly IReadOnlyCollection<TCollection> m_Source;
			private readonly Func<TCollection, TSelect> m_Selector;

			public SelectedReadOnlyCollection(IReadOnlyCollection<TCollection> source, Func<TCollection, TSelect> selector) {
				m_Source = source;
				m_Selector = selector;
			}

			public int Count => m_Source.Count;

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
			public IEnumerator<TSelect> GetEnumerator() {
				foreach (TCollection item in m_Source) {
					yield return m_Selector(item);
				}
			}
		}

		/// <summary>
		/// Returns an IReadOnlyList wrapping <paramref name="source"/>, which applies <paramref name="selector"/> when enumerating objects and indexing the read-only list.
		/// </summary>
		public static IReadOnlyList<TSelect> ListSelect<TList, TSelect>(this IReadOnlyList<TList> source, Func<TList, TSelect> selector) =>
			new SelectedReadOnlyList<TList, TSelect>(source, selector);

		private class SelectedReadOnlyList<TList, TSelect> : IReadOnlyList<TSelect> {
			private readonly IReadOnlyList<TList> m_Source;
			private readonly Func<TList, TSelect> m_Selector;

			public SelectedReadOnlyList(IReadOnlyList<TList> source, Func<TList, TSelect> selector) {
				m_Source = source;
				m_Selector = selector;
			}

			public int Count => m_Source.Count;

			public TSelect this[int index] => m_Selector(m_Source[index]);

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
			public IEnumerator<TSelect> GetEnumerator() {
				foreach (TList item in m_Source) {
					yield return m_Selector(item);
				}
			}
		}

		/// <summary>
		/// Creates an IReadOnlyList which represents the contents of <paramref name="source"/>. This function does not project <paramref name="source"/>,
		/// instead the returned object will enumerate items from <paramref name="source"/> when needed. <paramref name="source"/> is never enumerated more than once by this function.
		/// Any <see cref="IEnumerable{T}"/> implementation can be used, however you must specify the item count yourself.
		/// 
		/// The returned object implements <see cref="IDisposable"/>, however it is only necessary to call Dispose() when you supply a custom IEnumerable that returns an IEnumerator
		/// on which Dispose() needs to be called.
		/// </summary>
		public static IReadOnlyList<T> ToLazyList<T>(this IEnumerable<T> source, int count) => new LazyList<T>(source, count);

		private sealed class LazyList<T> : IReadOnlyList<T>, IDisposable {
			private readonly IEnumerator<T> m_Source;
			private readonly List<T> m_BackingList = new List<T>();

			public LazyList(IEnumerable<T> source, int count) {
				Count = count;
				m_Source = source.GetEnumerator();
			}

			public T this[int index] {
				get {
					while (index >= m_BackingList.Count && m_Source.MoveNext()) {
						m_BackingList.Add(m_Source.Current);
					}

					return m_BackingList[index];
				}
			}

			public int Count { get; }

			public void Dispose() => m_Source.Dispose();
			public IEnumerator<T> GetEnumerator() => m_BackingList.GetEnumerator();
			IEnumerator IEnumerable.GetEnumerator() => m_BackingList.GetEnumerator();
		}

		/// <summary>
		/// This will enumerate <paramref name="source"/> and determine if the count is equal to <paramref name="count"/>.
		/// It will stop as soon as it finds an item beyond the target count, which makes it faster than <see cref="Enumerable.Count{TSource}(System.Collections.Generic.IEnumerable{TSource})"/>
		/// </summary>
		/// <remarks>
		/// If <paramref name="source"/> implements <see cref="ICollection{T}"/>, then <see cref="ICollection{T}.Count"/> will be used and <paramref name="source"/>
		/// will not be enumerated.
		/// </remarks>
		public static bool CountEquals<T>(this IEnumerable<T> source, int count) {
			if (source is ICollection<T> ico) {
				return ico.Count == count;
			}

			int i = 0;
			foreach (T _ in source) {
				i++;
				if (i > count) {
					// There were more than {count}
					return false;
				}
			}
			if (i == count) {
				// There were exactly {count}
				return true;
			} else {
				// There were less than {count}
				return false;
			}
		}

		/// <summary>
		/// This will enumerate <paramref name="source"/> and determine if the count is greater than or equal to <paramref name="count"/>.
		/// It will stop as soon as it finds an item beyond the target count, which makes it faster than <see cref="Enumerable.Count{TSource}(IEnumerable{TSource})"/>
		/// </summary>
		/// <remarks>
		/// If <paramref name="source"/> implements <see cref="ICollection{T}"/>, then <see cref="ICollection{T}.Count"/> will be used and <paramref name="source"/>
		/// will not be enumerated.
		/// </remarks>
		public static bool CountIsGreaterThanOrEquals<T>(this IEnumerable<T> source, int count) {
			if (source is ICollection<T> ico) {
				return ico.Count >= count;
			}

			int i = 0;
			foreach (T _ in source) {
				i++;
				if (i >= count) {
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// This will enumerate <paramref name="source"/> and determine if the count is greater than <paramref name="count"/>.
		/// It will stop as soon as it finds an item beyond the target count, which makes it faster than <see cref="Enumerable.Count{TSource}(IEnumerable{TSource})"/>
		/// </summary>
		/// <remarks>
		/// If <paramref name="source"/> implements <see cref="ICollection{T}"/>, then <see cref="ICollection{T}.Count"/> will be used and <paramref name="source"/>
		/// will not be enumerated.
		/// </remarks>
		public static bool CountIsGreaterThan<T>(this IEnumerable<T> source, int count) {
			if (source is ICollection<T> ico) {
				return ico.Count > count;
			}

			int i = 0;
			foreach (T _ in source) {
				i++;
				if (i > count) {
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// This will enumerate <paramref name="source"/> and determine if the count is less than or equal to <paramref name="count"/>.
		/// It will stop as soon as it finds an item beyond the target count, which makes it faster than <see cref="Enumerable.Count{TSource}(IEnumerable{TSource})"/>
		/// </summary>
		/// <remarks>
		/// If <paramref name="source"/> implements <see cref="ICollection{T}"/>, then <see cref="ICollection{T}.Count"/> will be used and <paramref name="source"/>
		/// will not be enumerated.
		/// </remarks>
		public static bool CountIsLessThanOrEquals<T>(this IEnumerable<T> source, int count) {
			if (source is ICollection<T> ico) {
				return ico.Count <= count;
			}

			int i = 0;
			foreach (T _ in source) {
				i++;
				if (i > count) {
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// This will enumerate <paramref name="source"/> and determine if the count is less than <paramref name="count"/>.
		/// It will stop as soon as it finds an item beyond the target count, which makes it faster than <see cref="Enumerable.Count{TSource}(IEnumerable{TSource})"/>
		/// </summary>
		/// <remarks>
		/// If <paramref name="source"/> implements <see cref="ICollection{T}"/>, then <see cref="ICollection{T}.Count"/> will be used and <paramref name="source"/>
		/// will not be enumerated.
		/// </remarks>
		public static bool CountIsLessThan<T>(this IEnumerable<T> source, int count) {
			if (source is ICollection<T> ico) {
				return ico.Count < count;
			}

			int i = 0;
			foreach (T _ in source) {
				i++;
				if (i >= count) {
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Returns the <see cref="LinkedListNode{T}"/> in a <see cref="LinkedList{T}"/>.
		/// </summary>
		public static IEnumerable<LinkedListNode<T>> GetNodes<T>(this LinkedList<T> list) {
			if (list.Count > 0) {
				LinkedListNode<T>? node = list.First;
				while (node != null) {
					yield return node;

					node = node.Next;
				}
			}
		}

		/// <summary>
		/// Returns all <see cref="LinkedListNode{T}"/>s in a <see cref="LinkedList{T}"/>, in reversed order, as opposed to all <typeparamref name="T"/>.
		/// </summary>
		/// <remarks>
		/// This is much faster than <see cref="Enumerable.Reverse{TSource}(IEnumerable{TSource})"/> as that function will enumerate the entire source and store the results,
		/// and then yielding that in reverse. This function yields <see cref="LinkedList{T}.Last"/> and then yields the previous node's <see cref="LinkedListNode{T}.Previous"/>
		/// until there are no more items.
		/// 
		/// This function is not called Reverse to avoid naming conflicts with the aforementioned function.
		/// </remarks>
		public static IEnumerable<LinkedListNode<T>> Backwards<T>(this LinkedList<T> list) {
			if (list.Count > 0) {
				LinkedListNode<T>? node = list.Last;
				while (node != null) {
					yield return node;

					node = node.Previous;
				}
			}
		}

		/// <summary>
		/// Enumerates all items in an <see cref="IList{T}"/> in reverse order.
		/// </summary>
		/// <remarks>
		/// This is much faster than <see cref="Enumerable.Reverse{TSource}(IEnumerable{TSource})"/> as that function will enumerate the entire source and store the results,
		/// and then yielding that in reverse. This function utilizes a reverse for loop over the <see cref="IList{T}"/>.
		/// 
		/// This function is not called Reverse to avoid naming conflicts with the aforementioned function.
		/// </remarks>
		public static IEnumerable<T> Backwards<T>(this IList<T> list) {
			for (int i = list.Count - 1; i >= 0; i--) {
				yield return list[i];
			}
		}

		/// <summary>
		/// Returns the items of <paramref name="source"/> in sets of <paramref name="batchSize"/>.
		/// </summary>
		/// <remarks>
		/// https://stackoverflow.com/a/13710023
		/// </remarks>
		public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int batchSize) {
			using IEnumerator<T> enumerator = source.GetEnumerator();
			while (enumerator.MoveNext()) {
				yield return YieldBatchElements(enumerator, batchSize - 1);
			}
		}

		private static IEnumerable<T> YieldBatchElements<T>(IEnumerator<T> source, int batchSize) {
			yield return source.Current;

			for (int i = 0; i < batchSize && source.MoveNext(); i++) {
				yield return source.Current;
			}
		}

		/// <summary>
		/// Does effectively the same as <code>enumerable.ToArray().CopyTo(...)</code>, but does not convert the enumerable to an array.
		/// </summary>
		public static void CopyTo<T>(this IEnumerable<T> enumerable, T[] array, int index = 0, int count = 0) {
			int i = index;
			foreach (T item in enumerable) {
				if (i - index >= count) {
					break;
				}

				array[i] = item;
				i++;
			}
		}

		/// <summary>
		/// Enumerates all duplicate items in an enumeration.
		/// If a duplicate occurs more than twice in an enumeration it will be yielded one time less than the amount of times it occurs in the source enumeration.
		/// For example, a duplicate occuring 4 times will be yielded 3 times.
		/// You can combine this with .Distinct() to avoid this.
		/// </summary>
		/// <param name="enumerable">The source enumerable.</param>
		/// <param name="equalityComparer">Uses <see cref="EqualityComparer{T}.Default"/> if null.</param>
		public static IEnumerable<T> Duplicates<T>(this IEnumerable<T> enumerable, IEqualityComparer<T>? equalityComparer = null) {
			var d = new HashSet<T>(equalityComparer ?? EqualityComparer<T>.Default);
			foreach (T t in enumerable) {
				if (!d.Add(t)) {
					yield return t;
				}
			}
		}

		/// <summary>
		/// Enumerates all duplicate items in an enumerable.
		/// If a duplicate occurs more than twice in an enumeration it will be yielded one time less than the amount of times it occurs in the source enumeration.
		/// For example, a duplicate occuring 4 times will be yielded 3 times.
		/// You can combine this with .Distinct() to avoid this.
		/// </summary>
		/// <param name="enumerable">The source enumerable.</param>
		/// <param name="equalityComparer">Uses <see cref="EqualityComparer{T}.Default"/> if null.</param>
		/// <param name="selector">Determine equality based on this selector.</param>
		public static IEnumerable<TSource> Duplicates<TSource, TValue>(this IEnumerable<TSource> enumerable, Func<TSource, TValue> selector, IEqualityComparer<TValue>? equalityComparer = null) {
			var d = new HashSet<TValue>(equalityComparer ?? EqualityComparer<TValue>.Default);
			foreach (var t in enumerable) {
				if (!d.Add(selector(t))) {
					yield return t;
				}
			}
		}

		/// <summary>
		/// - If <paramref name="key"/> exists in <paramref name="dict"/>, then the value stored at that key will be returned.
		/// - Otherwise, <paramref name="addValue"/> will be added to <paramref name="dict"/> using <paramref name="key"/>, and then returned from this function.
		/// </summary>
		public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue addValue) where TKey : notnull {
			return dict.GetOrAdd(key, _ => addValue);
		}

		/// <summary>
		/// - If <paramref name="key"/> exists in <paramref name="dict"/>, then the value stored at that key will be returned.
		/// - Otherwise, the return value of <paramref name="addFactory"/> will be added to <paramref name="dict"/> using <paramref name="key"/>, and then returned from this function.
		/// </summary>
		public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> addFactory) where TKey : notnull {
#pragma warning disable 8600
			if (dict.TryGetValue(key, out TValue value)) {
#pragma warning restore 8600
				return value;
			} else {
				TValue ret = addFactory(key);
				dict.Add(key, ret);
				return ret;
			}
		}

		/// <summary>
		/// Yields all items in the source enumeration which return a value when passed into <paramref name="selector"/>, which has not been encountered before while encountering.
		/// </summary>
		public static IEnumerable<T> DistinctBy<T, TCompare>(this IEnumerable<T> enumerable, Func<T, TCompare> selector) where TCompare : IEquatable<TCompare> {
			HashSet<TCompare> distinctTc = new HashSet<TCompare>();
			foreach (T item in enumerable) {
				if (distinctTc.Add(selector(item))) {
					yield return item;
				}
			}
		}

		/// <summary>
		/// Returns an <see cref="IReadOnlyCollection{T}"/> wrapping <paramref name="source"/>.
		/// </summary>
		public static IReadOnlyCollection<T> AsReadonly<T>(this ICollection<T> source) => new ReadOnlyCollection<T>(source);

		private class ReadOnlyCollection<T> : IReadOnlyCollection<T> {
			private readonly ICollection<T> m_Source;

			public int Count => m_Source.Count;

			public ReadOnlyCollection(ICollection<T> source) {
				m_Source = source;
			}


			public IEnumerator<T> GetEnumerator() => m_Source.GetEnumerator();
			IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)m_Source).GetEnumerator();
		}
		
		/// <summary>
		/// Returns an <see cref="IReadOnlyCollection{T}"/> wrapping <paramref name="source"/>.
		/// </summary>
		public static IReadOnlyList<T> AsReadonly<T>(this IList<T> source) => new ReadOnlyList<T>(source);

		private class ReadOnlyList<T> : IReadOnlyList<T> {
			private readonly IList<T> m_Source;

			public int Count => m_Source.Count;

			public ReadOnlyList(IList<T> source) {
				m_Source = source;
			}

			public IEnumerator<T> GetEnumerator() => m_Source.GetEnumerator();
			IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)m_Source).GetEnumerator();
			public T this[int index] => m_Source[index];
		}

		public static TSource MaxBy<TSource, TSelect>(this IEnumerable<TSource> source, Func<TSource, TSelect> selector) where TSelect : IComparable<TSelect> {
			IEnumerator<TSource> enumerator = source.GetEnumerator();
			if (!enumerator.MoveNext()) {
				throw new InvalidOperationException("Sequence is empty");
			}
			TSource candidate = enumerator.Current;
			while (enumerator.MoveNext()) {
				if (selector(candidate).CompareTo(selector(enumerator.Current)) > 0) {
					candidate = enumerator.Current;
				}
			}
			enumerator.Dispose();
			return candidate;
		}

		public static TSource MinBy<TSource, TSelect>(this IEnumerable<TSource> source, Func<TSource, TSelect> selector) where TSelect : IComparable<TSelect> {
			IEnumerator<TSource> enumerator = source.GetEnumerator();
			if (!enumerator.MoveNext()) {
				throw new InvalidOperationException("Sequence is empty");
			}
			TSource candidate = enumerator.Current;
			while (enumerator.MoveNext()) {
				if (selector(candidate).CompareTo(selector(enumerator.Current)) < 0) {
					candidate = enumerator.Current;
				}
			}
			enumerator.Dispose();
			return candidate;
		}

		public static TSource MaxBy<TSource, TSelect>(this IEnumerable<TSource> source, Func<TSource, TSelect> selector, IComparer<TSelect> comparer) {
			IEnumerator<TSource> enumerator = source.GetEnumerator();
			if (!enumerator.MoveNext()) {
				throw new InvalidOperationException("Sequence is empty");
			}
			TSource candidate = enumerator.Current;
			while (enumerator.MoveNext()) {
				if (comparer.Compare(selector(candidate), selector(enumerator.Current)) > 0) {
					candidate = enumerator.Current;
				}
			}
			enumerator.Dispose();
			return candidate;
		}

		public static TSource MinBy<TSource, TSelect>(this IEnumerable<TSource> source, Func<TSource, TSelect> selector, IComparer<TSelect> comparer) {
			IEnumerator<TSource> enumerator = source.GetEnumerator();
			if (!enumerator.MoveNext()) {
				throw new InvalidOperationException("Sequence is empty");
			}
			TSource candidate = enumerator.Current;
			while (enumerator.MoveNext()) {
				if (comparer.Compare(selector(candidate), selector(enumerator.Current)) < 0) {
					candidate = enumerator.Current;
				}
			}
			enumerator.Dispose();
			return candidate;
		}
		
		/// <summary>
		/// Yields all items in the source enumeration that are not <see langword="null"/>. This function offers null safety when using C# 8.0 nullable reference types.
		/// </summary>
		public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> enumerable) where T : class {
			// ReSharper disable once LoopCanBeConvertedToQuery
			foreach (T? item in enumerable) {
				if (!(item is null)) {
					yield return item;
				}
			}
		}

		/// <summary>
		/// Yields all items in the source enumeration that have a value. This function offers safety when using <see cref="Nullable{T}"/>
		/// </summary>
		public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> enumerable) where T : struct {
			// ReSharper disable once LoopCanBeConvertedToQuery
			foreach (T? item in enumerable) {
				if (item.HasValue) {
					yield return item.Value;
				}
			}
		}
	}
}
