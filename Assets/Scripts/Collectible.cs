using UnityEngine;

public class Collectible : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            PlayerEvents.OnCoinsCollected?.Invoke(1);
            Destroy(gameObject);
        }
    }
}
