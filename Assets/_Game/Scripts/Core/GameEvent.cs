namespace Superdude.Core
{
    /// <summary>
    /// Базовый класс для всех событий проекта.
    /// Наследуйте от него, чтобы создать новое событие.
    /// </summary>
    public abstract class GameEvent { }

    // ── Input ────────────────────────────────────────────────────────────
    public class InputDirectionEvent : GameEvent
    {
        public UnityEngine.Vector2 Direction;
    }

    public class PauseToggledEvent : GameEvent
    {
        public bool IsPaused;
    }

    // ── Game State ───────────────────────────────────────────────────────
    public class GameStartedEvent       : GameEvent { }
    public class GameOverEvent          : GameEvent { }
    public class GameRestartedEvent     : GameEvent { }
    public class MainMenuRequestedEvent : GameEvent { }

    // ── Chain ────────────────────────────────────────────────────────────
    public class ChainGrewEvent : GameEvent
    {
        public int NewLength;
    }

    public class ChainBrokenEvent : GameEvent
    {
        public int BreakIndex;
        public int LostSegments;
    }

    // ── Score / Timer ────────────────────────────────────────────────────
    public class ScoreChangedEvent : GameEvent
    {
        public int Score;
    }

    public class TimerTickEvent : GameEvent
    {
        public float Remaining;
    }

    public class TimerEndedEvent : GameEvent { }

    public class ScoreFinalizedEvent : GameEvent
    {
        public int Score;
        public int BestScore;
        public bool IsNewRecord;
    }

    // ── PowerUp ──────────────────────────────────────────────────────────
    public class SpeedBoostEvent : GameEvent
    {
        public float Multiplier;
        public float Duration;
    }

    // ── Passenger ────────────────────────────────────────────────────────
    /// <summary>Пассажир вышел за нижний край — не был пойман.</summary>
    public class PassengerMissedEvent : GameEvent { }
}
