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
    /*
     * Her er det helt sikkert masse mere greier vi kan slette.
     * Cluet ligger i å oppdatere transform til bone + initial offset.
     */

    public class BodyPartCombo
    {
        public GameObject GameObject { get; set; }
        public SpriteSkin Skin { get; set; }
        public HingeJoint2D Joint { get; set; }
        public Rigidbody2D Body { get; set; }
        public Collider2D Collider { get; set; }

        public Vector2 PositionOffset { get; set; }
        public float AngleOffset { get; set; }

        public Vector2 TransformPosition { get; set; }
        public float TransformAngle { get; set; }

        public Vector2 InitialBodyPosition { get; set; }
        public float InitialBodyAngle { get; set; }

        public Vector2 AnimatedBodyPosition { get; set; }
        public float AnimatedBodyAngle { get; set; }

        public Vector2 EndSimBodyPosition { get; set; }
        public float EndSimBodyAngle { get; set; }

        public Vector3 BonePosition { get; set; }

        public float BoneAngle { get; set; }
    }

    private bool isOn = false;
    public bool IsOn => isOn;

    private readonly List<BodyPartCombo> bodyParts = new List<BodyPartCombo>();

    private bool isLerpingBack = false;

    private bool shouldLog = false;

    public Vector2[] initialOffsets;

    public BodyPartCombo[] allCombos;

    void Start()
    {
        shouldLog = false; // transform.parent.name == "Fighter A";

        var log = new StringBuilder();

        var spriteSkins = GetComponentsInChildren<SpriteSkin>();
        foreach (var skin in spriteSkins)
        {
            var body = skin.gameObject.GetComponent<Rigidbody2D>();
            var joint = skin.gameObject.GetComponent<HingeJoint2D>();

            body.simulated = false;

            if (body.gameObject.name == "Pelvis")
                log.AppendLine($"{body.gameObject.name} starts at {body.position} ({body.rotation})");

            var collider = skin.gameObject.GetComponent<Collider2D>();
            var part = new BodyPartCombo
            {
                GameObject = skin.gameObject,

                PositionOffset = body.position - (Vector2)skin.boneTransforms[0].position,
                AngleOffset = body.rotation - skin.boneTransforms[0].rotation.eulerAngles.z,

                TransformPosition = body.transform.position,
                TransformAngle = body.transform.rotation.eulerAngles.z,

                Body = body,
                InitialBodyPosition = body.position,
                InitialBodyAngle = body.rotation,
                AnimatedBodyPosition = body.position,
                AnimatedBodyAngle = body.rotation,

                Skin = skin,

                BonePosition = skin.boneTransforms[0].position,
                BoneAngle = skin.boneTransforms[0].rotation.eulerAngles.z,

                Joint = joint,
                Collider = collider
            };

            
            bodyParts.Add(part);
        }

        initialOffsets = bodyParts.Select(x => x.PositionOffset).ToArray();
        allCombos = bodyParts.ToArray();

        if (shouldLog)
            Debug.Log(log.ToString());
    }

    void Update()
    {
        foreach (var part in bodyParts)
        {
            if (!part.Body.simulated) {
                
                part.AnimatedBodyPosition = part.Skin.boneTransforms[0].position + (Vector3)part.PositionOffset;

                part.AnimatedBodyAngle = part.Skin.boneTransforms[0].rotation.eulerAngles.z + part.AngleOffset;

                part.GameObject.transform.position = part.AnimatedBodyPosition;
                part.GameObject.transform.rotation = Quaternion.Euler(0, 0, part.AnimatedBodyAngle);
            }
        }
    }

    public void Toggle(Action outerToggle = null)
    {
        if (isLerpingBack) return;

        isOn = !isOn;

        if (!isOn)
        {
            isLerpingBack = true;
            StartCoroutine(LerpBack(outerToggle));
        }
        else
        {
            var sb = new StringBuilder();
            
            outerToggle?.Invoke();
            foreach (var bp in bodyParts)
            {
                bp.Skin.enabled = false;

                if (bp.GameObject.name == "Pelvis")
                    sb.AppendLine($"{bp.GameObject.name} initialized to {bp.AnimatedBodyPosition} ({bp.AnimatedBodyAngle})");

                bp.Body.simulated = true;
            }

            if (shouldLog)
                Debug.Log(sb.ToString());
        }
    }

    private IEnumerator LerpBack(Action outerToggle = null)
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

            if (bp.GameObject.name == "Pelvis")
                log.AppendLine($"{bp.GameObject.name} to lerp back from {bp.EndSimBodyPosition} ({bp.EndSimBodyAngle}) to {bp.AnimatedBodyPosition} ({bp.AnimatedBodyAngle})");
        }

        if (shouldLog)
            Debug.Log(log.ToString());

        while (current < end)
        {
            var t = (duration - (end - current)) / duration;

            foreach (var bp in bodyParts)
            {
                bp.Body.position = Vector2.Lerp(bp.EndSimBodyPosition, bp.AnimatedBodyPosition, t);
                bp.Body.rotation = Mathf.Lerp(bp.EndSimBodyAngle, bp.AnimatedBodyAngle, t);
            }
            
            yield return new WaitForNextFrameUnit();

            current = Time.time;
        }

        log.Clear();

        foreach (var bp in bodyParts)
        {
            bp.Body.position = bp.AnimatedBodyPosition;
            bp.Body.rotation = bp.AnimatedBodyAngle;
            bp.Body.simulated = false;
            bp.Body.gravityScale = 1;
            bp.Skin.enabled = true;

            if (bp.GameObject.name == "Pelvis")
               log.AppendLine($"{bp.GameObject.name} end lerp at {bp.AnimatedBodyPosition} ({bp.AnimatedBodyAngle})");
        }

        if (shouldLog)
            Debug.Log(log.ToString());
 
        isLerpingBack = false;

        outerToggle?.Invoke();
    }
}
