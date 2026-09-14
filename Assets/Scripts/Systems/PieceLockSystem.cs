using Rotatris.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Rotatris.Systems
{
    /// <summary>
    /// The two mutations that happen when a falling piece settles: baking
    /// its cells into the structure, then spinning the whole structure
    /// </summary>
    public static class PieceLockSystem
    {
        public static void Lock(StructureState grid, FallingPieceState piece)
        {
            foreach (var cell in piece.GetCells())
                grid.Cells[cell] = piece.type;
        }

        public static void RotateAroundCore90(StructureState grid)
        {
            var rotated = new Dictionary<Vector2Int, PieceType>();
            Vector2Int core = grid.CorePosition;
            rotated[core] = PieceType.Core;

            foreach (var kv in grid.Cells)
            {
                Vector2Int oldPosition = kv.Key;
                if (oldPosition == core) continue;

                Vector2Int relative = oldPosition - core;
                Vector2Int rotatedRelative = new(-relative.y, relative.x);
                Vector2Int newPosition = core + rotatedRelative;

                if (!StructureQuerySystem.InBounds(grid, newPosition))
                {
                    Debug.LogWarning($"Structure rotation would move cell {oldPosition} outside the board to {newPosition}.");
                    return;
                }

                rotated[newPosition] = kv.Value;
            }

            grid.Cells.Clear();
            foreach (var kv in rotated) grid.Cells[kv.Key] = kv.Value;
            grid.Cells[core] = PieceType.Core;
        }
    }
}