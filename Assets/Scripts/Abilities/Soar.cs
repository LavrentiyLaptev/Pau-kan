using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soar : Ability
{
    public PlayerController playerController;
    public int activeJumpCount = 2;

    void Update(){
        if(Input.GetKeyDown(KeyCode.Alpha1)){
            Activate();
        }
    }

    public override void Activate()
    {
        
        allAbilities.isSoar = true;

        playerController.maxJumps = activeJumpCount;

        base.Activate();
    }

    public override void Deactivate()
    {
        allAbilities.isSoar = false;

        playerController.maxJumps = 1;

        base.Deactivate();
    }
}
