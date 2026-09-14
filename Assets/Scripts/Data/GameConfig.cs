using UnityEngine;

namespace Rotatris
{
    /// <summary>
    /// Centralized ScriptableObject for game balance and configuration.
    /// Create an instance via Assets -> Create -> Rotatris -> Game Config.
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Rotatris/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Board Settings")]
        [Tooltip("X range: -HalfWidth to +HalfWidth (Total width = HalfWidth * 2 + 1)")]
        [SerializeField] private int halfWidth = 7;
        [SerializeField] private int height = 18;
        [SerializeField] private int spawnX = 0;

        [Header("Target Zone Settings")]
        [Tooltip("Size of the square target zone centered around the Core")]
        [SerializeField] private int targetZoneSize = 5;

        [Header("Gameplay Balance")]
        [SerializeField] private float baseFallInterval = 0.8f;
        [SerializeField] private float softDropInterval = 0.06f;
        [SerializeField] private int scorePerPiece = 10;
        [SerializeField] private string bestScorePrefsKey = "Rotatris_BestScore";

        [Header("Visuals")]
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private float cellPadding = 0.06f;

        // ---------------------------------------------------------
        // BOARD & SPAWN PROPERTIES
        // ---------------------------------------------------------

        public int HalfWidth => halfWidth;
        public int Height => height;
        public int SpawnX => spawnX;
        public int SpawnY => height - 2;

        // ---------------------------------------------------------
        // CORE & TARGET ZONE PROPERTIES
        // ---------------------------------------------------------

        public int CoreY => height / 2;
        public int TargetZoneSize => targetZoneSize;
        public int TargetZoneHalfWidth => targetZoneSize / 2;
        public int TargetZoneMinX => -TargetZoneHalfWidth;
        public int TargetZoneMaxX => TargetZoneHalfWidth;
        public int TargetZoneMinY => CoreY - TargetZoneHalfWidth;
        public int TargetZoneMaxY => CoreY + TargetZoneHalfWidth;

        // ---------------------------------------------------------
        // GAMEPLAY PROPERTIES
        // ---------------------------------------------------------

        public float BaseFallInterval => baseFallInterval;
        public float SoftDropInterval => softDropInterval;
        public int ScorePerPiece => scorePerPiece;
        public string BestScorePrefsKey => bestScorePrefsKey;

        // ---------------------------------------------------------
        // VISUAL PROPERTIES
        // ---------------------------------------------------------

        public float CellSize => cellSize;
        public float CellPadding => cellPadding;
    }
}