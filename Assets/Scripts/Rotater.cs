using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotater : MonoBehaviour
{
    public Vector3 rotationAxis;
    public float speed;
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(rotationAxis, speed * Time.fixedDeltaTime);
    }
}
