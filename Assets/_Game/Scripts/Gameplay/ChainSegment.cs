using UnityEngine;
using Superdude.Core;

namespace Superdude.Gameplay
{
    /// <summary>
    /// Одно звено цепочки с симуляцией верёвочной физики (Verlet Integration).
    /// Звено тянется к предыдущему звену (Target), но при этом
    /// испытывает гравитацию — провисает вниз как живая цепочка.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class ChainSegment : MonoBehaviour, IPoolable
    {
        public Transform Target       { get; set; }
        public int       SegmentIndex { get; set; }
        public float     Spacing      { get; set; } = 0.4f;

        [Header("Physics Simulation")]
        [Tooltip("Сила гравитации на звено (вниз)")]
        public float Gravity        = 9.8f;

        [Tooltip("Затухание скорости [0..1] — выше = меньше болтанки")]
        [Range(0.8f, 0.99f)]
        public float Damping        = 0.95f;

        [Tooltip("Жёсткость пружины к Target [0..1] — выше = цепочка короче провисает")]
        [Range(0.1f, 1f)]
        public float Stiffness      = 0.5f;

        private SpriteRenderer _sprite;

        // Verlet: текущая и предыдущая позиция
        private Vector2 _velocity = Vector2.zero;

        // ── Lifecycle ────────────────────────────────────────────────────

        private void Awake()
        {
            _sprite = GetComponent<SpriteRenderer>();
        }

        private void LateUpdate()
        {
            if (Target == null) return;

            // 1. Применяем гравитацию
            _velocity += Vector2.down * Gravity * Time.deltaTime;

            // 2. Применяем затухание
            _velocity *= Damping;

            // 3. Двигаем позицию по скорости
            Vector2 pos = transform.position;
            pos += _velocity * Time.deltaTime;

            // 4. Ограничение расстояния до Target (пружина)
            Vector2 targetPos = Target.position;
            Vector2 delta     = targetPos - pos;
            float   dist      = delta.magnitude;

            if (dist > Spacing)
            {
                // Тянем к Target с силой Stiffness
                Vector2 correction = delta.normalized * (dist - Spacing);
                pos       += correction * Stiffness;
                // Корректируем скорость чтобы не накапливать ошибку
                _velocity += correction * Stiffness / Time.deltaTime * 0.01f;
            }

            transform.position = new Vector3(pos.x, pos.y, transform.position.z);
        }

        // ── IPoolable ────────────────────────────────────────────────────

        public void OnSpawn()
        {
            Target       = null;
            SegmentIndex = 0;
            _velocity    = Vector2.zero;
        }

        public void OnDespawn()
        {
            Target       = null;
            SegmentIndex = 0;
            _velocity    = Vector2.zero;
        }

        // ── Visual ───────────────────────────────────────────────────────

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
