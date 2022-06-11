using System;
using System.Collections;
using System.Collections.Generic;
using ATVR;
using UnityEngine;

public class PlayerHeightOnTiltedFloorSurface : MonoBehaviour
{
    [Header("Sets player Y to floor height under head.")]
    public Transform localPlayer;

    public LayerMask floorLayer = -1;

    private void Update()
    {
        // set player on floor....?E?E!
        var floorHeightUnderHead = 0f;
        if (Physics.Raycast(VRInput.Head.transform.position + Vector3.up * 10, 
                Vector3.down, out var hit, 999, floorLayer))
        {
            floorHeightUnderHead = hit.point.y;
        }
        
        var pos = localPlayer.transform.position;
        pos.y = floorHeightUnderHead;
        localPlayer.transform.position = pos;
    }
}
