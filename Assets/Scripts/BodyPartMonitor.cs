using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class BodyPartMonitor : MonoBehaviour
{
    public Vector2 PositionOffset;
    public float AngleOffset;
    
    public Vector2 TransformPosition;
    public float TransformRotation;
    public Vector2 SkinPosition;
    public float SkinRotation;
    public Vector2[] BonePositions;
    public float[] BoneRotations;
    public Vector2 BodyPosition;
    public float BodyRotation;

    public Vector2 ColliderOffset;

    private SpriteSkin spriteSkin;
    private Rigidbody2D body;

    // Start is called before the first frame update
    void Start()
    {
        spriteSkin = GetComponent<SpriteSkin>();
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        PositionOffset = body.position - (Vector2)spriteSkin.boneTransforms[0].position;
        AngleOffset = body.rotation - spriteSkin.boneTransforms[0].rotation.eulerAngles.z;

        TransformPosition = transform.position;
        TransformRotation = transform.rotation.eulerAngles.z;
        SkinPosition = spriteSkin.transform.position;
        SkinRotation = spriteSkin.transform.rotation.eulerAngles.z;
        BodyPosition = body.position;
        BodyRotation = body.rotation;
        BonePositions = spriteSkin.boneTransforms.Select(x => (Vector2)x.position).ToArray();
        BoneRotations = spriteSkin.boneTransforms.Select(x => x.rotation.eulerAngles.z).ToArray();
        ColliderOffset = GetComponent<Collider2D>().offset;
    }
}
