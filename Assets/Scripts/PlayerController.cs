using System;
using System.Collections;
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
    public Vector2 pelvisForce;

    private Animator animator;

    private Rigidbody2D body;
    private RagdollToggle ragdollToggle;

    private FighterAction currentAction = FighterAction.None;
    private bool actionInProgress = false;
    private bool takingDamage = false;
    private Rigidbody2D pelvisBody;

    private GameManager manager;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        body = GetComponent<Rigidbody2D>();
        ragdollToggle = GetComponentInChildren<RagdollToggle>();
        pelvisBody = rootSprite.transform.Find("Pelvis").GetComponent<Rigidbody2D>();

        opponent = FindObjectsOfType<PlayerController>().FirstOrDefault(x => x != this);

        manager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (actionInProgress || takingDamage || !manager.GameStarted)
        {
            return;
        }

        inputSource.Update();

        if (inputSource.ActionInput != FighterAction.None)
        {
            var inRange = Vector2.Distance(transform.position, opponent.transform.position) < 2f;
            if
            (
                inRange &&
                !opponent.actionInProgress
            )
            {
                Debug.Log($"{name} attacks {opponent.name}");

                actionInProgress = true;
                currentAction = inputSource.ActionInput;
                if (currentAction == FighterAction.Unmask)
                {
                    if (manager.IsInMaskality)
                    {
                        animator.SetTrigger(currentAction.ToString());
                        opponent.ReceiveAction(currentAction);
                    }
                    else
                    {
                        actionInProgress = false;
                        currentAction = FighterAction.None;
                    }
                }
                else
                {
                    animator.SetTrigger(currentAction.ToString());
                    opponent.ReceiveAction(currentAction);
                }
            }
            else
            {
                Debug.Log($"{name} is too far away");

                actionInProgress = true;
                if (!Enum.TryParse(inputSource.ActionInput.ToString() + "Miss", out currentAction))
                    currentAction = Enum.Parse<FighterAction>(inputSource.ActionInput.ToString());
                animator.SetTrigger(currentAction.ToString());
            }

            return;
        }

        direction = inputSource.DirectionInput;
        addForce = direction * acceleration;
    }

    private void ReceiveAction(FighterAction receivedAction)
    {
        Debug.Log(name + " should receive action " + receivedAction);

        if (receivedAction == FighterAction.GrabThrow)
        {
            takingDamage = true;
            StartThrown();
        }
        else if (receivedAction == FighterAction.Punch)
        {
            takingDamage = true;
            animator.SetTrigger("Hit");
            TakeDamage(10);
        }
    }

    private void StartThrown()
    {
        animator.SetTrigger("Thrown");
    }

    private void FixedUpdate()
    {
        body.AddForce(addForce, ForceMode2D.Force);
        body.velocity = Vector2.ClampMagnitude(body.velocity, maxVelocity);

        currentForce = body.totalForce;
        currentVelocity = body.velocity;

        if (!actionInProgress)
        {
            animator.SetFloat("xVelocity", body.velocity.x * transform.localScale.x);
        }

        if (pelvisForce != Vector2.zero)
        {
            pelvisBody.AddForce(pelvisForce, ForceMode2D.Impulse);
            pelvisForce = Vector2.zero;
        }
    }

    public void Done()
    {
        Debug.Log($"{name} done with action");

        if (actionInProgress)
        {
            actionInProgress = false;
            currentAction = FighterAction.None;
            Debug.Log($"{name} reset state");
        }
        else if (takingDamage && !ragdollToggle.IsOn)
        {
            takingDamage = false;
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

    public void Ragdoll()
    {
        Debug.Log(name + " Received Ragdoll trigger");

        TakeDamage(20);

        foreach (var child in rootSprite.GetComponentsInChildren<Collider2D>())
        {
            foreach (var otherChild in opponent.rootSprite.GetComponentsInChildren<Collider2D>())
            {
                Physics2D.IgnoreCollision(child, otherChild, true);
            }
        }

        ragdollToggle.Toggle(() =>
        {
            body.simulated = !ragdollToggle.IsOn;
        });

        pelvisForce = new Vector2(transform.localScale.x * -50, 0);
        Debug.Log($"Added full force");

        StartCoroutine(FlyABitAndGetUp());
    }

    private void TakeDamage(int damage)
    {
        SendMessageUpwards("Hit", new Tuple<PlayerController, int>(this, damage), SendMessageOptions.DontRequireReceiver);
    }

    private IEnumerator FlyABitAndGetUp()
    {
        yield return new WaitForFixedUpdate();

        while (pelvisBody.velocity.magnitude > .1f)
        {
            yield return new WaitForEndOfFrame();
        }

        var xOffset = pelvisBody.transform.position.x - transform.position.x;
        transform.position = new Vector3(transform.position.x + xOffset, transform.position.y, transform.position.z);
        ragdollToggle.Shift(new Vector2(xOffset, 0));

        ragdollToggle.Toggle(() =>
        {
            body.simulated = !ragdollToggle.IsOn;
        });

        foreach (var child in rootSprite.GetComponentsInChildren<Collider2D>())
        {
            foreach (var otherChild in opponent.rootSprite.GetComponentsInChildren<Collider2D>())
            {
                Physics2D.IgnoreCollision(child, otherChild, false);
            }
        }

        Debug.Log($"{name} got back up");

        takingDamage = false;
    }
}
