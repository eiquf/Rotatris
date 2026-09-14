using Rotatris.Creation;
using Rotatris.InputHandling;
using Rotatris.Presentation;
using Rotatris.Systems;
using UnityEngine;

namespace Rotatris.Bootstrap
{
    /// <summary>
    /// The only entry point for the prototype.
    ///
    /// Controls:
    ///   A / Left Arrow   - move left
    ///   D / Right Arrow  - move right
    ///   S / Down Arrow   - soft drop (hold)
    ///   Space            - hard drop
    ///   W / X / Up Arrow - rotate clockwise
    ///   Z                - rotate counter-clockwise
    ///   C                - hold / swap piece
    ///   P / Escape       - pause
    ///   R                - restart after game over
    /// </summary>
    public class GameBootstrapper : MonoBehaviour
    {
        [SerializeField] private GameConfig _config;

        private ScoreSystem _scoreSystem;
        private GameFlowController _flow;

        private Camera _cam;

        private Vector2 PanelOrigin => new(_config.HalfWidth + 2.5f, _config.Height - 2f);

        private void Awake()
        {
            SetupCamera();

            var cellPool = new CellViewPool(SquareSpriteFactory.Build(), CreateRoot("CellPool"));

            var boardRenderer = CreateRoot("BoardRenderer").gameObject.AddComponent<BoardRenderer>();
            boardRenderer.Init(cellPool, _config);

            var ui = CreateRoot("UI").gameObject.AddComponent<UIManager>();
            ui.Init(_cam);

            _scoreSystem = new ScoreSystem(_config);

            _flow = new GameFlowController(new UnityInputProvider(), _config, _scoreSystem);
            Wire(_flow, boardRenderer, ui);

            _flow.StartNewGame();
        }

        private void Update() => _flow.Tick(Time.deltaTime);

        private Transform CreateRoot(string name)
        {
            var t = new GameObject(name).transform;
            t.SetParent(transform);
            return t;
        }

        private void SetupCamera()
        {
            var camGO = Camera.main != null ? Camera.main.gameObject : new GameObject("Main Camera");
            if (camGO.GetComponent<Camera>() == null) camGO.AddComponent<Camera>();
            _cam = camGO.GetComponent<Camera>();
            _cam.orthographic = true;
            _cam.orthographicSize = _config.Height / 2f + 1.5f;
            _cam.backgroundColor = new Color(0.07f, 0.08f, 0.09f);

            camGO.transform.position = new Vector3(3.5f, _config.Height / 2f, -10f);
        }

        /// <summary>All coupling between game logic and presentation lives here, and only here.</summary>
        private void Wire(GameFlowController _flow, BoardRenderer renderer, UIManager ui)
        {
            _flow.BoardReset += () =>
            {
                renderer.ResetBoard();
                renderer.DrawTargetZoneHighlight();
            };
            _flow.StructureChanged += renderer.SyncStructure;
            _flow.PieceChanged += renderer.DrawPiece;
            _flow.GhostChanged += renderer.DrawGhost;
            _flow.NextQueueChanged += types => renderer.DrawNextPreview(types, PanelOrigin + new Vector2(0, -1f));
            _flow.ScoreChanged += ui.SetScore;
            _flow.BestChanged += ui.SetBest;
            _flow.HeldChanged += ui.SetHold;
            _flow.StatusChanged += status =>
            {
                if (string.IsNullOrEmpty(status)) ui.HideStatus();
                else ui.ShowStatus(status);
            };

            _flow.StatusChanged += status =>
            {
                if (string.IsNullOrEmpty(status))
                    ui.HideStatus();
                else if (status == "_paused")
                    ui.ShowStatus("PAUSED" + "Press P or ESC to continue");
                else
                    ui.ShowStatus("GAME OVER" + $"{status}\n\nPress R to Restart");
            };
        }
    }
}