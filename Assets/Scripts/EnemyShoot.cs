using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    public EnemyProjectile projectile;
    public Transform firePoint;
    public float shootDistance;
    public float damage;
    public float attackTime;
    private float time;
    public float rotationSpeed;
    public Transform player;


    private void Start(){

    }

    void FixedUpdate(){
        if(player != null){
            AttackTimer();
            LookAtTarget();
        }
    }

    private void AttackTimer(){
        time += Time.fixedDeltaTime;
        if(time >= attackTime && Vector3.Distance(transform.position, player.position) <= shootDistance){
            Shoot();
        }
    }

    private void Shoot(){
        time = 0;

        EnemyProjectile enemyProjectile = Instantiate(projectile);

        enemyProjectile.transform.position = firePoint.position;
        enemyProjectile.transform.rotation = firePoint.rotation;
        
        enemyProjectile.projectileDamage = damage;

        enemyProjectile.Init(player.position);
    }

    private void LookAtTarget(){
        Vector3 lookVector = (player.position - transform.position).normalized;
        transform.forward = Vector3.Lerp(transform.forward, lookVector, Time.fixedDeltaTime * rotationSpeed);
    }
}
