using UnityEngine;

[CreateAssetMenu(menuName = "LuchaLibre/DummyInputSource", order = 1)]
public class DummyInputSource : InputSource
{
    public override void Update()
    {
        DirectionInput = new Vector2(0, 0);
    }
}