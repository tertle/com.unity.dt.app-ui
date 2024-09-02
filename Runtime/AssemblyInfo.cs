using System.Runtime.CompilerServices;
#if UNITY_EDITOR
using UnityEditor.UIElements;
#endif

[assembly: InternalsVisibleTo("Unity.AppUI.Tests")]
[assembly: InternalsVisibleTo("Unity.AppUI.Editor.Tests")]
[assembly: InternalsVisibleTo("Unity.AppUI.Editor")]
[assembly: InternalsVisibleTo("Unity.AppUI.Redux")]
#if UNITY_EDITOR
[assembly: UxmlNamespacePrefix("Unity.AppUI.UI", "appui")]
#endif
