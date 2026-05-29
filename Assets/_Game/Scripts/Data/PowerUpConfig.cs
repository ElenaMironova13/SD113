using UnityEngine;

namespace Superdude.Core
{
    /// <summary>
    /// Конфиг одного типа усиления (постер с супергероем и др.).
    /// Создать: ПКМ → Create → Superdude → PowerUp Config
    /// Сохранить в Assets/_Game/Configs/PosterConfig.asset
    /// </summary>
    [CreateAssetMenu(
        fileName = "PowerUpConfig",
        menuName  = "Superdude/PowerUp Config")]
    public class PowerUpConfig : ScriptableObject
    {
        [Header("Visuals")]
        public Sprite Sprite;

        [Header("Movement")]
        [Tooltip("Скорость падения постера (Units/sec)")]
        [Min(0.1f)] public float FallSpeed = 2.5f;

        [Header("Effect")]
        [Tooltip("Тип усиления")]
        public PowerUpType Type = PowerUpType.SpeedBoost;

        [Tooltip("Множитель эффекта (для SpeedBoost: 1.5 = +50% скорости)")]
        [Range(1.1f, 3f)] public float EffectMultiplier = 1.5f;

        [Tooltip("Длительность эффекта (секунды)")]
        [Min(0.5f)] public float Duration = 3f;

        [Header("Effects")]
        [Tooltip("Префаб эффекта при подборе (опционально)")]
        public GameObject PickupEffect;
    }

    public enum PowerUpType
    {
        SpeedBoost
        // Место для будущих типов: ScoreMultiplier, Shield и т.д.
    }
}
