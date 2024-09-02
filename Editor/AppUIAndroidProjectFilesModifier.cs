#if UNITY_ANDROID
using UnityEngine;
using UnityEditor;
using UnityEditor.Android;
using System.IO;

#if !UNITY_2023_2_OR_NEWER
class AppUIAndroidBuildProcessor : IPostGenerateGradleAndroidProject
{
    const string k_PackagePath = "Packages/com.unity.dt.app-ui";
    public int callbackOrder => 0;

    static string GetAbsolutePath(string relativePath)
    {
        return Path.Combine(
            UnityEditor.PackageManager.PackageInfo.FindForAssetPath(relativePath).resolvedPath,
            relativePath[(k_PackagePath.Length + 1)..]);
    }

    public void OnPostGenerateGradleAndroidProject(string path)
    {
        var activityJavaPath = @"Packages/com.unity.dt.app-ui/Runtime/Core/Platform/Android/Plugins/Android/AppUIActivity.java";
        var gameActivityJavaPath = @"Packages/com.unity.dt.app-ui/Runtime/Core/Platform/Android/Plugins/Android/AppUIGameActivity.java";

        var activityJavaSrcPath = GetAbsolutePath(activityJavaPath);
        var gameActivitySrcPath = GetAbsolutePath(gameActivityJavaPath);

        var activityJavaDstPath = Path.Combine(path, $"src/main/java/com/unity3d/player/appui/{Path.GetFileName(activityJavaPath)}");
        var gameActivityDstPath = Path.Combine(path, $"src/main/java/com/unity3d/player/appui/{Path.GetFileName(gameActivityJavaPath)}");

        Debug.Log($"Activity Java Path: {activityJavaSrcPath} -> {activityJavaDstPath}");
        Debug.Log($"Game Activity Java Path: {gameActivitySrcPath} -> {gameActivityDstPath}");

#if !UNITY_2023_2_OR_NEWER
        File.Copy(activityJavaSrcPath, activityJavaDstPath, true);
#else
        if (PlayerSettings.Android.applicationEntry.HasFlag(AndroidApplicationEntry.GameActivity))
            File.Copy(gameActivitySrcPath, gameActivityDstPath, true);
        else if (File.Exists(gameActivityDstPath))
            File.Delete(gameActivityDstPath);

        if (PlayerSettings.Android.applicationEntry.HasFlag(AndroidApplicationEntry.Activity))
            File.Copy(activityJavaSrcPath, activityJavaDstPath, true);
        else if (File.Exists(activityJavaDstPath))
            File.Delete(activityJavaDstPath);
#endif
    }
}

#else

public class AppUIAndroidProjectFilesModifier : AndroidProjectFilesModifier
{
    const string k_PackagePath = "Packages/com.unity.dt.app-ui";

    static string GetAbsolutePath(string relativePath)
    {
        return Path.Combine(
            UnityEditor.PackageManager.PackageInfo.FindForAssetPath(relativePath).resolvedPath,
            relativePath[(k_PackagePath.Length + 1)..]);
    }

    public override AndroidProjectFilesModifierContext Setup()
    {
        var ctx = new AndroidProjectFilesModifierContext();

        var activityJavaPath = @"Packages/com.unity.dt.app-ui/Runtime/Core/Platform/Android/Plugins/Android/AppUIActivity.java";
        var gameActivityJavaPath = @"Packages/com.unity.dt.app-ui/Runtime/Core/Platform/Android/Plugins/Android/AppUIGameActivity.java";

        var activityJavaSrcPath = GetAbsolutePath(activityJavaPath);
        var gameActivityJavaSrcPath = GetAbsolutePath(gameActivityJavaPath);

        if (PlayerSettings.Android.applicationEntry.HasFlag(AndroidApplicationEntry.GameActivity))
            ctx.AddFileToCopy(
                gameActivityJavaSrcPath,
                $"unityLibrary/src/main/java/com/unity3d/player/appui/AppUIGameActivity.java");

        if (PlayerSettings.Android.applicationEntry.HasFlag(AndroidApplicationEntry.Activity))
            ctx.AddFileToCopy(
                activityJavaSrcPath,
                $"unityLibrary/src/main/java/com/unity3d/player/appui/AppUIActivity.java");
        return ctx;
    }

    public override void OnModifyAndroidProjectFiles(AndroidProjectFiles projectFiles)
    {
    }
}

#endif
#endif