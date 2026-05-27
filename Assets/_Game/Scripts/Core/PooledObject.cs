using UnityEngine;

namespace Superdude.Core
{
    /// <summary>
    /// Компонент-маркер на каждом префабе, который спаунится через ObjectPool.
    /// Хранит обратную ссылку на свой пул и умеет вернуться в него.
    ///
    /// Используйте Release() вместо Destroy() для пулированных объектов.
    /// </summary>
    public class PooledObject : MonoBehaviour
    {
        // Заполняется ObjectPool при создании экземпляра
        internal System.Action<PooledObject> ReturnToPool;

        // instanceID префаба-источника, нужен ObjectPool для сортировки по пулам
        internal int PrefabKey;

        /// <summary>
        /// Возвращает объект в пул: деактивирует GameObject,
        /// вызывает OnDespawn у всех IPoolable-компонентов.
        /// </summary>
        public void Release()
        {
            if (ReturnToPool == null)
            {
                Debug.LogWarning($"[PooledObject] ReturnToPool не задан на {name}. Уничтожаю объект.", this);
                Destroy(gameObject);
                return;
            }

            // Вызываем OnDespawn перед деактивацией
            foreach (var poolable in GetComponents<IPoolable>())
                poolable.OnDespawn();

            ReturnToPool.Invoke(this);
        }
    }
}
