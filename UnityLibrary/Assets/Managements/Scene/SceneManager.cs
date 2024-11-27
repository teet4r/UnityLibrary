using Cysharp.Threading.Tasks;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;
using UnityEngine;
using System.IO;
using System.Text;
using System;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public partial class SceneManager : SingletonBehaviour<SceneManager>
{
    public async UniTask LoadSceneAsync(SceneName sceneName, Func<UniTask> enterCall = default, Func<UniTask> exitCall = default)
    {
        if (enterCall != default)
            await enterCall.Invoke();
        await UnitySceneManager.LoadSceneAsync((int)SceneName.Empty);
        
        if (exitCall != default)
            await exitCall.Invoke();
        await UnitySceneManager.LoadSceneAsync((int)sceneName);
    }
}

#if UNITY_EDITOR
public partial class SceneManager
{
    #region Scene/현재 Scene에서 시작
    private const string _MENU_PATH = "Scene/현재 Scene에서 시작";
    private const string _START_CURRENT_SCENE = "StartCurrentScene";

    /// <summary>
    /// 에디터 처음 열 때 해당 메뉴 체크 여부 확인 후 체크 마크 표시
    /// </summary>
    [InitializeOnLoadMethod]
    private static void _Run()
    {
        EditorApplication.delayCall += () =>
        {
            var startCurrentScene = EditorPrefs.GetBool(_START_CURRENT_SCENE, true);
            Menu.SetChecked(_MENU_PATH, startCurrentScene);

            SceneAsset sa = null;
            if (!startCurrentScene && EditorBuildSettings.scenes.Length > 0)
            {
                var firstScenePath = EditorBuildSettings.scenes[0].path;
                sa = AssetDatabase.LoadAssetAtPath<SceneAsset>(firstScenePath);
            }
            EditorSceneManager.playModeStartScene = sa;
        };
    }

    [MenuItem(_MENU_PATH, priority = 0)]
    private static void _CurrentSceneStart()
    {
        var startCurrentScene = EditorPrefs.GetBool(_START_CURRENT_SCENE, true);
        EditorPrefs.SetBool(_START_CURRENT_SCENE, !startCurrentScene);
        Menu.SetChecked(_MENU_PATH, !startCurrentScene);

        SceneAsset sa = null;
        if (startCurrentScene && EditorBuildSettings.scenes.Length > 0)
        {
            var firstScenePath = EditorBuildSettings.scenes[0].path;
            sa = AssetDatabase.LoadAssetAtPath<SceneAsset>(firstScenePath);
        }
        EditorSceneManager.playModeStartScene = sa;
    }
    #endregion
    #region Scene/Scene Enum 생성
    private static string _creationPath = $"{Application.dataPath}/Managements/Scene";
    private const string _FILE_NAME = "SceneName.cs";

    [MenuItem("Scene/Scene Enum 생성", priority = 100)]
    private static void _GenerateSceneNameEnums()
    {
        // 활성화되어있는 씬에 대해서만 Enum 생성
        StringBuilder enums = new();

        foreach (var scene in EditorBuildSettings.scenes)
        {
            int lastSlash = scene.path.LastIndexOf('/');
            int period = scene.path.IndexOf('.', lastSlash);

            enums.AppendLine($"\t{scene.path.Substring(lastSlash + 1, period - (lastSlash + 1))},");
        }

        StringBuilder script = new();

        script.AppendLine("public enum SceneName")
            .AppendLine("{")
            .Append(enums)
            .Append("}");

        if (!Directory.Exists(_creationPath))
            Directory.CreateDirectory(_creationPath);
        StreamWriter sw = new(File.Create($"{_creationPath}/{_FILE_NAME}"));

        sw.Write(script.ToString());
        sw.Close();

        AssetDatabase.Refresh();

        Debug.Log("Generate Scene Enums!");
    }
    #endregion
}
#endif