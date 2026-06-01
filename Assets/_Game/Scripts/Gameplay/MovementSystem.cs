using UnityEngine;
using Superdude.Core;

namespace Superdude.Gameplay
{
    /// <summary>
    /// Отвечает исключительно за физическое перемещение Rigidbody2D.
    /// Не знает об игроке, цепочке или состоянии игры —
    /// принимает Direction снаружи и двигает тело.
    ///
    /// Клампит позицию по CameraBounds, чтобы игрок не вышел за экран.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class MovementSystem : MonoBehaviour
    {
        // Текущее направление — выставляется PlayerController
        public Vector2 Direction { get; set; } = Vector2.zero;

        // Текущая скорость с учётом активных модификаторов
        public float CurrentSpeed => _baseSpeed * _speedMultiplier;

        private Rigidbody2D _rb;
        private float _baseSpeed;
        private float _speedMultiplier = 1f;

        // ── Lifecycle ────────────────────────────────────────────────────

        public void Initialize(float baseSpeed)
        {
            _rb        = GetComponent<Rigidbody2D>();
            _baseSpeed = baseSpeed;

            // Rigidbody2D настройки для top-down 2D
            _rb.gravityScale  = 0f;
            _rb.freezeRotation = true;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        private void FixedUpdate()
        {
            if (Direction == Vector2.zero) return;

            var newPos = _rb.position + Direction * (CurrentSpeed * Time.fixedDeltaTime);
            newPos = ClampToBounds(newPos);
            _rb.MovePosition(newPos);
        }

        // ── Public API ───────────────────────────────────────────────────

        /// <summary>
        /// Применяет временный множитель скорости (от PowerUp).
        /// Повторный вызов перезаписывает предыдущий.
        /// </summary>
        public void ApplySpeedMultiplier(float multiplier)
        {
            _speedMultiplier = multiplier;
        }

        public void ResetSpeedMultiplier()
        {
            _speedMultiplier = 1f;
        }

        public void SetBaseSpeed(float speed)
        {
            _baseSpeed = speed;
        }

        // ── Bounds ───────────────────────────────────────────────────────

        private static Vector2 ClampToBounds(Vector2 pos)
        {
            // Небольшой отступ, чтобы спрайт не выходил за край
            const float margin = 0.3f;

            return new Vector2(
                Mathf.Clamp(pos.x, CameraBounds.Left  + margin, CameraBounds.Right - margin),
                Mathf.Clamp(pos.y, CameraBounds.Bottom + margin, CameraBounds.Top  - margin)
            );
        }
    }
}
