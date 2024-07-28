
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour {
    public AudioSource globalAudio;
    public AudioSource music;
    
    public GameObject starlow;
    public GameObject starmed;
    public GameObject starhigh;
    public GameObject starultra;

    // Settings text
    public TextMeshProUGUI vsyncText;
    public TextMeshProUGUI textureText;
    public TextMeshProUGUI resText;
    public TextMeshProUGUI motionText;
    public TextMeshProUGUI exposureText;
    public TextMeshProUGUI versionText;
    public TextMeshProUGUI ssrText;
    public TextMeshProUGUI ssaoText;
    public TextMeshProUGUI timeofdayText;
    public TextMeshProUGUI resPercentText;
    public TextMeshProUGUI dlssText;
    public TextMeshProUGUI graphicsApiText;
    public TextMeshProUGUI brandCosmeticStyleText;

    // Settings widgets
    public GameObject settingW;
    public GameObject faqW;
    public GameObject creditsW;
    public GameObject characterW;
    public GameObject characterWScene;
    public GameObject downButton;

    public GameObject bottomBar;
    public GameObject tutorialAskPopup;
    public GameObject showtimeButton;
    public Animator logoAnim;
    public GameObject logo;
    public GameObject logoTwo;

    public Button charcustomBtn;
    public Button editorBtn;
    public Button sandboxBtn;

    CanvasGroup settingG;
    CanvasGroup faqG;
    CanvasGroup creditsG;
    CanvasGroup characterG;

    public RawImage fade;
    public RawImage animatronicImg;

    private bool barUp;
    public BarMove[] bars;
    public Image barGrid;  // reminds me of a large bearded wizard - Floof

    public VolumeProfile regularVolume;

    public bool stopUpdate;
    public bool deletePlayerPrefsAll;
    public GameObject player;

    public bool disableRaytracing;
    public bool fadeWhichWay = false;
    public string sceneLoadCache;

    private bool isHalloween;
    private Animator downButtonAnimator;

    private void Start()
    {
        TextureXR.maxViews = 2;
        if (!stopUpdate)
        {
            if (PlayerPrefs.GetInt("Tutorial Save 0") != 0 || InternalGameVersion.gameName != "Faz-Anim")
            {
                charcustomBtn.interactable = true;
                editorBtn.interactable = true;
                sandboxBtn.interactable = true;
            }
            settingG = settingW.GetComponent<CanvasGroup>();
            faqG = faqW.GetComponent<CanvasGroup>();
            creditsG = creditsW.GetComponent<CanvasGroup>();
            characterG = characterW.GetComponent<CanvasGroup>();
            versionText.text = "Ver. " + InternalGameVersion.gameVersion;
            downButtonAnimator = downButton.GetComponent<Animator>();
            
            barGrid.color = barGrid.color.WithAlpha(0f);
        }
        if (disableRaytracing)
        {
            if (PlayerPrefs.GetInt("Settings: SSR") >= 5)
            {
                PlayerPrefs.SetInt("Settings: SSR", 4);
            }
            if (PlayerPrefs.GetInt("Settings: SSAO") >= 3)
            {
                PlayerPrefs.SetInt("Settings: SSAO", 1);
            }
        }
        QualitySave.FirstTimeSave();
        UpdateSettings();
        if (InternalGameVersion.isVR == "true")
        {
            Debug.Log("IsVR is True.");
            StartShow(false);
        }
        else
        {
            Debug.Log("IsVR is False.");
            fadeWhichWay = false;
        }
        //SceneManager.LoadScene("Arcade Mr Hugs", LoadSceneMode.Single);
        
        // Halloween check
        var date = DateTime.Now;
        var targetDate = new DateTime(date.Year, 10, 31, 12, 0, 0);
        isHalloween = Math.Abs((targetDate - date).TotalDays) <= 3.0d;
        
        // Playing the cog animation and the music in sync
        // + Spoopy easter-egg where the cog stops spinning during Halloween
        if (!isHalloween) {
            logoAnim.Play("Cog animation");
            music.Play();
        }
    }

    private void Update()
    {
        if (fadeWhichWay)
            fade.color = new Color(1, 1, 1, Mathf.Min(1,fade.color.a + (2 * Time.deltaTime)));
        else
            fade.color = new Color(1, 1, 1, Mathf.Max(0,fade.color.a - (.4f * Time.deltaTime)));

        if (tutorialAskPopup && tutorialAskPopup.activeSelf && animatronicImg) {
            animatronicImg.color = Color.Lerp(animatronicImg.color, Color.clear, Time.deltaTime * 2.5f);
            animatronicImg.uvRect = new Rect(
                Mathf.InverseLerp(animatronicImg.uvRect.x, 1.1f, Time.deltaTime * 3f),
                animatronicImg.uvRect.y,
                1, 1
            );
        }
    }

    void FixedUpdate()
    {
        if(sceneLoadCache != "")
        {
            SceneLoadCheck();
        }
        if (deletePlayerPrefsAll)
        {
            deletePlayerPrefsAll = false;
            PlayerPrefs.DeleteAll();
        }
        if (!stopUpdate)
        {
            Destroy(InternalGameVersion.gameName == "Faz-Anim" ? logoTwo : logo);
            if (barUp)
            {
                settingG.alpha += .1f;
                faqG.alpha += .1f;
                creditsG.alpha += .1f;
                characterG.alpha += .1f;
                barGrid.color = Color.Lerp(barGrid.color, barGrid.color.WithAlpha(0.25f), 0.08f);
            }
            else
            {
                settingG.alpha -= .15f;
                faqG.alpha -= .15f;
                creditsG.alpha -= .15f;
                characterG.alpha -= .15f;
                barGrid.color = Color.Lerp(barGrid.color, barGrid.color.WithAlpha(0f), 0.08f);
            }
            if (settingG.alpha <= 0)
            {
                settingW.SetActive(false);
            }
            if (faqG.alpha <= 0)
            {
                faqW.SetActive(false);
            }
            if (creditsG.alpha <= 0)
            {
                creditsW.SetActive(false);
            }
            if (characterG.alpha <= 0)
            {
                characterW.SetActive(false);
                characterWScene.SetActive(false);
            }
        }
    }

    public void StartEditor()
    {
        if (sceneLoadCache == "")
        {
            globalAudio.clip = (AudioClip)Resources.Load("big tap");
            globalAudio.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
            globalAudio.Play();
            sceneLoadCache = "Bit Crusher";
            fadeWhichWay = true;
        }
    }
    public void StartShow(bool sandbox) {
        if (sceneLoadCache == "") {
            globalAudio.clip = (AudioClip)Resources.Load("big tap");
            globalAudio.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
            globalAudio.Play();
            if (!sandbox) {
                if (tutorialAskPopup) {
                    barUp = true;
                    bottomBar.SetActive(false);
                    foreach (BarMove t in bars) {
                        t.transition = true;
                    }
                    tutorialAskPopup.SetActive(true);
                } else {
                    Debug.LogError($"NO {nameof(tutorialAskPopup)} FOUND");
                }
            } else {
                sceneLoadCache = "Sandbox";
            }
            fadeWhichWay = true;
        }
    }

    void SceneLoadCheck()
    {
        if(fadeWhichWay && fade.color.a >= 1)
        {
            SceneManager.LoadScene(sceneLoadCache, LoadSceneMode.Single);
        }
    }

    public void SettingsMenu()
    {
        MenuFunc();
        creditsW.SetActive(false);
        faqW.SetActive(false);
        settingW.SetActive(true);
        characterW.SetActive(false);
        characterWScene.SetActive(false);
        showtimeButton.SetActive(false);
    }
    public void FAQMenu()
    {
        MenuFunc();
        creditsW.SetActive(false);
        faqW.SetActive(true);
        settingW.SetActive(false);
        characterW.SetActive(false);
        characterW.SetActive(false);
        showtimeButton.SetActive(false);
    }
    public void CreditsMenu()
    {
        MenuFunc();
        creditsW.SetActive(true);
        faqW.SetActive(false);
        settingW.SetActive(false);
        characterW.SetActive(false);
        characterWScene.SetActive(false);
        showtimeButton.SetActive(false);
    }
    public void CharacterMenu()
    {
        MenuFunc();
        creditsW.SetActive(false);
        faqW.SetActive(false);
        settingW.SetActive(false);
        characterW.SetActive(true);
        characterWScene.SetActive(true);
        showtimeButton.SetActive(false);
    }

    public void MenuFunc()
    {
        globalAudio.clip = (AudioClip)Resources.Load("big tap");
        globalAudio.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
        globalAudio.Play();
        barUp = true;
        downButton.SetActive(true);
        downButtonAnimator.SetTrigger("Normal");
        downButtonAnimator.Play("Normal");
        foreach (var bar in bars) {
            bar.transition = true;
        }
    }

    public void Down()
    {
        globalAudio.clip = (AudioClip)Resources.Load("tap");
        globalAudio.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
        globalAudio.Play();
        downButtonAnimator.SetTrigger("Pressed");
        barUp = false;
        foreach (var bar in bars) {
            bar.transition = false;
        }
        showtimeButton.SetActive(true);
    }

    public void TitleScreenMenu()
    {
        if (sceneLoadCache == "")
        {
            globalAudio.clip = (AudioClip)Resources.Load("tap");
            globalAudio.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
            globalAudio.Play();
            if (PlayerPrefs.GetInt("Intro: TutorialA") == 0)
            {
                ////////////////////////////////////////////////SceneManager.LoadScene("Tutorial", LoadSceneMode.Single);
                sceneLoadCache = "Title Screen";
                fadeWhichWay = true;
            }
            else
            {
                sceneLoadCache = "Title Screen";
                fadeWhichWay = true;
            }
        }
    }

    public void Exit()
    {
        globalAudio.clip = (AudioClip)Resources.Load("big tap");
        globalAudio.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
        globalAudio.Play();
        Application.Quit();
    }

    public void SetQuality(int quality)
    {
        PlayFeedbackSound("ting");
        PlayerPrefs.SetInt("Settings: Quality", quality);
        QualitySave.ApplySavedQualitySettings();
        UpdateSettings();
    }
    public void SetSSR(int offset)
    {
        PlayFeedbackSound("ting");
        int check = PlayerPrefs.GetInt("Settings: SSR") + offset;
        if (check < 0)
            check = 0;
        if (check > 6 && !disableRaytracing)
            check = 6;
        if (check > 4 && disableRaytracing)
            check = 4;
        PlayerPrefs.SetInt("Settings: SSR", check);
        UpdateSettings();
    }
    public void SetSSAO(int offset)
    {
        PlayFeedbackSound("ting");
        int check = PlayerPrefs.GetInt("Settings: SSAO") + offset;
        if (check < 0)
            check = 0;
        if (check > 5 && !disableRaytracing)
            check = 5;
        if (check > 2 && disableRaytracing)
            check = 2;
        PlayerPrefs.SetInt("Settings: SSAO", check);
        UpdateSettings();
    }
    
    public void SetTextureQ(int offset)
    {
        int check = PlayerPrefs.GetInt("Settings: Texture") + offset;
        switch (check) {
            case < 0:
                check = 0;
                break;
            case > 3:
                check = 3;
                break;
            default:
                PlayFeedbackSound("ting");
                break;
        }
        PlayerPrefs.SetInt("Settings: Texture", check);
        UpdateSettings();
    }

    public void SetWindowed(int quality)
    {
        PlayFeedbackSound("ting");
        PlayerPrefs.SetInt("Settings: Windowed", quality);
        UpdateSettings();
    }

    public void SetMotionBlur(int onoff)
    {
        PlayFeedbackSound("ting");
        PlayerPrefs.SetInt("Settings: Motion Blur", onoff);
        UpdateSettings();
    }

    public void SetAutoExposure(int onoff)
    {
        PlayFeedbackSound("ting");
        PlayerPrefs.SetInt("Settings: Auto Exposure", onoff);
        UpdateSettings();
    }

    public void SetPlaybackRate(int offset)
    {
        int check = PlayerPrefs.GetInt("Settings: Playback") + offset;
        switch (check) {
            case < 0:
                check = 0;
                break;
            case > 1:
                check = 1;
                break;
            default:
                PlayFeedbackSound("ting");
                break;
        }
        PlayerPrefs.SetInt("Settings: Playback", check);
        UpdateSettings();
    }
    public void SetTimeOfDay(int offset)
    {
        int check = PlayerPrefs.GetInt("Settings: Time of Day") + offset;
        switch (check) {
            case < 0:
                check = 0;
                break;
            case > 4:
                check = 4;
                break;
            default:
                PlayFeedbackSound("ting");
                break;
        }
        PlayerPrefs.SetInt("Settings: Time of Day", check);
        UpdateSettings();
    }
    public void SetBrandingStyle(int offset) {
        int check = PlayerPrefs.GetInt("Settings: Branding Style") + offset;
        if (check < BrandingStyle.minValue)
            check = BrandingStyle.maxValue;
        else if (check > BrandingStyle.maxValue)
            check = BrandingStyle.minValue;
        
        PlayerPrefs.SetInt("Settings: Branding Style", check);
        PlayFeedbackSound("ting");
        UpdateSettings();
    }
    public void SetVsync(int onOff)
    {
        PlayFeedbackSound("ting");
        PlayerPrefs.SetInt("Settings: VSync", onOff);
        UpdateSettings();
    }
    public void SetResPercent(int quality)
    {
        int check = PlayerPrefs.GetInt("Settings: Res Percent") + quality;
        switch (check) {
            case < 0:
                check = 0;
                break;
            case > 18:
                check = 18;
                break;
            default:
                PlayFeedbackSound("ting");
                break;
        }
        PlayerPrefs.SetInt("Settings: Res Percent", check);
        UpdateSettings();
    }
    public void SetDLSS(int quality)
    {
        int check = PlayerPrefs.GetInt("Settings: DLSS") + quality;
        switch (check) {
            case < 0:
                check = 0;
                break;
            case > 4:
                check = 4;
                break;
            default:
                PlayFeedbackSound("ting");
                break;
        }
        PlayerPrefs.SetInt("Settings: DLSS", check);
        UpdateSettings();
    }

    public void PlayFeedbackSound(string sound) {
        globalAudio.clip = (AudioClip) Resources.Load(sound);
        globalAudio.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
        globalAudio.Play();
    }
    
    /** Called when the player updates the settings somehow */
    void UpdateSettings() {
        QualitySave.ApplySavedQualitySettings();
        if (!stopUpdate)
        {
            if (vsyncText != null)
            {
                vsyncText.text = QualitySettings.vSyncCount == 0 ? "Off" : "On";
                starlow.SetActive(false);
                starmed.SetActive(false);
                starhigh.SetActive(false);
                starultra.SetActive(false);
                switch (PlayerPrefs.GetInt("Settings: Quality"))
                {
                    case 0:
                        starlow.SetActive(true);
                        break;
                    case 1:
                        starmed.SetActive(true);
                        break;
                    case 2:
                        starhigh.SetActive(true);
                        break;
                    case 3:
                        starultra.SetActive(true);
                        break;
                    default:
                        break;
                }
                switch (PlayerPrefs.GetInt("Settings: Texture"))
                {
                    case 0:
                        textureText.text = "Very Low";
                        break;
                    case 1:
                        textureText.text = "Low";
                        break;
                    case 2:
                        textureText.text = "Medium";
                        break;
                    case 3:
                        textureText.text = "High";
                        break;
                    default:
                        break;
                }
                switch (PlayerPrefs.GetInt("Settings: Windowed"))
                {

                    case 0:
                        resText.text = "Fullscreen";
                        break;
                    case 1:
                        resText.text = "Windowed";
                        break;
                    default:
                        break;
                }
                switch (PlayerPrefs.GetInt("Settings: Motion Blur"))
                {
                    case 0:
                        motionText.text = "Off";
                        break;
                    case 1:
                        motionText.text = "On";
                        break;
                    default:
                        break;
                }
                switch (PlayerPrefs.GetInt("Settings: Auto Exposure"))
                {
                    case 0:
                        exposureText.text = "Off";
                        break;
                    case 1:
                        exposureText.text = "On";
                        break;
                    default:
                        break;
                }
                switch (PlayerPrefs.GetInt("Settings: SSR"))
                {
                    case 0:
                        ssrText.text = "Off";
                        break;
                    case 1:
                        ssrText.text = "SSR Low";
                        break;
                    case 2:
                        ssrText.text = "SSR Medium";
                        break;
                    case 3:
                        ssrText.text = "SSR High";
                        break;
                    case 4:
                        ssrText.text = "SSR Ultra";
                        break;
                    case 5:
                        ssrText.text = "RT Performance";
                        break;
                    case 6:
                        ssrText.text = "RT Quality";
                        break;
                    default:
                        break;
                }
                switch (PlayerPrefs.GetInt("Settings: SSAO"))
                {
                    case 0:
                        ssaoText.text = "Off";
                        break;
                    case 1:
                        ssaoText.text = "SS AO";
                        break;
                    case 2:
                        ssaoText.text = "(WIP) SS GI";
                        break;
                    case 3:
                        ssaoText.text = "(WIP) RT AO";
                        break;
                    case 4:
                        ssaoText.text = "(WIP) RT GI Performance";
                        break;
                    case 5:
                        ssaoText.text = "(WIP) RT GI Quality";
                        break;
                    default:
                        break;
                }
                switch (PlayerPrefs.GetInt("Settings: Time of Day"))
                {
                    case 0:
                        timeofdayText.text = "PC Clock";
                        break;
                    case 1:
                        timeofdayText.text = "Day";
                        break;
                    case 2:
                        timeofdayText.text = "Night";
                        break;
                    case 3:
                        timeofdayText.text = "Rainy";
                        break;
                    case 4:
                        timeofdayText.text = "Sunset";
                        break;
                    default:
                        break;  
                }
                resPercentText.text = (100 - (PlayerPrefs.GetInt("Settings: Res Percent") * 5)).ToString() + '%';
                switch (PlayerPrefs.GetInt("Settings: DLSS"))
                {
                    case 0:
                        dlssText.text = "Off";
                        break;
                    case 1:
                        dlssText.text = "Ultra Performance";
                        break;
                    case 2:
                        dlssText.text = "Max Performance";
                        break;
                    case 3:
                        dlssText.text = "Balanced";
                        break;
                    case 4:
                        dlssText.text = "Quality";
                        break;
                    default:
                        break;
                }
            }

            switch ((BrandingStyle.Enum) PlayerPrefs.GetInt("Settings: Branding Style")) {
                case BrandingStyle.Enum.Classic:
                    brandCosmeticStyleText.text = "Faz-Anim";
                    break;
                case BrandingStyle.Enum.RockAfire:
                    brandCosmeticStyleText.text = "Rock-Afire";
                    break;
                default:
                    break;
            }
        }
    }
}