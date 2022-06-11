using ATVR;
using HoratiusSimpleGrab;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class HgVibrateOnHighlightTool : MonoBehaviour
{
    [SerializeField]
    private HoverGrabTool _hgTool;
    public HoverGrabTool hgTool
    {
        get
        {
            if (_hgTool == null)
            {
                _hgTool = GetComponent<HoverGrabTool>();
            }
            return _hgTool;
        }
    }

    public float vibrateOnHighlight = 0.5f;
    public float vibrateDuration = 0.1f;

    private void OnValidate()
    {
        if (hgTool != null)
        {
        }
    }

    private void OnEnable()
    {
        hgTool.events.OnToolHighlightEvent.AddListener(OnToolHighlight);
    }

    private void OnDisable()
    {
        hgTool.events.OnToolHighlightEvent.RemoveListener(OnToolHighlight);
    }

    private void OnToolHighlight(HoverGrabbableObject obj)
    {
        VRInput.Get(obj.GetHighlightedHand()).VibrateLong(vibrateDuration, vibrateOnHighlight);
    }


}
