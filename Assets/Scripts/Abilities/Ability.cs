using System.Collections;
using UnityEngine;

public class Ability : MonoBehaviour
{
    public AbilityType type;
    public AllAbilities allAbilities;
    private float currentDuration;
    public float duration = 30;
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

        while(currentDuration < duration){
            yield return new WaitForFixedUpdate();
            currentDuration += Time.fixedDeltaTime;
            imageAmountController.SetAmount(currentDuration, duration);
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
