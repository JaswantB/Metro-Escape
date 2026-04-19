using Unity.VisualScripting;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    private int currentScore = 0;
    private int coinsCollected = 0;
    private float scoreTimer = 0f;

    [SerializeField] private float scoreIncreaseInterval = 1f;
    [SerializeField] private int coinScoreValue = 5;
    private bool isGameOver = false;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Update()
    {
        if (isGameOver)
        {
            return;
        }
        scoreTimer += Time.deltaTime;
        if (scoreTimer >= scoreIncreaseInterval)
        {
            scoreTimer = 0f;
            currentScore++;
            PlayerEvents.OnScoreIncreased?.Invoke();
        }
    }
    private void OnEnable()
    {
        PlayerEvents.OnPlayerHit += HandleGameOver;
        PlayerEvents.OnScoreReset += ResetScore;
        PlayerEvents.OnCoinsCollected += CollectCoins;
    }
    private void OnDisable()
    {
        PlayerEvents.OnPlayerHit -= HandleGameOver;
        PlayerEvents.OnScoreReset -= ResetScore;
        PlayerEvents.OnCoinsCollected -= CollectCoins;
    }
    private void HandleGameOver()
    {
        isGameOver = true;
    }
    private void CollectCoins(int amount)
    {
        coinsCollected += amount;
        currentScore = coinScoreValue;
        PlayerEvents.OnScoreIncreased?.Invoke();
    }
    private void ResetScore()
    {
        currentScore = 0;
        coinsCollected = 0;
        scoreTimer = 0f;
        isGameOver = false;
    }
    public int GetCurrentScore()
    {
        return currentScore;
    }
    public int GetCoinsCollected()
    {
        return coinsCollected;
    }

}
