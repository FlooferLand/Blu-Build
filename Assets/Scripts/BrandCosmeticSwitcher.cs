using UnityEngine;

public static class BrandingStyle {
    public enum Enum {
        Classic = 0,
        RockAfire = 1
    }

    public static int minValue = 0;
    public static int maxValue = 1;
}

/**
 * Changes between different things (mainly 3D models) based on user preference (Ex: Faz-Anim's, Reel To Reel's, etc)
 */
public class BrandCosmeticSwitcher : MonoBehaviour {
    public GameObject classic;
    public GameObject rockAfire;

    private void OnEnable() {
        switch ((BrandingStyle.Enum)PlayerPrefs.GetInt("Settings: Branding Style")) {
            default: // Sets the style if it is incorrect
                PlayerPrefs.SetInt("Settings: Branding Style", (int)BrandingStyle.Enum.Classic);
                classic.SetActive(true);
                rockAfire.SetActive(false);
                break;
            case BrandingStyle.Enum.Classic:
                classic.SetActive(true);
                rockAfire.SetActive(false);
                break;
            case BrandingStyle.Enum.RockAfire:
                classic.SetActive(false);
                rockAfire.SetActive(true);
                break;
        }
    }
}