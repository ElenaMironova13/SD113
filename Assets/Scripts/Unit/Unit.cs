using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2 startPos = new Vector2(0, 10);
    [SerializeField] private float topBound = 10f;
    [SerializeField] private float bottomBound = -10f;
    [SerializeField] private bool isLinked = false;

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
        
        bc.size = new Vector2(1, 1);
        bc.offset = Vector2.zero;
        bc.isTrigger = true;

        transform.position = startPos;
        ready = true;
    }

    void FixedUpdate()
    {
        if (isLinked) return;
        
        if (transform.position.y < bottomBound)
        {
            rb.linearVelocity = Vector2.zero;
            transform.position = new Vector3(startPos.x, topBound, 0);
        }
        else
        {
            CheckLink();
        }
    }

    void CheckLink()
    {
        var players = FindObjectsByType<PlayerPawn>(FindObjectsSortMode.None);
        
        foreach (var p in players)
        {
            float dist = Vector2.Distance(transform.position, p.transform.position);
            Debug.Log("Distance to player: " + dist);
            
            if (dist < 1.5f)
            {
                AttachToPlayer(p);
                return;
            }
        }
    }

    void AttachToPlayer(PlayerPawn player)
    {
        if (isLinked || player == null) return;

        Debug.Log("Attempting to link to: " + player.name);

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        
        isLinked = true;

        if (player.AttachedUnit == null)
        {
            player.AttachedUnit = gameObject;
            Debug.Log("First unit linked");
        }
        else
        {
            Unit u = player.AttachedUnit.GetComponent<Unit>();
            while (u != null && u.NextUnit != null)
            {
                u = u.NextUnit;
            }
            if (u != null) 
            {
                u.NextUnit = this;
                Debug.Log("Added to chain");
            }
        }
        
        Debug.Log("Linked: " + name);
    }
}