using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UnityEngine.XR.OpenXR.Samples.ControllerSample {
    public class ActionToVector2SliderISX : MonoBehaviour {
        [SerializeField] private InputActionReference m_ActionReference;

        [SerializeField] public Slider xAxisSlider = null;

        [SerializeField] public Slider yAxisSlider = null;

        public InputActionReference actionReference {
            get => m_ActionReference;
            set => m_ActionReference = value;
        }

        private void Update() {
            if (actionReference != null && actionReference.action != null && xAxisSlider != null &&
                yAxisSlider != null) {
                if (actionReference.action.enabled) SetVisible(gameObject, true);

                var value = actionReference.action.ReadValue<Vector2>();
                xAxisSlider.value = value.x;
                yAxisSlider.value = value.y;
            }
            else {
                SetVisible(gameObject, false);
            }
        }


        private void OnEnable() {
            if (xAxisSlider == null)
                Debug.LogWarning(
                    "ActionToSlider Monobehaviour started without any associated X-axis slider.  This input won't be reported.",
                    this);

            if (yAxisSlider == null)
                Debug.LogWarning(
                    "ActionToSlider Monobehaviour started without any associated Y-axis slider.  This input won't be reported.",
                    this);
        }

        private void SetVisible(GameObject go, bool visible) {
            var graphic = go.GetComponent<Graphic>();
            if (graphic != null)
                graphic.enabled = visible;

            var graphics = go.GetComponentsInChildren<Graphic>();
            for (int i = 0; i < graphics.Length; i++) graphics[i].enabled = visible;
        }
    }
}