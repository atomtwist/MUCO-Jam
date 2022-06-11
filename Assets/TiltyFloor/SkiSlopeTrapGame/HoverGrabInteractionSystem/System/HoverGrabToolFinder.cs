using ATVR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;


public class HoverGrabToolFinder : MonoBehaviour
{
    private static HoverGrabToolFinder _instance;
    public static HoverGrabToolFinder instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<HoverGrabToolFinder>();
            }
            return _instance;
        }
    }

    public HoverGrabTool grabToolLeft;
    public HoverGrabTool grabToolRight;

    public float throwVelocity = 1.2f;
    public float throwAngularVelocity = 0.9f;

    private void OnValidate()
    {
        if (grabToolLeft == null)
        {
            grabToolLeft = GetComponentsInChildren<HoverGrabTool>(true).Where(gt => gt.hand == Hand.Secondary).FirstOrDefault();
        }
        if (grabToolRight == null)
        {
            grabToolRight = GetComponentsInChildren<HoverGrabTool>(true).Where(gt => gt.hand == Hand.Primary).FirstOrDefault();
        }
    }

}
