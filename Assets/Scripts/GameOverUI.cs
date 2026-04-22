using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using TMPro;
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
        PlayerEvents.OnPlayerHit += GameOver;
    }
    private void OnDisable()
    {
        PlayerEvents.OnPlayerHit -= GameOver;
    }
    private void GameOver()
    {
        int currentScore = ScoreManager.Instance.GetCurrentScore();
        int bestScore = PlayerPrefs.GetInt("HighScore", 0);

        if (currentScore > bestScore)
        {
            bestScore = currentScore;
            PlayerPrefs.GetInt("HighScore", bestScore);

        }
        scoreText.text = ScoreManager.Instance.GetCurrentScore().ToString();
        highScore.text = bestScore.ToString();
        coinText.text = ScoreManager.Instance.GetCoinsCollected().ToString();
        gameOverPanel.SetActive(true);
        Time.timeScale = 1f;

        canvasGroup.alpha = 0f;
        gameOverPanel.transform.localScale = Vector3.one * 2f;

        canvasGroup.DOFade(1f, 0.4f).SetUpdate(true);
        gameOverPanel.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack).SetUpdate(true);
        retryButton.onClick.AddListener(OnRetryClicked);
        quitButton.onClick.AddListener(OnQuit);
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
