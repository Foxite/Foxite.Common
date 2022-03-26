using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Foxite.Common.AsyncLinq {
	// These are in a separate namespace to avoid conflicts with EntityFramework
	public static class AsyncLinqExtensions {
		public static IAsyncEnumerable<TSelect> Select<TSource, TSelect>(this IAsyncEnumerable<TSource> source, Func<TSource, TSelect> selector) =>
			new AsyncSelectEnumerable<TSource, TSelect>(source, selector);

		private class AsyncSelectEnumerable<TSource, TSelect> : IAsyncEnumerable<TSelect> {
			private readonly IAsyncEnumerable<TSource> m_Source;
			private readonly Func<TSource, TSelect> m_Selector;

			public AsyncSelectEnumerable(IAsyncEnumerable<TSource> source, Func<TSource, TSelect> selector) {
				m_Source = source;
				m_Selector = selector;
			}

			public IAsyncEnumerator<TSelect> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
				new AsyncSelectEnumerator(m_Source, m_Selector);

			private class AsyncSelectEnumerator : IAsyncEnumerator<TSelect> {
				private readonly IAsyncEnumerator<TSource> m_Source;
				private readonly Func<TSource, TSelect> m_Selector;
				private TSelect m_Current = default!;
				private bool m_Initialized = false;
				private bool m_ReachedEnd = false;

				public AsyncSelectEnumerator(IAsyncEnumerable<TSource> source, Func<TSource, TSelect> selector) {
					m_Source = source.GetAsyncEnumerator();
					m_Selector = selector;
				}

				public TSelect Current {
					get {
						if (m_ReachedEnd) {
							throw new InvalidOperationException("The enumerator has reached the end of the enumeration.");
						} else if (m_Initialized) {
							return m_Current;
						} else {
							throw new InvalidOperationException("The enumerator has not yet been initialized.");
						}
					}
				}

				public ValueTask DisposeAsync() => m_Source.DisposeAsync();

				public async ValueTask<bool> MoveNextAsync() {
					m_Initialized = true;
					bool ret = await m_Source.MoveNextAsync();
					if (ret) {
						m_Current = m_Selector(m_Source.Current);
					} else {
						m_ReachedEnd = true;
					}
					return ret;
				}
			}
		}
	}
}
