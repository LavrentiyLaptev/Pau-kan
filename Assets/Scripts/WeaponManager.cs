using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public Transform playerTransform;
    public Transform testTarget;
    public Weapon testWeapon;
    public Transform[] weaponHolders = new Transform[4];
    public Weapon[] weapons = new Weapon[4];

    void Update(){
        if(Input.GetKeyDown(KeyCode.Q)){
            SetUpWeapon(testWeapon);
        }
    }

    public void SetUpWeapon(Weapon weapon){
        int freeIndex = GetFreeWeaponIndex();
        if(freeIndex == int.MinValue || weaponHolders[freeIndex] == null) return;
        Transform holder = weaponHolders[freeIndex];
        Weapon newWeapon = Instantiate(weapon, holder);
        weapons[freeIndex] = newWeapon;
        // newWeapon.transform.localPosition = Vector3.zero;
        // newWeapon.transform.forward = Vector3.forward;
        newWeapon.playerTransform = playerTransform;
        newWeapon.testTarget = testTarget;
    }

    private int GetFreeWeaponIndex(){
        int indexToReturn = int.MinValue;

        for (int i = 0; i < weapons.Length; i++)
        {
            if(weapons[i] == null){
                return i;
            }
        }

        return indexToReturn;
    }
}
