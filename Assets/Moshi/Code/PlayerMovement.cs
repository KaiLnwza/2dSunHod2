using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement & Jump")]
    public float moveSpeed = 8f;
    public float jumpForce = 14f;
    public int maxJumps = 1;

    [Header("Dash (หลบบอส)")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.8f;

    [Header("Attack (ฟันดาบ)")]
    public Transform attackPoint; // จุดศูนย์กลางวงกลมฟันดาบข้างหน้าตัวละคร
    public float attackRange = 0.8f;
    public int attackDamage = 20;
    public LayerMask enemyLayers; // Layer ของบอส/ศัตรู
    public float attackCooldown = 0.35f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Animation")]
    public Animator animator;

    private Rigidbody2D rb;
    private Collider2D[] playerColliders;
    private float moveInput;
    private bool isGrounded;
    private int jumpsLeft;
    private GameObject currentOneWayPlatform;

    // สถานะ Dash, ย่อ, โจมตี
    private bool isDashing;
    private bool canDash = true;
    private bool isCrouching;
    private float nextAttackTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerColliders = GetComponentsInChildren<Collider2D>();
        jumpsLeft = maxJumps;
    }

    void Update()
    {
        // ถ้ากำลัง Dash อยู่ ให้ล็อกการควบคุมอื่นชั่วคราว
        if (isDashing) return;

        // 1. รับค่าเดินซ้าย-ขวา
        moveInput = Input.GetAxisRaw("Horizontal");

        // เช็กพื้น
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        if (isGrounded && rb.linearVelocity.y <= 0.1f)
        {
            jumpsLeft = maxJumps;
        }

        // 2. ระบบย่อตัว (กด S หรือ ลูกศรลง ค้างไว้ตอนอยู่บนพื้น)
        if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && isGrounded)
        {
            isCrouching = true;
            moveInput = 0f; // ย่อตัวอยู่กับที่ (หากอยากให้คลานได้ให้ลบบรรทัดนี้ออก)
        }
        else
        {
            isCrouching = false;
        }

        // หันหน้าตัวละคร (ห้ามกลับด้านตอนย่อตัว)
        if (!isCrouching)
        {
            if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
            else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);
        }

        // 3. กดกระโดด หรือ กดลงทะลุ Platform (ถ้ากด S ค้างแล้วกด Spacebar จะร่วงทะลุพื้น)
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))
        {
            if (isCrouching && currentOneWayPlatform != null)
            {
                StartCoroutine(DisableCollision());
            }
            else if (!isCrouching && jumpsLeft > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                jumpsLeft--;
            }
        }

        // 4. กด Dash หลบบอส (ปุ่ม Left Shift หรือ K)
        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.K)) && canDash && !isCrouching)
        {
            StartCoroutine(Dash());
        }

        // 5. กดฟันดาบ (ปุ่ม J หรือ คลิกเมาส์ซ้าย)
        if ((Input.GetKeyDown(KeyCode.J) || Input.GetMouseButtonDown(0)) && Time.time >= nextAttackTime && !isCrouching)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }

        // 6. ส่งค่าไปควบคุม Animator
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(moveInput));
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetFloat("VerticalVelocity", rb.linearVelocity.y);
            animator.SetBool("IsCrouching", isCrouching);
        }
    }

    void FixedUpdate()
    {
        if (isDashing) return;
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    private void Attack()
    {
        if (animator != null) animator.SetTrigger("Attack");

        if (attackPoint != null)
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
            foreach (Collider2D enemy in hitEnemies)
            {
                // เรียกใช้ BossHealth ของเพื่อนเพื่อลดเลือดบอส
                BossHealth boss = enemy.GetComponent<BossHealth>();
                if (boss != null)
                {
                    boss.TakeDamage(attackDamage);
                }
            }
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f; // ปิดแรงโน้มถ่วงตอนพุ่งตัว
        rb.linearVelocity = new Vector2(transform.localScale.x * dashSpeed, 0f);

        if (animator != null) animator.SetTrigger("Dash");

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("OneWayPlatform"))
            currentOneWayPlatform = collision.gameObject;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("OneWayPlatform"))
            currentOneWayPlatform = null;
    }

    private IEnumerator DisableCollision()
    {
        Collider2D platformCollider = currentOneWayPlatform.GetComponent<Collider2D>();
        foreach (Collider2D col in playerColliders) Physics2D.IgnoreCollision(col, platformCollider, true);
        yield return new WaitForSeconds(0.25f);
        foreach (Collider2D col in playerColliders) Physics2D.IgnoreCollision(col, platformCollider, false);
    }

    // วาดวงกลมระยะฟันดาบในหน้าจอ Scene
    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
    public bool IsDashing()
    {
        return isDashing;
    }
}