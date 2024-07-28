using UnityEngine;

public class DoorLock : MonoBehaviour {
    public string lockKeyPref;
    private Rigidbody rb;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        if (PlayerPrefs.GetInt(lockKeyPref) != 1) rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.layer == 11) {
            if (PlayerPrefs.GetInt(lockKeyPref) != 1)
                rb.constraints = RigidbodyConstraints.FreezeAll;
            else
                rb.constraints = RigidbodyConstraints.None;
        }
    }
}