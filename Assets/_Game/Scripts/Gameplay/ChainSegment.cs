using UnityEngine;
using Superdude.Core;

namespace Superdude.Gameplay
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ChainSegment : MonoBehaviour, IPoolable
    {
        public Transform Target       { get; set; }
        public int       SegmentIndex { get; set; }
        public float     Spacing      { get; set; } = 0.4f;

        [Header("Physics Simulation")]
        [Tooltip("Сила гравитации на звено (вниз)")]
        public float Gravity   = 9.8f;

        [Tooltip("Затухание скорости [0..1]")]
        [Range(0.8f, 0.99f)]
        public float Damping   = 0.95f;

        [Tooltip("Жёсткость пружины к Target [0..1]")]
        [Range(0.1f, 1f)]
        public float Stiffness = 0.5f;

        private SpriteRenderer _sprite;
        private Vector2        _velocity = Vector2.zero;

        // ── Lifecycle ────────────────────────────────────────────────────

        private void Awake()
        {
            _sprite = GetComponent<SpriteRenderer>();
        }

        private void LateUpdate()
        {
            if (Target == null) return;

            float dt = Time.deltaTime;
            if (dt <= 0f) return; // пауза — пропускаем кадр

            // 1. Гравитация и затухание
            _velocity += Vector2.down * Gravity * dt;
            _velocity *= Damping;

            // 2. Двигаем позицию
            Vector2 pos = transform.position;
            pos += _velocity * dt;

            // 3. Пружина к Target — только если есть ненулевое расстояние
            Vector2 targetPos = Target.position;
            Vector2 delta     = targetPos - pos;
            float   dist      = delta.magnitude;

            if (dist > Spacing && dist > 0.0001f) // защита от деления на 0
            {
                Vector2 dir        = delta / dist; // вручную нормализуем
                Vector2 correction = dir * (dist - Spacing);
                pos       += correction * Stiffness;
                _velocity += correction * (Stiffness * 0.1f); // без деления на dt
            }

            // 4. Финальная проверка перед записью
            if (float.IsNaN(pos.x) || float.IsNaN(pos.y))
            {
                Debug.LogWarning($"[ChainSegment] NaN позиция на {name}, сбрасываю к Target");
                pos       = targetPos;
                _velocity = Vector2.zero;
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
