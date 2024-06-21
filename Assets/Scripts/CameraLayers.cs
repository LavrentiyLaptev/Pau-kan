using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLayers : MonoBehaviour
{
    private Camera mainCamera;
    void Start(){
        mainCamera = GetComponent<Camera>();
        mainCamera.cullingMask = ~LayerMask.GetMask("Weapon");
    }
}
