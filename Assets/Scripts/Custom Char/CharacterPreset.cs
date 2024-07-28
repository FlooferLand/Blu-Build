using System;
using UnityEngine;

[Serializable]
public class CharacterPreset {
    [Range(0.0f, 1.0f)] public float bodySmoothness;

    public bool bodyMetallic;
    public Color32 bodyColor;
    public Color32 metalColor;
    public uint characterHat;
    public uint characterFace;
    public uint characterFaceDecal;
}