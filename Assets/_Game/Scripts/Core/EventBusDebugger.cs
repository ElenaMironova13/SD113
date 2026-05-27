using UnityEngine;

namespace Superdude.Core
{
    /// <summary>
    /// Вспомогательный компонент для отладки EventBus в Editor.
    /// Добавьте на любой GameObject в сцене Game во время разработки.
    /// В финальном билде не влияет на производительность — только Editor-код.
    /// </summary>
    public class EventBusDebugger : MonoBehaviour
    {
#if UNITY_EDITOR
        [Header("Subscriber counts (live)")]
        [SerializeField] private int _inputDirection;
        [SerializeField] private int _pauseToggled;
        [SerializeField] private int _gameStarted;
        [SerializeField] private int _gameOver;
        [SerializeField] private int _chainGrew;
        [SerializeField] private int _chainBroken;
        [SerializeField] private int _scoreChanged;
        [SerializeField] private int _timerTick;
        [SerializeField] private int _timerEnded;
        [SerializeField] private int _speedBoost;

        private void Update()
        {
            _inputDirection = EventBus.GetSubscriberCount<InputDirectionEvent>();
            _pauseToggled   = EventBus.GetSubscriberCount<PauseToggledEvent>();
            _gameStarted    = EventBus.GetSubscriberCount<GameStartedEvent>();
            _gameOver       = EventBus.GetSubscriberCount<GameOverEvent>();
            _chainGrew      = EventBus.GetSubscriberCount<ChainGrewEvent>();
            _chainBroken    = EventBus.GetSubscriberCount<ChainBrokenEvent>();
            _scoreChanged   = EventBus.GetSubscriberCount<ScoreChangedEvent>();
            _timerTick      = EventBus.GetSubscriberCount<TimerTickEvent>();
            _timerEnded     = EventBus.GetSubscriberCount<TimerEndedEvent>();
            _speedBoost     = EventBus.GetSubscriberCount<SpeedBoostEvent>();
        }
#endif
    }
}
