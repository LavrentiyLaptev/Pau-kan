using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHealth;
    private float health;
    public ParticleSystem hitEffect;
    private Vector3 respawnPoint;

    void Start(){
        respawnPoint = transform.position;
        health = maxHealth;

        EnemyList.Instance.AddEnemy(transform);
    }

    public void TakeDamage(float damage){
        if(damage <= 0) return;
        
        health -= damage;
        Debug.Log(health);

        if(health <= 0){
            Death();
        }

        hitEffect.Play();
    }

    private void Death(){
        // EnemyList.Instance.RemoveEnemy(transform);
        transform.position = respawnPoint;
        health = maxHealth;
        // Destroy(gameObject);

    }
}
