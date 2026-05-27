namespace Superdude.Core
{
    /// <summary>
    /// Интерфейс для объектов, живущих в пуле.
    /// Реализуется компонентом на каждом префабе, который спаунится через ObjectPool.
    /// 
    /// OnSpawn  — вызывается при взятии из пула (аналог Awake/Start для пулированных объектов).
    /// OnDespawn — вызывается перед возвратом в пул (сброс состояния).
    /// </summary>
    public interface IPoolable
    {
        void OnSpawn();
        void OnDespawn();
    }
}
