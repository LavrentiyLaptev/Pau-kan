using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ProceduralSpider : MonoBehaviour
{
    public float heightSpeed;
    public float rotationSpeed;
    public List<ProceduralLeg> legs = new List<ProceduralLeg>();
    public float baseYOffset;
    public float stepsInterval;
    private int stepIndex = 0;
    private Vector3 prevPos;
    public float dstBtwSteps;
    float a = 0;
    float b = 0;
    private Vector3 p1 = Vector3.zero;
    private Vector3 p2 = Vector3.zero;
    void Start(){

    }
    void FixedUpdate(){
        ControlHeight();
        ControlAngle();
        if(Vector3.Distance(transform.position, prevPos) >= dstBtwSteps){
            prevPos = transform.position;
            ProceduralSteps();
            stepIndex++;
            if(stepIndex > 3){
                stepIndex = 0;
            }
        }
    }

    private void ControlHeight(){
        int j = 0;
        float summaryHeight = 0f;
        foreach (var leg in legs)
        {
            if(leg.isStick){
                j++;
                summaryHeight += leg.target.position.y;
            }
        }
        if(j == 0) return;
        float avgHeight = summaryHeight/j;
        Vector3 avgPos = new Vector3(transform.position.x, baseYOffset + avgHeight, transform.position.z);
        // Vector3 avgPos = transform.up + new Vector3(0, baseYOffset + avgHeight, 0);
        transform.position = Vector3.Lerp(transform.position, avgPos, heightSpeed * Time.fixedDeltaTime);
    }

    private void ControlAngle(){
        int j = 0;
        float totalX = 0;
        float totalY = 0;
        foreach (var leg in legs)
        {
            if(!leg.isStick) continue;
            j++;
            totalX += leg.target.position.z;
            totalY += leg.target.position.y;
        }
        float avgX = totalX/j;
        float avgY = totalY/j;

        a = 0;
        b = 0;

        float sum1 = 0;
        float sum2 = 0;
        j = 0;
        for (int i = 0; i < legs.Count; i++)
        {
            if(!legs[i].isStick) continue;
            j++;
            sum1 += (legs[i].target.position.z - avgX) * (legs[i].target.position.y - avgY);
            sum2 += Mathf.Pow(legs[i].target.position.z - avgX, 2);
        }
        a = sum1 / sum2;
        b = avgY - a * avgX;

        p2 = new Vector3(0, a*-1 + b, -1);
        p1 = new Vector3(0, a*1 + b, 1);

        Vector3 dir = (p1 - p2).normalized;
        // float angle = Vector3.Angle(dir, Vector3.forward);
        Quaternion rotation = Quaternion.FromToRotation(dir, Vector3.forward);
        rotation.x = -rotation.x;
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.fixedDeltaTime);
    }

    private void ProceduralSteps(){
        if(stepIndex == 0){
            legs[0].FindNewPositon();
            legs[5].FindNewPositon();
        }else if(stepIndex == 1){
            legs[1].FindNewPositon();
            legs[4].FindNewPositon();
        }else if(stepIndex == 2){
            legs[3].FindNewPositon();
            legs[6].FindNewPositon();
        }else if(stepIndex == 3){
            legs[2].FindNewPositon();
            legs[7].FindNewPositon();
        }
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(p1 + transform.position, p2 + transform.position);
    }
}
