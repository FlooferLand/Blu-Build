using System.Collections.Generic;
using UnityEngine;

public class GizmoDraw : MonoBehaviour {
    public static Dictionary<string, string> iconTypes = new() {
        { "Seat Enter", "Seat Enter.png" },
        { "Seat Exit", "Seat Exit.png" }
    };

    private void OnDrawGizmos() {
        if (iconTypes.ContainsKey(gameObject.tag)) Gizmos.DrawIcon(transform.position, iconTypes[gameObject.tag], true);
    }
}