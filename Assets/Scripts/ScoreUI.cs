using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text coinText;

    private void OnEnable()
    {
        PlayerEvents.OnCoinsCollected+= UpdateCoinText;
        PlayerEvents.OnScoreIncreased+= UpdateScoreText;
    }
    private void OnDisable()
    {
        PlayerEvents.OnCoinsCollected-= UpdateCoinText;
        PlayerEvents.OnScoreIncreased-= UpdateScoreText;
    }
    private void UpdateCoinText(int coins)
    {
        coinText.text=ScoreManager.Instance.GetCoinsCollected().ToString();
        coinText.text = "Coins: " + coins.ToString();
        Debug.Log("Coins: " + coins);
    }
    private void UpdateScoreText()
    {
        int currentScore = ScoreManager.Instance.GetCurrentScore();
        scoreText.text = "Score: " + currentScore.ToString();
        Debug.Log("Score: " + currentScore);
    }

}
