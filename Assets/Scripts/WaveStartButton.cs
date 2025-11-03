using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class WaveStartButton : MonoBehaviour
{
    [Header("Interaction")]
    [Tooltip("Tag of the player object that can press this button")]
    public string playerTag = "Player";
    [Tooltip("Key to press to start the next wave")]
    public KeyCode interactKey = KeyCode.E;
    [Tooltip("Allow starting the wave by shooting the button")]
    public bool triggerOnBulletHit = false;

    [Header("References")]
    [Tooltip("Drag your WaveSpawner (tagged WaveLogic) or leave empty to auto-find")]
    public WaveSpawner waveSpawner;
    [Tooltip("Optional: a world-space TMP/Text to show instructions")]
    public GameObject prompt; // enable/disable when player is in range

    [Header("Feedback")]
    public float pressSquash = 0.9f;
    public float squashTime = 0.07f;
    public AudioSource pressSfx;

    private bool playerInRange;
    private bool isAnimating;

    void Awake()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true; // important for proximity detection
    }

    void Start()
    {
        if (!waveSpawner)
        {
            var spawnerObj = GameObject.FindGameObjectWithTag("WaveLogic");
            if (spawnerObj) waveSpawner = spawnerObj.GetComponent<WaveSpawner>();
        }
        if (!waveSpawner) Debug.LogWarning("[WaveStartButton] No WaveSpawner found. Tag it 'WaveLogic' or assign reference.");

        if (prompt) prompt.SetActive(false);
    }

    void Update()
    {
        if (!playerInRange) return;
        if (!waveSpawner) return;

        // show hint only when in break
        if (prompt) prompt.SetActive(waveSpawner.IsInBreak);

        if (Input.GetKeyDown(interactKey))
        {
            TryStartWave();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = true;
            if (prompt && waveSpawner) prompt.SetActive(waveSpawner.IsInBreak);
        }
        else if (triggerOnBulletHit && other.CompareTag("Bullet"))
        {
            TryStartWave();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
            if (prompt) prompt.SetActive(false);
        }
    }

    private void TryStartWave()
    {
        if (!waveSpawner) return;

        // Only allow if the spawner is in break
        // (If you didn't add IsInBreak property, you can just call StartNextWave() unconditionally)
        #if UNITY_EDITOR
        if (!waveSpawner.IsInBreak)
        {
            Debug.Log("[WaveStartButton] Wave is already running.");
            return;
        }
        #endif

        waveSpawner.StartNextWave();
        PlayPressFx();
        if (prompt) prompt.SetActive(false);
    }

    private void PlayPressFx()
    {
        if (pressSfx) pressSfx.Play();
        if (!isAnimating) StartCoroutine(SquashAnim());
    }

    System.Collections.IEnumerator SquashAnim()
    {
        isAnimating = true;
        var t = 0f;
        var start = transform.localScale;
        var down = new Vector3(start.x * pressSquash, start.y * pressSquash, start.z);

        // squash
        while (t < squashTime)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(start, down, t / squashTime);
            yield return null;
        }

        // unsquash
        t = 0f;
        while (t < squashTime)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(down, start, t / squashTime);
            yield return null;
        }
        transform.localScale = start;
        isAnimating = false;
    }
}
