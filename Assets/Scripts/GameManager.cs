using UnityEngine;
public enum GameState
{
    Playing,
    GameOver

}

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public GameState CurrentState { get; private set; }
    [SerializeField] private AudioClip gamePlayMusic;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private void Start()
    {
        SetState(GameState.Playing);
        if (AudioManager.Instance != null)
            AudioManager.Instance.ChangeMusic(gamePlayMusic);
    }
    public void OnEnable()
    {
        PlayerEvents.OnPlayerDead += HandlePlayerDead;
    }
    private void OnDisable()
    {
        PlayerEvents.OnPlayerDead -= HandlePlayerDead;
    }
    private void HandlePlayerDead()
    {
        SetState(GameState.GameOver);
    }
    public void SetState(GameState gameState)
    {
        CurrentState = gameState;
        switch (gameState)
        {
            case GameState.Playing:
                Time.timeScale = 1f;
                PlayerEvents.OnScoreReset?.Invoke();
                break;
            case GameState.GameOver:
                Time.timeScale = 0f;
                break;
        }
        Debug.Log($"[GameManager] Sate: {gameState}");
    }
}
