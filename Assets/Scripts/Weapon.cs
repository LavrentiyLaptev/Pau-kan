using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Projectile projectile;
    public EnemyList enemyList;
    public Transform firePoint;
    public Transform origin;
    public Transform playerTransform;
    public Transform testTarget;
    public float aimingSpeed;
    public float aimDistance = 10;
    public float timeBtwShots = 1f;
    private float t = 0;
    public float maxAimingAngle;
    private Vector3 targetPosition = Vector3.positiveInfinity;
    private Vector3 direction;
    private int enemyLayer = 0;
    private LayerMask rayToEnemyMask;
    private bool aiming = false;

    void Start(){
        origin.parent = transform.parent;
        rayToEnemyMask = LayerMask.GetMask("Ground", "Enemy");
        enemyLayer = LayerMask.NameToLayer("Enemy");
    }

    void Update(){
        if(Input.GetMouseButtonDown(0) && aiming){
            Shoot();
        }
    }
    void FixedUpdate(){
        AimingLogic();
        if(t >= timeBtwShots){
            Shoot();
        }
        t += Time.fixedDeltaTime;
    }

    private void Shoot(){
        if(!aiming) return;
        t = 0;
        Projectile newProjectile = Instantiate(projectile, firePoint.position, firePoint.rotation);
        newProjectile.Init(testTarget.position);
    }

    private void GetTarget(){
        float minDst = float.PositiveInfinity;
        Vector3 closestTarget = Vector3.positiveInfinity;
        foreach (var enemy in enemyList.enemysTransforms)
        {   
            float distance = Vector3.Distance(transform.position, enemy.position);
            if(distance < minDst){
                minDst = distance;
                closestTarget = enemy.position;
            }
        }

        targetPosition = closestTarget;
    }

    private void AimingLogic(){
        direction = (testTarget.position - origin.position).normalized;
        float angle = Vector3.Angle(origin.forward, direction);
        if(angle <= maxAimingAngle && CheckRay(testTarget.position)){
            aiming = true;
            Aiming(direction);
        }else{
            aiming = false;
            Aiming(playerTransform.forward);
        }
    }

    private void Aiming(Vector3 dir){
        transform.forward = Vector3.Lerp(transform.forward, dir, aimingSpeed * Time.fixedDeltaTime);
    }

    private bool CheckRay(Vector3 targetPos){
        RaycastHit hit;
        if(Physics.Raycast(origin.position, (targetPos - origin.position).normalized,  out hit, aimDistance, rayToEnemyMask, QueryTriggerInteraction.Ignore)){
            if(hit.transform.gameObject.layer == enemyLayer){
                return true;
            }
        }
        return false;
    }
}
