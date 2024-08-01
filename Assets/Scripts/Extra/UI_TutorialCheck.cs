using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_TutorialCheck : MonoBehaviour {
    public TextMeshProUGUI stateInfoText;

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

    private void LoadGameScene(bool skipTutorial) {
        string sceneName = InternalGameData.buildType == GameBuildType.Faz ? "Front Entrance" : "Rival Restaurant";

        stateInfoText.enabled = true;
        stateInfoText.text = $"Loading \"{sceneName}\"..";
        stateInfoText.ForceMeshUpdate(true, true);

        TutorialManager.skippedTutorial = skipTutorial;
        SceneManager.LoadScene(sceneName);
    }

    /**
     * Loads the game with the tutorial
     */
    public void LoadTutorial() {
        LoadGameScene(false);
    }

    /**
     * Skips the tutorial
     */
    public void LoadGame() {
        LoadGameScene(true);
    }
}