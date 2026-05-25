using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Unit : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private Vector2 startPosition = new Vector2(0, 10);
    [SerializeField] private float topBoundary = 10f;
    [SerializeField] private float bottomBoundary = -10f;
    [SerializeField] private bool isAttached = false;

    private Rigidbody2D _rb;
    private FixedJoint2D _joint;
    private bool _isInitialized;

    public Vector2 StartPosition
    {
        get => startPosition;
        set => startPosition = value;
    }

    public float TopBoundary
    {
        get => topBoundary;
        set => topBoundary = value;
    }

    public float BottomBoundary
    {
        get => bottomBoundary;
        set => bottomBoundary = value;
    }

    public bool IsAttached
    {
        get => isAttached;
        set => isAttached = value;
    }

    private void Awake()
    {
        if (_isInitialized)
            return;

        _rb = GetComponent<Rigidbody2D>();
        
        var collider = GetComponent<BoxCollider2D>();
        collider.size = new Vector2(1, 1);
        collider.offset = Vector2.zero;

        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }

        transform.position = startPosition;
        _isInitialized = true;
    }

    private void FixedUpdate()
    {
        if (!isAttached)
        {
            CheckBoundary();
        }
    }

    private void CheckBoundary()
    {
        if (transform.position.y < bottomBoundary)
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        _rb.linearVelocity = Vector2.zero;
        transform.position = new Vector3(startPosition.x, topBoundary, 0);
    }

    public void AttachTo(GameObject target)
    {
        if (isAttached || target == null)
            return;

        _joint = gameObject.AddComponent<FixedJoint2D>();
        _joint.connectedBody = target.GetComponent<Rigidbody2D>();
        _joint.connectedAnchor = Vector2.zero;
        _joint.anchor = Vector2.zero;
        
        isAttached = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isAttached)
            return;

        var player = collision.gameObject.GetComponent<PlayerPawn>();
        if (player != null)
        {
            AttachTo(collision.gameObject);
        }
    }
}