using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorCollider : MonoBehaviour
{
    public static FloorCollider instance;
    public Collider collider;
    
    private void Awake()
    {
        instance = this;
        collider = GetComponent<Collider>();
    }
    
}