using UnityEngine;

public class ExitApp : MonoBehaviour {
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
            print("Quit app");
        }
    }
}