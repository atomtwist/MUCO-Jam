using ATVR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollowController : MonoBehaviour {

    public Hand Hand;

    private Vector3 currentSmoothed;
    private Quaternion currentSmoothedRot;

    private Transform lastHandTransform = null;

    public void Awake()
    {
        if (Hand == Hand.Primary) VRInput.PrimarySmoothed = transform;
        else if (Hand == Hand.Secondary) VRInput.SecondarySmoothed = transform;
    }

    public void LateUpdate()
    {
        var handTransform = VRInput.Get(Hand).transform;
        if(handTransform != lastHandTransform)
        {
            lastHandTransform = handTransform;
            transform.position = handTransform.position;
            transform.rotation = handTransform.rotation;
            currentSmoothed = transform.position;
            currentSmoothedRot = transform.rotation;
        }

        var pos = handTransform.position;
        {
            var Alpha = Vector3.Distance(currentSmoothed, pos) / .08f;
            Alpha = Mathf.Clamp(Alpha, 0, 1);
            currentSmoothed = Vector3.Lerp(currentSmoothed, pos, Alpha);
        }

        var rot = handTransform.rotation;
        {
            var Alpha = Quaternion.Angle(currentSmoothedRot, rot) * .8f;
            Alpha = Mathf.Clamp(Alpha, 0, 1);
            currentSmoothedRot = Quaternion.Slerp(currentSmoothedRot, rot, Alpha);
        }


        transform.position = currentSmoothed;
        transform.rotation = currentSmoothedRot;
    }

}
