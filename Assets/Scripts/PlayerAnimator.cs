using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    Animator anim;
    public bool isJumping;
    public bool isDead;

    [SerializeField] float LaneChangeCoolDown = 0.5f;
    float lastChangeTime;

    // Cached Animator Parameter Hashes for performance
    private static readonly int RollHash = Animator.StringToHash("Roll");
    private static readonly int SlidingTriggerHash = Animator.StringToHash("Sliding");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int IsGroundedHash = Animator.StringToHash("isGrounded");
    private static readonly int IsSlidingHash = Animator.StringToHash("isSliding");
    private static readonly int LaneSwitchHash = Animator.StringToHash("LaneSwitch");
    private static readonly int IsDeadHash = Animator.StringToHash("isDead");
    private static readonly int DieHash = Animator.StringToHash("Die");
    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void OnEnable()
    {
        PlayerEvents.onJump += OnJumpEvent;
        PlayerEvents.onGroundChanged += HandleGround;
        PlayerEvents.OnSlideStart += OnSlide;
        PlayerEvents.OnSlideEnd += SlideEnd;
        PlayerEvents.OnLaneChanged += OnLaneChange;
        PlayerEvents.OnPlayerHit += OnPlayerHit;
    }

    void OnDisable()
    {
        PlayerEvents.onJump -= OnJumpEvent;
        PlayerEvents.onGroundChanged -= HandleGround;
        PlayerEvents.OnSlideStart -= OnSlide;
        PlayerEvents.OnSlideEnd -= SlideEnd;
        PlayerEvents.OnLaneChanged -= OnLaneChange;
        PlayerEvents.OnPlayerHit -= OnPlayerHit;
    }

    void OnJumpEvent()
    {
        if (isDead) return;
        if (isJumping) return;
        isJumping = true;
        anim.ResetTrigger(RollHash);
        anim.ResetTrigger(SlidingTriggerHash);
        anim.SetTrigger(JumpHash);
        //    anim.SetFloat("VerticalVelocity", 1f);
        //    anim.SetBool("isGrounded", false);
    }
    void HandleGround(bool Grounded)
    {
        anim.SetBool(IsGroundedHash, Grounded);
        if (Grounded)
        {
            isJumping = false;
            anim.ResetTrigger(JumpHash);
        }
    }
    void OnSlide()
    {
        if (isDead) return;
        anim.ResetTrigger(JumpHash);
        anim.SetTrigger(SlidingTriggerHash);
        anim.SetBool(IsSlidingHash, true);
    }
    void SlideEnd()
    {
        anim.SetBool(IsSlidingHash, false);
    }
    void OnLaneChange(int dir)
    {
        if (isDead) return;

        if (isJumping) return;


        if (Time.time - lastChangeTime < LaneChangeCoolDown)
            return;

        lastChangeTime = Time.time;

        anim.ResetTrigger(LaneSwitchHash);
        anim.SetTrigger(LaneSwitchHash);
    }
    void OnPlayerHit()
    {
        Debug.Log($"[PlayerAnimator] OnPlayerHit event received! setting isDead=true and trigger Die");
        isDead = true;
        anim.SetBool(IsDeadHash, true);
        anim.ResetTrigger(JumpHash);
        anim.ResetTrigger(LaneSwitchHash);
        anim.ResetTrigger(SlidingTriggerHash);
        anim.SetTrigger(DieHash);
    }
}
