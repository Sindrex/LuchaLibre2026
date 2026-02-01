using UnityEngine;

public class GameController : MonoBehaviour
{
    public float gameSpeed = 1f;

    void Start()
    {
        Time.timeScale = gameSpeed;
    }
}
