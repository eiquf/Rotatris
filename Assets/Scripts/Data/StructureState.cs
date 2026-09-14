using System.Collections.Generic;
using UnityEngine;

namespace Rotatris.Data
{
    /// <summary>
    /// Pure data: which cells are filled with which piece type, and the
    /// board's dimensions.
    /// </summary>
    public class StructureState
    {
        public readonly Dictionary<Vector2Int, PieceType> Cells = new();

        public readonly int HalfWidth;
        public readonly int Height;

        public Vector2Int CorePosition => new(0, Height / 2);

        public StructureState(int halfWidth, int height)
        {
            HalfWidth = halfWidth;
            Height = height;
            Cells[CorePosition] = PieceType.Core;
        }
    }
}