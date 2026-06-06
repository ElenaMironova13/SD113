using UnityEngine;
using Superdude.Core;

namespace Superdude.Gameplay
{
    /// <summary>
    /// Обратный отсчёт раунда.
    /// Использует Time.unscaledDeltaTime — не замораживается при паузе.
    /// Только TimerSystem знает о времени — UI и другие системы
    /// подписываются на события.
    ///
    /// Публикует:
    ///   TimerTickEvent  { float Remaining } — каждую секунду
    ///   TimerEndedEvent                     — когда время вышло
    /// </summary>
    public class TimerSystem : MonoBehaviour
    {
        public float Remaining { get; private set; }

        private GameConfig       _config;
        private GameStateManager _state;
        private bool             _running;
        private float            _nextTick; // время следующего тика (в unscaled time)

        // ── Lifecycle ────────────────────────────────────────────────────

        private void Start()
        {
            _config = ServiceLocator.Get<GameConfig>();
            _state  = ServiceLocator.Get<GameStateManager>();

            ServiceLocator.Register<TimerSystem>(this);
        }

        private void OnEnable()
        {
            EventBus.Subscribe<GameStartedEvent>(OnGameStarted);
            EventBus.Subscribe<GameRestartedEvent>(OnGameStarted);
            EventBus.Subscribe<GameOverEvent>(OnGameOver);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameStartedEvent>(OnGameStarted);
            EventBus.Unsubscribe<GameRestartedEvent>(OnGameStarted);
            EventBus.Unsubscribe<GameOverEvent>(OnGameOver);
        }

        // ── Update ───────────────────────────────────────────────────────

        private void Update()
        {
            if (!_running) return;

            // Не считаем время во время паузы
            if (_state.IsState(GameState.Paused)) return;

            Remaining -= Time.deltaTime;

            // Тик каждую секунду
            if (Time.unscaledTime >= _nextTick)
            {
                _nextTick += 1f;
                EventBus.Publish(new TimerTickEvent
                {
                    Remaining = Mathf.Max(0f, Remaining)
                });
            }

            if (Remaining <= 0f)
            {
                Remaining = 0f;
                _running  = false;
                EventBus.Publish(new TimerEndedEvent());
            }
        }

        // ── Event handlers ───────────────────────────────────────────────

        private void OnGameStarted(GameStartedEvent e)   => StartTimer();
        private void OnGameStarted(GameRestartedEvent e) => StartTimer();
        private void OnGameOver(GameOverEvent e)         => _running = false;

        // ── Internal ─────────────────────────────────────────────────────

        private void StartTimer()
        {
            Remaining = _config.RoundDuration;
            _nextTick = Time.unscaledTime + 1f;
            _running  = true;

            // Публикуем начальный тик чтобы HUD сразу показал время
            EventBus.Publish(new TimerTickEvent { Remaining = Remaining });
        }
    }
}
