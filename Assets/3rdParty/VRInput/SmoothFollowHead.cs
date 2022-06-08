using System.Collections;
using System.Collections.Generic;
using ATVR;
using UnityEngine;

public class SmoothFollowHead : MonoBehaviour
{
    private Vector3 currentSmoothed;
    private Quaternion currentSmoothedRot;

    private Transform lastHeadTransform = null;

    public void Awake()
    {
        VRInput.HeadSmoothed = transform;
    }

    public void LateUpdate()
    {
        var headTransform = VRInput.Head;
        if (headTransform != lastHeadTransform)
        {
            lastHeadTransform = headTransform;
            transform.position = headTransform.position;
            transform.rotation = headTransform.rotation;
            currentSmoothed = transform.position;
            currentSmoothedRot = transform.rotation;
        }

        var pos = headTransform.position;
        {
            var Alpha = Vector3.Distance(currentSmoothed, pos) / .16f;
            Alpha = Mathf.Clamp(Alpha, 0, 1);
            currentSmoothed = Vector3.Lerp(currentSmoothed, pos, Alpha);
        }

        var rot = headTransform.rotation;
        {
            var Alpha = Quaternion.Angle(currentSmoothedRot, rot) * 1.6f;
            Alpha = Mathf.Clamp(Alpha, 0, 1);
            currentSmoothedRot = Quaternion.Slerp(currentSmoothedRot, rot, Alpha);
        }


        transform.position = currentSmoothed;
        transform.rotation = currentSmoothedRot;
    }
}