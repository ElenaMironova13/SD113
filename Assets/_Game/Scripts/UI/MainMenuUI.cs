using UnityEngine;
using UnityEngine.UI;
using Superdude.Core;
using Superdude.Gameplay;

namespace Superdude.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private Text   _bestScoreText;

        private void Awake()
        {
            if (_startButton != null)
                _startButton.onClick.AddListener(() => EventBus.Publish(new GameStartedEvent()));

            EventBus.Subscribe<GameStartedEvent>(OnGameStarted);
            EventBus.Subscribe<MainMenuRequestedEvent>(OnMainMenuRequested);

            ShowBestScore();
            // MainMenu активен по умолчанию — не скрываем в Awake
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<GameStartedEvent>(OnGameStarted);
            EventBus.Unsubscribe<MainMenuRequestedEvent>(OnMainMenuRequested);
        }

        private void OnGameStarted(GameStartedEvent e) => gameObject.SetActive(false);

        private void OnMainMenuRequested(MainMenuRequestedEvent e)
        {
            ShowBestScore();
            gameObject.SetActive(true);
        }

        private void ShowBestScore()
        {
            if (_bestScoreText == null) return;
            if (!ServiceLocator.IsRegistered<SaveSystem>()) return;
            int best = ServiceLocator.Get<SaveSystem>().BestScore;
            _bestScoreText.text = best > 0 ? $"Рекорд: {best}" : "";
        }
    }
}
