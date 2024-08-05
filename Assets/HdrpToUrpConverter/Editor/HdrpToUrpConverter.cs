#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

// TODO: Convert Shader.PropertyToID to HdrpShader.PropertyToID and UrpShader.PropertyToID respectively
// TODO: Figure out why using HdrpShader and UrpShader instead of Shader breaks the ability to find the ID by a string name

#if UNITY_EDITOR
// ReSharper disable InconsistentNaming
public class HdrpToUrpConverter : EditorWindow {
    private class ShaderProperties {
        // Basic colour
        public static readonly CrossShaderProperty Color = new(both: "_BaseColor", CrossShaderProperty.Type.Color);
        public static readonly CrossShaderProperty AlbedoMap = new(hdrp: "_BaseColorMap", urp: "_BaseMap", CrossShaderProperty.Type.Texture);

        // Properties
        public static readonly CrossShaderProperty Smoothness = new(both: "_Smoothness", CrossShaderProperty.Type.Range);
        public static readonly CrossShaderProperty Metallic = new(both: "_Metallic", CrossShaderProperty.Type.Float);

        // Detail
        public static readonly CrossShaderProperty DetailMap = new(hdrp: "_DetailMap", urp: "_DetailMask", CrossShaderProperty.Type.Texture);
        public static readonly CrossShaderProperty DetailAlbedoMap = new(hdrp: null, urp: "_DetailAlbedoMap", CrossShaderProperty.Type.Texture);
        public static readonly CrossShaderProperty DetailAlbedoMapScale = new(hdrp: null, urp: "_DetailAlbedoMapScale", CrossShaderProperty.Type.Float);
        public static readonly CrossShaderProperty DetailNormalMap = new(hdrp: null, urp: "_DetailNormalMap", CrossShaderProperty.Type.Texture);
        public static readonly CrossShaderProperty DetailNormalMapScale = new(hdrp: null, urp: "_DetailNormalMapScale", CrossShaderProperty.Type.Float);

        // Normal map
        public static readonly CrossShaderProperty NormalMap = new(hdrp: "_NormalMap", urp: "_BumpMap", CrossShaderProperty.Type.Texture);
        public static readonly CrossShaderProperty NormalIntensity = new(hdrp: "_NormalScale", urp: "_BumpScale", CrossShaderProperty.Type.Float);
        public static readonly CrossShaderProperty HeightMap = new(hdrp: "_HeightMap", urp: "_ParallaxMap", CrossShaderProperty.Type.Texture);

        // Emission
        public static readonly CrossShaderProperty EmissionMap = new(hdrp: "_EmissiveColorMap", urp: "_EmissionMap", CrossShaderProperty.Type.Texture);
        public static readonly CrossShaderProperty EmissionColor = new(hdrp: "_EmissiveColor", urp: "_EmissionColor", CrossShaderProperty.Type.Color);

        public static readonly string HDRP_EmissionToggle = "_EmissiveIntensityUnit";
        public static readonly string HDRP_EmissionIntensity = "_EmissiveIntensity";

        // Specular
        public static readonly CrossShaderProperty SpecularColor = new("_SpecularColor", urp: "_SpecColor", CrossShaderProperty.Type.Color);
        public static readonly CrossShaderProperty SpecularMap = new("_SpecularColorMap", urp: "_SpecGlossMap", CrossShaderProperty.Type.Texture);

        // Masks
        public static readonly HdrpMaskRemapper HDRP_MainMask = new("_MaskMap", "_MetallicGlossMap", "_OcclusionMap", DetailMap.urp, "_SpecGlossMap");

        public static readonly string URP_OcclusionStrength = "_OcclusionStrength";
        // private static readonly HdrpMaskRemapper HDRP_DetailMask = new("_DetailMap", "_MetallicGlossMap",  "_OcclusionMap", DetailMap.urp, Smoothness.urp);
        // private static readonly string HDRP_DetailMask = "_DetailMap";

        // Misc
        public static readonly string HDRP_MaterialType = "_MaterialID";
        public static readonly string HDRP_SupportDecals = "_SupportDecals";
    }

    /** Enables extra debug stuff */
    public static bool ExtraLogging = false;
    
    /** Doesn't override existing materials; instead, makes new ones */
    public static bool MakeNewMaterial = false;
    
    /** Creates a subfolder for the new unpacked mask textures */
    public static bool PutTexturesInSubfolder = false;

    // Types
    public static readonly Shader HdrpShader = Shader.Find("HDRP/Lit");
    public static readonly Shader UrpShader  = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("URP/Lit");
    
    enum HdrpMaterialType {
        Subsurface = 0,
        Standard = 1,
        Anisotropy = 2,
        Iridescence = 3,
        SpecularColor = 4,
        Translucent = 5
    }

