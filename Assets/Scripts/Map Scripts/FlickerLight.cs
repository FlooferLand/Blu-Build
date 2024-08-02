using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

[ExecuteInEditMode]
public class FlickerLight : MonoBehaviour {
    public float maxBright = 50;
    public float minBright = 0;
    public float time = 1;
    private Light lightData;
    private float randomTimer = 1;

    private void Awake() {
        lightData = GetComponent<Light>();
    }

    private void Update() {
        if (!lightData) return;
        
        // Random length to power intensity to minBright, random starting brightness to get there
        if (time >= 1) {
            time = 0;
            randomTimer = Random.Range(1.0f, 100.0f);
            lightData.intensity = Random.Range(minBright, maxBright);
        }

        time = Mathf.Min(1, time + randomTimer * Time.deltaTime);
        // Iterate
        lightData.intensity = Mathf.Lerp(lightData.intensity, minBright, time);
    }
}