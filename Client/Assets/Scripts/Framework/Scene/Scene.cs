/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/17 03:29:00
** desc:  场景类;
*********************************************************************************/

using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class Scene
    {
        private int _sceneID;

        public int SceneID { get { return _sceneID; } }

        public IEnumerator<float> UnloadScene()
        {
            yield return Timing.WaitForOneFrame;
        }

        public IEnumerator<float> LoadScene(int id, SceneLoadEventHandler handler)
        {
            _sceneID = id;
            yield return Timing.WaitForOneFrame;
        }
    }
}
