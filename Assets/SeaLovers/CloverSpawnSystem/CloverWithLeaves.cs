using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloverWithLeaves : MonoBehaviour
{
    public bool found { get; private set; }
    public Renderer r;
    public Material unfoundMat;
    public Material foundMat;
    public AudioSource foundSound;

    // maybe hands highlight it..?
    public void Highlight()
    {
        r.sharedMaterial = foundMat;
    }

    public void Unhighlight()
    {
        r.sharedMaterial = unfoundMat;
    }

    public void PickClover()
    {
        if (!found)
        {
            found = true;

            CloverGameplay.instance.CloverScoreUp();

            // destroy?
            Highlight();

            foundSound.Play();

        }
    }
}