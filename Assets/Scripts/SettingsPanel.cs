using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    public Slider volumeSlider;
    public Button ApplyButton;
    public Button RevertButton;
    public float savedVolume=1f;
    private void OnEnable()
    {
        if (AudioManager.Instance != null)
        {
            volumeSlider.value = AudioManager.Instance.musicSource.volume;
            volumeSlider.value = savedVolume;
        }
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        ApplyButton.onClick.AddListener(OnApplyButtonClicked);
        RevertButton.onClick.AddListener(OnRevertButtonClicked);
    }
    private void OnDisable()
    {
        volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
        ApplyButton.onClick.RemoveListener(OnApplyButtonClicked);
        RevertButton.onClick.RemoveListener(OnRevertButtonClicked);
    }
    private void OnApplyButtonClicked()
    {
        savedVolume = volumeSlider.value;
        PlayerEvents.OnVolumeChanged?.Invoke(savedVolume);
    }
    private void OnRevertButtonClicked()
    {
        volumeSlider.value = savedVolume;
        PlayerEvents.OnVolumeChanged?.Invoke(savedVolume);
    }
    private void OnVolumeChanged(float volume)
    {
        PlayerEvents.OnVolumeChanged?.Invoke(volume);
    }

}
