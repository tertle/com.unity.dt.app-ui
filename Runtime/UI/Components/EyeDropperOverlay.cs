using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AppUI.UI
{
    class EyeDropperOverlay : Image, INotifyValueChanging<Color>
    {
        new const string ussClassName = "appui-eye-dropper-overlay";

        const string magnifierUssClassName = "appui-eye-dropper-magnifier";

        const string gridUssClassName = magnifierUssClassName + "__grid";

        const string rowUssClassName = magnifierUssClassName + "__row";

        const string cellUssClassName = magnifierUssClassName + "__cell";

        const string colorNameLabelUssClassName = magnifierUssClassName + "__color-name-label";

        static RenderTexture s_ScreenshotRenderTexture;
        static RenderTexture s_FlippedRenderTexture;
        static Texture2D s_ReadableTexture;

        VisualElement m_Magnifier;

        VisualElement m_MagnifierGrid;

        TextElement m_ColorNameLabel;

        Color m_Value;

        Color m_TempValue;

        readonly Color m_InitialValue;

        public EyeDropperOverlay(Color initialValue)
        {
            AddToClassList(ussClassName);
            pickingMode = PickingMode.Position;
            m_InitialValue = initialValue;
            focusable = true;

            CreateMagnifier();
            hierarchy.Add(m_Magnifier);

            RegisterEvents();
        }

        void RegisterEvents()
        {
            RegisterCallback<PointerMoveEvent>(OnPointerMove);
            RegisterCallback<PointerDownEvent>(OnPointerDown);
            RegisterCallback<KeyDownEvent>(OnKeyDown);

            // Ensure we capture all pointer events
            RegisterCallback<PointerEnterEvent>(OnPointerEnter);
        }

        void OnPointerEnter(PointerEnterEvent evt)
        {
            Focus(); // Ensure we maintain focus for keyboard events
        }

        void OnPointerMove(PointerMoveEvent evt)
        {
            if (!isActive) return;

            Vector2 mousePos = evt.position;

            // Show magnifier if hidden
            if (m_Magnifier.style.visibility == Visibility.Hidden)
            {
                m_Magnifier.style.visibility = Visibility.Visible;
            }

            // Update magnifier position (offset to avoid cursor overlap)
            var magnifierX = mousePos.x + 20;
            var magnifierY = mousePos.y - 100;

            // Keep magnifier on screen
            if (magnifierX + m_Magnifier.layout.width + 20 > layout.width)
                magnifierX = mousePos.x - m_Magnifier.layout.width - 20;
            if (magnifierY < 0)
                magnifierY = mousePos.y + 20;
            if (magnifierY + m_Magnifier.layout.height + 20 > layout.height)
                magnifierY = layout.height - m_Magnifier.layout.height - 20;

            m_Magnifier.style.translate = new Translate(magnifierX, magnifierY);

            // Update magnifier content
            UpdateMagnifierContent(mousePos);
        }

        public bool isActive { get; private set; }

        void CreateMagnifier()
        {
            m_Magnifier = new VisualElement
            {
                name = magnifierUssClassName,
                pickingMode = PickingMode.Ignore,
            };
            m_Magnifier.AddToClassList(magnifierUssClassName);
            m_Magnifier.usageHints |= UsageHints.DynamicTransform;

            // Create 8x8 grid
            m_MagnifierGrid = new VisualElement
            {
                name = gridUssClassName,
                pickingMode = PickingMode.Ignore,
            };
            m_MagnifierGrid.AddToClassList(gridUssClassName);

            for (var y = 0; y < 9; y++)
            {
                var row = new VisualElement
                {
                    name = rowUssClassName,
                    pickingMode = PickingMode.Ignore
                };
                row.AddToClassList(rowUssClassName);

                for (var x = 0; x < 9; x++)
                {
                    var cell = new VisualElement
                    {
                        name = cellUssClassName,
                        pickingMode = PickingMode.Ignore
                    };
                    cell.AddToClassList(cellUssClassName);
                    cell.usageHints |= UsageHints.DynamicColor;

                    // Center cell gets special highlight
                    if (x == 4 && y == 4)
                    {
                        cell.style.borderTopColor = Color.black;
                        cell.style.borderLeftColor = Color.black;
                    }
                    if (x == 5 && y == 4)
                    {
                        cell.style.borderLeftColor = Color.black;
                    }
                    if (x == 4 && y == 5)
                    {
                        cell.style.borderTopColor = Color.black;
                    }

                    row.Add(cell);
                }
                m_MagnifierGrid.Add(row);
            }

            m_Magnifier.Add(m_MagnifierGrid);

            // Create color name label
            m_ColorNameLabel = new TextElement
            {
                name = colorNameLabelUssClassName,
                pickingMode = PickingMode.Ignore,
            };
            m_ColorNameLabel.AddToClassList(colorNameLabelUssClassName);
            m_ColorNameLabel.usageHints |= UsageHints.DynamicColor;

            m_Magnifier.Add(m_ColorNameLabel);

            // Initially hide magnifier until first mouse move
            m_Magnifier.style.visibility = Visibility.Hidden;
        }

        public void StartEyedropper(Vector2 mousePosition)
        {
            var width = Screen.width;
            var height = Screen.height;
            if (!s_ScreenshotRenderTexture || s_ScreenshotRenderTexture.width != width || s_ScreenshotRenderTexture.height != height)
            {
                if (s_ScreenshotRenderTexture)
                    UnityEngine.Object.Destroy(s_ScreenshotRenderTexture);
                s_ScreenshotRenderTexture = new RenderTexture(width, height, 24);
            }
            var scale = new Vector2(1, SystemInfo.graphicsUVStartsAtTop ? -1 : 1);
            var offset = new Vector2(0, SystemInfo.graphicsUVStartsAtTop ? 1 : 0);
            if (!s_FlippedRenderTexture || s_FlippedRenderTexture.width != width || s_FlippedRenderTexture.height != height)
            {
                if (s_FlippedRenderTexture)
                    UnityEngine.Object.Destroy(s_FlippedRenderTexture);
                s_FlippedRenderTexture = new RenderTexture(width, height, 24);
            }


            ScreenCapture.CaptureScreenshotIntoRenderTexture(s_ScreenshotRenderTexture);
            Graphics.Blit(s_ScreenshotRenderTexture, s_FlippedRenderTexture, scale, offset);

            RenderTexture.active = s_FlippedRenderTexture;
            s_ReadableTexture = new Texture2D(width, height, TextureFormat.RGB24, false);
            s_ReadableTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            s_ReadableTexture.Apply();
            RenderTexture.active = null;
            image = s_ReadableTexture;

            isActive = true;

            OnPointerMove(PointerMoveEvent.GetPooled(new Event
            {
                type = EventType.MouseMove,
                mousePosition = mousePosition,
            }));
        }

        void UpdateMagnifierContent(Vector2 centerPos)
        {
            // Sample 8x8 area around cursor
            var centerX = Mathf.RoundToInt(centerPos.x);
            var centerY = Mathf.RoundToInt(layout.height - centerPos.y); // Flip Y coordinate

            var gridChildren = m_MagnifierGrid.Children();
            var rowIndex = 0;

            foreach (var row in gridChildren)
            {
                var cellChildren = row.Children();
                var cellIndex = 0;

                foreach (var cell in cellChildren)
                {
                    var sampleX = centerX + (cellIndex - 4);
                    var sampleY = centerY + ((m_MagnifierGrid.childCount - rowIndex) - 4);

                    var pixelColor = SamplePixelColor(sampleX, sampleY);
                    cell.style.backgroundColor = pixelColor;

                    // Store center color
                    if (cellIndex == 4 && rowIndex == 4)
                    {
                        var previousColor = m_TempValue;
                        m_TempValue = pixelColor;
                        if (previousColor != m_TempValue)
                        {
                            using var evt = ChangingEvent<Color>.GetPooled();
                            evt.previousValue = previousColor;
                            evt.newValue = m_TempValue;
                            evt.target = this;
                            SendEvent(evt);
                            UpdateColorLabel(m_TempValue);
                        }
                    }

                    cellIndex++;
                }
                rowIndex++;
            }
        }

        void UpdateColorLabel(Color color)
        {
            var hexColor = ColorUtility.ToHtmlStringRGB(color);
            var colorName = GetColorName(color);

            if (colorName.StartsWith("#"))
            {
                m_ColorNameLabel.text = $"#{hexColor}";
            }
            else
            {
                m_ColorNameLabel.text = $"{colorName}\n#{hexColor}";
            }

            // Set text color based on brightness for better readability
            var brightness = (color.r * 0.299f + color.g * 0.587f + color.b * 0.114f);
            m_ColorNameLabel.style.backgroundColor = color;
            m_ColorNameLabel.style.color = brightness > 0.5f ? Color.black : Color.white;
        }

        Color SamplePixelColor(int x, int y)
        {
            if (image is not Texture2D tex)
                return Color.magenta;

            // x and y are in logical pixel unit, we need to convert them to texture coordinates
            // we just need to normalize them then multiply by texture size

            var normalizedX = Mathf.Clamp01(x / layout.width);
            var normalizedY = Mathf.Clamp01(y / layout.height);

            x = Mathf.RoundToInt(normalizedX * tex.width);
            y = Mathf.RoundToInt(normalizedY * tex.height);

            return tex.GetPixel(x, y);
        }

        void OnPointerDown(PointerDownEvent evt)
        {
            if (!isActive) return;

            // Only respond to left mouse button
            if (evt.button == 0)
            {
                CompletePicking(true);
            }
        }

        void OnKeyDown(KeyDownEvent evt)
        {
            if (!isActive) return;

            if (evt.keyCode == KeyCode.Escape)
            {
                evt.StopPropagation();
                CompletePicking(false);
            }
        }

        void CompletePicking(bool success)
        {
            if (!isActive) return;

            isActive = false;
            value = success ? m_TempValue : m_InitialValue;
        }

        static string GetColorName(Color color)
        {
            // Basic color name detection
            float r = color.r, g = color.g, b = color.b;
            return $"#{ColorUtility.ToHtmlStringRGB(color)}";
        }

        public void SetValueWithoutNotify(Color newValue)
        {
            m_Value = newValue;
        }

        public Color value
        {
            get => m_Value;
            set
            {
                using var evt = ChangeEvent<Color>.GetPooled(m_Value, value);
                evt.target = this;
                SetValueWithoutNotify(value);
                SendEvent(evt);
            }
        }
    }
}
