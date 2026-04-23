using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private TMP_Text highScore;

    [SerializeField] private Button retryButton;
    [SerializeField] private Button quitButton;

    private void OnEnable()
    {
        PlayerEvents.OnPlayerDead += OnPlayerDeadHandler;
    }

    private void OnDisable()
    {
        PlayerEvents.OnPlayerDead -= OnPlayerDeadHandler;
    }

    private void Start()
    {
        retryButton.onClick.AddListener(OnRetryClicked);
        quitButton.onClick.AddListener(OnQuit);
    }

    private void OnPlayerDeadHandler()
    {
        ShowGameOver();
    }

    private void ShowGameOver()
    {
        int currentScore = ScoreManager.Instance.GetCurrentScore();
        int bestScore = PlayerPrefs.GetInt("HighScore", 0);

        if (currentScore > bestScore)
        {
            bestScore = currentScore;
            PlayerPrefs.SetInt("HighScore", bestScore);
        }

        scoreText.text = currentScore.ToString();
        highScore.text = bestScore.ToString();
        coinText.text = ScoreManager.Instance.GetCoinsCollected().ToString();

        gameOverPanel.SetActive(true);

        Time.timeScale = 0f; // safe now

        canvasGroup.alpha = 0f;
        gameOverPanel.transform.localScale = Vector3.one * 2f;

        canvasGroup.DOFade(1f, 0.4f).SetUpdate(true);
        gameOverPanel.transform.DOScale(Vector3.one, 0.4f)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
    }

    public void OnRetryClicked()
    {
        Time.timeScale = 1f;

        PlayerEvents.OnScoreReset?.Invoke();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnQuit()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }
}