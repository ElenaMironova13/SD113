using System.Collections;
using UnityEngine;
using Superdude.Core;

namespace Superdude.Gameplay
{
    /// <summary>
    /// Мозг суперчувака. Подписывается на InputDirectionEvent и события состояния.
    /// Делегирует физику в MovementSystem, не трогает Rigidbody2D напрямую.
    ///
    /// GameObject-структура префаба Player:
    ///   Player (этот компонент + MovementSystem + Rigidbody2D + Collider2D)
    ///   └── Visual (SpriteRenderer)
    /// </summary>
    [RequireComponent(typeof(MovementSystem))]
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer _visual;

        private MovementSystem   _movement;
        private GameStateManager _stateManager;
        private GameConfig       _config;

        private Coroutine _speedBoostCoroutine;
        private bool      _isActive;

        // ── Lifecycle ────────────────────────────────────────────────────

        private void Awake()
        {
            _movement = GetComponent<MovementSystem>();
        }

        private void Start()
        {
            _stateManager = ServiceLocator.Get<GameStateManager>();
            _config       = ServiceLocator.Get<GameConfig>();

            _movement.Initialize(_config.PlayerSpeed);
        }

        private void OnEnable()
        {
            EventBus.Subscribe<InputDirectionEvent>(OnInputDirection);
            EventBus.Subscribe<GameStartedEvent>(OnGameStarted);
            EventBus.Subscribe<GameRestartedEvent>(OnGameStarted);
            EventBus.Subscribe<PauseToggledEvent>(OnPauseToggled);
            EventBus.Subscribe<GameOverEvent>(OnGameOver);
            EventBus.Subscribe<SpeedBoostEvent>(OnSpeedBoost);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<InputDirectionEvent>(OnInputDirection);
            EventBus.Unsubscribe<GameStartedEvent>(OnGameStarted);
            EventBus.Unsubscribe<GameRestartedEvent>(OnGameStarted);
            EventBus.Unsubscribe<PauseToggledEvent>(OnPauseToggled);
            EventBus.Unsubscribe<GameOverEvent>(OnGameOver);
            EventBus.Unsubscribe<SpeedBoostEvent>(OnSpeedBoost);
        }

        // ── Event handlers ───────────────────────────────────────────────

        private void OnGameStarted(GameStartedEvent e)  => SetActive(true);
        private void OnGameStarted(GameRestartedEvent e) => SetActive(true);

        private void OnPauseToggled(PauseToggledEvent e)
        {
            // При паузе останавливаем движение, но не сбрасываем направление —
            // после снятия паузы игрок продолжит двигаться в том же направлении
            _movement.Direction = e.IsPaused ? Vector2.zero : _lastDirection;
        }

        private void OnGameOver(GameOverEvent e)
        {
            SetActive(false);
            if (_speedBoostCoroutine != null)
            {
                StopCoroutine(_speedBoostCoroutine);
                _movement.ResetSpeedMultiplier();
            }
        }

        private void OnInputDirection(InputDirectionEvent e)
        {
            if (!_isActive) return;
            _lastDirection      = e.Direction;
            _movement.Direction = e.Direction;
        }

        private void OnSpeedBoost(SpeedBoostEvent e)
        {
            if (!_isActive) return;

            if (_speedBoostCoroutine != null)
                StopCoroutine(_speedBoostCoroutine);

            _speedBoostCoroutine = StartCoroutine(SpeedBoostRoutine(e));
        }

        // ── Speed Boost ──────────────────────────────────────────────────

        private IEnumerator SpeedBoostRoutine(SpeedBoostEvent e)
        {
            _movement.ApplySpeedMultiplier(e.Multiplier);

            // Визуальная индикация буста — мигание спрайта
            if (_visual != null)
                _visual.color = Color.yellow;

            yield return new WaitForSeconds(e.Duration);

            _movement.ResetSpeedMultiplier();

            if (_visual != null)
                _visual.color = Color.white;

            _speedBoostCoroutine = null;
        }

        // ── Helpers ──────────────────────────────────────────────────────

        private Vector2 _lastDirection = Vector2.zero;

        private void SetActive(bool active)
        {
            _isActive = active;
            if (!active)
                _movement.Direction = Vector2.zero;
        }
    }
}
