using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    Animator anim;
    public bool isJumping;
    public bool isDead;

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
        if (isDead) return;
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
        if (isDead) return;
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
        if (isDead) return;

        if (isJumping) return;


        if (Time.time - lastChangeTime < LaneChangeCoolDown)
            return;

        lastChangeTime = Time.time;

        anim.ResetTrigger("LaneSwitch");
        anim.SetTrigger("LaneSwitch");
    }
    void OnPlayerHit()
    {
        isDead=true;
        anim.SetBool("isDead", true);
        anim.ResetTrigger("Jump");
        anim.ResetTrigger("LaneSwitch");
        anim.ResetTrigger("Sliding");
        anim.SetTrigger("Die");
    }
}
