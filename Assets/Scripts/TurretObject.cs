using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class TurretObject : MonoBehaviour
{
    [Header("Targeting")]
    [SerializeField] private float range = 5f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private Transform firePoint;

    [Header("Projectile")]
    [SerializeField] private GameObject bulletPrefab;

    [Header("Ammo / Reload (uses PlayerValues.Honey)")]
    [SerializeField] private int magazineSize = 50;      // bullets per magazine
    [SerializeField] private int reloadCostHoney = 25;    // price to reload when empty
    [SerializeField] private KeyCode reloadKey = KeyCode.E;
    [SerializeField] private string playerTag = "Player";

    [Header("Prompt (optional)")]
    [Tooltip("Child UI object (Canvas/Text) shown when turret is empty and player is nearby.")]
    [SerializeField] private GameObject promptObject; // must be a CHILD, not the turret root
    [Tooltip("Legacy Text component for the prompt. Use TMP_Text instead if you use TextMeshPro.")]
    [SerializeField] private Text promptText;

    // Runtime state
    public int Ammo { get; private set; }
    public bool OutOfAmmo => Ammo <= 0;

    private float fireCooldown;
    private bool playerInRange;
    private PlayerValues playerValues;

    void Awake()
    {
        // This collider is your “interaction radius”
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void Start()
    {
        Ammo = magazineSize;

        // Find PlayerValues (prefer tag "Logic")
        var logic = GameObject.FindGameObjectWithTag("Logic");
        if (logic) playerValues = logic.GetComponent<PlayerValues>();
        if (!playerValues) playerValues = FindObjectOfType<PlayerValues>();
        if (!playerValues)
            Debug.LogWarning("[TurretObject] No PlayerValues found. Put PlayerValues in scene (tag 'Logic' recommended).");

        // Safety: do not allow promptObject to be the turret root
        if (promptObject == gameObject)
        {
            Debug.LogError("[TurretObject] 'Prompt Object' must be a CHILD (Canvas/Text), not the turret root. Clearing.");
            promptObject = null;
        }

        // Auto-find a child named "ReloadPrompt" if none assigned
        if (!promptObject)
        {
            var child = transform.Find("ReloadPrompt");
            if (child) promptObject = child.gameObject;
        }

        // Ensure it’s a child
        if (promptObject && !promptObject.transform.IsChildOf(transform))
        {
            Debug.LogError("[TurretObject] 'Prompt Object' should be a CHILD of the turret. Clearing to avoid disabling the turret.");
            promptObject = null;
        }

        if (promptObject) promptObject.SetActive(false);
        UpdatePrompt();
    }

    void Update()
    {
        // Handle paid reload ONLY when empty
        if (playerInRange && Input.GetKeyDown(reloadKey))
        {
            TryReload(); // will only reload when Ammo == 0 and enough Honey
        }

        UpdatePrompt();

        // Firing logic
        fireCooldown -= Time.deltaTime;
        if (OutOfAmmo) return; // stop firing at 0

        var target = FindNearestEnemy();
        if (!target) return;

        if (fireCooldown <= 0f)
        {
            FireAtTarget(target.transform.position);
            fireCooldown = 1f / Mathf.Max(0.01f, fireRate);
        }
    }

    private GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float minDist = Mathf.Infinity;
        Vector3 pos = transform.position;

        foreach (var e in enemies)
        {
            if (!e) continue;
            float d = Vector2.Distance(pos, e.transform.position);
            if (d <= range && d < minDist)
            {
                minDist = d;
                closest = e;
            }
        }
        return closest;
    }

    private void FireAtTarget(Vector3 targetPos)
    {
        if (!bulletPrefab || !firePoint) return;
        if (OutOfAmmo) return;

        Vector3 dir = (targetPos - firePoint.position).normalized;

        var bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        var bs = bullet.GetComponent<BulletScript>();
        if (bs != null) bs.overrideDirection = dir;

        Ammo = Mathf.Max(0, Ammo - 1); // consume ammo so it can't shoot forever
    }

    /// <summary>
    /// Reload only when the magazine is completely empty (Ammo == 0).
    /// Spends Honey; returns true if reloaded to full.
    /// </summary>
    public bool TryReload()
    {
        if (!OutOfAmmo) return false;           // NOT allowed unless empty
        if (!playerValues)
        {
            Debug.LogWarning("[TurretObject] No PlayerValues -> cannot buy ammo.");
            return false;
        }
        if (playerValues.Honey < reloadCostHoney) return false; // must have money

        playerValues.Honey -= reloadCostHoney;
        Ammo = magazineSize; // full mag
        UpdatePrompt();
        return true;
    }

    private void UpdatePrompt()
    {
        if (!promptObject && !promptText) return;

        // Show prompt only if:
        // - Player is in range
        // - Turret is empty (Ammo == 0)
        bool show = playerInRange && OutOfAmmo;

        if (promptObject) promptObject.SetActive(show);

        if (promptText)
        {
            if (show)
            {
                string cost = playerValues ? reloadCostHoney.ToString() : "?";
                // Example: "Reload (E) – Honey: 25"
                promptText.text = $"Reload ({reloadKey}) – Honey: {cost}";
            }
            else
            {
                // Clear when not needed
                if (promptText.text.Length > 0) promptText.text = "";
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = true;
            UpdatePrompt();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
            if (promptObject) promptObject.SetActive(false);
            if (promptText) promptText.text = "";
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
