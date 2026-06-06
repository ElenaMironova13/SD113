using UnityEngine;
using Superdude.Core;

public class UIDebugger : MonoBehaviour
{
    private void Awake()
    {
        EventBus.Subscribe<GameOverEvent>(e =>
            Debug.Log("[UIDebugger] GameOverEvent получен"));
        EventBus.Subscribe<ScoreFinalizedEvent>(e =>
            Debug.Log($"[UIDebugger] ScoreFinalizedEvent: Score={e.Score}, Best={e.BestScore}, IsRecord={e.IsNewRecord}"));
        EventBus.Subscribe<TimerEndedEvent>(e =>
            Debug.Log("[UIDebugger] TimerEndedEvent получен"));
    }
}