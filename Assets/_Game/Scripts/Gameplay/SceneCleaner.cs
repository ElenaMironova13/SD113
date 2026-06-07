using UnityEngine;
using Superdude.Core;

namespace Superdude.Gameplay
{
    /// <summary>
    /// При переходе в MainMenu или GameOver возвращает все
    /// активные FallingObject в пул — убирает пассажиров,
    /// чемоданы и постеры со сцены.
    /// Вешается на [Systems].
    /// </summary>
    public class SceneCleaner : MonoBehaviour
    {
        private void Awake()
        {
            EventBus.Subscribe<GameOverEvent>(OnClean);
            EventBus.Subscribe<MainMenuRequestedEvent>(OnClean);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<GameOverEvent>(OnClean);
            EventBus.Unsubscribe<MainMenuRequestedEvent>(OnClean);
        }

        private void OnClean(GameOverEvent e)          => CleanAll();
        private void OnClean(MainMenuRequestedEvent e) => CleanAll();

        private void CleanAll()
        {
            // Находим все активные FallingObject и возвращаем в пул
            var falling = FindObjectsByType<FallingObject>(FindObjectsSortMode.None);
            foreach (var obj in falling)
            {
                if (obj.gameObject.activeInHierarchy)
                    obj.GetComponent<PooledObject>()?.Release();
            }

            // Также возвращаем свободные звенья (FreeSegment)
            var free = FindObjectsByType<FreeSegment>(FindObjectsSortMode.None);
            foreach (var seg in free)
            {
                if (seg.gameObject.activeInHierarchy)
                    seg.Release();
            }
        }
    }
}
