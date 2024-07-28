using UnityEngine;
using UnityEngine.UI;

public class Static : MonoBehaviour {
    public bool flash;
    public bool noAlpha = false;
    private float alpha;

    private RawImage staticUI;

    // Start is called before the first frame update
    private void Start() {
        staticUI = GetComponent<RawImage>();
        alpha = staticUI.color.a;
    }

    // Update is called once per frame
    private void Update() {
        staticUI.uvRect = new Rect(Random.Range(0, 0.55f), Random.Range(0, 0.64f), staticUI.uvRect.width,
            staticUI.uvRect.height);
    }

    private void FixedUpdate() {
        if (!noAlpha) {
            if (staticUI.color.a > alpha) {
                var currColor = staticUI.color;
                currColor.a -= .1f;
                staticUI.color = currColor;
            }

            if (flash) {
                var currColor = staticUI.color;
                currColor.a = 1;
                staticUI.color = currColor;
                flash = false;
            }
        }
    }
}