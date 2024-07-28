using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UnityEngine.XR.OpenXR.Samples.ControllerSample {
    public class DisplayDeviceInfoFromActionISX : MonoBehaviour {
        [SerializeField] private InputActionProperty m_Property;

        [SerializeField] private GameObject m_RootObject = null;

        [SerializeField] private Text m_TargetText;

        public InputActionProperty property {
            get => m_Property;
            set => m_Property = value;
        }

        public GameObject rootObject {
            get => m_RootObject;
            set => m_RootObject = value;
        }

        public Text targetText {
            get => m_TargetText;
            set => m_TargetText = value;
        }


        private void Update() {
            if (m_Property != null && m_Property.action != null && m_Property.action.controls.Count > 0) {
                if (m_RootObject != null)
                    m_RootObject.SetActive(true);


                var device = m_Property.action.controls[0].device;
                if (targetText != null) {
                    m_TargetText.text = $"{device.name}\n{device.deviceId}\n";
                    bool useComma = false;
                    foreach (var usg in device.usages)
                        if (!useComma) {
                            useComma = true;
                            m_TargetText.text += $"{usg}";
                        }
                        else {
                            m_TargetText.text += $"{usg},";
                        }
                }

                return;
            }

            if (m_RootObject != null)
                m_RootObject.SetActive(false);

            // No Matching devices:
            if (m_TargetText != null)
                m_TargetText.text = "<No Device Connected>";
        }

        private void OnEnable() {
            if (targetText == null)
                Debug.LogWarning(
                    "DisplayDeviceInfo Monobehaviour has no Target Text set. No information will be displayed.");
        }
    }
}