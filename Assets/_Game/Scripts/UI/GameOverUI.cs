using UnityEngine;
using UnityEngine.UI;
using Superdude.Core;

namespace Superdude.UI
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] private Text   _scoreText;
        [SerializeField] private Text   _bestScoreText;
        [SerializeField] private Text   _newRecordText;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _mainMenuButton;

        private void Awake()
        {
            if (_restartButton != null)
                _restartButton.onClick.AddListener(() => {
                    gameObject.SetActive(false);
                    EventBus.Publish(new GameRestartedEvent());
                });
            if (_mainMenuButton != null)
                _mainMenuButton.onClick.AddListener(() => {
                    gameObject.SetActive(false);
                    EventBus.Publish(new MainMenuRequestedEvent());
                });

            EventBus.Subscribe<ScoreFinalizedEvent>(OnScoreFinalized);
            EventBus.Subscribe<GameStartedEvent>(OnGameStarted);
            EventBus.Subscribe<GameRestartedEvent>(OnGameRestarted);

            if (_newRecordText != null) _newRecordText.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<ScoreFinalizedEvent>(OnScoreFinalized);
            EventBus.Unsubscribe<GameStartedEvent>(OnGameStarted);
            EventBus.Unsubscribe<GameRestartedEvent>(OnGameRestarted);
        }

        private void OnScoreFinalized(ScoreFinalizedEvent e)
        {
            if (_scoreText     != null) _scoreText.text     = $"Спасено: {e.Score}";
            if (_bestScoreText != null) _bestScoreText.text = $"Рекорд: {e.BestScore}";
            if (_newRecordText != null) _newRecordText.gameObject.SetActive(e.IsNewRecord);
            gameObject.SetActive(true);
        }

        private void OnGameStarted(GameStartedEvent e)     => gameObject.SetActive(false);
        private void OnGameRestarted(GameRestartedEvent e) => gameObject.SetActive(false);
    }
}
