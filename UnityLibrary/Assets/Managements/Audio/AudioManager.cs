using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public partial class AudioManager : SingletonBehaviour<AudioManager>
{
    public override bool IsLoaded => !this.IsNull() && Bgm.IsLoaded && Sfx.IsLoaded;
    public Bgm Bgm => _bgm;
    public Sfx Sfx => _sfx;

    [SerializeField] private Bgm _bgm;
    [SerializeField] private Sfx _sfx;
}

#if UNITY_EDITOR
public partial class AudioManager
{
    #region Audio/Bgm, Sfx Enum 생성
    private static string _creationPath = $"{Application.dataPath}/Managements/Audio";
    private const string _BGM_FILE_NAME = "BgmName.cs";
    private const string _SFX_FILE_NAME = "SfxName.cs";

    [MenuItem("Audio/Bgm, Sfx Enum 생성", priority = 0)]
    private static void _GenerateBgmSfxNameEnums()
    {
        StringBuilder bgmEnums = new();
        StringBuilder sfxEnums = new();

        string bgmDir = $"{_creationPath}/Bgms";
        string sfxDir = $"{_creationPath}/Sfxs";

        if (!Directory.Exists(bgmDir))
            Directory.CreateDirectory(bgmDir);
        if (!Directory.Exists(sfxDir))
            Directory.CreateDirectory(sfxDir);

        var bgms = Directory.GetFiles($"{_creationPath}/Bgms");
        var sfxs = Directory.GetFiles($"{_creationPath}/Sfxs");

        for (int i = 0; i < bgms.Length; ++i)
        {
            if (bgms[i].EndsWith(".meta"))
                continue;
            var lastSlash = bgms[i].LastIndexOf('\\');
            var firstDot = bgms[i].IndexOf('.');
            bgmEnums.AppendLine($"\t{bgms[i].Substring(lastSlash + 1, firstDot - (lastSlash + 1))},");
        }
        for (int i = 0; i < sfxs.Length; ++i)
        {
            if (sfxs[i].EndsWith(".meta"))
                continue;
            var lastSlash = sfxs[i].LastIndexOf('\\');
            var firstDot = sfxs[i].IndexOf('.');
            sfxEnums.AppendLine($"\t{sfxs[i].Substring(lastSlash + 1, firstDot - (lastSlash + 1))},");
        }

        StringBuilder bgmScript = new();
        StringBuilder sfxScript = new();

        bgmScript.AppendLine("public enum BgmName")
            .AppendLine("{")
            .Append(bgmEnums)
            .Append("}");
        sfxScript.AppendLine("public enum SfxName")
            .AppendLine("{")
            .Append(sfxEnums)
            .Append("}");

        if (!Directory.Exists(_creationPath))
            Directory.CreateDirectory(_creationPath);
        StreamWriter swBgm = new(File.Create($"{_creationPath}/{_BGM_FILE_NAME}"));
        StreamWriter swSfx = new(File.Create($"{_creationPath}/{_SFX_FILE_NAME}"));

        swBgm.Write(bgmScript.ToString());
        swSfx.Write(sfxScript.ToString());
        swBgm.Close();
        swSfx.Close();

        AssetDatabase.Refresh();

        Debug.Log("Generate Bgm/Sfx Enums!");
    }
    #endregion
}
#endif