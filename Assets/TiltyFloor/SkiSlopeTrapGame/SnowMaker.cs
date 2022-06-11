using System;
using System.Collections;
using System.Collections.Generic;
using ATVR;
using Normal.Realtime;
using ToyBoxHHH;
using UnityEngine;

public class SnowMaker : MonoBehaviour
{
    public HoverGrabTool GetTool(Hand hand)
    {
        return hand == Hand.Primary
            ? HoverGrabToolFinder.instance.grabToolRight
            : HoverGrabToolFinder.instance.grabToolLeft;
    }

    public GameObject snowballPrefab;
    public List<GameObject> snowballSpawnedList = new List<GameObject>();
    public Transform snowballParent;

    public Vector3 spawnOffsetPos;

    public void ClearAll()
    {
        foreach (var s in snowballSpawnedList)
        {
            Realtime.Destroy(s);
        }

        snowballSpawnedList.Clear();
    }

    [DebugButton]
    public GameObject SpawnOne()
    {
        var s = Realtime.Instantiate(snowballPrefab.name, new Realtime.InstantiateOptions()
        {
            destroyWhenLastClientLeaves = true,
            destroyWhenOwnerLeaves = false,
            ownedByClient = false,
        });

        return s;
    }

    private void Update()
    {
        SpawnOnClick(Hand.Primary);
        SpawnOnClick(Hand.Secondary);
    }

    private void SpawnOnClick(Hand h)
    {
        if (!GetTool(h).isHighlighting)
        {
            if (VRInput.Get(h).GetPressDown(Button.Grip))
            {
                // spawn thing
                var s = SpawnOne();

                s.GetComponent<RealtimeTransform>().RequestOwnership();
                // gotta position it!!!
                var grabPos = GetTool(h).gizmo.TransformPoint(spawnOffsetPos);
                s.transform.position = grabPos;

                // grab it
                GrabWithWeirdHack(GetTool(h).transform, GetTool(h), s, grabPos);
                //GetTool(h).Grab(s.GetComponent<HoverGrabbableObject>());
            }
        }
    }


    private void GrabWithWeirdHack(Transform grabHand, HoverGrabTool grabTool, GameObject spawnedObj, Vector3 oPos)
    {
        // grab it. after a frame cause otherwise something wrong with the pivot?R??R?R?R?R?R?
        // save pos/rot offsets this frame, and then move it and grab it the next frame.... so it keeps the offsets.
        var offsetPos = grabHand.InverseTransformPoint(oPos);// oPos;
        var offsetRot = Quaternion.Inverse(grabHand.rotation) * spawnedObj.transform.rotation;
        StartCoroutine(pTween.WaitFrames(1, () =>
        {
            spawnedObj.transform.position = grabHand.TransformPoint(offsetPos);
            spawnedObj.transform.rotation = grabHand.rotation * offsetRot;
            grabTool.Grab(spawnedObj.GetComponent<HoverGrabbableObject>());
        }));
    }
}