using Rotatris.Creation;
using Rotatris.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Rotatris.Presentation
{
    /// <summary>
    /// Draws the board using pooled generated square sprites - no imported
    /// art required. Responsible for: locked structure cells, the active
    /// falling piece, its ghost/landing preview, and the next-pieces panel.
    /// </summary>
    public class BoardRenderer : MonoBehaviour
    {
        private GameConfig _config;
        private CellViewPool _cellPool;
        private Transform _structureRoot, _pieceRoot, _ghostRoot, _nextRoot, _zoneRoot;

        private readonly Dictionary<Vector2Int, SpriteRenderer> _structureViews = new();
        private readonly List<SpriteRenderer> _pieceViews = new();
        private readonly List<SpriteRenderer> _ghostViews = new();
        private readonly List<SpriteRenderer> _nextViews = new();
        private readonly List<SpriteRenderer> _zoneViews = new();

        public void Init(CellViewPool pool, GameConfig config)
        {
            _cellPool = pool;
            _config = config;

            _zoneRoot = new GameObject("TargetZoneHighlight").transform;
            _structureRoot = new GameObject("StructureCells").transform;
            _pieceRoot = new GameObject("FallingPiece").transform;
            _ghostRoot = new GameObject("Ghost").transform;
            _nextRoot = new GameObject("NextPreview").transform;

            _zoneRoot.SetParent(transform);
            _structureRoot.SetParent(transform);
            _pieceRoot.SetParent(transform);
            _ghostRoot.SetParent(transform);
            _nextRoot.SetParent(transform);
        }

        /// <summary>Call at the start of every new game: returns all pooled cells before the next draw.</summary>
        public void ResetBoard()
        {
            _cellPool.ReleaseAll(_structureViews);
            _cellPool.ReleaseAll(_pieceViews);
            _cellPool.ReleaseAll(_ghostViews);
            _cellPool.ReleaseAll(_nextViews);
            _cellPool.ReleaseAll(_zoneViews);
        }

        private void ApplyCell(SpriteRenderer sr, Vector2Int gridPos, Color color, float alpha, int sortOrder)
        {
            sr.transform.localPosition = new Vector3(gridPos.x * _config.CellSize, gridPos.y * _config.CellSize, 0);
            float scale = _config.CellSize - _config.CellPadding;
            sr.transform.localScale = new Vector3(scale, scale, 1f);
            var c = color; c.a = alpha;
            sr.color = c;
            sr.sortingOrder = sortOrder;
        }

        /// <summary>
        /// Draws a faint background tint behind the 5x5 target zone only - every other cell on the board is left plain, per spec.
        /// </summary>
        public void DrawTargetZoneHighlight()
        {
            _cellPool.ReleaseAll(_zoneViews);

            int halfW = _config.TargetZoneHalfWidth;
            var tint = new Color(1f, 0.95f, 0.6f, 0.12f);
            for (int x = -halfW; x <= halfW; x++)
            {
                for (int y = _config.TargetZoneMinY; y <= _config.TargetZoneMaxY; y++)
                {
                    var sr = _cellPool.Get(_zoneRoot);
                    sr.transform.localPosition = new Vector3(x * _config.CellSize, y * _config.CellSize, 0.1f);
                    sr.transform.localScale = new Vector3(_config.CellSize, _config.CellSize, 1f);
                    sr.color = tint;
                    sr.sortingOrder = -1; // behind locked structure cells (order 0) and everything else
                    _zoneViews.Add(sr);
                }
            }
        }

        /// <summary>
        /// Rebuilds every locked-structure cell view. 
        /// </summary>
        public void SyncStructure(StructureState grid)
        {
            _cellPool.ReleaseAll(_structureViews);

            foreach (var kv in grid.Cells)
            {
                var color = TetrominoData.Colors[kv.Value];
                var sr = _cellPool.Get(_structureRoot);
                ApplyCell(sr, kv.Key, color, 1f, 0);
                _structureViews[kv.Key] = sr;
            }
        }

        public void DrawPiece(FallingPieceState piece)
        {
            if (piece == null) { SetActive(_pieceViews, false); return; }
            var cells = piece.GetCells();
            EnsurePool(_pieceViews, _pieceRoot, cells.Length);
            SetActive(_pieceViews, true);
            for (int i = 0; i < cells.Length; i++)
                ApplyCell(_pieceViews[i], cells[i], TetrominoData.Colors[piece.type], 1f, 2);
        }

        public void DrawGhost(FallingPieceState ghost)
        {
            if (ghost == null) { SetActive(_ghostViews, false); return; }
            var cells = ghost.GetCells();
            EnsurePool(_ghostViews, _ghostRoot, cells.Length);
            SetActive(_ghostViews, true);
            for (int i = 0; i < cells.Length; i++)
                ApplyCell(_ghostViews[i], cells[i], TetrominoData.Colors[ghost.type], 0.28f, 1);
        }

        public void DrawNextPreview(PieceType[] nextTypes, Vector2 panelOrigin)
        {
            _cellPool.ReleaseAll(_nextViews);

            for (int slot = 0; slot < nextTypes.Length; slot++)
            {
                var offsets = TetrominoData.RotateOffsets(TetrominoData.BaseOffsets[nextTypes[slot]], 0);
                var color = TetrominoData.Colors[nextTypes[slot]];
                Vector2 slotOrigin = panelOrigin + new Vector2(0, -slot * 3.2f);

                foreach (var o in offsets)
                {
                    var sr = _cellPool.Get(_nextRoot);
                    sr.transform.localPosition = new Vector3(slotOrigin.x + o.x * 0.7f, slotOrigin.y + o.y * 0.7f, 0);
                    sr.transform.localScale = Vector3.one * 0.62f;
                    var c = color; c.a = 1f;
                    sr.color = c;
                    sr.sortingOrder = 2;
                    _nextViews.Add(sr);
                }
            }
        }

        private void EnsurePool(List<SpriteRenderer> pool, Transform parent, int count)
        {
            while (pool.Count < count)
                pool.Add(_cellPool.Get(parent));
            for (int i = 0; i < pool.Count; i++)
                pool[i].gameObject.SetActive(i < count);
        }

        private void SetActive(List<SpriteRenderer> pool, bool active)
        {
            foreach (var sr in pool) sr.gameObject.SetActive(active);
        }
    }
}