using System.Collections.Generic;
using UnityEngine;

namespace Rotatris.Creation
{
    /// <summary>
    /// Pools the small colored-square GameObjects used for every cell
    /// (locked structure, falling piece, ghost, next-preview, target-zone tint).
    /// </summary>
    public class CellViewPool
    {
        private readonly Sprite _sprite;
        private readonly Transform poolRoot;
        private readonly Stack<SpriteRenderer> free = new();

        public CellViewPool(Sprite _sprite, Transform poolRoot)
        {
            this._sprite = _sprite;
            this.poolRoot = poolRoot;
        }

        public SpriteRenderer Get(Transform parent)
        {
            SpriteRenderer sr = free.Count > 0 ? free.Pop() : Create();
            sr.transform.SetParent(parent, false);
            sr.gameObject.SetActive(true);
            return sr;
        }

        public void Release(SpriteRenderer sr)
        {
            if (sr == null) return;
            sr.gameObject.SetActive(false);
            sr.transform.SetParent(poolRoot, false);
            free.Push(sr);
        }

        public void ReleaseAll(List<SpriteRenderer> list)
        {
            foreach (var sr in list) Release(sr);
            list.Clear();
        }

        public void ReleaseAll(Dictionary<Vector2Int, SpriteRenderer> map)
        {
            foreach (var sr in map.Values) Release(sr);
            map.Clear();
        }

        private SpriteRenderer Create()
        {
            var go = new GameObject("cell");
            go.transform.SetParent(poolRoot, false);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = _sprite;
            go.SetActive(false);
            return sr;
        }
    }
}