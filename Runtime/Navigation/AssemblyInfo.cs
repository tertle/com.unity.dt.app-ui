using System.Runtime.CompilerServices;
#if UNITY_EDITOR
using UnityEditor.UIElements;
#endif

[assembly: InternalsVisibleTo("Unity.AppUI.Tests")]
[assembly: InternalsVisibleTo("Unity.AppUI.Editor")]
#if UNITY_EDITOR
[assembly: UxmlNamespacePrefix("Unity.AppUI.Navigation", "nav")]
#endif
