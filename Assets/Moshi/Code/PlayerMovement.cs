using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement & Jump")]
    public float moveSpeed = 8f;
    public float jumpForce = 14f;
    public int maxJumps = 2;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float checkRadius = 0.25f;
    public LayerMask groundLayer;
    public LayerMask oneWayPlatformLayer; // ช่องใหม่สำหรับเลือก Layer พื้นลอยโดยเฉพาะ

    [Header("Animation")]
    public Animator animator;

    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private float moveInput;
    private bool isGrounded;
    private int jumpsLeft;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        jumpsLeft = maxJumps;
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        if (isGrounded && rb.linearVelocity.y <= 0.1f)
        {
            jumpsLeft = maxJumps;
        }

        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) && jumpsLeft > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpsLeft--;
        }

        // เช็กว่าใต้เท้ามี OneWayPlatform อยู่ไหม เมื่อกด S หรือ ลูกศรลง
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            Collider2D platform = Physics2D.OverlapCircle(groundCheck.position, checkRadius, oneWayPlatformLayer);
            if (platform != null)
            {
                StartCoroutine(DisableCollision(platform));
            }
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(moveInput));
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetFloat("VerticalVelocity", rb.linearVelocity.y);
        }

        // ปล่อยปุ่มกระโดดกลางอากาศ ให้ความเร็วพุ่งขึ้นลดลงครึ่งหนึ่ง (โดดเตี้ยลง)
        if ((Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.Space)) && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    private IEnumerator DisableCollision(Collider2D platformCollider)
    {
        Physics2D.IgnoreCollision(playerCollider, platformCollider, true);
        yield return new WaitForSeconds(0.35f); // เพิ่มเวลาเป็น 0.35 วินาทีให้ร่วงพ้นตัวพื้นแน่นอน
        Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    }
}