using UnityEngine;
using Superdude.Core;

namespace Superdude.Gameplay
{
    public class ScoreSystem : MonoBehaviour
    {
        public int CurrentScore { get; private set; }

        // Получаем SaveSystem лениво при первом обращении —
        // не зависим от порядка Awake/Start компонентов
        private SaveSystem _save => ServiceLocator.Get<SaveSystem>();

        private void Awake()
        {
            ServiceLocator.Register<ScoreSystem>(this);
        }

        private void OnEnable()
        {
            EventBus.Subscribe<ScoreChangedEvent>(OnScoreChanged);
            EventBus.Subscribe<GameStartedEvent>(OnGameStarted);
            EventBus.Subscribe<GameRestartedEvent>(OnGameStarted);
            EventBus.Subscribe<GameOverEvent>(OnGameOver);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<ScoreChangedEvent>(OnScoreChanged);
            EventBus.Unsubscribe<GameStartedEvent>(OnGameStarted);
            EventBus.Unsubscribe<GameRestartedEvent>(OnGameStarted);
            EventBus.Unsubscribe<GameOverEvent>(OnGameOver);
        }

        private void OnGameStarted(GameStartedEvent e)   => CurrentScore = 0;
        private void OnGameStarted(GameRestartedEvent e) => CurrentScore = 0;

        private void OnScoreChanged(ScoreChangedEvent e)
        {
            CurrentScore += e.Score;
            Debug.Log($"[ScoreSystem] Счёт: {CurrentScore}");
        }

        private void OnGameOver(GameOverEvent e)
        {
            bool isRecord = _save.TrySaveRecord(CurrentScore, out int best);

            EventBus.Publish(new ScoreFinalizedEvent
            {
                Score       = CurrentScore,
                BestScore   = best,
                IsNewRecord = isRecord
            });
        }
    }
}
