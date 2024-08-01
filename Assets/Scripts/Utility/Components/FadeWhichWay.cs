using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FadeWhichWay : MonoBehaviour {
    public FadeType type;
    private MaskableGraphic fading;
    
    public enum FadeType {
        Default,
        FadeIn,
        FadeOut
    }

    private void Start() {
        fading = GetComponent<MaskableGraphic>();
        if (!fading) {
            Debug.LogError($"The {nameof(FadeWhichWay)} component expected a {nameof(MaskableGraphic)} component! (ex: an Image, RawImage, etc)");
            Destroy(this);
        }
    }

    public void Update() {
        if (!fading) return;

        if (type is FadeType.FadeIn or FadeType.Default)
            fading.color = new Color(1, 1, 1, Mathf.Lerp(fading.color.a, 1.0f, Time.deltaTime * 0.8f));
        else if (type is FadeType.FadeOut)
            fading.color = new Color(1, 1, 1, Mathf.Lerp(fading.color.a, 0.0f, Time.deltaTime * 0.8f));
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(FadeWhichWay))]
public class FadeWhichWayEditor : Editor {
    public override void OnInspectorGUI() {
        var fadeObj = (FadeWhichWay) target;

        bool foundValidComponent = 
            fadeObj.GetComponents<MaskableGraphic>().Length > 0;

        if (!foundValidComponent) {
            var content = new GUIContent("No valid component found!\nCannot fade a component with no colour attribute!");
            content.tooltip = "Add a component like Image or RawImage";
            var style = new GUIStyle();
            style.normal.textColor = Color.red;
            GUILayout.Label(content, style);
            GUILayout.Space(10);
        }
        
        DrawDefaultInspector();
    }
}
#endif
