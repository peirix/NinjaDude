using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D body;
    CapsuleCollider2D bodyCollider;
    Vector2 moveInput;
    bool isAlive = true;
    float slideTime = 0.4f;
    float timeRemaining = 0;
    Animator animator;
    float fallingTime = 0;
    float lastCheck = 0;
    bool canJump = false;
    bool isWallSliding;

    [SerializeField]float moveSpeed = 2f;
    [SerializeField]float slideSpeed = 20f;
    [SerializeField]float jumpSpeed = 5f;
    [SerializeField]float fallCutoff = 0.05f;
    [SerializeField]Transform groundCheck;
    [SerializeField]LayerMask groundLayer;
    [SerializeField]Transform wallCheck;
    [SerializeField]float wallSlidingSpeed = 1f;
    [SerializeField]TextMeshProUGUI isWalledText;
    [SerializeField]TextMeshProUGUI isGroundedText;
    [SerializeField]TextMeshProUGUI fallTimeText;
    [SerializeField]TextMeshProUGUI velocityText;
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
        lastCheck = Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (body.velocity.y < 0) {
            fallingTime += Time.deltaTime - lastCheck;
        }
        if (fallingTime > fallCutoff) {
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", true);
        }
        fallTimeText.text = fallingTime.ToString();
        lastCheck = Time.deltaTime;
        Run();
        FlipSprite();
        WallSlide();
    }

    void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * moveSpeed, body.velocity.y);
        timeRemaining -= Time.deltaTime;
        if (timeRemaining < 0f) {
            animator.SetBool("isRolling", false);
            body.SetRotation(0);
        } else {
            playerVelocity = new Vector2(slideSpeed * body.transform.localScale.x, body.velocity.y);
        }

        animator.SetBool("isRunning", Mathf.Abs(moveInput.x) > 0);
        
        body.velocity = playerVelocity;
        velocityText.text = "x:" + body.velocity.x + ", y:" + body.velocity.y;

        // bool playerHasHorizontalSpeed = Mathf.Abs(xRigidbody.velocity.x) > Mathf.Epsilon;
        // animator.SetBool("isRunning", playerHasHorizontalSpeed);
    }

    void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(body.velocity.x) > Mathf.Epsilon;

        if (playerHasHorizontalSpeed) {
            body.transform.localScale = new(Mathf.Sign(body.velocity.x), 1);
        }
    }

    void OnMove(InputValue value)
    {
        if (!isAlive) { return; }
        moveInput = value.Get<Vector2>();
    }

    void OnSlide(InputValue value)
    {
        if (value.isPressed)
        {
            animator.SetBool("isRolling", true);
            timeRemaining = Time.deltaTime + slideTime;
            // body.SetRotation(90 * (bodyRender.flipX ? -1 : 1));
            body.velocity = new(slideSpeed, 0);
        }
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed && canJump)
        {
            canJump = false;
            animator.SetBool("isJumping", true);
            body.velocity = new(0, jumpSpeed);
        }
    }

    private bool IsGrounded()
    {
        bool isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        isGroundedText.text = isGrounded ? "Ja" : "Nei";
        if (isGrounded && fallingTime != 0) {
            canJump = true;
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", false);
            fallingTime = 0;
        }
        return isGrounded;
    }

    private bool IsWalled()
    {
        bool isWalled = Physics2D.OverlapCircle(wallCheck.position, 0.2f, groundLayer);
        
        animator.SetBool("isWalled", isWalled);
        isWalledText.text = isWalled ? "Ja" : "Nei";
        return isWalled;
    }

    private void WallSlide()
    {
        if (!IsGrounded() && IsWalled() && body.velocity.x != 0f)
        {
            isWallSliding = true;
            body.velocity = new (body.velocity.x, Mathf.Clamp(body.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }
}
