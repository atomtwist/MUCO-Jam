namespace HoratiusSimpleGrab
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public static class JointGrabUtils
    {
        private static GameObject _dummy;
        public static GameObject dummy
        {
            get
            {
                if (_dummy == null)
                {
                    _dummy = new GameObject("[dummy]");
                }
                return _dummy;
            }
        }

        /// <summary>
        /// links grabTransforms with joints
        /// </summary>
        private static Dictionary<Transform, ConfigurableJoint> handJointDict = new Dictionary<Transform, ConfigurableJoint>();

        public static void JointGrab(Transform grabTransform, Rigidbody grabbedObj)
        {
            // controller has the joint, controller can be connected to only one grabbed object. but grabbed obj can be grabbed by any number of grabbers
            var joint = GetOrCreateJoint(grabTransform);

            // perform other grab mechanics on this transform before grabbing?
            // such as reposition based on grabPoint and resetting velocities?
            grabbedObj.velocity = grabbedObj.angularVelocity = Vector3.zero;

            joint.autoConfigureConnectedAnchor = true;

            joint.connectedBody = grabbedObj;
        }

        /// <summary>
        /// Gets or creates a "hand" joint.
        /// </summary>
        /// <returns></returns>
        public static ConfigurableJoint GetOrCreateJoint(Transform grabTransform)
        {
            ConfigurableJoint joint = handJointDict.ContainsKey(grabTransform) ? handJointDict[grabTransform] : null;

            if (joint == null)
            {
                var go = new GameObject("[Gen] Grab Joint");
                go.transform.SetParent(grabTransform.transform);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;

                var rb = go.AddComponent<Rigidbody>();
                rb.constraints = RigidbodyConstraints.FreezeAll;
                rb.useGravity = false;

                joint = go.AddComponent<ConfigurableJoint>();
                SetDefaultHandJointSettings(joint);

                handJointDict[grabTransform] = joint;
            }

            return joint;
        }

        /// <summary>
        /// Sets a range of 'magic' values on the given <see cref="ConfigurableJoint"/> in order to setup the joint as a 'hand joint' for VR use.
        /// </summary>
        /// <param name="joint">The joint.</param>
        public static void SetDefaultHandJointSettings(ConfigurableJoint joint)
        {
            joint.xMotion = joint.yMotion = joint.zMotion = ConfigurableJointMotion.Locked;
            joint.angularXMotion = joint.angularYMotion = joint.angularZMotion = ConfigurableJointMotion.Free;

            var positionDrive = new JointDrive()
            {
                positionSpring = 200000,
                positionDamper = 1,
            };
            joint.xDrive = joint.yDrive = joint.zDrive = positionDrive;

            joint.rotationDriveMode = RotationDriveMode.Slerp;
            joint.slerpDrive = new JointDrive()
            {
                positionSpring = 200000,
                positionDamper = 1,
                maximumForce = float.MaxValue
            };
        }

        public static void JointUngrab(Transform grabTransform, Rigidbody grabbedObj)
        {
            var joint = GetOrCreateJoint(grabTransform);
            joint.connectedBody = null;
        }
    }
}
