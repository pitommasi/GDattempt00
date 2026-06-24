using UnityEngine;

public class InputDebugger : MonoBehaviour {
    private void Update() {
        if (Input.GetKeyDown(KeyCode.A))
            Debug.Log("A pressed");

        if (Input.GetKeyDown(KeyCode.D))
            Debug.Log("D pressed");

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            Debug.Log("Left Arrow pressed");

        if (Input.GetKeyDown(KeyCode.RightArrow))
            Debug.Log("Right Arrow pressed");

        if (Input.GetKeyDown(KeyCode.Space))
            Debug.Log("Space pressed");

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) ||
            Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)) {
            Debug.Log("Horizontal key is being held");
        }
    }
}