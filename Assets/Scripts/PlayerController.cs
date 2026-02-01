using System;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerController opponent;

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

    private FighterAction currentAction = FighterAction.None;
    private bool actionInProgress = false;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        body = GetComponent<Rigidbody2D>();
        ragdollToggle = GetComponentInChildren<RagdollToggle>();

        opponent = FindObjectsOfType<PlayerController>().FirstOrDefault(x => x != this);
    }

    void Update()
    {
        if (actionInProgress)
        {
            return;
        }
        
        inputSource.Update();

        if (inputSource.ActionInput != FighterAction.None)
        {
            if (Vector2.Distance(transform.position, opponent.transform.position) < 2f)
            {
                // TODO: Check if opponent is blocking

                actionInProgress = true;
                currentAction = inputSource.ActionInput;
                animator.SetTrigger(currentAction.ToString());
                opponent.ReceiveAction(currentAction);
            }
            else
            {
                actionInProgress = true;
                currentAction = Enum.Parse<FighterAction>(inputSource.ActionInput.ToString() + "Miss");
                animator.SetTrigger(currentAction.ToString());
            }

            return;
        }

        direction = inputSource.DirectionInput;
        addForce = direction * acceleration;

        if (Input.GetKeyUp(KeyCode.Space))
        {
            ragdollToggle.Toggle(() =>
            {
                body.simulated = !ragdollToggle.IsOn;
            });
        }
    }

    private void ReceiveAction(FighterAction receivedAction)
    {
        Debug.Log("Should receive action " + receivedAction);
    }

    private void FixedUpdate()
    {
        body.AddForce(addForce, ForceMode2D.Force);
        body.velocity = Vector2.ClampMagnitude(body.velocity, maxVelocity);

        currentForce = body.totalForce;
        currentVelocity = body.velocity;

        animator.SetFloat("xVelocity", body.velocity.x);
    }

    public void Done()
    {
        if (actionInProgress)
        {
            actionInProgress = false;
            currentAction = FighterAction.None;
        }
    }

    public void Grab()
    {
        Debug.Log($"{gameObject.name} grabs opponent");
    }

    public void Throw()
    {
        Debug.Log($"{gameObject.name} throws opponent");
    }
}
