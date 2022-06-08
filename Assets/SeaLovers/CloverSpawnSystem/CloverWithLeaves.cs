using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloverWithLeaves : MonoBehaviour
{
    public bool found { get; private set; }
    public Renderer r;
    public Material unfoundMat;
    public Material foundMat;
    public Material alreadyFoundMat;
    public AudioSource foundSound;

    public float alreadyFoundHighlight = 3f;
    public float firstFoundHighlight = 5f;
    
    // maybe hands highlight it..?
    public void Highlight()
    {
        r.sharedMaterial = foundMat;
        StopAllCoroutines();
        StartCoroutine(pTween.Wait(firstFoundHighlight, () =>
        {
            r.sharedMaterial = unfoundMat;
        }));
    }

    public void Unhighlight()
    {
        r.sharedMaterial = unfoundMat;
    }

    public void HighlightAlreadyFound()
    {
        if (r.sharedMaterial == foundMat)
            return;
        
        r.sharedMaterial = alreadyFoundMat;
        StopAllCoroutines();
        StartCoroutine(pTween.Wait(alreadyFoundHighlight, () =>
        {
            r.sharedMaterial = unfoundMat;
        }));
    }

    public void PickClover()
    {
        // destroy?
        Highlight();

        if (!found)
        {
            foundSound.Play();
        }
        
        found = true;

        CloverGameplay.instance.CloverScoreUpdate();

    }
}