using System.Collections.Generic;
using UnityEngine.UI;

namespace UnityEngine.XR.OpenXR.Samples.ControllerSample {
    public class TrackingModeOrigin : MonoBehaviour {
        private static readonly List<XRInputSubsystem> s_InputSubsystems = new();

        [SerializeField] private Image m_RecenteredImage = null;

        [SerializeField] private Color m_RecenteredOffColor = Color.red;

        [SerializeField] private Color m_RecenteredColor = Color.green;

        [SerializeField] private float m_RecenteredColorResetTime = 1.0f;

        [SerializeField] private TrackingOriginModeFlags m_CurrentTrackingOriginMode;

        [SerializeField] private Text m_CurrentTrackingOriginModeDisplay = null;

        [SerializeField] private TrackingOriginModeFlags m_DesiredTrackingOriginMode;

        [SerializeField] private TrackingOriginModeFlags m_SupportedTrackingOriginModes;

        private float m_LastRecenteredTime = 0.0f;
        public TrackingOriginModeFlags currentTrackingOriginMode => m_CurrentTrackingOriginMode;

        public TrackingOriginModeFlags desiredTrackingOriginMode {
            get => m_DesiredTrackingOriginMode;
            set => m_DesiredTrackingOriginMode = value;
        }

        public TrackingOriginModeFlags supportedTrackingOriginModes => m_SupportedTrackingOriginModes;

        private void Update() {
            XRInputSubsystem subsystem = null;

            SubsystemManager.GetSubsystems(s_InputSubsystems);
            if (s_InputSubsystems.Count > 0) subsystem = s_InputSubsystems[0];

            m_SupportedTrackingOriginModes =
                subsystem?.GetSupportedTrackingOriginModes() ?? TrackingOriginModeFlags.Unknown;

            if ((m_CurrentTrackingOriginMode != m_DesiredTrackingOriginMode) &
                (m_DesiredTrackingOriginMode != TrackingOriginModeFlags.Unknown))
                subsystem?.TrySetTrackingOriginMode(m_DesiredTrackingOriginMode);
            m_CurrentTrackingOriginMode = subsystem?.GetTrackingOriginMode() ?? TrackingOriginModeFlags.Unknown;

            if (m_CurrentTrackingOriginModeDisplay != null)
                m_CurrentTrackingOriginModeDisplay.text = m_CurrentTrackingOriginMode.ToString();

            if (m_RecenteredImage != null) {
                float lerp = (Time.time - m_LastRecenteredTime) / m_RecenteredColorResetTime;
                lerp = Mathf.Clamp(lerp, 0.0f, 1.0f);
                m_RecenteredImage.color = Color.Lerp(m_RecenteredColor, m_RecenteredOffColor, lerp);
            }
        }

        private void OnEnable() {
            SubsystemManager.GetSubsystems(s_InputSubsystems);
            for (int i = 0; i < s_InputSubsystems.Count; i++)
                s_InputSubsystems[i].trackingOriginUpdated += TrackingOriginUpdated;
        }

        private void OnDisable() {
            SubsystemManager.GetSubsystems(s_InputSubsystems);
            for (int i = 0; i < s_InputSubsystems.Count; i++)
                s_InputSubsystems[i].trackingOriginUpdated -= TrackingOriginUpdated;
        }

        public void OnDesiredSelectionChanged(int newValue) {
            desiredTrackingOriginMode = (TrackingOriginModeFlags)(newValue == 0 ? 0 : 1 << (newValue - 1));
        }

        private void TrackingOriginUpdated(XRInputSubsystem obj) {
            m_LastRecenteredTime = Time.time;
        }
    }
}