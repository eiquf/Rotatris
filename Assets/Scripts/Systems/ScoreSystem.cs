using UnityEngine;

namespace Rotatris.Systems
{
    /// <summary>
    /// Reads and writes player high score to PlayerPrefs using GameConfig parameters.
    /// </summary>
    public class ScoreSystem
    {
        private readonly string _bestScoreKey;

        public ScoreSystem(GameConfig config) => _bestScoreKey = config.BestScorePrefsKey;

        public int LoadBestScore() => PlayerPrefs.GetInt(_bestScoreKey, 0);

        public void SaveBestScore(int currentScore)
        {
            int currentBest = LoadBestScore();

            if (currentScore > currentBest)
            {
                PlayerPrefs.SetInt(_bestScoreKey, currentScore);
                PlayerPrefs.Save();
            }
        }

        public bool TryUpdateBestScore(int currentScore, out int newBestScore)
        {
            int currentBest = LoadBestScore();

            if (currentScore > currentBest)
            {
                PlayerPrefs.SetInt(_bestScoreKey, currentScore);
                PlayerPrefs.Save();
                newBestScore = currentScore;
                return true;
            }

            newBestScore = currentBest;
            return false;
        }
    }
}