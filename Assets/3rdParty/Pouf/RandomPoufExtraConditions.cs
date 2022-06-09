using UnityEngine;
using System.Collections;
using Effects;

public class RandomPoufExtraConditions : MonoBehaviour
{
    public Collider thisCollider;

    public float poufSize = 0.5f;

    public string[] noPoufCollider;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts[0].thisCollider == thisCollider)
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
                HitEffects.SpawnEffect(collision.contacts[0].point, collision.contacts[0].normal * collision.relativeVelocity.magnitude, EffectType.Pouf, poufSize);

        }

    }
}
