using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform cameraHolder;
    public float mouseSpeed;
    

    void Update(){
        MouseInputs();
    }   

    private void MouseInputs(){
        transform.Rotate(0, Input.GetAxis("Mouse X") * mouseSpeed * Time.deltaTime, 0);
        cameraHolder.Rotate(-Input.GetAxis("Mouse Y") * mouseSpeed * Time.deltaTime, 0, 0, Space.Self);
    }
}
