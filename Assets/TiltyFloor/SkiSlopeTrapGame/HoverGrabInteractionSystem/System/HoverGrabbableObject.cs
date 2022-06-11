using ATVR;
using HoratiusSimpleGrab;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ToyBoxHHH;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class HoverGrabbableObject : MonoBehaviour
{
    #region public HIGHLIGHT stuff
    private HashSet<Component> highlightersHashset = new HashSet<Component>();

    public bool isHighlighted
    {
        get
        {
            return highlightersHashset.Count > 0;
        }
    }
    private bool prevIsHighlighted = false;
    public Hand GetHighlightedHand()
    {
        var hand = highlightersHashset.FirstOrDefault();
        if (hand != null)
        {
            var hgt = (hand as HoverGrabTool);
            if (hgt != null)
            {
                return hgt.hand;
            }
        }
        return Hand.Primary;
    }

    [Serializable]
    public class HighlightEffects
    {
        public UnityEvent OnHighlighted;
        public UnityEvent OnUnhighlighted;
    }
    public HighlightEffects highlightEvents = new HoverGrabbableObject.HighlightEffects();

    public void Highlight(Component whoIsAsking)
    {
        highlightersHashset.Add(whoIsAsking);
        // do the nice highlight effect
        // or do it in update
    }

    public void Unhighlight(Component whoIsAsking)
    {
        highlightersHashset.Remove(whoIsAsking);
        if (!highlightersHashset.Any())
        {
            // unhighlight effect
        }
    }

    [DebugButton]
    public void RevealHighlighters()
    {
        Debug.Log("Highlights on this object: " + highlightersHashset.Count, this);
        int i = 0;
        foreach (var highligh in highlightersHashset)
        {
            Debug.Log(i + ". " + highligh.name + " which is a " + highligh, highligh);
        }
    }

    private void Update_Highlighting()
    {
        if (isHighlighted != prevIsHighlighted)
        {
            prevIsHighlighted = isHighlighted;
            if (isHighlighted)
                highlightEvents.OnHighlighted?.Invoke();
            else
                highlightEvents.OnUnhighlighted?.Invoke();

        }
    }

    #endregion

    // interactable when there are no inhibitors.
    public bool interactable => !interactableInhibitors.Any();

    #region inhibitors
    private HashSet<Component> interactableInhibitors = new HashSet<Component>();
    public void AddInhibitor(Component who)
    {
        interactableInhibitors.Add(who);
    }
    public void RemoveInhibitor(Component who)
    {
        interactableInhibitors.Remove(who);
    }
    #endregion

    //private void Start()
    //{
    //    NetworkManager.Instance.realtime.didConnectToRoom += Realtime_didConnectToRoom;
    //}
    //private void OnDestroy()
    //{
    //    NetworkManager.Instance.realtime.didConnectToRoom -= Realtime_didConnectToRoom;
    //}

    //private void Realtime_didConnectToRoom(Normal.Realtime.Realtime realtime)
    //{
    //    // just a quick unhighlight on connect to room -> no more shitty bugs hopefully....?!?!?!
    //    highlightEvents.OnUnhighlighted?.Invoke();
    //}

    [Header("auto ref")]
    [SerializeField]
    private HgRequestOwnership _hgRequestOwnership;
    public HgRequestOwnership hgRequestOwnership
    {
        get
        {
            if (_hgRequestOwnership == null)
            {
                _hgRequestOwnership = GetComponent<HgRequestOwnership>();
            }
            return _hgRequestOwnership;
        }
    }


    private void Update()
    {
        Update_Highlighting();

    }

    public HoverGrabTool GetGrabTool()
    {
        var handT = whoGrabsMe.FirstOrDefault();
        if (handT != null)
        {
            var hand = handT.GetComponent<HoverGrabTool>();
            if (hand != null)
            {
                return hand;
            }
        }
        return null;
    }

    public Hand GetGrabHand()
    {
        var handT = whoGrabsMe.FirstOrDefault();
        if (handT != null)
        {
            var hand = handT.GetComponent<HoverGrabTool>();
            if (hand != null)
            {
                return hand.hand;
            }
        }
        return Hand.Primary;
    }

    public HashSet<Transform> whoGrabsMe = new HashSet<Transform>();
    public bool isGrabbed => whoGrabsMe.Any();


    [Space]
    public UnityEvent OnGrabUnityEvent;
    public UnityEvent OnUngrabUnityEvent;
    public event GrabDelegate OnGrabHappened, OnUngrabHappened;
    public delegate void GrabDelegate(Transform grabHand);

    public void Grab(Transform grabHand)
    {
        StopAllCoroutines();

        whoGrabsMe.Add(grabHand);

        OnGrabHappened?.Invoke(grabHand);
        OnGrabUnityEvent?.Invoke();
    }

    public void Ungrab(Transform grabHand)
    {
        StopAllCoroutines();

        whoGrabsMe.Remove(grabHand);

        OnUngrabUnityEvent?.Invoke();
        OnUngrabHappened?.Invoke(grabHand);

    }

}
