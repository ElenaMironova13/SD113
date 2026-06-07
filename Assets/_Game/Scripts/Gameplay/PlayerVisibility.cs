using UnityEngine;
using Superdude.Core;

namespace Superdude.Gameplay
{
    /// <summary>
    /// Скрывает визуал игрока в MainMenu.
    /// Скрывает только SpriteRenderer — физика и коллайдеры остаются.
    /// Вешается на GameObject Player.
    /// </summary>
    public class PlayerVisibility : MonoBehaviour
    {
        [Tooltip("Дочерний объект Visual со SpriteRenderer — назначить в Inspector")]
        [SerializeField] private GameObject _visual;

        private void Awake()
        {
            EventBus.Subscribe<GameStartedEvent>(OnGameStarted);
            EventBus.Subscribe<GameRestartedEvent>(OnGameStarted);
            EventBus.Subscribe<GameOverEvent>(OnGameOver);
            EventBus.Subscribe<MainMenuRequestedEvent>(OnMainMenu);

            HideVisual();
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<GameStartedEvent>(OnGameStarted);
            EventBus.Unsubscribe<GameRestartedEvent>(OnGameStarted);
            EventBus.Unsubscribe<GameOverEvent>(OnGameOver);
            EventBus.Unsubscribe<MainMenuRequestedEvent>(OnMainMenu);
        }

        private void OnGameStarted(GameStartedEvent e)    => ShowVisual();
        private void OnGameStarted(GameRestartedEvent e)  => ShowVisual();
        private void OnGameOver(GameOverEvent e)          => HideVisual();
        private void OnMainMenu(MainMenuRequestedEvent e) => HideVisual();

        private void ShowVisual() { if (_visual != null) _visual.SetActive(true); }
        private void HideVisual() { if (_visual != null) _visual.SetActive(false); }
    }
}
