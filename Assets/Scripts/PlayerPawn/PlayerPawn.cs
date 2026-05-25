using UnityEngine;

[RequireComponent(typeof(PlayerPawnMovement))]
[RequireComponent(typeof(PlayerPawnAbility))]
public class PlayerPawn : MonoBehaviour
{
    [Header("Управление")]
    [SerializeField] private KeyCode moveUpKey = KeyCode.W;
    [SerializeField] private KeyCode moveDownKey = KeyCode.S;
    [SerializeField] private KeyCode moveLeftKey = KeyCode.A;
    [SerializeField] private KeyCode moveRightKey = KeyCode.D;
    [SerializeField] private KeyCode abilityKey = KeyCode.Space;

    [Header("Присоединённые Unit")]
    [SerializeField] private GameObject attachedUnit;

    private PlayerPawnMovement _movement;
    private PlayerPawnAbility _ability;

    public GameObject AttachedUnit
    {
        get => attachedUnit;
        set => attachedUnit = value;
    }

    private void Awake()
    {
        _movement = GetComponent<PlayerPawnMovement>();
        _ability = GetComponent<PlayerPawnAbility>();
    }

    private void Update()
    {
        HandleMovementInput();
        HandleAbilityInput();
    }

    private void HandleMovementInput()
    {
        Vector2 inputDirection = Vector2.zero;

        if (Input.GetKey(moveUpKey))
            inputDirection.y = 1f;
        else if (Input.GetKey(moveDownKey))
            inputDirection.y = -1f;

        if (Input.GetKey(moveLeftKey))
            inputDirection.x = -1f;
        else if (Input.GetKey(moveRightKey))
            inputDirection.x = 1f;

        _movement.SetInputDirection(inputDirection.normalized);
    }

    private void HandleAbilityInput()
    {
        if (Input.GetKeyDown(abilityKey))
        {
            _ability.Activate();
        }
    }
}