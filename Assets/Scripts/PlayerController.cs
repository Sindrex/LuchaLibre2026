using UnityEngine;

public interface IInputSource
{

}

public class PlayerController : MonoBehaviour
{
    public GameObject rootSprite;

    public InputSource inputSource;
    public float moveSpeed;

    public Vector2 currentForce;

    public Vector2 direction;
    public float addForce;

    private Rigidbody2D spriteBody;

    void Start()
    {
        spriteBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        inputSource.Update();

        direction = inputSource.DirectionInput;
        addForce = direction.x * moveSpeed;
        spriteBody.AddForce(transform.forward * addForce, ForceMode2D.Force);
        spriteBody.totalForce = new Vector2(Mathf.Clamp(spriteBody.totalForce.x, -1, 1), spriteBody.totalForce.y);
        currentForce = spriteBody.totalForce;
    }
}
