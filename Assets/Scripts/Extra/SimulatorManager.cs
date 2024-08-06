using System.Collections.Generic;
using Tymski;
using UnityEditor;
using UnityEngine;

public class SimulatorManager : MonoBehaviour {
    [Header("Faz-Anim")]
    public string fazProductName = "";
    public string fazVersion = "";
    public Texture fazTexture;
    public List<SceneReference> fazScenes;
    
    [Header("Cease-n-Desist")]
    public string cdProductName = "";
    public string cdVersion = "";
    public Texture cdTexture;
    public List<string> cdScenes;

    #if UNITY_EDITOR
        private void Start() {
            string productName;
            string version;
            if (PlayerSettings.productName == fazProductName) {
                productName = fazProductName;
                version = fazVersion;
            } else if (PlayerSettings.productName == cdProductName) {
                productName = cdProductName;
                version = cdProductName;
            } else {
                productName = null;
                version = null;
            }

            if (productName != null && version != null) {
                Debug.Log($"Started as \"{productName}\" ({version})");
            } else {
                Debug.LogError($"Unrecognized product name \"{productName}\" and/or version {version}");
            }
        }
    #endif
}