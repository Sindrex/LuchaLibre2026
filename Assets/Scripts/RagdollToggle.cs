using System;
using System.Collections;
using System.Collections.Generic;
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
        public Rigidbody2D RigidBody { get; set; }
        public Collider2D Collider { get; set; }

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
    
    void Start()
    {
        var spriteSkins = GetComponentsInChildren<SpriteSkin>();
        foreach (var skin in spriteSkins)
        {
            var body = skin.gameObject.GetComponent<Rigidbody2D>();
            var joint = skin.gameObject.GetComponent<HingeJoint2D>();

            body.simulated = false;
            
            var obj = new BodyPartCombo
            {
                GameObject = skin.gameObject,

                RigidBody = body,
                BodyPosition = body.position,
                BodyAngle = body.rotation,

                Skin = skin,
                BonePosition = skin.boneTransforms[0].position,
                BoneAngle = skin.boneTransforms[0].rotation.z,

                Joint = joint,
                Collider = skin.gameObject.GetComponent<Collider2D>()
            };
            bodyParts.Add(obj);
        }
    }

    public void Toggle(Action outerToggle = null)
    {
        if (isLerpingBack) return;

        var spriteSkins = GetComponentsInChildren<SpriteSkin>();
        Debug.Log($"Toggling SpriteSkins from {!isOn} to {isOn} for {spriteSkins.Length}");
        isOn = !isOn;

        if (!isOn)
        {
            isLerpingBack = true;
            StartCoroutine(LerpBack(outerToggle));
        }
        else
        {
            outerToggle?.Invoke();
            foreach (var bp in bodyParts)
            {
                bp.Skin.enabled = false;
                bp.RigidBody.simulated = true;
            }
        }
    }

    private IEnumerator LerpBack(Action outerToggle = null)
    {
        var duration = 0.25f;
        var current = Time.time;
        var end = current + duration;

        foreach (var bp in bodyParts)
        {
            bp.RigidBody.gravityScale = 0;
            bp.EndSimBodyAngle = bp.RigidBody.rotation;
            bp.EndSimBodyPosition = bp.RigidBody.position;
        }

        while (current < end)
        {
            var t = (duration - (end - current)) / duration;

            foreach (var bp in bodyParts)
            {
                bp.RigidBody.position = Vector2.Lerp(bp.EndSimBodyPosition, bp.BodyPosition, t);
                bp.RigidBody.rotation = Mathf.Lerp(bp.EndSimBodyAngle, bp.BodyAngle, t);
            }

            yield return new WaitForNextFrameUnit();

            current = Time.time;
        }

        foreach (var bp in bodyParts)
        {
            bp.RigidBody.simulated = false;
            bp.RigidBody.gravityScale = 1;
            bp.Skin.enabled = true;
        }

        isLerpingBack = false;

        outerToggle?.Invoke();
    }
}
