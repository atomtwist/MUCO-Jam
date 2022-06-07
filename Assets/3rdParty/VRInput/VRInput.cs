using UnityEngine;
using System.Collections;

namespace ATVR
{


    public enum Button
    {
        System, 
        ButtonTwo, 
        Grip, 
        Joystick, 
        Trigger, 
        ButtonOne, 
    }


    public enum Hand
    {
        Primary,
        Secondary,
    }

    public enum PhysicalHand
    {
        Left,
        Right
    }

    public enum Handedness
    {
        RightHanded,
        LeftHanded
    }

    public enum ButtonPressType
    {
        PressDown,
        PressUp,
        Pressed
    }

    public static class VRInput
    {
        static VRInput()
        {
            if(Application.isPlaying  && PlayerPrefs.HasKey("ANIMVR_HANDEDNESS"))
            {
                Handedness = PlayerPrefs.GetString("ANIMVR_HANDEDNESS") == "Left" ? Handedness.LeftHanded : Handedness.RightHanded;
            }
            else
            {
                Handedness = Handedness.RightHanded;
            }
        }

        public static bool isOculus {
            get {
                //this needs to be properlz implemented
                return true;
            }
        }

        public static Handedness Handedness { get; private set; }

        public static VRController SecondaryHand { get { return Handedness == Handedness.RightHanded ? LeftHand : RightHand; } }
        public static VRController PrimaryHand { get { return Handedness == Handedness.RightHanded ? RightHand : LeftHand; } }

        public static VRController LeftHand { get; set; }
        public static VRController RightHand { get; set; }

        public static Transform Head;

        public static Transform SecondarySmoothed;
        public static Transform PrimarySmoothed;
        public static Transform HeadSmoothed;

        public static Transform GetSmooth(Hand hand) { return hand == Hand.Primary ? PrimarySmoothed : SecondarySmoothed; }

        public static Hand PhysicalToLogical(PhysicalHand hand)
        {
            if(Handedness == Handedness.RightHanded)
            {
                return hand == PhysicalHand.Left ? Hand.Secondary : Hand.Primary;
            }
            else
            {
                return hand == PhysicalHand.Left ? Hand.Primary : Hand.Secondary;
            }
        }

        public static void SetHandedness(Handedness handedness)
        {
            PlayerPrefs.SetString("ANIMVR_HANDEDNESS", handedness == Handedness.RightHanded ? "Right" : "Left");
            Handedness = handedness;
        }

        public static VRController Get(Hand hand)
        {
            if (hand == Hand.Secondary)
                return VRInput.SecondaryHand;
            else if (hand == Hand.Primary)
                return VRInput.PrimaryHand;
            return null;
        }  
        
        public static VRController GetOpposite(Hand hand)
        {
            if (hand == Hand.Secondary)
                return VRInput.PrimaryHand;
            else if (hand == Hand.Primary)
                return VRInput.SecondaryHand;
            return null;
        }
        
        public static Vector3 GetControllerCentroid() {
            return (VRInput.SecondaryHand.transform.localPosition+ VRInput.PrimaryHand.transform.localPosition) / 2.0f;
        }

        public static Quaternion GetControllerOrientation() {
            Vector3 direction = VRInput.RightHand.transform.position - VRInput.LeftHand.transform.position;
            Vector3 up = (VRInput.LeftHand.transform.forward + VRInput.RightHand.transform.forward) / 2.0f;
            return Quaternion.LookRotation(direction, up);
        }

        public static float GetControllerDistance() {
            return Vector3.Distance(VRInput.SecondaryHand.transform.localPosition, VRInput.PrimaryHand.transform.localPosition);
        }
    }
}
