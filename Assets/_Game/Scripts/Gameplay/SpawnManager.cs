using System.Collections;
using UnityEngine;
using Superdude.Core;

namespace Superdude.Gameplay
{
    /// <summary>
    /// Спаунит падающие объекты (пассажир / чемодан / постер)
    /// с интервалом из GameConfig и случайным выбором по весам.
    ///
    /// Активен только в состоянии Playing.
    /// Интервал уменьшается со временем (нарастание сложности).
    /// </summary>
    public class SpawnManager : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject _passengerPrefab;
        [SerializeField] private GameObject _suitcasePrefab;
        [SerializeField] private GameObject _posterPrefab;

        private GameConfig       _config;
        private GameStateManager _state;
        private ObjectPool       _pool;

        private Coroutine _spawnCoroutine;
        private float     _currentInterval;
        private float     _elapsedTime;

        // ── Lifecycle ────────────────────────────────────────────────────

        private void Start()
        {
            _config = ServiceLocator.Get<GameConfig>();
            _state  = ServiceLocator.Get<GameStateManager>();
            _pool   = ServiceLocator.Get<ObjectPool>();

            ValidatePrefabs();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<GameStartedEvent>(OnGameStarted);
            EventBus.Subscribe<GameRestartedEvent>(OnGameStarted);
            EventBus.Subscribe<GameOverEvent>(OnGameOver);
            EventBus.Subscribe<PassengerMissedEvent>(OnPassengerMissed);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameStartedEvent>(OnGameStarted);
            EventBus.Unsubscribe<GameRestartedEvent>(OnGameStarted);
            EventBus.Unsubscribe<GameOverEvent>(OnGameOver);
            EventBus.Unsubscribe<PassengerMissedEvent>(OnPassengerMissed);
        }

        // ── Event handlers ───────────────────────────────────────────────

        private void OnGameStarted(GameStartedEvent e)   => StartSpawning();
        private void OnGameStarted(GameRestartedEvent e) => StartSpawning();

        private void OnGameOver(GameOverEvent e)
        {
            if (_spawnCoroutine != null)
            {
                StopCoroutine(_spawnCoroutine);
                _spawnCoroutine = null;
            }
        }

        /// <summary>
        /// Штраф за пропущенного пассажира — ускоряем интервал спауна.
        /// Больше давление → игрок вынужден активнее двигаться.
        /// </summary>
        private void OnPassengerMissed(PassengerMissedEvent e)
        {
            _currentInterval = Mathf.Max(
                _config.SpawnIntervalMin,
                _currentInterval - 0.1f
            );
        }

        // ── Spawn loop ───────────────────────────────────────────────────

        private void StartSpawning()
        {
            if (_spawnCoroutine != null)
                StopCoroutine(_spawnCoroutine);

            _currentInterval = _config.SpawnInterval;
            _elapsedTime     = 0f;
            _spawnCoroutine  = StartCoroutine(SpawnLoop());
        }

        private IEnumerator SpawnLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(_currentInterval);

                if (!_state.IsState(GameState.Playing))
                {
                    yield return new WaitUntil(() => _state.IsState(GameState.Playing));
                }

                SpawnRandom();
                UpdateDifficulty();
            }
        }

        private void SpawnRandom()
        {
            var prefab = PickPrefab();
            if (prefab == null) return;

            var pos = new Vector3(RandomX(), CameraBounds.Top + 0.5f, 0f);
            _pool.Get(prefab, pos);
        }

        /// <summary>
        /// Выбирает префаб по вероятностным весам из GameConfig.
        /// </summary>
        private GameObject PickPrefab()
        {
            int total = _config.PassengerWeight
                      + _config.SuitcaseWeight
                      + _config.PosterWeight;

            int roll = Random.Range(0, total);

            if (roll < _config.PassengerWeight)
                return _passengerPrefab;

            roll -= _config.PassengerWeight;

            if (roll < _config.SuitcaseWeight)
                return _suitcasePrefab;

            return _posterPrefab;
        }

        /// <summary>
        /// Каждые 10 секунд уменьшаем интервал спауна — нарастание сложности.
        /// </summary>
        private void UpdateDifficulty()
        {
            _elapsedTime += _currentInterval;

            if (_elapsedTime >= 10f)
            {
                _elapsedTime     = 0f;
                _currentInterval = Mathf.Max(
                    _config.SpawnIntervalMin,
                    _currentInterval - _config.SpawnIntervalDecrement
                );

                Debug.Log($"[SpawnManager] Интервал спауна: {_currentInterval:F2}s");
            }
        }

        /// <summary>
        /// Случайная X-позиция в пределах SpawnWidthFraction экрана.
        /// </summary>
        private float RandomX()
        {
            float half = (CameraBounds.Right - CameraBounds.Left)
                       * _config.SpawnWidthFraction * 0.5f;
            return Random.Range(-half, half);
        }

        // ── Validation ───────────────────────────────────────────────────

        private void ValidatePrefabs()
        {
            if (_passengerPrefab == null)
                Debug.LogError("[SpawnManager] Passenger Prefab не назначен!", this);
            if (_suitcasePrefab == null)
                Debug.LogError("[SpawnManager] Suitcase Prefab не назначен!", this);
            if (_posterPrefab == null)
                Debug.LogError("[SpawnManager] Poster Prefab не назначен!", this);
        }
    }
}
