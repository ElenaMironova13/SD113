using UnityEngine;
using Superdude.Core;

namespace Superdude.Gameplay
{
    /// <summary>
    /// Сохраняет и загружает рекорд через PlayerPrefs.
    /// Регистрируется в ServiceLocator — ScoreSystem обращается напрямую.
    ///
    /// Намеренно минимален: только рекорд.
    /// Расширяется на шаге 12 если понадобится полноценный save-файл.
    /// </summary>
    public class SaveSystem : MonoBehaviour
    {
        private const string BestScoreKey = "BestScore";

        public int BestScore { get; private set; }

        // ── Lifecycle ────────────────────────────────────────────────────

        private void Awake()
        {
            ServiceLocator.Register<SaveSystem>(this);
            BestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
            Debug.Log($"[SaveSystem] Рекорд загружен: {BestScore}");
        }

        // ── Public API ───────────────────────────────────────────────────

        /// <summary>
        /// Сохраняет счёт если он лучше текущего рекорда.
        /// Возвращает true если побит рекорд, out best — актуальный рекорд.
        /// </summary>
        public bool TrySaveRecord(int score, out int best)
        {
            bool isNew = score > BestScore;

            if (isNew)
            {
                BestScore = score;
                PlayerPrefs.SetInt(BestScoreKey, BestScore);
                PlayerPrefs.Save();
                Debug.Log($"[SaveSystem] Новый рекорд: {BestScore}");
            }

            best = BestScore;
            return isNew;
        }

        /// <summary>
        /// Сброс рекорда — для тестирования.
        /// </summary>
        public void ResetRecord()
        {
            BestScore = 0;
            PlayerPrefs.DeleteKey(BestScoreKey);
            PlayerPrefs.Save();
            Debug.Log("[SaveSystem] Рекорд сброшен");
        }
    }
}
