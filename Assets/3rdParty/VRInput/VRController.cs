using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ATVR;
using UnityEngine.XR;


public enum AxisArea
{
    Middle,
    Right,
    Bottom,
    Left,
    Top
}

public class VRController : MonoBehaviour
{
    public PhysicalHand hand;
    public bool useAvatar;

    public Transform smoothTransform
    {
        get { return VRInput.GetSmooth(VRInput.PhysicalToLogical(hand)) ?? transform; }
    }

    //register controllers with the static input class
    public void Awake()
    {
        Debug.Log("Registering hand: " + hand);
        if (hand == PhysicalHand.Left)
        {
            VRInput.LeftHand = this;
        }
        else if (hand == PhysicalHand.Right)
        {
            VRInput.RightHand = this;
        }
    }

    private bool parented;

    public void Disable()
    {
        Debug.Log("Disable hand: " + hand);
        if (hand == PhysicalHand.Left)
            VRInput.LeftHand = null;
        else if (hand == PhysicalHand.Right)
            VRInput.RightHand = null;
    }

    public void Destroy()
    {
        if (hand == PhysicalHand.Left)
            VRInput.LeftHand = null;
        else if (hand == PhysicalHand.Right)
            VRInput.RightHand = null;
    }

    Vector2 oldAxis0, deltaAxis0;

    public Vector3 delta;
    public Quaternion rotDelta;

    public bool occupied { get; set; }
    public bool isTracking;
    private Vector3 oldControllerPos;
    private Quaternion oldControllerRot;
    private Vector3 oldVelocity;
    private Vector3 oldAngularVelocity;
    private bool triggered;

    void ControllerDelta()
    {
        if (oldControllerPos != Vector3.zero)
            delta = transform.position - oldControllerPos;
        oldControllerPos = transform.position;

        if (oldControllerRot != Quaternion.identity)
        {
            rotDelta = Quaternion.Inverse(oldControllerRot) * transform.rotation;
        }

        oldControllerRot = transform.rotation;
    }


    void Update()
    {
        ControllerDelta();


        if (useAvatar)
        {
            isTracking = UpdateVelocity(GetXRNodeHand(), ref oldVelocity);
            isTracking |= UpdateAngularVelocity(GetXRNodeHand(), ref oldAngularVelocity);
        }
        else
        {
            isTracking = UpdatePose(GetXRNodeHand(), ref oldControllerPos, ref oldControllerRot, ref oldVelocity);
            isTracking |= UpdateAngularVelocity(GetXRNodeHand(), ref oldAngularVelocity);
            transform.position = oldControllerPos;
            transform.rotation = oldControllerRot;
        }

        Late();
    }

    private static bool UpdateAngularVelocity(XRNode node, ref Vector3 angularVelocity)
    {
        //pos rot vel
        List<XRNodeState> nodeStates = new List<XRNodeState>();
        InputTracking.GetNodeStates(nodeStates);

        foreach (XRNodeState nodeState in nodeStates)
        {
            if (nodeState.nodeType == node)
            {
                Vector3 nodeAV;

                //make sure we only update when we#re tracking and got fresh data
                bool gotAV = nodeState.TryGetAngularVelocity(out nodeAV);

                if (gotAV)
                    angularVelocity = nodeAV;

                return gotAV;
            }
        }

        return false;
    }

    private static bool UpdateVelocity(XRNode node, ref Vector3 velocity)
    {
        //pos rot vel
        List<XRNodeState> nodeStates = new List<XRNodeState>();
        InputTracking.GetNodeStates(nodeStates);

        foreach (XRNodeState nodeState in nodeStates)
        {
            if (nodeState.nodeType == node)
            {
                Vector3 nodePosition;
                Quaternion nodeRotation;
                Vector3 nodeVelocity;

                bool gotVelocity = nodeState.TryGetVelocity(out nodeVelocity);

                if (gotVelocity)
                    velocity = nodeVelocity;

                return gotVelocity;
            }
        }

        return false;
    }

    // Given an XRNode, get the current position & rotation. If it's not tracking, don't modify the position & rotation.
    private static bool UpdatePose(XRNode node, ref Vector3 position, ref Quaternion rotation, ref Vector3 velocity)
    {
        //pos rot vel
        List<XRNodeState> nodeStates = new List<XRNodeState>();
        InputTracking.GetNodeStates(nodeStates);

        foreach (XRNodeState nodeState in nodeStates)
        {
            if (nodeState.nodeType == node)
            {
                Vector3 nodePosition;
                Quaternion nodeRotation;
                Vector3 nodeVelocity;
                bool gotPosition = nodeState.TryGetPosition(out nodePosition);
                bool gotRotation = nodeState.TryGetRotation(out nodeRotation);
                bool gotVelocity = nodeState.TryGetVelocity(out nodeVelocity);

                //make sure we only update when we#re tracking and got fresh data

                if (gotPosition)
                    position = nodePosition;
                if (gotRotation)
                    rotation = nodeRotation;
                if (gotVelocity)
                    velocity = nodeVelocity;

                return gotPosition;
            }
        }

        return false;
    }

    #region Unity Input Wrapper

    XRNode GetXRNodeHand()
    {
        return hand == PhysicalHand.Left ? XRNode.LeftHand : XRNode.RightHand;
        ;
    }

    InputDevice GetXRDevice()
    {
        return InputDevices.GetDeviceAtXRNode(GetXRNodeHand());
    }


