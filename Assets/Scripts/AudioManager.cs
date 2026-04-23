using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [Header("Music")]
    public AudioSource musicSource;
    public AudioClip backGroundMusic;

    [Header("SFX Clips")]
    public AudioSource SFXsource;
    //public AudioClip jumpSFX;
    public AudioClip landSFX;
    public AudioClip slideSFX;
    public AudioClip playerHitSFX;
    public AudioClip laneChangeSFX;
    public AudioClip coinSFX;
    //public AudioClip scoreIncreaseSFX;

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
        //PlayerEvents.onJump += OnJump;
        PlayerEvents.onGroundChanged += OnGroundChanged;
        PlayerEvents.OnSlideStart += OnSlideStart;
        //PlayerEvents.OnSlideEnd += OnSlideEnd;
        PlayerEvents.OnPlayerHit += OnPlayerHit;
        PlayerEvents.OnLaneChanged += OnLaneChanged;
        //PlayerEvents.OnScoreIncreased += OnScoreIncreased;
        PlayerEvents.OnCoinsCollected += OnCoinsCollected;
        PlayerEvents.OnVolumeChanged += OnVolumeChanged;
    }
    private void OnDisable()
    {
        //PlayerEvents.onJump -= OnJump;
        PlayerEvents.onGroundChanged -= OnGroundChanged;
        PlayerEvents.OnSlideStart -= OnSlideStart;
        //PlayerEvents.OnSlideEnd -= OnSlideEnd;
        PlayerEvents.OnPlayerHit -= OnPlayerHit;
        PlayerEvents.OnLaneChanged -= OnLaneChanged;
        // PlayerEvents.OnScoreIncreased -= OnScoreIncreased;
        PlayerEvents.OnCoinsCollected -= OnCoinsCollected;
        PlayerEvents.OnVolumeChanged -= OnVolumeChanged;
    }
    private void OnVolumeChanged(float volume)
    {
        musicSource.volume = volume;
    }

    // private void OnJump()
    // {
    //     PlaySfx(jumpSFX);
    // }
    private void OnGroundChanged(bool isGrounded)
    {
        if (isGrounded)
        {
            PlaySfx(landSFX);
        }
    }
    private void OnSlideStart()
    {
        PlaySfx(slideSFX);
    }
    private void OnPlayerHit()
    {
        PlaySfx(playerHitSFX);
    }
    private void OnLaneChanged(int lane)
    {
        PlaySfx(laneChangeSFX);
    }
    // private void OnScoreIncreased()
    // {
    //     PlaySfx(scoreIncreaseSFX);
    // }
    private void OnCoinsCollected(int coins)
    {
        PlaySfx(coinSFX);
    }
    public void PlaySfx(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }
        SFXsource.PlayOneShot(clip);
    }
    public void ChangeMusic(AudioClip newClip)
    {
        if (newClip == null) return;

        if (musicSource.clip == newClip) return;

        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.loop = true;
        musicSource.Play();
    }
    private void PlayMusic()
    {
        musicSource.clip = backGroundMusic;
        musicSource.loop = true;
        musicSource.Play();
    }
}
