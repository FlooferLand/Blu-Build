using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CursorUtils", menuName = "Game/CursorUtils")]
public class CursorUtils : ScriptableObject {
    private static CursorUtils _instance;
    public static CursorUtils Instance {
        get {
            if (!_instance) _instance = Resources.Load<CursorUtils>("CursorUtils");
            return _instance;
        }
    }
    
    public Texture2D defaultCursor;
    public Texture2D pointCursor;

    private void Awake() {
        if (!Instance.defaultCursor) Instance.defaultCursor = Resources.Load<Texture2D>("Cursors/Metal Default");
        if (!Instance.pointCursor) Instance.pointCursor = Resources.Load<Texture2D>("Cursors/Metal Point");
    }

    public static void Set(CursorKind cursor) {
        Texture2D texture = cursor switch {
            CursorKind.Default => Instance.defaultCursor,
            CursorKind.Point => Instance.pointCursor,
            _ => throw new ArgumentOutOfRangeException(nameof(cursor), cursor, null)
        };
        Cursor.SetCursor(texture, Vector2.up, CursorMode.Auto);
    }
}

public enum CursorKind {
    Default,
    Point
}
