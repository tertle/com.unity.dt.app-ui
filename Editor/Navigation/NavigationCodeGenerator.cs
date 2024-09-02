using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Unity.AppUI.Navigation.Editor
{
    /// <summary>
    /// Code generator for navigation graphs.
    /// </summary>
    class NavigationCodeGenerator
    {
        /// <summary>
        /// Generates code for the given <see cref="NavGraphViewAsset"/>.
        /// </summary>
        /// <param name="asset"> The asset to generate code for. </param>
        public static void GenerateCode(NavGraphViewAsset asset)
        {
            var path = AssetDatabase.GetAssetPath(asset);
            var codePath = path.Replace(".asset", "");

            var output = EditorUtility.SaveFilePanel("Save generated code", Path.GetDirectoryName(codePath), Path.GetFileName(codePath), "cs");

            if (string.IsNullOrEmpty(output))
                return;

            using (var streamWriter = new StreamWriter(output, false))
            {
                streamWriter.WriteLine("// This file is auto-generated. Do not edit it directly.");
                streamWriter.WriteLine("// Date: " + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                streamWriter.WriteLine("using System;");
                streamWriter.WriteLine("using System.Collections.Generic;");
                streamWriter.WriteLine("using UnityEngine;");
                streamWriter.WriteLine("");
                streamWriter.WriteLine("namespace Unity.AppUI.Navigation.Generated");
                streamWriter.WriteLine("{");

                streamWriter.WriteLine("    public static partial class Actions");
                streamWriter.WriteLine("    {");
                foreach (var action in asset.nodes.Where(n => n is NavAction).Cast<NavAction>())
                {
                    streamWriter.WriteLine("        public const string " + GetVariableName(action.name) + " = \"" + action.name + "\";");
                }
                streamWriter.WriteLine("    }");

                streamWriter.WriteLine("    public static partial class Destinations");
                streamWriter.WriteLine("    {");
                foreach (var destination in asset.nodes.Where(n => n is NavDestination).Cast<NavDestination>())
                {
                    streamWriter.WriteLine("        public const string " + GetVariableName(destination.name) + " = \"" + destination.name + "\";");
                }
                streamWriter.WriteLine("    }");

                streamWriter.WriteLine("    public static partial class Graphs");
                streamWriter.WriteLine("    {");
                foreach (var graph in asset.nodes.Where(n => n is NavGraph).Cast<NavGraph>())
                {
                    streamWriter.WriteLine("        public const string " + GetVariableName(graph.name) + " = \"" + graph.name + "\";");
                }
                streamWriter.WriteLine("    }");

                streamWriter.WriteLine("}");
            }

            AssetDatabase.Refresh();
            Debug.Log("Generated code for navigation graph " + asset.name + " at " + output);
        }

        static string GetVariableName(string name)
        {
            return name.Replace(" ", "_").ToLower();
        }
    }
}
