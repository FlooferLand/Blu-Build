using UnityEngine;

public class Instaspawn : MonoBehaviour {
    public GameObject[] spawn;

    private void Awake() {
        for (int i = 0; i < spawn.Length; i++) spawn[i].SetActive(true);
    }
}