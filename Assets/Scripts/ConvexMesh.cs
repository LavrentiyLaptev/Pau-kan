using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvexMesh : MonoBehaviour
{
    void Start(){
        GetComponent<MeshCollider>().convex = true;
    }
}
