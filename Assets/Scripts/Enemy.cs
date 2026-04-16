using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 30;
    public int touchDamage = 20;
    public float knockbackForce = 8f;
    public float damageCooldown = 1f;
    public float destroyDelay = 0.6f;
    public float moveSpeed = 2f;
    public float detectionRange = 8f;
    public bool spriteFacesRight = true;

    private int currentHealth;
    private Animator animator;
    private Collider2D enemyCollider;
    private bool isDead;
    private float nextDamageTime;
    private Rigidbody2D rb;
    private Transform player;
    private Vector3 startScale;
    private static int aliveEnemyCount;
    private static int countedSceneHandle = -1;
    void Awake()
    {
        int currentSceneHandle = SceneManager.GetActiveScene().handle;

        if (countedSceneHandle != currentSceneHandle)
        {
            countedSceneHandle = currentSceneHandle;
            aliveEnemyCount = 0;
            GameManager.monstersKilled = false;
        }

        aliveEnemyCount++;
    }

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        enemyCollider = GetComponent<Collider2D>();

        rb = GetComponent<Rigidbody2D>();
        startScale = transform.localScale;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void FixedUpdate()
    {
        if (isDead) return;
        if (rb == null) return;
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            float direction = Mathf.Sign(player.position.x - transform.position.x);
            rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
            SetRunning(true);
            FaceDirection(direction);
        }
        else
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            SetRunning(false);
        }
    }

    void FaceDirection(float direction)
    {
        if (direction == 0f) return;

        float xScale = Mathf.Abs(startScale.x);

        if (direction > 0f)
        {
            xScale = spriteFacesRight ? xScale : -xScale;
        }
        else
        {
            xScale = spriteFacesRight ? -xScale : xScale;
        }

        transform.localScale = new Vector3(xScale, startScale.y, startScale.z);
    }

    void SetRunning(bool running)
    {
        if (animator == null) return;

        animator.SetBool("isRunning", running);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (animator != null)
        {
            animator.SetTrigger("takeHit");
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
        aliveEnemyCount--;

        if (aliveEnemyCount <= 0)
        {
            aliveEnemyCount = 0;
            GameManager.monstersKilled = true;
        }
        SetRunning(false);

        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }

        if (animator != null)
        {
            animator.SetTrigger("death");
        }

        Destroy(gameObject, destroyDelay);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        TryDamagePlayer(collision.gameObject);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        TryDamagePlayer(collision.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        TryDamagePlayer(other.gameObject);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        TryDamagePlayer(other.gameObject);
    }

    void TryDamagePlayer(GameObject other)
    {
        if (isDead) return;
        if (Time.time < nextDamageTime) return;

        PlayerMovement player = other.GetComponent<PlayerMovement>();

        if (player == null) return;

        Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;
        player.TakeDamage(touchDamage, knockbackDirection, knockbackForce);
        nextDamageTime = Time.time + damageCooldown;
    }
}