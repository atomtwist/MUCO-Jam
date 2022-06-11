namespace HoratiusSimpleGrab
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    public static class TransformGrabUtils
    {
        // =======================================================================  transform script grab 
        public static void TransformScriptGrab(Transform grabHand, Transform grabbedObj, bool doPos = true, bool doRot = true)
        {
            var fp = grabHand.GetComponent<SimpleGrabFakeParenting>();
            if (fp == null)
                fp = grabHand.gameObject.AddComponent<SimpleGrabFakeParenting>();

            fp.SetFollower(grabbedObj);
            fp.doPosition = doPos;
            fp.doRotation = doRot;
        }

        public static void TransformScriptUngrab(Transform grabHand, Transform grabbedObj)
        {
            var fp = grabHand.GetComponent<SimpleGrabFakeParenting>();
            if (fp != null)
            {
                fp.followMe = null;
            }
        }



        // ======================================================================== other shit???

        // each hand can only grab one thing. but one thing can be grabbed by many hands...?!?!?
        private static Dictionary<Transform, Transform> handsGrabbingOtherThings = new Dictionary<Transform, Transform>();

        // old parents of grabbed objs
        private static Dictionary<Transform, Transform> grabbedObjectsOldParentsDict = new Dictionary<Transform, Transform>();

        public static void TransformParentGrab(Transform grabHand, Transform grabbedObj)
        {
            grabbedObjectsOldParentsDict.Add(grabbedObj, grabbedObj.parent);
            grabbedObj.SetParent(grabHand.transform);
            handsGrabbingOtherThings.Add(grabHand, grabbedObj);
        }

        public static void TransformParentUngrab(Transform grabHand, Transform grabbedObj)
        {
            handsGrabbingOtherThings.Remove(grabHand);

            grabbedObjectsOldParentsDict.TryGetValue(grabbedObj, out var oldParent);
            grabbedObjectsOldParentsDict.Remove(grabbedObj);
            if (oldParent != null)
            {
                grabbedObj.SetParent(oldParent);
            }
            else
            {
                grabbedObj.SetParent(null);
            }
        }



        // ======================================================================= rigidbody move script grab
        private static Dictionary<GameObject, Hg_FakeParentingRigidbody> controllerGrabbedTransformDictRb = new Dictionary<GameObject, Hg_FakeParentingRigidbody>();

        public static void RigidbodyGrab(GameObject grabHand, Rigidbody grabbedObj)
        {
            // get/add fake parenting component to controller
            Hg_FakeParentingRigidbody fp;
            if (!controllerGrabbedTransformDictRb.TryGetValue(grabHand, out fp))
            {
                fp = grabHand.gameObject.AddComponent<Hg_FakeParentingRigidbody>();
                controllerGrabbedTransformDictRb[grabHand] = fp;
            }

            // ungrab grabbedObj from all other controllers
            foreach (var c in controllerGrabbedTransformDictRb)
            {
                if (c.Value.fakeChild == grabbedObj)
                {
                    c.Value.SetFakeParenting(null);
                }
            }
            fp.SetFakeParenting(grabbedObj, true, true, update: true, fixedUpdate: false, lateUpdate: false);

        }

        public static void RigidbodyUngrab(GameObject grabGO, Rigidbody grabbedObj)
        {
            Hg_FakeParentingRigidbody fp;
            if (controllerGrabbedTransformDictRb.TryGetValue(grabGO, out fp))
            {
                fp.SetFakeParenting(null);
            }
        }

    }
}
