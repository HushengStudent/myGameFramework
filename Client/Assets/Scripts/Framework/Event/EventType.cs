/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/03/22 23:53:32
** desc:  事件类型;
*********************************************************************************/

namespace Framework.EventModule
{
    public enum EventType : int
    {
        RESOURCE_MGR_INIT = 1,   //资源管理器初始化;
        POOL_MGR_INIT = 2,     //对象池初始化;
        CAMERA_MGR_INIT = 3,   //相机初始化;

        INIT_GAME_FINISH_EVENT = 4,   //初始化游戏完成;


        ON_SCENE_UNLOAD = 11,   //卸载场景;
        ON_SCENE_LOADED = 12,   //加载场景;


    }
}
