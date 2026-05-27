using UnityEngine;

public class SimplePlayerInput : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private KeyCode moveUpKey = KeyCode.W;
    [SerializeField] private KeyCode moveDownKey = KeyCode.S;
    [SerializeField] private KeyCode moveLeftKey = KeyCode.A;
    [SerializeField] private KeyCode moveRightKey = KeyCode.D;
    [SerializeField] private KeyCode abilityKey = KeyCode.Space;

    private Vector2 _inputMove;
    private bool _abilityPressed;

    public Vector2 InputMove => _inputMove;
    public bool AbilityPressed => _abilityPressed;

    private void Update()
    {
        ReadInput();
    }

    private void ReadInput()
    {
        float horizontal = 0;
        float vertical = 0;

        if (Input.GetKey(moveUpKey) || Input.GetKey(KeyCode.UpArrow))
        {
            vertical = 1;
        }
        else if (Input.GetKey(moveDownKey) || Input.GetKey(KeyCode.DownArrow))
        {
            vertical = -1;
        }

        if (Input.GetKey(moveLeftKey) || Input.GetKey(KeyCode.LeftArrow))
        {
            horizontal = -1;
        }
        else if (Input.GetKey(moveRightKey) || Input.GetKey(KeyCode.RightArrow))
        {
            horizontal = 1;
        }

        _inputMove = new Vector2(horizontal, vertical).normalized;

        _abilityPressed = Input.GetKeyDown(abilityKey);
    }

    public Vector2 GetMoveInput()
    {
        return _inputMove;
    }

    public bool GetAbilityDown()
    {
        if (_abilityPressed)
        {
            _abilityPressed = false;
            return true;
        }
        return false;
    }
}