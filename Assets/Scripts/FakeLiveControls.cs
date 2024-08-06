using System;
using UnityEngine;

/** Handles show selector and live editor impostors */
public class FakeLiveControls : MonoBehaviour {
    public ShowId show;
    
    [Header("Controller (usually GlobalController)")]
    public GameObject controller;

    [Header("References")]
    public Button3D showStartButton;

    private void OnEnable() {
        this.DontAllowNullYouDumbass(controller);
        if (!controller) {
            controller = GameObject.FindWithTag("Global Controller");
        }
        
        showStartButton.funcWindow = (int) show;
        showStartButton.controller = controller;
    }
}
