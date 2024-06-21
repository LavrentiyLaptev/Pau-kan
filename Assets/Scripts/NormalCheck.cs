using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalCheck : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F)){
            GetNormal();
        }
    }

    private void GetNormal(){
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, Physics.AllLayers, QueryTriggerInteraction.Ignore)){
            Debug.Log(hit.normal);
        }
    }
}
