using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderVision : Ability
{
    public Camera cameraMain;
    public int defaultFov = 60;
    public int abilityFov = 90;
    public float changeFovTime = 1f;
    private Coroutine transitionCoroutine;

    public override void Activate()
    {
        base.Activate();

        FovTransition(abilityFov);

    }

    public override void Deactivate()
    {
        base.Deactivate();

        FovTransition(defaultFov);
    }

    private void FovTransition(float to){

        if(transitionCoroutine != null){
            StopCoroutine(transitionCoroutine);
        }
        transitionCoroutine = StartCoroutine(SmoothChangeFov(to));

    }

    private IEnumerator SmoothChangeFov(float to){
        float from = cameraMain.fieldOfView;

        if(from < to){

            float valueInSecond = to - from;

            while(cameraMain.fieldOfView < to){
                cameraMain.fieldOfView += valueInSecond * changeFovTime * Time.deltaTime;
                yield return new WaitForEndOfFrame(); 
            }

            cameraMain.fieldOfView = to;

        }else{

            float valueInSecond = from - to;

            while(cameraMain.fieldOfView > to){
                cameraMain.fieldOfView -= valueInSecond * changeFovTime * Time.deltaTime;
                yield return new WaitForEndOfFrame(); 
            }

            cameraMain.fieldOfView = to;
        }
    }
}
