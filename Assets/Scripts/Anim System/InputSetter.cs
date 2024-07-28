using UnityEngine;

public class InputSetter : MonoBehaviour {
    public GameObject thePlayer;
    public int mapping;

    // Start is called before the first frame update
    private void OnEnable() {
        thePlayer.GetComponent<InputHandler>().valveMapping = mapping;
    }
}