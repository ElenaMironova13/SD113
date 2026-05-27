using System.Collections.Generic;
using UnityEngine;

namespace Superdude.Core
{
    /// <summary>
    /// Пул объектов, сгруппированный по префабу.
    /// Регистрируется в ServiceLocator при Awake.
    ///
    /// Использование:
    ///   // Взять объект из пула (или создать новый)
    ///   var go = ServiceLocator.Get&lt;ObjectPool&gt;().Get(myPrefab, position, rotation);
    ///
    ///   // Вернуть в пул
    ///   go.GetComponent&lt;PooledObject&gt;().Release();
    ///
    /// На префабе обязательно должен быть компонент PooledObject.
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {
        [Header("Prewarm")]
        [Tooltip("Список префабов и количество экземпляров для прогрева при старте.")]
        [SerializeField] private PrewarmEntry[] _prewarmEntries;

        // prefab instanceID → стек свободных объектов
        private readonly Dictionary<int, Stack<PooledObject>> _free =
            new Dictionary<int, Stack<PooledObject>>();

        // Все когда-либо созданные объекты — для очистки при смене сцены
        private readonly List<PooledObject> _all = new List<PooledObject>();

        // Родитель для деактивированных объектов (порядок в Hierarchy)
        private Transform _container;

        // ── Lifecycle ────────────────────────────────────────────────────

        private void Awake()
        {
            ServiceLocator.Register<ObjectPool>(this);

            _container = new GameObject("[Pool Container]").transform;
            _container.SetParent(transform);

            Prewarm();
        }

        private void Prewarm()
        {
            if (_prewarmEntries == null) return;

            foreach (var entry in _prewarmEntries)
            {
                if (entry.Prefab == null) continue;

                for (int i = 0; i < entry.Count; i++)
                {
                    var obj = CreateNew(entry.Prefab);
                    Recycle(obj);
                }
            }
        }

        // ── Public API ───────────────────────────────────────────────────

        /// <summary>
        /// Берёт объект из пула. Если свободных нет — создаёт новый.
        /// Вызывает OnSpawn у всех IPoolable-компонентов.
        /// </summary>
        public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            var key = prefab.GetInstanceID();

            PooledObject obj;

            if (_free.TryGetValue(key, out var stack) && stack.Count > 0)
            {
                obj = stack.Pop();
            }
            else
            {
                obj = CreateNew(prefab);
            }

            obj.transform.SetPositionAndRotation(position, rotation);
            obj.gameObject.SetActive(true);

            foreach (var poolable in obj.GetComponents<IPoolable>())
                poolable.OnSpawn();

            return obj.gameObject;
        }

        /// <summary>
        /// Перегрузка с rotation = Quaternion.identity.
        /// </summary>
        public GameObject Get(GameObject prefab, Vector3 position)
            => Get(prefab, position, Quaternion.identity);

        /// <summary>
        /// Возвращает все активные объекты данного префаба обратно в пул.
        /// Используется при GameOver для мгновенной очистки сцены.
        /// </summary>
        public void ReleaseAll(GameObject prefab)
        {
            var key = prefab.GetInstanceID();

            foreach (var obj in _all)
            {
                if (obj == null) continue;
                if (obj.gameObject.activeSelf && obj.gameObject.GetInstanceID() != key)
                {
                    // Ищем объекты с тем же prefab-ключом через кастомное поле
                    // (prefabKey проставляется в CreateNew)
                    if (obj.PrefabKey == key)
                        obj.Release();
                }
            }
        }

        // ── Internal ─────────────────────────────────────────────────────

        private PooledObject CreateNew(GameObject prefab)
        {
            var go = Instantiate(prefab, _container);
            go.SetActive(false);

            var pooled = go.GetComponent<PooledObject>();
            if (pooled == null)
            {
                Debug.LogError($"[ObjectPool] Префаб {prefab.name} не имеет компонента PooledObject. Добавляю автоматически.", prefab);
                pooled = go.AddComponent<PooledObject>();
            }

            pooled.PrefabKey     = prefab.GetInstanceID();
            pooled.ReturnToPool  = Recycle;

            _all.Add(pooled);
            return pooled;
        }

        private void Recycle(PooledObject obj)
        {
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(_container);

            var key = obj.PrefabKey;

            if (!_free.ContainsKey(key))
                _free[key] = new Stack<PooledObject>();

            _free[key].Push(obj);
        }

        // ── Stats (debug) ────────────────────────────────────────────────

        public int GetFreeCount(GameObject prefab)
        {
            var key = prefab.GetInstanceID();
            return _free.TryGetValue(key, out var s) ? s.Count : 0;
        }

        public int GetTotalCount() => _all.Count;
    }

    [System.Serializable]
    public class PrewarmEntry
    {
        public GameObject Prefab;
        [Min(0)] public int Count = 5;
    }
}
