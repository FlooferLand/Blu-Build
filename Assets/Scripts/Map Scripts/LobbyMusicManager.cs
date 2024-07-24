using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyMusicManager : MonoBehaviour {
    private AudioSource[] sources;
    private Player[] players;
    
    private void Start() {
        sources = GetComponentsInChildren<AudioSource>();
        foreach (AudioSource source in sources) {
            source.Play();
        }
    }

    private void Update() {
        // TODO: Make it so the music stops playing when a show selector / stage is active
    }
}
