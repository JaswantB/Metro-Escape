using UnityEngine;
using System;

public class ChunkPrefab : MonoBehaviour, IChunk
{
    public Transform endPoint;

    // Decoupled event system
    public event Action OnChunkReset;

    public Transform GetEndpoint()
    {
        return endPoint;
    }
    public void ResetCollectibles()
    {
        // Fire the event to notify all listening child elements
        OnChunkReset?.Invoke();
    }
}
public interface IChunk
{
    Transform GetEndpoint();
}