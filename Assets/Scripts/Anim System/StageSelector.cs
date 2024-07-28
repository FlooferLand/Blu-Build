using System;
using UnityEngine;
using UnityEngine.Video;

[Serializable]
public class StageSelector {
    public enum StageT {
        Standard,
        Mini,
        Retrofit,
        Unofficial
    }

    [Header("Stage")] public GameObject stage;

    [Header("Description")] public string stageName;

    public string stageDesc;
    public string stageDate;
    public Sprite stageIcon;
    public StageT stageType;

    [Header("Characters")] public CharacterPos[] stageCharacters;

    [Header("Special Objects")] public GameObject curtain;

    [HideInInspector] public Curtain_Valves curtainValves;

    public GameObject lights;

    [HideInInspector] public LightController[] lightValves;

    public TurntableController[] tableValves;
    public TextureController texController;
    public ShowTV[] tvs;

    public void Startup() {
        //Curtains
        if (curtain != null) curtainValves = curtain.GetComponent<Curtain_Valves>();
        //Find amount of lights
        int count = 0;
        foreach (Transform child in lights.transform) {
            count++;
            foreach (Transform grandChild in child)
                count++;
        }

        lightValves = new LightController[count];

        //Apply Lights
        count = 0;
        foreach (Transform child in lights.transform) {
            lightValves[count] = child.GetComponent<LightController>();
            count++;
            foreach (Transform grandChild in child) {
                lightValves[count] = grandChild.GetComponent<LightController>();
                count++;
            }
        }
    }
}

[Serializable]
public class CharacterPos {
    public string characterName;
    public Vector3 characterPos;
    public Vector3 characterRot;
}


[Serializable]
public class ShowTV {
    public bool drawer;
    public int bitOff;
    public VideoPlayer[] tvs;
}