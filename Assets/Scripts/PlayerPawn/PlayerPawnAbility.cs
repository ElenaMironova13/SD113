using UnityEngine;

public class PlayerPawnAbility : MonoBehaviour
{
    [SerializeField] private bool isEnabled = true;

    public bool IsEnabled
    {
        get => isEnabled;
        set => isEnabled = value;
    }

    public virtual void Activate()
    {
        if (!isEnabled)
            return;
    }

    public virtual void Deactivate()
    {
    }
}