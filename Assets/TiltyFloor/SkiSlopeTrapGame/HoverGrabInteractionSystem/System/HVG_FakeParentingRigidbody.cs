using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// Component stays on the parent! Child is referenced and moved.
public class Hg_FakeParentingRigidbody : MonoBehaviour
{
    [Header("Fake Child")]
    [SerializeField]
    private Rigidbody _fakeChild;
    public Rigidbody fakeChild
    {
        get
        {
            return _fakeChild;
        }
    }

    [Header("FakeParent params")]
    [SerializeField]
    private bool _pos;

    [SerializeField]
    private bool _rot;

    [SerializeField]
    private bool _update = true, _fixedUpdate, _lateUpdate;

    private Vector3 _offsetPos;
    private Quaternion _offsetRot;

    public void SetFakeParenting(Rigidbody fakeChild, bool pos = true, bool rot = true, bool update = true, bool fixedUpdate = false, bool lateUpdate = false)
    {
        _fakeChild = fakeChild;
        _pos = pos;
        _rot = rot;
        _update = update;
        _fixedUpdate = fixedUpdate;
        _lateUpdate = lateUpdate;

        if (fakeChild != null)
        {
            // offsetRos is the InverseTransformPoint, so when we set it we must use TransformPoint. Effectively, offsetPos would be the localPosition if the fakeChild was a real child of transform. 
            _offsetPos = transform.InverseTransformPoint(fakeChild.position);
            // offsetRot is the local rotation applied compared to transform.rotation. It would be the localRotation if the offsetPos would be a real child.
            _offsetRot = Quaternion.Inverse(transform.rotation) * fakeChild.rotation;
        }
    }

    private void FixedUpdate()
    {
        if (_fixedUpdate)
        {
            FakeParentApply();
        }
    }

    private void Update()
    {
        if (_update)
        {
            FakeParentApply();
        }
    }

    private void LateUpdate()
    {
        if (_lateUpdate)
        {
            FakeParentApply();
        }
    }

    private void FakeParentApply()
    {
        if (_fakeChild != null)
        {
            if (_pos)
                _fakeChild.MovePosition(transform.TransformPoint(_offsetPos));
            if (_rot)
                _fakeChild.MoveRotation(transform.rotation.normalized * _offsetRot.normalized);
        }
    }
}
