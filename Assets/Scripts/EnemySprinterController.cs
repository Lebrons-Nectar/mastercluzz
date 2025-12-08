using UnityEngine;
using System.Collections;

public class EnemySprinterController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 6.0f;
    private float currentSpeed;
    public Transform target;

    [Header("Health Scaling")]
    public float baseHealth = 3f;
    public float extraHealthPerStep = 1.5f;
    public int wavesPerStep = 10;

    [Header("References")]
    public GameObject enemy;
    public WaveSpawner WS;
    public GameObject damageNumberPrefab;

    private float health;
    private bool isDead = false;
    private bool isAttacking = false;
    [SerializeField] private Animator animator;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    void Start()
    {
        if (!enemy) enemy = gameObject;

        // Wave spawner
        if (!WS)
        {
            GameObject w = GameObject.FindGameObjectWithTag("WaveLogic");
            if (w) WS = w.GetComponent<WaveSpawner>();
        }

        // Target Hive First (Priority 1)
        if (!target)
        {
            GameObject hive = GameObject.FindGameObjectWithTag("Hive");
            if (hive) target = hive.transform;
            // Fallback to Player (Priority 2)
            else
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player) target = player.transform;
            }
        }

        currentSpeed = speed;

        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!spriteRenderer) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
        audioSource = GetComponent<AudioSource>();

        // ---- HEALTH SCALING ----
        int currentWave = (WS != null) ? WS.currWave : 1;
        int step = Mathf.Max(0, (currentWave - 1) / Mathf.Max(1, wavesPerStep));
        health = baseHealth + step * extraHealthPerStep;
    }

    void Update()
    {
        if (!target)
        {
            GameObject hive = GameObject.FindGameObjectWithTag("Hive");
            if (hive) target = hive.transform;
            else
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player) target = player.transform;
            }
        }

        if (isDead || !target) return;

        // Move
        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            currentSpeed * Time.deltaTime
        );

        transform.position = new Vector3(transform.position.x, transform.position.y, 1f);

        // Flip Sprite
        if (spriteRenderer)
        {
            // If target is to the left, flip (assuming sprite faces right by default)
            spriteRenderer.flipX = target.position.x < transform.position.x;
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead || isAttacking) return;
        if (other.CompareTag("Hive"))
        {
            // Trigger attack on Hive
            if (other.TryGetComponent<HiveUpgrade>(out var hive))
            {
                StartCoroutine(AttackRoutine(() => hive.DestroyHive()));
            }
        }
        if (other.CompareTag("Player"))
        {
            // Trigger attack on Player
            StartCoroutine(AttackRoutine(() => 
            {
                CharacterMovement.playerHealth--;
                Die();
            }));
        }
        else if (other.CompareTag("Bullet"))
        {
            float dmg = 1f;

            var bs = other.GetComponent<BulletScript>();
            if (bs != null)
            {
                try
                {
                    var field = bs.GetType().GetField("damage");
                    if (field != null && field.FieldType == typeof(float))
                    {
                        dmg = (float)field.GetValue(bs);
                    }
                }
                catch { }
            }

            TakeDamage(dmg);
            Destroy(other.gameObject);
        }
    }

    // Called by traps / grenade / barbed wire via SendMessage("TakeDamage", float)
    public void TakeDamage(float dmg)
    {
        if (isDead) return;

        health -= dmg;

        if (damageNumberPrefab)
        {
            GameObject go = Instantiate(damageNumberPrefab, transform.position, Quaternion.identity);
            var dn = go.GetComponent<DamageNumber>();
            if (dn != null) dn.Init(dmg);
        }

        if (health <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // Stop flying sound
        if (audioSource) audioSource.Stop();

        if (WS != null)
        {
            WS.EnterNameHere(gameObject);
        }

        Destroy(enemy != null ? enemy : gameObject);
    }

    // ---------- SLOW FOR BARBED WIRE ----------
    public void SetSlowed(float slowMultiplier)
    {
        currentSpeed = speed * slowMultiplier;
    }

    public void RemoveSlow()
    {
        if (!isAttacking) currentSpeed = speed;
    }

    private IEnumerator AttackRoutine(System.Action onComplete)
    {
        isAttacking = true;
        currentSpeed = 0f; // Stop moving
        
        if (animator) animator.SetTrigger("Attack");

        yield return new WaitForSeconds(0.5f); // Wait for animation duration

        onComplete?.Invoke();
        
        if (!isDead)
        {
            isAttacking = false;
            currentSpeed = speed;
        }
    }
}
