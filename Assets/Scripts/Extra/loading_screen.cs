using UnityEngine;
using UnityEngine.UI;

public class loading_screen : MonoBehaviour {
    public int current;
    public int maximum;
    public string loadingMessage;
    public Image mask;
    public Text text;

    private void Update() {
        float fillAmount = current / (float)maximum;
        mask.fillAmount = fillAmount;
        text.text = loadingMessage;
    }
}