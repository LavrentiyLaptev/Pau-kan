using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClonePair : Ability
{
    public WeaponManager weaponManager;
    public override void Activate()
    {
        base.Activate();

        weaponManager.CloneWeapon(0, 4);
        weaponManager.CloneWeapon(1, 5);
    }

    public override void Deactivate()
    {
        base.Deactivate();

        weaponManager.RemoveWeapon(4);
        weaponManager.RemoveWeapon(5);
    }
}
