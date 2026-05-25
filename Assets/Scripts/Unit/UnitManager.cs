using UnityEngine;

public class UnitManager : MonoBehaviour
{
    [Header("Настройки Unit")]
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private int unitCount = 1;
    [SerializeField] private Vector2 startPosition = new Vector2(0, 10);
    [SerializeField] private float topBoundary = 10f;
    [SerializeField] private float bottomBoundary = -10f;
    [SerializeField] private float spawnInterval = 0.5f;

    private float _spawnTimer;
    private int _spawnedCount;

    private void Awake()
    {
        if (unitPrefab == null)
        {
            var existingUnit = FindFirstObjectByType<Unit>();
            if (existingUnit != null)
            {
                unitPrefab = existingUnit.gameObject;
            }
        }
    }

    private void Update()
    {
        if (_spawnedCount >= unitCount)
            return;

        _spawnTimer += Time.deltaTime;

        if (_spawnTimer >= spawnInterval)
        {
            SpawnUnit();
            _spawnTimer = 0f;
        }
    }

    public void SpawnUnit()
    {
        if (_spawnedCount >= unitCount)
            return;

        var unitGameObject = Instantiate(unitPrefab, startPosition, Quaternion.identity);
        
        var unit = unitGameObject.GetComponent<Unit>();
        if (unit != null)
        {
            unit.StartPosition = startPosition;
            unit.TopBoundary = topBoundary;
            unit.BottomBoundary = bottomBoundary;
        }

        _spawnedCount++;
    }

    public void ResetUnits()
    {
        _spawnedCount = 0;
    }
}