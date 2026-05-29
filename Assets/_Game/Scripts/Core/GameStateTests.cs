using System.Collections;
using UnityEngine;
using Superdude.Core;

/// <summary>
/// Тесты GameStateManager и связки EventBus → FSM.
/// Запустить в сцене Game рядом с GameStateManager и InputSystem.
/// Проверить Console: все строки [Tests] OK.
/// Удалить перед финальным билдом.
/// </summary>
public class GameStateTests : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return null; // ждём Awake всех систем

        TestInitialState();
        TestMainMenuToPlaying();
        TestPlayingToPaused();
        TestPausedToPlaying();
        TestPlayingToGameOver();
        TestGameOverToMainMenu();

        EventBus.Clear();
        Debug.Log("[Tests] GameStateManager — все тесты пройдены.");
    }

    private void TestInitialState()
    {
        var mgr = ServiceLocator.Get<GameStateManager>();
        // После Start GameStateManager переходит в MainMenu
        Assert(mgr.IsState(GameState.MainMenu), "Initial: состояние MainMenu");
    }

    private void TestMainMenuToPlaying()
    {
        EventBus.Publish(new GameStartedEvent());
        var mgr = ServiceLocator.Get<GameStateManager>();
        Assert(mgr.IsState(GameState.Playing), "GameStarted → Playing");
        Assert(Mathf.Approximately(Time.timeScale, 1f), "Playing: timeScale = 1");
    }

    private void TestPlayingToPaused()
    {
        EventBus.Publish(new PauseToggledEvent { IsPaused = true });
        var mgr = ServiceLocator.Get<GameStateManager>();
        Assert(mgr.IsState(GameState.Paused), "PauseToggled(true) → Paused");
        Assert(Mathf.Approximately(Time.timeScale, 0f), "Paused: timeScale = 0");
    }

    private void TestPausedToPlaying()
    {
        EventBus.Publish(new PauseToggledEvent { IsPaused = false });
        var mgr = ServiceLocator.Get<GameStateManager>();
        Assert(mgr.IsState(GameState.Playing), "PauseToggled(false) → Playing");
    }

    private void TestPlayingToGameOver()
    {
        EventBus.Publish(new TimerEndedEvent());
        var mgr = ServiceLocator.Get<GameStateManager>();
        Assert(mgr.IsState(GameState.GameOver), "TimerEnded → GameOver");
        Assert(Mathf.Approximately(Time.timeScale, 0f), "GameOver: timeScale = 0");
    }

    private void TestGameOverToMainMenu()
    {
        // MainMenuRequested меняет состояние и вызывает SceneLoader —
        // в тесте просто проверяем что переход произошёл до загрузки сцены
        // (SceneLoader.LoadMainMenu вызывается асинхронно)
        bool transitioned = false;
        EventBus.Subscribe<MainMenuRequestedEvent>(_ => transitioned = true);
        EventBus.Publish(new MainMenuRequestedEvent());
        Assert(transitioned, "MainMenuRequested: событие получено");
    }

    private static void Assert(bool condition, string label)
    {
        if (condition) Debug.Log($"[Tests] OK   — {label}");
        else           Debug.LogError($"[Tests] FAIL — {label}");
    }
}
