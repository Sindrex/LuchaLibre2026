using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D.Animation;
public class RagdollToggle : MonoBehaviour
{
    public class BodyPartCombo
    {
        public GameObject GameObject { get; set; }
        public SpriteSkin Skin { get; set; }
        public HingeJoint2D Joint { get; set; }
        public Rigidbody2D Body { get; set; }
        public Collider2D Collider { get; set; }

        public Vector2 TransformPosition { get; set; }
        public float TransformAngle { get; set; }

        public Vector2 BodyPosition { get; set; }
        public float BodyAngle { get; set; }

        public Vector2 EndSimBodyPosition { get; set; }
        public float EndSimBodyAngle { get; set; }

        public Vector3 BonePosition { get; set; }

        public float BoneAngle { get; set; }
    }

    private bool isOn = false;
    public bool IsOn => isOn;

    private readonly List<BodyPartCombo> bodyParts = new List<BodyPartCombo>();

    private bool isLerpingBack = false;

    private Rigidbody2D root;

    void Start()
    {
        var log = new StringBuilder();

        var spriteSkins = GetComponentsInChildren<SpriteSkin>();
        foreach (var skin in spriteSkins)
        {
            var body = skin.gameObject.GetComponent<Rigidbody2D>();
            var joint = skin.gameObject.GetComponent<HingeJoint2D>();

            body.simulated = false;

            if (body.gameObject.name == "Pelvis")
            {
                root = body;
            }

            log.AppendLine($"{body.gameObject.name} starts at {body.position} ({body.rotation})");

            var part = new BodyPartCombo
            {
                GameObject = skin.gameObject,

                TransformPosition = body.transform.position,
                TransformAngle = body.transform.rotation.z,

                Body = body,
                BodyPosition = body.position,
                BodyAngle = body.rotation,

                Skin = skin,
                BonePosition = skin.boneTransforms[0].position,
                BoneAngle = skin.boneTransforms[0].rotation.z,

                Joint = joint,
                Collider = skin.gameObject.GetComponent<Collider2D>()
            };
            bodyParts.Add(part);
        }

        Debug.Log(log.ToString());
    }

    //void Update()
    //{
    //    if (isOn || isLerpingBack) return;
    //    foreach (var part in bodyParts)
    //    {
    //        part.BodyPosition = part.Body.position;
    //        part.BodyAngle = part.Body.rotation;
    //    }
    //}

    public void Toggle(Action outerToggle = null, Action<Vector2> setOuterPosition = null)
    {
        if (isLerpingBack) return;

        var spriteSkins = GetComponentsInChildren<SpriteSkin>();
        Debug.Log($"Toggling SpriteSkins from {!isOn} to {isOn} for {spriteSkins.Length}");
        isOn = !isOn;

        if (!isOn)
        {
            isLerpingBack = true;
            StartCoroutine(LerpBack(outerToggle, setOuterPosition));
        }
        else
        {
            outerToggle?.Invoke();
            foreach (var bp in bodyParts)
            {
                bp.Skin.enabled = false;
                bp.Body.simulated = true;
            }
        }
    }

    private IEnumerator LerpBack(Action outerToggle = null, Action<Vector2> setOuterPosition = null)
    {
        var duration = 0.25f;
        var current = Time.time;
        var end = current + duration;

        var log = new StringBuilder();

        foreach (var bp in bodyParts)
        {
            bp.Body.gravityScale = 0;
            bp.EndSimBodyAngle = bp.Body.rotation;
            bp.EndSimBodyPosition = bp.Body.position;

            log.AppendLine($"{bp.GameObject.name} to lerp back from {bp.EndSimBodyPosition} ({bp.EndSimBodyAngle}) to {bp.BodyPosition} ({bp.BodyAngle})");
        }

        Debug.Log(log.ToString());

        while (current < end)
        {
            var t = (duration - (end - current)) / duration;

            foreach (var bp in bodyParts)
            {
                bp.Body.position = Vector2.Lerp(bp.EndSimBodyPosition, bp.BodyPosition, t);
                bp.Body.rotation = Mathf.Lerp(bp.EndSimBodyAngle, bp.BodyAngle, t);
            }

            setOuterPosition?.Invoke(new Vector2(root.position.x, 2));
            
            yield return new WaitForNextFrameUnit();

            current = Time.time;
        }

        foreach (var bp in bodyParts)
        {
            bp.Body.position = bp.BodyPosition;
            bp.Body.rotation = bp.BodyAngle;
            bp.Body.simulated = false;
            bp.Body.gravityScale = 1;
            bp.Skin.enabled = true;
        }

        isLerpingBack = false;

        outerToggle?.Invoke();
    }
}
