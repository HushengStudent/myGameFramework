/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/12/29 20:59:07
** desc:  相机CommandBuffer执行者;
*********************************************************************************/

using UnityEngine;

namespace Framework
{
    public class CameraCBExecutor : MonoBehaviour
    {
        private Camera _camera;

        private Camera Camera
        {
            get
            {
                if (!_camera)
                {
                    _camera = GetComponent<Camera>();
                }
                return _camera;
            }
        }

        private void OnPostRender()
        {
            CommandBufferMgr.singleton.Execute(Camera);
        }
    }
}
