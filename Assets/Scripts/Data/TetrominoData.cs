using System.Collections.Generic;
using UnityEngine;

namespace Rotatris.Data
{
    /// <summary>
    /// Static shape/color data for every piece. Pure data 
    /// </summary>
    public static class TetrominoData
    {
        // Base (spawn-orientation) cell offsets from a local pivot at (0,0).
        public static readonly Dictionary<PieceType, Vector2Int[]> BaseOffsets = new()
        {
            { PieceType.I, new[] { new Vector2Int(-1,0), new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(2,0) } },
            { PieceType.O, new[] { new Vector2Int(0,0),  new Vector2Int(1,0), new Vector2Int(0,1), new Vector2Int(1,1) } },
            { PieceType.T, new[] { new Vector2Int(-1,0), new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(0,1) } },
            { PieceType.S, new[] { new Vector2Int(-1,0), new Vector2Int(0,0), new Vector2Int(0,1), new Vector2Int(1,1) } },
            { PieceType.Z, new[] { new Vector2Int(-1,1), new Vector2Int(0,1), new Vector2Int(0,0), new Vector2Int(1,0) } },
            { PieceType.J, new[] { new Vector2Int(-1,1), new Vector2Int(-1,0),new Vector2Int(0,0), new Vector2Int(1,0) } },
            { PieceType.L, new[] { new Vector2Int(1,1),  new Vector2Int(-1,0),new Vector2Int(0,0), new Vector2Int(1,0) } },
        };

        public static readonly Dictionary<PieceType, Color> Colors = new()
        {
            { PieceType.I,    new Color(0.20f, 0.80f, 0.85f) },
            { PieceType.O,    new Color(0.95f, 0.85f, 0.20f) },
            { PieceType.T,    new Color(0.65f, 0.30f, 0.85f) },
            { PieceType.S,    new Color(0.35f, 0.80f, 0.35f) },
            { PieceType.Z,    new Color(0.90f, 0.25f, 0.30f) },
            { PieceType.J,    new Color(0.25f, 0.40f, 0.90f) },
            { PieceType.L,    new Color(0.95f, 0.55f, 0.15f) },
            { PieceType.Core, new Color(0.90f, 0.90f, 0.92f) },
        };

        public static readonly PieceType[] AllPieces =
            { PieceType.I, PieceType.J, PieceType.L, PieceType.O, PieceType.S, PieceType.T, PieceType.Z };

        /// <summary>Rotate a single offset 90 degrees clockwise around (0,0).</summary>
        public static Vector2Int RotateCW(Vector2Int v) => new(v.y, -v.x);

        /// <summary>Rotate a single offset 90 degrees counter-clockwise around (0,0).</summary>
        public static Vector2Int RotateCCW(Vector2Int v) => new(-v.y, v.x);

        /// <summary>Apply `turns` clockwise 90-degree rotations (negative = CCW) to a full offset set.</summary>
        public static Vector2Int[] RotateOffsets(Vector2Int[] baseOffsets, int turns)
        {
            int n = ((turns % 4) + 4) % 4;
            var result = new Vector2Int[baseOffsets.Length];
            for (int i = 0; i < baseOffsets.Length; i++)
            {
                Vector2Int v = baseOffsets[i];
                for (int t = 0; t < n; t++) v = RotateCW(v);
                result[i] = v;
            }
            return result;
        }
    }
}