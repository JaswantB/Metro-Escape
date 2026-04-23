using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    private int currentScore = 0;
    private int coinsCollected = 0;
    private float scoreTimer = 0f;
    private float score = 0;
    [SerializeField] private float scoreMultiplier = 10f;
    private bool isGameOver = false;

    // faster = smoother

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
        PlayerEvents.OnPlayerHit += HandleGameOver;
        PlayerEvents.OnScoreReset += ResetScore;
        PlayerEvents.OnCoinsCollected += CollectCoins; // only coins, no score change
    }

    private void OnDisable()
    {
        PlayerEvents.OnPlayerHit -= HandleGameOver;
        PlayerEvents.OnScoreReset -= ResetScore;
        PlayerEvents.OnCoinsCollected -= CollectCoins;
    }

    private void Update()
    {
        if (GameManager.instance?.CurrentState != GameState.Playing)
        {
            return;
        }
        scoreTimer += Time.deltaTime;
        currentScore = Mathf.FloorToInt(scoreTimer * scoreMultiplier);
        PlayerEvents.OnScoreIncreased?.Invoke();
        Debug.Log($"Score: {currentScore}");
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
        currentScore = 0;
        coinsCollected = 0;
        scoreTimer = 0f;
        isGameOver = false;
    }

    public int GetCurrentScore() => currentScore;
    public int GetCoinsCollected() => coinsCollected;
}