    [MenuItem("Tools/Convert HDRP Lit Materials to URP Lit")]
    public static void ConvertMaterials() {
        if (!UrpShader) {
            Debug.LogError("Shaders not found. Ensure the URP and HDRP packages are installed.\nThe HDRP package is required, as all HDRP materials use the \"InternalErrorShader\" without it!");
            return;
        }

        int converted = 0;
        string[] materialGUIDs = AssetDatabase.FindAssets("t:Material");
        foreach (string guid in materialGUIDs) {
            string pathStr = AssetDatabase.GUIDToAssetPath(guid);
            var path = new FileInfo(pathStr);
            Material baseMat = AssetDatabase.LoadAssetAtPath<Material>(pathStr);

            // DEBUG
            // if (baseMat.name.ToLower().Trim() != "testmat") {
            //     continue;
            // }
            
            if (baseMat.shader == HdrpShader) {
                if (ConvertMaterial(baseMat, baseMatPath:path) is { } mat) {
                    if (MakeNewMaterial) {
                        string filename = $"{Path.GetFileNameWithoutExtension(pathStr)} (New){Path.GetExtension(pathStr)}";
                        string newPath = Path.Combine(pathStr, "..", filename);
                        LogDebugOnly($"Created material at path \"{newPath}\"");
                        AssetDatabase.CreateAsset(mat, newPath);
                    } else {
                        baseMat = mat;
                        EditorUtility.SetDirty(baseMat);
                        LogDebugOnly($"Got material at path \"{path}\"");
                    }
                    converted += 1;
                }
            }
        }

        if (converted > 0) {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"{converted} HDRP materials were successfully converted to URP.");
        } else {
            Debug.Log("Found no HDRP materials that could be converted.");  
        }
    }

    private static Material? ConvertMaterial(Material baseMat, FileInfo baseMatPath) {
        var materialType = (HdrpMaterialType) baseMat.GetInt(ShaderProperties.HDRP_MaterialType);
        var mat = new Material(baseMat) {
            shader = UrpShader
        };
        
        // Transfer over misc properties with the same name
        for (int i = 0; i < ShaderUtil.GetPropertyCount(baseMat.shader); i++) {
            string propertyName = ShaderUtil.GetPropertyName(baseMat.shader, i);
            var propertyType = ShaderUtil.GetPropertyType(baseMat.shader, i);
            if (mat.HasProperty(propertyName)) {
                switch (propertyType) {
                    case ShaderUtil.ShaderPropertyType.Color:
                        mat.SetColor(propertyName, baseMat.GetColor(propertyName));
                        break;
                    case ShaderUtil.ShaderPropertyType.Vector:
                        mat.SetVector(propertyName, baseMat.GetVector(propertyName));
                        break;
                    case ShaderUtil.ShaderPropertyType.Int:
                        mat.SetInteger(propertyName, baseMat.GetInteger(propertyName));
                        break;
                    case ShaderUtil.ShaderPropertyType.Float:
                    case ShaderUtil.ShaderPropertyType.Range:
                        mat.SetFloat(propertyName, baseMat.GetFloat(propertyName));
                        break;
                    case ShaderUtil.ShaderPropertyType.TexEnv:
                        mat.SetTexture(propertyName, baseMat.GetTexture(propertyName));
                        break;
                }
            }
        }

        // NOTE: The "Property () already exists with a different type: 0" errors mean the type of the property isn't a texture
        {
            // Transfer over additional HDRP material properties
            var additional = new[] {
                ShaderProperties.Color,
                ShaderProperties.AlbedoMap,
                
                ShaderProperties.Metallic,
                
                ShaderProperties.NormalMap,
                ShaderProperties.NormalIntensity,
                ShaderProperties.HeightMap,
                
                ShaderProperties.SpecularColor,
                ShaderProperties.SpecularMap,
                
                ShaderProperties.EmissionMap,
                ShaderProperties.EmissionColor
            };
            foreach (var property in additional) {
                switch (property.type) {
                    case CrossShaderProperty.Type.Texture:
                        var map = ReadableEditorTexture.Fetch(baseMat, baseMatPath, property.hdrp);
                        if (map is not null)
                            mat.SetTexture(property.urp, map.texture);
                        break;
                    case CrossShaderProperty.Type.Range:
                        mat.SetFloat(property.urp, baseMat.GetFloat(property.hdrp));
                        break;
                    case CrossShaderProperty.Type.Float:
                        mat.SetFloat(property.urp, baseMat.GetFloat(property.hdrp));
                        break;
                    case CrossShaderProperty.Type.Int:
                        mat.SetInteger(property.urp, baseMat.GetInteger(property.hdrp));
                        break;
                    case CrossShaderProperty.Type.Color:
                        mat.SetColor(property.urp, baseMat.GetColor(property.hdrp));
                        break;
                    case CrossShaderProperty.Type.Vector:
                        mat.SetVector(property.urp, baseMat.GetVector(property.hdrp));
                        break;
                }
            }

            // Unwrap and add the mask map if needed
            mat = UnpackMaskMap(baseMat, baseMatPath, ShaderProperties.HDRP_MainMask, mat);
        }

        return mat;
    }

    private static Material UnpackMaskMap(Material baseMat, FileInfo baseMatPath, HdrpMaskRemapper mask, Material mat) {
        if (ReadableEditorTexture.Fetch(baseMat, baseMatPath, mask.hdrpMask, out string? maskTexturePath) is not { } maskMap || maskTexturePath == null) {
            LogDebugOnly($"Could not read mask texture at path \"{ShaderProperties.HDRP_MainMask}\"");
            return mat;
        }
        
        int width = maskMap.texture.width;
        int height = maskMap.texture.height;
        LogDebugOnly($"Found map of {width}x{height} in \"{baseMatPath.FullName}\"");
        
        // Copy textures
        var r = new List<Color>();
        var g = new List<Color>();
        var b = new List<Color>();
        var a = new List<Color>();
        Color[] maskColors;
        try {
            maskColors = maskMap.texture.GetPixels();
        }
        catch (ArgumentException exception) {
            Debug.LogError($"Call to GetPixels failed on mask map \"{maskMap.texture.name}\".\nDoes the HDRP mask texture field exist on its shader?\n\n{exception}");
            return mat;
        }
        for (int y = 0; y < maskMap.texture.height; y++) {
            for (int x = 0; x < maskMap.texture.width; x++) {
                Color color = maskColors[Mathf.Min(x, width - 1) + Mathf.Min(y, height - 1) * width];
                r.Add(new Color(color.r, color.r, color.r));
                g.Add(new Color(color.g, color.g, color.g));
                b.Add(new Color(color.b, color.b, color.b));
                a.Add(new Color(color.a, color.a, color.a));
            }
        }
        
        // Paths
        LogDebugOnly($"- BASE MAT PATH {baseMatPath}");
        
        // Creating the new textures
        if (mask == ShaderProperties.HDRP_MainMask) {
            var metallic = createIndividualMaskTextureAsset(maskTexturePath, width, height, r.ToArray(), "Metallic", Color.black);
            var occlusion = createIndividualMaskTextureAsset(maskTexturePath, width, height, g.ToArray(), "AO", Color.white);
            var detail = createIndividualMaskTextureAsset(maskTexturePath, width, height, b.ToArray(), "Detail", Color.black);
            var smoothness = createIndividualMaskTextureAsset(maskTexturePath, width, height, a.ToArray(), "Smoothness", Color.gray);

            mat.SetFloat(ShaderProperties.URP_OcclusionStrength, occlusion is not null ? 1 : 0);
            
            mat.SetTexture(mask.urpR, metallic);
            mat.SetTexture(mask.urpG, occlusion);
            mat.SetTexture(mask.urpB, detail);
            mat.SetTexture(mask.urpA, smoothness);
            return mat;
        }
        
        Debug.LogError($"Remap implementation not found for HDRP mask at property '{mask.hdrpMask}'");
        return mat;
    }
    
    // TODO: Don't make a texture if its the default colour and return null!
    private static Texture2D? createIndividualMaskTextureAsset(string maskTexturePath, int width, int height, Color[] pixels, string name, Color defaultColor) {
        // Texture might contain useless data!
        int timesFound = pixels.Count(color => color == defaultColor);
        if (timesFound > pixels.Length - 5) {
            return null;
        }

        // Creating the texture
        var texture = new Texture2D(width, height, TextureFormat.RFloat, false);
        texture.SetPixels(pixels);
        texture.Apply();

        // Getting / making the paths
        string parentPath;
        if (Directory.GetParent(maskTexturePath)?.FullName is { } parent) {
            if (parent.IndexOf("Assets", StringComparison.InvariantCultureIgnoreCase) is var i and > 0) {
                parentPath = parent[i..];
            } else {
                Debug.LogWarning($"Failed to get path for texture \"{maskTexturePath}\"\nRecovery will be attempted, but you should check if it actually worked or if it just placed the new texture in the wrong place.");
                parentPath = parent;
                return null;
            }
        } else {
            Debug.LogWarning($"Failed to get path for texture \"{maskTexturePath}\"");
            return null;
        }
        string textureName = Path.GetFileName(maskTexturePath)
            .Replace("_Mask", "")
            .Replace("Mask", "")
            .Replace("_mask", "")
            .Replace("mask", "")
            .Replace("_MADS", "")
            .Replace("_MAODS", "")
            .Replace("MADS", "")
            .Replace("MAODS", "");
        if (textureName.Contains('.')) {
            textureName = textureName.Split('.')[0];
        }
        if (textureName.Contains('_')) {
            textureName = textureName.Split('_')[0];
        }
        if (textureName.EndsWith('_') || textureName.EndsWith('.')) {
            textureName = textureName[..^1];
        }

        string folderPath = Path.Combine(parentPath, textureName);  // AssetDatabase.GUIDToAssetPath(guid);
        string filename = $"{textureName}_{name}.png";
        string texturePath = PutTexturesInSubfolder ? Path.Combine(folderPath, filename) : filename;

        if (PutTexturesInSubfolder) {
            if (!AssetDatabase.IsValidFolder(folderPath)) {
                // LogDebugOnly($"Creating folder at path \"{folderPath}\"");
                AssetDatabase.CreateFolder(parentPath, textureName);
            }
        }

        // LogDebugOnly($"Creating sub-texture of mask texture at path \"{texturePath}\"");
        File.WriteAllBytes(texturePath, texture.EncodeToPNG());
        AssetDatabase.ImportAsset(texturePath);
        AssetDatabase.Refresh();
        
        return AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D)) as Texture2D;
    }

    public static void LogDebugOnly(string str) {
        if (ExtraLogging) {
            Debug.Log(str);
        }
    }
}
#endif

