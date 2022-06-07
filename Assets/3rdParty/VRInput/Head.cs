using System.Collections;
using System.Collections.Generic;
using ATVR;
using UnityEngine;

public class Head : MonoBehaviour
{
    private void Awake()
    {
        VRInput.Head = transform;
    }
}