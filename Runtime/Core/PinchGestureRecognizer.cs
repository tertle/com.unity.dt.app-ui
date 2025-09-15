using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.AppUI.Core
{
    /// <summary>
    /// A continuous gesture recognizer that interprets pinching gestures involving two touches.
    /// Uses initial distance threshold and movement angle (dot product) to differentiate from panning.
    /// Handles intermittent touch updates.
    /// </summary>
    public class PinchGestureRecognizer : GestureRecognizer<float>
    {
        // --- Constants for Tuning ---

        /// <summary>
        /// Minimum change in distance from the *initial* touch distance required to
        /// potentially start the pinch gesture.
        /// </summary>
        const float k_StartDistanceThreshold = 20.0f; // Original threshold value

        /// <summary>
        /// Dot product threshold for detecting parallel movement. If the dot product of
        /// normalized movement vectors is above this, it's considered panning. (Range: -1 to 1)
        /// </summary>
        const float k_DotProductThreshold = -0.2f; // Tune this

        /// <summary>
        /// Minimum change in the scale ratio (currentDistance / gestureBeganDistance)
        /// required to trigger a Changed event. Prevents spamming events for tiny movements.
        /// </summary>
        const float k_RatioDeltaThreshold = 1e-3f; // 0.001

        /// <summary>
        /// Small squared magnitude threshold to avoid division by zero or issues with vectors.
        /// </summary>
        const float k_VecSqrMagnitudeThreshold = 1e-6f; // Threshold for normalizing vectors

        /// <summary>
        /// Number of frames to tolerate not pinching before ending the pinch gesture.
        /// </summary>
        const int k_PinchStopFrameTolerance = 1;

        /// <summary>
        /// Number of frames to tolerate before starting the pinch gesture.
        /// </summary>
        const int k_PinchStartFrameTolerance = 1;

        readonly HashSet<int> m_TouchingFingers = new HashSet<int>();

        // --- Finger Tracking ---
        int m_FingerId1 = -1; // ID of the first tracked finger

        int m_FingerId2 = -1; // ID of the second tracked finger

        // Distance when the gesture *actually* started (Began state). Used for scaling ratio.
        float m_GestureBeganDistance;

        // NEW: Distance when the second finger was initially tracked. Used for start threshold.
        float m_InitialTouchDistance;

        // Last scaling ratio reported (relative to m_GestureBeganDistance)
        float m_LastRatio = 1f;

        // --- Position & State ---
        Vector2 m_Position1; // Last known position of finger 1

        Vector2 m_Position2; // Last known position of finger 2

        int m_PanningFramesCounter;

        int m_PinchStartAttemptFrames;

        // --- Helper Properties ---
        bool isTrackingFingers => m_FingerId1 != -1 && m_FingerId2 != -1;

        //---------------------------------------------------------------------
        // Gesture State Management (EndAnyGesture remains the same)
        //---------------------------------------------------------------------
        void EndAnyGesture(GestureRecognizerState endState = GestureRecognizerState.Ended)
        {
            var previousState = state;
            value = 0;
            state = previousState is GestureRecognizerState.Began or GestureRecognizerState.Changed
                ? endState
                : GestureRecognizerState.Failed;

            if (previousState is GestureRecognizerState.Began or GestureRecognizerState.Changed)
            {
                // Debug.LogWarning($"Gesture Ended/Failed. Prev State: {previousState}, New State: {state}. Triggered by EndAnyGesture({endState})");
            }

            m_PinchStartAttemptFrames = 0;
            m_GestureBeganDistance = 0f;
            m_LastRatio = 1f;

            // Keep m_InitialTouchDistance until Reset
        }

        /// <inheritdoc />
        public override void Reset()
        {
            var previousState = state;
            state = GestureRecognizerState.Possible;
            m_FingerId1 = -1;
            m_FingerId2 = -1;
            m_TouchingFingers.Clear();
            value = 0;
            hasChangedThisFrame = false;
            m_Position1 = Vector2.zero;
            m_Position2 = Vector2.zero;
            m_InitialTouchDistance = 0f;
            m_GestureBeganDistance = 0f;
            m_LastRatio = 1f;
            m_PanningFramesCounter = 0;
            m_PinchStartAttemptFrames = 0;

            if (previousState != GestureRecognizerState.Possible)
            {
                // Debug.LogWarning($"Gesture Reset. Prev State: {previousState}, New State: {state}. Triggered by Reset()");
            }
        }

        //---------------------------------------------------------------------
        // Finger Tracking Logic (TrackAnyNewFingerWhenPossible needs update)
        //---------------------------------------------------------------------
        bool IsTrackedFinger(int fingerId)
        {
            return fingerId == m_FingerId1 || fingerId == m_FingerId2;
        }

        void TrackFinger(AppUITouch touch)
        {
            var alreadyTouching = m_TouchingFingers.Contains(touch.fingerId);

            if (touch.phase is TouchPhase.Began or TouchPhase.Stationary)
            {
                if (!alreadyTouching)
                    m_TouchingFingers.Add(touch.fingerId);
            }

            if (!IsTrackedFinger(touch.fingerId))
            {
                if (isTrackingFingers)
                {
                    if (!alreadyTouching && touch.phase == TouchPhase.Began)

                        // Debug.LogWarning("Third finger detected, failing pinch gesture.");
                        EndAnyGesture(GestureRecognizerState.Failed);
                }
                else
                {
                    var justTracked = false;
                    var initialPosition = touch.position;

                    if (m_FingerId1 == -1 && m_FingerId2 != touch.fingerId)
                    {
                        m_FingerId1 = touch.fingerId;
                        m_Position1 = initialPosition;
                        justTracked = true;
                        if (!alreadyTouching)
                            m_TouchingFingers.Add(touch.fingerId);
                    }
                    else if (m_FingerId2 == -1 && m_FingerId1 != touch.fingerId)
                    {
                        m_FingerId2 = touch.fingerId;
                        m_Position2 = initialPosition;
                        justTracked = true;
                        if (!alreadyTouching)
                            m_TouchingFingers.Add(touch.fingerId);
                    }

                    // If we just acquired the second finger, record INITIAL distance
                    if (justTracked && isTrackingFingers)
                    {
                        m_InitialTouchDistance = Vector2.Distance(m_Position1, m_Position2); // Record initial distance
                        m_LastRatio = 1f;
                        m_GestureBeganDistance = 0f; // Not started yet

                        // Debug.Log($"Tracking fingers {m_FingerId1} and {m_FingerId2}. Initial Distance: {m_InitialTouchDistance:F2}");
                    }
                }
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                if (touch.fingerId == m_FingerId1)
                    m_Position1 = touch.position;
                else
                    m_Position2 = touch.position;
            }
        }

        //---------------------------------------------------------------------
        // UntrackFinger (Remains the same)
        //---------------------------------------------------------------------
        void UntrackFinger(int fingerId)
        {
            m_TouchingFingers.Remove(fingerId);
            var lostTrackedFinger = false;
            if (fingerId == m_FingerId1)
            {
                m_FingerId1 = -1;
                lostTrackedFinger = true;
            }
            else if (fingerId == m_FingerId2)
            {
                m_FingerId2 = -1;
                lostTrackedFinger = true;
            }

            if (lostTrackedFinger)
            {
                // Debug.LogWarning($"UntrackFinger causing EndAnyGesture. Lost fingerId: {fingerId}.");
                EndAnyGesture();
            }

            if (!isTrackingFingers)
            {
                m_PinchStartAttemptFrames = 0;
            }
        }

        /// <inheritdoc />
        public override void Recognize(ReadOnlySpan<AppUITouch> appuiTouches)
        {
            base.Recognize(appuiTouches); // Clear hasChangedThisFrame
            var previousPosition1 = m_Position1;
            var previousPosition2 = m_Position2;

            // Process touches (updates positions, handles Ended/Canceled via UntrackFinger)
            foreach (var touch in appuiTouches)
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                    case TouchPhase.Stationary:
                    case TouchPhase.Moved:
                        TrackFinger(touch);
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        UntrackFinger(touch.fingerId);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            // Perform Gesture Analysis IF we are tracking two fingers
            if (isTrackingFingers)
            {
                // Calculate deltas based on potentially updated positions vs. start-of-frame positions
                var delta1 = m_Position1 - previousPosition1;
                var delta2 = m_Position2 - previousPosition2;
                var delta1SqrMag = delta1.sqrMagnitude;
                var delta2SqrMag = delta2.sqrMagnitude;

                var currentDistance = Vector2.Distance(m_Position1, m_Position2);
                var stationaryFinger1 = delta1SqrMag < k_VecSqrMagnitudeThreshold;
                var stationaryFinger2 = delta2SqrMag < k_VecSqrMagnitudeThreshold;
                var stationaryFingers = stationaryFinger1 && stationaryFinger2;

                // Calculate dot product for angle check (if fingers moved)
                var pinching = stationaryFinger1 || stationaryFinger2;
                var dotProduct = 0f;
                if (!pinching)
                {
                    var normDelta1 = delta1 / Mathf.Sqrt(delta1SqrMag);
                    var normDelta2 = delta2 / Mathf.Sqrt(delta2SqrMag);
                    dotProduct = Vector2.Dot(normDelta1, normDelta2);
                    pinching = dotProduct < k_DotProductThreshold;
                }

                switch (state)
                {
                    case GestureRecognizerState.Failed:
                    case GestureRecognizerState.Ended:
                    case GestureRecognizerState.Possible:
                    {
                        // Check for conditions to START the pinch

                        // Distance Threshold Check (using initial touch distance)
                        // Ensure initial distance was recorded (should be > 0 if tracking 2 fingers)
                        var distanceThresholdMet = m_InitialTouchDistance > k_VecSqrMagnitudeThreshold &&
                            Mathf.Abs(currentDistance - m_InitialTouchDistance) > k_StartDistanceThreshold;

                        // Optional: Log dot product for tuning
                        // Debug.Log($"Dot Product: {dotProduct:F3}");

                        // If one or both fingers didn't move significantly, don't consider it panning for this check.
                        // This allows starting a pinch even if one finger is stationary.
                        // START Condition: Distance changed significantly AND movement wasn't parallel panning
                        if (distanceThresholdMet && pinching)
                        {
                            m_PinchStartAttemptFrames++;
                            if (m_PinchStartAttemptFrames >= k_PinchStartFrameTolerance)
                            {
                                state = GestureRecognizerState.Began;
                                m_GestureBeganDistance = currentDistance; // Record distance at actual start
                                m_LastRatio = 1f;
                                value = 0;
                                m_PanningFramesCounter = 0;
                                hasChangedThisFrame = true;
                                // Debug.Log($"PINCH BEGAN (Angle Check): DistDiff={Mathf.Abs(currentDistance - m_InitialTouchDistance):F2}, isNotPanning=true, BeganDist={m_GestureBeganDistance:F2}");
                            }
                            else
                            {
                                // Tolerance not yet met, stay in current state
                            }
                        }
                        else
                        {
                            m_PinchStartAttemptFrames = 0; // Reset if conditions not met
                        }

                        break;
                    }
                    case GestureRecognizerState.Began:
                    case GestureRecognizerState.Changed:
                    {
                        if (stationaryFingers || pinching)
                        {
                            // Reset the panning counter as the condition is met for pinching
                            m_PanningFramesCounter = 0;

                            // CONTINUE condition: Use same logic as before (ratio based on m_GestureBeganDistance)
                            if (Mathf.Abs(m_GestureBeganDistance) < k_VecSqrMagnitudeThreshold * 100)
                            {
                                // Slightly larger threshold for safety
                                m_GestureBeganDistance = currentDistance;
                                if (Mathf.Abs(m_GestureBeganDistance) < k_VecSqrMagnitudeThreshold * 100)
                                {
                                    EndAnyGesture(GestureRecognizerState.Failed);
                                    return;
                                }

                                m_LastRatio = 1f;
                            }

                            var currentRatio = currentDistance / m_GestureBeganDistance;
                            var newDelta = currentRatio - m_LastRatio;

                            if (Mathf.Abs(newDelta) > k_RatioDeltaThreshold)
                            {
                                m_LastRatio = currentRatio;
                                value = newDelta;
                                state = GestureRecognizerState.Changed;
                                hasChangedThisFrame = true;
                            }
                        }
                        else
                        {
                            m_PanningFramesCounter++;

                            if (m_PanningFramesCounter >= k_PinchStopFrameTolerance)
                            {
                                // If motion has become parallel panning, end the pinch gesture.
                                // Debug.LogWarning($"Pinch gesture ended (dot product: {dotProduct:F4}).");
                                EndAnyGesture(); // End the gesture
                                // Note: State is now Ended. Further logic in this block is skipped.
                            }
                            else
                            {
                                // Tolerance not yet exceeded, ignore this frame's "bad" input
                                // Do nothing - keep the state as Began/Changed, don't update 'value'
                                // Debug.Log($"Potential panning frame ignored (dot product: {dotProduct:F4}, count: {m_PanningFramesCounter})");
                            }
                        }
                        break;
                    }
                }
            }
        }
    }
}
