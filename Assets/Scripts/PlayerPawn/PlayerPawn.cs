using UnityEngine;

[RequireComponent(typeof(PlayerPawnMovement))]
[RequireComponent(typeof(PlayerPawnAbility))]
[RequireComponent(typeof(SimplePlayerInput))]
public class PlayerPawn : MonoBehaviour
{
    [Header("Присоединённые Unit")]
    [SerializeField] private GameObject attachedUnit;

    private PlayerPawnMovement _movement;
    private PlayerPawnAbility _ability;
    private SimplePlayerInput _input;

    public GameObject AttachedUnit
    {
        get => attachedUnit;
        set => attachedUnit = value;
    }

    private void Awake()
    {
        _movement = GetComponent<PlayerPawnMovement>();
        _ability = GetComponent<PlayerPawnAbility>();
        _input = GetComponent<SimplePlayerInput>();
    }

    private void Update()
    {
        HandleMovementInput();
        HandleAbilityInput();
    }

    private void HandleMovementInput()
    {
        Vector2 inputDirection = _input.GetMoveInput();
        
        if (_movement != null)
        {
            _movement.SetInputDirection(inputDirection);
        }
    }

    private void HandleAbilityInput()
    {
        if (_input.GetAbilityDown() && _ability != null)
        {
            _ability.Activate();
        }
    }
}