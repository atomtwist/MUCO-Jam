using System.Collections;
using System.Collections.Generic;
using ToyBoxHHH;
using UnityEngine;

public class CloverSpawner : MonoBehaviour
{
    public GameObject bigCloverPatchPrefab;
    public List<GameObject> bigCloverPatchSpawnedList = new List<GameObject>();
    public Transform bigCloverPatchParent;
    public Vector2 bigCloverPatchScaleRange = new Vector2(0.9f, 1.1f);

    public GameObject fourLeafPrefab;
    public List<GameObject> fourLeafSpawnedList = new List<GameObject>();
    public Transform fourLeafParent;
    public Vector2 fourLeafScaleRange = new Vector2(0.7f, 1.3f);

    public List<CloverSpawner> subSpawners = new List<CloverSpawner>();

    [Space] public int fourLeafs = 7;
    public int bigPatches = 10;

    public Vector2 spawnSize = new Vector2(19, 9);

    public int equalDistributionMonteCarlo = 10;

    private void OnValidate()
    {
        RefreshSubSpawners();
    }

    [DebugButton]
    private void RefreshSubSpawners()
    {
        subSpawners.Clear();
        subSpawners.AddRange(GetComponentsInChildren<CloverSpawner>());
        subSpawners.Remove(this);
        SetDirtyMe();
    }

    [DebugButton]
    public void ClearAll()
    {
        for (int i = 0; i < subSpawners.Count; i++)
        {
            subSpawners[i].ClearAll();
        }

        for (int i = 0; i < bigCloverPatchSpawnedList.Count; i++)
        {
            if (Application.isPlaying)
            {
                Destroy(bigCloverPatchSpawnedList[i].gameObject);
            }
            else
            {
                DestroyImmediate(bigCloverPatchSpawnedList[i].gameObject);
            }
        }

        bigCloverPatchSpawnedList.Clear();

        for (int i = 0; i < fourLeafSpawnedList.Count; i++)
        {
            if (Application.isPlaying)
            {
                Destroy(fourLeafSpawnedList[i].gameObject);
            }
            else
            {
                DestroyImmediate(fourLeafSpawnedList[i].gameObject);
            }
        }

        fourLeafSpawnedList.Clear();
    }

    public GameObject Spawn(GameObject prefab, List<GameObject> list, Transform parent, Vector3 pos, Quaternion rot,
        Vector2 scaleRange)
    {
        var p = Instantiate(prefab, pos, rot, parent);
        p.transform.localScale *= scaleRange.Random();
        list.Add(p);
        return p;
    }

    [DebugButton]
    public void SpawnAll()
    {
        for (int i = 0; i < fourLeafs; i++)
        {
            var pos = GetPosFurthestFromObjects(fourLeafSpawnedList);
            Spawn(fourLeafPrefab, fourLeafSpawnedList, fourLeafParent, pos,
                Quaternion.Euler(0, Random.Range(0, 360f), 0), fourLeafScaleRange);
        }

        for (int i = 0; i < bigPatches; i++)
        {
            var pos = GetPosFurthestFromObjects(bigCloverPatchSpawnedList);
            Spawn(bigCloverPatchPrefab, bigCloverPatchSpawnedList, bigCloverPatchParent, pos,
                Quaternion.Euler(0, Random.Range(0, 360), 0), bigCloverPatchScaleRange);
        }

        for (int i = 0; i < subSpawners.Count; i++)
        {
            subSpawners[i].SpawnAll();
        }

        SetDirtyMe();
    }

    private void SetDirtyMe()
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    // get pos furthest from all other objects, by trying a few (equalDistributionMonteCarlo) random positions and getting the furthest one.
    private Vector3 GetPosFurthestFromObjects(List<GameObject> list)
    {
        // return a position that's furthest away from the other things with a bit of monte carlo...
        var pos = RandomPos();
        Vector3 maxPos = pos;
        float absoluteMaxDist = GetMinDist(pos, list);
        for (int i = 0; i < equalDistributionMonteCarlo; i++)
        {
            pos = RandomPos();

            float minDist = GetMinDist(pos, list);
            if (minDist > absoluteMaxDist)
            {
                absoluteMaxDist = minDist;
                maxPos = pos;
            }
        }

        return maxPos;
    }

    private Vector3 RandomPos()
    {
        return transform.position + new Vector3(Random.Range(-1f, 1f) * 0.5f * spawnSize.x, 0,
            Random.Range(-1, 1f) * 0.5f * spawnSize.y);
    }

    // min dist to all other four leaf clovers
    private float GetMinDist(Vector3 pos, List<GameObject> list)
    {
        var minSqrDist = float.MaxValue;
        for (int i = 0; i < list.Count; i++)
        {
            var dist = pos - list[i].transform.position;
            var distSqr = dist.sqrMagnitude;
            if (distSqr < minSqrDist)
            {
                minSqrDist = distSqr;
            }
        }

        return Mathf.Sqrt(minSqrDist);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnSize.x, 0.1f, spawnSize.y));
    }
}