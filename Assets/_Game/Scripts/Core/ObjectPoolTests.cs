using System.Collections;
using UnityEngine;
using Superdude.Core;

/// <summary>
/// Тесты ObjectPool. Запустить в сцене с объектом ObjectPool на сцене.
/// Проверить Console: все строки [Tests] OK.
///
/// Удалить перед финальным билдом.
/// </summary>
public class ObjectPoolTests : MonoBehaviour
{
    [SerializeField] private GameObject _testPrefab;

    private IEnumerator Start()
    {
        // Даём ObjectPool время на Awake
        yield return null;

        if (_testPrefab == null)
        {
            Debug.LogError("[PoolTests] Назначьте _testPrefab в Inspector (любой префаб с PooledObject).");
            yield break;
        }

        TestGetActivatesObject();
        yield return null;
        TestReleaseDeactivatesObject();
        yield return null;
        TestGetAfterReleaseReusesObject();
        yield return null;
        TestOnSpawnOnDespawnCalled();

        Debug.Log("[Tests] ObjectPool — все тесты пройдены.");
    }

    private void TestGetActivatesObject()
    {
        var pool = ServiceLocator.Get<ObjectPool>();
        var go   = pool.Get(_testPrefab, Vector3.zero);

        Assert(go != null,           "Get: объект не null");
        Assert(go.activeSelf,        "Get: объект активен");
    }

    private void TestReleaseDeactivatesObject()
    {
        var pool   = ServiceLocator.Get<ObjectPool>();
        var go     = pool.Get(_testPrefab, Vector3.zero);
        var pooled = go.GetComponent<PooledObject>();

        pooled.Release();

        Assert(!go.activeSelf, "Release: объект деактивирован");
    }

    private void TestGetAfterReleaseReusesObject()
    {
        var pool   = ServiceLocator.Get<ObjectPool>();
        var go1    = pool.Get(_testPrefab, Vector3.zero);
        go1.GetComponent<PooledObject>().Release();

        int before = pool.GetTotalCount();
        var go2    = pool.Get(_testPrefab, Vector3.zero);
        int after  = pool.GetTotalCount();

        Assert(go1 == go2,       "Reuse: тот же экземпляр повторно");
        Assert(before == after,  "Reuse: новый объект не создан");
    }

    private void TestOnSpawnOnDespawnCalled()
    {
        var pool = ServiceLocator.Get<ObjectPool>();

        // Создаём временный GO с MockPoolable для проверки колбэков
        var prefabGo = new GameObject("MockPrefab");
        prefabGo.AddComponent<PooledObject>();
        var mock = prefabGo.AddComponent<MockPoolable>();

        var go     = pool.Get(prefabGo, Vector3.zero);
        Assert(mock.SpawnCount == 1, "OnSpawn: вызван один раз");

        go.GetComponent<PooledObject>().Release();
        Assert(mock.DespawnCount == 1, "OnDespawn: вызван один раз");

        Destroy(prefabGo);
    }

    private static void Assert(bool condition, string label)
    {
        if (condition) Debug.Log($"[Tests] OK   — {label}");
        else           Debug.LogError($"[Tests] FAIL — {label}");
    }
}

/// <summary>Вспомогательный компонент для теста колбэков.</summary>
public class MockPoolable : MonoBehaviour, Superdude.Core.IPoolable
{
    public int SpawnCount;
    public int DespawnCount;

    public void OnSpawn()   => SpawnCount++;
    public void OnDespawn() => DespawnCount++;
}
