using Rotatris.Data;
using Rotatris.InputHandling;
using System;
using UnityEngine;

namespace Rotatris.Systems
{
    /// <summary>
    /// owns the structure state, the active/_held pieces, timers, _score, and game-over rules.
    /// </summary>
    public class GameFlowController
    {
        private readonly IInputProvider input;

        private readonly GameConfig _config;

        public StructureState Grid { get; private set; }
        public FallingPieceState Active { get; private set; }

        private PieceQueue _pieceQueue;
        private PieceType? _held;
        private bool _holdUsedThisPiece;

        private ScoreSystem _scoreSystem;

        private float _fallTimer;
        private bool _paused;
        private bool _gameOver;

        private int _score;
        private int _best;

        public event Action BoardReset;
        public event Action<StructureState> StructureChanged;
        public event Action<FallingPieceState> PieceChanged;
        public event Action<FallingPieceState> GhostChanged;
        public event Action<PieceType[]> NextQueueChanged;
        public event Action<int> ScoreChanged;
        public event Action<int> BestChanged;
        public event Action<PieceType?> HeldChanged;
        public event Action<string> StatusChanged;

        public GameFlowController(IInputProvider input, GameConfig config, ScoreSystem scoreSystem)
        {
            this.input = input;
            _config = config;

            _best = scoreSystem.LoadBestScore();

            _scoreSystem = scoreSystem;
        }

        public void StartNewGame()
        {
            Grid = new StructureState(_config.HalfWidth, _config.Height);
            _pieceQueue = new();
            _held = null;
            _holdUsedThisPiece = false;
            _score = 0;
            _gameOver = false;
            _paused = false;
            _fallTimer = 0f;

            BoardReset?.Invoke();
            StructureChanged?.Invoke(Grid);
            ScoreChanged?.Invoke(_score);
            BestChanged?.Invoke(_best);
            HeldChanged?.Invoke(_held);
            StatusChanged?.Invoke(null);

            SpawnNext();
        }

        public void Tick(float deltaTime)
        {
            if (_gameOver)
            {
                if (input.RestartPressed()) StartNewGame();
                return;
            }

            if (input.PausePressed())
            {
                _paused = !_paused;
                StatusChanged?.Invoke(_paused ? "_paused" : null);
            }
            if (_paused) return;

            HandleInput();

            float interval = input.SoftDropHeld() ? _config.SoftDropInterval : _config.BaseFallInterval;
            _fallTimer += deltaTime;
            if (_fallTimer >= interval)
            {
                _fallTimer = 0f;
                StepGravity();
            }

            if (_gameOver) return;

            PieceChanged?.Invoke(Active);
            GhostChanged?.Invoke(GhostSystem.Compute(Grid, Active, _config));
        }

        private void HandleInput()
        {
            if (input.MoveLeftPressed()) PieceMovementSystem.TryMoveHorizontal(Grid, Active, -1);
            if (input.MoveRightPressed()) PieceMovementSystem.TryMoveHorizontal(Grid, Active, 1);

            if (input.RotateCwPressed()) PieceMovementSystem.TryRotate(Grid, Active, 1);
            if (input.RotateCcwPressed()) PieceMovementSystem.TryRotate(Grid, Active, -1);

            if (input.HardDropPressed()) HardDrop();
            if (input.HoldPressed()) HoldSwap();
        }

        private void StepGravity()
        {
            if (PieceMovementSystem.TryStepDown(Grid, Active, _config.TargetZoneMinY)) return;
            LockActivePiece();
        }

        private void HardDrop()
        {
            while (PieceMovementSystem.TryStepDown(Grid, Active, _config.TargetZoneMinY)) { }
            LockActivePiece();
        }

        private void SpawnNext() => SpawnPiece(_pieceQueue.Dequeue());

        private void SpawnPiece(PieceType type)
        {
            Active = new FallingPieceState(type, new Vector2Int(_config.SpawnX, _config.SpawnY));
            _holdUsedThisPiece = false;

            if (!StructureQuerySystem.CellsAreFree(Grid, Active.GetCells()))
            {
                EndGame("BLOCKED OUT");
                return;
            }

            NextQueueChanged?.Invoke(_pieceQueue.PeekNext());
        }

        private void LockActivePiece()
        {
            PieceLockSystem.Lock(Grid, Active);
            PieceLockSystem.RotateAroundCore90(Grid);
            StructureChanged?.Invoke(Grid);

            if (StructureQuerySystem.HasSealedHoleInTargetZone(Grid))
            {
                EndGame("GAP SEALED IN TARGET ZONE");
                return;
            }

            _score += _config.ScorePerPiece;
            ScoreChanged?.Invoke(_score);

            if (_score > _best)
            {
                _best = _score;
                _scoreSystem.SaveBestScore(_best);
                BestChanged?.Invoke(_best);
            }

            SpawnNext();
        }

        private void HoldSwap()
        {
            if (_holdUsedThisPiece) return;

            var currentType = Active.type;
            if (_held.HasValue) SpawnPiece(_held.Value);
            else SpawnPiece(_pieceQueue.Dequeue());

            _held = currentType;
            HeldChanged?.Invoke(_held);
            _holdUsedThisPiece = true;
        }

        private void EndGame(string reason)
        {
            _gameOver = true;
            StatusChanged?.Invoke($"GAME OVER ({reason})\n_score {_score}   _best {_best}\nPress R to restart");
        }
    }
}