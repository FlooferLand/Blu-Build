using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class IntroScreen : MonoBehaviour {
    public AudioSource globalAudio;
    public FadeWhichWay fader;
    
    private void Awake() {
        fader.gameObject.SetActive(true);
    }

    public void Continue() {
        globalAudio.clip = (AudioClip)Resources.Load("tap");
        globalAudio.pitch = Random.Range(0.95f, 1.05f);
        globalAudio.Play();
        if (PlayerPrefs.GetInt("Intro: TutorialA") == 0) {
            // SceneManager.LoadScene("Tutorial", LoadSceneMode.Single);
            SceneManager.LoadScene("Title Screen", LoadSceneMode.Single);
        }
        else {
            SceneManager.LoadScene("Title Screen", LoadSceneMode.Single);
        }
    }
}
