using UnityEngine;

public class Collectible : MonoBehaviour
{
    private Collider coinCollider;
    private Renderer[] coinRenderers;
    private ChunkPrefab parentChunk;
    private Vector3 initialLocalPosition;

    private void Awake()
    {
        coinCollider = GetComponent<Collider>();
        // Only get renderers from children, NOT including any parent animator
        coinRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        // Find the parent track chunk
        parentChunk = GetComponentInParent<ChunkPrefab>();

        // Cache the original design-time position from the prefab
        initialLocalPosition = transform.localPosition;
    }

    private void OnEnable()
    {
        if (parentChunk != null)
        {
            parentChunk.OnChunkReset += HandleChunkReset;
        }
    }

    private void OnDisable()
    {
        if (parentChunk != null)
        {
            parentChunk.OnChunkReset -= HandleChunkReset;
        }
    }

    private void HandleChunkReset()
    {
        // Restore to the original design position
        transform.localPosition = initialLocalPosition;

        // Make sure the object is active in the hierarchy
        gameObject.SetActive(true);

        // Reset collider and renderers to make them visible and collectible again
        ResetCollectible();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!coinCollider.enabled) return;

        if (other.TryGetComponent<Player>(out Player player))
        {
            PlayerEvents.OnCoinsCollected?.Invoke(1);
            HideCoin();
        }
    }

    private void HideCoin()
    {
        coinCollider.enabled = false;
        foreach (Renderer r in coinRenderers)
            r.enabled = false;
    }

    public void ResetCollectible()
    {
        coinCollider.enabled = true;
        foreach (Renderer r in coinRenderers)
            r.enabled = true;
    }
}