using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

/**
 * For visible fake animatronics that are going to be replaced with the actual animated counterparts
 */
public class FakeCharacter : MonoBehaviour {
    [Header("Info")]
    public ShowId show = ShowId.None;
    public GameObject stage;
    public GameObject speakerL;
    public GameObject speakerR;
    public GameObject[] cameras;
    
    [Header("Info (read-only)")]
    public List<CharacterStagePosition> characters;
    
    [Header("References")]
    public FakeLiveControls liveControls;
    public GameObject stageHolder;
    public GameObject characterHolder;

    [Header("References (Live Controls)")]
    public Button3D button;
    public GameObject liveEditor;
    public GameObject showSelector;
    
    private GlobalController controller;
    
    public void Start() {
        controller = FindFirstObjectByType<GlobalController>();

        if (show != ShowId.None) {
            button.funcWindow = (int) show;
            button.ui = controller.gameObject;
        }
        else {
            Debug.LogError($"Show creator has no clue what show to make in '{gameObject.name}/{nameof(FakeCharacter)}'!");
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(FakeCharacter))]
public class FakeCharacterEditor : Editor {
    private void Awake() {
        var character = target as FakeCharacter;
        if (!character) return;

        if (character.stage && character.stageHolder.transform.childCount < 1) {
            Instantiate(character.stage, character.stageHolder.transform);
        }
    }

    public override void OnInspectorGUI() {
        var groupStyle = new GUIStyle {
            normal = new GUIStyleState {
                textColor = Color.Lerp(Color.gray, Color.white, 0.6f)
            },
            fontStyle = FontStyle.Bold
        };

        GUILayout.Label("Component that simplifies placing down existing characters on new maps");
        GUILayout.Space(20);
        
        DrawDefaultInspector();
        GUILayout.Space(20);
        
        var character = (FakeCharacter) target;
        if (!character) return;
        
        GUILayout.Label("Util", groupStyle);
        GUILayout.Space(10);
        if (GUILayout.Button("Create character")) {
            var obj = new GameObject();
            var comp = obj.AddComponent<CharacterStagePosition>();
            comp.model = obj;
            obj.transform.parent = character.characterHolder.transform;
        }
    }
    
    private void OnSceneGUI() {
        var show = target as FakeCharacter;
        if (!show) return;

        show.characters.Clear();
        var parent = show.characterHolder.transform;
        for (int i = 0; i < parent.childCount; i++) {
            var transform = parent.GetChild(i);
            var comp = transform.gameObject.GetComponent<CharacterStagePosition>();
            if (comp && comp.isActiveAndEnabled) {
                show.characters.Insert(i, comp);

                if (comp.alwaysVisible) {
                    comp.transform.GetChild(0)?.gameObject.SetActive(true);
                } else {
                    comp.transform.GetChild(0)?.gameObject.SetActive(false);
                }
            }
        }
    }

    private void OnDestroy() {
        var character = target as FakeCharacter;
        if (!character) return;

        while (character.stageHolder.transform.childCount > 1) {
            DestroyImmediate(character.stageHolder.transform.GetChild(0).gameObject);
        }
    }
}
#endif