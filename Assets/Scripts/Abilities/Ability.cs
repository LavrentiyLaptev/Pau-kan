using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    public AbilityType type;
    public AllAbilities allAbilities;
    public float currentDuration;
    public float maxDuration;
    public ImageAmountController imageAmountController;
    protected Coroutine durationCoroutine;

    public virtual void Activate(){
        if(durationCoroutine != null){
            StopCoroutine(durationCoroutine);
        }

        durationCoroutine = StartCoroutine(AbilityDuration());
    }

    public virtual void Deactivate(){
        
    }

    protected virtual IEnumerator AbilityDuration(){

        imageAmountController.gameObject.SetActive(true);

        currentDuration = 0;

        while(currentDuration < maxDuration){
            yield return new WaitForFixedUpdate();
            currentDuration += Time.fixedDeltaTime;
            imageAmountController.SetAmount(currentDuration, maxDuration);
        }

        imageAmountController.gameObject.SetActive(false);
        
        Deactivate();
    }
}

public enum AbilityType{
    Wings,
    ExtraHands,
    SpiderVision,
    ChitinousShell,
    Minions,
    Dash,
    SpiderWeb,
}
