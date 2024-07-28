using UnityEngine;

public class PlayAudioOnEnabled : MonoBehaviour {
    private void OnEnable() {
        GetComponent<AudioSource>().Play();
    }
}