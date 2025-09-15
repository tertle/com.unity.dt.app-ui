using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// CircularProgress UI element. This is a circular progress bar.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class CircularProgress : Progress
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId innerRadiusProperty = nameof(innerRadius);

#endif

        static Material s_Material;

        /// <summary>
        /// The CircularProgress main styling class.
        /// </summary>
        public new const string ussClassName = "appui-circular-progress";

        static readonly int k_InnerRadius = Shader.PropertyToID("_InnerRadius");

        static readonly int k_Rounded = Shader.PropertyToID("_Rounded");

        static readonly int k_Start = Shader.PropertyToID("_Start");

        static readonly int k_End = Shader.PropertyToID("_End");

        static readonly int k_BufferStart = Shader.PropertyToID("_BufferStart");

        static readonly int k_BufferEnd = Shader.PropertyToID("_BufferEnd");

        static readonly int k_Color = Shader.PropertyToID("_Color");

        static readonly int k_AA = Shader.PropertyToID("_AA");

        static readonly int k_BufferOpacity = Shader.PropertyToID("_BufferOpacity");

        static readonly int k_Phase = Shader.PropertyToID("_Phase");

        float m_InnerRadius = k_DefaultInnerRadius;

        const float k_DefaultInnerRadius = 0.38f;

        /// <summary>
        /// The inner radius of the CircularProgress.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public float innerRadius
        {
            get => m_InnerRadius;
            set
            {
                var changed = !Mathf.Approximately(m_InnerRadius, value);
                m_InnerRadius = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in innerRadiusProperty);
#endif
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public CircularProgress()
        {
            AddToClassList(ussClassName);

            innerRadius = k_DefaultInnerRadius;
        }

        /// <summary>
        /// Generates the textures for the CircularProgress.
        /// </summary>
        protected override void GenerateTextures()
        {
            if (!s_Material)
            {
                s_Material = MaterialUtils.CreateMaterial("Hidden/App UI/CircularProgress");
                if (!s_Material)
                {
                    ReleaseTextures();
                    return;
                }
            }

            var rect = contentRect;

            if (!rect.IsValid())
            {
                ReleaseTextures();
                return;
            }

            var dpi = Mathf.Max(Platform.scaleFactor, 1f);
            var rectSize = rect.size * dpi;

            if (!rectSize.IsValidForTextureSize())
            {
                ReleaseTextures();
                return;
            }

            if (m_RT && (Mathf.Abs(m_RT.width - rectSize.x) > 1 || Mathf.Abs(m_RT.height - rectSize.y) > 1))
                ReleaseTextures();

            if (!m_RT)
                m_RT = RenderTexture.GetTemporary((int) rectSize.x, (int) rectSize.y, 24);

            s_Material.SetColor(k_Color, colorOverride);
            s_Material.SetFloat(k_InnerRadius, innerRadius);
            s_Material.SetInt(k_Rounded, roundedProgressCorners ? 1 : 0);
            s_Material.SetFloat(k_Start, 0);
            s_Material.SetFloat(k_End, value);
            s_Material.SetFloat(k_BufferStart, 0);
            s_Material.SetFloat(k_BufferEnd, bufferValue);
            s_Material.SetFloat(k_AA, 2.0f / rectSize.x);
            s_Material.SetVector(k_Phase, TimeUtils.GetCurrentTimeVector());
            s_Material.SetFloat(k_BufferOpacity, bufferOpacity);
            if (variant == Variant.Indeterminate)
                s_Material.EnableKeyword("PROGRESS_INDETERMINATE");
            else
                s_Material.DisableKeyword("PROGRESS_INDETERMINATE");

            var prevRt = RenderTexture.active;
            Graphics.Blit(null, m_RT, s_Material);
            RenderTexture.active = prevRt;
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Defines the UxmlFactory for the CircularProgress.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<CircularProgress, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="CircularProgress"/>.
        /// </summary>
        public new class UxmlTraits : Progress.UxmlTraits { }
#endif
    }
}
