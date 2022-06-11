using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowTerrain : MonoBehaviour
{
    public static SnowTerrain instance;

    private void Awake()
    {
        instance = this;
    }
}
