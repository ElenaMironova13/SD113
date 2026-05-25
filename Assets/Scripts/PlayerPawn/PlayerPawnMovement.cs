using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerPawnMovement : MonoBehaviour
{
    [Header("Скорость")]
    [SerializeField] private float moveSpeed = 5f;
    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }

    [Header("Инерция (коэффициент)")]
    [SerializeField] private float startInertia = 0.1f;
    [SerializeField] private float directionChangeInertia = 0.1f;
    [SerializeField] private float stopInertia = 0.1f;

    private Rigidbody2D _rb;
    private Vector2 _inputDirection;
    private Vector2 _currentVelocity;
    private Vector2 _lastNonZeroDirection = Vector2.down;
    private float _currentInertia;

    private enum InertiaType { Start, DirectionChange, Stop }
    private InertiaType _currentInertiaType = InertiaType.Stop;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.bodyType = RigidbodyType2D.Dynamic;
        _rb.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        ApplyMovementWithInertia();
    }

    public void SetInputDirection(Vector2 direction)
    {
        if (_inputDirection == Vector2.zero && direction != Vector2.zero)
        {
            _currentInertiaType = InertiaType.Start;
            _currentInertia = startInertia;
        }
        else if (_inputDirection != Vector2.zero && direction != Vector2.zero && Vector2.Dot(_inputDirection, direction) < 0)
        {
            _currentInertiaType = InertiaType.DirectionChange;
            _currentInertia = directionChangeInertia;
        }
        else if (direction == Vector2.zero)
        {
            _currentInertiaType = InertiaType.Stop;
            _currentInertia = stopInertia;
        }

        if (direction != Vector2.zero)
        {
            _lastNonZeroDirection = direction;
        }

        _inputDirection = direction;
    }

    private void ApplyMovementWithInertia()
    {
        Vector2 targetVelocity = _inputDirection * moveSpeed;

        if (_inputDirection == Vector2.zero)
        {
            _currentInertiaType = InertiaType.Stop;
            _currentInertia = stopInertia;
        }
        else if (_currentVelocity == Vector2.zero && _inputDirection != Vector2.zero)
        {
            _currentInertiaType = InertiaType.Start;
            _currentInertia = startInertia;
        }

        _currentVelocity = Vector2.Lerp(_currentVelocity, targetVelocity, _currentInertia);

        _rb.linearVelocity = _currentVelocity;
    }
}