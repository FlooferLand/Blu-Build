using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour {
    public bool fadeBool;
    public RawImage fade;
    public GameObject[] screens;
    public Button left;
    public Button right;
    public GameObject finish;
    private int currentScreen = 0;

    private void Start() {
        left.gameObject.SetActive(true);
        right.gameObject.SetActive(true);
        ChangeScreen(1, true);
        ChangeScreen(-1, true);
        PlayerPrefs.SetInt("Intro: TutorialA", 1);
    }

    private void FixedUpdate() {
        if (!fadeBool) {
            var currColor = fade.color;
            currColor.a -= .01f;
            fade.color = currColor;
        }
        else {
            var currColor = fade.color;
            currColor.a += .01f;
            fade.color = currColor;
        }

        if (fadeBool && fade.color.a > 1) SceneManager.LoadScene("Title Screen", LoadSceneMode.Single);
    }

    public void ChangeScreen(int adder) {
        ChangeScreen(adder, false);
    }

    public void ChangeScreen(int adder, bool nosound) {
        if (!nosound) {
            var sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
            sc.clip = (AudioClip)Resources.Load("tap");
            sc.pitch = Random.Range(0.95f, 1.05f);
            sc.Play();
        }

        currentScreen += adder;
        currentScreen = Mathf.Min(Mathf.Max(currentScreen, 0), screens.Length - 1);

        if (currentScreen != screens.Length - 1) {
            finish.SetActive(false);
            right.interactable = true;
        }
        else {
            finish.SetActive(true);
            right.interactable = false;
        }

        if (currentScreen == 0)
            left.interactable = false;
        else
            left.interactable = true;
        for (int i = 0; i < screens.Length; i++)
            if (currentScreen == i)
                screens[i].SetActive(true);
            else
                screens[i].SetActive(false);
        if (currentScreen == 0) screens[0].SetActive(true);
    }

    public void Finish() {
        left.interactable = false;
        right.interactable = false;
        finish.GetComponent<Button>().interactable = false;
        var sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
        sc.clip = (AudioClip)Resources.Load("tap");
        sc.pitch = Random.Range(0.95f, 1.05f);
        sc.Play();
        fadeBool = true;
        var currColor = fade.color;
        currColor.a = 0;
        fade.color = currColor;
    }
}