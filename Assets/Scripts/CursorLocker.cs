using UnityEngine;

public class CursorLocker : MonoBehaviour
{
    void Update(){
        if(Input.GetKeyDown(KeyCode.Tab)){
            SwitchCursorState();
        }
    }

    private void SwitchCursorState(){
        if(Cursor.visible){
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }else{
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
