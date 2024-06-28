using System.Security.Cryptography;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector2 inputDirs = Vector2.zero;
    private Vector3 moveDirectionForward;
    private Vector3 moveDirectionRight;
    private Rigidbody rb;
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
    private Vector3 lastCornerPoint = Vector3.zero;
    
    private Vector3 newSurfacePoint = Vector3.zero;
    public bool cornerState = false;
    public bool DebugCornerDetection = false;

    public float curveRayDst = 0.6f;
    public float furtherPointStep = 0.1f;

    [Header("Circular Movement")]
    public float maxSphereRadius = 0.6f;
    [SerializeField] private Vector3 lastSurfacePoint = Vector3.zero;
    public Vector3 circularPoint;
    public float circularRadius;
    [SerializeField] private bool yPositive = true;
    public bool circularState = false;
    [SerializeField] float xSpeedCoef;
    public SphereMovement sphereMovement;
    public float spherePointOffset = 0.1f;

    [Header("Jump")]
    public bool useGravity = true;
    public Vector3 jumpDir;
    public float jumpForce;
    public float extraJumpForce;
    public float flyForce;
    private int currentJumpCount = 0;
    public int maxJumps = 1;
    [Header("Gravity")]
    public CustomGravity customGravity;


    void Start()
    {
        groundMask = LayerMask.GetMask("Ground");
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Inputs();
        GetTouchPounts();
        if(Input.GetKeyDown(KeyCode.Space)){
            Jump();
        }
        CheckGravityState();
        FreeMovement();
    }

    void FixedUpdate(){
        
        // Movement();
        // CircularMovement();
        // CircularStateLogic();
    }

    private void Inputs(){
        inputDirs = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    private void Jump(){
        currentJumpCount++;
        if(useGravity && currentJumpCount > maxJumps) return;
        if(inputDirs.magnitude > 0){
            rb.AddForce((transform.forward * inputDirs.y * jumpDir.z + 
                        transform.up * jumpDir.y + 
                        transform.right * inputDirs.x) * 
                        (currentJumpCount <= 1 ? jumpForce : extraJumpForce), ForceMode.Impulse);
        }else{
            rb.AddForce((transform.forward * jumpDir.z + 
                        transform.up * jumpDir.y + 
                        transform.right * jumpDir.x) * 
                        (currentJumpCount <= 1 ? jumpForce : extraJumpForce), ForceMode.Impulse);
        }
        
    }

    private void ApplyGravity(Vector3 gravityDir){
        transform.Translate(gravityDir * gravity * Time.deltaTime, Space.World);
    }

    private void Movement(){
        if(useGravity) return;
        Vector3 forceDir = ((moveDirectionForward * inputDirs.y) + (moveDirectionRight * inputDirs.x)).normalized;
        lastDir = forceDir;
        transform.Translate(forceDir * speed * Time.deltaTime, Space.World);
    }

    private void FreeMovement(){
        if(!useGravity) return;
        Vector3 forceDir = ((transform.forward * inputDirs.y) + (transform.right * inputDirs.x)).normalized;
        forceDir.y = 0;
        rb.AddForce(forceDir * flyForce * Time.deltaTime, ForceMode.Force);
    }

    private void GetTouchPounts(){
        
        Collider[] cols = new Collider[10];
        int colNums = Physics.OverlapSphereNonAlloc(transform.position, sphereRadius, cols, groundMask, QueryTriggerInteraction.Ignore);
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
            circularState = false;
            lastSurfaceNormal = surfaceNormal;

            cornerState = false;
            GetDirection(surfaceNormal);
            Movement();
        }else{ 
            
            if(useGravity) return;

            // CornerStateLogic();

            // CircularStateLogic();

            SphereStateLogic();

        }
    }

    private void CheckGravityState(){
        if(Vector3.Distance(lastSurfacePoint, transform.position) >= maxSphereRadius){
            useGravity = true;
            customGravity.ApplyGravity(Time.deltaTime);
        }else if(useGravity){
            useGravity = false;
            customGravity.ResetGravityAccelerationTime();
            currentJumpCount = 0;
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

        if(circularState == false){
            yPositive = transform.position.y > circularPoint.y;
        }
        
        circularState = true;

        CircularMovement();
    }

    private bool SphereStateLogic(){

        circularRadius = Vector3.Distance(transform.position, lastSurfacePoint);

        sphereMovement.sphereRadius = circularRadius;

        sphereMovement.spherePosition = lastSurfacePoint;
        
        sphereMovement.speed = speed;

        sphereMovement.inputDirs = inputDirs;

        sphereMovement.UpdateMovement();

        return true;
    }

    private Vector3 GetBehindNormal(){
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
        moveDirectionForward = Vector3.ProjectOnPlane(transform.forward, surfaceNormal).normalized;
        moveDirectionRight = Vector3.ProjectOnPlane(transform.right, surfaceNormal).normalized;
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
        if(inputDirs.magnitude == 0) return;

        Vector3 sphereNormal = (transform.position - circularPoint).normalized;

        Vector3 moveDir;

        Vector3 projectDir = Vector3.up;

        // float xPos = transform.position.x - circularPoint.x;
        // float yPos = transform.position.y - circularPoint.y;
        // float zPos = transform.position.z - circularPoint.z;

        // if(yPos > xPos && yPos > zPos){
        //     projectDir = Vector3.up;
        // }else if(xPos > yPos && xPos > zPos){
        //     projectDir = Vector3.right;
        // }else if(zPos > yPos && zPos > xPos){
        //     projectDir = Vector3.forward;
        // }

        Vector3 sphereDirForward = Vector3.ProjectOnPlane(transform.forward, projectDir);
        Vector3 sphereDirRight = Vector3.ProjectOnPlane(transform.right, projectDir);

        // Vector3 sphereDirForward = Vector3.Cross(transform.forward, projectDir);
        // Vector3 sphereDirRight = Vector3.Cross(transform.right, projectDir);

        moveDir = (sphereDirForward * inputDirs.y + sphereDirRight * inputDirs.x);
        // moveDir = inputDirs;

        xSpeedCoef = Mathf.Abs(transform.position.y - circularPoint.y) / circularRadius;

        if(xSpeedCoef <= 0) xSpeedCoef = 0.1f;
        if(xSpeedCoef > 1) xSpeedCoef = 1;

        xSpeedCoef = 1;

        // XYZ movement

        //  * (yPositive? 1 : -1)

        float z = transform.position.z + (moveDir.z * xSpeedCoef *  speed * Time.fixedDeltaTime);

        float x = transform.position.x + (moveDir.x * xSpeedCoef * speed * Time.fixedDeltaTime);

        float y = circularPoint.y + ((yPositive? 1 : -1) * Mathf.Sqrt((circularRadius * circularRadius) - Mathf.Pow(x - circularPoint.x, 2) - Mathf.Pow(z - circularPoint.z, 2)));

        // Debug.Log($"Further XYZ : {x}, {y}, {z}");

        if(float.IsNaN(y) || (Mathf.Abs(y) > circularRadius + circularPoint.y)){
            yPositive = !yPositive;
            // Debug.Log("switch y sphere state");
            return;
        }

        // loopIndex = 0;

        Vector3 pointOnSphere = new Vector3(x, y, z);

        transform.position = pointOnSphere;
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(lastSurfacePoint, 0.1f);
    }
}
