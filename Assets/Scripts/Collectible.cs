using UnityEngine;

public class Collectible : MonoBehaviour
{
    private Collider coinCollider;
    private Renderer[] coinRenderers;

    private void Awake()
    {
        coinCollider = GetComponent<Collider>();
        // Only get renderers from children, NOT including any parent animator
        coinRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
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