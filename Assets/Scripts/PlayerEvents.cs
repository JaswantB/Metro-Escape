using System;
public class PlayerEvents
{
    public static Action onJump;
    public static Action <bool> onGroundChanged;

    public static Action OnSlideStart;
    public static Action OnSlideEnd;
    
    public static Action OnPlayerHit;
    public static Action<int>OnLaneChanged;

    public static Action OnScoreIncreased;
    public static Action OnScoreReset;
    public static Action <int>OnCoinsCollected;
    
}
