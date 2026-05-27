namespace Superdude.Core
{
    /// <summary>
    /// Константы имён слоёв (Physics 2D → Layer Collision Matrix).
    /// Имена должны совпадать с тем, что настроено в Project Settings → Tags and Layers.
    /// </summary>
    public static class Layers
    {
        public const string Player    = "Player";
        public const string Chain     = "Chain";
        public const string Passenger = "Passenger";
        public const string Obstacle  = "Obstacle";
        public const string PowerUp   = "PowerUp";
    }

    /// <summary>
    /// Константы тегов GameObject.
    /// </summary>
    public static class Tags
    {
        public const string Player     = "Player";
        public const string ChainHead  = "ChainHead";
    }
}
