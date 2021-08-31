using System.Collections.Generic;

namespace Foxite.Common.Unity.Pathfinding {
	public abstract class Pathfinder<TCellType> where TCellType : PathfindingCell {
		public abstract IEnumerable<TCell> FindPath<TCell>(TCell start, TCell end) where TCell : TCellType;
	}
}
