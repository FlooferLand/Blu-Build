using UnityEngine;

public class SnapObject : MonoBehaviour {
    public enum SnapType {
        snapChild,
        snapParent
    }

    public SnapType snapType;

    private void Update() {
        switch (snapType) {
            case SnapType.snapChild:
                transform.position = new Vector3(Mathf.Round(transform.root.position.x * 10) / 10,
                    Mathf.Round(transform.root.position.y * 10) / 10, Mathf.Round(transform.root.position.z * 10) / 10);
                break;
            case SnapType.snapParent:
                transform.eulerAngles = new Vector3(Mathf.Round(transform.root.eulerAngles.x / 45) * 45,
                    Mathf.Round(transform.root.eulerAngles.y / 45) * 45,
                    Mathf.Round(transform.root.eulerAngles.z / 45) * 45);
                break;
        }
    }
}