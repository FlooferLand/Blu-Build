using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_TutorialCheck : MonoBehaviour {
    public TextMeshProUGUI stateInfoText;
    
    private void LoadGameScene(bool skipTutorial) {
        const string sceneName = InternalGameVersion.gameName != "Faz-Anim" ? "Rival Restaurant" : "Front Entrance";
        
        stateInfoText.enabled = true;
        stateInfoText.text = $"Loading \"{sceneName}\"..";
        stateInfoText.ForceMeshUpdate(true, true);
        
        TutorialManager.skippedTutorial = skipTutorial;
        SceneManager.LoadScene(sceneName);
    }
    
    private void Awake() {
        var canvas = GetComponent<Canvas>();
        
        // Skipping if the player already did the tutorial (or if they previously skipped it)
        if (!TutorialManager.ShouldDoTutorial()) {
            canvas.enabled = false;
            LoadGameScene(true);
        }

        canvas.enabled = true;
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
