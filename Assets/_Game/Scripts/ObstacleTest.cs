using UnityEngine;
using Superdude.Core;
using Superdude.Gameplay;

public class FallTest : MonoBehaviour
{
    [SerializeField] private GameObject _passengerPrefab;
    [SerializeField] private GameObject _obstaclePrefab;
    [SerializeField] private GameObject _powerUpPrefab;

    private void Update()
    {
        var pool = ServiceLocator.Get<ObjectPool>();

        // 1 Ч спаун пассажира над игроком
        if (Input.GetKeyDown(KeyCode.Alpha1))
            pool.Get(_passengerPrefab, new Vector3(0, 4, 0));

        // 2 Ч спаун чемодана
        if (Input.GetKeyDown(KeyCode.Alpha2))
            pool.Get(_obstaclePrefab, new Vector3(0, 4, 0));

        // 3 Ч спаун постера
        if (Input.GetKeyDown(KeyCode.Alpha3))
            pool.Get(_powerUpPrefab, new Vector3(0, 4, 0));
    }
}
