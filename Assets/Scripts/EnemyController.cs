using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 4.0f;
    private float currentSpeed;
    public Transform target;               // Hive

    [Header("Health Scaling")]
    public float baseHealth = 5f;          // HP on waves 1–wavesPerStep
    public float extraHealthPerStep = 2f;  // extra HP every step
    public int wavesPerStep = 10;          // every X waves they get tankier

    [Header("References")]
    public GameObject enemy;               // usually this gameObject
    public WaveSpawner WS;                 // set via tag "WaveLogic" if left empty
    public GameObject damageNumberPrefab;  // floating damage text prefab

    private float health;
    private bool isDead = false;
    private bool isAttacking = false;
    [SerializeField] private Animator animator;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    void Start()
    {
        if (!enemy) enemy = gameObject;

        // Find WaveSpawner
        if (!WS)
        {
            GameObject w = GameObject.FindGameObjectWithTag("WaveLogic");
            if (w) WS = w.GetComponent<WaveSpawner>();
        }

        // Find Target (Prioritize Player)
        if (!target)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj) target = playerObj.transform;
            else
            {
                // Fallback to Hive if Player is dead/missing?
                GameObject hiveObj = GameObject.FindGameObjectWithTag("Hive");
                if (hiveObj) target = hiveObj.transform;
            }
        }

        currentSpeed = speed;

        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!spriteRenderer) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
        if (!audioSource) audioSource = GetComponentInChildren<AudioSource>();

        // ---- HEALTH SCALING ----
        int currentWave = (WS != null) ? WS.currWave : 1;
        int step = Mathf.Max(0, (currentWave - 1) / Mathf.Max(1, wavesPerStep));
        health = baseHealth + step * extraHealthPerStep;
        // Debug.Log($"Normal enemy wave {currentWave}, HP = {health}");
    }

    void Update()
    {
        if (isDead || !target) return;

        // Move toward hive
        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            currentSpeed * Time.deltaTime
        );

        // keep z constant
        transform.position = new Vector3(transform.position.x, transform.position.y, 1f);

        // Flip Sprite
        if (spriteRenderer)
        {
            // If target is to the left, flip (assuming sprite faces right by default)
            spriteRenderer.flipX = target.position.x < transform.position.x;
        }
    }

    // ---------- COLLISION ----------
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
            // default bullet dmg = 1
            float dmg = 1f;

            // Try to read a public float "damage" from BulletScript, if it exists
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

            // optional: destroy bullet on hit
            Destroy(other.gameObject);
        }
    }

    // Called by traps / grenade / barbed wire via SendMessage("TakeDamage", float)
    public void TakeDamage(float dmg)
    {
        if (isDead) return;

        health -= dmg;

        // Floating damage number
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

        // notify spawner
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

        // If we didn't die in the callback (like attacking Hive), resume? 
        // Usually destroying hive is end of game or massive event, so maybe doesn't matter.
        // But if player dodges? (Collider logic happens on Enter, so dodging impossible once entered).
        
        if (!isDead)
        {
            isAttacking = false;
            currentSpeed = speed;
        }
    }
}
