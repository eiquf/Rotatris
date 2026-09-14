using Rotatris.Data;
using UnityEngine;

namespace Rotatris.Systems
{
    /// <summary>
    /// Mutates a <see cref="FallingPieceState"/> in response to player/gravity
    /// input, validating every move against <see cref="StructureQuerySystem"/>
    /// first. Contains no rendering or Unity lifecycle code - GameConfig is
    /// the only Unity-adjacent thing it touches, purely for numeric limits.
    /// </summary>
    public static class PieceMovementSystem
    {
        public static bool TryStepDown(StructureState grid, FallingPieceState piece, int targetMinY)
        {
            var candidatePivot = piece.pivot + Vector2Int.down;
            var candidateCells = piece.GetCellsAt(candidatePivot, piece.rotation);

            foreach (var cell in candidateCells)
                if (cell.y < targetMinY)
                    return false;

            if (!StructureQuerySystem.CellsAreFree(grid, candidateCells))
                return false;

            piece.pivot = candidatePivot;
            return true;
        }

        public static bool TryMoveHorizontal(StructureState grid, FallingPieceState piece, int dx)
        {
            var candidatePivot = piece.pivot + new Vector2Int(dx, 0);
            var candidateCells = piece.GetCellsAt(candidatePivot, piece.rotation);

            if (!StructureQuerySystem.IsValidCoreAlignment(grid, candidateCells))
                return false;

            if (!StructureQuerySystem.CellsAreFree(grid, candidateCells))
                return false;

            piece.pivot = candidatePivot;
            return true;
        }

        public static bool TryRotate(StructureState grid, FallingPieceState piece, int turns)
        {
            int newRotation = piece.rotation + turns;
            var candidateCells = piece.GetCellsAt(piece.pivot, newRotation);

            if (!StructureQuerySystem.IsValidCoreAlignment(grid, candidateCells))
                return false;

            if (!StructureQuerySystem.CellsAreFree(grid, candidateCells))
                return false;

            piece.rotation = newRotation;
            return true;
        }
    }
}