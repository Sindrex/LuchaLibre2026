using UnityEngine;

public enum InputPurpose
{
    ANY, QUIT, START_GAME,
    P1_MOVE_UP, P1_MOVE_DOWN, P1_MOVE_LEFT, P1_MOVE_RIGHT,
    P1_PUNCH_1,
    P2_MOVE_UP, P2_MOVE_DOWN, P2_MOVE_LEFT, P2_MOVE_RIGHT,
    P2_PUNCH_1
}

public static class InputController
{
    public static bool InputEnabled = true;

    public static bool GetInput(InputPurpose purpose)
    {
        if(!InputEnabled)
        {
            return false;
        }

        switch (purpose)
        {
            case InputPurpose.P1_MOVE_UP:
                return Input.GetKey(KeyCode.W);
            case InputPurpose.P1_MOVE_DOWN:
                return Input.GetKey(KeyCode.S);
            case InputPurpose.P1_MOVE_LEFT:
                return Input.GetKey(KeyCode.A);
            case InputPurpose.P1_MOVE_RIGHT:
                return Input.GetKey(KeyCode.D);
            case InputPurpose.P1_PUNCH_1:
                return Input.GetKeyDown(KeyCode.O);
            case InputPurpose.P2_MOVE_UP:
                return Input.GetKey(KeyCode.UpArrow);
            case InputPurpose.P2_MOVE_DOWN:
                return Input.GetKey(KeyCode.DownArrow);
            case InputPurpose.P2_MOVE_LEFT:
                return Input.GetKey(KeyCode.LeftArrow);
            case InputPurpose.P2_MOVE_RIGHT:
                return Input.GetKey(KeyCode.RightArrow);
            case InputPurpose.P2_PUNCH_1:
                return Input.GetKeyDown(KeyCode.P);
            case InputPurpose.ANY:
                return Input.anyKeyDown;
            case InputPurpose.QUIT:
                return Input.GetKeyDown(KeyCode.Escape);
            case InputPurpose.START_GAME:
                return Input.GetKeyDown(KeyCode.Space);
        }
        return false;
    }
}
