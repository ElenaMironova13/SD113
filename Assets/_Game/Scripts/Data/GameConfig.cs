using UnityEngine;

namespace Superdude.Core
{
    /// <summary>
    /// Главный конфиг игры. Все числовые параметры — здесь.
    /// Создать: ПКМ в Project → Create → Superdude → Game Config
    /// Сохранить в Assets/_Game/Configs/GameConfig.asset
    /// </summary>
    [CreateAssetMenu(
        fileName = "GameConfig",
        menuName  = "Superdude/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Player Movement")]
        [Tooltip("Базовая скорость суперчувака (Units/sec)")]
        [Min(0.1f)] public float PlayerSpeed = 5f;

        [Header("Timer")]
        [Tooltip("Длительность раунда в секундах")]
        [Min(5f)] public float RoundDuration = 60f;

        [Header("Chain")]
        [Tooltip("Сколько звеньев добавляет один пассажир")]
        [Min(1)] public int SegmentsPerPassenger = 1;

        [Tooltip("Расстояние между звеньями цепочки (Units)")]
        [Min(0.05f)] public float SegmentSpacing = 0.4f;

        [Header("Spawning")]
        [Tooltip("Начальный интервал спауна объектов (секунды)")]
        [Min(0.1f)] public float SpawnInterval = 1.5f;

        [Tooltip("Минимальный интервал спауна — не опустится ниже (секунды)")]
        [Min(0.1f)] public float SpawnIntervalMin = 0.4f;

        [Tooltip("На сколько уменьшается интервал спауна каждые 10 секунд")]
        [Min(0f)]   public float SpawnIntervalDecrement = 0.1f;

        [Tooltip("Ширина зоны спауна: доля ширины экрана [0..1]")]
        [Range(0.1f, 1f)] public float SpawnWidthFraction = 0.9f;

        [Header("Spawn Weights")]
        [Tooltip("Вероятностный вес пассажира среди всех объектов")]
        [Min(1)] public int PassengerWeight = 6;

        [Tooltip("Вероятностный вес чемодана (Obstacle)")]
        [Min(1)] public int SuitcaseWeight  = 3;

        [Tooltip("Вероятностный вес постера (PowerUp)")]
        [Min(1)] public int PosterWeight    = 1;

        [Header("PowerUp — Speed Boost")]
        [Tooltip("Множитель скорости от постера (1.5 = +50%)")]
        [Range(1.1f, 3f)] public float SpeedBoostMultiplier = 1.5f;

        [Tooltip("Длительность буста скорости (секунды)")]
        [Min(0.5f)] public float SpeedBoostDuration = 3f;
    }
}
