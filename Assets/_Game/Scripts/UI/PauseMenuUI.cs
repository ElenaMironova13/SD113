using UnityEngine;
using UnityEngine.UI;
using Superdude.Core;

namespace Superdude.UI
{
    /// <summary>
    /// Меню паузы. Появляется при PauseToggledEvent { IsPaused = true }.
    ///
    /// Иерархия Canvas:
    ///   PauseMenu Canvas
    ///   ├── ResumeButton
    ///   └── MainMenuButton
    /// </summary>
    public class PauseMenuUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _mainMenuButton;

        private void Awake()
        {
            if (_resumeButton != null)
                _resumeButton.onClick.AddListener(OnResumeClicked);

            if (_mainMenuButton != null)
                _mainMenuButton.onClick.AddListener(OnMainMenuClicked);

            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            EventBus.Subscribe<PauseToggledEvent>(OnPauseToggled);
            EventBus.Subscribe<GameOverEvent>(OnGameOver);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<PauseToggledEvent>(OnPauseToggled);
            EventBus.Unsubscribe<GameOverEvent>(OnGameOver);
        }

        // ── Button handlers ──────────────────────────────────────────────

        private void OnResumeClicked()
        {
            EventBus.Publish(new PauseToggledEvent { IsPaused = false });
        }

        private void OnMainMenuClicked()
        {
            // Снимаем паузу перед уходом в меню
            EventBus.Publish(new PauseToggledEvent { IsPaused = false });
            EventBus.Publish(new MainMenuRequestedEvent());
        }

        // ── Event handlers ───────────────────────────────────────────────

        private void OnPauseToggled(PauseToggledEvent e)
        {
            gameObject.SetActive(e.IsPaused);
        }

        private void OnGameOver(GameOverEvent e)
        {
            gameObject.SetActive(false);
        }
    }
}
