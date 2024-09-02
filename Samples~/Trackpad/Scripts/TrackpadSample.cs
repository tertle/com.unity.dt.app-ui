using System;
using Unity.AppUI.Core;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Unity.AppUI.Samples
{
    public class TrackpadSample : MonoBehaviour
    {
        public float dotMoveFactor = 10f;

        public float dotScaleFactor = 2f;

        public float dotMinSize = 1f;

        public float dotMaxSize = 10f;

        public float cameraZoomSpeed = 2f;

        public float cameraMoveSpeed = 2f;

        public float cameraMinSize = 0.5f;

        public float cameraMaxSize = 5f;

        public UIDocument uiDocument;

        VisualElement m_Dot;

        Camera m_Camera;

        Vector2 m_Offset;

        void Start()
        {
            m_Camera = GetComponent<Camera>();
            var panel = uiDocument.rootVisualElement.Q<Panel>("panel");
            m_Dot = panel.Q<VisualElement>("trackpad-viz__dot");
            panel.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            panel.RegisterCallback<WheelEvent>(OnWheel);
            panel.RegisterCallback<PinchGestureEvent>(OnMagnify);
        }

        void OnGeometryChanged(GeometryChangedEvent evt)
        {
            var size = m_Dot.parent.contentRect.size;
            m_Dot.transform.position = new Vector3(size.x / 2f, size.y / 2f, 0f);
        }

        void OnMagnify(PinchGestureEvent evt)
        {
            var scale = evt.gesture.deltaMagnification * dotScaleFactor + m_Dot.transform.scale.x;
            scale = Mathf.Clamp(scale, dotMinSize, dotMaxSize);
            m_Dot.transform.scale = new Vector3(scale, scale, 1f);
        }

        void OnWheel(WheelEvent evt)
        {
            if (evt.modifiers != EventModifiers.None)
                return;

            var size = m_Dot.parent.contentRect.size;
            var x = Mathf.Clamp(m_Dot.transform.position.x + evt.delta.x * dotMoveFactor, 0f, size.x);
            var y = Mathf.Clamp(m_Dot.transform.position.y + evt.delta.y * dotMoveFactor, 0f, size.y);
            m_Dot.transform.position = new Vector3(x, y, 0f);

            m_Offset.x += evt.delta.x;
            m_Offset.y += evt.delta.y;
        }

        void Update()
        {
            var magGesture = AppUIInput.pinchGesture;
            if (magGesture.state is GestureRecognizerState.Changed or GestureRecognizerState.Began)
            {
                m_Camera.orthographicSize =
                    Mathf.Clamp(m_Camera.orthographicSize - magGesture.deltaMagnification * cameraZoomSpeed, cameraMinSize, cameraMaxSize);
            }

            var tr = transform;
            var pos = tr.position;
            tr.position = new Vector3(
                pos.x - m_Offset.x * cameraMoveSpeed * m_Camera.orthographicSize,
                pos.y,
                pos.z + m_Offset.y * cameraMoveSpeed * m_Camera.orthographicSize);

            m_Offset = Vector2.zero;
        }
    }
}
