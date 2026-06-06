using UnityEngine;

namespace Superdude.Core
{
    /// <summary>
    /// Конечный автомат состояний игры.
    /// Регистрируется в ServiceLocator. Подписывается на EventBus.
    /// Все переходы между состояниями только через него.
    ///
    /// Диаграмма переходов:
    ///   None ──► MainMenu ──► Playing ──► Paused ──► Playing
    ///                                  └──────────► GameOver ──► MainMenu
    ///                                                           └──► Playing (restart)
    /// </summary>
    public class GameStateManager : MonoBehaviour
    {
        public GameState Current { get; private set; } = GameState.None;

        // ── Lifecycle ────────────────────────────────────────────────────

        private void Awake()
        {
            ServiceLocator.Register<GameStateManager>(this);
        }

        private void OnEnable()
        {
            EventBus.Subscribe<GameStartedEvent>(OnGameStarted);
            EventBus.Subscribe<GameRestartedEvent>(OnGameRestarted);
            EventBus.Subscribe<PauseToggledEvent>(OnPauseToggled);
            EventBus.Subscribe<TimerEndedEvent>(OnTimerEnded);
            EventBus.Subscribe<GameOverEvent>(OnGameOver);
            EventBus.Subscribe<MainMenuRequestedEvent>(OnMainMenuRequested);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameStartedEvent>(OnGameStarted);
            EventBus.Unsubscribe<GameRestartedEvent>(OnGameRestarted);
            EventBus.Unsubscribe<PauseToggledEvent>(OnPauseToggled);
            EventBus.Unsubscribe<TimerEndedEvent>(OnTimerEnded);
            EventBus.Unsubscribe<GameOverEvent>(OnGameOver);
            EventBus.Unsubscribe<MainMenuRequestedEvent>(OnMainMenuRequested);
        }

        private void Start()
        {
            TransitionTo(GameState.MainMenu);
        }

        // ── Public API ───────────────────────────────────────────────────

        /// <summary>
        /// Проверяет, разрешён ли переход в целевое состояние из текущего.
        /// Используется системами для защитных проверок.
        /// </summary>
        public bool IsState(GameState state) => Current == state;

        // ── Event handlers ───────────────────────────────────────────────

        private void OnGameStarted(GameStartedEvent e)
        {
            if (Current == GameState.MainMenu)
                TransitionTo(GameState.Playing);
        }

        private void OnGameRestarted(GameRestartedEvent e)
        {
            if (Current == GameState.GameOver)
            {
                SceneLoader.LoadGame();
            }
        }

        private void OnPauseToggled(PauseToggledEvent e)
        {
            if (e.IsPaused && Current == GameState.Playing)
                TransitionTo(GameState.Paused);
            else if (!e.IsPaused && Current == GameState.Paused)
                TransitionTo(GameState.Playing);
        }

        private void OnTimerEnded(TimerEndedEvent e)
        {
            if (Current == GameState.Playing)
                TransitionTo(GameState.GameOver);
        }

        private void OnGameOver(GameOverEvent e)
        {
            if (Current == GameState.Playing)
                TransitionTo(GameState.GameOver);
        }

        private void OnMainMenuRequested(MainMenuRequestedEvent e)
        {
            TransitionTo(GameState.MainMenu);
            // Не перезагружаем сцену — UI сам покажет MainMenu через событие
        }

        // ── FSM core ─────────────────────────────────────────────────────

        private void TransitionTo(GameState next)
        {
            if (Current == next) return;

            Debug.Log($"[GameState] {Current} → {next}");

            Current = next;

            ApplySideEffects(next);
        }

        /// <summary>
        /// Побочные эффекты при входе в состояние:
        /// пауза физики, Time.timeScale и т.д.
        /// </summary>
        private void ApplySideEffects(GameState state)
        {
            switch (state)
            {
                case GameState.Playing:
                    Time.timeScale = 1f;
                    break;

                case GameState.Paused:
                    Time.timeScale = 0f;
                    break;

                case GameState.GameOver:
                    Time.timeScale = 0f;
                    EventBus.Publish(new GameOverEvent());
                    break;

                case GameState.MainMenu:
                    Time.timeScale = 1f;
                    break;
            }
        }
    }
}
