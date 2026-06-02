using System.Collections.Generic;
using UnityEngine;
using Superdude.Core;

namespace Superdude.Gameplay
{
    public class ChainManager : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Transform суперчувака — голова цепочки")]
        [SerializeField] private Transform _playerTransform;

        [Tooltip("Префаб одного звена цепочки (ChainSegment + PooledObject)")]
        [SerializeField] private GameObject _segmentPrefab;

        private readonly List<ChainSegment> _segments = new List<ChainSegment>();

        private ObjectPool _pool;
        private GameConfig _config;

        public int Length => _segments.Count;

        // ── Lifecycle ────────────────────────────────────────────────────

        private void Awake()
        {
            ServiceLocator.Register<ChainManager>(this);
        }

        private void Start()
        {
            if (_playerTransform == null)
            {
                Debug.LogError("[ChainManager] Player Transform не назначен!", this);
                return;
            }
            if (_segmentPrefab == null)
            {
                Debug.LogError("[ChainManager] Segment Prefab не назначен!", this);
                return;
            }

            _pool   = ServiceLocator.Get<ObjectPool>();
            _config = ServiceLocator.Get<GameConfig>();

            Debug.Log("[ChainManager] Инициализирован. Head: " + _playerTransform.name);
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

        public void AddSegments(int count)
        {
            for (int i = 0; i < count; i++)
                SpawnSegment();

            RefreshIndices();
            RefreshVisuals();

            EventBus.Publish(new ChainGrewEvent { NewLength = _segments.Count });
        }

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

        public int GetSegmentIndex(Transform segmentTransform)
        {
            for (int i = 0; i < _segments.Count; i++)
                if (_segments[i].transform == segmentTransform)
                    return i;
            return -1;
        }

        public bool IsSegment(Transform t) => GetSegmentIndex(t) >= 0;

        // ── Internal ─────────────────────────────────────────────────────

        private void SpawnSegment()
        {
            var spawnPos = _segments.Count > 0
                ? _segments[^1].transform.position
                : _playerTransform.position;

            var go      = _pool.Get(_segmentPrefab, spawnPos);
            var segment = go.GetComponent<ChainSegment>();

            segment.Target  = _segments.Count > 0
                ? _segments[^1].transform
                : _playerTransform;
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
                if (_segments[i] != null)
                    _segments[i].GetComponent<PooledObject>().Release();
            _segments.Clear();
        }

        private void OnGameOver(GameOverEvent e)         => ClearAll();
        private void OnGameRestarted(GameRestartedEvent e) => ClearAll();
    }
}
