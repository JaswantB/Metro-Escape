using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    private int currentScore = 0;
    private int coinsCollected = 0;
    private float scoreTimer = 0f;
    private bool isGameOver = false;

    [SerializeField] private float scoreIncreaseInterval = 0.1f; // faster = smoother

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        PlayerEvents.OnPlayerHit      += HandleGameOver;
        PlayerEvents.OnScoreReset     += ResetScore;
        PlayerEvents.OnCoinsCollected += CollectCoins; // only coins, no score change
    }

    private void OnDisable()
    {
        PlayerEvents.OnPlayerHit      -= HandleGameOver;
        PlayerEvents.OnScoreReset     -= ResetScore;
        PlayerEvents.OnCoinsCollected -= CollectCoins;
    }

    private void Update()
    {
        if (isGameOver) return;
        scoreTimer += Time.deltaTime;
        if (scoreTimer >= scoreIncreaseInterval)
        {
            scoreTimer = 0f;
            currentScore++;
            PlayerEvents.OnScoreIncreased?.Invoke();
        }
    }

    private void CollectCoins(int amount)
    {
        coinsCollected += amount;
    }

    private void HandleGameOver()
    {
        isGameOver = true;
    }

    private void ResetScore()
    {
        currentScore  = 0;
        coinsCollected = 0;
        scoreTimer    = 0f;
        isGameOver    = false;
    }

    public int GetCurrentScore()    => currentScore;
    public int GetCoinsCollected()  => coinsCollected;
}