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
        public int SceneID { get; private set; }

        public IEnumerator<float> UnloadScene()
        {
            yield return Timing.WaitForOneFrame;
        }

        public IEnumerator<float> LoadScene(int id, SceneLoadEventHandler handler)
        {
            SceneID = id;
            yield return Timing.WaitForOneFrame;
        }
    }
}
