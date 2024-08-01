using UnityEngine;

public class Fade : MonoBehaviour {
    public byte fadeTo;
    public byte fadeSpeed;
    private byte fade = 255;
    private CanvasGroup group;

    private void Awake() {
        group = GetComponent<CanvasGroup>();
    }

    private void FixedUpdate() {
        for (int i = 0; i < fadeSpeed; i++) {
            if (fade < fadeTo)
                fade++;
            else if (fade > fadeTo)
                fade--;
            else
                break;
            group.alpha = fade / 255.0f;
        }
    }
}