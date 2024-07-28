using UnityEngine;

public class swingsign : MonoBehaviour {
    public float maxMin;
    private float speed;

    private void Start() {
        transform.rotation = Quaternion.Euler(0, Random.RandomRange(0.0f, 180.0f), 0);
    }

    private void Update() {
        speed += Random.RandomRange(-maxMin / 5.0f, maxMin / 5.0f) * Time.deltaTime;
        speed = Mathf.Clamp(speed, -maxMin, maxMin);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + speed, 0);
    }
}