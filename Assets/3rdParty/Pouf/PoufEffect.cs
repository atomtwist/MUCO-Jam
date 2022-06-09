using UnityEngine;
using System.Collections;

public class PoufEffect : MonoBehaviour {

    public AnimationCurve poufScale;
    public AnimationCurve poufMove;

    const float scale = 0.15f;

	public void PoufAround(Vector3 startPos, Vector3 direction, Vector3 ninty, float effectSize)
    {
        StartCoroutine(PoufMotion(startPos, direction, ninty, effectSize));
    }

    IEnumerator PoufMotion (Vector3 startPos, Vector3 direction, Vector3 ninty, float effectSize)
    {
        direction = Quaternion.AngleAxis(Random.Range(-15f, -12f), ninty.normalized) * direction; 
        
        Vector3 endPos = startPos + direction.normalized * 0.7f * Random.Range(0.95f, 1.05f) * effectSize;


		Vector3 startRot = transform.eulerAngles = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
         
        float lerp = 0;

        float speed = Random.Range(0.95f, 1.05f);


        while (lerp < 1) {
            lerp += Time.deltaTime * speed;

            float animLerp = poufMove.Evaluate(lerp);

            transform.position = Vector3.Lerp(startPos, endPos, animLerp);
            transform.eulerAngles = startRot;
            transform.RotateAround(ninty.normalized, animLerp * 3);

            transform.localScale = Vector3.one * poufScale.Evaluate(lerp) * scale * effectSize;
    
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
