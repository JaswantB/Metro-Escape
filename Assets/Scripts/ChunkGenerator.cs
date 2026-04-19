using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    //[SerializeField] private List<GameObject> chunkPrefab;
    [SerializeField] private List<GameObject> easyChunk;
    [SerializeField] private List<GameObject> medChunk;
    [SerializeField] private List<GameObject> hardChunk;

    [SerializeField] private int chunkAmount = 12;
    [SerializeField] private float chunkLength = 20;
    [SerializeField] private Transform chunkParent;
    [SerializeField] private float moveSpeed = 5f;


    int hardStreak = 0;
    List<GameObject> Chunks;
    private void Start()
    {
        SpawnChunks();
    }
    void Update()
    {
        MoveChunks();
    }
    GameObject SelectedNextPrefab()
    {
        if(hardStreak >= 5)
        {
            int index=Random.Range(0,easyChunk.Count);
            hardStreak=0;
            return easyChunk[index];
        }
        float rand=Random.value;
        if(rand < 0.2f)
        {
            int index=Random.Range(0,easyChunk.Count);
            hardStreak=0;
            return easyChunk[index];
        }
        else if(rand < 0.5f)
        {
            int index=Random.Range(0,medChunk.Count);
            hardStreak=0;
            return medChunk[index];
        }
        else
        {
            int index=Random.Range(0,hardChunk.Count);
            hardStreak++;
            return hardChunk[index];
        }
    }
    private void SpawnChunks()
    {
        Chunks = new List<GameObject>();
        for (int i = 0; i < chunkAmount; i++)
        {

            GameObject selectedPrefab;
            GameObject newChunk;
            if (i == 0)
            {
                selectedPrefab = easyChunk[Random.Range(0, easyChunk.Count)];
                newChunk = Instantiate(selectedPrefab, transform.position, Quaternion.identity, chunkParent);
            }
            else
            {
                selectedPrefab=SelectedNextPrefab();
                GameObject lastChunk = Chunks[Chunks.Count - 1];
                IChunk chunkData = lastChunk.GetComponent<IChunk>();
                Transform endPoint = chunkData.GetEndpoint();
                newChunk = Instantiate(selectedPrefab, endPoint.position, Quaternion.identity, chunkParent);
            }
            Chunks.Add(newChunk);
            Debug.Log("Chunk Instantiating");
        }
    }

    private void MoveChunks()
    {
        //Chunks= new List<GameObject>();

        for (int i = 0; i < Chunks.Count; i++)
        {
            GameObject currentChunk = Chunks[i];

            currentChunk.transform.Translate(-Vector3.forward * moveSpeed * Time.deltaTime);

            if (currentChunk.transform.position.z < Camera.main.transform.position.z - chunkLength)
            {

                GameObject lastChunk = Chunks[Chunks.Count - 1];
                if (currentChunk == lastChunk)
                {
                    continue;
                }
                else
                {
                    GameObject selectedPrefab= SelectedNextPrefab();
                    IChunk chunkData = lastChunk.GetComponent<IChunk>();

                    Transform endPoint = chunkData.GetEndpoint();
                    currentChunk.transform.position = endPoint.position;
                    Chunks.RemoveAt(i);
                    Chunks.Add(currentChunk);
                    i--;
                }
            }
        }
    }
}

