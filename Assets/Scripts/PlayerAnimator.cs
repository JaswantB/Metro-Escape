using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    Animator anim;
    public bool isJumping;

    [SerializeField] float LaneChangeCoolDown = 0.5f;
    float lastChangeTime;
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
        if (isJumping) return;
        isJumping = true;
        anim.ResetTrigger("Roll");
        anim.ResetTrigger("Sliding");
        anim.SetTrigger("Jump");
        //    anim.SetFloat("VerticalVelocity", 1f);
        //    anim.SetBool("isGrounded", false);
    }
    void HandleGround(bool Grounded)
    {
        anim.SetBool("isGrounded", Grounded);
        if (Grounded)
        {
            isJumping = false;
            anim.ResetTrigger("Jump");
        }
    }
    void OnSlide()
    {
        anim.ResetTrigger("Jump");
        anim.SetTrigger("Sliding");
        anim.SetBool("isSliding", true);
    }
    void SlideEnd()
    {
        anim.SetBool("isSliding", false);
    }
    void OnLaneChange(int dir)
    {
        if (isJumping) return;

        if (Time.time - lastChangeTime < LaneChangeCoolDown)
            return;

        lastChangeTime = Time.time;

        anim.ResetTrigger("LaneSwitch");
        anim.SetTrigger("LaneSwitch");
    }
    void OnPlayerHit()
    {
        anim.SetTrigger("Die");
    }
}
