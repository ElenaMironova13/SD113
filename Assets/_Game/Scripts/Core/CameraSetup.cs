using UnityEngine;

namespace Superdude.Core
{
    /// <summary>
    /// Настройка ортографической камеры.
    /// Вешается на Main Camera в сцене Game.
    /// Вычисляет мировые границы экрана и кладёт их в CameraBounds
    /// для использования в MovementSystem и SpawnManager.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraSetup : MonoBehaviour
    {
        private Camera _cam;

        private void Awake()
        {
            _cam = GetComponent<Camera>();
            _cam.orthographic = true;

            CalculateBounds();
        }

        private void CalculateBounds()
        {
            float height = _cam.orthographicSize;
            float width  = height * _cam.aspect;

            CameraBounds.Top    =  height;
            CameraBounds.Bottom = -height;
            CameraBounds.Right  =  width;
            CameraBounds.Left   = -width;
        }
    }

    /// <summary>
    /// Статическое хранилище границ экрана в мировых координатах.
    /// Заполняется CameraSetup при Awake.
    /// </summary>
    public static class CameraBounds
    {
        public static float Top;
        public static float Bottom;
        public static float Left;
        public static float Right;
    }
}
