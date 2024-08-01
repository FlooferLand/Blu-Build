using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RandomTitleScreen : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    public Texture2D[] characters;
    private bool hovered = false;
    private RawImage image;
    private AudioSource sillySound;

    private void Start() {
        sillySound = GetComponent<AudioSource>();
        image = GetComponent<RawImage>();
        SetRandomTexture();
        if (InternalGameData.buildType != GameBuildType.Faz) Destroy(gameObject);
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (hovered) {
            sillySound.Play();
            SetRandomTexture();
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        image.color = Color.white;
        hovered = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        image.color = Color.Lerp(Color.white, Color.black, 0.08f);
        hovered = false;
    }

    private void SetRandomTexture() {
        image.texture = characters[Random.Range(0, characters.Length)];
    }
}