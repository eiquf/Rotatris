using System;
using System.Collections.Generic;
using Rotatris.Data;

namespace Rotatris.Systems
{
    /// <summary>
    /// Keeps enough pieces buffered to show a "next 3" preview at all times.
    /// </summary>
    public class PieceQueue
    {
        private readonly Queue<PieceType> _queue = new();
        private readonly Random _rng;
        private const int PreviewCount = 3;

        public PieceQueue(int? seed = null)
        {
            _rng = seed.HasValue ? new Random(seed.Value) : new Random();
            RefillIfNeeded();
        }

        private void RefillIfNeeded()
        {
            while (_queue.Count < PreviewCount + 1)
            {
                var bag = new List<PieceType>(TetrominoData.AllPieces);
                
                for (int i = bag.Count - 1; i > 0; i--)
                {
                    int j = _rng.Next(i + 1);
                    (bag[i], bag[j]) = (bag[j], bag[i]);
                }
                foreach (var p in bag) _queue.Enqueue(p);
            }
        }

        public PieceType Dequeue()
        {
            RefillIfNeeded();
            var next = _queue.Dequeue();
            RefillIfNeeded();
            return next;
        }

        public PieceType[] PeekNext(int count = PreviewCount)
        {
            RefillIfNeeded();
            var arr = new PieceType[count];
            int i = 0;
            foreach (var p in _queue)
            {
                if (i >= count) break;
                arr[i++] = p;
            }
            return arr;
        }
    }
}