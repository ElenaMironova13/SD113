using UnityEngine;
using UnityEngine.UI;
using Superdude.Core;

namespace Superdude.UI
{
    /// <summary>
    /// Экран окончания игры.
    /// Показывает итоговый счёт, рекорд и кнопки Restart / MainMenu.
    ///
    /// Иерархия Canvas:
    ///   GameOver Canvas
    ///   ├── ScoreText
    ///   ├── BestScoreText
    ///   ├── NewRecordText   (показывается только при новом рекорде)
    ///   ├── RestartButton
    ///   └── MainMenuButton
    /// </summary>
    public class GameOverUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Text   _scoreText;
        [SerializeField] private Text   _bestScoreText;
        [SerializeField] private Text   _newRecordText;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _mainMenuButton;

        private void Awake()
        {
            if (_restartButton != null)
                _restartButton.onClick.AddListener(OnRestartClicked);

            if (_mainMenuButton != null)
                _mainMenuButton.onClick.AddListener(OnMainMenuClicked);

            if (_newRecordText != null)
                _newRecordText.gameObject.SetActive(false);

            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            EventBus.Subscribe<ScoreFinalizedEvent>(OnScoreFinalized);
            EventBus.Subscribe<GameStartedEvent>(OnGameStarted);
            EventBus.Subscribe<GameRestartedEvent>(OnGameStarted);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<ScoreFinalizedEvent>(OnScoreFinalized);
            EventBus.Unsubscribe<GameStartedEvent>(OnGameStarted);
            EventBus.Unsubscribe<GameRestartedEvent>(OnGameStarted);
        }

        // ── Button handlers ──────────────────────────────────────────────

        private void OnRestartClicked()
        {
            gameObject.SetActive(false);
            EventBus.Publish(new GameRestartedEvent());
        }

        private void OnMainMenuClicked()
        {
            gameObject.SetActive(false);
            EventBus.Publish(new MainMenuRequestedEvent());
        }

        // ── Event handlers ───────────────────────────────────────────────

        private void OnScoreFinalized(ScoreFinalizedEvent e)
        {
            if (_scoreText != null)
                _scoreText.text = $"Спасено: {e.Score}";

            if (_bestScoreText != null)
                _bestScoreText.text = $"Рекорд: {e.BestScore}";

            if (_newRecordText != null)
                _newRecordText.gameObject.SetActive(e.IsNewRecord);

            gameObject.SetActive(true);
        }

        private void OnGameStarted(GameStartedEvent e)   => gameObject.SetActive(false);
        private void OnGameStarted(GameRestartedEvent e) => gameObject.SetActive(false);
    }
}
