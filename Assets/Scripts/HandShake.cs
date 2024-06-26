using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class HandShake : MonoBehaviour
{
    public Rigidbody rb;
    public Transform transformToShake;
    private float shakeTime;
    public float yMaxShake = 0.75f;
    public float yShakeFrequency = 3f;
    private Vector2 inputDirs = Vector2.zero;
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        Shake();
        inputDirs = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
    }
    

    private void Shake(){
        
        transformToShake.localPosition = Vector3.zero + new Vector3(Mathf.Sin(shakeTime * yShakeFrequency * inputDirs.magnitude) * yMaxShake, 0, 0);

        shakeTime += Time.fixedDeltaTime;
    }
}
