using System;
using UnityEngine;

namespace Unity.AppUI.Core
{
    /// <summary>
    /// Base Implementation for the Platform communication.
    /// </summary>
    class PlatformImpl : IPlatformImpl
    {
        public PlatformImpl() {}

        public float lowFrequencyUpdateInterval { get; set; } = 2.0f;

        event Action<bool> darkModeChangedInternal;

        public event Action<bool> darkModeChanged
        {
            add
            {
                darkModeChangedInternal += value;
                if (darkModeChangedInternal != null)
                    m_DarkModePollingEnabled = true;
            }
            remove
            {
                darkModeChangedInternal -= value;
                if (darkModeChangedInternal == null)
                    m_DarkModePollingEnabled = false;
            }
        }

        protected event Action<bool> highContrastChangedInternal;

        public event Action<bool> highContrastChanged
        {
            add
            {
                highContrastChangedInternal += value;
                if (highContrastChangedInternal != null)
                    m_HighContrastPollingEnabled = true;
            }
            remove
            {
                highContrastChangedInternal -= value;
                if (highContrastChangedInternal == null)
                    m_HighContrastPollingEnabled = false;
            }
        }

        protected event Action<bool> reduceMotionChangedInternal;

        bool m_ReduceMotionPollingEnabled;

        public event Action<bool> reduceMotionChanged
        {
            add
            {
                reduceMotionChangedInternal += value;
                if (reduceMotionChangedInternal != null)
                    m_ReduceMotionPollingEnabled = true;
            }
            remove
            {
                reduceMotionChangedInternal -= value;
                if (reduceMotionChangedInternal == null)
                    m_ReduceMotionPollingEnabled = false;
            }
        }

        event Action<Dir> layoutDirectionChangedInternal;

        public event Action<Dir> layoutDirectionChanged
        {
            add
            {
                layoutDirectionChangedInternal += value;
                if (layoutDirectionChangedInternal != null)
                    m_LayoutDirectionPollingEnabled = true;
            }
            remove
            {
                layoutDirectionChangedInternal -= value;
                if (layoutDirectionChangedInternal == null)
                    m_LayoutDirectionPollingEnabled = false;
            }
        }

        event Action<float> scaleFactorChangedInternal;

        bool m_ScaleFactorPollingEnabled;

        public event Action<float> scaleFactorChanged
        {
            add
            {
                scaleFactorChangedInternal += value;
                if (scaleFactorChangedInternal != null)
                    m_ScaleFactorPollingEnabled = true;
            }
            remove
            {
                scaleFactorChangedInternal -= value;
                if (scaleFactorChangedInternal == null)
                    m_ScaleFactorPollingEnabled = false;
            }
        }

        event Action<float> textScaleFactorChangedInternal;

        bool m_TextScaleFactorPollingEnabled;

        public event Action<float> textScaleFactorChanged
        {
            add
            {
                textScaleFactorChangedInternal += value;
                if (textScaleFactorChangedInternal != null)
                    m_TextScaleFactorPollingEnabled = true;
            }
            remove
            {
                textScaleFactorChangedInternal -= value;
                if (textScaleFactorChangedInternal == null)
                    m_TextScaleFactorPollingEnabled = false;
            }
        }

        event Action systemColorChangedInternal;

        bool m_SystemColorPollingEnabled;

        public event Action systemColorChanged
        {
            add
            {
                systemColorChangedInternal += value;
                if (systemColorChangedInternal != null)
                    m_SystemColorPollingEnabled = true;
            }
            remove
            {
                systemColorChangedInternal -= value;
                if (systemColorChangedInternal == null)
                    m_SystemColorPollingEnabled = false;
            }
        }

        public virtual float referenceDpi => Platform.baseDpi;

        public virtual float scaleFactor => m_LastScaleFactor;

        public virtual float textScaleFactor => m_LastTextScaleFactor;

        public virtual bool isTouchGestureSupported { get; protected set; }

        public virtual bool darkMode => m_LastDarkMode;

        public virtual bool highContrast => m_LastHighContrast;

        public virtual bool reduceMotion => m_LastReduceMotion;

        public virtual bool isHapticFeedbackSupported => false;

        bool m_DarkModePollingEnabled;

        bool m_HighContrastPollingEnabled;

        bool m_LayoutDirectionPollingEnabled;

        double m_LastLowFrequencyUpdateTime = 0;

        bool m_LastDarkMode = false;

        bool m_LastHighContrast = false;

        bool m_LastReduceMotion = false;

        int m_LastLayoutDirection = 0;

        float m_LastScaleFactor = 1f;

        float m_LastTextScaleFactor = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorWindow m_LastEditorWindow;
#endif

