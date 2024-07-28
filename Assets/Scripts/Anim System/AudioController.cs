using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {
    public List<int> soundBits;
    public List<AudioClip> soundClips;
    public List<bool> boolChecks;
    private AudioSource aud;
    private Mack_Valves bitChart;

    // Start is called before the first frame update
    private void Start() {
        bitChart = GameObject.Find("Mack Valves").GetComponent<Mack_Valves>();
        aud = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    public void Update() {
        for (int i = 0; i < soundBits.Count; i++)
            if (bitChart.topDrawer[soundBits[i] - 1]) {
                if (!boolChecks[i]) {
                    aud.PlayOneShot(soundClips[i]);
                    boolChecks[i] = true;
                }
            }
            else {
                boolChecks[i] = false;
            }
    }
}