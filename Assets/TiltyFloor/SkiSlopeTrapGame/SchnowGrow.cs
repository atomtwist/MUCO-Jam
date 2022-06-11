using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Normal.Realtime;
using UnityEngine;

public class SchnowGrow : MonoBehaviour
{
    private HashSet<Transform> floor = new HashSet<Transform>();

    public Rigidbody rb;
    public float maxSpeed = 10f;
    public float growthRate = 1f;
    public AnimationCurve growRateCurve = AnimationCurve.Linear(0, 0, 1f, 1f);
    public float maxScale = 2f;

    [Space] public float destroyVel = 10f;
    public float destroyVelHead = 10f;

    public GameObject particleEffectOnDestroy;

    private void OnCollisionEnter(Collision other)
    {
        var didHitVelThreshold = false;


        // if we hit terrain
        if (other.gameObject.GetComponentInParent<SnowTerrain>())
        {
            floor.Add(other.transform);

            // if velocity, destroy.
            if (other.relativeVelocity.magnitude > destroyVel)
                didHitVelThreshold = true;
        }

        var ff = other.gameObject.GetComponent<FlashOnFaceHit>();
        if (ff != null)
        {
            if (other.relativeVelocity.magnitude > destroyVelHead)
            {
                didHitVelThreshold = true;
                // we hit velocity thershold on face. do a face flash.
                ff.FlashIt();
            }
        }

        if (didHitVelThreshold)
        {
            if (particleEffectOnDestroy != null)
            {
                var pp = Instantiate(particleEffectOnDestroy, transform.position, transform.rotation, null);
                pp.transform.localScale = transform.localScale;
            }

            Realtime.Destroy(gameObject);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.GetComponentInParent<SnowTerrain>())
        {
            floor.Remove(other.transform);
        }
    }

    private void Update()
    {
        if (floor.Any())
        {
            // grow... with velocity
            var speed = rb.velocity.magnitude;
            var s01 = speed / maxSpeed;
            var curScale = transform.localScale.x / maxScale;
            transform.localScale += Vector3.one * growthRate * s01 * Time.deltaTime * growRateCurve.Evaluate(curScale);
        }
    }
}