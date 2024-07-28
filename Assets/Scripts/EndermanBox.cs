using TMPro;
using UnityEngine;

public class EndermanBox : MonoBehaviour {
    private const string SmallestPositionString = "1:P(0 0 0):R(0 0)";

    [Header("Input boxes (position)")] public TMP_InputField xPosField;

    public TMP_InputField yPosField;
    public TMP_InputField zPosField;

    [Header("Input boxes (rotation)")] public TMP_InputField xRotField;

    public TMP_InputField yRotField;

    [Header("Misc")] public Player player;

    private CharacterController playerController;

    [Header("Read-only")] public string version { get; } = "1";

    public Vector3 newPosition { get; private set; }
    public Vector2 newRotation { get; private set; }

    private void Start() {
        if (player == null) {
            player = GameObject.FindWithTag("Player").GetComponent<Player>();
            Debug.LogError(
                $"Player was null inside {nameof(EndermanBox)}!\nIt was fixed at runtime, but it should be fixed in-editor!");
        }

        playerController = player.GetComponent<CharacterController>();
        UpdateTextFields();
    }

    private void FixedUpdate() {
        if (playerController && playerController.velocity.magnitude > 0f) UpdateTextFields();
    }

    private void UpdateTextFields() {
        // Position
        xPosField.text = NumDisplay(player.transform.position.x);
        yPosField.text = NumDisplay(player.transform.position.y);
        zPosField.text = NumDisplay(player.transform.position.z);

        // Rotation
        xRotField.text = NumDisplay(player.camXRotation); // Head left/right
        yRotField.text = NumDisplay(player.transform.rotation.y);
    }

    private void UpdateInternalData() {
        newPosition = new Vector3(float.Parse(xPosField.text), float.Parse(yPosField.text),
            float.Parse(zPosField.text));
        newRotation = new Vector2(float.Parse(xRotField.text), float.Parse(yRotField.text));
    }

    public void Copy() {
        UpdateTextFields();
        GUIUtility.systemCopyBuffer =
            $"{version}:P({xPosField.text} {yPosField.text} {zPosField.text}):R({xRotField.text} {yRotField.text})";
    }

    public void Paste() {
        string input = GUIUtility.systemCopyBuffer.Trim().ToLower();
        string[] tokens = input.Split(':');

        foreach (string t in tokens) {
            if (t.Length < SmallestPositionString.Length) continue;
            string token = t.Replace(":", "").Trim();

            switch (token[0]) {
                case 'p': {
                    string[] posStr = token.Substring(2, token.Length - 3).Split(' ', 3);
                    xPosField.text = posStr[0];
                    yPosField.text = posStr[1];
                    zPosField.text = posStr[2];
                    break;
                }
                case 'r': {
                    string[] rotStr = token.Substring(2, token.Length - 3).Split(' ', 2);
                    xRotField.text = rotStr[0];
                    yRotField.text = rotStr[1];
                    break;
                }
            }
        }

        UpdateInternalData();
    }

    public void TeleportToLocation() {
        // Setting the player's position
        player.transform.position = newPosition;

        player.camXRotation = newRotation.x;
        player.transform.rotation =
            Quaternion.Euler(player.transform.rotation.x, newRotation.y, player.transform.rotation.z);

        player.PlayerCamScript.transform.eulerAngles = new Vector3(newRotation.y,
            player.PlayerCamScript.transform.eulerAngles.y, player.PlayerCamScript.transform.eulerAngles.z);
        player.transform.eulerAngles =
            new Vector3(player.transform.eulerAngles.x, newRotation.x, player.transform.eulerAngles.z);
    }

    private string NumDisplay(float num) {
        return $"{num:0.00}";
    }
}