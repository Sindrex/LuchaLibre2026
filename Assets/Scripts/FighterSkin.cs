using UnityEngine;

public class FighterSkin : MonoBehaviour
{
    public GameObject mainContainer;
    public GameObject spriteSheet;

    // Start is called before the first frame update
    void Start()
    {
        var renderers = mainContainer.GetComponentsInChildren<SpriteRenderer>();
        foreach (var spriteRenderer in renderers)
        {
            var name = spriteRenderer.gameObject.name;
            var spriteObj = spriteSheet.transform.Find(name);
            var sprite = spriteObj.GetComponent<SpriteRenderer>().sprite;
            spriteRenderer.sprite = sprite;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
