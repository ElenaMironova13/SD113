using UnityEngine;
using Superdude.Core;
using Superdude.Gameplay;

public class ChainTest : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ServiceLocator.Get<ChainManager>().AddSegments(1);
            Debug.Log("Chain length: " + ServiceLocator.Get<ChainManager>().Length);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            ServiceLocator.Get<ChainManager>().BreakAt(0);
            Debug.Log("Chain broken");
        }
    }
}