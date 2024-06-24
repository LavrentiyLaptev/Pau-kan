using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Vector3 teleportPoint;

    void OnTriggerEnter(Collider col){
        col.transform.position = teleportPoint;
    }
}
