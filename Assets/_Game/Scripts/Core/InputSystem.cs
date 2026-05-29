using UnityEngine;

namespace Superdude.Core
{
    /// <summary>
    /// Читает ввод с клавиатуры и публикует события в EventBus.
    /// Не знает ни о каких системах, только публикует.
    ///
    /// Публикует:
    ///   InputDirectionEvent  — каждый кадр, пока есть нажатие (в Playing)
    ///   PauseToggledEvent    — при нажатии P (переключает паузу)
    ///
    /// Регистрируется в ServiceLocator для возможного отключения из тестов.
    /// </summary>
    public class InputSystem : MonoBehaviour
    {
        private GameStateManager _stateManager;
        private bool _isPaused;

        // Кэшируем направление — публикуем только при изменении,
        // чтобы не спамить событиями каждый кадр без движения
        private Vector2 _lastDirection = Vector2.zero;

        // ── Lifecycle ────────────────────────────────────────────────────

        private void Awake()
        {
            ServiceLocator.Register<InputSystem>(this);
        }

        private void Start()
        {
            // GameStateManager регистрируется раньше (тот же Awake-порядок),
            // поэтому Get безопасен в Start
            _stateManager = ServiceLocator.Get<GameStateManager>();
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

        // ── Update ───────────────────────────────────────────────────────

        private void Update()
        {
            ReadPauseInput();
            ReadMovementInput();
        }

        // ── Pause ────────────────────────────────────────────────────────

        private void ReadPauseInput()
        {
            // Пауза доступна только в Playing и Paused
            if (!_stateManager.IsState(GameState.Playing) &&
                !_stateManager.IsState(GameState.Paused))
                return;

            if (Input.GetKeyDown(KeyCode.P) ||
                Input.GetKeyDown(KeyCode.Escape))
            {
                _isPaused = !_isPaused;
                EventBus.Publish(new PauseToggledEvent { IsPaused = _isPaused });
            }
        }

        // ── Movement ─────────────────────────────────────────────────────

        private void ReadMovementInput()
        {
            // Движение только в Playing
            if (!_stateManager.IsState(GameState.Playing))
            {
                // Сбрасываем, чтобы при возобновлении не было "залипшего" направления
                if (_lastDirection != Vector2.zero)
                {
                    _lastDirection = Vector2.zero;
                    EventBus.Publish(new InputDirectionEvent { Direction = Vector2.zero });
                }
                return;
            }

            var dir = ReadRawDirection();

            if (dir == _lastDirection) return;

            _lastDirection = dir;
            EventBus.Publish(new InputDirectionEvent { Direction = dir });
        }

        /// <summary>
        /// Читает сырое направление с WASD и стрелок.
        /// Горизонталь приоритетнее вертикали — стандарт для snake-игр.
        /// Диагонали исключены.
        /// </summary>
        private static Vector2 ReadRawDirection()
        {
            // Горизонталь
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                return Vector2.right;
            if (Input.GetKey(KeyCode.LeftArrow)  || Input.GetKey(KeyCode.A))
                return Vector2.left;

            // Вертикаль
            if (Input.GetKey(KeyCode.UpArrow)    || Input.GetKey(KeyCode.W))
                return Vector2.up;
            if (Input.GetKey(KeyCode.DownArrow)  || Input.GetKey(KeyCode.S))
                return Vector2.down;

            return Vector2.zero;
        }

        // ── Event handlers ───────────────────────────────────────────────

        private void OnPauseToggled(PauseToggledEvent e)
        {
            _isPaused = e.IsPaused;
        }

        private void OnGameOver(GameOverEvent e)
        {
            // Блокируем ввод — GameStateManager уже выставит GameOver,
            // но сбрасываем локальный флаг паузы на чистый старт
            _isPaused    = false;
            _lastDirection = Vector2.zero;
        }
    }
}
