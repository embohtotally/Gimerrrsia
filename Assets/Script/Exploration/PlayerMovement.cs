using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Connections")]
    public BattleSystem battleSystem;

    // Private variables
    [SerializeField] Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;
    private float lastMoveX;
    //private float lastMoveY;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (rb.simulated == false) rb.simulated = true;
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        //movement.y = Input.GetAxisRaw("Vertical");

        if (animator != null)
        {
            animator.SetFloat("Speed", movement.sqrMagnitude);

            if (movement.sqrMagnitude > 0.01f)
            {
                lastMoveX = movement.x;
                //lastMoveY = movement.y;
                animator.SetFloat("Horizontal", movement.x);
                //animator.SetFloat("Vertical", movement.y);
            }
            else
            {
                animator.SetFloat("Horizontal", lastMoveX);
                //animator.SetFloat("Vertical", lastMoveY);
            }
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    // This is now much simpler
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Manifestation manifestation = other.GetComponent<Manifestation>();
            if (manifestation != null && manifestation.battlePrefab != null)
            {
                this.enabled = false; // Stop moving
                StartCoroutine(battleSystem.StartBattle(manifestation.battlePrefab, this));
                Destroy(other.gameObject, 3f);
            }
        }
    }
}