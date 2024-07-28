using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour {
    public enum ControllerType {
        keyboard,
        gamepad
    }

    public enum PlayerState {
        normal,
        frozenBody,
        frozenCam,
        frozenAll,
        frozenCamUnlock,
        frozenAllUnlock
    }

    private static readonly int Sprint = Animator.StringToHash("Sprint");

    [Header("Initial")]
    //Attatched Objects
    public Camera PlayerCamScript;

    public TMP_Text playerText;
    public TMP_Text agitText;
    public FAde fadeObj;
    public Image keyboardLayout;

    //Position, Movement, Buttons
    [Tooltip("Initial Camera X position")] public float camXRotation;

    [Tooltip("Initial Camera Y position")] public float camYRotation;

    //Speeds and Base attributes
    [Header("VR")] public bool isVR = false;

    public GameObject XRRig;

    [Header("Speed")] public float baseSpeed = 2f;

    public float crouchSpeed = 1f;
    public float sprintSpeed = 2.5f;

    [Header("Jump")] public bool enableJump;

    public float gravity = 12f;
    public float jumpSpeed = 9f;
    public float airControl = 1;
    public float airTurnSpeed = 1;

    [Header("Crouch")] public bool enableCrouch;

    public float camInitialHeight = 0.657f;
    public float camCrouchHeight = 0f;
    public GameObject feet;
    public GameObject unCrouch;

    [Header("WorldSpace UI")] public bool enableUIClick;

    public GameObject cursor;
    public TMP_Text cursorText;
    public LayerMask uiLayerMask;
    public LayerMask uiCueLayerMask;
    public LayerMask playerLayerMask;

    [Header("Flashlight")] public bool enableFlashlight;

    public GameObject flashlight;
    public GameObject VRflashlight;
    public int flashState;

    [Header("CameraZoom")] public bool enableCamZoom;

    public float maxFov = 170;
    public float minFov = 1;

    [Header("CameraSmooth")] public bool enableCamSmooth;

    public float smoothSpeed;
    public float maxVeclocity;

    [Header("CameraProfiles")] public VolumeProfile lowEffectsVolume;

    public AudioSource footstepSpeaker;
    public FootstepType[] meshTypes;
    public GameObject headBone;
    public GameObject neckBone;
    public GameObject spineBone;

    [Header("PlayerState")] public PlayerState playerState;

    [Header("Player Items")] public CharacterItems[] itemList;

    public string currentItem;
    public float itemWheelCooldown = 0;
    public PlayerInteractions playerInter;
    public ScreenShotItem screenShotItem;
    public PrizeDeleteItem prizeDeleteItem;
    public GameObject pauseMenu;
    public bool canPause = true;
    public float JumpBool;

    [HideInInspector] public Vector2 holdingRotation;

    [Header("Gamepad")] public ControllerType controlType;

    public Vector2 GPJoy;
    public Vector2 GPCam;
    public Vector2 GPZoom;

    [Header("Anims")] private Animator _animator;

    private Vector2 camAcceleration;
    private float camHeight;

    //Other
    private CharacterController CharCont;
    private bool clickGamepad = false;
    private bool crouchBool;
    private bool crouchGamepad = false;
    private Vector2 CStick;
    private bool fixedcamera = false;
    private bool fixedUpdatelowerFPS;
    private bool flashGamepad = false;
    private float flashsmoothScroll = 63;

    //New Input
    [SerializeField] private Controller gamepad;

    private float groundedTimeAnim = 0;
    private bool holdGamepad = false;
    private Vector2 JoyStick;
    private int JumpFrames;
    private bool jumpGamepad = false;
    private Vector3 moveDirection = Vector3.zero;
    private bool newvr = false;

    [Header("Footstep")] private Vector3 oldPosition;

    private bool runGamepad = false;
    private float smoothScroll;
    private float timedelta;

    private void Awake() {
        DontDestroyOnLoad(gameObject);
        camHeight = camInitialHeight;
        smoothScroll = PlayerCamScript.fieldOfView;
        Cursor.lockState = CursorLockMode.Locked;
        //Initialize Variables
        _animator = transform.Find("PlayerModel").GetComponent<Animator>();
        pauseMenu.SetActive(false);
        CharCont = GetComponent<CharacterController>();
        oldPosition = transform.position;
        gamepad = new Controller();
        gamepad.Gamepad.Click.canceled += ctx => clickGamepad = false;
        gamepad.Gamepad.Click.performed += ctx => clickGamepad = true;
        gamepad.Gamepad.Jump.canceled += ctx => jumpGamepad = false;
        gamepad.Gamepad.Jump.performed += ctx => jumpGamepad = true;
        gamepad.Gamepad.Flashlight.canceled += ctx => flashGamepad = false;
        gamepad.Gamepad.Flashlight.started += ctx => flashGamepad = true;
        gamepad.Gamepad.Run.canceled += ctx => runGamepad = false;
        gamepad.Gamepad.Run.performed += ctx => runGamepad = true;
        gamepad.Gamepad.Crouch.canceled += ctx => crouchGamepad = false;
        gamepad.Gamepad.Crouch.performed += ctx => crouchGamepad = true;
        gamepad.Gamepad.Hold.canceled += ctx => holdGamepad = false;
        gamepad.Gamepad.Hold.performed += ctx => holdGamepad = true;
        gamepad.Gamepad.Horizontal.performed += ctx => GPJoy.x = ctx.ReadValue<float>();
        gamepad.Gamepad.Vertical.performed += ctx => GPJoy.y = ctx.ReadValue<float>();
        gamepad.Gamepad.Horizontal.canceled += ctx => GPJoy.x = 0;
        gamepad.Gamepad.Vertical.canceled += ctx => GPJoy.y = 0;
        gamepad.Gamepad.CamHorizontal.performed += ctx => GPCam.x = ctx.ReadValue<float>();
        gamepad.Gamepad.CamVertical.performed += ctx => GPCam.y = ctx.ReadValue<float>();
        gamepad.Gamepad.Zoom.performed += ctx => GPZoom.x = ctx.ReadValue<float>();
        gamepad.Gamepad.FlashZoom.performed += ctx => GPZoom.y = ctx.ReadValue<float>();
        gamepad.Gamepad.Zoom.canceled += ctx => GPZoom.x = 0;
        gamepad.Gamepad.FlashZoom.canceled += ctx => GPZoom.y = 0;
        gamepad.Gamepad.CamHorizontal.canceled += ctx => GPCam.x = 0;
        gamepad.Gamepad.CamVertical.canceled += ctx => GPCam.y = 0;
    }

    private void Update() {
        if (isVR && !fixedcamera)
            if (!PlayerCamScript) {
                fixedcamera = true;
                PlayerCamScript = Camera.main;
            }

        //VR Mess
        if (isVR) {
            var temp = XRRig.transform.position;
            var trans = PlayerCamScript.transform.position - transform.position;
            trans.y = 0;
            transform.position += trans;
            XRRig.transform.position = temp;
            if (!newvr) {
                newvr = true;
                flashlight = VRflashlight;
                jumpSpeed /= 3;
                gravity /= 9;
            }
        }

        //Joystick
        JoyStickCheck();

        //Cam Crouch Code
        PlayerCamScript.transform.localPosition = new Vector3(PlayerCamScript.transform.localPosition.x,
            Mathf.Lerp(PlayerCamScript.transform.localPosition.y, camHeight, Time.deltaTime * 5),
            PlayerCamScript.transform.localPosition.z);

        //Camera Code
        if (playerState != PlayerState.frozenCam && playerState != PlayerState.frozenAll &&
            playerState != PlayerState.frozenAllUnlock && playerState != PlayerState.frozenCamUnlock && !isVR) {
            //Cam Zoom
            if (enableCamZoom) CamZoomCheck();

            bool camMove = true;
            //Items
            switch (currentItem) {
                case "Item Pickup":
                    playerInter.enabled = true;
                    screenShotItem.enabled = false;
                    prizeDeleteItem.enabled = false;
                    camMove = playerInter.PickupCheck(false);
                    break;
                case "Item Freeze":
                    playerInter.enabled = true;
                    screenShotItem.enabled = false;
                    prizeDeleteItem.enabled = false;
                    camMove = playerInter.PickupCheck(true);
                    break;
                case "Screenshot":
                    playerInter.enabled = false;
                    screenShotItem.enabled = true;
                    prizeDeleteItem.enabled = false;
                    screenShotItem.ScreenshotCheck();
                    break;
                case "Item Eraser":
                    playerInter.enabled = false;
                    screenShotItem.enabled = false;
                    prizeDeleteItem.enabled = true;
                    prizeDeleteItem.DeleterCheck();
                    break;
            }

            //Camera
            if (camMove) CameraMove(CStick);
        }

        //Flashlight
        if (enableFlashlight) FlashlightCheck();

        //Body Code
        if (playerState != PlayerState.frozenBody && playerState != PlayerState.frozenAll &&
            playerState != PlayerState.frozenAllUnlock) {
            //Item Wheel
            if (Input.GetKeyDown(KeyCode.Z) && controlType == ControllerType.keyboard) OpenItemWheel();
            if (itemWheelCooldown > 0) itemWheelCooldown -= Time.deltaTime;

            //Pause
            if (Input.GetKeyDown(KeyCode.Escape) && controlType == ControllerType.keyboard && canPause) {
                pauseMenu.SetActive(true);
                playerState = PlayerState.frozenAllUnlock;
            }

            //Jump
            if (!isVR) {
                if (enableJump && ((Input.GetKey(KeyCode.Space) && controlType == ControllerType.keyboard) ||
                                   (jumpGamepad && controlType == ControllerType.gamepad))) {
                    JumpBool++;
                    UncrouchCheck();
                }
                else {
                    JumpBool = 0;
                }
            }

            //Crouch
            if (enableCrouch) CrouchCheck();
            //Footstep
            if (Vector3.Distance(transform.position, oldPosition) > 1.3f && CharCont.isGrounded) FootstepSoundCheck();
            //Move
            MovePlayer(JoyStick, false);
        }
        else {
            //UnPause
            if (Input.GetKeyDown(KeyCode.Escape) && controlType == ControllerType.keyboard)
                if (pauseMenu.activeSelf) {
                    pauseMenu.SetActive(false);
                    playerState = PlayerState.normal;
                    Cursor.lockState = CursorLockMode.Locked;
                }
        }

        switch (Cursor.lockState) {
            case CursorLockMode.None:
                if ((playerState != PlayerState.frozenAllUnlock && playerState != PlayerState.frozenCamUnlock) ||
                    playerState == PlayerState.normal) Cursor.lockState = CursorLockMode.Locked;
                break;
            case CursorLockMode.Locked:
                if (playerState == PlayerState.frozenAllUnlock || playerState == PlayerState.frozenCamUnlock)
                    Cursor.lockState = CursorLockMode.None;
                break;
        }

        //Anim
        AnimCheck();
    }

    private void FixedUpdate() {
        fixedUpdatelowerFPS = !fixedUpdatelowerFPS;

        if (fixedUpdatelowerFPS)
            //UI Click
            if (enableUIClick)
                RayCastClick();
    }


    private void LateUpdate() {
        headBone.transform.rotation = Quaternion.Lerp(headBone.transform.rotation,
            Quaternion.LookRotation(PlayerCamScript.transform.forward, headBone.transform.up), 0.4f);
        neckBone.transform.rotation = Quaternion.Lerp(neckBone.transform.rotation,
            Quaternion.LookRotation(PlayerCamScript.transform.forward, neckBone.transform.up), 0.4f);
        spineBone.transform.rotation = Quaternion.Lerp(spineBone.transform.rotation,
            Quaternion.LookRotation(PlayerCamScript.transform.forward, spineBone.transform.up), 0.2f);
    }

    private void OnEnable() {
        Cursor.lockState = CursorLockMode.Locked;
        gamepad.Gamepad.Enable();
    }

    private void OnDisable() {
        gamepad.Gamepad.Disable();
    }

    private void FootstepSoundCheck() {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity,
                ~playerLayerMask))
            foreach (var t in meshTypes)
                if (hit.collider.gameObject.name == t.meshName) {
                    footstepSpeaker.clip = t.clips[Random.Range(0, t.clips.Length)];
                    footstepSpeaker.volume = t.volume * Random.Range(0.9f, 1.1f);

                    //Volume
                    float nowSpeed = 0.3f;
                    if ((Input.GetKey(KeyCode.LeftShift) && controlType == ControllerType.keyboard) ||
                        (runGamepad && controlType == ControllerType.gamepad))
                        nowSpeed = .7f;
                    if ((Input.GetKey(KeyCode.LeftControl) && enableCrouch && controlType == ControllerType.keyboard) ||
                        (crouchGamepad && enableCrouch && controlType == ControllerType.gamepad))
                        nowSpeed = .2f;
                    footstepSpeaker.volume *= nowSpeed;
                    //footstepSpeaker.pitch = Random.Range(0.9f, 1.1f);
                    footstepSpeaker.Play();
                }

        oldPosition = transform.position;
    }

    private float Remap(float val, float in1, float in2, float out1, float out2) {
        return out1 + (val - in1) * (out2 - out1) / (in2 - in1);
    }

    private float realModulo(float a, float b) {
        return a - b * Mathf.Floor(a / b);
    }

    private void CameraMove(Vector2 axis) {
        if (enableCamSmooth) {
            camAcceleration += axis;
            if (camAcceleration.x > 0)
                camAcceleration.x -= smoothSpeed;
            else if (camAcceleration.x < 0) camAcceleration.x += smoothSpeed;
            if (camAcceleration.x < .1f && camAcceleration.x > -.1f) camAcceleration.x = 0;
            if (camAcceleration.y > 0)
                camAcceleration.y -= smoothSpeed;
            else if (camAcceleration.y < 0) camAcceleration.y += smoothSpeed;
            if (camAcceleration.y < .5f && camAcceleration.y > -.5f) camAcceleration.y = 0;
            camAcceleration.x = Mathf.Max(Mathf.Min(camAcceleration.x, maxVeclocity), -maxVeclocity);
            camAcceleration.y = Mathf.Max(Mathf.Min(camAcceleration.y, maxVeclocity), -maxVeclocity);
            camXRotation += camAcceleration.x / 100f;
            camYRotation += camAcceleration.y / 100f;
        }
        else {
            camXRotation += axis.x;
            camYRotation += axis.y;
        }

        camYRotation = Mathf.Clamp(camYRotation, -85, 85);

        PlayerCamScript.transform.eulerAngles = new Vector3(camYRotation, PlayerCamScript.transform.eulerAngles.y,
            PlayerCamScript.transform.eulerAngles.z);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, camXRotation, transform.eulerAngles.z);
    }

    public void MovePlayer(Vector2 axis, bool sprint) {
        float newy = PlayerCamScript.transform.position.y - transform.position.y;
        CharCont.height = (newy * (1 / camInitialHeight) / 2f + .5f) * 1.3f;
        CharCont.center = new Vector3(0f,
            Remap(newy, feet.transform.localPosition.y, camInitialHeight, feet.transform.localPosition.y, 0), 0);
        var newMoveDir = Vector3.zero;

        //Void Bounce
        if (transform.position.y < -20) {
            transform.position = new Vector3(transform.position.x, 100f, transform.position.z);
            newMoveDir = transform.position;
        }

        float nowSpeed = baseSpeed;
        if ((Input.GetKey(KeyCode.LeftShift) && controlType == ControllerType.keyboard) ||
            (runGamepad && controlType == ControllerType.gamepad) || (sprint && !crouchBool)) {
            nowSpeed = sprintSpeed;
            _animator.SetBool(Sprint, true);
        }
        else {
            _animator.SetBool(Sprint, false);
        }

        if (crouchBool) nowSpeed = crouchSpeed;

        Vector3 transForward;
        Vector3 transRight;

        if (isVR) {
            transForward = PlayerCamScript.transform.forward;
            transRight = PlayerCamScript.transform.right;
            transForward.y = 0;
            transRight.y = 0;
            transForward.Normalize();
            transRight.Normalize();
        }
        else {
            transForward = transform.forward;
            transRight = transform.right;
        }

        if (CharCont.isGrounded) {
            newMoveDir = transForward * (axis.y * nowSpeed) + transRight * (axis.x * nowSpeed);

            //Jumping
            if (JumpBool == 1)
                JumpFrames++;
            else
                JumpFrames = 0;
            if (JumpFrames == 1)
                newMoveDir.y = jumpSpeed;
        }
        else {
            newMoveDir =
                (transForward * (axis.y * nowSpeed) + transRight * (axis.x * nowSpeed * airTurnSpeed)) * airControl +
                new Vector3(0, newMoveDir.y, 0);
        }

        moveDirection = Vector3.Lerp(moveDirection, newMoveDir, 10f * Time.deltaTime);
        moveDirection.y -= gravity * Time.deltaTime;
        CharCont.Move(moveDirection * Time.deltaTime);
    }

    private void RayCastClick() {
        cursor.SetActive(false);
        RaycastHit hit;
        if (Physics.Raycast(PlayerCamScript.transform.position, PlayerCamScript.transform.forward, out hit, 10f,
                uiLayerMask)) {
            var hitcol = hit.collider.GetComponent<Button3D>();
            if (hitcol != null) {
                cursor.SetActive(true);
                cursorText.text = hitcol.buttonText;
                hitcol.Highlight(gameObject.name);

                switch (mouseCheck()) {
                    case true:
                        hitcol.StartClick(gameObject.name);
                        break;
                    case false:
                        hitcol.EndClick(gameObject.name);
                        break;
                }
            }
        }
    }

    private bool mouseCheck() {
        switch (controlType) {
            case ControllerType.keyboard:
                if (Input.GetMouseButton(0)) return true;

                return false;
            case ControllerType.gamepad:
                if (clickGamepad) return true;

                return false;
        }

        return false;
    }

    private void FlashlightCheck() {
        if ((Input.GetKeyDown(KeyCode.F) && controlType == ControllerType.keyboard) ||
            (flashGamepad && controlType == ControllerType.gamepad) || flashState == 1) {
            flashGamepad = false;
            flashlight.SetActive(!flashlight.activeSelf);
            if (flashlight.activeSelf) {
                var sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
                Resources.Load("ting");
                sc.clip = (AudioClip)Resources.Load("Flashlight On");
                sc.pitch = Random.Range(0.95f, 1.05f);
                sc.Play();
            }
            else {
                var sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
                Resources.Load("ting");
                sc.clip = (AudioClip)Resources.Load("Flashlight Off");
                sc.pitch = Random.Range(0.95f, 1.05f);
                sc.Play();
            }
        }

        if (controlType == ControllerType.keyboard) {
            if (Input.GetKey(KeyCode.LeftAlt)) {
                flashsmoothScroll += Input.GetAxis("Mouse ScrollWheel") * 25f;
                flashsmoothScroll = Mathf.Clamp(flashsmoothScroll, 5, 160);
                if (Input.GetAxis("Mouse ScrollWheel") != 0 && flashsmoothScroll > 5 && flashsmoothScroll < 160) {
                    var sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
                    sc.clip = (AudioClip)Resources.Load("Flashlight Click");
                    sc.pitch = .5F + flashsmoothScroll / 320;
                    sc.Play();
                }
            }
        }
        else {
            flashsmoothScroll += GPZoom.y;
            flashsmoothScroll = Mathf.Clamp(flashsmoothScroll, minFov, maxFov);
        }

        flashlight.GetComponent<Light>().spotAngle = Mathf.Lerp(flashlight.GetComponent<Light>().spotAngle,
            flashsmoothScroll, Time.deltaTime * 5);
    }

    private void CrouchCheck() {
        if (CharCont.isGrounded && !(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.Print))) {
            bool isKeyboard = controlType == ControllerType.keyboard;
            bool isGamepad = controlType == ControllerType.gamepad;

            // TODO: CHECKME: Crouching might be broken on Gamepad
            if ((isKeyboard && Input.GetKeyDown(KeyCode.LeftControl)) || (isGamepad && crouchGamepad != crouchBool)) {
                crouchBool = true;
                timedelta = 0;
            }

            if ((isKeyboard && (Input.GetKeyUp(KeyCode.LeftControl) || !Input.GetKey(KeyCode.LeftControl))) ||
                (isGamepad && crouchGamepad != crouchBool)) {
                UncrouchCheck();
                timedelta = 0;
            }
        }
        else {
            timedelta += Time.deltaTime;
            crouchBool = false;
        }

        _animator.SetBool("Crouch", crouchBool);
        //Crouch height
        if (!crouchBool)
            camHeight = camInitialHeight;
        else
            camHeight = camCrouchHeight;
    }

    private void UncrouchCheck() {
        crouchBool = false;
        if (Physics.Raycast(unCrouch.transform.position, transform.TransformDirection(Vector3.up), out var hit,
                Mathf.Infinity))
            if (hit.point.y < PlayerCamScript.transform.position.y + camInitialHeight)
                crouchBool = true;
    }

    private void JoyStickCheck() {
        CStick = Vector2.zero;
        JoyStick = Vector2.zero;
        switch (controlType) {
            case ControllerType.keyboard:
                if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) JoyStick.y += 1.0f;
                if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) JoyStick.x -= 1.0f;
                if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) JoyStick.y -= 1.0f;
                if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) JoyStick.x += 1.0f;
                CStick.x = Input.GetAxis("Mouse X") * 1.5f;
                CStick.y = Input.GetAxis("Mouse Y") * -1.5f;
                break;
            case ControllerType.gamepad:
                JoyStick = GPJoy;
                CStick = GPCam * 2;
                break;
        }

        //Anims
        _animator.SetFloat("Velocity Z", Mathf.Min(JoyStick.y * 10, 1), 0.1f, Time.deltaTime);
        _animator.SetFloat("Velocity X", Mathf.Min(JoyStick.x * 10, 1), 0.1f, Time.deltaTime);
        JoyStick = JoyStick.normalized;
    }

    private void AnimCheck() {
        if (!CharCont.isGrounded)
            groundedTimeAnim = 0;
        else
            groundedTimeAnim += Time.deltaTime;
        if (groundedTimeAnim > 0.1f)
            _animator.SetBool("Grounded", true);
        else
            _animator.SetBool("Grounded", false);
    }

    private void CamZoomCheck() {
        if (controlType == ControllerType.keyboard) {
            if (!Input.GetKey(KeyCode.LeftAlt)) {
                if (Input.GetAxis("Mouse ScrollWheel") != 0)
                    timedelta = 0;
                else
                    timedelta += Time.deltaTime;
                smoothScroll += Input.GetAxis("Mouse ScrollWheel") * 25f;
                smoothScroll = Mathf.Clamp(smoothScroll, minFov, maxFov);
            }
        }
        else {
            smoothScroll += GPZoom.x;
            smoothScroll = Mathf.Clamp(smoothScroll, minFov, maxFov);
        }

        PlayerCamScript.fieldOfView = Mathf.Lerp(PlayerCamScript.fieldOfView, smoothScroll, Time.deltaTime * 5);
    }

    public void DisplayMessage(Texture2D image, byte todarkness, byte tospeed) {
        SetFade(todarkness, tospeed);
        var sm = transform.Find("Player UI").Find("System Message").gameObject;
        sm.SetActive(true);
        sm.transform.Find("Message").GetComponent<RawImage>().texture = image;
        sm.GetComponent<Animator>().Play("SystemMessage");
    }

    public void OpenItemWheel() {
        var iw = transform.Find("Player UI").Find("Item Wheel").gameObject;
        if (iw.activeSelf) {
            AdvanceItemWheel();
        }
        else {
            List<string> theItemList;
            int currItem = LoadItemWheelItems(out theItemList);
            if (theItemList.Count > 1) {
                iw.SetActive(true);
                iw.GetComponent<Animator>().Play("New State", 0, 0);
                iw.transform.Find("Middle").GetComponent<Image>().sprite = itemNameToIcon(theItemList[currItem]);
                iw.transform.Find("Bottom").GetComponent<Image>().sprite =
                    itemNameToIcon(theItemList[(int)realModulo(currItem + 1, theItemList.Count)]);
                iw.transform.Find("Top").GetComponent<Image>().sprite =
                    itemNameToIcon(theItemList[(int)realModulo(currItem - 1, theItemList.Count)]);
                iw.transform.Find("Name").GetComponent<Text>().text = theItemList[currItem];
            }
        }
    }

    private void AdvanceItemWheel() {
        var iw = transform.Find("Player UI").Find("Item Wheel").gameObject;
        if (itemWheelCooldown <= 0) {
            itemWheelCooldown = 0.5f;
            List<string> theItemList;
            int currItem = LoadItemWheelItems(out theItemList);
            if (theItemList.Count > 1) {
                var sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
                sc.clip = (AudioClip)Resources.Load("ItemWheel");
                sc.Play();
                iw.SetActive(true);
                currentItem = theItemList[(int)realModulo(currItem + 1, theItemList.Count)];
                iw.GetComponent<Animator>().Play("ItemWheel", 0, 0);
            }
        }
    }

    public void GatherItemWheelIcons() {
        List<string> theItemList;
        int currItem = LoadItemWheelItems(out theItemList);
        if (theItemList.Count > 1) {
            var iw = transform.Find("Player UI").Find("Item Wheel").gameObject;
            iw.transform.Find("Middle").GetComponent<Image>().sprite = itemNameToIcon(theItemList[currItem]);
            iw.transform.Find("Bottom").GetComponent<Image>().sprite =
                itemNameToIcon(theItemList[(int)realModulo(currItem + 1, theItemList.Count)]);
            iw.transform.Find("Top").GetComponent<Image>().sprite =
                itemNameToIcon(theItemList[(int)realModulo(currItem - 1, theItemList.Count)]);
            iw.transform.Find("Name").GetComponent<Text>().text = theItemList[currItem];
        }
    }

    private Sprite itemNameToIcon(string name) {
        for (int i = 0; i < itemList.Length; i++)
            if (itemList[i].itemName == name)
                return itemList[i].icon;
        return null;
    }

    private int LoadItemWheelItems(out List<string> list) {
        list = new List<string>();
        int currItem = 0;
        for (int i = 0; i < itemList.Length; i++)
            if (itemList[i].unlockString == "")
                list.Add(itemList[i].itemName);
            else if (PlayerPrefs.GetInt(itemList[i].unlockString) == 1) list.Add(itemList[i].itemName);
        for (int i = 0; i < list.Count; i++)
            if (list[i] == currentItem)
                currItem = i;
        return currItem;
    }

    public void FromUiToNormal(int tospeed) {
        playerState = PlayerState.normal;
        SetFade(0, (byte)tospeed);
        var sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
        sc.clip = (AudioClip)Resources.Load("SystemClose");
        sc.Play();
    }

    public void SetFade(byte todarkness, byte tospeed) {
        fadeObj.fadeSpeed = tospeed;
        fadeObj.fadeTo = todarkness;
    }
}


[Serializable]
public class FootstepType {
    public string meshName;
    public AudioClip[] clips;

    [Range(0.0f, 1.0f)] public float volume;
}

[Serializable]
public class CharacterItems {
    public string itemName;
    public Sprite icon;
    public string unlockString;
}