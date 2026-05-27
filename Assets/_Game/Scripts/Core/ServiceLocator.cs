using System;
using System.Collections.Generic;
using UnityEngine;

namespace Superdude.Core
{
    /// <summary>
    /// Лёгкий контейнер сервисов.
    /// Заменяет россыпь Singleton-ов единой точкой доступа.
    ///
    /// Регистрация (в Bootstrap или Awake сервиса):
    ///   ServiceLocator.Register<IScoreSystem>(this);
    ///
    /// Получение (в любом месте):
    ///   var score = ServiceLocator.Get<IScoreSystem>();
    ///
    /// Важно: Register вызывать до любого Get.
    /// Bootstrap гарантирует порядок инициализации (шаг 12).
    /// </summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services =
            new Dictionary<Type, object>();

        // ── Register ─────────────────────────────────────────────────────

        public static void Register<T>(T service)
        {
            var type = typeof(T);

            if (_services.ContainsKey(type))
            {
                Debug.LogWarning($"[ServiceLocator] Overwriting existing service: {type.Name}");
            }

            _services[type] = service;
        }

        // ── Get ───────────────────────────────────────────────────────────

        /// <summary>
        /// Возвращает сервис. Бросает исключение если не найден —
        /// это намеренно: молчаливый null хуже явной ошибки.
        /// </summary>
        public static T Get<T>()
        {
            var type = typeof(T);

            if (_services.TryGetValue(type, out var service))
                return (T)service;

            throw new InvalidOperationException(
                $"[ServiceLocator] Service not found: {type.Name}. " +
                $"Убедитесь что Bootstrap зарегистрировал его до обращения.");
        }

        /// <summary>
        /// Безопасная версия — возвращает default если сервис не найден.
        /// Используйте только когда наличие сервиса необязательно.
        /// </summary>
        public static T GetOrDefault<T>()
        {
            var type = typeof(T);
            return _services.TryGetValue(type, out var service) ? (T)service : default;
        }

        public static bool IsRegistered<T>()
        {
            return _services.ContainsKey(typeof(T));
        }

        // ── Unregister / Clear ────────────────────────────────────────────

        public static void Unregister<T>()
        {
            _services.Remove(typeof(T));
        }

        /// <summary>
        /// Полная очистка. Вызывается Bootstrap-ом при завершении игры
        /// или в тестах для изоляции.
        /// </summary>
        public static void Clear()
        {
            _services.Clear();
        }
    }
}
