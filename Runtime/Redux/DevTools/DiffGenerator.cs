using System;
using System.Collections.Generic;
using System.Text;

namespace Unity.AppUI.Redux.DevTools
{
    static class DiffGenerator
    {
        internal static string CreateDiffRichText(string oldText, string newText)
        {
            // Split the texts into lines
            var oldLines = oldText.Split('\n');
            var newLines = newText.Split('\n');

            // Compute LCS table
            var lcs = ComputeLcsTable(oldLines, newLines);

            // Build diff output
            var diff = BuildDiff(oldLines, newLines, lcs);

            return diff;
        }

        // Computes the Longest Common Subsequence (LCS) table for lines
        static int[,] ComputeLcsTable(string[] a, string[] b)
        {
            var n = a.Length;
            var m = b.Length;
            var lcs = new int[n + 1, m + 1];

            // Initialize the LCS table
            for (var i = 0; i <= n; i++)
                lcs[i, 0] = 0;
            for (var j = 0; j <= m; j++)
                lcs[0, j] = 0;

            // Fill in the LCS table
            for (var i = 1; i <= n; i++)
            {
                var lineA = a[i - 1];
                for (var j = 1; j <= m; j++)
                {
                    var lineB = b[j - 1];
                    if (lineA == lineB)
                    {
                        lcs[i, j] = lcs[i - 1, j - 1] + 1;
                    }
                    else
                    {
                        lcs[i, j] = Math.Max(lcs[i - 1, j], lcs[i, j - 1]);
                    }
                }
            }

            return lcs;
        }

        // Builds the diff output by backtracking through the LCS table
        static string BuildDiff(string[] a, string[] b, int[,] lcs)
        {
            var i = a.Length;
            var j = b.Length;

            // Use a stack to reverse the order during backtracking
            var ops = new Stack<System.Action>();
            var builder = new StringBuilder();

            while (i > 0 || j > 0)
            {
                if (i > 0 && j > 0 && a[i - 1] == b[j - 1])
                {
                    // No change, line is in both sequences
                    var index = i - 1; // Capture variable for closure
                    ops.Push(() => builder.AppendLine(EscapeText(a[index])));
                    i--;
                    j--;
                }
                else if (j > 0 && (i == 0 || lcs[i, j - 1] >= lcs[i - 1, j]))
                {
                    // Insertion
                    var index = j - 1;
                    ops.Push(() => builder.AppendLine(WrapInserted(EscapeText(b[index]))));
                    j--;
                }
                else if (i > 0 && (j == 0 || lcs[i, j - 1] < lcs[i - 1, j]))
                {
                    // Deletion
                    var index = i - 1;
                    ops.Push(() => builder.AppendLine(WrapDeleted(EscapeText(a[index]))));
                    i--;
                }
            }

            // Execute the actions in order
            while (ops.Count > 0)
            {
                ops.Pop()();
            }

            return builder.ToString();
        }

        // Wraps inserted text with green color tags
        static string WrapInserted(string text)
        {
            return $"<color=#3BB047>+{text}</color>";
        }

        // Wraps deleted text with red color tags
        static string WrapDeleted(string text)
        {
            return $"<color=#BF4444>-{text}</color>";
        }

        // Escapes special characters in the text
        static string EscapeText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return text
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;");
        }
    }
}
