using System;
using System.Collections;
using UnityEngine;

public class movement : MonoBehaviour
{
    //-------------------
    Animator animator;
    //------dashvar--------
    private bool dashbutton;
    [SerializeField] TrailRenderer tr;
    public bool candash;
    private bool isdashing;
    private float dashcd = 1f;
    private float dashtime = .25f;
    public float dashspeed = 20f;
    //------move var-------
    [SerializeField] Rigidbody2D rb;
    float movedir;
    public float movespeed = 5f;
    public float jumpspeed = 5f;
    public bool OnGround;
    public Transform groundCheck;
    public LayerMask GroundLayer;
    public LayerMask padlayer;
    private float coyotetime = 0.2f;
    private bool jumpbutton;
    private float coyotecounter;
    private float jumpbuffertime = 0.2f;
    private float jumpbuffercounter;
    public int candoublejump;
    bool isFacingRight;
    //------------------wallvar
    public float walljumpingduration=1f;
    private float wallCounter;
    private float walltime=0.5f;
    public bool isWallSliding;
    private float wallSlidingSpeed = 7f;
    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private Vector2 wallJumpingPower = new Vector2(25f,40f);
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    //-----------------------------
    private void Start()
    {
        animator=GetComponent<Animator>();
    }
    private void Awake()
    {
        candash = true;
    }
    //-----------------------------
    void Update()
    {
        //-----animations
        animator.SetFloat("yVelocity", rb.velocity.y);
        animator.SetFloat("xVelocity", Math.Abs(rb.velocity.x));
        if (rb.velocity.y > 0.1f)
        {
            animator.SetTrigger("jump");
        }
        animator.SetBool("onground", OnGround);
        //-----------------------
        jumpbutton = Input.GetButtonDown("Jump");
        dashbutton = Input.GetKeyDown(KeyCode.X);
        //horizonal movement stuff

        OnGround = Physics2D.OverlapCircle(groundCheck.position, .2f, GroundLayer);
        animator.SetBool("onground",OnGround);
        movedir = Input.GetAxisRaw("Horizontal");

        if (OnGround || platcoll())
        {
            candoublejump = 0;
        }
        //----jump stuff

        if (!OnGround && jumpbutton && candoublejump < 1)
        {
            jump();
            candoublejump += 1;
        }

        if (OnGround && !jumpbutton)
        {
            coyotecounter = coyotetime;
        }

        else
        {
            coyotecounter -= Time.fixedDeltaTime;
        }

        if (jumpbutton)
        {
            jumpbuffercounter = jumpbuffertime;
        }
        else
        {
            jumpbuffercounter -= Time.fixedDeltaTime;
        }

        if (jumpbuffercounter > 0f && coyotecounter > 0f)
        {
            jump();
            candoublejump += 1;
            jumpbuffercounter = 0f;
        }

        if (jumpbutton && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.8f);
            coyotecounter = 0f;
        }

        //------dash stuff
        if (candash && dashbutton)
        {

            StartCoroutine(dash());

        }
        //--------------wall stuff calls
        WallSlide();
        WallJump();
        if (!isWallJumping)
        {
            Flip();
        }
    }
    //------wall stuff-------------------------------------------------------------------
    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.1f, wallLayer);
    }

    private void WallSlide()
    {
        if(!IsWalled())
        {
            isWallSliding = false;
        }
        if (IsWalled() && !OnGround && movedir != 0f)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, -wallSlidingSpeed);
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }
        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;
            if (transform.localScale.x != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector2 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }
        }
    }
    //--------------------------------------------------------------------------
    void jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpspeed);
    }
    private void FixedUpdate()
    {
        
        //------------------------------
        if (isdashing)
        {
            return;
        }
        if (!isWallJumping)
        {
            rb.velocity = new Vector2(movespeed * movedir, rb.velocity.y);
        }
    }

    bool platcoll()
    {
        return Physics2D.OverlapCircle(groundCheck.position, .1f, padlayer);
    }

    private void Flip()
    {
        if (isFacingRight && movedir < 0f || !isFacingRight && movedir > 0f)
        {
            Vector3 localScale = transform.localScale;
            isFacingRight = !isFacingRight;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void dashf()
    {
        if (Input.GetAxisRaw("Vertical") == 0)
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            if (rb.transform.localScale.x > 0)
            {
                rb.velocity = new Vector2(-dashspeed, 0f);
            }
            else
            {
                rb.velocity = new Vector2(dashspeed, 0f);
            }
        }
        else
        {
            //Debug.Log("diag or vert");
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.velocity=new Vector2(movedir, Input.GetAxisRaw("Vertical")) * dashspeed/Mathf.Sqrt(2);
        }
    }
    //-----dashcoroutine------------------------------------------------------
    IEnumerator dash()
    {
        candash = false;
        isdashing = true;
        rb.gravityScale = 3.8f;
        dashf();
        tr.emitting = true;
        yield return new WaitForSeconds(dashtime);
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        isdashing = false;
        tr.emitting = false;
        yield return new WaitForSeconds(dashcd);
        candash = true;
    }
    //--------------walljumpcoroutine
}
    