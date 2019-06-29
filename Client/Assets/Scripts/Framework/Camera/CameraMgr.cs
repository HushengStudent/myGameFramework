/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/06/24 01:45:07
** desc:  相机管理;
*********************************************************************************/

using UnityEngine;

namespace Framework
{
    public class CameraMgr : MonoSingleton<CameraMgr>
    {
        public Camera MainCamera { get; private set; }
        public Camera MainUICamera { get; private set; }
    }
}