using System.Collections;
using UnityEngine;
using Superdude.Core;

public class DevBootstrap : MonoBehaviour
{
    public bool AutoStartGame = true;

    private void Awake()
    {
        if (AutoStartGame)
            StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        yield return null;
        yield return null;
        EventBus.Publish(new GameStartedEvent());
        Debug.Log("[DevBootstrap] GameStartedEvent → Playing");
    }
}