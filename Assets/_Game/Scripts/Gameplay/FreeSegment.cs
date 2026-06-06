using System.Collections.Generic;
using UnityEngine;
using Superdude.Core;

namespace Superdude.Gameplay
{
    /// <summary>
    /// Добавляется динамически на каждое звено после разрыва цепочки.
    /// Хранит ссылку на группу (FreeChainGroup) — все звенья одного
    /// оторвавшегося хвоста знают друг о друге.
    ///
    /// При коллизии игрока с ЛЮБЫМ звеном группы — вся группа
    /// присоединяется к цепочке через ChainManager.AddSegments().
    /// </summary>
    public class FreeSegment : MonoBehaviour
    {
        [SerializeField] private float _fallSpeed   = 3f;
        [SerializeField] private float _lateralDamp = 0.92f;

        // Группа к которой принадлежит это звено
        public FreeChainGroup Group { get; set; }

        private Vector2 _velocity;
        private bool    _active;

        // ── Activation ───────────────────────────────────────────────────

        public void Activate(Vector2 inheritedVelocity, FreeChainGroup group)
        {
            Group = group;

            var segment = GetComponent<ChainSegment>();
            if (segment != null)
            {
                segment.Target  = null;
                segment.enabled = false;
            }

            _velocity = inheritedVelocity;
            _active   = true;
        }

        // ── Update ───────────────────────────────────────────────────────

        private void Update()
        {
            if (!_active) return;

            _velocity.y -= _fallSpeed * Time.deltaTime;
            _velocity.x *= _lateralDamp;
            transform.Translate(_velocity * Time.deltaTime);

            if (transform.position.y < CameraBounds.Bottom - 1f)
                Despawn();
        }

        // ── Collision — игрок касается звена ────────────────────────────

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_active) return;
            if (!other.CompareTag(Tags.Player)) return;
            if (Group == null || Group.IsAbsorbed) return;

            Group.AbsorbToChain();
        }

        // ── Despawn ──────────────────────────────────────────────────────

        public void Release()
        {
            if (!_active) return;
            _active = false;

            var segment = GetComponent<ChainSegment>();
            if (segment != null) segment.enabled = true;

            Destroy(this);
            GetComponent<PooledObject>().Release();
        }

        private void Despawn()
        {
            Group?.NotifyDespawned(this);
            Release();
        }
    }

    // ────────────────────────────────────────────────────────────────────
    // Группа свободных звеньев — один оторвавшийся хвост
    // ────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Контейнер для группы FreeSegment одного разрыва.
    /// Не MonoBehaviour — живёт как обычный C# объект.
    /// </summary>
    public class FreeChainGroup
    {
        public bool IsAbsorbed { get; private set; }

        private readonly List<FreeSegment> _members;
        private readonly ChainManager      _chain;

        public FreeChainGroup(List<FreeSegment> members, ChainManager chain)
        {
            _members = members;
            _chain   = chain;
        }

        /// <summary>
        /// Присоединяет все оставшиеся звенья к цепочке игрока.
        /// Вызывается при коллизии любого звена с игроком.
        /// </summary>
        public void AbsorbToChain()
        {
            if (IsAbsorbed) return;
            IsAbsorbed = true;

            int count = _members.Count;

            // Возвращаем все звенья в пул — ChainManager создаст новые
            foreach (var seg in _members)
                if (seg != null) seg.Release();

            _members.Clear();

            // Добавляем эквивалентное количество звеньев в активную цепочку
            if (count > 0)
                _chain.AddSegments(count);
        }

        /// <summary>Звено само упало за экран — убираем из группы.</summary>
        public void NotifyDespawned(FreeSegment seg)
        {
            _members.Remove(seg);
        }
    }
}
