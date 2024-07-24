using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RandomTitleScreen : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    public Texture2D[] characters;
    private RawImage image;
    private AudioSource sillySound;
    private bool hovered = false;

    void Start() {
        sillySound = GetComponent<AudioSource>();
        image = GetComponent<RawImage>();
        SetRandomTexture();
        if (InternalGameVersion.gameName != "Faz-Anim") Destroy(this.gameObject);
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (hovered) {
            sillySound.Play();
            SetRandomTexture();
        }
    }

    private void SetRandomTexture() {
        image.texture = characters[Random.Range(0, characters.Length)];
    }

    public void OnPointerEnter(PointerEventData eventData) {
        image.color = Color.Lerp(Color.white, Color.cyan, 0.1f);
        hovered = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        image.color = Color.white;
        hovered = false;
    }
}
