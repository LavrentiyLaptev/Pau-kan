using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bar : MonoBehaviour
{
    private float xMaxSize;
    private RectTransform rectTransform;

    void Start(){
        rectTransform = GetComponent<RectTransform>();
        xMaxSize = rectTransform.sizeDelta.x;
    }

    public void SetBarValue(float value, float maxValue){
        float x = value/maxValue * xMaxSize;
        if(x < 0) x = 0;
        rectTransform.sizeDelta = new Vector2(x, rectTransform.sizeDelta.y);
    }
}
