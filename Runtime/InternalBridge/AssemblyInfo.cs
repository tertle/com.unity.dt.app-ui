using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Unity.AppUI")]
[assembly: InternalsVisibleTo("Unity.AppUI.MVVM")]
[assembly: InternalsVisibleTo("Unity.AppUI.Navigation")]
[assembly: InternalsVisibleTo("Unity.AppUI.Redux")]
[assembly: InternalsVisibleTo("Unity.AppUI.Undo")]
[assembly: InternalsVisibleTo("Unity.AppUI.Tests")]

#if UNITY_EDITOR
[assembly: InternalsVisibleTo("Unity.AppUI.Editor")]
[assembly: InternalsVisibleTo("Unity.AppUI.Navigation.Editor")]
[assembly: InternalsVisibleTo("Unity.AppUI.Tests.Editor")]
#endif