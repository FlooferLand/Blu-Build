using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class HighlightMissingScripts : MonoBehaviour {
    [MenuItem("Tools/Find Missing Scripts")]
    private static void FindMissing() {
        var brokenGameObjects = new List<GameObject>();
        var allGameObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject go in allGameObjects) {
            var components = go.GetComponents<Component>();
            if (components.Any(component => component == null)) {
                brokenGameObjects.Add(go);
            }
        }

        if (brokenGameObjects.Count > 0) {
            Debug.Log($"Found {brokenGameObjects.Count} GameObjects with missing scripts.");
            foreach (var gameObject in brokenGameObjects) {
                Debug.LogWarning("Missing script on GameObject: " + gameObject.name, gameObject);
                EditorGUIUtility.PingObject(gameObject);
            }
        } else {
            Debug.Log("No missing scripts found.");
        }
    }
}
