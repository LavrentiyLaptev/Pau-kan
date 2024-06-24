using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CustomGravity customGravity;
    public float speed;
    public float sphereRadius;
    public float jumpUpForce;
    public float jumpForwardForce;
    public float normalDotTreshHold;
    public float angleRayDistance = 10;
    public float forceTOSurface = 10;
    private Vector2 moveDirection = Vector2.zero;
    private Rigidbody rb;
    public bool loackDir = false;
    public Vector3 moveDirForward = Vector3.zero;
    public Vector3 moveDirRight = Vector3.zero;
    Vector3 touchPoint = Vector3.zero;
    private int groundMask = 0;
    private SphereCollider sphereCollider;
    [SerializeField] private Vector3 surfaceNormal = Vector3.zero;
    public bool isCrawl = false;

    // DEBUG FIELDS

    private Vector3[] cahsedAngleTouchs = new Vector3[2];

    void Start(){

        cahsedAngleTouchs[0] = Vector3.zero;
        cahsedAngleTouchs[1] = Vector3.zero;

        sphereCollider = GetComponent<SphereCollider>();
        groundMask = LayerMask.GetMask("Ground");
        rb = GetComponent<Rigidbody>();
    }

    void Update(){
        Inputs();
    }

    void FixedUpdate(){
        Movement();
        CheckTouchPoints();
        CheckEndOfSurface();
        // RaycastTouchPoints();
    }

    private void Inputs(){
        moveDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if(Input.GetKeyDown(KeyCode.Space) && !rb.useGravity){
            Jump();
        }
    }

    private void Movement(){
        Vector3 forceDir = new Vector3(moveDirection.x, 0, moveDirection.y).normalized;
        if(moveDirForward == Vector3.zero){
            // rb.AddRelativeForce(forceDir * Time.fixedDeltaTime * speed, ForceMode.Impulse);
            // rb.AddForce(transform.forward* moveDirection.y * speed * Time.fixedDeltaTime, ForceMode.Impulse);
            // rb.AddForce(transform.right * moveDirection.x * Time.fixedDeltaTime * speed, ForceMode.Impulse);
        }else{
            forceDir = ((moveDirForward * moveDirection.y) + (moveDirRight * moveDirection.x)).normalized;
            // rb.AddForce(moveDirForward * moveDirection.y * speed * Time.fixedDeltaTime, ForceMode.Impulse);
            // rb.AddForce(moveDirRight * moveDirection.x * speed * Time.fixedDeltaTime, ForceMode.Impulse);
            // rb.AddForce(forceDir * speed * Time.fixedDeltaTime, ForceMode.Impulse);
            transform.Translate(forceDir * speed * Time.fixedDeltaTime, Space.World);
        }
    }

    private void Jump(){
        Debug.Log("Jump");
        rb.AddRelativeForce(0, jumpUpForce, jumpForwardForce, ForceMode.Impulse);
    }

    private void CheckTouchPoints(){
        int maxColliders = 10;
        Collider[] cols = new Collider[maxColliders];
        int collnums = Physics.OverlapSphereNonAlloc(transform.position, sphereRadius, cols, groundMask, QueryTriggerInteraction.Ignore);
        
        if(!customGravity.enabled){

            if(cols.Length > 0 && cols[0] != null){
                rb.useGravity = false;
            }else{
                rb.useGravity = true;
            }

        }

        float minAngle = float.MaxValue;
        Vector3 closestPoint = Vector3.positiveInfinity;

        foreach (var col in cols)
        {

            if(col == null) continue;
            Vector3 contactPoint = Vector3.positiveInfinity;
            if(col is MeshCollider mCol && !mCol.convex){
                contactPoint = GetClosestPointOnMeshCollider(mCol);
            }else{
                contactPoint = col.ClosestPoint(transform.position);
            }
            
            float angle = Vector3.Angle(transform.forward, (contactPoint - transform.position).normalized);
            Debug.Log(angle);
            if(angle < minAngle){
                minAngle = angle;
                closestPoint = contactPoint;
                touchPoint = contactPoint;
            }
        }
        
        if(closestPoint != Vector3.positiveInfinity){
            SetNormalBasedDirection(closestPoint);
        }
    }

    private void RaycastTouchPoints(){
        RaycastHit[] hits = Physics.SphereCastAll(transform.position  + transform.up * sphereRadius, sphereRadius, -transform.up, 0.1f, groundMask, QueryTriggerInteraction.Ignore);

        if(hits.Length > 0){
            // rb.useGravity = false;
        }else{
            // rb.useGravity = true;
            moveDirForward = Vector3.zero;
        }

        float minAngle = float.MaxValue;
        Vector3 closestNormal = Vector3.positiveInfinity;

        foreach (var hit in hits)
        {
            float angle = Vector3.Angle(transform.forward, hit.normal);
            if(angle < minAngle){
                minAngle = angle;
                closestNormal = hit.normal;
                touchPoint = hit.point;
            }
        }
        Debug.Log(minAngle);
        moveDirForward = Vector3.ProjectOnPlane(transform.forward, closestNormal).normalized;
        moveDirRight = Vector3.ProjectOnPlane(transform.right, closestNormal).normalized;

    }

    private Vector3 GetClosestPointOnMeshCollider(MeshCollider meshCollider){
        Mesh mesh = meshCollider.sharedMesh;
        float minDst = float.MaxValue;
        Vector3 closestPoint = Vector3.positiveInfinity;

        foreach (var vert in mesh.vertices)
        {
            Vector3 worldVert = meshCollider.transform.TransformPoint(vert);
            float distance = Vector3.Distance(transform.position, worldVert);
            if(distance < minDst){
                minDst = distance;
                closestPoint = worldVert;
            }
        }
        return closestPoint;
    }

    private void SetNormalBasedDirection(Vector3 groundPoint){
        RaycastHit hit;
        if(Physics.Raycast(transform.position, (groundPoint - transform.position).normalized,  out hit, sphereRadius + 0.01f, groundMask)){
            
            float dotProduct = Vector3.Dot(rb.velocity.normalized, hit.normal);
            if(dotProduct >= normalDotTreshHold){
                Debug.Log(dotProduct);
                // moveDirForward = RaycastDir();
                // customGravity.gravityDirection = -Vector3.up;
                return;
            } 
            surfaceNormal = hit.normal;
            moveDirForward = Vector3.ProjectOnPlane(transform.forward, hit.normal);
            moveDirRight = Vector3.ProjectOnPlane(transform.right, hit.normal);
            moveDirForward.Normalize();
            moveDirRight.Normalize();
            // customGravity.gravityDirection = -hit.normal.normalized;
        }else{
            // customGravity.gravityDirection = -Vector3.up;
            moveDirForward = Vector3.zero;
        }
    }

    private Vector3 RaycastDir(){
        Vector3 angleMoveDirection = Vector3.zero;
        if(rb.velocity.magnitude == 0) return angleMoveDirection;
        Vector3 direction = rb.velocity.normalized;
        Vector3 groundDirection = (touchPoint - transform.position).normalized;
        Vector3[] castPoints = new Vector3[2];
        castPoints[0] = transform.position - direction * sphereRadius;
        castPoints[1] = transform.position + direction * sphereRadius;

        Vector3[] resultPoints = new Vector3[2];
        resultPoints[0] = Vector3.positiveInfinity;
        resultPoints[1] = resultPoints[0];
        for (int i = 0; i < 2; i++)
        {
            RaycastHit hit;
            if(Physics.Raycast(castPoints[i], groundDirection, out hit, angleRayDistance, groundMask, QueryTriggerInteraction.Ignore)){
                resultPoints[i] = hit.point;
            }else{
                return angleMoveDirection;
            }
        }
        cahsedAngleTouchs[0] = resultPoints[0];
        cahsedAngleTouchs[1] = resultPoints[1];
        return angleMoveDirection = (resultPoints[0] - resultPoints[1]).normalized;

    }

    private void CheckEndOfSurface(){
        RaycastHit hit;
        if(Physics.Raycast(transform.position - moveDirForward * sphereRadius, -surfaceNormal, out hit, sphereRadius, groundMask, QueryTriggerInteraction.Ignore)){
            
        }else{
            moveDirForward = -surfaceNormal;

            // rb.AddForce(-surfaceNormal * forceTOSurface * Time.fixedDeltaTime);
            Debug.Log("END OF SURFACE DETECTED");
        }
    }

    void OnDrawGizmos(){
        // Gizmos.color = Color.blue;
        // Gizmos.DrawSphere(touchPoint, 0.1f);
    }
}
