using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private Vector2 inp;

    public Rigidbody rb;

    public float force = 10f;

    private void Update()
    {
        inp = new Vector2(Input.GetKey(KeyCode.A) ? -1 : Input.GetKey(KeyCode.D) ? 1 : 0, 
            Input.GetKey(KeyCode.S) ? -1 : Input.GetKey(KeyCode.W) ? 1 : 0);
    }

    private void FixedUpdate()
    {
        rb.AddTorque(new Vector3(inp.x, 0, inp.y) * force);
        
        
        
    }
}