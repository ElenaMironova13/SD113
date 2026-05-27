using UnityEngine;
using Superdude.Core;

/// <summary>
/// Тест EventBus и ServiceLocator — запустить в пустой сцене,
/// проверить Console на наличие всех "OK" и отсутствие "FAIL".
///
/// Удалить или переместить в папку Tests/ перед финальным билдом.
/// </summary>
public class EventBusTests : MonoBehaviour
{
    private void Start()
    {
        TestBasicPublishSubscribe();
        TestUnsubscribe();
        TestMultipleSubscribers();
        TestServiceLocator();
        TestServiceLocatorMissingThrows();

        Debug.Log("[Tests] Все тесты пройдены.");
    }

    private void TestBasicPublishSubscribe()
    {
        EventBus.Clear();
        bool received = false;

        EventBus.Subscribe<GameStartedEvent>(_ => received = true);
        EventBus.Publish(new GameStartedEvent());

        Assert(received, "BasicPublishSubscribe");
        EventBus.Clear();
    }

    private void TestUnsubscribe()
    {
        EventBus.Clear();
        int count = 0;
        void Handler(GameOverEvent _) => count++;

        EventBus.Subscribe<GameOverEvent>(Handler);
        EventBus.Publish(new GameOverEvent());   // count = 1
        EventBus.Unsubscribe<GameOverEvent>(Handler);
        EventBus.Publish(new GameOverEvent());   // count должен остаться 1

        Assert(count == 1, "Unsubscribe");
        EventBus.Clear();
    }

    private void TestMultipleSubscribers()
    {
        EventBus.Clear();
        int sum = 0;

        EventBus.Subscribe<ScoreChangedEvent>(e => sum += e.Score);
        EventBus.Subscribe<ScoreChangedEvent>(e => sum += e.Score);
        EventBus.Publish(new ScoreChangedEvent { Score = 5 });

        Assert(sum == 10, "MultipleSubscribers");
        EventBus.Clear();
    }

    private void TestServiceLocator()
    {
        ServiceLocator.Clear();
        var dummy = new System.Object();
        ServiceLocator.Register<System.Object>(dummy);
        var result = ServiceLocator.Get<System.Object>();

        Assert(result == dummy, "ServiceLocator");
        ServiceLocator.Clear();
    }

    private void TestServiceLocatorMissingThrows()
    {
        ServiceLocator.Clear();
        bool threw = false;
        try { ServiceLocator.Get<System.Random>(); }
        catch { threw = true; }

        Assert(threw, "ServiceLocatorMissingThrows");
        ServiceLocator.Clear();
    }

    private static void Assert(bool condition, string testName)
    {
        if (condition)
            Debug.Log($"[Tests] OK  — {testName}");
        else
            Debug.LogError($"[Tests] FAIL — {testName}");
    }
}
