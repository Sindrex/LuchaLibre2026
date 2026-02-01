using UnityEngine;

public abstract class InputSource : ScriptableObject
{
    public Vector2 DirectionInput { get; protected set; }
    public FighterAction ActionInput { get; protected set; }
    public abstract void Update();
}

public enum FighterAction
{
    None,
    GrabThrow,
    GrabThrowMiss
}