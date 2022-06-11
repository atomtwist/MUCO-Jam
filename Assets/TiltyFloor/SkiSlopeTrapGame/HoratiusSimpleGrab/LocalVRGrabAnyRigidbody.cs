namespace HoratiusSimpleGrab
{

    using UnityEngine.Events;
    using ATVR;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// Simple grab script to put on an object that can be grabbed. Checks using Physics.OverlapSphere 
    /// </summary>
    public class LocalVRGrabAnyRigidbody : MonoBehaviour
    {
        private static Dictionary<Transform, Rigidbody> handGrabbingRigidbody = new Dictionary<Transform, Rigidbody>();

        [SerializeField]
        private List<Transform> _grabPoints = new List<Transform>();

        public bool canGrabUsingButton = true;
        public ATVR.Button vrGrabButton = Button.Trigger;

        public float velMultiplier = 1.5f;
        public float angVelMultiplier = 0.7f;

        public float grabRadius = 0.05f;
        private static Collider[] resultsCache = new Collider[20];

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

        public List<Collider> ignoreColliders = new List<Collider>();

        public KinematicType setKinematicOnGrab = LocalVRGrabAnyRigidbody.KinematicType.DoNotChange;
        public KinematicType setKinematicOnRelease = LocalVRGrabAnyRigidbody.KinematicType.DoNotChange;

        public enum KinematicType
        {
            DoNotChange,
            On,
            Off
        }

        public delegate void Grab();
        public event Grab GrabStarted, GrabEnded;
        public UnityEvent OnGrabStart, OnGrabEnd;

        public bool isGrabbed
        {
            get
            {
                if (rb == null)
                    return false;
                return handGrabbingRigidbody.Values.Any(r => r == rb);
            }
        }

        /// <summary>
        /// this function moves the object so that the grabHand pos/rot matches the nearest grab point - such as handle of racket or holes of bowling ball
        /// </summary>
        /// <param name="grabHand"></param>
        public void MoveToNearestGrabPoint(Transform grabHand)
        {
            // these should somehow depend on scale...?
            var positionWeight = 1f;
            var rotationWeight = 1f;

            if (_grabPoints.Count > 0)
            {
                var handPos = grabHand.position;
                var handRot = grabHand.rotation.eulerAngles;
                var shortest = float.MaxValue;
                Transform nearestGrabPoint = null;
                for (int i = 0; i < _grabPoints.Count; i++)
                {
                    var gp = _grabPoints[i];
                    var distanceSqr = (handPos - gp.transform.position).sqrMagnitude * positionWeight;
                    var deltaRotSqr = (handRot - gp.transform.rotation.eulerAngles).sqrMagnitude * rotationWeight;

                    var combinedDelta = distanceSqr + deltaRotSqr;
                    if (combinedDelta < shortest)
                    {
                        nearestGrabPoint = gp;
                        shortest = combinedDelta;
                    }
                }

                // now we know the nearest grab point. move and rotate the object so that grab point matches the hand pos/rot
                RotateTheRightWaySelfieTennisMethod(grabHand, transform, nearestGrabPoint);
            }
        }

        private void RotateTheRightWaySelfieTennisMethod(Transform hand, Transform equippable, Transform grabPointObj)
        {
            var objectGrabPoint = grabPointObj;
            var grabPoint = hand.transform;

            Vector3 selOffset;
            selOffset = grabPoint.position - objectGrabPoint.position;
            equippable.transform.position += selOffset;

            Quaternion selOffsetRot;
            selOffsetRot = Quaternion.Inverse(objectGrabPoint.rotation) * grabPoint.rotation;

            //var newObj = new GameObject();
            var newObj = JointGrabUtils.dummy;
            newObj.transform.position = grabPoint.position;
            newObj.transform.rotation = objectGrabPoint.rotation;
            var oldSelectedObjParent = equippable.transform.parent;
            equippable.transform.SetParent(newObj.transform);
            newObj.transform.rotation *= selOffsetRot;
            equippable.transform.SetParent(oldSelectedObjParent);
            //Destroy(newObj);
            //newObj.SetActive(false);
        }

        private void Update()
        {
            var hand = VRInput.RightHand;
            Update_HandGrab(hand);

            hand = VRInput.LeftHand;
            Update_HandGrab(hand);

        }

        private void Update_HandGrab(VRController hand)
        {
            if (hand == null)
                return;

            if (!IsGrabbingSomething(hand))
            {
                if (canGrabUsingButton && hand.GetPressDown(vrGrabButton))
                {
                    // find obj
                    var resultCount = Physics.OverlapSphereNonAlloc(hand.transform.position, grabRadius, resultsCache);
                    for (int i = 0; i < resultCount; i++)
                    {
                        var c = resultsCache[i];
                        if (ignoreColliders.Contains(c))
                            continue;

                        var r = c.attachedRigidbody;
                        if (r != null && r == this.rb)
                        {
                            PerformGrab(hand);

                            return;
                        }
                    }

                }
            }
            else
            {
                if (hand.GetPressUp(vrGrabButton))
                {
                    PerformRelease(hand);
                }
            }

        }

        public void PerformGrab(VRController hand)
        {
            if (!IsGrabbingSomething(hand))
            {
                MoveToNearestGrabPoint(hand.transform);

                handGrabbingRigidbody.Add(hand.transform, rb);

                JointGrabUtils.JointGrab(hand.transform, rb);

                switch (setKinematicOnGrab)
                {
                    case KinematicType.DoNotChange:
                        break;
                    case KinematicType.On:
                        rb.isKinematic = true;
                        break;
                    case KinematicType.Off:
                        rb.isKinematic = false;
                        break;
                    default:
                        break;
                }

                //event grab
                OnGrabStart.Invoke();
                if (GrabStarted != null)
                    GrabStarted.Invoke();
            }
        }

        public void PerformRelease(VRController hand)
        {
            if (IsGrabbingSomething(hand))
            {
                if (handGrabbingRigidbody.ContainsKey(hand.transform))
                {
                    var r = handGrabbingRigidbody[hand.transform];
                    if (r == rb)
                    {
                        handGrabbingRigidbody.Remove(hand.transform);
                        JointGrabUtils.JointUngrab(hand.transform, r);

                        switch (setKinematicOnRelease)
                        {
                            case KinematicType.DoNotChange:
                                break;
                            case KinematicType.On:
                                r.isKinematic = true;
                                break;
                            case KinematicType.Off:
                                r.isKinematic = false;
                                break;
                            default:
                                break;
                        }

                        r.velocity = hand.GetVelocity() * velMultiplier;
                        r.angularVelocity = hand.GetAngularVelocity() * angVelMultiplier;

                        //event ungrab
                        OnGrabEnd.Invoke();
                        if (GrabEnded != null)
                            GrabEnded.Invoke();
                    }
                }
            }
        }

        public static bool IsGrabbingSomething(VRController hand)
        {
            if (hand == null)
                return false;

            // if contains key, it is probably grabbing...??
            if (handGrabbingRigidbody.ContainsKey(hand.transform))
            {
                // if the thing is not null, def grabbing. else we should remove it, and it's not grabbing actually.
                if (handGrabbingRigidbody[hand.transform] == null)
                {
                    handGrabbingRigidbody.Remove(hand.transform);
                    return false;
                }
                else
                {
                    // not null. we are def grabbing something...
                    return true;
                }
            }

            return false;
        }

    }
}