using UnityEngine;

public class FakeChildObject : MonoBehaviour {
    public GameObject tracker;

    private void Update() {
        transform.position = tracker.transform.position;
    }
}