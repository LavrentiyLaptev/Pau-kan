using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float cameraSpeed;
    public Transform target;
    private float Yoffset;
    void Start(){
        Yoffset = transform.position.y - target.position.y;
    }
    void LateUpdate(){
        Vector3 targetPos = new Vector3(transform.position.x, target.position.y + Yoffset, target.position.z);
        transform.position = targetPos;
    }
}
