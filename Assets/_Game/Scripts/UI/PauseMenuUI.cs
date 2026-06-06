using UnityEngine;
using UnityEngine.UI;
using Superdude.Core;

namespace Superdude.UI
{
    public class PauseMenuUI : MonoBehaviour
    {
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _mainMenuButton;

        private void Awake()
        {
            if (_resumeButton != null)
                _resumeButton.onClick.AddListener(() =>
                    EventBus.Publish(new PauseToggledEvent { IsPaused = false }));

            if (_mainMenuButton != null)
                _mainMenuButton.onClick.AddListener(() => {
                    EventBus.Publish(new PauseToggledEvent { IsPaused = false });
                    EventBus.Publish(new MainMenuRequestedEvent());
                });

            // Подписываемся в Awake — гарантированно до любых событий
            EventBus.Subscribe<PauseToggledEvent>(OnPauseToggled);
            EventBus.Subscribe<GameOverEvent>(OnGameOver);

            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<PauseToggledEvent>(OnPauseToggled);
            EventBus.Unsubscribe<GameOverEvent>(OnGameOver);
        }

        private void OnPauseToggled(PauseToggledEvent e) => gameObject.SetActive(e.IsPaused);
        private void OnGameOver(GameOverEvent e)          => gameObject.SetActive(false);
    }
}
