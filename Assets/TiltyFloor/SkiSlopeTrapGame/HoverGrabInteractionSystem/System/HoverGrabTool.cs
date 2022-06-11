using ATVR;
using HoratiusSimpleGrab;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class HoverGrabTool : MonoBehaviour
{
    public Button button = Button.Grip;
    
    public float highlightGrabRadius = 0.01f;

    private Collider[] _resultsCache = new Collider[50];

    private HoverGrabbableObject _highlightedObj = null;
    private HoverGrabbableObject _grabbedObj = null;

    public bool isGrabbing => _grabbedObj != null;
    public HoverGrabbableObject grabbedObj => _grabbedObj;

    public bool isHighlighting => _highlightedObj != null;
    public HoverGrabbableObject highlightedObj => _highlightedObj;

    public Vector3 fakeVelocity { get; private set; }
    public Vector3 smoothFakeVelocity { get; private set; }
    private Vector3 prevPos;

    [Serializable]
    public class Events
    {
        public UnityEvent<HoverGrabbableObject> OnToolHighlightEvent;
        public UnityEvent<HoverGrabbableObject> OnToolUnhighlightEvent;
        public UnityEvent<HoverGrabbableObject> OnToolGrabEvent;
        public UnityEvent<HoverGrabbableObject> OnToolUngrabEvent;
    }
    public Events events;

    #region public Hashset Inhibitors
    private HashSet<Component> _inhibitors = new HashSet<Component>();
    public void AddInhibitor(Component who)
    {
        _inhibitors.Add(who);
    }
    public void RemoveInhibitor(Component who)
    {
        _inhibitors.Remove(who);
    }
    public bool inhibited => _inhibitors.Any();
    #endregion

    private void Reset()
    {
        this.button = Button.Grip;
    }

    protected void OnEnable()
    {
        prevPos = transform.position;
    }

    protected void Update()
    {
        //base.Update();

        Update_Highlights();

        Update_Grab();

        fakeVelocity = (transform.position - prevPos) / Mathf.Max(0.001f, Time.deltaTime);
        smoothFakeVelocity = Vector3.Lerp(smoothFakeVelocity, fakeVelocity, 0.2f);
        prevPos = transform.position;

    }

    public Hand hand;
    public Transform gizmo;
    
    private void Update_Highlights()
    {
        HoverGrabbableObject newHighlightedObj = null;

        // if we do not have a grabbed obj
        if (_grabbedObj == null)
        // find the new nearest highlighted object
        {
            // cannot highlight new shit when inhibited.
            if (!inhibited)
            {
                int count = Physics.OverlapSphereNonAlloc(gizmo.position, highlightGrabRadius, _resultsCache);

                // find the nearest one w/ closestTo function, to highlight it.
                var foundColliders = _resultsCache.Take(count)
                    // you can filter here by if the collider has a Stamp component, or a Ring Of Fire or some shit.
                    .Where(c =>
                    {
                        var bsmo = c.GetComponentInParent<HoverGrabbableObject>();
                        if (bsmo != null && !bsmo.interactable)
                        {
                            return false;
                        }

                        return bsmo != null;// && !bsmo.isGrabbed;
                    });
                var closestCollider = FindClosestGrabbable(gizmo.transform.position, foundColliders);
                if (closestCollider != null)
                {
                    // tell the previous highlighted object to not be highlighted anymore by this component,
                    newHighlightedObj = closestCollider.GetComponentInParent<HoverGrabbableObject>();
                }
            }
        }


        // old highlighted Exit because new, different highlight was found
        if (_highlightedObj != null && _highlightedObj != newHighlightedObj)
        {
            _highlightedObj.Unhighlight(this);
            //Debug.Log("Hand " + this.name + " unhighlighted " + _highlightedObj, _highlightedObj);
            events.OnToolUnhighlightEvent?.Invoke(_highlightedObj);

        }

        // CONSIDER. there is a highlight bug when an obj is released in the middle of another, and somehow one of the objects remains highlighted forever probably because it was highlighted while it was grabbed or something, or it was the last highlighted obj before the grab. but it seems that during the grab highlights are cleared. but maybe the obj itself is not...
        var prevHighlightedObj = _highlightedObj;

        // change highlight
        if (_highlightedObj != newHighlightedObj)
        {
            _highlightedObj = newHighlightedObj;
        }

        // can only do a new highlight when not inhibited.
        if (!inhibited)
        {
            if (_highlightedObj != null && prevHighlightedObj != _highlightedObj)
            {
                _highlightedObj.Highlight(this);
                //Debug.Log("Hand " + this.name + " HIGHlighted " + _highlightedObj, _highlightedObj);

                events.OnToolHighlightEvent?.Invoke(_highlightedObj);
            }
        }

    }

    private void Update_Grab()
    {
        // if button press, grab the shit.
        if (VRInput.Get(this.hand).GetPressDown(this.button))
        {
            // only grab when we have not grabbed something already in some crazy way...1?!?
            if (_grabbedObj == null)
            {
                // cannot grab when inhibited. can always release tho.
                if (!inhibited)
                {
                    if (_highlightedObj != null)
                    {
                        if (!_highlightedObj.isGrabbed)
                        {
                            // CONSIDER UnHighlight() here because we should do the unhighlight before the grab. ?!?!?!??!?! thanks best regards /Horatiu

                            Grab(_highlightedObj);
                        }
                    }
                }
            }
        }

        if (VRInput.Get(this.hand).GetPressUp(this.button))
        {
            if (_grabbedObj != null)
            {
                UngrabCurrentObj();
            }
        }
    }

    public void Grab(HoverGrabbableObject obj)
    {
        _grabbedObj = obj;
        _grabbedObj.Grab(transform);
        events.OnToolGrabEvent?.Invoke(obj);
    }

    public void UngrabCurrentObj()
    {
        events.OnToolUngrabEvent?.Invoke(_grabbedObj);
        _grabbedObj.Ungrab(transform);
        _grabbedObj = null;
    }


    /// <summary>
    /// Returns the nearest grabbable to a grabPoint position.
    /// </summary>
    /// <param name="grabPoint">position (on controller)</param>
    /// <param name="grabbables">list of objects to grab</param>
    /// <returns>nearest obj to grabPoint in grabbables list</returns>
    public Collider FindClosestGrabbable(Vector3 grabPoint, IEnumerable<Collider> grabbables)
    {
        var minSqrDist = float.MaxValue;
        Vector3 realClosestPoint = Vector3.zero;
        Collider closestGrabber = null;
        // check if someone was grabbed
        foreach (var c in grabbables)
        {
            var closestPoint = c.ClosestPoint(grabPoint);
            var newSqrDist = (closestPoint - grabPoint).sqrMagnitude;
            if (newSqrDist < minSqrDist)
            {
                minSqrDist = newSqrDist;
                realClosestPoint = closestPoint;
                closestGrabber = c;
            }
        }
        //Debug.DrawLine(grabPoint, realClosestPoint, Color.red, 0.1f);

        return closestGrabber;
    }

}
