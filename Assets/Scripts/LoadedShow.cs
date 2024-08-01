using UnityEngine;

/**
 * A loaded in show (not the impostor/fake ones)
 * UI_PlayRecord will slowly be phased out for this
 */
public class LoadedShow : MonoBehaviour {
    [Header("References")]
    public GameObject liveEditor;
    public GameObject showSelector;
    public GameObject stage;
    public GameObject characters;
    public UI_PlayRecord playRecord;
    
}
