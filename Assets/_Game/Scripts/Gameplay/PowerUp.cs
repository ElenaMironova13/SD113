using UnityEngine;
using Superdude.Core;

namespace Superdude.Gameplay
{
    /// <summary>
    /// Усиление (постер с супергероем) — падает вниз, при касании игрока
    /// публикует SpeedBoostEvent который подхватывает PlayerController.
    /// </summary>
    public class PowerUp : FallingObject
    {
        private PowerUpConfig _config;
        private bool          _picked;

        // ── IPoolable ────────────────────────────────────────────────────

        public override void OnSpawn()
        {
            base.OnSpawn();
            _picked = false;

            if (_config == null)
                _config = ServiceLocator.Get<PowerUpConfig>();

            FallSpeed = _config.FallSpeed;
        }

        // ── Collision ────────────────────────────────────────────────────

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (_picked || !IsActive) return;
            if (!other.CompareTag(Tags.Player)) return;

            _picked  = true;
            IsActive = false;

            // Спаун эффекта
            if (_config.PickupEffect != null)
                Instantiate(_config.PickupEffect, transform.position, Quaternion.identity);

            // Публикуем буст — PlayerController подхватит
            EventBus.Publish(new SpeedBoostEvent
            {
                Multiplier = _config.EffectMultiplier,
                Duration   = _config.Duration
            });

            GetComponent<PooledObject>().Release();
        }
    }
}
