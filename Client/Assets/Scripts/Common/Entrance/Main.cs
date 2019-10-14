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

    /// 初始化游戏;
    private void StartGame()
    {
        GameMgr.AssetBundleModel = AssetBundleModel;

        GameMgr.Instance.Launch();
    }
}
