using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Ability
{
    public PlayerHealth playerHealth;

    public override void Activate()
    {
        base.Activate();
        playerHealth.SetImmuneState(true);
    }

    public override void Deactivate()
    {
        base.Deactivate();
        playerHealth.SetImmuneState(false);
    }
}
