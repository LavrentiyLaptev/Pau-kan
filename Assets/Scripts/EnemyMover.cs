using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    public Transform targetTransform;
    public float speed;
    public float comfortDistance;

    void FixedUpdate(){
        if(targetTransform != null){
            Move();
        }
    }

    private void Move(){
        if(Vector3.Distance(transform.position, targetTransform.position) <= comfortDistance) return;
        Vector3 targetPos = targetTransform.position;
        targetPos.y = transform.position.y;
        Vector3 translateDir = (targetPos - transform.position).normalized;

        transform.Translate(translateDir * speed * Time.fixedDeltaTime, Space.World);
    }
}
