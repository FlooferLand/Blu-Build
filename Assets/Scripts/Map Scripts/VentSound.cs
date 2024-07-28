using UnityEngine;

public class VentSound : MonoBehaviour {
    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.layer == 11) {
            var sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
            sc.clip = (AudioClip)Resources.Load("VentHit");
            sc.Play();
        }
    }
}