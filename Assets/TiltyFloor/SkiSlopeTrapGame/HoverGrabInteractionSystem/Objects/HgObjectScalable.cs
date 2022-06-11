using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class HgObjectScalable : MonoBehaviour
{
    private static GameObject _dummy;
    public static GameObject dummy
    {
        get
        {
            if (_dummy == null)
            {
                _dummy = new GameObject("[dummy]");
                _dummy.hideFlags = HideFlags.HideAndDontSave;
            }
            return _dummy;
        }
    }

    public float scaleFactorPerSec = 2.5f;

    public Vector3 minScale = new Vector3(0.01f, 0.01f, 0.01f);
    public Vector3 maxScale = new Vector3(1f, 1f, 1f);

    public Transform toScale;

    private void OnEnable()
    {
        // fuck
    }
    private void OnDisable()
    {

    }

    private void Reset()
    {
        toScale = transform;
    }

    private void OnValidate()
    {
        if (toScale == null)
            toScale = transform;
    }

    public bool IsScaleUnderMax()
    {
        if (toScale.localScale.x > maxScale.x)
            return false;
        if (toScale.localScale.y > maxScale.y)
            return false;
        if (toScale.localScale.z > maxScale.z)
            return false;

        return true;
    }

    public bool IsScaleOverMin()
    {
        if (toScale.localScale.x < minScale.x)
            return false;
        if (toScale.localScale.y < minScale.y)
            return false;
        if (toScale.localScale.z < minScale.z)
            return false;

        return true;
    }

    public void Update_DoScaleIt(Vector2 joy, Transform scaleAroundPosition)
    {
        if (joy.x > 0)
        {
            if (IsScaleUnderMax())
            {
                // scale around a particular position
                dummy.transform.position = scaleAroundPosition.position;
                var oldParent = toScale.parent;
                toScale.SetParent(dummy.transform);
                dummy.transform.localScale *= 1 + (scaleFactorPerSec - 1f) * Time.deltaTime * joy.x;
                toScale.SetParent(oldParent);
                toScale.localPosition = Vector3.zero;
            }
        }
        else if (joy.x < 0)
        {
            if (IsScaleOverMin())
            {
                // scale around a particular position
                dummy.transform.position = scaleAroundPosition.position;
                var oldParent = toScale.parent;
                toScale.SetParent(dummy.transform);
                dummy.transform.localScale /= 1 + (scaleFactorPerSec - 1f) * Time.deltaTime * Mathf.Abs(joy.x);
                toScale.SetParent(oldParent);
                toScale.localPosition = Vector3.zero;
            }
        }
    }
}