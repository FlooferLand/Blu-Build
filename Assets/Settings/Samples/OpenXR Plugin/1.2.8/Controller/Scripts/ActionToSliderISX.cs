using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UnityEngine.XR.OpenXR.Samples.ControllerSample {
    public class ActionToSliderISX : MonoBehaviour {
        [SerializeField] private InputActionReference m_ActionReference;

        [SerializeField] private Slider slider = null;

        private Graphic graphic = null;
        private Graphic[] graphics = { };

        public InputActionReference actionReference {
            get => m_ActionReference;
            set => m_ActionReference = value;
        }

        private void Update() {
            if (actionReference != null && actionReference.action != null && slider != null) {
                if (actionReference.action.enabled) SetVisible(true);

                float value = actionReference.action.ReadValue<float>();
                slider.value = value;
            }
            else {
                SetVisible(false);
            }
        }

        private void OnEnable() {
            if (slider == null)
                Debug.LogWarning(
                    "ActionToSlider Monobehaviour started without any associated slider. This input will not be reported.",
                    this);

            graphic = gameObject.GetComponent<Graphic>();
            graphics = gameObject.GetComponentsInChildren<Graphic>();
        }

        private void SetVisible(bool visible) {
            if (graphic != null)
                graphic.enabled = visible;

            for (int i = 0; i < graphics.Length; i++) graphics[i].enabled = visible;
        }
    }
}