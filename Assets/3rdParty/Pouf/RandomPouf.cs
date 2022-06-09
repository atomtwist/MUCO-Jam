using UnityEngine;
using System.Collections;
using Effects;

public class RandomPouf : MonoBehaviour
{
    public float poufSize = 0.5f;
    
    public string[] noPoufCollider;

    void OnCollisionEnter(Collision collision)
    {
        bool canPouf = true;

        for (int i = 0; i < noPoufCollider.Length; i++)
        {
            if (collision.collider.gameObject.tag == noPoufCollider[i])
                canPouf = false;

            if (canPouf && collision.rigidbody != null && collision.rigidbody.gameObject.tag == noPoufCollider[i])
                canPouf = false;

            if (!canPouf)
                break;
        }


        if (canPouf)
            HitEffects.SpawnEffect(collision.contacts[0].point, collision.contacts[0].normal * collision.impactForceSum.magnitude, EffectType.Pouf, poufSize);




    }
}
