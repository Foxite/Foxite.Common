using System.Collections.Generic;
using System.Linq;

namespace Foxite.Common.Unity.Pathfinding {
	public class BreadthFirstPathfinder : Pathfinder<PathfindingCell> {
		/// <summary>
		/// Finds a path between two <see cref="PathfindingCell"/>s.
		/// </summary>
		/// <returns>
		/// If start == end: An empty enumeration
		/// If no path is possible: An single null item
		/// Otherwise: A lazy enumeration with the steps in the path, including <paramref name="start"/> end <paramref name="end"/>.
		/// </returns>
		public override IEnumerable<TCell> FindPath<TCell>(TCell start, TCell end) {
			if (start == end) {
				yield break;
			}

			// The PriorityQueue library has a FastPriorityQueue that has tons of optimizations but there's a big problem: you need to define the max size and it doesn't resize itself.
			// SimplyPriorityQueue isn't as fast, but still pretty fast and it has no maximum size.
			var frontier = new Queue<TCell>();
			var cameFrom = new Dictionary<TCell, TCell>();
			

			// We start searching at the end and look for the start.
			// We do it this way because when we create a path, it will be backwards.
			// So if we do this backwards to begin with, then the returned list will actually be forwards.
			frontier.Enqueue(end);
			

			// This is a local function so we can break out of the nested loop more easily
			bool traverse() {
				while (frontier.Count > 0) {
					TCell current = frontier.Dequeue();
					foreach (TCell next in current.Neighbors.Except(cameFrom.Keys)) {
						frontier.Enqueue(next);
						cameFrom[next] = current;
						if (next == start) {
							return true;
						}
					}
				}
				return false;
			}

			if (traverse()) {
				TCell current = start;
				while (current != end) {
					current = cameFrom[current];
					yield return current;
				}
			} else {
				yield return null;
			}
		}
	}
}
