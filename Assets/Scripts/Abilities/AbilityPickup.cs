using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityPickup : MonoBehaviour
{
    public AbilityType abilityType;
    void OnTriggerEnter(Collider col){
        if(col.CompareTag("Player"))
        {
            ActivateAbilityFromPlayer(col.gameObject);
        }
    }

    private void ActivateAbilityFromPlayer(GameObject playerObject){
        Ability[] abilities = playerObject.transform.Find("PlayerAbilities").GetComponents<Ability>();

        foreach (var ability in abilities)
        {
            if(ability.type == abilityType){
                ability.Activate();
                // gameObject.SetActive(false);
                return;
            }
        }
    }
}
