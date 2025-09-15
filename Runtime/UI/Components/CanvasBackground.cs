using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// A CanvasBackground is a VisualElement used as the background of a Canvas.
    /// </summary>
    public class CanvasBackground : BaseVisualElement
    {
#if ENABLE_RUNTIME_DATA_BINDINGS
        internal static readonly BindingId thickLinesProperty = nameof(thickLines);

        internal static readonly BindingId lineColorProperty = nameof(lineColor);

        internal static readonly BindingId thickLineColorProperty = nameof(thickLineColor);

        internal static readonly BindingId gridBackgroundColorProperty = nameof(gridBackgroundColor);

        internal static readonly BindingId thicknessProperty = nameof(thickness);

        internal static readonly BindingId thickLineThicknessProperty = nameof(thickLineThickness);

        internal static readonly BindingId scaleProperty = nameof(scale);

        internal static readonly BindingId offsetProperty = nameof(offset);

        internal static readonly BindingId spacingProperty = nameof(spacing);

        internal static readonly BindingId nextGridScaleFactorProperty = nameof(nextGridScaleFactor);

        internal static readonly BindingId drawPointsProperty = nameof(drawPoints);
#endif
        /// <summary>
        /// The CanvasBackground main USS class name.
        /// </summary>
        public const string ussClassName = "appui-canvas-background";

        static Material s_Material;

        static readonly Vertex[] k_Vertices = new Vertex[4];

        static readonly ushort[] k_Indices = { 0, 1, 2, 2, 3, 0 };

        [Preserve]
        static CanvasBackground()
        {
            k_Vertices[0].tint = Color.white;
            k_Vertices[1].tint = Color.white;
            k_Vertices[2].tint = Color.white;
            k_Vertices[3].tint = Color.white;
        }

        static readonly CustomStyleProperty<Color> k_GridBackgroundColorProperty = new CustomStyleProperty<Color>("--grid-background-color");

        static readonly CustomStyleProperty<Color> k_LineColorProperty = new CustomStyleProperty<Color>("--line-color");

        static readonly CustomStyleProperty<int> k_DrawPointsProperty = new CustomStyleProperty<int>("--draw-points");

        static readonly CustomStyleProperty<int> k_LineSpacingProperty = new CustomStyleProperty<int>("--line-spacing");

        static readonly CustomStyleProperty<Color> k_ThickLineColorProperty = new CustomStyleProperty<Color>("--thick-line-color");

        static readonly CustomStyleProperty<int> k_ThickLineThicknessProperty = new CustomStyleProperty<int>("--thick-line-thickness");

        static readonly CustomStyleProperty<int> k_ThickLinesProperty = new CustomStyleProperty<int>("--thick-lines");

        static readonly CustomStyleProperty<float> k_ThicknessProperty = new CustomStyleProperty<float>("--thickness");

        static readonly CustomStyleProperty<float> k_NextGridScaleFactor = new CustomStyleProperty<float>("--next-grid-scale-factor");

        const float k_DefaultNextGridScaleFactor = 10.0f;

        const float k_DefaultSpacing = 200f;

        const int k_DefaultThickLines = 10;

        const float k_DefaultThickness = 2f;

        const float k_DefaultThickLineThickness = 4f;

        static readonly Color k_DefaultLineColor = new Color(0f, 0f, 0f, 0.18f);

        static readonly Color k_DefaultThickLineColor = new Color(0f, 0f, 0f, 0.38f);

        static readonly Color k_DefaultGridBackgroundColor = new Color(0.17f, 0.17f, 0.17f, 1.0f);

        static readonly int k_Color = Shader.PropertyToID("_Color");

        static readonly int k_Opacity = Shader.PropertyToID("_Opacity");

        static readonly int k_Thickness = Shader.PropertyToID("_Thickness");

        static readonly int k_Spacing = Shader.PropertyToID("_Spacing");

        static readonly int k_Scale = Shader.PropertyToID("_Scale");

        static readonly int k_TexSize = Shader.PropertyToID("_TexSize");

        float m_Spacing = k_DefaultSpacing;

        bool m_DrawPoints;

        RenderTexture m_RT;

        float m_Scale = 1f;

        Vector2 m_Offset;

        float m_NextGridScaleFactor = k_DefaultNextGridScaleFactor;

        float m_ThickLines = k_DefaultThickLines;

        Color m_LineColor = k_DefaultLineColor;

        Color m_ThickLineColor = k_DefaultThickLineColor;

        Color m_GridBackgroundColor = k_DefaultGridBackgroundColor;

        float m_Thickness = k_DefaultThickness;

        float m_ThickLineThickness = k_DefaultThickLineThickness;

        /// <summary>
        /// The number of thick lines.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        float thickLines
        {
            get => m_ThickLines;
            set
            {
                m_ThickLines = value;
                MarkDirtyRepaint();

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in thickLinesProperty);
#endif
            }
        }

        /// <summary>
        /// The color of the lines.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        Color lineColor
        {
            get => m_LineColor;
            set
            {
                m_LineColor = value;
                MarkDirtyRepaint();

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in lineColorProperty);
#endif
            }
        }

        /// <summary>
        /// The color of the thick lines.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        Color thickLineColor
        {
            get => m_ThickLineColor;
            set
            {
                m_ThickLineColor = value;
                MarkDirtyRepaint();

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in thickLineColorProperty);
#endif
            }
        }

        /// <summary>
        /// The color of the grid background.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        Color gridBackgroundColor
        {
            get => m_GridBackgroundColor;
            set
            {
                m_GridBackgroundColor = value;
                style.backgroundColor = m_GridBackgroundColor;
                MarkDirtyRepaint();

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in gridBackgroundColorProperty);
#endif
            }
        }

        /// <summary>
        /// The thickness of the lines.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        float thickness
        {
            get => m_Thickness;
            set
            {
                m_Thickness = value;
                MarkDirtyRepaint();

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in thicknessProperty);
#endif
            }
        }

        /// <summary>
        /// The thickness of the thick lines.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        float thickLineThickness
        {
            get => m_ThickLineThickness;
            set
            {
                m_ThickLineThickness = value;
                MarkDirtyRepaint();

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in thickLineThicknessProperty);
#endif
            }
        }

        /// <summary>
        /// The scale factor to use for the grid.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public float scale
        {
            get => m_Scale;
            set
            {
                m_Scale = value;
                MarkDirtyRepaint();

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in scaleProperty);
#endif
            }
        }

        /// <summary>
        /// The offset of the grid.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public Vector2 offset
        {
            get => m_Offset;
            set
            {
                m_Offset = value;
                MarkDirtyRepaint();

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in offsetProperty);
#endif
            }
        }

        /// <summary>
        /// The spacing between lines or points.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public float spacing
        {
            get => m_Spacing;
            set
            {
                m_Spacing = value;
                MarkDirtyRepaint();

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in spacingProperty);
#endif
            }
        }

        /// <summary>
        /// The scale factor to use for the next grid.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public float nextGridScaleFactor
        {
            get => m_NextGridScaleFactor;
            set
            {
                m_NextGridScaleFactor = value;
                MarkDirtyRepaint();

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in nextGridScaleFactorProperty);
#endif
            }
        }

        /// <summary>
        /// Either draw points or lines.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public bool drawPoints
        {
            get => m_DrawPoints;
            set
            {
                m_DrawPoints = value;
                MarkDirtyRepaint();

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in drawPointsProperty);
#endif
            }
        }

        /// <summary>
        /// Instantiates a <see cref="CanvasBackground"/> element.
        /// </summary>
        public CanvasBackground()
        {
            AddToClassList(ussClassName);
            pickingMode = PickingMode.Ignore;

            RegisterCallback<CustomStyleResolvedEvent>(OnCustomStyleResolved);
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
            generateVisualContent = OnGenerateVisualContent;
        }

        void OnDetachFromPanel(DetachFromPanelEvent e)
        {
            ReleaseTextures();
        }

        void OnCustomStyleResolved(CustomStyleResolvedEvent e)
        {
            var eCustomStyle = e.customStyle;
            if (eCustomStyle.TryGetValue(k_ThickLinesProperty, out int thicklinesValue))
            {
                thickLines = thicklinesValue;
            }

            if (eCustomStyle.TryGetValue(k_ThicknessProperty, out var thicknessProp))
            {
                thickness = thicknessProp;
            }

            if (eCustomStyle.TryGetValue(k_NextGridScaleFactor, out var nextGridScaleFactor))
            {
                this.nextGridScaleFactor = nextGridScaleFactor;
            }

            if (eCustomStyle.TryGetValue(k_ThickLineThicknessProperty, out var thickLinesThicknessValue))
            {
                thickLineThickness = thickLinesThicknessValue;
            }

            if (eCustomStyle.TryGetValue(k_ThickLineColorProperty, out Color thicklineColorValue))
            {
                thickLineColor = thicklineColorValue;
            }

            if (eCustomStyle.TryGetValue(k_LineColorProperty, out Color lineColorValue))
            {
                lineColor = lineColorValue;
            }

            if (eCustomStyle.TryGetValue(k_LineSpacingProperty, out var lineSpacing))
            {
                spacing = lineSpacing;
            }

            if (eCustomStyle.TryGetValue(k_DrawPointsProperty, out var drawPointsProp))
            {
                drawPoints = drawPointsProp > 0;
            }

            if (eCustomStyle.TryGetValue(k_GridBackgroundColorProperty, out Color gridColorValue))
            {
                gridBackgroundColor = gridColorValue;
                style.backgroundColor = gridBackgroundColor;
            }
        }

        void OnGenerateVisualContent(MeshGenerationContext context)
        {
            if (!s_Material)
            {
                s_Material = MaterialUtils.CreateMaterial("Hidden/App UI/CanvasBackground");
                if (!s_Material)
                {
                    ReleaseTextures();
                    return;
                }
            }

            if (!contentRect.IsValid())
            {
                ReleaseTextures();
                return;
            }

            var rtSize = ExVisualElement.GetRenderTextureSize(contentRect, 4096);

            if (rtSize.x < 1 || rtSize.y < 1)
            {
                ReleaseTextures();
                return;
            }

            if (m_RT && (m_RT.width != rtSize.x || m_RT.height != rtSize.y))
                ReleaseTextures();

            if (!m_RT)
                m_RT = RenderTexture.GetTemporary(rtSize.x, rtSize.y, 24);

            var prevRt = RenderTexture.active;
            RenderTexture.active = m_RT;
            // Draw starts here
            Draw();
            // Draw finishes here
            RenderTexture.active = prevRt;

            var left = contentRect.xMin;
            var right = contentRect.xMax;
            var top = contentRect.yMin;
            var bottom = contentRect.yMax;

            k_Vertices[0].position = new Vector3(left, bottom, Vertex.nearZ);
            k_Vertices[1].position = new Vector3(left, top, Vertex.nearZ);
            k_Vertices[2].position = new Vector3(right, top, Vertex.nearZ);
            k_Vertices[3].position = new Vector3(right, bottom, Vertex.nearZ);

            var mwd = context.Allocate(k_Vertices.Length, k_Indices.Length, m_RT);

#if !UNITY_2023_1_OR_NEWER
            // Since the texture may be stored in an atlas, the UV coordinates need to be
            // adjusted. Simply rescale them in the provided uvRegion.
            var uvRegion = mwd.uvRegion;
#else
            var uvRegion = new Rect(0, 0, 1, 1);
#endif
            k_Vertices[0].uv = new Vector2(0, 0) * uvRegion.size + uvRegion.min;
            k_Vertices[1].uv = new Vector2(0, 1) * uvRegion.size + uvRegion.min;
            k_Vertices[2].uv = new Vector2(1, 1) * uvRegion.size + uvRegion.min;
            k_Vertices[3].uv = new Vector2(1, 0) * uvRegion.size + uvRegion.min;

            mwd.SetAllVertices(k_Vertices);
            mwd.SetAllIndices(k_Indices);
        }

        void ReleaseTextures()
        {
            if (m_RT)
                RenderTexture.ReleaseTemporary(m_RT);
            m_RT = null;
        }

        void Draw()
        {
            GL.Clear(true, true, Color.clear);

            const float minSpacing = 2f;
            var maxSpacing = m_Spacing;
            var newSpacing = m_Spacing;
            var targetSpacing = newSpacing * nextGridScaleFactor;

            var currentSpacing = newSpacing * scale;

            if (targetSpacing > 0)
            {
                while (currentSpacing >= targetSpacing)
                {
                    newSpacing /= nextGridScaleFactor;
                    currentSpacing = newSpacing * scale;
                }
            }

            while (currentSpacing < targetSpacing || targetSpacing == 0)
            {
                s_Material.SetColor(k_Color, lineColor);
                s_Material.SetFloat(k_Opacity, Mathf.InverseLerp(minSpacing, maxSpacing, currentSpacing));
                s_Material.SetFloat(k_Thickness, thickness);
                s_Material.SetFloat(k_Spacing, newSpacing);
                s_Material.SetFloat(k_Scale, scale);
                if (drawPoints)
                    s_Material.EnableKeyword("DRAW_POINTS_ON");
                else
                    s_Material.DisableKeyword("DRAW_POINTS_ON");
                var texSize = new Vector4(
                    contentRect.width,
                    contentRect.height,
                    offset.x,
                    offset.y);
                s_Material.SetVector(k_TexSize, texSize);
                Graphics.Blit(null, m_RT, s_Material);

                if (thickLines > 0 && thickLineThickness > 0)
                {
                    // draw thick lines
                    s_Material.SetColor(k_Color, thickLineColor);
                    s_Material.SetFloat(k_Thickness, thickLineThickness);
                    s_Material.SetFloat(k_Spacing, newSpacing * thickLines);
                    Graphics.Blit(null, m_RT, s_Material);
                }

                newSpacing *= nextGridScaleFactor;
                currentSpacing = newSpacing * scale;

                if (targetSpacing == 0)
                    break;
            }
        }
    }
}
