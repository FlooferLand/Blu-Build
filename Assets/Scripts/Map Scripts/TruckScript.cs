using UnityEngine;

public class TruckScript : MonoBehaviour {
    private Vector3 old;

    private void Start() {
        old = transform.position;
    }

    // Update is called once per frame
    private void Update() {
        float newtime = Mathf.Min(Time.deltaTime, 0.03f);
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x + Random.Range(-0.01f, 0.01f) * newtime * 60, old.x - 0.5f,
                old.x + 0.5f), transform.position.y,
            Mathf.Clamp(transform.position.z + Random.Range(-0.01f, 0.01f) * newtime * 60, old.z - 0.5f,
                old.z + 0.5f));
    }
}