using UnityEngine;

public class LobbyMusicManager : MonoBehaviour {
    private Player[] players;
    private AudioSource[] sources;

    private void Start() {
        sources = GetComponentsInChildren<AudioSource>();
        foreach (var source in sources) source.Play();
    }

    private void Update() {
        // TODO: Make it so the music stops playing when a show selector / stage is active
    }
}