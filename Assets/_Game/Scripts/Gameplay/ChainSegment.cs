using System.Collections.Generic;
using UnityEngine;
using Superdude.Core;

namespace Superdude.Gameplay
{
    /// <summary>
    /// Одно звено цепочки. Следует за предыдущим звеном (или головой)
    /// с задержкой по истории позиций — стандартный алгоритм змейки.
    ///
    /// Не знает о ChainManager — получает ссылку на цель (Target)
    /// и индекс в цепочке (SegmentIndex) снаружи.
    ///
    /// Реализует IPoolable — сбрасывает состояние при возврате в пул.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class ChainSegment : MonoBehaviour, IPoolable
    {
        // Трансформ, за которым следует это звено (предыдущее звено или голова)
        public Transform Target { get; set; }

        // Позиция этого звена в цепочке (0 = ближайшее к голове)
        public int SegmentIndex { get; set; }

        // Расстояние которое нужно поддерживать до Target
        public float Spacing { get; set; } = 0.4f;

        private SpriteRenderer _sprite;

        // ── Lifecycle ────────────────────────────────────────────────────

        private void Awake()
        {
            _sprite = GetComponent<SpriteRenderer>();
        }

        private void LateUpdate()
        {
            if (Target == null) return;

            // Двигаемся к Target только если дальше Spacing
            var delta = Target.position - transform.position;
            if (delta.magnitude > Spacing)
            {
                transform.position = Target.position - delta.normalized * Spacing;
            }
        }

        // ── IPoolable ────────────────────────────────────────────────────

        public void OnSpawn()
        {
            Target       = null;
            SegmentIndex = 0;
        }

        public void OnDespawn()
        {
            Target       = null;
            SegmentIndex = 0;
        }

        // ── Visual ───────────────────────────────────────────────────────

        /// <summary>
        /// Меняет прозрачность в зависимости от позиции в цепочке —
        /// дальние звенья чуть прозрачнее.
        /// </summary>
        public void UpdateVisual(int totalSegments)
        {
            if (_sprite == null) return;
            float alpha = Mathf.Lerp(0.5f, 1f,
                1f - (float)SegmentIndex / Mathf.Max(1, totalSegments));
            var c = _sprite.color;
            _sprite.color = new Color(c.r, c.g, c.b, alpha);
        }
    }
}
