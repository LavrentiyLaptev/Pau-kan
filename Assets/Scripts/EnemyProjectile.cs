using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyProjectile : Projectile
{
    public override void TargetDetecting(GameObject target)
    {
        // base.TargetDetecting(target);
        if(target.CompareTag("Player")){
            PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
            playerHealth.TakeDamage(projectileDamage);
        }
        Destroy(gameObject);
    }
}
