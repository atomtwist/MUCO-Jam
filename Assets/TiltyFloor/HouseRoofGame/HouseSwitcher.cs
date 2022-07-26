using System;
using System.Collections;
using System.Collections.Generic;
using ATVR;
using Normal.Realtime;
using ToyBoxHHH;
using UnityEngine;

public class HouseSwitcher : MonoBehaviour
{
    public List<Transform> places = new List<Transform>();
    private int placeIndex;
    
    private void Update()
    {
        if (VRInput.Get(Hand.Primary).GetPressDown(Button.ButtonOne))
        {
            GoToNextPlace(); 
        }
    }

    [DebugButton]
    private void GoToNextPlace()
    {
        placeIndex++;
        placeIndex = placeIndex % places.Count;
        transform.GetComponent<RealtimeTransform>().RequestOwnership();
        transform.position = places[placeIndex].position;
        transform.rotation = places[placeIndex].rotation;
        transform.localScale = places[placeIndex].localScale;
    }
}
