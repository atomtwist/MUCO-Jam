using ATVR;
using HoratiusSimpleGrab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HgObjectMovable : MonoBehaviour
{
    public enum GrabMethod
    {
        RigidbodyJoint = 0,
        TransformParent = 1,
        RigidbodyMove = 2,
    }

    public GrabMethod grabMethod = GrabMethod.TransformParent;

    [Header("Settings for RigidbodyJoint method")]
    public bool keepRbKinematicStatus = true;
    private bool prevIsKinematic = false;
    public bool PrevIsKinematic => prevIsKinematic;

    [Header("Settings for TransformParent method")]
    public bool doPos = true;
    public bool doRot = true;

    //[Header("Settings for RigidbodyMove method")]

    [Header("auto-ref")]
    [SerializeField]
    private Rigidbody _rb;
    public Rigidbody rb
    {
        get
        {
            if (_rb == null)
            {
                _rb = GetComponent<Rigidbody>();
            }
            return _rb;
        }
    }

    [SerializeField]
    private HoverGrabbableObject _hg;
    public HoverGrabbableObject hg
    {
        get
        {
            if (_hg == null)
            {
                _hg = GetComponent<HoverGrabbableObject>();
            }
            return _hg;
        }
    }


    private void OnEnable()
    {
        hg.OnGrabHappened += Hg_OnGrabHappened;
        hg.OnUngrabHappened += Hg_OnUngrabHappened;
    }
    private void OnDisable()
    {
        hg.OnGrabHappened -= Hg_OnGrabHappened;
        hg.OnUngrabHappened -= Hg_OnUngrabHappened;
    }

    public void SetPrevKineStatus(bool on)
    {
        prevIsKinematic = on;
    }

    private void Hg_OnGrabHappened(Transform grabHand)
    {
        if (grabMethod == GrabMethod.RigidbodyJoint)
        {
            if (keepRbKinematicStatus)
                prevIsKinematic = rb.isKinematic;

            rb.isKinematic = false;

            // grab with a joint on controller
            JointGrabUtils.JointGrab(grabHand, rb);
        }
        else if (grabMethod == GrabMethod.TransformParent)
        {
            TransformGrabUtils.TransformScriptGrab(grabHand, transform, doPos, doRot);
        }
        else if (grabMethod == GrabMethod.RigidbodyMove)
        {
            if (keepRbKinematicStatus)
                prevIsKinematic = rb.isKinematic;

            TransformGrabUtils.RigidbodyGrab(grabHand.gameObject, rb);

            rb.useGravity = false;
            rb.isKinematic = true;
        }
    }

    public float throwVelocity => HoverGrabToolFinder.instance.throwVelocity;
    public float throwAngularVelocity => HoverGrabToolFinder.instance.throwAngularVelocity;

    private void Hg_OnUngrabHappened(Transform grabHand)
    {
        if (grabMethod == GrabMethod.RigidbodyJoint)
        {
            // grab with a joint on controller
            JointGrabUtils.JointUngrab(grabHand, rb);

            if (keepRbKinematicStatus)
                rb.isKinematic = prevIsKinematic;

            ApplyThrowForces(grabHand);

        }
        else if (grabMethod == GrabMethod.TransformParent)
        {
            TransformGrabUtils.TransformScriptUngrab(grabHand, transform);
        }
        else if (grabMethod == GrabMethod.RigidbodyMove)
        {
            TransformGrabUtils.RigidbodyUngrab(grabHand.gameObject, rb);

            if (keepRbKinematicStatus)
                rb.isKinematic = prevIsKinematic;

            rb.useGravity = true;

            ApplyThrowForces(grabHand);
        }

    }

    private void ApplyThrowForces(Transform grabHand)
    {
        if (!rb.isKinematic)
        {
            // add controller velocity for throw
            var hgt = grabHand.GetComponent<HoverGrabTool>();
            rb.velocity = hgt.smoothFakeVelocity * throwVelocity;
            rb.angularVelocity = VRInput.Get(hgt.hand).GetAngularVelocity() * throwAngularVelocity;
        }
    }
}
