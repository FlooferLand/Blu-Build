using System.Collections;
using SFB;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UI_SidePanel : MonoBehaviour {
    //Windows
    public GameObject WindowMenu;
    public GameObject WindowSound;
    public GameObject WindowShow;
    public GameObject WindowCamera;
    public GameObject WindowFlows;
    public GameObject WindowBitVis;

    //Text
    public Text psiText;
    public Text volumeText;
    public Text spacialText;
    public Text curtainText;
    public Text bonesText;
    public Text upperLightText;
    public Text rolfeKlunkText;
    public Text camFilterText;
    public Text camSmoothText;
    public Text soundvolumeText;
    public Text stagevolumeText;
    public Text signalSwapText;

    //Cam Filters
    public int currentCamProfile = 0;
    public VolumeProfile[] camProfiles;
    public Text flowProfileText;
    public Text flowNumText;
    public Text flowSpeedInText;
    public Text flowSpeedOutText;
    public Text flowWeightInText;
    public Text flowWeightOutText;
    public Text flowSlamInText;
    public Text flowSlamOutText;
    public Text flowSlamSpeedInText;
    public Text flowSlamSpeedOutText;
    public string FileExtention;

    //Other
    public GameObject areaLights;
    public UI_PlayRecord showPanelUI;
    public Static staticUI;

    public float[] copyPasteValues = new float[8];
    private DynamicBone[] allDynamics;
    private int flowNumber = 0;

    //Flow Controls
    private int flowProfile = 0;

    private bool hidepanels = false;

    private void Awake() {
        StartCoroutine(AwakeCoroutine());
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            if (hidepanels) {
                hidepanels = !hidepanels;
                transform.parent.localPosition -= Vector3.one * 100;
                showPanelUI.transform.parent.localPosition -= Vector3.one * 100;
            }
            else {
                hidepanels = !hidepanels;
                transform.parent.localPosition += Vector3.one * 100;
                showPanelUI.transform.parent.localPosition += Vector3.one * 100;
            }
        }

        if (Input.GetKeyDown(KeyCode.F1) || Input.GetKeyDown(KeyCode.BackQuote)) showPanelUI.pauseSong();
        if (Input.GetKeyDown(KeyCode.F2)) showPanelUI.pauseSong();
        // showPanelUI.seek();
    }

    private IEnumerator AwakeCoroutine() {
        yield return new WaitForSeconds(1f);
        allDynamics = FindObjectsByType<DynamicBone>(FindObjectsSortMode.None);
        yield return new WaitForSeconds(.2f);
        FlowUpdate();
        yield return new WaitForSeconds(.2f);
        showPanelUI.thePlayer = GameObject.Find("Player");
        if (InternalGameVersion.isVR != "true")
            if (showPanelUI.thePlayer != null)
                VHSToggle(0);
        var gg = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
        soundvolumeText.GetComponent<Text>().text = Mathf.Ceil(gg.volume * 100).ToString();
    }

    public void Upperlights(int input) {
        var sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
        Resources.Load("ting");
        sc.clip = (AudioClip)Resources.Load("Tech Lights");
        sc.pitch = Random.Range(0.95f, 1.05f);
        sc.Play();
        if (areaLights.activeSelf) {
            upperLightText.GetComponent<Text>().text = "Off";
            areaLights.SetActive(false);
        }
        else {
            upperLightText.GetComponent<Text>().text = "On";
            areaLights.SetActive(true);
        }
    }

    public void SwapWindow(int input) {
        staticUI.flash = true;
        WindowCamera.SetActive(false);
        WindowMenu.SetActive(false);
        WindowShow.SetActive(false);
        WindowSound.SetActive(false);
        WindowFlows.SetActive(false);
        WindowBitVis.SetActive(false);
        switch (input) {
            case 0:
                WindowMenu.SetActive(true);
                break;
            case 1:
                WindowSound.SetActive(true);
                break;
            case 2:
                WindowShow.SetActive(true);
                break;
            case 3:
                WindowCamera.SetActive(true);
                break;
            case 5:
                WindowFlows.SetActive(true);
                break;
            case 6:
                WindowBitVis.SetActive(true);
                break;
        }
    }

    public void DynamicSwitch(int input) {
        if (bonesText.GetComponent<Text>().text == "Off") {
            bonesText.text = "On";
            allDynamics = FindObjectsByType<DynamicBone>(FindObjectsSortMode.None);
            foreach (var bone in allDynamics) bone.enabled = true;
        }
        else {
            bonesText.text = "Off";
            allDynamics = FindObjectsByType<DynamicBone>(FindObjectsSortMode.None);
            foreach (var bone in allDynamics) bone.enabled = false;
        }
    }

    public void StageVolume(int input) {
        var yea = FindObjectsOfType(typeof(InstrumentSound)) as InstrumentSound[];
        for (int i = 0; i < yea.Length; i++) {
            yea[i].volume = Mathf.Min(Mathf.Max(yea[i].volume + input * .05f, 0), 1);
            stagevolumeText.text = Mathf.Ceil(yea[i].volume * 100).ToString();
        }
    }

    public void SignalSwap(int input) {
        if (showPanelUI.signalChange == UI_PlayRecord.SignalChange.normal) {
            signalSwapText.text = "On";
            switch (input) {
                case 0:
                    showPanelUI.signalChange = UI_PlayRecord.SignalChange.PreCU;
                    break;
                case 1:
                    showPanelUI.signalChange = UI_PlayRecord.SignalChange.PrePTT;
                    break;
            }
        }
        else {
            signalSwapText.text = "Off";
            showPanelUI.signalChange = UI_PlayRecord.SignalChange.normal;
        }
    }

    public void RolfeKlunkSwap(int input) {
        showPanelUI.swap = !showPanelUI.swap;
        if (showPanelUI.swap)
            rolfeKlunkText.text = "Klunk";
        else
            rolfeKlunkText.text = "Rolfe";
        showPanelUI.SwapCheck();
    }

    public void VHSToggle(int input) {
        currentCamProfile = Mathf.Max(Mathf.Min(currentCamProfile + input, camProfiles.Length - 1), 0);
        camFilterText.text = camProfiles[currentCamProfile].name;
        Camera.main.GetComponent<Volume>().profile = camProfiles[currentCamProfile];
        var gg = GameObject.Find("Global Controller").GetComponent<GlobalController>();
        gg.ApplyCamSettings(GameObject.Find("Player"));
        var P2 = GameObject.Find("Player 2");
        if (P2 != null) gg.ApplyCamSettings(P2);
    }

    public void SetSmoothCam(int input) {
        if (showPanelUI.thePlayer.GetComponent<Player>().enableCamSmooth) {
            camSmoothText.text = "Off";
            showPanelUI.thePlayer.GetComponent<Player>().enableCamSmooth = false;
        }
        else {
            camSmoothText.text = "On";
            showPanelUI.thePlayer.GetComponent<Player>().enableCamSmooth = true;
        }
    }

    public void AutoCurtains(int input) {
        for (int i = 0; i < showPanelUI.stages.Length; i++)
            if (showPanelUI.stages[i].curtainValves != null) {
                if (showPanelUI.stages[i].curtainValves.curtainOverride) {
                    curtainText.text = "Off";
                    showPanelUI.stages[i].curtainValves.curtainOverride = false;
                }
                else {
                    curtainText.text = "On";
                    showPanelUI.stages[i].curtainValves.curtainOverride = true;
                }
            }
    }

    public void PSIChange(int input) {
        showPanelUI.mackValves.GetComponent<Mack_Valves>().PSI =
            Mathf.Max(5, showPanelUI.mackValves.GetComponent<Mack_Valves>().PSI + input);
        psiText.text = showPanelUI.mackValves.GetComponent<Mack_Valves>().PSI + " PSI";
    }

    public void MusicVolumeChange(int input) {
        for (int i = 0; i < showPanelUI.speakerR.Length; i++)
            showPanelUI.speakerR[i].GetComponent<AudioSource>().volume += input * .05f;
        for (int i = 0; i < showPanelUI.speakerL.Length; i++)
            showPanelUI.speakerL[i].GetComponent<AudioSource>().volume += input * .05f;
        volumeText.GetComponent<Text>().text =
            Mathf.Ceil(showPanelUI.speakerR[0].GetComponent<AudioSource>().volume * 100).ToString();
    }


    public void SoundVolumeChange(int input) {
        var gg = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
        var ga = GameObject.Find("GlobalAmbience").GetComponent<AudioSource>();
        gg.volume += input * .05f;
        ga.volume += input * .05f;
        soundvolumeText.GetComponent<Text>().text = Mathf.Ceil(gg.volume * 100).ToString();
    }

    public void SpacialToggle(int input) {
        if (showPanelUI.speakerR[0].GetComponent<AudioSource>().spatialBlend == 0) {
            for (int i = 0; i < showPanelUI.speakerR.Length; i++)
                showPanelUI.speakerR[i].GetComponent<AudioSource>().spatialBlend = 1;
            for (int i = 0; i < showPanelUI.speakerL.Length; i++)
                showPanelUI.speakerL[i].GetComponent<AudioSource>().spatialBlend = 1;
            spacialText.GetComponent<Text>().text = "On";
        }
        else {
            for (int i = 0; i < showPanelUI.speakerR.Length; i++)
                showPanelUI.speakerR[i].GetComponent<AudioSource>().spatialBlend = 0;
            for (int i = 0; i < showPanelUI.speakerL.Length; i++)
                showPanelUI.speakerL[i].GetComponent<AudioSource>().spatialBlend = 0;
            spacialText.GetComponent<Text>().text = "Off";
        }
    }

    public void FlowProfileUpDown(int input) {
        flowNumber = 0;
        flowProfile += input;
        if (flowProfile < 0) flowProfile = 0;
        if (flowProfile > showPanelUI.characters.Length - 1) flowProfile = showPanelUI.characters.Length - 1;
        FlowUpdate();
    }

    public void FlowNumberUpDown(int input) {
        GameObject theCharacter = null;
        foreach (Transform child in showPanelUI.characterHolder.transform)
            if (child.name == showPanelUI.characters[flowProfile].characterName)
                theCharacter = child.gameObject;
        if (theCharacter == null) return;
        flowNumber += input;
        if (flowNumber < 0) flowNumber = 0;
        if (flowNumber > theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().cylBit.Count - 1)
            flowNumber = theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().cylBit.Count - 1;
        FlowUpdate();
    }

    public void FlowCopy() {
        copyPasteValues[0] = float.Parse(flowSpeedInText.text);
        copyPasteValues[1] = float.Parse(flowSpeedOutText.text);
        copyPasteValues[2] = float.Parse(flowWeightInText.text);
        copyPasteValues[3] = float.Parse(flowWeightOutText.text);
        copyPasteValues[4] = float.Parse(flowSlamInText.text);
        copyPasteValues[5] = float.Parse(flowSlamOutText.text);
        copyPasteValues[6] = float.Parse(flowSlamSpeedInText.text);
        copyPasteValues[7] = float.Parse(flowSlamSpeedOutText.text);
    }

    public void FlowPaste() {
        GameObject theCharacter = null;
        foreach (Transform child in showPanelUI.characterHolder.transform)
            if (child.name == showPanelUI.characters[flowProfile].characterName)
                theCharacter = child.gameObject;
        if (theCharacter == null) return;

        var characterValves = theCharacter.transform.GetChild(0).GetComponent<Character_Valves>();
        characterValves.flowControlIn[flowNumber] = copyPasteValues[0];
        characterValves.flowControlOut[flowNumber] = copyPasteValues[1];
        characterValves.gravityScale[flowNumber] = copyPasteValues[2];
        characterValves.gravityScaleOut[flowNumber] = copyPasteValues[3];
        characterValves.smashControlIn[flowNumber] = copyPasteValues[4];
        characterValves.smashControlOut[flowNumber] = copyPasteValues[5];
        characterValves.smashSpeedIn[flowNumber] = copyPasteValues[6];
        characterValves.smashSpeedOut[flowNumber] = copyPasteValues[7];
        FlowUpdate();
    }

    public void FlowUpdate() {
        GameObject theCharacter = null;
        foreach (Transform child in showPanelUI.characterHolder.transform)
            if (child.name == showPanelUI.characters[flowProfile].characterName)
                theCharacter = child.gameObject;
        if (theCharacter == null) return;


        var characterValves = theCharacter.transform.GetChild(0).GetComponent<Character_Valves>();
        flowProfileText.text = showPanelUI.characters[flowProfile].characterName;
        flowNumText.text =
            SearchBitChartName(characterValves.cylBit[flowNumber], characterValves.cylDrawer[flowNumber]);
        flowSpeedInText.text = characterValves.flowControlIn[flowNumber].ToString();
        flowSpeedOutText.text = characterValves.flowControlOut[flowNumber].ToString();
        flowWeightInText.text = characterValves.gravityScale[flowNumber].ToString();
        flowWeightOutText.text = characterValves.gravityScaleOut[flowNumber].ToString();
        flowSlamInText.text = characterValves.smashControlIn[flowNumber].ToString();
        flowSlamOutText.text = characterValves.smashControlOut[flowNumber].ToString();
        flowSlamSpeedInText.text = characterValves.smashSpeedIn[flowNumber].ToString();
        flowSlamSpeedOutText.text = characterValves.smashSpeedOut[flowNumber].ToString();
    }

    public void FlowUpdater(GameObject thetest) {
        string[] paths = new string[1];

        paths[0] = Application.dataPath + "/StreamingAssets/Flows/Default." + FileExtention;

        if (paths.Length > 0)
            if (paths[0] != "") {
                var thefile = flowFormat.ReadFromFile(paths[0]);
                for (int i = 0; i < thefile.characters.Length; i++) {
                    GameObject theCharacter = null;
                    if (thefile.characters[i] != null) {
                        switch (thefile.characters[i].name) {
                            case "Billy Bob":
                                thefile.characters[i].name = "Unknown Mech";
                                break;
                            case "Looney Bird":
                                thefile.characters[i].name = "Pizza Cam";
                                break;
                            case "Rolfe & Earl":
                                thefile.characters[i].name = "Chuck E. Cheese";
                                break;
                            case "Mitzi":
                                thefile.characters[i].name = "Helen Henny";
                                break;
                            case "Sun":
                                thefile.characters[i].name = "Building";
                                break;
                            case "Klunk":
                                thefile.characters[i].name = "Uncle Pappy";
                                break;
                            case "Beach Bear":
                                thefile.characters[i].name = "Jasper T. Jowls";
                                break;
                            case "Fatz":
                                thefile.characters[i].name = "Mr. Munch";
                                break;
                            case "Dook":
                                thefile.characters[i].name = "Pasqually";
                                break;
                        }

                        if (thetest.name == thefile.characters[i].name) {
                            theCharacter = thetest;
                            Debug.Log("Flows: Character found: " + thefile.characters[i].name);
                        }
                    }

                    if (theCharacter != null) {
                        int extra = thefile.characters[i].flowsIn.Length / 2;
                        var cv = theCharacter.transform.GetComponent<Character_Valves>();
                        if (extra < cv.flowControlIn.Length)
                            //Old File Format
                            for (int e = 0; e < cv.flowControlIn.Length; e++) {
                                cv.flowControlIn[e] = thefile.characters[i].flowsIn[e] / 1000f;
                                cv.flowControlOut[e] = thefile.characters[i].flowsOut[e] / 1000f;
                                cv.gravityScale[e] = thefile.characters[i].weightIn[e] / 1000f;
                                cv.gravityScaleOut[e] = thefile.characters[i].weightOut[e] / 1000f;
                            }
                        else
                            //New File Format
                            for (int e = 0; e < extra; e++) {
                                cv.flowControlIn[e] = thefile.characters[i].flowsIn[e] / 1000f;
                                cv.flowControlOut[e] = thefile.characters[i].flowsOut[e] / 1000f;
                                cv.gravityScale[e] = thefile.characters[i].weightIn[e] / 1000f;
                                cv.gravityScaleOut[e] = thefile.characters[i].weightOut[e] / 1000f;
                                Debug.Log(cv.smashControlIn.Length);
                                cv.smashControlIn[e] = thefile.characters[i].flowsIn[e + extra] / 1000f;
                                cv.smashControlOut[e] = thefile.characters[i].flowsOut[e + extra] / 1000f;
                                cv.smashSpeedIn[e] = thefile.characters[i].weightIn[e + extra] / 1000f;
                                cv.smashSpeedOut[e] = thefile.characters[i].weightOut[e + extra] / 1000f;
                            }
                    }
                }
            }
    }

    public string SearchBitChartName(int bit, bool drawer) {
        if (drawer) bit += 150;
        var windowMaker = showPanelUI.GetComponent<UI_WindowMaker>();
        for (int i = 0; i < windowMaker.recordingGroups.Length; i++)
        for (int e = 0; e < windowMaker.recordingGroups[i].inputNames.Length; e++) {
            int finalBitNum = 0;
            if (windowMaker.recordingGroups[i].inputNames[e].drawer) finalBitNum += 150;
            if (windowMaker.recordingGroups[i].inputNames[e].index[0] + finalBitNum == bit)
                return windowMaker.recordingGroups[i].inputNames[e].name;
        }

        return "Nothing";
    }

    public void FlowInSpeedUpDown(int input) {
        GameObject theCharacter = null;
        foreach (Transform child in showPanelUI.characterHolder.transform)
            if (child.name == showPanelUI.characters[flowProfile].characterName)
                theCharacter = child.gameObject;
        if (theCharacter == null) return;
        theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().flowControlIn[flowNumber] = Mathf.Max(0,
            (Mathf.Round(theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().flowControlIn[flowNumber] *
                         100) + input) / 100.00f);
        FlowUpdate();
    }

    public void FlowOutSpeedUpDown(int input) {
        GameObject theCharacter = null;
        foreach (Transform child in showPanelUI.characterHolder.transform)
            if (child.name == showPanelUI.characters[flowProfile].characterName)
                theCharacter = child.gameObject;
        if (theCharacter == null) return;
        theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().flowControlOut[flowNumber] = Mathf.Max(0,
            (Mathf.Round(
                 theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().flowControlOut[flowNumber] * 100) +
             input) / 100.00f);
        FlowUpdate();
    }

    public void FlowInWeightUpDown(int input) {
        GameObject theCharacter = null;
        foreach (Transform child in showPanelUI.characterHolder.transform)
            if (child.name == showPanelUI.characters[flowProfile].characterName)
                theCharacter = child.gameObject;
        if (theCharacter == null) return;
        theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().gravityScale[flowNumber] = Mathf.Max(0,
            (Mathf.Round(theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().gravityScale[flowNumber] *
                         100) + input) / 100.00f);
        FlowUpdate();
    }

    public void FlowOutWeightUpDown(int input) {
        GameObject theCharacter = null;
        foreach (Transform child in showPanelUI.characterHolder.transform)
            if (child.name == showPanelUI.characters[flowProfile].characterName)
                theCharacter = child.gameObject;
        if (theCharacter == null) return;
        theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().gravityScaleOut[flowNumber] = Mathf.Max(0,
            (Mathf.Round(
                 theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().gravityScaleOut[flowNumber] *
                 100) +
             input) / 100.00f);
        ;
        FlowUpdate();
    }

    public void SmashControlIn(int input) {
        GameObject theCharacter = null;
        foreach (Transform child in showPanelUI.characterHolder.transform)
            if (child.name == showPanelUI.characters[flowProfile].characterName)
                theCharacter = child.gameObject;
        if (theCharacter == null) return;
        theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().smashControlIn[flowNumber] = Mathf.Max(0,
            (Mathf.Round(
                 theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().smashControlIn[flowNumber] * 100) +
             input) / 100.00f);
        ;
        FlowUpdate();
    }

    public void SmashControlOut(int input) {
        GameObject theCharacter = null;
        foreach (Transform child in showPanelUI.characterHolder.transform)
            if (child.name == showPanelUI.characters[flowProfile].characterName)
                theCharacter = child.gameObject;
        if (theCharacter == null) return;
        theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().smashControlOut[flowNumber] = Mathf.Max(0,
            (Mathf.Round(
                 theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().smashControlOut[flowNumber] *
                 100) +
             input) / 100.00f);
        ;
        FlowUpdate();
    }

    public void SmashSpeedIn(int input) {
        GameObject theCharacter = null;
        foreach (Transform child in showPanelUI.characterHolder.transform)
            if (child.name == showPanelUI.characters[flowProfile].characterName)
                theCharacter = child.gameObject;
        if (theCharacter == null) return;
        theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().smashSpeedIn[flowNumber] = Mathf.Max(0,
            (Mathf.Round(theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().smashSpeedIn[flowNumber] *
                         100) + input) / 100.00f);
        ;
        FlowUpdate();
    }

    public void SmashSpeedOut(int input) {
        GameObject theCharacter = null;
        foreach (Transform child in showPanelUI.characterHolder.transform)
            if (child.name == showPanelUI.characters[flowProfile].characterName)
                theCharacter = child.gameObject;
        if (theCharacter == null) return;
        theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().smashSpeedOut[flowNumber] = Mathf.Max(0,
            (Mathf.Round(theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().smashSpeedOut[flowNumber] *
                         100) + input) / 100.00f);
        ;
        FlowUpdate();
    }

    public void FlowSave(int input) {
        //Save to file
        Cursor.lockState = CursorLockMode.None;
        string path = StandaloneFileBrowser.SaveFilePanel("Save Flows", "", "MyFlows", FileExtention);
        Cursor.lockState = CursorLockMode.Locked;
        if (!string.IsNullOrEmpty(path)) {
            //Gather Data
            var characterFlows = new flowControls[showPanelUI.characters.Length];
            for (int i = 0; i < characterFlows.Length; i++) {
                GameObject theCharacter = null;
                foreach (Transform child in showPanelUI.characterHolder.transform)
                    if (child.name == showPanelUI.characters[i].characterName)
                        theCharacter = child.gameObject;
                if (theCharacter == null) {
                    Debug.Log("Error Character");
                }
                else {
                    characterFlows[i] = new flowControls();
                    characterFlows[i].name = showPanelUI.characters[i].characterName;
                    var cv = theCharacter.transform.GetChild(0).GetComponent<Character_Valves>();
                    characterFlows[i].flowsIn = new int[cv.flowControlIn.Length * 2];
                    characterFlows[i].flowsOut = new int[cv.gravityScaleOut.Length * 2];
                    characterFlows[i].weightIn = new int[cv.gravityScale.Length * 2];
                    characterFlows[i].weightOut = new int[cv.gravityScaleOut.Length * 2];
                    int extra = characterFlows[i].flowsIn.Length / 2;
                    for (int e = 0; e < extra; e++) {
                        characterFlows[i].flowsIn[e] = Mathf.RoundToInt(cv.flowControlIn[e] * 1000);
                        characterFlows[i].flowsOut[e] = Mathf.RoundToInt(cv.flowControlOut[e] * 1000);
                        characterFlows[i].weightIn[e] = Mathf.RoundToInt(cv.gravityScale[e] * 1000);
                        characterFlows[i].weightOut[e] = Mathf.RoundToInt(cv.gravityScaleOut[e] * 1000);
                        characterFlows[i].flowsIn[e + extra] = Mathf.RoundToInt(cv.smashControlIn[e] * 1000);
                        characterFlows[i].flowsOut[e + extra] = Mathf.RoundToInt(cv.smashControlOut[e] * 1000);
                        characterFlows[i].weightIn[e + extra] = Mathf.RoundToInt(cv.smashSpeedIn[e] * 1000);
                        characterFlows[i].weightOut[e + extra] = Mathf.RoundToInt(cv.smashSpeedOut[e] * 1000);
                    }
                }
            }

            //Save
            var shw = new flowFormat { characters = characterFlows };
            shw.Save(path);
        }
        else {
            Debug.Log("No Save Path");
        }
    }

    public void FlowLoad(int input) {
        StartCoroutine(FlowLoadRoutine(input));
    }

    private IEnumerator FlowLoadRoutine(int input) {
        string[] paths = new string[1];
        if (input == 0) {
            Cursor.lockState = CursorLockMode.None;
            paths = StandaloneFileBrowser.OpenFilePanel("Browse Flow Controls", "", FileExtention, false);
            Cursor.lockState = CursorLockMode.Locked;
        }
        else {
            paths[0] = Application.dataPath + "/StreamingAssets/Flows/Default." + FileExtention;
        }

        if (paths.Length > 0)
            if (paths[0] != "") {
                var thefile = flowFormat.ReadFromFile(paths[0]);
                for (int i = 0; i < thefile.characters.Length; i++) {
                    GameObject theCharacter = null;
                    foreach (Transform child in showPanelUI.characterHolder.transform)
                        if (thefile.characters[i] != null) {
                            switch (thefile.characters[i].name) {
                                case "Billy Bob":
                                    thefile.characters[i].name = "Unknown Mech";
                                    break;
                                case "Looney Bird":
                                    thefile.characters[i].name = "Pizza Cam";
                                    break;
                                case "Rolfe & Earl":
                                    thefile.characters[i].name = "Chuck E. Cheese";
                                    break;
                                case "Mitzi":
                                    thefile.characters[i].name = "Helen Henny";
                                    break;
                                case "Sun":
                                    thefile.characters[i].name = "Building";
                                    break;
                                case "Klunk":
                                    thefile.characters[i].name = "Uncle Pappy";
                                    break;
                                case "Beach Bear":
                                    thefile.characters[i].name = "Jasper T. Jowls";
                                    break;
                                case "Fatz":
                                    thefile.characters[i].name = "Mr. Munch";
                                    break;
                                case "Dook":
                                    thefile.characters[i].name = "Pasqually";
                                    break;
                            }

                            if (child.name == thefile.characters[i].name) {
                                theCharacter = child.gameObject;
                                Debug.Log("Flows: Character found: " + thefile.characters[i].name);
                            }
                        }

                    if (theCharacter == null) {
                        if (thefile.characters[i] != null)
                            Debug.Log("Flows: No Character named " + thefile.characters[i].name);
                        else
                            Debug.Log("Flows: Null Character. Just ignore probably.");
                    }
                    else {
                        int extra = thefile.characters[i].flowsIn.Length / 2;
                        var cv = theCharacter.transform.GetChild(0).GetComponent<Character_Valves>();
                        if (extra < cv.flowControlIn.Length)
                            //Old File Format
                            for (int e = 0; e < cv.flowControlIn.Length; e++) {
                                cv.flowControlIn[e] = thefile.characters[i].flowsIn[e] / 1000f;
                                cv.flowControlOut[e] = thefile.characters[i].flowsOut[e] / 1000f;
                                cv.gravityScale[e] = thefile.characters[i].weightIn[e] / 1000f;
                                cv.gravityScaleOut[e] = thefile.characters[i].weightOut[e] / 1000f;
                            }
                        else
                            //New File Format
                            for (int e = 0; e < extra; e++) {
                                cv.flowControlIn[e] = thefile.characters[i].flowsIn[e] / 1000f;
                                cv.flowControlOut[e] = thefile.characters[i].flowsOut[e] / 1000f;
                                cv.gravityScale[e] = thefile.characters[i].weightIn[e] / 1000f;
                                cv.gravityScaleOut[e] = thefile.characters[i].weightOut[e] / 1000f;
                                cv.smashControlIn[e] = thefile.characters[i].flowsIn[e + extra] / 1000f;
                                cv.smashControlOut[e] = thefile.characters[i].flowsOut[e + extra] / 1000f;
                                cv.smashSpeedIn[e] = thefile.characters[i].weightIn[e + extra] / 1000f;
                                cv.smashSpeedOut[e] = thefile.characters[i].weightOut[e + extra] / 1000f;
                            }
                    }
                }
            }

        FlowUpdate();
        yield return null;
    }
}