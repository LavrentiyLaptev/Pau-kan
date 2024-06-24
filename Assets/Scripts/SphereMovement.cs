using UnityEngine;

public class SphereMovement : MonoBehaviour
{
    public Vector3 spherePosition;
    public float sphereRadius = 5.0f;
    public float speed = 1.0f;
    public Vector2 inputDirs = Vector2.zero;
    public bool testSphereMovement = false;

    void Update(){
        if(testSphereMovement){
            inputDirs = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            UpdateMovement();
        }
    }

    public void UpdateMovement(){

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        // forward.y = 0;
        // right.y = 0;

        Vector3 moveDirection = (forward * inputDirs.y + right * inputDirs.x).normalized;
        Vector3 currentPosition = transform.position - spherePosition;

        Vector3 newPosition = currentPosition + moveDirection * speed * Time.deltaTime;
        newPosition = spherePosition + newPosition.normalized * sphereRadius;

        transform.position = newPosition;
    }
}
