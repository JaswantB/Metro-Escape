using UnityEngine;

public class ChunkPrefab : MonoBehaviour, IChunk
{
    public Transform endPoint;

    public Transform GetEndpoint()
    {
        return endPoint;
    }
}
public interface IChunk
{
    Transform GetEndpoint();
}