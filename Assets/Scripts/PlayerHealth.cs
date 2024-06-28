using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public Bar healthBar;
    public float maxHealth = 100;
    private float health;
    private bool damageImmune = false;

    void Start(){
        health = maxHealth;
    }

    public void TakeDamage(float damage){
        if(damage <= 0 || damageImmune) return;

        health -= damage;

        healthBar.SetBarValue(health, maxHealth);

        if(health <= 0){
            Death();
        }
    }

    private void Death(){
        health = maxHealth;
        // gameObject.SetActive(false);
    }

    public void SetImmuneState(bool state){
        damageImmune = state;
    }
}
