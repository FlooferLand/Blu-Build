using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Compilation;
using UnityEditor.XR.Management;
using UnityEngine;
using UnityEngine.XR.Management;

[CustomEditor(typeof(SimulatorManager))]
public class GameInfoManager : Editor {
    [HideInInspector] public bool fazAnim;
    [HideInInspector] public bool vr;

    public const string PersistentFileName = "InternalGameBuildType.txt";
    public const string FazAnimIdent = "faz";
    public const string CdIdent = "cd";

    private static readonly List<string> FazMissingScenes = new() {
        "Assets/Scenes/Arcade Mr Hugs/Arcade Mr Hugs.unity",
        "Assets/Scenes/Tutorial/Tutorial.unity"
    };

    private static readonly List<string> CdMissingScenes = new() {
        "Assets/Simulator/Scenes/Studio C/Studio C.unity",
        "Assets/Simulator/Scenes/Front Entrance/Rival Restaurant.unity",
        "Assets/Simulator/Scenes/CRAE/CRAE.unity",
        "Assets/Simulator/Scenes/CYBERS/CYBERS.unity",  // DELETE 🤖  DELETE 🤖
        "Assets/Scenes/Tutorial/Tutorial.unity"
    };

    public override void OnInspectorGUI() {
        var manager = (SimulatorManager) target;
        DrawDefaultInspector();
        
        if (PlayerSettings.productName == manager.fazProductName) {
            fazAnim = true;
        } else if (PlayerSettings.productName == manager.cdProductName) {
            fazAnim = false;
        } else {
            Debug.LogWarning($"Product name \"{PlayerSettings.productName}\" doesn't match the two valid ones!\nPlease update it in the player settings");
        }
        
        #region Faz-Anim button
        GUI.backgroundColor = fazAnim ? Color.green : Color.grey;
        if (GUILayout.Button("Set " + manager.fazProductName)) SetFazAnim();  // "Expensive method invocation" kill yourself ⛈️⚡
        if (EditorGUI.EndChangeCheck() && fazAnim) {
            if (vr)
                PlayerSettings.bundleVersion = manager.fazVersion + " VR";
            else
                PlayerSettings.bundleVersion = manager.fazVersion;
        }
        GUILayout.Space(5);
        #endregion

        // Bad code, I know. Get better.
        #region CD button
        GUI.backgroundColor = fazAnim ? Color.grey : Color.green;
        if (GUILayout.Button("Set " + manager.cdProductName.Replace(manager.fazProductName, "").Replace("(", "").Replace(")", ""))) SetCd();
        if (EditorGUI.EndChangeCheck() && !fazAnim) {
            if (vr)
                PlayerSettings.bundleVersion = manager.cdVersion + " VR";
            else
                PlayerSettings.bundleVersion = manager.cdVersion;
        }
        GUILayout.Space(5);
        #endregion
        
        #region VR section
        GUILayout.Space(20);
        GUI.backgroundColor = vr ? Color.green : Color.grey;
        if (GUILayout.Button("Toggle VR Build")) SetVR();
        GUILayout.Space(20);
        #endregion
        
        // Notes!
        AddMissingNote(FazMissingScenes, manager.fazProductName);
        AddMissingNote(CdMissingScenes, manager.cdProductName);
    }
    
    private void AddMissingNote(List<string> missingScenes, string name) {
        string missingStr = missingScenes.Aggregate(
            $"NOTE: {name} is missing the following scenes:\n",
            (current, scene) => current + $"- {scene}\n"
        );
        GUILayout.Label(missingStr);
    }
    
    public void SetFazAnim() {
        var manager = (SimulatorManager)target;
        fazAnim = true;
        
        // Find valid Scene paths and make a list of EditorBuildSettingsScene
        var editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();
        for (int i = 0; i < manager.fazScenes.Count; i++) {
            var scene = manager.fazScenes[i];
            if (scene is null || string.IsNullOrEmpty(scene.ScenePath)) {
                Debug.LogError($"Scene at index {i} wasn't found inside {nameof(GameInfoManager)}");
                continue;
            }
            editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(scene.ScenePath, true));
        }

        // Set the Build Settings window Scene list
        EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
        
        // Other stuff
        PlayerSettings.productName = manager.fazProductName;
        var tex = new Texture2D[8];
        for (int i = 0; i < 8; i++) tex[i] = manager.fazTexture as Texture2D;
        PlayerSettings.SetIcons(NamedBuildTarget.Standalone, tex, IconKind.Any);
        
        // Writing the special game info data
        WriteSpecialBuildData(GameBuildType.Faz);
    }

    public void SetCd() {
        var manager = (SimulatorManager)target;
        fazAnim = false;
        
        // Find valid Scene paths and make a list of EditorBuildSettingsScene
        var editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();
        foreach (string scenePath in manager.cdScenes)
            if (!string.IsNullOrEmpty(scenePath))
                editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(scenePath, true));

        // Set the Build Settings window Scene list
        EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();

        PlayerSettings.productName = manager.cdProductName;
        var tex = new Texture2D[8];
        for (int i = 0; i < 8; i++) tex[i] = manager.cdTexture as Texture2D;
        PlayerSettings.SetIcons(NamedBuildTarget.Standalone, tex, IconKind.Any);
        
        // Writing the special game info data
        WriteSpecialBuildData(GameBuildType.Cd);
    }

    public void SetVR() {
        XRGeneralSettingsPerBuildTarget generalSettings = null;
        EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.k_SettingsKey, out generalSettings);
        if (!generalSettings) {
            EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.k_SettingsKey, out generalSettings);
            if (!generalSettings) {
                string searchText = "t:XRGeneralSettings";
                string[] assets = AssetDatabase.FindAssets(searchText);
                if (assets.Length > 0) {
                    string path = AssetDatabase.GUIDToAssetPath(assets[0]);
                    generalSettings =
                        AssetDatabase.LoadAssetAtPath(path, typeof(XRGeneralSettingsPerBuildTarget)) as
                            XRGeneralSettingsPerBuildTarget;
                }
            }

            EditorBuildSettings.AddConfigObject(XRGeneralSettings.k_SettingsKey, generalSettings, true);
        }

        var buildTargetGroup = EditorGUILayout.BeginBuildTargetSelectionGrouping();
        var settings = generalSettings.SettingsForBuildTarget(buildTargetGroup);

        var serializedSettingsObject = new SerializedObject(settings);
        serializedSettingsObject.Update();
        var initOnStart = serializedSettingsObject.FindProperty("m_InitManagerOnStart");
        if (vr)
            initOnStart.boolValue = false;
        else
            initOnStart.boolValue = true;
        serializedSettingsObject.ApplyModifiedProperties();
        vr = !vr;
        CompilationPipeline.RequestScriptCompilation();
    }

    public void WriteSpecialBuildData(GameBuildType buildType) {
        string ident = (buildType == GameBuildType.Faz ? FazAnimIdent : CdIdent);
        string path = Path.Combine(Application.dataPath, "Resources", PersistentFileName);
        File.WriteAllText(path, ident);
        AssetDatabase.Refresh();
        Debug.Log($"Wrote {ident} build data to \"{PersistentFileName}\"\nFull path: \"{path}\"");
    }
}