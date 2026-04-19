using System;
public class PlayerEvents
{
    public static Action onJump;
    public static Action <bool> onGroundChanged;

    public static Action OnSlideStart;
    public static Action OnSlideEnd;
    
    public static Action<int>OnLaneChanged;
    
}