    private Dictionary<Button, bool> lastButtonStates = new Dictionary<Button, bool>()
    {
        {Button.System, false},
        {Button.ButtonTwo, false},
        {Button.ButtonOne, false},
        {Button.Joystick, false},
        {Button.Trigger, false},
        {Button.Grip, false},
    };

    //update dictionary at end of frame
    private void Late()
    {
        lastButtonStates[Button.System] = GetXRDeviceButtonState(Button.System);
        lastButtonStates[Button.ButtonTwo] = GetXRDeviceButtonState(Button.ButtonTwo);
        lastButtonStates[Button.Joystick] = GetXRDeviceButtonState(Button.Joystick);
        lastButtonStates[Button.Trigger] = GetXRDeviceButtonState(Button.Trigger);
        lastButtonStates[Button.Grip] = GetXRDeviceButtonState(Button.Grip);
        lastButtonStates[Button.ButtonOne] = GetXRDeviceButtonState(Button.ButtonOne);
    }

    private bool GetButtonState(Button button, ButtonPressType pressType)
    {
        bool currentButton = GetXRDeviceButtonState(button);

        if (pressType == ButtonPressType.PressDown)
        {
            if (lastButtonStates[button] == false && currentButton)
            {
                return true;
            }
        }

        if (pressType == ButtonPressType.PressUp)
        {
            if (lastButtonStates[button] && currentButton == false)
            {
                return true;
            }
        }

        if (pressType == ButtonPressType.Pressed)
        {
            return currentButton;
        }

        return false;
    }

    bool GetXRDeviceButtonState(Button button)
    {
        bool state;
        switch (button)
        {
            case Button.System:
                GetXRDevice().TryGetFeatureValue(CommonUsages.menuButton, out state);
                return state;
            case Button.ButtonTwo:
                GetXRDevice().TryGetFeatureValue(CommonUsages.secondaryButton, out state);
                return state;
            case Button.Grip:
                GetXRDevice().TryGetFeatureValue(CommonUsages.gripButton, out state);
                return state;
            case Button.Joystick:
                GetXRDevice().TryGetFeatureValue(CommonUsages.primary2DAxisClick, out state);
                return state;
            case Button.Trigger:
                GetXRDevice().TryGetFeatureValue(CommonUsages.triggerButton, out state);
                if (state && GetTrigger() > 0.01f) return true;
                return false;
            case Button.ButtonOne:
                GetXRDevice().TryGetFeatureValue(CommonUsages.primaryButton, out state);
                return state;
            default:
                GetXRDevice().TryGetFeatureValue(CommonUsages.primary2DAxisClick, out state);
                return state;
        }
    }

    #endregion


    public bool GetPressDown(Button button)
    {
        return GetButtonState(button, ButtonPressType.PressDown);
    }

    public bool GetPressUp(Button button)
    {
        return GetButtonState(button, ButtonPressType.PressUp);
    }

    public bool GetPress(Button button)
    {
        return GetButtonState(button, ButtonPressType.Pressed);
    }

    //Touch Beh
    public bool GetTouchDown(Button button)
    {
        return GetButtonState(button, ButtonPressType.PressDown);
    }

    public bool GetTouchUp(Button button)
    {
        return GetButtonState(button, ButtonPressType.PressUp);
    }

    public bool GetTouch(Button button)
    {
        return GetButtonState(button, ButtonPressType.Pressed);
    }

    public AxisArea GetAxisArea(float middleRadius)
    {
        var axis = GetJoystick();

        if (axis.magnitude < middleRadius) return AxisArea.Middle;

        Vector2 from = Vector2.one;

        float ang = Vector2.Angle(from, axis);
        Vector3 cross = Vector3.Cross(from, axis);

        if (cross.z > 0)
            ang = 360 - ang;

        int quadrant = Mathf.Clamp(Mathf.FloorToInt(ang / 360 * 4), 0, 3);
        return (AxisArea) (quadrant + 1);
    }


    public Vector3 GetVelocity()
    {
        return oldVelocity;
    }

    public Vector3 GetAngularVelocity()
    {
        return oldAngularVelocity;
    }

    public Vector2 GetDeltaAxis()
    {
        return deltaAxis0;
    }

    public Vector2 GetJoystick()
    {
        Vector2 joyStick;
        GetXRDevice().TryGetFeatureValue(CommonUsages.primary2DAxis, out joyStick);
        return joyStick;
    }

    public float GetGrip()
    {
        float grip;
        GetXRDevice().TryGetFeatureValue(CommonUsages.grip, out grip);
        return grip;
    }

    public float GetTrigger()
    {
        float trigger;
        GetXRDevice().TryGetFeatureValue(CommonUsages.trigger, out trigger);
        return trigger;
    }

    /// <summary>
    /// Vibrates controller with #amount# intensity (0..1) for one frame. 
    /// </summary>
    public void Vibrate(float intensity)
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(GetXRNodeHand());

        HapticCapabilities capabilities;
        if (device.TryGetHapticCapabilities(out capabilities))
        {
            if (capabilities.supportsImpulse)
            {
                uint channel = 0;
                float amplitude = intensity;
                float duration = .01f;
                device.SendHapticImpulse(channel, amplitude, duration);
            }
        }
    }

    public void VibrateLong(float seconds, float intensity)
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(GetXRNodeHand());

        HapticCapabilities capabilities;
        if (device.TryGetHapticCapabilities(out capabilities))
        {
            if (capabilities.supportsImpulse)
            {
                uint channel = 0;
                float amplitude = intensity;
                float duration = seconds;
                device.SendHapticImpulse(channel, amplitude, duration);
            }
        }
    }
}