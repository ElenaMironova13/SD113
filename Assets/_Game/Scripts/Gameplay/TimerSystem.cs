using System.Collections;
using UnityEngine;
using Superdude.Core;

namespace Superdude.Gameplay
{
    public class TimerSystem : MonoBehaviour
    {
        public float Remaining { get; private set; }

        private GameConfig       _config;
        private GameStateManager _state;
        private bool             _running;
        private float            _nextTick;

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
            EventBus.Subscribe<MainMenuRequestedEvent>(OnMainMenu);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameStartedEvent>(OnGameStarted);
            EventBus.Unsubscribe<GameRestartedEvent>(OnGameStarted);
            EventBus.Unsubscribe<GameOverEvent>(OnGameOver);
            EventBus.Unsubscribe<MainMenuRequestedEvent>(OnMainMenu);
        }

        private void Update()
        {
            if (!_running) return;
            if (_state.IsState(GameState.Paused)) return;

            Remaining -= Time.deltaTime;

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

        private void OnGameStarted(GameStartedEvent e)    => StartTimer();
        private void OnGameStarted(GameRestartedEvent e)  => StartTimer();
        private void OnGameOver(GameOverEvent e)          => _running = false;
        private void OnMainMenu(MainMenuRequestedEvent e) => _running = false;

        private void StartTimer()
        {
            Remaining = _config.RoundDuration;
            _nextTick = Time.unscaledTime + 1f;
            _running  = true;
            EventBus.Publish(new TimerTickEvent { Remaining = Remaining });
        }
    }
}
