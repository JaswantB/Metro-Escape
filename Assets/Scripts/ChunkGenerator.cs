using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    [Header("Chunk Prefabs")]
    [SerializeField] private List<GameObject> chunkPrefabs;

    [Header("Spawning Settings")]
    [SerializeField] private int chunkAmount = 12;
    [SerializeField] private float chunkLength = 20f;
    [SerializeField] private Transform chunkParent;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float initialSpawnOffset = 7.5f;

    [Header("Score Progression")]
    [SerializeField] private float transitionSpeed = 2f; // How fast speed scales up (units/sec)

    [Header("Object Pooling Settings")]
    [SerializeField] private int poolCount = 2; // Safe minimum: chunkPrefabs.Count × poolCount ≥ chunkAmount + 2

    private float targetSpeed = 15f;

    // FIX 1: Cached camera — no more Camera.main in Update loop
    private Camera mainCamera;

    private List<GameObject> activeChunks;
    private List<GameObject> chunkPool;

    // FIX 2: Class-level list — reused via Clear(), no heap allocation per call
    private readonly List<GameObject> inactiveChunks = new List<GameObject>();

    private bool isStopped = false;

    void OnEnable()
    {
        PlayerEvents.OnPlayerHit += StopGeneration;
        PlayerEvents.OnScoreChanged += HandleScoreChanged;
    }

    void OnDisable()
    {
        PlayerEvents.OnPlayerHit -= StopGeneration;
        PlayerEvents.OnScoreChanged -= HandleScoreChanged;
    }

    private void Start()
    {
        mainCamera = Camera.main; // Cache once here
        InitializePool();
        moveSpeed = 10f;
        isStopped = false;
        SpawnChunks();
    }

    void Update()
    {
        if (isStopped || GameManager.instance?.CurrentState != GameState.Playing) return;

        // Gradually transition current moveSpeed to targetSpeed
        moveSpeed = Mathf.MoveTowards(moveSpeed, targetSpeed, transitionSpeed * Time.deltaTime);

        MoveChunks();
    }

    private void InitializePool()
    {
        if (chunkPrefabs == null || chunkPrefabs.Count == 0)
        {
            Debug.LogError("[ChunkGenerator] No chunk prefabs assigned!");
            return;
        }

        chunkPool = new List<GameObject>();

        foreach (GameObject prefab in chunkPrefabs)
        {
            if (prefab == null) continue;

            for (int i = 0; i < poolCount; i++)
            {
                GameObject obj = Instantiate(prefab, chunkParent);
                obj.SetActive(false);
                chunkPool.Add(obj);
            }
        }
    }

    private GameObject GetPooledChunk(GameObject excludeChunk = null)
    {
        // FIX 2 in action: Clear() reuses memory instead of new List<>() allocating fresh
        inactiveChunks.Clear();

        foreach (GameObject obj in chunkPool)
        {
            if (obj != null && !obj.activeInHierarchy && obj != excludeChunk)
                inactiveChunks.Add(obj);
        }

        // Fallback: if excluded chunk is the only option, allow it
        if (inactiveChunks.Count == 0 && excludeChunk != null && !excludeChunk.activeInHierarchy)
            inactiveChunks.Add(excludeChunk);

        if (inactiveChunks.Count > 0)
        {
            return inactiveChunks[Random.Range(0, inactiveChunks.Count)];
        }

        // Last resort: expand pool dynamically (shouldn't happen with correct poolCount)
        Debug.LogWarning("[ChunkGenerator] Pool exhausted. Increase poolCount!");
        GameObject randomPrefab = chunkPrefabs[Random.Range(0, chunkPrefabs.Count)];
        GameObject newObj = Instantiate(randomPrefab, chunkParent);
        newObj.SetActive(false);
        chunkPool.Add(newObj);
        return newObj;
    }

    private void SpawnChunks()
    {
        activeChunks = new List<GameObject>();

        for (int i = 0; i < chunkAmount; i++)
        {
            GameObject newChunk = GetPooledChunk();
            if (newChunk == null) continue;

            if (i == 0)
            {
                newChunk.transform.SetPositionAndRotation(
                    new Vector3(transform.position.x, transform.position.y, transform.position.z + initialSpawnOffset),
                    Quaternion.identity
                );
            }
            else
            {
                // FIX 3: Extracted helper — no duplicate positioning logic
                PositionChunkAfter(newChunk, activeChunks[activeChunks.Count - 1]);
            }

            ResetChunk(newChunk);
            newChunk.SetActive(true);
            activeChunks.Add(newChunk);
        }
    }

    private void MoveChunks()
    {
        for (int i = 0; i < activeChunks.Count; i++)
        {
            GameObject currentChunk = activeChunks[i];
            currentChunk.transform.Translate(-Vector3.forward * moveSpeed * Time.deltaTime);

            // FIX 1 in action: mainCamera instead of Camera.main
            if (currentChunk.transform.position.z < mainCamera.transform.position.z - chunkLength)
            {
                GameObject lastChunk = activeChunks[activeChunks.Count - 1];

                if (currentChunk == lastChunk) continue;

                currentChunk.SetActive(false);
                activeChunks.RemoveAt(i);

                GameObject newChunk = GetPooledChunk(currentChunk);

                if (newChunk != null)
                {
                    PositionChunkAfter(newChunk, lastChunk);
                    ResetChunk(newChunk);
                    newChunk.SetActive(true);
                    activeChunks.Add(newChunk);
                }

                i--;
            }
        }
    }

    // FIX 3: Single positioning method used by both SpawnChunks & MoveChunks (DRY)
    // ✅ Cache ChunkPrefab reference alongside IChunk
    private void PositionChunkAfter(GameObject chunk, GameObject referenceChunk)
    {
        IChunk chunkData = referenceChunk.GetComponentInChildren<IChunk>();

        // 🔍 Add this - tells us EXACTLY where endpoint is in world space
        if (chunkData != null)
            Debug.Log($"Endpoint World Position Z: {chunkData.GetEndpoint().position.z}");
        else
            Debug.LogError("IChunk still null!");

        chunk.transform.SetPositionAndRotation(
            chunkData != null
                ? chunkData.GetEndpoint().position
                : referenceChunk.transform.position + Vector3.forward * chunkLength,
            Quaternion.identity
        );
    }
    private void ResetChunk(GameObject chunk)
    {
        IChunk chunkData = chunk.GetComponentInChildren<IChunk>();
        if (chunkData != null)
        {
            chunkData.ResetCollectibles();
        }
    }
    private void HandleScoreChanged(int score)
    {
        float previousTargetSpeed = targetSpeed;
        // Set target speed based on score milestones
        if (score < 500)
            targetSpeed = 15f;
        else if (score < 1500)
            targetSpeed = 20f;
        else if (score < 3000)
            targetSpeed = 25f;
        else
            targetSpeed = 30f;

        if (targetSpeed != previousTargetSpeed)
        {
            Debug.Log($"[ChunkGenerator] Score is {score}. Target speed increased from {previousTargetSpeed} to {targetSpeed}!");
        }
    }

    private void StopGeneration()
    {
        isStopped = true;
    }


}