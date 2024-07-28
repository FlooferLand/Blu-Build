using UnityEngine;

public class DoorSound : MonoBehaviour {
    public float zRotationAwake;
    private HingeJoint hj;
    private AudioSource source;
    private bool stillShut = true;
    private float timeOpen;

    private void Awake() {
        hj = GetComponent<HingeJoint>();
        zRotationAwake = hj.angle;
        source = GetComponent<AudioSource>();
    }

    private void FixedUpdate() {
        if (!stillShut && timeOpen < Time.time - 1 && hj.angle < zRotationAwake + 2 && hj.angle > zRotationAwake - 2) {
            stillShut = true;

            int sound = Random.Range(0, 4);
            var clip = Resources.Load("DoorClose0" + sound) as AudioClip;
            source.clip = clip;
            source.pitch = Random.Range(0.98f, 1.02f);
            source.Play();
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (GetComponent<Rigidbody>().constraints == RigidbodyConstraints.None && timeOpen < Time.time - 1 &&
            collision.gameObject.layer == 11) {
            stillShut = false;
            timeOpen = Time.time;
            int sound = Random.Range(0, 4);
            var clip = Resources.Load("DoorOpen0" + sound) as AudioClip;
            source.clip = clip;
            source.pitch = Random.Range(0.98f, 1.02f);
            source.Play();
        }
    }
}