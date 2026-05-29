using UnityEngine;

namespace Superdude.Core
{
    /// <summary>
    /// Конфиг одного типа пассажира.
    /// Создать: ПКМ → Create → Superdude → Passenger Config
    /// Сохранить в Assets/_Game/Configs/PassengerConfig.asset
    /// </summary>
    [CreateAssetMenu(
        fileName = "PassengerConfig",
        menuName  = "Superdude/Passenger Config")]
    public class PassengerConfig : ScriptableObject
    {
        [Header("Visuals")]
        [Tooltip("Спрайт пассажира (опционально — можно задать прямо на префабе)")]
        public Sprite Sprite;

        [Header("Movement")]
        [Tooltip("Скорость падения пассажира (Units/sec)")]
        [Min(0.1f)] public float FallSpeed = 3f;

        [Header("Chain")]
        [Tooltip("Сколько звеньев добавляет этот тип пассажира")]
        [Min(1)] public int SegmentsAdded = 1;

        [Header("Score")]
        [Tooltip("Очков за присоединение одного пассажира")]
        [Min(0)] public int ScoreValue = 10;

        [Header("Effects")]
        [Tooltip("Префаб эффекта при присоединении (опционально)")]
        public GameObject JoinEffect;
    }
}
