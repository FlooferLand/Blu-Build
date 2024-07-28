using UnityEngine;
using UnityEngine.UI;

public class BarMove : MonoBehaviour {
    public bool transition;
    public float startY = 0.5f;
    public float endY = 0f;
    public float randomSpeedMin;
    public float randomSpeedMax;
    private RawImage rect;
    private float speed;
    private bool transitionOld;

    private void Start() {
        rect = GetComponent<RawImage>();
    }

    // Update is called once per frame
    private void FixedUpdate() {
        if (transitionOld != transition) {
            transitionOld = transition;
            speed = Random.Range(randomSpeedMin, randomSpeedMax);
        }

        if (transition && rect.uvRect.y > endY)
            rect.uvRect = new Rect(0, rect.uvRect.y - speed, rect.uvRect.width, rect.uvRect.height);
        else if (!transition && rect.uvRect.y < startY)
            rect.uvRect = new Rect(0, rect.uvRect.y + speed, rect.uvRect.width, rect.uvRect.height);
    }
}