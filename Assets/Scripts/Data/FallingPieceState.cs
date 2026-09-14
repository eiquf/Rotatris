using UnityEngine;

namespace Rotatris.Data
{
    /// <summary>
    /// Pure data + math for the piece currently under player/gravity control.
    /// </summary>
    public class FallingPieceState
    {
        public PieceType type;
        public Vector2Int pivot;
        public int rotation; // number of clockwise quarter-turns from spawn orientation

        public FallingPieceState(PieceType type, Vector2Int pivot, int rotation = 0)
        {
            this.type = type;
            this.pivot = pivot;
            this.rotation = rotation;
        }

        public FallingPieceState Clone() => new(type, pivot, rotation);

        public Vector2Int[] GetCells() => GetCellsAt(pivot, rotation);

        public Vector2Int[] GetCellsAt(Vector2Int atPivot, int atRotation)
        {
            var offsets = TetrominoData.RotateOffsets(TetrominoData.BaseOffsets[type], atRotation);
            var cells = new Vector2Int[offsets.Length];
            for (int i = 0; i < offsets.Length; i++) cells[i] = atPivot + offsets[i];
            return cells;
        }
    }
}