using UnityEngine;
using System.Collections;
using Effects;
using ToyBoxHHH;

namespace Effects
{
    public enum EffectType
    {
        None,
        Pouf,
        HeadPouf
    }
}

public class HitEffects : MonoBehaviour
{

    const int poolsize = 100;

    public GameObject pouf;
    static GameObject[] poufs = new GameObject[0];
    static PoufEffect[] poufEffects = new PoufEffect[0];

    public GameObject headPouf;
    static GameObject[] headPoufs = new GameObject[0];
    static PoufEffect[] headPoufEffects = new PoufEffect[0];

    static Vector3 lastHit;
    static float lastHitTime;

    
    [DebugButton]
    void TestPouf(float size)
    {
        SpawnEffect(transform.position,Vector3.up, EffectType.HeadPouf,size);
    }
    
    void OnEnable()
    {
        poufs = new GameObject[poolsize];
        poufEffects = new PoufEffect[poolsize];
        headPoufs = new GameObject[poolsize];
        headPoufEffects = new PoufEffect[poolsize];

        for (int i = 0; i < poolsize; i++) {
            poufs[i] = Instantiate(pouf);
            poufEffects[i] = poufs[i].GetComponent<PoufEffect>();
            poufs[i].transform.parent = transform;
            poufs[i].SetActive(false);

            headPoufs[i] = Instantiate(headPouf);
            headPoufEffects[i] = headPoufs[i].GetComponent<PoufEffect>();
            headPoufs[i].transform.parent = transform;
            headPoufs[i].SetActive(false);

        }

        lastHit = new Vector3(1000, -1000, 34.5f);
    }

    public static void SpawnEffect(Vector3 impactPosition, Vector3 impactDirection, EffectType effectType, float effectSize)
    {
        /*if (impactDirection.magnitude < 5f)
            return;*/

        if (Vector3.Distance(impactPosition, lastHit) < 0.4f && Time.realtimeSinceStartup - lastHitTime < 0.4f)
            return;

        lastHit = impactPosition;
        lastHitTime = Time.realtimeSinceStartup;

        SpawnPoufs(effectType, impactPosition, impactDirection, effectSize);
    }

    static void SpawnPoufs(EffectType effectType, Vector3 impactPosition, Vector3 impactDirection, float effectSize)
    {
        if (Application.isPlaying) {
            int poufAmount = Random.Range(8, 12);

            int[] spawnPoufs = new int[poufAmount];

            for (int i = 0; i < poufAmount; i++)
                spawnPoufs[i] = -1;

            int nr = 0;

            // choose effect type
            GameObject[] targetEffectGOs;// = poufs;
            PoufEffect[] targetPoufEffects;// = poufEffects;
            switch (effectType) {
            case EffectType.HeadPouf:
                targetEffectGOs = headPoufs;
                targetPoufEffects = headPoufEffects;
                break;
            case EffectType.Pouf:
                targetEffectGOs = poufs;
                targetPoufEffects = poufEffects;
                break;
            default:
                targetEffectGOs = poufs;
                targetPoufEffects = poufEffects;
                break;
            }

            for (int i = 0; i < targetEffectGOs.Length; i++) {
                if (targetEffectGOs[i] != null) {
                    if (!targetEffectGOs[i].activeInHierarchy) {
                        spawnPoufs[nr] = i;
                        nr++;
                        if (nr == spawnPoufs.Length)
                            break;
                    }
                }

            }

            Vector3 perpVec = Vector3.Cross(impactDirection, Vector3.right).normalized * impactPosition.magnitude;
            perpVec = Quaternion.AngleAxis(Random.Range(0, 360), impactDirection) * perpVec;

            for (int i = 0; i < spawnPoufs.Length; i++)
                if (spawnPoufs[i] != -1) {
                    float angle = 360f / spawnPoufs.Length * i;
                    Vector3 direction = Quaternion.AngleAxis(angle, impactDirection) * perpVec;
                    Vector3 ninty = Quaternion.AngleAxis(90, impactDirection) * direction;

                    targetEffectGOs[spawnPoufs[i]].SetActive(true);
                    targetPoufEffects[spawnPoufs[i]].PoufAround(impactPosition, direction, ninty, effectSize);

                }


        }
    }


}
