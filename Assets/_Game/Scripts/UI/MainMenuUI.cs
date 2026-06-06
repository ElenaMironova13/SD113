using UnityEngine;
using UnityEngine.UI;
using Superdude.Core;

namespace Superdude.UI
{
    /// <summary>
    /// Главное меню. Показывается при старте и после возврата из GameOver.
    /// Кнопка Start публикует GameStartedEvent — GameStateManager переходит в Playing.
    ///
    /// Иерархия Canvas:
    ///   MainMenu Canvas
    ///   ├── TitleText
    ///   └── StartButton
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button _startButton;
        [SerializeField] private Text   _bestScoreText;

        private void Awake()
        {
            if (_startButton != null)
                _startButton.onClick.AddListener(OnStartClicked);
        }

        private void OnEnable()
        {
            EventBus.Subscribe<GameStartedEvent>(OnGameStarted);
            EventBus.Subscribe<MainMenuRequestedEvent>(OnMainMenuRequested);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameStartedEvent>(OnGameStarted);
            EventBus.Unsubscribe<MainMenuRequestedEvent>(OnMainMenuRequested);
        }

        private void Start()
        {
            // Показываем рекорд при входе в меню
            ShowBestScore();
            gameObject.SetActive(true);
        }

        // ── Button handler ───────────────────────────────────────────────

        private void OnStartClicked()
        {
            EventBus.Publish(new GameStartedEvent());
        }

        // ── Event handlers ───────────────────────────────────────────────

        private void OnGameStarted(GameStartedEvent e)
        {
            gameObject.SetActive(false);
        }

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
