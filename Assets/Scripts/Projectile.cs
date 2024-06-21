using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody rb;
    public float speed;
    
    public void Init(Vector3 target)
    {
        Destroy(gameObject, 5);
        Vector3 direction = (target - transform.position).normalized;
        transform.forward = direction;

        if(rb == null){
            rb = GetComponent<Rigidbody>();
        }

        rb.AddForce(direction * speed, ForceMode.Impulse);
    }
}