#if UNITY_EDITOR
public class ReadableEditorTexture : IDisposable {
    protected TextureImporter importer;
    public Texture2D texture;
    protected ReadableEditorTexture(TextureImporter importer, Texture2D texture) {
        this.importer = importer;
        this.texture = texture;
    }

    public static ReadableEditorTexture? Fetch(Material mat, FileInfo matPath, string propertyId) {
        return Fetch(mat, matPath, propertyId, out string? _);
    }

    public static ReadableEditorTexture? Fetch(Material mat, FileInfo matPath, string propertyId, out string? texturePath) {
        string location = $"{nameof(ReadableEditorTexture)}.{nameof(Fetch)}";

        // Loading the texture importer
        TextureImporter importer;
        if (mat.HasTexture(propertyId) && mat.GetTexture(propertyId) is { } tex1) {
            texturePath = AssetDatabase.GetAssetPath(tex1);
            
            if (AssetImporter.GetAtPath(texturePath) is not { } assetImporter) {
                Debug.LogError($"Failed loading importer for asset \"{matPath.FullName}\"\n: {location}");
                return null;
            }
            if (assetImporter is not TextureImporter porter) {
                Debug.LogError($"Failed cast to {nameof(TextureImporter)} for texture \"{propertyId}\" (\"{matPath.FullName}\")\n: {location}");
                return null;
            }
            importer = porter;
        }
        else {
            HdrpToUrpConverter.LogDebugOnly($"Material \"{mat.name}\" does not have a texture for ID \"{propertyId}\"\n: {location}");
            texturePath = null;
            return null;
        }

        // Loading the texture
        importer.isReadable = true;
        importer.SaveAndReimport();
        if (mat.HasTexture(propertyId) && mat.GetTexture(propertyId) is Texture2D importedTex) {
            return new ReadableEditorTexture(importer, importedTex);
        }

        // Unable to load the texture
        Debug.LogError($"Failed loading texture \"{matPath.FullName}\"\nMaterial does not have a texture for ID \"{propertyId}\"\n: {location}");
        importer.isReadable = false;
        importer.SaveAndReimport();
        return null;
    }
    
    public void Dispose() {
        importer.isReadable = false;
        importer.SaveAndReimport();
    }
}
#endif

public class CrossShaderProperty {
    public readonly string hdrp;
    public readonly string urp;
    public readonly Type type;

    public enum Type {
        Color,
        Vector,
        Range,
        Float,
        Texture,
        Int
    }
    
    public CrossShaderProperty(string both, Type type) {
        hdrp = both;
        urp = both;
        this.type = type;
    }
    
    public CrossShaderProperty(string hdrp, string urp, Type type) {
        this.hdrp = hdrp;
        this.urp = urp;
        this.type = type;
    }
}

public class HdrpMaskRemapper {
    public readonly string hdrpMask;
    public readonly string urpR, urpG, urpB, urpA;
    
    public HdrpMaskRemapper(string mask, string urpR, string urpG, string urpB, string urpA) {
        hdrpMask = mask;
        this.urpR = urpR;
        this.urpG = urpG;
        this.urpB = urpB;
        this.urpA = urpA;
    }
}
