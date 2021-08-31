using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Foxite.Common.Unity {
	public static class HexUtil {
		// We use evenq coordinates when storing hexes, and cube coordinates for algorithms.
		public static Vector2Int CubeToEvenq(Vector3Int cube) {
			int col = cube.x;
			int row = cube.z + (cube.x + (cube.x & 1)) / 2;
			return new Vector2Int(col, row);
		}

		public static Vector3Int EvenqToCube(Vector2Int oddr) {
			int x = oddr.x;
			int z = oddr.y - (oddr.x + (oddr.x & 1)) / 2;
			int y = -x - z;
			return new Vector3Int(x, y, z);
		}

		public static IEnumerable<Vector3Int> CubeSixDirections(Vector3Int offset) => CubeSixDirections().Select(dir => dir + offset);
		public static IEnumerable<Vector3Int> CubeSixDirections() {
			yield return new Vector3Int(1, -1, 0);
			yield return new Vector3Int(1, 0, -1);
			yield return new Vector3Int(0, 1, -1);
			yield return new Vector3Int(-1, 1, 0);
			yield return new Vector3Int(-1, 0, 1);
			yield return new Vector3Int(0, -1, 1);
		}

		public static int CubeDistance(Vector3Int a, Vector3Int b) {
			return (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z)) / 2;
		}
	}
}
