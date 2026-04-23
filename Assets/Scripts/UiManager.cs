using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
public class UiManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button PlayButton;
    public Button ExitButton;
    public Button settingsButton;
    public Button backButton;

    [Header("Panels")]
    public RectTransform mainMenuPanel;
    public RectTransform settingsPanel;

    [Header("Loading Screen")]
    public GameObject loadingScreen;
    public Slider loadingBar;

    [SerializeField] private AudioClip menuMusic;

    private void Awake()
    {
        settingsPanel.localScale = Vector3.zero;
    }

    private void Start()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.ChangeMusic(menuMusic);

        mainMenuPanel.localScale = Vector3.zero;
        mainMenuPanel.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
    }
    private void OnEnable()
    {
        PlayButton.onClick.AddListener(OnPlayButtonClicked);
        ExitButton.onClick.AddListener(OnExitButtonClicked);
        settingsButton.onClick.AddListener(OnSettingsButtonClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);
    }
    private void OnDisable()
    {
        PlayButton.onClick.RemoveListener(OnPlayButtonClicked);
        ExitButton.onClick.RemoveListener(OnExitButtonClicked);
        settingsButton.onClick.RemoveListener(OnSettingsButtonClicked);
        backButton.onClick.RemoveListener(OnBackButtonClicked);
    }

    private void OnPlayButtonClicked()
    {
        mainMenuPanel.DOScale(0f, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            StartCoroutine(LoadScene());
        });

        Debug.Log("Play Button Clicked");
    }
    IEnumerator LoadScene()
    {
        loadingScreen.SetActive(true);
        loadingBar.value = 0f;

        AsyncOperation operation = SceneManager.LoadSceneAsync("EndlessRunner");
        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            loadingBar.value = operation.progress;
            yield return null;
        }
        loadingBar.DOValue(1f, 0.5f).OnComplete(() =>
        {
            operation.allowSceneActivation = true;
        });
    }
    private void OnSettingsButtonClicked()
    {
        mainMenuPanel.DOScale(0f, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            settingsPanel.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
        });
    }
    private void OnBackButtonClicked()
    {
        settingsPanel.DOScale(0f, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            mainMenuPanel.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
        });
    }

    void OnExitButtonClicked()
    {
        Debug.Log("Exit Button Clicked");
        Application.Quit();
    }
}

