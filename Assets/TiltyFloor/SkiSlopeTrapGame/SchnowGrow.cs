using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Normal.Realtime;
using UnityEngine;

public class SchnowGrow : MonoBehaviour
{
    private HashSet<Transform> floor = new HashSet<Transform>();

    public GameObject terrain => SnowTerrain.instance.gameObject;

    public Rigidbody rb;
    public float maxSpeed = 10f;
    public float growthRate = 1f;
    public AnimationCurve growRateCurve = AnimationCurve.Linear(0, 0, 1f, 1f);
    public float maxScale = 2f;

    [Space] public float destroyVel = 10f;

    public GameObject particleEffectOnDestroy;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject == terrain)
        {
            floor.Add(other.transform);
        }
        

        // if vel > destroy limit
        // DESTROY\111
        // WITH PARTICLES N SOUND
        if (other.relativeVelocity.magnitude > destroyVel)
        {
            if (particleEffectOnDestroy != null)
            {
                var pp = Instantiate(particleEffectOnDestroy, transform.position, transform.rotation, null);
                pp.transform.localScale = transform.localScale;
            }

            var ff = other.gameObject.GetComponent<FlashOnFaceHit>();
            if (ff!=null)
            {
                ff.FlashIt();
            }

            Realtime.Destroy(gameObject);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject == terrain)
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