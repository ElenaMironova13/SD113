using System.Collections.Generic;
using UnityEngine;
using Superdude.Core;

namespace Superdude.Gameplay
{
    /// <summary>
    /// Управляет цепочкой звеньев (змейкой).
    /// Голова цепи — Transform игрока, задаётся через Initialize().
    ///
    /// Публикует:
    ///   ChainGrewEvent   — при добавлении звеньев
    ///   ChainBrokenEvent — при разрыве цепочки
    ///
    /// Регистрируется в ServiceLocator — Passenger и Obstacle
    /// обращаются к нему при коллизии (шаг 8).
    /// </summary>
    public class ChainManager : MonoBehaviour
    {
        // Список звеньев: [0] = ближайшее к голове, [^1] = хвост
        private readonly List<ChainSegment> _segments = new List<ChainSegment>();

        private Transform   _head;           // Transform игрока
        private ObjectPool  _pool;
        private GameConfig  _config;
        private GameObject  _segmentPrefab;

        public int Length => _segments.Count;

        // ── Lifecycle ────────────────────────────────────────────────────

        private void Awake()
        {
            ServiceLocator.Register<ChainManager>(this);
        }

        private void OnEnable()
        {
            EventBus.Subscribe<GameOverEvent>(OnGameOver);
            EventBus.Subscribe<GameRestartedEvent>(OnGameRestarted);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameOverEvent>(OnGameOver);
            EventBus.Unsubscribe<GameRestartedEvent>(OnGameRestarted);
        }

        // ── Public API ───────────────────────────────────────────────────

        /// <summary>
        /// Вызывается из сцены (или Bootstrap) после того как
        /// Player и ObjectPool готовы.
        /// </summary>
        public void Initialize(Transform head, GameObject segmentPrefab)
        {
            _head          = head;
            _segmentPrefab = segmentPrefab;
            _pool          = ServiceLocator.Get<ObjectPool>();
            _config        = ServiceLocator.Get<GameConfig>();
        }

        /// <summary>
        /// Добавляет N звеньев в конец цепочки.
        /// Вызывается Passenger при присоединении.
        /// </summary>
        public void AddSegments(int count)
        {
            for (int i = 0; i < count; i++)
                SpawnSegment();

            RefreshIndices();
            RefreshVisuals();

            EventBus.Publish(new ChainGrewEvent { NewLength = _segments.Count });
        }

        /// <summary>
        /// Разрывает цепочку начиная с breakIndex (включительно).
        /// Все звенья с breakIndex до конца возвращаются в пул.
        /// Вызывается Obstacle при столкновении.
        /// </summary>
        public void BreakAt(int breakIndex)
        {
            if (breakIndex < 0 || breakIndex >= _segments.Count) return;

            int lost = _segments.Count - breakIndex;

            for (int i = _segments.Count - 1; i >= breakIndex; i--)
            {
                _segments[i].GetComponent<PooledObject>().Release();
                _segments.RemoveAt(i);
            }

            RefreshIndices();
            RefreshVisuals();

            EventBus.Publish(new ChainBrokenEvent
            {
                BreakIndex   = breakIndex,
                LostSegments = lost
            });
        }

        /// <summary>
        /// Возвращает индекс звена по его Transform.
        /// Используется CollisionHandler для определения точки разрыва.
        /// </summary>
        public int GetSegmentIndex(Transform segmentTransform)
        {
            for (int i = 0; i < _segments.Count; i++)
            {
                if (_segments[i].transform == segmentTransform)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Проверяет, является ли Transform звеном этой цепочки.
        /// </summary>
        public bool IsSegment(Transform t)
        {
            return GetSegmentIndex(t) >= 0;
        }

        // ── Internal ─────────────────────────────────────────────────────

        private void SpawnSegment()
        {
            // Спавним у хвоста (или у головы если цепочка пустая)
            var spawnPos = _segments.Count > 0
                ? _segments[^1].transform.position
                : _head.position;

            var go      = _pool.Get(_segmentPrefab, spawnPos);
            var segment = go.GetComponent<ChainSegment>();

            // Новое звено следует за хвостом или за головой
            segment.Target  = _segments.Count > 0
                ? _segments[^1].transform
                : _head;
            segment.Spacing = _config.SegmentSpacing;

            _segments.Add(segment);
        }

        private void RefreshIndices()
        {
            for (int i = 0; i < _segments.Count; i++)
                _segments[i].SegmentIndex = i;
        }

        private void RefreshVisuals()
        {
            foreach (var seg in _segments)
                seg.UpdateVisual(_segments.Count);
        }

        private void ClearAll()
        {
            for (int i = _segments.Count - 1; i >= 0; i--)
            {
                if (_segments[i] != null)
                    _segments[i].GetComponent<PooledObject>().Release();
            }
            _segments.Clear();
        }

        // ── Event handlers ───────────────────────────────────────────────

        private void OnGameOver(GameOverEvent e)    => ClearAll();
        private void OnGameRestarted(GameRestartedEvent e) => ClearAll();
    }
}
