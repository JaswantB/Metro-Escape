using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [Header("Music")]
    public AudioSource musicSource;
    public AudioClip backGroundMusic;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            PlayMusic();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnEnable()
    {
        PlayerEvents.OnVolumeChanged += OnVolumeChanged;
    }
    private void OnDisable()
    {
        PlayerEvents.OnVolumeChanged -= OnVolumeChanged;
    }
    private void OnVolumeChanged(float volume)
    {
        musicSource.volume = volume;
    }
    private void PlayMusic()
    {
        musicSource.clip = backGroundMusic;
        musicSource.loop = true;
        musicSource.Play();
    }


}
