using Normal.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class HgRequestOwnership : MonoBehaviour
{
    [SerializeField]
    private HoverGrabbableObject _grabbableObj;
    public HoverGrabbableObject grabbableObj
    {
        get
        {
            if (_grabbableObj == null)
            {
                _grabbableObj = GetComponent<HoverGrabbableObject>();
            }
            return _grabbableObj;
        }
    }

    [Header("Request ownership on these transforms when grabbing.")]
    public RealtimeTransform[] realtimeTransforms;

    private void OnEnable()
    {
        grabbableObj.OnGrabHappened += GrabbableObject_OnGrabHappened;
        grabbableObj.OnUngrabHappened += GrabbableObject_OnUngrabHappened;
    }

    private void OnDisable()
    {
        grabbableObj.OnGrabHappened -= GrabbableObject_OnGrabHappened;
        grabbableObj.OnUngrabHappened -= GrabbableObject_OnUngrabHappened;
    }

    private void GrabbableObject_OnGrabHappened(Transform obj)
    {
        StopAllCoroutines();
        RequestOwnershipOnAll();
    }

    private void GrabbableObject_OnUngrabHappened(Transform obj)
    {
        //StartCoroutine(pTween.Wait(1f, () =>
        //{
        //    ClearOwnershipOnAll();
        //}));
    }


    private void RequestOwnershipOnAll()
    {
        foreach (var rt in realtimeTransforms)
        {
            rt.RequestOwnership();
        }
    }

}