using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : Projectile
{
    public override void TargetDetecting(GameObject target)
    {

        // base.TargetDetecting(target);
        if(target.CompareTag("Enemy")){
            Debug.Log(name + " hit the target ");
            Enemy enemy = target.GetComponent<Enemy>();
            enemy.TakeDamage(projectileDamage);
        }
        Destroy(gameObject);
    }
}
