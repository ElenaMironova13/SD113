using System;
using System.Collections.Generic;
using UnityEngine;

namespace Superdude.Core
{
    /// <summary>
    /// Центральная шина событий.
    /// Все системы общаются только через неё — без прямых ссылок друг на друга.
    ///
    /// Использование:
    ///   EventBus.Subscribe<GameStartedEvent>(OnGameStarted);   // в OnEnable
    ///   EventBus.Unsubscribe<GameStartedEvent>(OnGameStarted); // в OnDisable
    ///   EventBus.Publish(new GameStartedEvent());              // откуда угодно
    /// </summary>
    public static class EventBus
    {
        private static readonly Dictionary<Type, List<Delegate>> _handlers =
            new Dictionary<Type, List<Delegate>>();

        // ── Subscribe ────────────────────────────────────────────────────

        public static void Subscribe<T>(Action<T> handler) where T : GameEvent
        {
            var type = typeof(T);

            if (!_handlers.ContainsKey(type))
                _handlers[type] = new List<Delegate>();

            if (!_handlers[type].Contains(handler))
                _handlers[type].Add(handler);
        }

        // ── Unsubscribe ──────────────────────────────────────────────────

        public static void Unsubscribe<T>(Action<T> handler) where T : GameEvent
        {
            var type = typeof(T);

            if (_handlers.ContainsKey(type))
                _handlers[type].Remove(handler);
        }

        // ── Publish ──────────────────────────────────────────────────────

        public static void Publish<T>(T gameEvent) where T : GameEvent
        {
            var type = typeof(T);

            if (!_handlers.ContainsKey(type))
                return;

            // Копируем список перед итерацией — подписчик может отписаться
            // внутри своего обработчика без InvalidOperationException
            var snapshot = new List<Delegate>(_handlers[type]);

            foreach (var handler in snapshot)
            {
                try
                {
                    ((Action<T>)handler).Invoke(gameEvent);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[EventBus] Exception in handler for {type.Name}: {e}");
                }
            }
        }

        // ── Debug / Cleanup ──────────────────────────────────────────────

        /// <summary>
        /// Очищает все подписки. Вызывается Bootstrap-ом при смене сцены
        /// или в тестах для изоляции.
        /// </summary>
        public static void Clear()
        {
            _handlers.Clear();
        }

        /// <summary>
        /// Возвращает количество подписчиков для типа события (для дебага).
        /// </summary>
        public static int GetSubscriberCount<T>() where T : GameEvent
        {
            var type = typeof(T);
            return _handlers.ContainsKey(type) ? _handlers[type].Count : 0;
        }
    }
}
