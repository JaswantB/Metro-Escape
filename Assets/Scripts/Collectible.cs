using UnityEngine;

public class Collectible : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.TryGetComponent<Player>(out Player player))
        {
            PlayerEvents.OnCoinsCollected?.Invoke(1);
            gameObject.SetActive(false);
        }
    }
    public void ResetCollectible()
    {
        gameObject.SetActive(true);
    }
}
