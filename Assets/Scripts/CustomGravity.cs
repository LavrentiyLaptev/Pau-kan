using UnityEngine;

public class CustomGravity : MonoBehaviour
{
    public AnimationCurve gravityCurve;
    public float gravityForce = 9.81f;
    public float maxGravityTime;
    [SerializeField] private float currentGravityTime;
    private Rigidbody rb;

    void Start(){
        rb = GetComponent<Rigidbody>();
    }

    public void ApplyGravity(float deltaTime){
        currentGravityTime += deltaTime;
        if(currentGravityTime > maxGravityTime){
            currentGravityTime = maxGravityTime;
        }
        rb.AddForce(-Vector3.up * gravityForce * gravityCurve.Evaluate(currentGravityTime/maxGravityTime) * maxGravityTime * deltaTime);
    }

    public void ResetGravityAccelerationTime(){
        currentGravityTime = 0;
    }
}
