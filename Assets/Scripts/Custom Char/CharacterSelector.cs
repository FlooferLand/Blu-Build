using System;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[Serializable]
public class CharacterSelector {
    [Serializable]
    public enum PremadeAnimations {
        Activate
    }

    public GameObject mainCharacter;
    public string characterName;
    public Sprite icon;
    public int currentCostume;
    public CharacterCostume[] allCostumes;

    [SerializedDictionary("Animation type", "Controller")]
    public SerializedDictionary<PremadeAnimations, RuntimeAnimatorController> premadeAnimations = new();

    public void SwapCharacter(int costumeIndex) {
        currentCostume = costumeIndex;
    }
}

[Serializable]
public class CharacterCostume {
    //This is if a costume has a different rig.
    public enum CostumeT {
        Costume,
        Retrofit,
        Innards
    }

    public string costumeName;
    public string costumeDesc;
    public Sprite costumeIcon;
    public string yearOfCostume;
    public CostumeT costumeType;
    public Vector3 offsetPos;
}