/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2017/12/25 00:28:32
** desc:  场景管理;
*********************************************************************************/

using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Framework
{
    public class SceneMgr : Singleton<SceneMgr>
    {
        public Scene CurScene { get; private set; }

        public override void Init()
        {
            base.Init();
            CurScene = null;
        }

        public IEnumerator<float> TransToScene(int sceneId, SceneLoadEventHandler handler)
        {
            if (CurScene != null)
            {
                IEnumerator<float> unloadItor = CurScene.UnloadScene();
                while (unloadItor.MoveNext())
                {
                    yield return Timing.WaitForOneFrame;
                }
            }
            else
            {
                CurScene = new Scene();
            }
            IEnumerator<float> loadItor = CurScene.LoadScene(sceneId, handler);
            while (loadItor.MoveNext())
            {
                yield return Timing.WaitForOneFrame;
            }
        }
    }
}
