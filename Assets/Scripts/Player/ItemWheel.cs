using UnityEngine;

public class ItemWheel : MonoBehaviour {
    public Player player;

    public void GatherIcons() {
        player.GatherItemWheelIcons();
    }

    public void Disable() {
        GetComponent<Animator>().Play("New State");
        gameObject.SetActive(false);
    }
}