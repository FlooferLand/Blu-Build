using UnityEngine;

public class MiniTableTurn : MonoBehaviour {
    public GameObject holder;
    public string character;
    public float offset;
    private GameObject characterObj;

    private void LateUpdate() {
        if (characterObj != null) {
            characterObj.transform.eulerAngles = new Vector3(characterObj.transform.eulerAngles.x,
                transform.eulerAngles.y + offset, characterObj.transform.eulerAngles.z);
            characterObj.transform.position = new Vector3(transform.position.x, characterObj.transform.position.y,
                transform.position.z);
        }
        else {
            var gg = holder.transform.Find(character);
            if (gg != null) characterObj = gg.gameObject;
        }
    }
}