using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MovieStage : MonoBehaviour {
    public UI_PlayRecord playUI;
    public Text textScreen;
    public Text textScreen2;
    public Light[] areaLights;
    public GameObject neonLights;

    public float lightIntensity = 33;

    private bool onCheck = true;

    // Update is called once per frame
    private void Update() {
        if (onCheck) {
            if (lightIntensity > 0) {
                lightIntensity -= 4 * (34 - Mathf.Min(lightIntensity, 33)) * Time.deltaTime;
                for (int i = 0; i < areaLights.Length; i++) areaLights[i].intensity = lightIntensity;
            }

            if (!playUI.manager.playMovements) {
                lightIntensity = 0;
                onCheck = false;
                textScreen.text = "Show Starting";
                textScreen2.text = "Soon...";
                neonLights.SetActive(false);
            }
        }
        else {
            if (lightIntensity < 33) {
                lightIntensity += 4 * (lightIntensity + 1) * Time.deltaTime;
                for (int i = 0; i < areaLights.Length; i++) areaLights[i].intensity = lightIntensity;
            }

            if (playUI.manager.playMovements) {
                lightIntensity = 33;
                onCheck = true;
                string[] combined = Path
                    .GetFileName(playUI.manager.showtapeSegmentPaths[playUI.manager.currentShowtapeSegment])
                    .Split(new[] { " - " }, StringSplitOptions.None);
                if (combined.Length > 1)
                    textScreen.text = combined[0];
                else
                    textScreen.text = combined[0].Substring(0, combined[0].Length - 5);
                textScreen2.text = "Starts @ " + string.Format("{0:hh:mm tt}", DateTime.Now);
                neonLights.SetActive(true);
            }
        }
    }
}