using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(BoxCollider))]
public class Button3D : MonoBehaviour {
    public enum uisound {
        tap,
        bigTap,
        ting,
        help,
        buy,
        none,
        systemOpen,
        systemClose,
        deny,
        create,
        unboxCrate,
        sitDown
    }

    [FormerlySerializedAs("ui")] [CanBeNull] public GameObject controller;
    public string funcName;
    public int funcWindow;
    public bool sendPlayerNum = false;
    public float clickTime = 0;
    public string buttonText;

    public bool sendToTutorial;
    public string tutorialSend;
    public uisound uiSound = uisound.tap;
    public bool ignoreCollider = false;
    private bool click;
    private bool highlighted;
    private float highlightime;
    private RectTransform localrect;
    private AudioSource sc;
    private TutorialManager tut;

    private void Awake() {
        sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
        // this.DontAllowNullYouDumbass(controller);
        
        if (sendToTutorial) {
            var tt = GameObject.Find("Tutorial");
            if (tt != null) tut = tt.GetComponent<TutorialManager>();
        }

        if (!ignoreCollider) {
            GetComponent<BoxCollider>().size = new Vector3(GetComponent<RectTransform>().sizeDelta.x,
                GetComponent<RectTransform>().sizeDelta.y, 0f);
            localrect = GetComponent<RectTransform>();
        }
    }

    // Update is called once per frame
    private void Update() {
        if (click) clickTime += Time.deltaTime;
        if (clickTime > 0.2f) {
            clickTime = 0;
            click = false;
        }

        if (highlighted)
            highlightime += 0.1f;
        else
            highlightime -= 0.1f;
        highlighted = false;
        highlightime = Mathf.Clamp(highlightime, 0, 1);
        if (!ignoreCollider)
            localrect.localScale = new Vector3(Mathf.Max(0, 1.0f - highlightime / 20f),
                Mathf.Max(0, 1.0f - highlightime / 20f), 1);
    }

    private void OnDisable() {
        click = false;
    }

    public void Highlight(string name) {
        highlighted = true;
    }

    public void StartClick(string name) {
        click = true;
    }

    public void EndClick(string name) {
        if (click)
            if (funcName != "") {
                if (sc == null) sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
                switch (uiSound) {
                    case uisound.tap:
                        sc.clip = (AudioClip)Resources.Load("tap");
                        sc.pitch = Random.Range(0.95f, 1.05f);
                        break;
                    case uisound.bigTap:
                        sc.clip = (AudioClip)Resources.Load("big tap");
                        sc.pitch = Random.Range(0.98f, 1.02f);
                        break;
                    case uisound.ting:
                        sc.clip = (AudioClip)Resources.Load("ting");
                        sc.pitch = Random.Range(0.95f, 1.05f);
                        break;
                    case uisound.help:
                        sc.clip = (AudioClip)Resources.Load("help");
                        sc.pitch = 1.0f;
                        break;
                    case uisound.buy:
                        sc.clip = (AudioClip)Resources.Load("Purchase");
                        sc.pitch = 1.0f;
                        break;
                    case uisound.none:
                        sc.clip = null;
                        break;
                    case uisound.systemOpen:
                        sc.clip = (AudioClip)Resources.Load("SystemOpen");
                        sc.pitch = 1.0f;
                        break;
                    case uisound.systemClose:
                        sc.clip = (AudioClip)Resources.Load("SystemClose");
                        sc.pitch = 1.0f;
                        break;
                    case uisound.deny:
                        sc.clip = (AudioClip)Resources.Load("Deny");
                        sc.pitch = 1.0f;
                        break;
                    case uisound.create:
                        sc.clip = (AudioClip)Resources.Load("Create");
                        sc.pitch = 1.0f;
                        break;
                    case uisound.unboxCrate:
                        sc.clip = (AudioClip)Resources.Load("Crate Unbox");
                        sc.pitch = 1.0f;
                        break;
                    case uisound.sitDown:
                        sc.clip = (AudioClip)Resources.Load("Sit Down");
                        sc.pitch = 1.0f;
                        break;
                }

                sc.Play();
                click = false;

                int finalsend = funcWindow;

                if (sendPlayerNum) finalsend = name == "Player" ? 0 : 1;

                if (controller) {
                    if (sendToTutorial && tut) {
                        if (tut.AttemptAdvanceTutorial(tutorialSend)) controller.SendMessage(funcName, finalsend);
                    }
                    else {
                        controller.SendMessage(funcName, finalsend);
                    }
                } else {
                    Debug.LogWarning($"Field {nameof(controller)} inside {name}/{nameof(Button3D)} is null");
                }
            }
    }
}