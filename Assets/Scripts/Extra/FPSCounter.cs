using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour {
    private Text currentText;
    private float FPS = 0;

    private void Start() {
        currentText = GetComponent<Text>();
    }

    private void Update() {
        FPS += (Time.deltaTime / Time.timeScale - FPS) * 0.03f;
        if (QualitySettings.vSyncCount == 1)
            currentText.text = Mathf.Min((int)(1.0f / FPS), 60) + "FPS";
        else
            currentText.text = (int)(1.0f / FPS) + "FPS";
    }
}