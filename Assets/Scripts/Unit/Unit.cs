using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Unit : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2 startPos = new Vector2(0, 10);
    [SerializeField] private float topBound = 10f;
    [SerializeField] private float bottomBound = -10f;
    [SerializeField] private bool isLinked = false;

    [Header("Chain")]
    [SerializeField] private HingeJoint2D joint;
    [SerializeField] private Unit nextUnit;

    private Rigidbody2D rb;
    private BoxCollider2D bc;
    private bool ready;

    public Vector2 StartPos { get => startPos; set => startPos = value; }
    public float TopBound { get => topBound; set => topBound = value; }
    public float BottomBound { get => bottomBound; set => bottomBound = value; }
    public bool IsLinked { get => isLinked; set => isLinked = value; }
    public Unit NextUnit { get => nextUnit; set => nextUnit = value; }

    void Awake()
    {
        if (ready) return;
        
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1;
        rb.freezeRotation = true;
        rb.linearDamping = 0.5f;
        rb.angularDamping = 0.5f;
        
        bc.size = new Vector2(1, 1);
        bc.offset = Vector2.zero;

        transform.position = startPos;
        ready = true;
    }

    void FixedUpdate()
    {
        if (isLinked)
        {
            ApplyChainPhysics();
            return;
        }
        
        if (transform.position.y < bottomBound)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;
            transform.position = new Vector3(startPos.x, topBound, 0);
        }
        else
        {
            CheckForLink();
        }
    }

    void ApplyChainPhysics()
    {
        if (joint != null && joint.connectedBody != null)
        {
            Vector2 anchorPos = transform.TransformPoint(joint.anchor);
            Vector2 connectedPos = joint.connectedBody.transform.TransformPoint(joint.connectedAnchor);
            Vector2 toConnection = connectedPos - anchorPos;
            
            float distance = toConnection.magnitude;
            float targetDistance = 1f;
            
            if (Mathf.Abs(distance - targetDistance) > 0.1f)
            {
                Vector2 correction = toConnection.normalized * (distance - targetDistance) * 2f;
                rb.AddForce(correction, ForceMode2D.Force);
            }
        }
    }

    void CheckForLink()
    {
        var players = FindObjectsByType<PlayerPawn>(FindObjectsSortMode.None);
        
        foreach (var p in players)
        {
            if (Vector2.Distance(transform.position, p.transform.position) < 1.5f)
            {
                ConnectTo(p);
                return;
            }
        }
    }

    void ConnectTo(PlayerPawn player)
    {
        if (isLinked || player == null) return;

        Rigidbody2D anchorRb = null;
        
        if (player.AttachedUnit == null)
        {
            anchorRb = player.GetComponent<Rigidbody2D>();
        }
        else
        {
            Unit lastUnit = player.AttachedUnit.GetComponent<Unit>();
            while (lastUnit != null && lastUnit.NextUnit != null)
            {
                lastUnit = lastUnit.NextUnit;
            }
            anchorRb = lastUnit?.GetComponent<Rigidbody2D>();
        }

        if (anchorRb == null) return;

        joint = gameObject.AddComponent<HingeJoint2D>();
        joint.connectedBody = anchorRb;
        
        Vector2 directionToAnchor = ((Vector2)anchorRb.transform.position - (Vector2)transform.position).normalized;
        joint.connectedAnchor = -directionToAnchor * 1f;
        joint.anchor = Vector2.zero;
        
        joint.useLimits = false;
        joint.useMotor = false;
        joint.enableCollision = false;

        isLinked = true;

        if (player.AttachedUnit == null)
        {
            player.AttachedUnit = gameObject;
        }
        else
        {
            Unit lastUnit = player.AttachedUnit.GetComponent<Unit>();
            while (lastUnit != null && lastUnit.NextUnit != null)
            {
                lastUnit = lastUnit.NextUnit;
            }
            if (lastUnit != null)
            {
                lastUnit.NextUnit = this;
            }
        }
        
        Debug.Log("Linked: " + name);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isLinked) return;
        
        PlayerPawn p = other.GetComponent<PlayerPawn>();
        if (p != null) ConnectTo(p);
    }
}