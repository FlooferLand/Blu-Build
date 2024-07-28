using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class UI_PlaybackMarker : MonoBehaviour {
    public Color normalColour = Color.white;
    public Color bitHitColour = Color.cyan;
    protected RawImage image;

    private void Start() {
        image = GetComponent<RawImage>();
    }

    private void Update() {
        // image.color = Color.Lerp(image.color, normalColour, 12 * Time.deltaTime);
    }

    /**
     * Should be triggered when a bit is hit by the playback marker; For special effects
     */
    public void triggerBitHit() {
        image.color = bitHitColour;
    }

    /**
     * Should be triggered when a bit is no longer hit by the playback marker; For special effects
     */
    public void triggerBitReset() {
        // Unused for now!
        image.color = normalColour;
    }
}