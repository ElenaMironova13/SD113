using UnityEngine;
using UnityEngine.SceneManagement;

namespace Superdude.Core
{
    /// <summary>
    /// Имена сцен проекта.
    /// Используется везде вместо строковых литералов.
    /// </summary>
    public static class SceneNames
    {
        public const string Bootstrap = "Bootstrap";
        public const string MainMenu  = "MainMenu";
        public const string Game      = "Game";
    }

    /// <summary>
    /// Статический хелпер для переключения сцен.
    /// На шаге 12 будет заменён на async Addressables-версию.
    /// </summary>
    public static class SceneLoader
    {
        public static void Load(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public static void LoadMainMenu() => Load(SceneNames.MainMenu);
        public static void LoadGame()     => Load(SceneNames.Game);
    }
}
