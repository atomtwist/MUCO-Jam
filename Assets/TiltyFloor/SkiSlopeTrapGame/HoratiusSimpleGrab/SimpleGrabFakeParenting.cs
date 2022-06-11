namespace HoratiusSimpleGrab
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    public class SimpleGrabFakeParenting : MonoBehaviour
    {
        public Transform followMe;
        public Vector3 offsetPos;
        public Quaternion offsetRot;

        public bool doPosition = true;
        public bool doRotation = true;

        public void SetFollower(Transform follower)
        {
            followMe = follower;
            offsetPos = transform.InverseTransformPoint(follower.position);
            offsetRot = Quaternion.Inverse(transform.rotation) * follower.rotation;
        }

        private void Update()
        {
            DoIt();
        }

        private void DoIt()
        {
            if (followMe == null)
                return;

            if (doPosition)
                followMe.transform.position = transform.TransformPoint(offsetPos);
            if (doRotation)
                followMe.transform.rotation = transform.rotation * offsetRot;
        }

    }
}