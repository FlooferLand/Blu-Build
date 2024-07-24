using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_TutorialCheck : MonoBehaviour {
    public TextMeshProUGUI stateInfoText;
    
    private void LoadGameScene(bool skipTutorial) {
        stateInfoText.enabled = false;
        stateInfoText.text = "Loading..";
        stateInfoText.ForceMeshUpdate(true, true);
        
        TutorialManager.skippedTutorial = skipTutorial;
        if (InternalGameVersion.gameName != "Faz-Anim")
            SceneManager.LoadScene("Rival Restaurant");
        else
            SceneManager.LoadScene("Front Entrance");
    }
    
    private void Awake() {
        // Skipping if the player already did the tutorial (or if they previously skipped it)
        if (!TutorialManager.ShouldDoTutorial())
            LoadGameScene(true);

        GetComponent<Canvas>().enabled = true;
        stateInfoText.enabled = false;
    }

    /** Loads the game with the tutorial */
    public void LoadTutorial() {
        LoadGameScene(false);
    }

    /** Skips the tutorial */
    public void LoadGame() {
        LoadGameScene(true);
    }
}
