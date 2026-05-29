using UnityEngine;

namespace Superdude.Core
{
    /// <summary>
    /// Конфиг одного типа препятствия (чемодан и др.).
    /// Создать: ПКМ → Create → Superdude → Obstacle Config
    /// Сохранить в Assets/_Game/Configs/SuitcaseConfig.asset
    /// </summary>
    [CreateAssetMenu(
        fileName = "ObstacleConfig",
        menuName  = "Superdude/Obstacle Config")]
    public class ObstacleConfig : ScriptableObject
    {
        [Header("Visuals")]
        public Sprite Sprite;

        [Header("Movement")]
        [Tooltip("Скорость падения препятствия (Units/sec)")]
        [Min(0.1f)] public float FallSpeed = 4f;

        [Header("Chain Break")]
        [Tooltip("Разрывает цепочку при столкновении")]
        public bool BreaksChain = true;

        [Tooltip("Сколько звеньев теряется при разрыве (0 = все после точки удара)")]
        [Min(0)] public int SegmentsLost = 0;

        [Header("Effects")]
        [Tooltip("Префаб эффекта при разрыве цепочки (опционально)")]
        public GameObject BreakEffect;
    }
}
