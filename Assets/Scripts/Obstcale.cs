using UnityEngine;

public class Obstcale : MonoBehaviour
{
    private void Start()
    {
        Debug.Log($"[Obstcale] Script active on GameObject: {gameObject.name}");
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[Obstcale] Hit something! Name: {other.name}, Parent: {other.transform.parent?.name}");
        Player player = other.GetComponentInParent<Player>();
        if (player != null)
        {
            Debug.Log("[Obstcale] Found Player in parents of " + other.name);
            PlayerEvents.OnPlayerHit?.Invoke();
        }
        else
        {
            Debug.LogWarning("[Obstcale] Failed to find Player component in parents of " + other.name);
        }
    }
}