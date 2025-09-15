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
    /// LinearProgress UI element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class LinearProgress : Progress
    {
        static readonly int k_Start = Shader.PropertyToID("_Start");

        static readonly int k_End = Shader.PropertyToID("_End");

        static readonly int k_Rounded = Shader.PropertyToID("_Rounded");

        static readonly int k_BufferStart = Shader.PropertyToID("_BufferStart");

        static readonly int k_BufferEnd = Shader.PropertyToID("_BufferEnd");

        static readonly int k_Color = Shader.PropertyToID("_Color");

        static readonly int k_AA = Shader.PropertyToID("_AA");

        static readonly int k_Ratio = Shader.PropertyToID("_Ratio");

        static readonly int k_Padding = Shader.PropertyToID("_Padding");

        static readonly int k_BufferOpacity = Shader.PropertyToID("_BufferOpacity");

        static Material s_Material;

        /// <summary>
        /// The Progress main styling class.
        /// </summary>
        public new const string ussClassName = "appui-linear-progress";

        static readonly int k_Phase = Shader.PropertyToID("_Phase");

        /// <summary>
        /// Default constructor.
        /// </summary>
        public LinearProgress()
        {
            AddToClassList(ussClassName);
        }

        /// <summary>
        /// Generates the textures for the progress element.
        /// </summary>
        protected override void GenerateTextures()
        {
            if (!s_Material)
            {
                s_Material = MaterialUtils.CreateMaterial("Hidden/App UI/LinearProgress");
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
                m_RT = RenderTexture.GetTemporary((int)rectSize.x, (int)rectSize.y, 24);

            s_Material.SetColor(k_Color, colorOverride);
            s_Material.SetInt(k_Rounded, roundedProgressCorners ? 1 : 0);
            s_Material.SetFloat(k_Start, 0);
            s_Material.SetFloat(k_End, value);
            s_Material.SetFloat(k_BufferStart, 0);
            s_Material.SetFloat(k_BufferEnd, bufferValue);
            s_Material.SetFloat(k_BufferOpacity, bufferOpacity);
            s_Material.SetFloat(k_AA, 2.0f / rectSize.x);
            s_Material.SetVector(k_Phase, TimeUtils.GetCurrentTimeVector());
            s_Material.SetFloat(k_Ratio, rectSize.x / rectSize.y);
            s_Material.SetFloat(k_Padding, roundedProgressCorners ? rect.height * 0.5f / rect.width : 0);
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
        /// Factory class to instantiate a <see cref="LinearProgress"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<LinearProgress, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="LinearProgress"/>.
        /// </summary>
        public new class UxmlTraits : Progress.UxmlTraits { }

#endif
    }
}
