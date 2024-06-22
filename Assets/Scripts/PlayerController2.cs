using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
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
    public float newNormalRayDistance;
    private int groundMask;
    public float newNormalPointDst = 0.1f;
    public float cornerStateMinDst = 0.6f;
    public float cornerStateMaxDst = 0.6f;
    public float cornerLimitDst = 1f;
    private Vector3 lastDir = Vector3.zero;
    public Vector3 lastSurfaceNormal = Vector3.zero;
    [SerializeField] private Vector3 lastCornerPoint = Vector3.zero;
    
    private Vector3 newSurfacePoint = Vector3.zero;
    public bool cornerState = false;
    public bool DebugCornerDetection = false;

    public float curveRayDst = 0.6f;
    public float furtherPointStep = 0.1f;

    [Header("Circular Movement")]
    private Vector3 lastSurfacePoint = Vector3.zero;
    public Vector3 circularPoint;
    public float circularRadius;
    [SerializeField] private bool yPositive = true;
    private int loopIndex = 0;
    public bool circularState = false;


    void Start()
    {
        groundMask = LayerMask.GetMask("Ground");
    }

    void Update()
    {
        Inputs();
    }

    void FixedUpdate(){
        // GetTouchPounts();
        // Movement();
        CircularMovement();
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
                lastSurfacePoint = touchPoints[i];
            }
        }

        // if(cornerState && Vector3.Distance(transform.position, lastCornerPoint) >= cornerLimitDst){ // for Corner state
        //     cornerState = false;
        // }

        if(surfaceNormal.magnitude <= 1){
            
            lastSurfaceNormal = surfaceNormal;
            
            // Debug.Log(surfaceNormal + " || " + lastSurface);
            cornerState = false;
            GetDirection(surfaceNormal);
            Movement();
        }else{ 

            // CornerStateLogic();

            CircularStateLogic();
        }
    }

    private void CornerStateLogic(){
            float cornerDistance = Vector3.Distance(transform.position, lastCornerPoint);
            if(cornerState && cornerDistance <= cornerStateMinDst){
                // Debug.Log("NON Gravity CORNER STATE");
                GetDirection(lastSurfaceNormal);
                return;
            }

            if(cornerState && cornerDistance >= cornerStateMaxDst){
                // Debug.Log("Gravity CORNER STATE");
                ApplyGravity(-lastSurfaceNormal);
                return;
            }

            
            Vector3 prevNormal = GetBehindNormal(); // Если нет никаких точек, продолжаем двигаться получая, нормаль рейкастом с края сферы.
            if(prevNormal.magnitude <= 1){
                GetDirection(prevNormal);
                lastSurfaceNormal = prevNormal;
                return;
            }

            moveDirectionForward = transform.forward;
            moveDirectionRight = transform.right;
            ApplyGravity(-Vector3.up);
    }

    private void CircularStateLogic(){

        circularPoint = lastSurfacePoint;
        float r = Vector3.Distance(transform.position, circularPoint);
        circularRadius = r;

        CircularMovement();
    }

    private Vector3 GetBehindNormal(){
        // Debug.Log("GET BEHIND NORMAL");
        RaycastHit raycastHit;

        if(Physics.Raycast(transform.position - (lastDir * sphereRadius), -lastSurfaceNormal, out raycastHit, sphereRadius + 0.5f, groundMask, QueryTriggerInteraction.Ignore)){
            lastCornerPoint = raycastHit.point; // Луч с противоположного траектории движения сферы конца
            return raycastHit.normal;
        }else{

            // Сфера находится на углу, находим новую поверхность
            // Corner Trouble Detect
            cornerState = true;

            newSurfacePoint = lastCornerPoint + -lastSurfaceNormal * newNormalPointDst; // Преположительная точка на новой поверхности.
            
            Vector3 newNormal = GetSurfaceNormal(newSurfacePoint, newNormalRayDistance);

            if(DebugCornerDetection){
                Debug.Log(Vector3.Distance(transform.position, lastCornerPoint));
                Debug.Break();
            }
            
            if(Vector3.Dot(transform.forward, newNormal) < 0){
                return lastSurfaceNormal;
            }
            
            return newNormal;
        }
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

    private bool IsFatherSurfaceCurve(){

        Vector3 furtherPoint = Vector3.zero;
        RaycastHit hit;
        if(Physics.Raycast(transform.position, -lastSurfaceNormal, out hit, sphereRadius + 0.01f, groundMask, QueryTriggerInteraction.Ignore)){
            furtherPoint = hit.point + lastDir * furtherPointStep;
        }else{
            return false;
        }
        Vector3 furtherNormal = GetSurfaceNormal(furtherPoint, curveRayDst);

        float angle = Vector3.Angle(lastSurfaceNormal, furtherNormal);
        Debug.Log(angle);

        return angle > 0.01f;
    }

    private void CircularMovement(){
        Debug.Log("Circular Movement");
        if(inputDirs.magnitude == 0) return;

        Vector3 sphereNormal = (transform.position - circularPoint).normalized;

        Vector3 moveDir;

        Vector3 sphereDirForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
        Vector3 sphereDirRight = Vector3.ProjectOnPlane(transform.right, Vector3.up);

        moveDir = (sphereDirForward * inputDirs.y + sphereDirRight * inputDirs.x).normalized;

        float xSpeedCoef = Mathf.Abs(transform.position.y - circularPoint.y) / circularRadius;

        if(xSpeedCoef <= 0) xSpeedCoef = 0.1f;
        if(xSpeedCoef > 1) xSpeedCoef = 1;

        Debug.Log(xSpeedCoef);

        // XYZ movement

        //  * (yPositive? 1 : -1)

        float z = transform.position.z + moveDir.z * (yPositive? 1 : -1) * speed * xSpeedCoef * Time.fixedDeltaTime;

        float x = transform.position.x + moveDir.x * (yPositive? 1 : -1) * speed * xSpeedCoef * Time.fixedDeltaTime;

        float y = (yPositive? 1 : -1) * Mathf.Sqrt((circularRadius * circularRadius) - (x * x) - (z * z));

        Debug.Log($"Further XYZ : {x + circularPoint.x}, {y}, {circularPoint.z}");
        if(float.IsNaN(y) || (Mathf.Abs(y) > circularRadius)){
            yPositive = !yPositive;
            Debug.Log("switch y sphere state");
            return;
        }

        loopIndex = 0;

        Vector3 pointOnSphere = new Vector3(x, y, z) + circularPoint;

        transform.position = pointOnSphere;
    }

    void OnDrawGizmos(){
        // Gizmos.color = Color.blue;
        // Gizmos.DrawSphere(lastCornerPoint, 0.1f);

        // Gizmos.color = Color.red;
        // Gizmos.DrawSphere(newSurfacePoint, 0.1f);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(lastSurfacePoint, 0.1f);
    }
}
