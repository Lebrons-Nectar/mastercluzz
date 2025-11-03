using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour
{
    [Header("Enemy Prefabs (assign both)")]
    public GameObject enemyPrefabA;
    public GameObject enemyPrefabB;

    [Header("Difficulty / Wave Size")]
    [Tooltip("Enemies in wave 1.")]
    public int baseEnemies = 6;
    [Tooltip("How many more enemies added per wave.")]
    public float enemiesPerWave = 2.0f;
    [Tooltip("Random extra enemies each wave (0..variance).")]
    public int variance = 3;
    [Tooltip("Chance for prefab B versus A (0=all A, 1=all B).")]
    [Range(0f, 1f)] public float prefabBChance = 0.5f;

    [Header("Spawn Settings")]
    [Tooltip("Drag your spawn point Transforms here (2–8+ around the map).")]
    public List<Transform> spawnPoints = new List<Transform>();
    [Tooltip("If ON, picks a random spawn point each time; otherwise cycles in order.")]
    public bool randomizeSpawnPoint = true;

    [Header("Wave Timing")]
    [Tooltip("Used only to pace spawns inside a wave. Wave ends when timer <= 0 AND all enemies are dead.")]
    public int waveDuration = 12;

    [Header("Break UI (optional)")]
    public GameObject breakPanel;   // panel with a "Start Wave" button (optional)
    public Text breakLabel;         // e.g., "Wave 3 — press Start" (optional)
    public Button startWaveButton;  // hook its OnClick to StartNextWave() (optional)

    [Header("Wave Label (optional)")]
    public Text currentWaveText;

    [Header("References")]
    public LogicManager logicManager; // optional; tagged "Logic"

    [Header("Debug Info (read-only at runtime)")]
    public List<GameObject> spawnedEnemies = new List<GameObject>();  // alive this wave

    // ---- internals ----
    public int currWave = 0; // we start at 0; pressing StartNextWave() moves to 1
    private List<GameObject> enemiesToSpawn = new List<GameObject>();
    private float waveTimer;
    private float spawnTimer;
    private float spawnInterval;
    private int nextSpawnIndex;

    private enum SpawnerState { Break, Spawning, WaitingClear }
    private SpawnerState state = SpawnerState.Break;

    [Header("Input (optional)")]
    public bool allowKeyboardStart = true;
    public KeyCode startKey = KeyCode.E; // physical button script can also call StartNextWave()

    public bool IsInBreak => state == SpawnerState.Break;

    void Start()
    {
        if (!logicManager)
        {
            var logic = GameObject.FindGameObjectWithTag("Logic");
            if (logic) logicManager = logic.GetComponent<LogicManager>();
        }

        if (!enemyPrefabA || !enemyPrefabB)
        {
            Debug.LogError("[WaveSpawner] Assign both enemyPrefabA and enemyPrefabB.");
            enabled = false; return;
        }
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogError("[WaveSpawner] No spawn points set. Drag Transforms into 'spawnPoints'.");
            enabled = false; return;
        }

        EnterBreak(); // start paused until player starts the wave
    }

    void Update()
    {
        if (state == SpawnerState.Break && allowKeyboardStart && Input.GetKeyDown(startKey))
            StartNextWave();
    }

    void FixedUpdate()
    {
        // UI label
        if (currentWaveText)
        {
            if (state == SpawnerState.Break)
                currentWaveText.text = $"Wave {Mathf.Max(1, currWave + 1)} (waiting)";
            else
                currentWaveText.text = $"Wave {currWave}";
        }

        if (state == SpawnerState.Break) return;

        // Spawning cadence
        if (state == SpawnerState.Spawning)
        {
            if (spawnTimer <= 0f)
            {
                if (enemiesToSpawn.Count > 0)
                {
                    SpawnEnemy();
                    spawnTimer = spawnInterval;
                }
                else
                {
                    state = SpawnerState.WaitingClear; // all queued enemies spawned
                }
            }
            else
            {
                spawnTimer -= Time.fixedDeltaTime;
            }
        }

        // Wave pacing
        if (waveTimer > 0f) waveTimer -= Time.fixedDeltaTime;

        // End wave when timer elapsed AND all alive are gone
        if (waveTimer <= 0f && spawnedEnemies.Count == 0 && state != SpawnerState.Break)
        {
            if (logicManager) logicManager.EndWave();
            EnterBreak(); // infinite break until player starts again
        }
    }

    // === PUBLIC: call from UI button or your physical stand script ===
    public void StartNextWave()
    {
        if (!IsInBreak) return; // ignore if already running
        currWave++;
        GenerateWave();
        ExitBreak();
    }

    // --- helpers ---
    private void EnterBreak()
    {
        state = SpawnerState.Break;
        enemiesToSpawn.Clear();
        spawnedEnemies.Clear();
        spawnTimer = 0f;
        waveTimer = 0f;

        if (breakPanel) breakPanel.SetActive(true);
        if (breakLabel) breakLabel.text = $"Wave {Mathf.Max(1, currWave + 1)} — press Start";
        if (startWaveButton) startWaveButton.interactable = true;
    }

    private void ExitBreak()
    {
        state = SpawnerState.Spawning;
        if (breakPanel) breakPanel.SetActive(false);
    }

    private void GenerateWave()
    {
        // total enemies this wave = base + linear growth + small randomness
        int extra = (variance > 0) ? Random.Range(0, variance + 1) : 0;
        int totalThisWave = Mathf.Max(1, Mathf.RoundToInt(baseEnemies + enemiesPerWave * (currWave - 1)) + extra);

        enemiesToSpawn.Clear();
        spawnedEnemies.Clear();

        // random mix of A/B
        for (int i = 0; i < totalThisWave; i++)
        {
            bool pickB = Random.value < prefabBChance;
            enemiesToSpawn.Add(pickB ? enemyPrefabB : enemyPrefabA);
        }

        // shuffle (Fisher–Yates)
        for (int i = enemiesToSpawn.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (enemiesToSpawn[i], enemiesToSpawn[j]) = (enemiesToSpawn[j], enemiesToSpawn[i]);
        }

        spawnInterval = Mathf.Max(0.05f, (float)waveDuration / Mathf.Max(1, enemiesToSpawn.Count));
        waveTimer = waveDuration;
        spawnTimer = 0f;

        Debug.Log($"[WaveSpawner] Wave {currWave}: total={totalThisWave}, points={spawnPoints.Count}, interval={spawnInterval:0.00}s");
    }

    private void SpawnEnemy()
    {
        Transform point = randomizeSpawnPoint
            ? spawnPoints[Random.Range(0, spawnPoints.Count)]
            : spawnPoints[(nextSpawnIndex++) % spawnPoints.Count];

        var prefab = enemiesToSpawn[0];
        enemiesToSpawn.RemoveAt(0);

        var enemy = Instantiate(prefab, point.position, Quaternion.identity);
        spawnedEnemies.Add(enemy);
    }

    // called by enemies when they die/despawn
    public void EnterNameHere(GameObject enemyInstance)
    {
        if (!enemyInstance) return;
        spawnedEnemies.Remove(enemyInstance);
    }

    // legacy fallback (if something still calls without parameter)
    public void EnterNameHere()
    {
        if (spawnedEnemies.Count > 0) spawnedEnemies.RemoveAt(0);
    }
}
