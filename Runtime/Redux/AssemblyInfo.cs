using System.Runtime.CompilerServices;
#if UNITY_EDITOR
using UnityEditor.UIElements;
#endif

[assembly: InternalsVisibleTo("Unity.AppUI.Redux.Editor")]
[assembly: InternalsVisibleTo("Unity.AppUI.Tests")]
#if UNITY_EDITOR
[assembly: UxmlNamespacePrefix("Unity.AppUI.Redux", "redux")]
#endif
