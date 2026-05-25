using UnityEngine;

public class UnitManager : MonoBehaviour
{
    [Header("Unit Settings")]
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private int unitCount = 1;
    [SerializeField] private Vector2 spawnPos = new Vector2(0, 10);
    [SerializeField] private float topBound = 10f;
    [SerializeField] private float bottomBound = -10f;
    [SerializeField] private float spawnInterval = 0.5f;

    private float timer;
    private int spawned;

    private void Awake()
    {
        if (unitPrefab == null)
        {
            var u = FindFirstObjectByType<Unit>();
            if (u != null) unitPrefab = u.gameObject;
        }
    }

    private void Update()
    {
        if (spawned >= unitCount) return;
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnUnit();
            timer = 0f;
        }
    }

    private void SpawnUnit()
    {
        if (spawned >= unitCount || unitPrefab == null) return;

        var go = Instantiate(unitPrefab, spawnPos, Quaternion.identity);
        var u = go.GetComponent<Unit>();
        if (u != null)
        {
            u.StartPos = spawnPos;
            u.TopBound = topBound;
            u.BottomBound = bottomBound;
        }
        spawned++;
    }
}