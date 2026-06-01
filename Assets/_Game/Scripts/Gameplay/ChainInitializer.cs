using UnityEngine;
using Superdude.Core;

namespace Superdude.Gameplay
{
    /// <summary>
    /// Связывает ChainManager с Player и префабом сегмента.
    /// Вешается на тот же GameObject что и ChainManager ([Systems]).
    ///
    /// Отделён от ChainManager чтобы не тащить Inspector-ссылки
    /// в логический класс.
    /// </summary>
    public class ChainInitializer : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Transform суперчувака — голова цепочки")]
        [SerializeField] private Transform _playerTransform;

        [Tooltip("Префаб одного звена цепочки (ChainSegment + PooledObject)")]
        [SerializeField] private GameObject _segmentPrefab;

        private void Start()
        {
            if (_playerTransform == null)
            {
                Debug.LogError("[ChainInitializer] Player Transform не назначен!", this);
                return;
            }
            if (_segmentPrefab == null)
            {
                Debug.LogError("[ChainInitializer] Segment Prefab не назначен!", this);
                return;
            }

            var chainManager = ServiceLocator.Get<ChainManager>();
            chainManager.Initialize(_playerTransform, _segmentPrefab);
        }
    }
}