        public void UpdateLoop()
        {
            HighFrequencyUpdate();
            var currentTime = Time.unscaledTime;

            if (currentTime <= lowFrequencyUpdateInterval)
                return;

            if (currentTime - m_LastLowFrequencyUpdateTime > lowFrequencyUpdateInterval)
            {
                m_LastLowFrequencyUpdateTime = currentTime;
                LowFrequencyUpdate();
            }
        }

        protected virtual void HighFrequencyUpdate()
        {

        }

        protected virtual void LowFrequencyUpdate()
        {
            PollDarkMode();
            PollHighContrast();
            PollLayoutDirection();
            PollScaleFactor();
            PollTextScaleFactor();
            PollReduceMotion();
            PollSystemColor();
        }

        protected virtual void PollDarkMode()
        {
            if (!m_DarkModePollingEnabled)
                return;

            var newDarkMode = darkMode;
            if (m_LastDarkMode != newDarkMode)
            {
                m_LastDarkMode = newDarkMode;
                darkModeChangedInternal?.Invoke(newDarkMode);
            }
        }

        protected virtual void PollHighContrast()
        {
            if (!m_HighContrastPollingEnabled)
                return;

            var newHighContrast = highContrast;
            if (m_LastHighContrast != newHighContrast)
            {
                m_LastHighContrast = newHighContrast;
                highContrastChangedInternal?.Invoke(newHighContrast);
            }
        }

        protected virtual void PollReduceMotion()
        {
            if (!m_ReduceMotionPollingEnabled)
                return;

            var newReduceMotion = reduceMotion;
            if (m_LastReduceMotion != newReduceMotion)
            {
                m_LastReduceMotion = newReduceMotion;
                reduceMotionChangedInternal?.Invoke(newReduceMotion);
            }
        }

        protected virtual void PollLayoutDirection()
        {
            if (!m_LayoutDirectionPollingEnabled)
                return;

            var newLayoutDirection = layoutDirection;
            if (m_LastLayoutDirection != newLayoutDirection)
            {
                m_LastLayoutDirection = newLayoutDirection;
                layoutDirectionChangedInternal?.Invoke(newLayoutDirection == 1 ? Dir.Rtl : Dir.Ltr);
            }
        }

        protected virtual void PollScaleFactor()
        {
            if (!m_ScaleFactorPollingEnabled)
                return;

            var newScaleFactor = scaleFactor;
            if (!Mathf.Approximately(newScaleFactor, m_LastScaleFactor))
            {
                m_LastScaleFactor = newScaleFactor;
                scaleFactorChangedInternal?.Invoke(newScaleFactor);
            }
        }

        protected virtual void PollTextScaleFactor()
        {
            if (!m_TextScaleFactorPollingEnabled)
                return;

            var newTextScaleFactor = textScaleFactor;
            if (!Mathf.Approximately(newTextScaleFactor, m_LastTextScaleFactor))
            {
                m_LastTextScaleFactor = newTextScaleFactor;
                textScaleFactorChangedInternal?.Invoke(newTextScaleFactor);
            }
        }

        protected virtual void PollSystemColor()
        {
            if (!m_SystemColorPollingEnabled)
                return;

            // do nothing by default
        }

        /// <summary>
        /// The current touches on the trackpad.
        /// </summary>
        public virtual AppUITouch[] touches => null;

        public virtual void RunNativeHapticFeedback(HapticFeedbackType feedbackType)
        {
            Debug.LogWarning(Application.isEditor
                ? "Haptic Feedbacks are not supported in the Editor."
                : "Haptic Feedbacks are not supported on the current platform.");
        }

        public virtual void HandleNativeMessage(string message) {}

        public virtual void OnEnteredPlayMode() { }

        public virtual Color GetSystemColor(SystemColorType colorType) => Color.clear;

        public virtual int layoutDirection => m_LastLayoutDirection;

        protected void InvokeLayoutDirectionChanged(int layoutDirection)
        {
            layoutDirectionChangedInternal?.Invoke(layoutDirection == 1 ? Dir.Rtl : Dir.Ltr);
        }

        protected void InvokeHighContrastChanged(bool highContrastEnabled)
        {
            highContrastChangedInternal?.Invoke(highContrastEnabled);
        }

        protected void InvokeReduceMotionChanged(bool reduceMotion)
        {
            reduceMotionChangedInternal?.Invoke(reduceMotion);
        }

        protected void InvokeThemeChanged(bool darkModeEnabled)
        {
            darkModeChangedInternal?.Invoke(darkModeEnabled);
        }

        protected void InvokeTextScaleFactorChanged(float textScaleFactor)
        {
            textScaleFactorChangedInternal?.Invoke(textScaleFactor);
        }

        protected void InvokeScaleFactorChanged(float scaleFactor)
        {
            scaleFactorChangedInternal?.Invoke(scaleFactor);
        }

        protected void InvokeSystemColorChanged()
        {
            systemColorChangedInternal?.Invoke();
        }
    }
}
