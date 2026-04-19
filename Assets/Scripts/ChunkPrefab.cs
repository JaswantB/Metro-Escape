using UnityEngine;

public class ChunkPrefab : MonoBehaviour, IChunk
{
    public Transform endPoint;
    private Collectible[] collectibles;

    public Transform GetEndpoint()
    {
        return endPoint;
    }
    public void ResetCollectibles()
    {
        if (collectibles == null || collectibles.Length == 0)
        {
            collectibles = GetComponentsInChildren<Collectible>();
        }
        foreach (var collectible in collectibles)
        {
            collectible.ResetCollectible();
        }
    }
}
public interface IChunk
{
    Transform GetEndpoint();
}