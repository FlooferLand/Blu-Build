using UnityEngine;
using UnityEngine.Rendering;

public class DynamicResCam : MonoBehaviour {
    public float currentScale = 100;

    private void Start() {
        // Binds the dynamic resolution policy defined above.
        DynamicResolutionHandler.SetDynamicResScaler(SetDynamicResolutionScale,
            DynamicResScalePolicyType.ReturnsPercentage);
    }

    public float SetDynamicResolutionScale() {
        return currentScale;
    }
}