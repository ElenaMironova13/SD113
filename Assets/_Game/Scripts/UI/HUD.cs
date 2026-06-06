using UnityEngine;
using UnityEngine.UI;
using Superdude.Core;
using Superdude.Gameplay;

namespace Superdude.UI
{
    public class HUD : MonoBehaviour
    {
        [SerializeField] private Text _scoreText;
        [SerializeField] private Text _timerText;

        private void Awake()
        {
            EventBus.Subscribe<TimerTickEvent>(OnTimerTick);
            EventBus.Subscribe<ScoreChangedEvent>(OnScoreChanged);
            EventBus.Subscribe<GameStartedEvent>(OnGameStarted);
            EventBus.Subscribe<GameRestartedEvent>(OnGameRestarted);
            EventBus.Subscribe<GameOverEvent>(OnGameOver);
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<TimerTickEvent>(OnTimerTick);
            EventBus.Unsubscribe<ScoreChangedEvent>(OnScoreChanged);
            EventBus.Unsubscribe<GameStartedEvent>(OnGameStarted);
            EventBus.Unsubscribe<GameRestartedEvent>(OnGameRestarted);
            EventBus.Unsubscribe<GameOverEvent>(OnGameOver);
        }

        private void OnGameStarted(GameStartedEvent e)    { gameObject.SetActive(true); SetScore(0); }
        private void OnGameRestarted(GameRestartedEvent e) { gameObject.SetActive(true); SetScore(0); }
        private void OnGameOver(GameOverEvent e)           => gameObject.SetActive(false);

        private void OnTimerTick(TimerTickEvent e)
        {
            if (_timerText != null)
                _timerText.text = Mathf.CeilToInt(e.Remaining).ToString();
        }

        private void OnScoreChanged(ScoreChangedEvent e)
        {
            if (!ServiceLocator.IsRegistered<ScoreSystem>()) return;
            SetScore(ServiceLocator.Get<ScoreSystem>().CurrentScore);
        }

        private void SetScore(int score)
        {
            if (_scoreText != null) _scoreText.text = score.ToString();
        }
    }
}
