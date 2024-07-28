using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SaveWavFile : MonoBehaviour {
    private Text button;
    private MicController mic;

    private Text recordDisplay;
    private float recordingTimer;

    private AudioSource source;

    // Use this for initialization
    private void Start() {
        print("Persistent data path: " + Application.persistentDataPath);
        source = gameObject.GetComponent<AudioSource>();
        mic = gameObject.GetComponent<MicController>();
        button = transform.Find("Button_Mic").Find("Text").GetComponent<Text>();
        recordDisplay = transform.Find("RecordDisplay").Find("Text").GetComponent<Text>();
    }

    // Update is called once per frame
    private void Update() {
        // Animates the recording timer:
        if (mic.IsWorking) {
            recordingTimer -= Time.deltaTime;
            recordDisplay.text = "Remaining: " + recordingTimer.ToString("0.0");
            if (recordingTimer <= 0f) {
                recordingTimer = 0f;
                recordDisplay.text = "Time samples: " + source.timeSamples;
                StartRecording(); // Toggles the recording automatically.
            }
        }
    }

    // Toggles the recording status:
    public void StartRecording() {
        if (mic.IsWorking) {
            button.text = "Start recording";
            mic.WorkStop();
            // Timer:
            recordingTimer = 0f;
            recordDisplay.text = "Time samples: " + source.timeSamples;
        }
        else {
            button.text = "Stop recording";
            mic.WorkStart();
            // Timer:
            recordingTimer = 5f;
            recordDisplay.text = "Remaining: " + recordingTimer.ToString("0.0");
        }
    }

    // Player interfaces:
    public void Play() {
        source.Play();
    }

    public void Pause() {
        source.Pause();
    }

    public void Stop() {
        source.Stop();
    }

    // File control:
    public void DeleteClip() {
        source.clip = null;
        File.Delete(Path.Combine(Application.persistentDataPath, "MyFile.wav"));
    }

    public void SaveClip() {
        byte[] wavFile = OpenWavParser.AudioClipToByteArray(source.clip);
        File.WriteAllBytes(Path.Combine(Application.persistentDataPath, "MyFile.wav"), wavFile);
    }
}