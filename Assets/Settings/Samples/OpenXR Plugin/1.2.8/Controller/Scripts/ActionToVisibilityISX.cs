using UnityEngine.InputSystem;

namespace UnityEngine.XR.OpenXR.Samples.ControllerSample {
    public class ActionToVisibilityISX : MonoBehaviour {
        [SerializeField] private InputActionProperty m_ActionReference;


        [SerializeField] private GameObject m_TargetGameobject = null;

        public InputActionProperty actionReference {
            get => m_ActionReference;
            set => m_ActionReference = value;
        }

        public GameObject targetGameObject {
            get => m_TargetGameobject;
            set => m_TargetGameobject = value;
        }

        private void Start() {
            if (m_ActionReference != null && m_ActionReference.action != null)
                m_ActionReference.action.Enable();
        }

        private void Update() {
            if (m_TargetGameobject == null)
                return;

            if (m_ActionReference != null
                && m_ActionReference.action != null
                && m_ActionReference.action.controls.Count > 0
                && m_ActionReference.action.enabled) {
                m_TargetGameobject.SetActive(true);
                return;
            }

            // No Matching devices:
            m_TargetGameobject.SetActive(false);
        }
    }
}