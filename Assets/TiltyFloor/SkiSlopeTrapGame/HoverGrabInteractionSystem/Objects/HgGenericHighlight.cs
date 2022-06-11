using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HgGenericHighlight : MonoBehaviour
{
    [SerializeField] private HoverGrabbableObject _hoverObj;

    public HoverGrabbableObject hoverObj
    {
        get
        {
            if (_hoverObj == null)
            {
                _hoverObj = GetComponent<HoverGrabbableObject>();
            }

            return _hoverObj;
        }
    }

    public UnityEvent HighlightEvent;
    public UnityEvent UnhighlightEvent;

    private void OnEnable()
    {
        hoverObj.highlightEvents.OnHighlighted.AddListener(OnHighlight);
        hoverObj.highlightEvents.OnUnhighlighted.AddListener(OnUnhighlight);
    }

    private void OnDisable()
    {
        hoverObj.highlightEvents.OnHighlighted.RemoveListener(OnHighlight);
        hoverObj.highlightEvents.OnUnhighlighted.RemoveListener(OnUnhighlight);
    }

    protected virtual void OnHighlight()
    {
        HighlightEvent.Invoke();
    }

    protected virtual void OnUnhighlight()
    {
        UnhighlightEvent.Invoke();
    }
}