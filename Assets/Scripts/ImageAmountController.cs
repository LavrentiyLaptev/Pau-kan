using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageAmountController : MonoBehaviour
{
    public Image amountImage;

    public void SetAmount(float current, float max){
        amountImage.fillAmount = 1 - current/max;
    }
}
