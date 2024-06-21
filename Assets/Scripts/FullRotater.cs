using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullRotater : MonoBehaviour
{

    public float mouseSpeed = 400f;

    void Update(){
        MouseInputs();
    }   

    private void MouseInputs(){
        transform.Rotate(0, Input.GetAxis("Mouse X") * mouseSpeed * Time.deltaTime, 0, Space.World);
        transform.Rotate(-Input.GetAxis("Mouse Y") * mouseSpeed * Time.deltaTime, 0, 0, Space.Self);
    }
}
