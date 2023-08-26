using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D body;
    Vector2 moveInput;
    bool isAlive = true;
    float slideTime = 0.4f;
    float timeRemaining = 0;

    [SerializeField]
    float moveSpeed = 2f;

    [SerializeField]
    float slideSpeed = 20f;

    [SerializeField]
    float jumpSpeed = 5f;
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Run();
        FlipSprite();
    }

    void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * moveSpeed, body.velocity.y);
        timeRemaining -= Time.deltaTime;
        if (timeRemaining < 0f) {
            body.SetRotation(0);
        } else {
            playerVelocity = new Vector2(slideSpeed * transform.localScale.x, body.velocity.y);
        }
        
        body.velocity = playerVelocity;

        // bool playerHasHorizontalSpeed = Mathf.Abs(xRigidbody.velocity.x) > Mathf.Epsilon;
        // animator.SetBool("isRunning", playerHasHorizontalSpeed);
    }

    void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(body.velocity.x) > Mathf.Epsilon;

        if (playerHasHorizontalSpeed) {
            transform.localScale = new Vector2(Mathf.Sign(body.velocity.x), 1f);
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
            timeRemaining = Time.deltaTime + slideTime;
            body.SetRotation(90 * transform.localScale.x);
            body.velocity = new(slideSpeed, 0);
        }
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            body.velocity = new(0, jumpSpeed);
        }
    }
}
