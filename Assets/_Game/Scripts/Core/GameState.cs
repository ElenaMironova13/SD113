namespace Superdude.Core
{
    /// <summary>
    /// Все возможные состояния игры.
    /// GameStateManager переключает их, все системы читают текущее состояние.
    /// </summary>
    public enum GameState
    {
        None,
        MainMenu,
        Playing,
        Paused,
        GameOver
    }
}
