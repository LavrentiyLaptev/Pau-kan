using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyList : MonoBehaviour
{
    public static EnemyList Instance{get; private set;}
    public List<Transform> enemysTransforms = new List<Transform>();

    void Awake(){
        if(Instance != null && Instance != this){
            Destroy(this);
        }else{
            Instance = this;
        }
    }

    public void AddEnemy(Transform enemyTransform){
        // for (int i = 0; i < enemysTransforms.Count; i++)
        // {
        //     if(enemysTransforms[i] == null){
        //         enemysTransforms[i] = enemyTransform;
        //         return;
        //     }
        // }

        // enemysTransforms.Add(enemyTransform);

        enemysTransforms.Add(enemyTransform);
    }

    public void RemoveEnemy(Transform enemyTransform){
        // for (int i = 0; i < enemysTransforms.Count; i++)
        // {
        //     if(enemysTransforms[i] == enemyTransform){
        //         enemysTransforms[i] = null;
        //         return;
        //     }
        // }

        enemysTransforms.Remove(enemyTransform);
    }
}
