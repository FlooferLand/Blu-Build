using UnityEngine;

public class TriggerButton : MonoBehaviour {
    public LayerMask hitMask;
    public string attemptString;
    private TutorialManager gc;
    private bool waitwaitwait;

    private void Awake() {
        gc = GameObject.Find("Tutorial").GetComponent<TutorialManager>();
    }

    private void FixedUpdate() {
        if (waitwaitwait)
            if (gc.AttemptAdvanceTutorial(attemptString))
                Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        if (hitMask == (hitMask | (1 << other.gameObject.layer))) {
            if (gc.AttemptAdvanceTutorial(attemptString))
                Destroy(gameObject);
            else
                waitwaitwait = true;
        }
    }
}