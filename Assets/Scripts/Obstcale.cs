using UnityEngine;

public class Obstcale : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            PlayerEvents.OnPlayerHit?.Invoke();
        }
    }
}
