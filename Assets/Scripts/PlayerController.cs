using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject rootSprite;

    public InputSource inputSource;
    public float acceleration;
    public float maxVelocity;

    public Vector2 currentForce;
    public Vector2 currentVelocity;

    public Vector2 direction;
    public Vector2 addForce;

    private Animator animator;

    private Rigidbody2D body;
    private RagdollToggle ragdollToggle;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();

        body = GetComponent<Rigidbody2D>();

        ragdollToggle = GetComponentInChildren<RagdollToggle>();
    }

    void Update()
    {
        inputSource.Update();

        direction = inputSource.DirectionInput;
        addForce = direction * acceleration;

        if (Input.GetKeyUp(KeyCode.Space))
        {
            ragdollToggle.Toggle(() =>
            {
                body.simulated = !ragdollToggle.IsOn;
            }, (pos) =>
            {
                //transform.position = new Vector2(pos.x, transform.position.y);
            });
        }
    }

    private void FixedUpdate()
    {
        body.AddForce(addForce, ForceMode2D.Force);
        body.velocity = Vector2.ClampMagnitude(body.velocity, maxVelocity);

        currentForce = body.totalForce;
        currentVelocity = body.velocity;

        animator.SetFloat("xVelocity", body.velocity.x);
    }
}
