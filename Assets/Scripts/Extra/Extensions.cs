using UnityEngine;

public static class Extensions {
    public static Color WithAlpha(this Color color, float alpha) {
        return new Color(color.r, color.g, color.b, alpha);
    }

    /** Logs an error to warn you that the value is null. Usually used in OnEnable/Awake/Start for warnings regarding the Unity inspector */
    public static void DontAllowNullYouDumbass(this MonoBehaviour obj, Object toCheck) {
        if (toCheck) return;
        Debug.LogError($"Field '{toCheck.name}' of {nameof(obj.transform.parent.name)}/{nameof(obj.gameObject.name)}/{obj.name} has not been given a value in the Unity inspector");
    }
}