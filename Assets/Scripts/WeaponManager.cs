using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    private EnemyList enemyList;
    public Transform playerTransform;
    public Transform currentTarget;
    public Weapon testWeapon;
    public Transform[] weaponHolders = new Transform[4];
    public Weapon[] weapons = new Weapon[4];
    public float searchTargetInterval = 1f;

    void Start(){
        enemyList = EnemyList.Instance;
        StartCoroutine(TargetLooker());
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Q)){
            SetUpWeapon(testWeapon, GetFreeWeaponIndex());
        }
        // if(Input.GetKeyDown(KeyCode.F)){
        //     CloneWeapon(0, 4);
        //     CloneWeapon(1, 5);
        // }
    }


    public void SetUpWeapon(Weapon weapon, int index){
        if(index == int.MinValue || weaponHolders[index] == null) return;
        Transform holder = weaponHolders[index];
        Weapon newWeapon = Instantiate(weapon, holder);
        weapons[index] = newWeapon;
        newWeapon.playerTransform = playerTransform;
        newWeapon.currentTarget = currentTarget;
    }

    private int GetFreeWeaponIndex(){
        
        int indexToReturn = int.MinValue;

        for (int i = 0; i < 4; i++)
        {
            if(weapons[i] == null){
                return i;
            }
        }

        return indexToReturn;
    }

    public void CloneWeapon(int cloneIndex, int holderIndex){

        if(weapons[cloneIndex] != null){

            if(weapons[holderIndex] != null){
                RemoveWeapon(holderIndex);
            }

            SetUpWeapon(weapons[cloneIndex], holderIndex);
        }
    }

    public void RemoveWeapon(int index){
        if(weapons[index] != null){
            Destroy(weapons[index].gameObject);
        }
    }

    private void GetTarget(){
        float minAngle = float.PositiveInfinity;
        Transform closestTarget = null;
        foreach (var enemy in enemyList.enemysTransforms)
        {   

            Vector3 dir = enemy.position - transform.position;
            float angle = Vector3.Angle(transform.forward, dir);
            
            if(angle < minAngle){
                minAngle = angle;
                closestTarget = enemy;
            }
        }
        currentTarget = closestTarget;
        SetWeaponsTarget(currentTarget);
    }

    private void SetWeaponsTarget(Transform newTarget){
        foreach (var weapon in weapons)
        {
            if(weapon == null) continue;
            weapon.currentTarget = newTarget;
        }
    }

    private IEnumerator TargetLooker(){
        while(enabled){
            GetTarget();
            yield return new WaitForSeconds(searchTargetInterval);
        }
    }
}
