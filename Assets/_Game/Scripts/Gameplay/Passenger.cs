using UnityEngine;
using Superdude.Core;

namespace Superdude.Gameplay
{
    /// <summary>
    /// Пассажир — падает вниз, при касании ТОЛЬКО игрока
    /// присоединяется к цепочке через ChainManager.AddSegments().
    /// Касание звеньев цепочки игнорируется — пассажир летит дальше.
    ///
    /// При выходе за нижний край — штраф (публикует PassengerMissedEvent).
    /// </summary>
    public class Passenger : FallingObject
    {
        private PassengerConfig _config;
        private ChainManager    _chain;
        private bool            _joined;

        // ── IPoolable ────────────────────────────────────────────────────

        public override void OnSpawn()
        {
            base.OnSpawn();
            _joined = false;

            if (_config == null)
                _config = ServiceLocator.Get<PassengerConfig>();
            if (_chain == null)
                _chain = ServiceLocator.Get<ChainManager>();

            FallSpeed = _config.FallSpeed;
        }

        // ── Collision ────────────────────────────────────────────────────

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (_joined || !IsActive) return;

            // Присоединяемся ТОЛЬКО при касании игрока
            if (!other.CompareTag(Tags.Player)) return;

            Join();
        }

        private void Join()
        {
            _joined  = true;
            IsActive = false;

            if (_config.JoinEffect != null)
                Instantiate(_config.JoinEffect, transform.position, Quaternion.identity);

            _chain.AddSegments(_config.SegmentsAdded);
            EventBus.Publish(new ScoreChangedEvent { Score = _config.ScoreValue });

            GetComponent<PooledObject>().Release();
        }

        // ── Miss ─────────────────────────────────────────────────────────

        protected override void OnExitBottom()
        {
            if (!_joined)
                EventBus.Publish(new PassengerMissedEvent());

            base.OnExitBottom();
        }
    }
}
