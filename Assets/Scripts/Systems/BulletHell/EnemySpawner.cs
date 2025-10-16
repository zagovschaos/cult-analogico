using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public List<EnemyGroup> enemyGroups;
        public float spawnInterval = 2f;
        [HideInInspector] public int waveQuota;
        [HideInInspector] public int spawnCount;
    }

    [System.Serializable]
    public class EnemyGroup
    {
        public GameObject enemyPrefab;
        public int count = 5;
        [HideInInspector] public int spawnCount;
    }

    [Header("Wave Settings")]
    public List<Wave> waves = new List<Wave>(); // Initialize list
    [HideInInspector] public int currentWaveIndex = 0;

    [Header("Spawn Settings")]
    public List<Transform> spawnPoints = new List<Transform>(); // Initialize list
    public float waveInterval = 3f;

    [Header("Debug")]
    [SerializeField] private bool autoStart = true;
    [SerializeField] private bool showDebugLogs = true;

    private bool hasStarted = false;
    private float spawnTimer;
    private int enemiesAlive = 0;
    private int totalEnemiesKilled = 0;

    public static EnemySpawner Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Initialize all lists to prevent null references
        if (waves == null) waves = new List<Wave>();
        if (spawnPoints == null) spawnPoints = new List<Transform>();

        CalculateWaveQuota();

        if (autoStart)
        {
            StartSpawning();
        }
    }

    void Update()
    {
        if (hasStarted && currentWaveIndex < waves.Count && currentWaveIndex >= 0)
        {
            spawnTimer += Time.deltaTime;

            if (spawnTimer >= waves[currentWaveIndex].spawnInterval)
            {
                spawnTimer = 0f;
                SpawnEnemy();
            }
        }
    }

    void CalculateWaveQuota()
    {
        foreach (Wave wave in waves)
        {
            if (wave.enemyGroups == null)
            {
                wave.enemyGroups = new List<EnemyGroup>();
                continue;
            }

            wave.waveQuota = 0;
            foreach (EnemyGroup enemyGroup in wave.enemyGroups)
            {
                if (enemyGroup != null)
                {
                    wave.waveQuota += enemyGroup.count;
                }
            }
        }
    }

    public void StartSpawning()
    {
        if (!hasStarted && waves.Count > 0)
        {
            hasStarted = true;
            StartCoroutine(StartWave());
        }
        else if (waves.Count == 0)
        {
            Debug.LogError("No waves configured in EnemySpawner!");
        }
    }

    IEnumerator StartWave()
    {
        if (showDebugLogs) Debug.Log("Wave system started!");

        while (currentWaveIndex < waves.Count)
        {
            Wave currentWave = waves[currentWaveIndex];

            if (currentWave == null || currentWave.enemyGroups == null)
            {
                currentWaveIndex++;
                continue;
            }

            if (showDebugLogs) Debug.Log($"Starting Wave {currentWaveIndex + 1}: {currentWave.waveName}");

            yield return StartCoroutine(SpawnWave(currentWave));

            // Wait until all enemies are defeated
            yield return new WaitUntil(() => enemiesAlive <= 0);

            if (showDebugLogs) Debug.Log($"Wave {currentWaveIndex + 1} completed!");

            // Wait before next wave
            yield return new WaitForSeconds(waveInterval);

            currentWaveIndex++;

            if (currentWaveIndex >= waves.Count)
            {
                if (showDebugLogs) Debug.Log("All waves completed! You win!");
                break;
            }
        }
    }

    IEnumerator SpawnWave(Wave wave)
    {
        // Reset spawn counts
        wave.spawnCount = 0;
        foreach (EnemyGroup enemyGroup in wave.enemyGroups)
        {
            if (enemyGroup != null)
            {
                enemyGroup.spawnCount = 0;
            }
        }

        // Spawn enemies until wave quota is reached
        while (wave.spawnCount < wave.waveQuota)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(wave.spawnInterval);
        }
    }

    void SpawnEnemy()
    {
        if (currentWaveIndex >= waves.Count || waves[currentWaveIndex] == null) return;

        Wave currentWave = waves[currentWaveIndex];

        if (currentWave.spawnCount >= currentWave.waveQuota) return;

        // Get a random spawn point
        Transform spawnPoint = GetRandomSpawnPoint();
        if (spawnPoint == null)
        {
            Debug.LogWarning("No spawn points available!");
            return;
        }

        // Get a random enemy type that hasn't reached its count
        EnemyGroup enemyGroup = GetRandomEnemyGroup(currentWave);
        if (enemyGroup == null || enemyGroup.enemyPrefab == null)
        {
            Debug.LogWarning("No valid enemy group to spawn!");
            return;
        }

        // Spawn the enemy
        GameObject enemy = Instantiate(enemyGroup.enemyPrefab, spawnPoint.position, Quaternion.identity);
        enemyGroup.spawnCount++;
        currentWave.spawnCount++;
        enemiesAlive++;

        if (showDebugLogs)
        {
            Debug.Log($"Spawned {enemyGroup.enemyPrefab.name}. " +
                     $"Wave progress: {currentWave.spawnCount}/{currentWave.waveQuota}. " +
                     $"Enemies alive: {enemiesAlive}");
        }
    }

    EnemyGroup GetRandomEnemyGroup(Wave wave)
    {
        if (wave.enemyGroups == null) return null;

        List<EnemyGroup> availableGroups = new List<EnemyGroup>();

        foreach (EnemyGroup enemyGroup in wave.enemyGroups)
        {
            if (enemyGroup != null && enemyGroup.enemyPrefab != null &&
                enemyGroup.spawnCount < enemyGroup.count)
            {
                availableGroups.Add(enemyGroup);
            }
        }

        if (availableGroups.Count == 0) return null;

        return availableGroups[Random.Range(0, availableGroups.Count)];
    }

    Transform GetRandomSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            // Fallback: use spawner's position
            return transform;
        }

        // Filter out null spawn points
        List<Transform> validSpawnPoints = new List<Transform>();
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint != null)
            {
                validSpawnPoints.Add(spawnPoint);
            }
        }

        if (validSpawnPoints.Count == 0) return transform;

        return validSpawnPoints[Random.Range(0, validSpawnPoints.Count)];
    }

    // Call this when an enemy dies
    public void OnEnemyDeath()
    {
        enemiesAlive--;
        totalEnemiesKilled++;

        if (showDebugLogs) Debug.Log($"Enemy defeated! Enemies alive: {enemiesAlive}");

    }

    // For UI display
    public int GetCurrentWaveNumber() => currentWaveIndex + 1;
    public int GetTotalWaves() => waves.Count;
    public int GetEnemiesAlive() => enemiesAlive;
    public int GetTotalEnemiesKilled() => totalEnemiesKilled;

    // Method to manually add a wave (useful for testing)
    public void AddWave(string waveName, float spawnInterval = 2f)
    {
        Wave newWave = new Wave
        {
            waveName = waveName,
            spawnInterval = spawnInterval,
            enemyGroups = new List<EnemyGroup>()
        };
        waves.Add(newWave);
        CalculateWaveQuota();
    }

    private void CheckForGameWin()
    {
        if (currentWaveIndex >= waves.Count && enemiesAlive <= 0)
        {
            GameManager gameManager = FindObjectOfType<GameManager>();
            gameManager.GameWin();
            
            
        }
    }



}