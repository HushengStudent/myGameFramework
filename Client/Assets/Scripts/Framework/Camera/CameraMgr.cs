/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/06/24 01:45:07
** desc:  �������;
*********************************************************************************/

using UnityEngine;

namespace Framework
{
    public class CameraMgr : MonoSingleton<CameraMgr>, ISingleton
    {
        public Camera MainCamera { get; private set; }
        public Camera MainUICamera { get; private set; }

        public void OnInitialize()
        {
        }
    }
}