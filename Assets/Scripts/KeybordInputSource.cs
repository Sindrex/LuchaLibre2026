using UnityEngine;

[CreateAssetMenu(menuName = "LuchaLibre/InputSource", order = 1)]
public class KeybordInputSource : InputSource
{
    public KeyCode Forwards;
    public KeyCode Backwards;
    public KeyCode GrabThrowAction;
    public KeyCode PunchAction;

    public override void Update()
    {
        DirectionInput = new Vector2
        (
            Input.GetKey(Backwards) ? -1 : Input.GetKey(Forwards) ? 1 : 0,
            0
        );

        if (Input.GetKey(GrabThrowAction))
        {
            ActionInput = FighterAction.GrabThrow;
        }
        else if (Input.GetKey(PunchAction))
        {
            ActionInput = FighterAction.Punch;
        }
        else
        {
            ActionInput = FighterAction.None;
        }

    }
}