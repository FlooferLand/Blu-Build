using UnityEngine;
using UnityEngine.UI;

public class UI_PlaybackButton : MonoBehaviour {
    public RawImage PauseIcon;
    public RawImage PlayIcon;

    private void Start() {
        setPlaying(false);
    }

    public void setPlaying(bool isPlaying) {
        PauseIcon.enabled = isPlaying;
        PlayIcon.enabled = !isPlaying;
    }
}
