using UnityEngine;

namespace Unity.AppUI.UI
{
    static class MaterialUtils
    {
        /// <summary>
        /// Creates a material from a shader name.
        /// </summary>
        /// <param name="shaderName"> The name of the shader to create the material from.</param>
        /// <returns> The material created from the shader name. Null if the shader name is invalid.</returns>
        internal static Material CreateMaterial(string shaderName)
        {
            var shader = Shader.Find(shaderName);
            return shader ? new Material(shader)
            {
                hideFlags = HideFlags.HideAndDontSave
            } : null;
        }
    }
}
