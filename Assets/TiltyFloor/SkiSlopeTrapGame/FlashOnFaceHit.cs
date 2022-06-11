using System.Collections;
using System.Collections.Generic;
using ToyBoxHHH;
using UnityEngine;

public class FlashOnFaceHit : MonoBehaviour
{
    public CanvasGroup cg;
    public float flashDur = 0.1f;

    public void FlashIt()
    {
        cg.alpha = 1f;
        StartCoroutine(pTween.Wait(flashDur, () => { cg.alpha = 0f; }));
    }
}