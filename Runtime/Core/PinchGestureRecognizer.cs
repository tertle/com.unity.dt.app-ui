using System.Collections.Generic;
using UnityEngine;

namespace Unity.AppUI.Core
{
    /// <summary>
    /// A continuous gesture recognizer that interprets pinching gestures involving two touches.
    /// </summary>
    public class PinchGestureRecognizer : GestureRecognizer<float>
    {
        const float k_StartDistance = 20f;

        int m_FingerId1 = -1;
        int m_FingerId2 = -1;

        Vector2 m_Position1;
        Vector2 m_Position2;

        float m_StartDistance = 0;
        float m_LastRatio = 1f;

        bool isTrackingFingers => m_FingerId1 >= 0 && m_FingerId2 >= 0;

        readonly HashSet<int> m_TouchingFingers = new HashSet<int>();

        // readonly HashSet<int> m_CurrentFrameFingers = new HashSet<int>();

        void EndAnyGesture()
        {
            value = 0;
            state = state is GestureRecognizerState.Began or GestureRecognizerState.Changed
                ? GestureRecognizerState.Ended
                : GestureRecognizerState.Failed;
        }

        bool IsTrackedFinger(int fingerId) => fingerId == m_FingerId1 || fingerId == m_FingerId2;

        bool StartTrackingFinger(AppUITouch touch)
        {
            if (m_FingerId1 == -1 && m_FingerId2 != touch.fingerId)
                m_FingerId1 = touch.fingerId;
            else if (m_FingerId2 == -1 && m_FingerId1 != touch.fingerId)
                m_FingerId2 = touch.fingerId;

            return IsTrackedFinger(touch.fingerId);
        }

        void TrackAnyNewFingerWhenPossible(AppUITouch touch)
        {
            if (m_TouchingFingers.Contains(touch.fingerId))
                return;

            m_TouchingFingers.Add(touch.fingerId);

            if (!IsTrackedFinger(touch.fingerId))
            {
                if (isTrackingFingers)
                {
                    EndAnyGesture();
                }
                else
                {
                    var started = StartTrackingFinger(touch);
                    Debug.Assert(started);

                    if (touch.fingerId == m_FingerId1)
                        m_Position1 = touch.position;
                    else
                        m_Position2 = touch.position;

                    if (isTrackingFingers)
                    {
                        m_StartDistance = Vector2.Distance(m_Position1, m_Position2);
                        m_LastRatio = 1f;
                    }
                }
            }
        }

        void UntrackFinger(int fingerId)
        {
            m_TouchingFingers.Remove(fingerId);
            if (fingerId == m_FingerId1)
                m_FingerId1 = -1;
            else if (fingerId == m_FingerId2)
                m_FingerId2 = -1;
            if (!isTrackingFingers)
                EndAnyGesture();
        }

        void OnFingerMoved(AppUITouch touch)
        {
            var isTouch1 = touch.fingerId == m_FingerId1;
            var isTouch2 = touch.fingerId == m_FingerId2;

            if (isTrackingFingers && (isTouch1 || isTouch2) && m_TouchingFingers.Count == 2)
            {
                if (isTouch1)
                    m_Position1 = touch.position;
                else
                    m_Position2 = touch.position;

                var currentDistance = Vector2.Distance(m_Position1, m_Position2);

                var wasBegan = state == GestureRecognizerState.Began;

                if ((state is GestureRecognizerState.Failed or GestureRecognizerState.Ended)
                    && Mathf.Abs(currentDistance - m_StartDistance) > k_StartDistance)
                {
                    // try start recognizing again.
                    m_StartDistance = currentDistance;
                    m_LastRatio = 1f;
                    state = GestureRecognizerState.Began;
                }

                if (state is GestureRecognizerState.Began or GestureRecognizerState.Changed)
                {
                    // continue recognizing.
                    var currentRatio = currentDistance / m_StartDistance;
                    var newDelta = currentRatio - m_LastRatio;
                    if (Mathf.Abs(newDelta) > 1e-3)
                    {
                        m_LastRatio = currentRatio;
                        value = newDelta;
                        if (wasBegan)
                            state = GestureRecognizerState.Changed;
                        hasChangedThisFrame = true;
                    }
                }
            }
        }

        /// <inheritdoc />
        public override void Recognize(AppUITouch[] appuiTouches)
        {
            // clear before polling gesture.
            base.Recognize(appuiTouches);

            if (appuiTouches.Length == 0)
            {
                value = 0;
                return;
            }

            // m_CurrentFrameFingers.Clear();
            // foreach (var touchingFinger in m_TouchingFingers)
            // {
            //     m_CurrentFrameFingers.Add(touchingFinger);
            // }

            foreach (var touch in appuiTouches)
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                    case TouchPhase.Stationary:
                    {
                        TrackAnyNewFingerWhenPossible(touch);
                        // m_CurrentFrameFingers.Add(touch.fingerId);
                        break;
                    }
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                    {
                        // force to untrack all fingers so we can try to find them
                        // again in from another touch event.
                        m_FingerId1 = -1;
                        m_FingerId2 = -1;
                        UntrackFinger(touch.fingerId);
                        // m_CurrentFrameFingers.Remove(touch.fingerId);
                        break;
                    }
                    case TouchPhase.Moved:
                    {
                        TrackAnyNewFingerWhenPossible(touch);
                        // m_CurrentFrameFingers.Add(touch.fingerId);
                        OnFingerMoved(touch);
                        break;
                    }
                }
            }

            // clear lost fingers.
            // foreach (var fingerId in new HashSet<int>(m_TouchingFingers))
            // {
            //     if (!m_CurrentFrameFingers.Contains(fingerId))
            //         UntrackFinger(fingerId);
            // }

            // if we lost track of any finger, just abort any active magnify gesture.
            // if (m_FingerId1 >= 0 && !m_TouchingFingers.Contains(m_FingerId1))
            //     UntrackFinger(m_FingerId1);
            // if (m_FingerId2 >= 0 && !m_TouchingFingers.Contains(m_FingerId2))
            //     UntrackFinger(m_FingerId2);
        }

        /// <inheritdoc />
        public override void Reset()
        {
            state = GestureRecognizerState.Failed;
            m_FingerId1 = -1;
            m_FingerId2 = -1;
            m_TouchingFingers.Clear();
            value = 0;
            hasChangedThisFrame = false;
        }
    }
}
