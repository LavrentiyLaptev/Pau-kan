using UnityEngine;

public class CustomGravity : MonoBehaviour
{
    public AnimationCurve gravityCurve;
    public float gravityForce = 9.81f;
    public float simpleGravityForce = 9.81f;
    public float maxGravityTime;
    [SerializeField] private float currentGravityTime;
    private Rigidbody rb;
    public AllAbilities allAbilities;

    void Start(){
        rb = GetComponent<Rigidbody>();
    }

    public void ApplyGravity(float deltaTime){
        if(allAbilities.isSoar && Input.GetKey(KeyCode.Space)){
            SimpleGravity(deltaTime);
        }else{
            GravitationalAcceleration(deltaTime);
        }
    }

    private void GravitationalAcceleration(float deltaTime){
        currentGravityTime += deltaTime;
        if(currentGravityTime > maxGravityTime){
            currentGravityTime = maxGravityTime;
        }

        rb.AddForce(-Vector3.up * gravityForce * gravityCurve.Evaluate(currentGravityTime/maxGravityTime) * maxGravityTime * deltaTime);
    }

    private void SimpleGravity(float deltaTime){
        rb.AddForce(-Vector3.up * simpleGravityForce * deltaTime, ForceMode.VelocityChange);
    }

    public void ResetGravityAccelerationTime(){
        currentGravityTime = 0;
    }
}
