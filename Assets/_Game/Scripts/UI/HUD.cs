using UnityEngine;
using UnityEngine.UI;
using Superdude.Core;

namespace Superdude.UI
{
    /// <summary>
    /// Игровой HUD — счёт и таймер.
    /// Подписывается только на EventBus, не знает ни о каких gameplay-системах.
    ///
    /// Иерархия Canvas:
    ///   HUD Canvas
    ///   ├── ScoreText   (Text/TMP)
    ///   └── TimerText   (Text/TMP)
    /// </summary>
    public class HUD : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Text _scoreText;
        [SerializeField] private Text _timerText;

        private void OnEnable()
        {
            EventBus.Subscribe<ChainGrewEvent>(OnChainGrew);
            EventBus.Subscribe<TimerTickEvent>(OnTimerTick);
            EventBus.Subscribe<GameStartedEvent>(OnGameStarted);
            EventBus.Subscribe<GameRestartedEvent>(OnGameStarted);
            EventBus.Subscribe<GameOverEvent>(OnGameOver);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<ChainGrewEvent>(OnChainGrew);
            EventBus.Unsubscribe<TimerTickEvent>(OnTimerTick);
            EventBus.Unsubscribe<GameStartedEvent>(OnGameStarted);
            EventBus.Unsubscribe<GameRestartedEvent>(OnGameStarted);
            EventBus.Unsubscribe<GameOverEvent>(OnGameOver);
        }

        private void OnGameStarted(GameStartedEvent e)
        {
            gameObject.SetActive(true);
            UpdateScore(0);
        }

        private void OnGameStarted(GameRestartedEvent e)
        {
            gameObject.SetActive(true);
            UpdateScore(0);
        }

        private void OnGameOver(GameOverEvent e)
        {
            gameObject.SetActive(false);
        }

        private void OnChainGrew(ChainGrewEvent e)
        {
            // Счёт отображаем через ScoreChangedEvent — подпишемся отдельно
        }

        private void OnTimerTick(TimerTickEvent e)
        {
            if (_timerText != null)
                _timerText.text = Mathf.CeilToInt(e.Remaining).ToString();
        }

        private void UpdateScore(int score)
        {
            if (_scoreText != null)
                _scoreText.text = score.ToString();
        }

        // Отдельная подписка на очки чтобы не тянуть ScoreSystem
        private void Awake()
        {
            EventBus.Subscribe<ScoreChangedEvent>(OnScoreChanged);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<ScoreChangedEvent>(OnScoreChanged);
        }

        private void OnScoreChanged(ScoreChangedEvent e)
        {
            // ScoreChangedEvent содержит дельту, нам нужен накопленный счёт
            // Получаем из ServiceLocator если зарегистрирован
            if (Superdude.Core.ServiceLocator.IsRegistered<Superdude.Gameplay.ScoreSystem>())
            {
                int total = Superdude.Core.ServiceLocator
                    .Get<Superdude.Gameplay.ScoreSystem>().CurrentScore;
                UpdateScore(total);
            }
        }
    }
}
