using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [Header("Score")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private float countSpeed = 80f;

    [Header("Coins")]
    [SerializeField] private TMP_Text coinText;

    private float displayedScore = 0f;
    private int lastScore        = 0; 
    private int lastCoins        = 0;

    private void OnEnable()
    {
        PlayerEvents.OnCoinsCollected += UpdateCoinUI;
        PlayerEvents.OnScoreReset     += ResetUI;
    }

    private void OnDisable()
    {
        PlayerEvents.OnCoinsCollected -= UpdateCoinUI;
        PlayerEvents.OnScoreReset     -= ResetUI;
    }

    private void Update()
    {
        if (ScoreManager.Instance == null) return;

        int realScore = ScoreManager.Instance.GetCurrentScore();
        scoreText.text = realScore.ToString();
        // if (displayedScore < realScore)
        // {
        //     displayedScore = Mathf.MoveTowards(
        //         displayedScore, realScore, countSpeed * Time.deltaTime
        //     );
        //     int rounded = Mathf.RoundToInt(displayedScore);
        //     if (rounded != lastScore)
        //     {
        //         lastScore      = rounded;
        //         scoreText.text = rounded.ToString();
        //     }
        //}
    }

    private void UpdateCoinUI(int amount)
    {
        int coins = ScoreManager.Instance.GetCoinsCollected();
        if (coins != lastCoins)
        {
            lastCoins      = coins;
            coinText.text  = coins.ToString();
        }
    }

    private void ResetUI()
    {
        displayedScore = 0;
        lastScore      = 0;
        lastCoins      = 0;
        scoreText.text = "0";
        coinText.text  = "0";
    }
}