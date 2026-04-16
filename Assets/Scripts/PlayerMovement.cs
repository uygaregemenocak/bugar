using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 4f;
    public float jumpForce = 8f;
    public float attackCooldown = 0.5f;
    public int maxHealth = 100;
    public Transform attackPoint;
    public float attackRange = 0.8f;
    public int attackDamage = 10;
    public float attackHitDelay = 0.15f;
    public LayerMask enemyLayer;

    public float deathYLimit = -10f;
    public float restartDelay = 1f;
    public string restartSceneName = "MainScene";

    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded;
    private float moveInput;
    private float nextAttackTime;
    private bool isAttacking;
    private int currentHealth;
    private bool isDead;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isDead) return;

        if (transform.position.y <= deathYLimit)
        {
            Die();
            return;
        }

        moveInput = Input.GetAxisRaw("Horizontal");

        bool isMoving = Mathf.Abs(moveInput) > 0.01f;
        bool isFalling = rb.linearVelocity.y < -0.1f && !isGrounded;

        animator.SetBool("isRunning", isMoving);
        animator.SetBool("isJumping", !isGrounded && !isFalling);
        animator.SetBool("isFalling", isFalling);
        animator.speed = 0.25f;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        if ((Input.GetKeyDown(KeyCode.J) || Input.GetMouseButtonDown(0)) && Time.time >= nextAttackTime)
        {
            isAttacking = true;
            animator.ResetTrigger("attack");
            animator.SetTrigger("attack");
            CancelInvoke(nameof(AttackEnemies));
            CancelInvoke(nameof(EndAttack));
            Invoke(nameof(AttackEnemies), attackHitDelay);
            nextAttackTime = Time.time + attackCooldown;
            Invoke(nameof(EndAttack), attackCooldown);
        }

        if (moveInput > 0)
        {
            transform.localScale = new Vector3(2, 2, 1);
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(-2, 2, 1);
        }
    }

    void FixedUpdate()
    {
        if (isDead)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float currentSpeed = walkSpeed;
        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);
    }

    void EndAttack()
    {
        isAttacking = false;
    }

    void AttackEnemies()
    {
        if (attackPoint == null) return;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            Enemy enemy = enemyCollider.GetComponentInParent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        TakeDamage(damage, Vector2.zero, 0f);
    }

    public void TakeDamage(int damage, Vector2 knockbackDirection, float knockbackForce)
    {
        if (isDead) return;

        currentHealth -= damage;
        animator.SetTrigger("takeHit");

        if (knockbackForce > 0f)
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(new Vector2(knockbackDirection.x, 0.6f).normalized * knockbackForce, ForceMode2D.Impulse);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("death");
        Invoke(nameof(RestartGame), restartDelay);
    }

    void RestartGame()
    {
        SceneManager.LoadScene(restartSceneName);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void OnDrawGizmos()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}