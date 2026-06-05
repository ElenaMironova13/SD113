using UnityEngine;
using Superdude.Core;

namespace Superdude.Gameplay
{
    /// <summary>
    /// Препятствие (чемодан) — падает вниз, при касании звена цепочки
    /// разрывает её с этого индекса до конца через ChainManager.BreakAt().
    /// При попадании в голову (игрока) — разрывает всю цепочку.
    /// </summary>
    public class Obstacle : FallingObject
    {
        private ObstacleConfig _config;
        private ChainManager   _chain;
        private bool           _hit;

        // ── IPoolable ────────────────────────────────────────────────────

        public override void OnSpawn()
        {
            base.OnSpawn();
            _hit = false;

            if (_config == null)
                _config = ServiceLocator.Get<ObstacleConfig>();
            if (_chain == null)
                _chain = ServiceLocator.Get<ChainManager>();

            FallSpeed = _config.FallSpeed;
        }

        // ── Collision ────────────────────────────────────────────────────

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (_hit || !IsActive) return;
            if (!_config.BreaksChain) return;

            bool hitPlayer = other.CompareTag(Tags.Player);
            bool hitChain  = other.gameObject.layer ==
                             LayerMask.NameToLayer(Layers.Chain);

            if (!hitPlayer && !hitChain) return;

            _hit     = true;
            IsActive = false;

            // Спаун эффекта
            if (_config.BreakEffect != null)
                Instantiate(_config.BreakEffect, transform.position, Quaternion.identity);

            if (hitPlayer)
            {
                // Попал в голову — рвём всю цепочку
                _chain.BreakAt(0);
            }
            else
            {
                // Попал в звено — рвём с этого индекса
                int idx = _chain.GetSegmentIndex(other.transform);
                if (idx >= 0)
                    _chain.BreakAt(idx);
            }

            GetComponent<PooledObject>().Release();
        }
    }
}
