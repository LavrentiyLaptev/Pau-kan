using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;

public class PlayerController2 : MonoBehaviour
{
    private Vector2 inputDirs = Vector2.zero;
    private Vector3 moveDirectionForward;
    private Vector3 moveDirectionRight;
    public float speed;
    public float gravity;
    public float sphereRadius;
    public float angleStep = 10;
    public float newNormalRayDistance;
    private int groundMask;
    public float newNormalPointDst = 0.1f;
    public float cornerStateMinDst = 0.6f;
    public float cornerStateMaxDst = 0.6f;
    public float cornerLimitDst = 1f;
    private Vector3 lastDir = Vector3.zero;
    public Vector3 lastSurface = Vector3.zero;
    private Vector3 lastCornerPoint = Vector3.zero;
    
    private Vector3 newSurfacePoint = Vector3.zero;
    public bool cornerState = false;
    public bool DebugCornerDetection = false;

    void Start()
    {
        groundMask = LayerMask.GetMask("Ground");
    }

    void Update()
    {
        Inputs();
    }

    void FixedUpdate(){
        GetTouchPounts();
        Movement();
    }

    private void Inputs(){
        inputDirs = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    private void ApplyGravity(Vector3 gravityDir){
        transform.Translate(gravityDir * gravity * Time.fixedDeltaTime, Space.World);
    }

    private void Movement(){
        Vector3 forceDir = ((moveDirectionForward * inputDirs.y) + (moveDirectionRight * inputDirs.x)).normalized;
        lastDir = forceDir;
        transform.Translate(forceDir * speed * Time.fixedDeltaTime, Space.World);
    }

    private void GetTouchPounts(){
        
        Collider[] cols = new Collider[10];
        int colNums = Physics.OverlapSphereNonAlloc(transform.position, sphereRadius, cols, groundMask, QueryTriggerInteraction.Ignore);
        // int colNums = Physics.OverlapBoxNonAlloc(transform.position, Vector3.one * sphereRadius, cols, Quaternion.Euler(0, 0, 0), groundMask, QueryTriggerInteraction.Ignore);
        Vector3[] touchPoints = new Vector3[colNums];

        // FIND TOUCH POINTS

        for (int i = 0; i < colNums; i++)
        {
            touchPoints[i] = cols[i].ClosestPoint(transform.position);
        }

        // FIND NORMALS

        Vector3[] normals = new Vector3[colNums];
        for (int i = 0; i < colNums; i++)
        {
            normals[i] = GetSurfaceNormal(touchPoints[i], sphereRadius + 0.025f);
        }

        // GET CLOSEST NORMAL
        float minAngle = float.MinValue;
        Vector3 surfaceNormal = Vector3.positiveInfinity;

        for (int i = 0; i < normals.Length; i++)
        {   
            if(normals[i] == Vector3.positiveInfinity) continue;
            float angle = Vector3.Angle(normals[i], lastDir);
            if(angle > minAngle){
                minAngle = angle;
                surfaceNormal = normals[i];
            }
        }

        if(cornerState && Vector3.Distance(transform.position, lastCornerPoint) >= cornerLimitDst){
            cornerState = false;
        }

        if(surfaceNormal.magnitude <= 1){
            
            lastSurface = surfaceNormal;
            // Debug.Log(surfaceNormal + " || " + lastSurface);
            cornerState = false;
            GetDirection(surfaceNormal);

        }else{ 
            float cornerDistance = Vector3.Distance(transform.position, lastCornerPoint);
            if(cornerState && cornerDistance <= cornerStateMinDst){
                // Debug.Log("NON Gravity CORNER STATE");
                GetDirection(lastSurface);
                return;
            }

            if(cornerState && cornerDistance >= cornerStateMaxDst){
                // Debug.Log("Gravity CORNER STATE");
                ApplyGravity(-lastSurface);
                return;
                // lastCornerPoint = transform.position;
            }

            
            Vector3 prevNormal = GetBehindNormal();
            if(prevNormal.magnitude <= 1){
                GetDirection(prevNormal);
                lastSurface = prevNormal;
                return;
            }
            moveDirectionForward = transform.forward;
            moveDirectionRight = transform.right;
            ApplyGravity(-Vector3.up);
        }
    }

    private Vector3 GetBehindNormal(){
        // Debug.Log("GET BEHIND NORMAL");
        RaycastHit raycastHit;

        if(Physics.Raycast(transform.position - (lastDir * sphereRadius), -lastSurface, out raycastHit, sphereRadius + 0.1f, groundMask, QueryTriggerInteraction.Ignore)){
            lastCornerPoint = raycastHit.point;
            return raycastHit.normal;
        }
        // Corner Troundle Detect

        newSurfacePoint = lastCornerPoint + -lastSurface * newNormalPointDst;
        cornerState = true;
        Vector3 newNormal = GetSurfaceNormal(newSurfacePoint, newNormalRayDistance);

        if(DebugCornerDetection){
            Debug.Log(Vector3.Distance(transform.position, lastCornerPoint));
            Debug.Break();
        }
        
        if(Vector3.Dot(transform.forward, newNormal) < 0){
            return lastSurface;
        }
        
        return newNormal;
    }

    private Vector3 GetSurfaceNormal(Vector3 point, float rayDst){
        RaycastHit raycastHit;
        if(Physics.Raycast(transform.position, (point - transform.position).normalized, out raycastHit, rayDst, groundMask, QueryTriggerInteraction.Ignore)){
            
            return raycastHit.normal;
        }
        return Vector3.positiveInfinity;
    }

    private void GetDirection(Vector3 surfaceNormal){
        moveDirectionForward = Vector3.ProjectOnPlane(transform.forward, surfaceNormal);
        moveDirectionRight = Vector3.ProjectOnPlane(transform.right, surfaceNormal);
    }

    void OnDrawGizmos(){
        // Gizmos.color = Color.blue;
        // Gizmos.DrawSphere(lastCornerPoint, 0.1f);

        // Gizmos.color = Color.red;
        // Gizmos.DrawSphere(newSurfacePoint, 0.1f);
    }
}
