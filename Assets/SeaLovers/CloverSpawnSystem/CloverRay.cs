using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloverRay : MonoBehaviour
{
    // raycast from the camera to the ground.
    // if we hit a clover, count up 3 seconds and highlight it....??!?!?!
    // also limit to being close enough
    public Transform rayStart;
    public float maxDist = 3f;
    public Transform gizmo;
    private static RaycastHit[] resultsCache = new RaycastHit[100];

    public Collider floorCollider => FloorCollider.instance.collider;

    private Dictionary<CloverWithLeaves, float> cloverTimers = new Dictionary<CloverWithLeaves, float>();

    //private CloverWithLeaves curAimedClover;
    //private float curAimedCloverTimer = 0f;
    public float cloverViewTimeToPick = 3f;
    public float cloverAlreadyFoundTimeToShine = 0.7f;

    List<CloverWithLeaves> toPick = new List<CloverWithLeaves>();
    List<CloverWithLeaves> toHighlightAlreadyPicked = new List<CloverWithLeaves>();

    //List<CloverWithLeaves> toRemove = new List<CloverWithLeaves>();
    private float lastTimeToUpdateRare;

    private void Update()
    {
        bool gizmoPlaced = false;

        var count = Physics.RaycastNonAlloc(rayStart.position, rayStart.forward, resultsCache, 999);
        for (int i = 0; i < count; i++)
        {
            var r = resultsCache[i];

            // find the floor to put the gizmo?
            if (r.collider == floorCollider)
            {
                var magg = (r.point - rayStart.position).magnitude;
                Debug.Log(magg);
                if (magg < maxDist)
                {
                    // put gizmo here.
                    gizmo.position = r.point;
                    gizmo.up = r.normal;
                    gizmoPlaced = true;
                }
            }

            // find clover. add time to it.
            var aimedClover = r.transform.GetComponentInParent<CloverWithLeaves>();
            if (aimedClover != null)
            {
                var magg = (r.point - rayStart.position).magnitude;
                if (magg < maxDist)
                {
                    if (cloverTimers.ContainsKey(aimedClover))
                    {
                        cloverTimers[aimedClover] += Time.deltaTime;
                    }
                    else
                    {
                        cloverTimers[aimedClover] = 0;
                    }
                }
            }
        }

        toPick.Clear();
        toHighlightAlreadyPicked.Clear();
        //toRemove.Clear();

        // check timings
        foreach (var clo in cloverTimers)
        {
            if (clo.Key.found)
            {
                if (clo.Value >= cloverAlreadyFoundTimeToShine)
                {
                    toHighlightAlreadyPicked.Add(clo.Key);
                }
            }
            else
            if (clo.Value >= cloverViewTimeToPick)
            {
                toPick.Add(clo.Key);
            }
            
        }

        foreach (var p in toHighlightAlreadyPicked)
        {
            p.HighlightAlreadyFound();
        }

        foreach (var p in toPick)
        {
            p.PickClover();
            cloverTimers.Remove(p);
        }

        //foreach (var p in toRemove)
        //{
        //    cloverTimers.Remove(p);
        //}

        // rare update for clearing the old ones.
        if (Time.time - lastTimeToUpdateRare > cloverViewTimeToPick + 0.5f)
        {
            lastTimeToUpdateRare = Time.time;
            Debug.Log("Update rare");
            cloverTimers.Clear();
        }


        gizmo.gameObject.SetActive(gizmoPlaced);
    }
}