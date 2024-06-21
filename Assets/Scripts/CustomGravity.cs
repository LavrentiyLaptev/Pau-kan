using UnityEngine;

public class CustomGravity : MonoBehaviour
{
    public float gravityChangeSpeed;
    public float force;
    public Vector3 smoothDirection;
    public Vector3 gravityDirection; 
    public Rigidbody rb;

    void Start(){
        rb.useGravity = false;
    }

    void FixedUpdate(){
        smoothDirection = Vector3.Lerp(smoothDirection, gravityDirection, gravityChangeSpeed * Time.fixedDeltaTime);
        rb.AddForce(smoothDirection * force * Time.fixedDeltaTime, ForceMode.Impulse);
    }
}
