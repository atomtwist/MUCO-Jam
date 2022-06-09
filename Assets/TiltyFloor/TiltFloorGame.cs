using System;
using System.Collections;
using System.Collections.Generic;
using ATVR;
using Normal.Realtime;
using UnityEngine;

public class TiltFloorGame : MonoBehaviour
{
    [Header("Remember the TakeOwnership component")]
    public RealtimeTransform realtimeTransform;

    public Rigidbody rb;

    public float playerAddForceFromHead = 5f;

    public Transform localPlayer;

    public LayerMask floorLayer;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!NetworkManager.Instance.realtime.connected)
            return;
        
        if (realtimeTransform.isOwnedLocallySelf)
        {
            // add froces for all plaeyrs
            for (int i = 0; i < NetworkManager.Instance.avatarManager.avatars.Count; i++)
            {
                var a = NetworkManager.Instance.avatarManager.avatars[i];
                rb.AddForceAtPosition(Vector3.down * playerAddForceFromHead, a.head.position);

            }
            
        }

        // set player on floor....?E?E!
        var floorHeightUnderHead = 0f;
        if (Physics.Raycast(VRInput.Head.transform.position + Vector3.up*10, Vector3.down, out var hit, 999, floorLayer))
        {
            floorHeightUnderHead = hit.point.y;
        }
        
        var pos = localPlayer.transform.position;
        pos.y = floorHeightUnderHead;
        localPlayer.transform.position = pos;

    }
}
