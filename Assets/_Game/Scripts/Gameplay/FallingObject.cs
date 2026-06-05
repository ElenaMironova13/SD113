using UnityEngine;
using Superdude.Core;

namespace Superdude.Gameplay
{
    /// <summary>
    /// Базовый класс для всех падающих объектов: пассажир, чемодан, постер.
    /// Отвечает за движение вниз и возврат в пул при выходе за нижний край.
    ///
    /// Наследники переопределяют OnTriggerEnter2D для своей логики.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(PooledObject))]
    public abstract class FallingObject : MonoBehaviour, IPoolable
    {
        [Header("Fall")]
        [SerializeField] protected float FallSpeed = 3f;

        protected bool IsActive;

        // ── IPoolable ────────────────────────────────────────────────────

        public virtual void OnSpawn()
        {
            IsActive = true;
        }

        public virtual void OnDespawn()
        {
            IsActive = false;
        }

        // ── Movement ─────────────────────────────────────────────────────

        protected virtual void Update()
        {
            if (!IsActive) return;

            transform.Translate(Vector2.down * FallSpeed * Time.deltaTime);

            // Возврат в пул при выходе за нижний край экрана
            if (transform.position.y < CameraBounds.Bottom - 1f)
                OnExitBottom();
        }

        // ── Exit ─────────────────────────────────────────────────────────

        /// <summary>
        /// Вызывается когда объект вышел за нижний край.
        /// Наследник может переопределить для публикации события (штраф и т.д.)
        /// </summary>
        protected virtual void OnExitBottom()
        {
            GetComponent<PooledObject>().Release();
        }

        // ── Collision ────────────────────────────────────────────────────

        protected virtual void OnTriggerEnter2D(Collider2D other) { }
    }
}
