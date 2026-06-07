using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    [Header("Chunk Prefabs (Unified for Pooling)")]
    [SerializeField] private List<GameObject> chunkPrefabs;

    [Header("Fallback / Compatibility Lists")]
    [SerializeField] private List<GameObject> easyChunk;
    [SerializeField] private List<GameObject> medChunk;
    [SerializeField] private List<GameObject> hardChunk;

    [Header("Spawning Settings")]
    [SerializeField] private int chunkAmount = 12;
    [SerializeField] private float chunkLength = 20f;
    [SerializeField] private Transform chunkParent;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float initialSpawnOffset = 7.5f;

    [Header("Object Pooling Settings")]
    [SerializeField] private int poolCount = 5; // Number of pooled instances per unique prefab type

    private bool isGameOver = false;
    private List<GameObject> Chunks; // Stores the active chunks currently in play
    private List<GameObject> chunkPool; // Stores all pooled inactive & active chunk instances
    private List<GameObject> availablePrefabs; // Combined list of unique prefabs

    void OnEnable()
    {
        PlayerEvents.OnPlayerHit += StopGeneration;
    }

    void OnDisable()
    {
        PlayerEvents.OnPlayerHit -= StopGeneration;
    }

    private void Start()
    {
        InitializePool();
        SpawnChunks();
    }

    void Update()
    {
        if (GameManager.instance?.CurrentState != GameState.Playing) return;
        MoveChunks();
    }

    private void InitializePool()
    {
        // Gather unique prefabs from chunkPrefabs and easy/med/hard compatibility lists
        availablePrefabs = new List<GameObject>();

        if (chunkPrefabs != null)
        {
            foreach (var prefab in chunkPrefabs)
            {
                if (prefab != null && !availablePrefabs.Contains(prefab))
                    availablePrefabs.Add(prefab);
            }
        }

        // Maintain backward compatibility for populated fields in Inspector
        if (easyChunk != null)
        {
            foreach (var prefab in easyChunk)
            {
                if (prefab != null && !availablePrefabs.Contains(prefab))
                    availablePrefabs.Add(prefab);
            }
        }
        if (medChunk != null)
        {
            foreach (var prefab in medChunk)
            {
                if (prefab != null && !availablePrefabs.Contains(prefab))
                    availablePrefabs.Add(prefab);
            }
        }
        if (hardChunk != null)
        {
            foreach (var prefab in hardChunk)
            {
                if (prefab != null && !availablePrefabs.Contains(prefab))
                    availablePrefabs.Add(prefab);
            }
        }

        if (availablePrefabs.Count == 0)
        {
            Debug.LogError("[ChunkGenerator] No chunk prefabs assigned to chunkPrefabs, easyChunk, medChunk, or hardChunk lists!");
            return;
        }

        // Initialize the object pool
        chunkPool = new List<GameObject>();
        foreach (GameObject prefab in availablePrefabs)
        {
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
        List<GameObject> inactiveChunks = new List<GameObject>();

        // Proper activeInHierarchy check as requested by the user
        foreach (GameObject obj in chunkPool)
        {
            if (obj != null && !obj.activeInHierarchy && obj != excludeChunk)
            {
                inactiveChunks.Add(obj);
            }
        }

        // Fallback: If the only inactive chunk was the excluded one, allow it to be chosen to avoid freezing
        if (inactiveChunks.Count == 0 && excludeChunk != null && !excludeChunk.activeInHierarchy)
        {
            inactiveChunks.Add(excludeChunk);
        }

        if (inactiveChunks.Count > 0)
        {
            int randomIndex = Random.Range(0, inactiveChunks.Count);
            return inactiveChunks[randomIndex];
        }

        // Dynamic expansion fallback: if the pool is fully exhausted, instantiate a new one to prevent failure
        Debug.LogWarning("[ChunkGenerator] Object pool exhausted. Dynamically creating new instance.");
        GameObject randomPrefab = availablePrefabs[Random.Range(0, availablePrefabs.Count)];
        GameObject newObj = Instantiate(randomPrefab, chunkParent);
        newObj.SetActive(false);
        chunkPool.Add(newObj);
        return newObj;
    }

    private void SpawnChunks()
    {
        Chunks = new List<GameObject>();

        for (int i = 0; i < chunkAmount; i++)
        {
            GameObject newChunk = GetPooledChunk();
            if (newChunk == null) continue;

            if (i == 0)
            {
                Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + initialSpawnOffset);
                newChunk.transform.position = spawnPosition;
                newChunk.transform.rotation = Quaternion.identity;
            }
            else
            {
                GameObject lastChunk = Chunks[Chunks.Count - 1];
                IChunk chunkData = lastChunk.GetComponent<IChunk>();
                if (chunkData != null)
                {
                    Transform endPoint = chunkData.GetEndpoint();
                    newChunk.transform.position = endPoint.position;
                    newChunk.transform.rotation = Quaternion.identity;
                }
                else
                {
                    // Fallback positioning if script missing
                    Vector3 fallbackPos = lastChunk.transform.position + Vector3.forward * chunkLength;
                    newChunk.transform.position = fallbackPos;
                    newChunk.transform.rotation = Quaternion.identity;
                }
            }

            // Reset coins and other collectibles inside the chunk
            if (newChunk.TryGetComponent<ChunkPrefab>(out ChunkPrefab chunkPrefab))
            {
                chunkPrefab.ResetCollectibles();
            }

            newChunk.SetActive(true);
            Chunks.Add(newChunk);
        }
    }

    private void MoveChunks()
    {
        for (int i = 0; i < Chunks.Count; i++)
        {
            GameObject currentChunk = Chunks[i];

            // Move the chunk backward
            currentChunk.transform.Translate(-Vector3.forward * moveSpeed * Time.deltaTime);

            // Check if the chunk has moved off-screen (behind the player/camera)
            if (currentChunk.transform.position.z < Camera.main.transform.position.z - chunkLength)
            {
                GameObject lastChunk = Chunks[Chunks.Count - 1];

                // Do not recycle if it's the last chunk in the sequence (to avoid spawning errors)
                if (currentChunk == lastChunk)
                {
                    continue;
                }
                else
                {
                    // 1. Deactivate the recycled chunk (returning it to the pool)
                    currentChunk.SetActive(false);

                    // 2. Remove it from the active list
                    Chunks.RemoveAt(i);

                    // 3. Retrieve a random new chunk from the pool, excluding the one we just turned off (unless it's the only option)
                    GameObject newChunk = GetPooledChunk(currentChunk);

                    if (newChunk != null)
                    {
                        // 4. Position the new chunk at the endpoint of the current last chunk
                        IChunk chunkData = lastChunk.GetComponent<IChunk>();
                        if (chunkData != null)
                        {
                            Transform endPoint = chunkData.GetEndpoint();
                            newChunk.transform.position = endPoint.position;
                            newChunk.transform.rotation = Quaternion.identity;
                        }
                        else
                        {
                            Vector3 fallbackPos = lastChunk.transform.position + Vector3.forward * chunkLength;
                            newChunk.transform.position = fallbackPos;
                            newChunk.transform.rotation = Quaternion.identity;
                        }

                        // 5. Reset the collectibles (coins) on the recycled chunk
                        if (newChunk.TryGetComponent<ChunkPrefab>(out ChunkPrefab chunkPrefab))
                        {
                            chunkPrefab.ResetCollectibles();
                        }

                        // 6. Activate the chunk and add it to the active list
                        newChunk.SetActive(true);
                        Chunks.Add(newChunk);
                    }

                    // 7. Adjust index due to item removal
                    i--;
                }
            }
        }
    }

    private void StopGeneration()
    {
        isGameOver = true;
    }
}

