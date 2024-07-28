using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

[ExecuteInEditMode]
public class FlickerLight : MonoBehaviour {
    public float maxBright = 50;
    public float minBright = 0;
    public float time = 1;
    private HDAdditionalLightData light;
    private float randomTimer = 1;

    private void Start() {
        light = GetComponent<HDAdditionalLightData>();
    }

    private void Update() {
        //Random length to power intensity to minbright, random starting brightness to get there
        if (time >= 1) {
            time = 0;
            randomTimer = Random.Range(1.0f, 100.0f);
            light.intensity = Random.Range(minBright, maxBright);
        }

        time = Mathf.Min(1, time + randomTimer * Time.deltaTime);
        //Iterate
        light.intensity = Mathf.Lerp(light.intensity, minBright, time);
    }
}