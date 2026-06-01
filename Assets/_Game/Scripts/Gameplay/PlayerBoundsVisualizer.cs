using UnityEngine;
using Superdude.Core;

namespace Superdude.Gameplay
{
    /// <summary>
    /// Рисует границы экрана в Scene View для удобства расстановки объектов.
    /// Только Editor — в билде не работает.
    /// Вешается на тот же GameObject что и PlayerController.
    /// </summary>
    public class PlayerBoundsVisualizer : MonoBehaviour
    {
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // CameraBounds заполняется только в Play Mode
            if (!Application.isPlaying) return;

            Gizmos.color = new Color(0f, 1f, 0f, 0.3f);

            var center = new Vector3(
                (CameraBounds.Left + CameraBounds.Right)  / 2f,
                (CameraBounds.Top  + CameraBounds.Bottom) / 2f,
                0f);

            var size = new Vector3(
                CameraBounds.Right - CameraBounds.Left,
                CameraBounds.Top   - CameraBounds.Bottom,
                0f);

            Gizmos.DrawWireCube(center, size);
        }
#endif
    }
}
