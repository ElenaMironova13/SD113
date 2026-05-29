using UnityEngine;

namespace Superdude.Core
{
    /// <summary>
    /// Загружает все конфиги из Inspector и регистрирует их в ServiceLocator.
    /// Вешается на GameObject [Systems] в сцене Game.
    ///
    /// Любая система получает конфиг так:
    ///   var cfg = ServiceLocator.Get&lt;GameConfig&gt;();
    /// </summary>
    public class ConfigProvider : MonoBehaviour
    {
        [Header("Required")]
        [SerializeField] private GameConfig     _gameConfig;

        [Header("Object Configs")]
        [SerializeField] private PassengerConfig _passengerConfig;
        [SerializeField] private ObstacleConfig  _suitcaseConfig;
        [SerializeField] private PowerUpConfig   _posterConfig;

        private void Awake()
        {
            ValidateAndRegister(_gameConfig,      "GameConfig");
            ValidateAndRegister(_passengerConfig, "PassengerConfig");
            ValidateAndRegister(_suitcaseConfig,  "SuitcaseConfig (Obstacle)");
            ValidateAndRegister(_posterConfig,    "PosterConfig (PowerUp)");
        }

        private void ValidateAndRegister<T>(T config, string label) where T : ScriptableObject
        {
            if (config == null)
            {
                Debug.LogError($"[ConfigProvider] {label} не назначен в Inspector!", this);
                return;
            }
            ServiceLocator.Register<T>(config);
        }
    }
}
