using System.Collections;
using System.Collections.Generic;
using ATVR;
using Normal.Realtime;
using UnityEngine;

public class BallResetter : MonoBehaviour
{
    public RealtimeTransform realtimeTransform;

    public Rigidbody rb;

    public Transform restartPos;
    
    private void Update()
    {
        if (VRInput.Get(Hand.Primary).GetPressDown(Button.ButtonOne))
        {
            realtimeTransform.RequestOwnership();
            rb.position = restartPos.position;
            rb.rotation = restartPos.rotation;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            
        }
    }

}
