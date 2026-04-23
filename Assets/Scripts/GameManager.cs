using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private AudioClip gamePlayMusic;
    private void Start()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.ChangeMusic(gamePlayMusic);
    }
}
