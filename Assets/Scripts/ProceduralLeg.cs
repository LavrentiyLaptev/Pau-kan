using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralLeg : MonoBehaviour
{
    public Transform target;
    public Transform origin;
    public Vector3 prevPosition;
    public Rigidbody rb;
    public float sphereRadius = 1.2f;
    public float stepHeight;
    public float legSpeed;
    public float step1Speed;
    public bool isStick = true;
    private Vector3 legVelocity = Vector3.zero;
    public Vector3 raycastOffset;
    private const float rayDownDst = 2;
    private const float raySideDst = 2.5f;
    void Start()
    {
        prevPosition = target.position;
    }

    void FixedUpdate(){
        if(isStick){
            Stick();
            // if(Vector3.Distance(origin.position, prevPosition) >= sphereRadius){
            //     FindNewPositon();
            // }
        }
    }

    public void FindNewPositon(){
        // if(Vector3.Distance(origin.position, prevPosition) < sphereRadius) return;
        Vector3 dir = rb.velocity.normalized;
        Vector3 endPosition = origin.transform.position + dir * (sphereRadius * 0.9f);
        // transform.position = prevPosition;
        isStick = false;
        StartCoroutine(ProceduralStep(endPosition));

    }

    private void Stick(){
        target.position = prevPosition;
    }

    private IEnumerator ProceduralStep(Vector3 positionToStep){
        Vector3 dir = (positionToStep - target.position);
        Vector3 heightPoint = target.position + dir * sphereRadius * 0.6f;
        heightPoint.y += stepHeight;
        dir = dir.normalized;
        legVelocity = Vector3.zero;
        Vector3 step1Dir = (heightPoint - target.position).normalized;

        while(target.position.y < heightPoint.y){
            target.position += step1Dir * step1Speed * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        legVelocity = Vector3.zero;
        Vector3 step2Dir = (positionToStep - target.position).normalized;
        Vector3 endPoint = RaycastStepPoint();
        while(Vector3.Distance(target.position, endPoint) >= step1Speed * Time.fixedDeltaTime){
            target.position += (endPoint - target.position).normalized * step1Speed * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        target.position = endPoint;
        prevPosition = target.position;
        isStick = true;
    }

    private Vector3 RaycastStepPoint(){
        RaycastHit hit;
        if(Physics.Raycast(target.position + transform.up * 1, -transform.forward, out hit, raySideDst, LayerMask.GetMask("Ground"), QueryTriggerInteraction.Ignore)){
            Debug.Log("side point " + hit.point);
            return hit.point;
        }
        else if(Physics.Raycast(target.position + transform.up * 1, -transform.up, out hit, rayDownDst, LayerMask.GetMask("Ground"), QueryTriggerInteraction.Ignore)){
            Debug.Log("down point " + hit.point);
            return hit.point;
        } 
        return origin.position;
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(target.position + transform.up * 1, -target.up);
    }
}
