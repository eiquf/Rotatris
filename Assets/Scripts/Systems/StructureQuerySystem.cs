using System.Collections.Generic;
using UnityEngine;
using Rotatris.Data;

namespace Rotatris.Systems
{
    /// <summary>
    /// Pure, stateless read-only queries over a <see cref="StructureState"/>.
    /// No method here mutates anything - safe to call from rendering,
    /// ghost preview, or gameplay code alike.
    /// </summary>
    public static class StructureQuerySystem
    {
        public static bool IsOccupied(StructureState s, Vector2Int p) => s.Cells.ContainsKey(p);

        public static bool InBounds(StructureState s, Vector2Int p) =>
            p.x >= -s.HalfWidth && p.x <= s.HalfWidth && p.y >= 0 && p.y < s.Height;

        public static bool CellsAreFree(StructureState s, IEnumerable<Vector2Int> points)
        {
            foreach (var p in points)
            {
                if (!InBounds(s, p)) return false;
                if (IsOccupied(s, p)) return false;
            }
            return true;
        }

        /// <summary>
        /// сhecks if the core is closed
        /// </summary>
        public static bool IsCoreFullyClosed(StructureState s)
        {
            Vector2Int core = s.CorePosition;
            const int targetHalfSize = 2;

            for (int x = core.x - targetHalfSize; x <= core.x + targetHalfSize; x++)
                for (int y = core.y - targetHalfSize; y <= core.y + targetHalfSize; y++)
                    if (!IsOccupied(s, new Vector2Int(x, y)))
                        return false;

            return true;
        }

        /// <summary>rule 1: if the core is not closed, element must cross the x zone</summary>
        public static bool IsValidCoreAlignment(StructureState s, Vector2Int[] cells)
        {
            if (IsCoreFullyClosed(s)) return true;

            int minX = s.CorePosition.x - 2;
            int maxX = s.CorePosition.x + 2;

            foreach (var cell in cells)
                if (cell.x >= minX && cell.x <= maxX)
                    return true;

            return false;
        }

        /// <summary>rule 2: if the element touches the side of another one</summary>
        public static bool HasAdjacentNeighbor(StructureState s, Vector2Int[] cells)
        {
            Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

            foreach (var cell in cells)
                foreach (var dir in directions)
                    if (IsOccupied(s, cell + dir))
                        return true;

            return false;
        }

        public static bool HasSealedHoleInTargetZone(StructureState s)
        {
            var reachable = new HashSet<Vector2Int>();
            var queue = new Queue<Vector2Int>();
            int topY = s.Height - 1;

            for (int x = -s.HalfWidth; x <= s.HalfWidth; x++)
            {
                var p = new Vector2Int(x, topY);
                if (!IsOccupied(s, p) && reachable.Add(p))
                    queue.Enqueue(p);
            }

            Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                foreach (var direction in directions)
                {
                    var next = current + direction;
                    if (!InBounds(s, next) || IsOccupied(s, next)) continue;
                    if (reachable.Add(next)) queue.Enqueue(next);
                }
            }

            const int targetHalfSize = 2;
            Vector2Int core = s.CorePosition;

            for (int x = core.x - targetHalfSize; x <= core.x + targetHalfSize; x++)
            {
                for (int y = core.y - targetHalfSize; y <= core.y + targetHalfSize; y++)
                {
                    var p = new Vector2Int(x, y);
                    if (p == core) continue;
                    if (!IsOccupied(s, p) && !reachable.Contains(p))
                        return true;
                }
            }

            return false;
        }
    }
}