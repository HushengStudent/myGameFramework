/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2017/12/26 00:03:35
** desc:  游戏入口;
*********************************************************************************/

using UnityEngine;
using Framework;

public class Main : MonoBehaviour
{
    [SerializeField]
    public bool AssetBundleModel = false;

    void Awake()
    {
        StartGame();
    }

    /// <summary> 初始化游戏; </summary>
    private void StartGame()
    {
        LogHelper.PrintYellow("StartGame");

#if UNITY_EDITOR

        GameMgr.AssetBundleModel = AssetBundleModel;

        UnityEditor.EditorApplication.update += () =>
        {
            if (UnityEditor.EditorApplication.isPlaying && UnityEditor.EditorApplication.isCompiling)
            {
                LogHelper.PrintError("script update.");
                UnityEditor.EditorApplication.isPlaying = false;
            }
        };

#endif

        GameMgr.singleton.Launch();

    }
}
