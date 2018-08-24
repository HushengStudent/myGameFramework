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
        private Scene _scene = null;

        public Scene CurScene { get { return _scene; } }

        protected override void InitEx()
        {
            _scene = null;
            LogUtil.LogUtility.Print("[SceneMgr]SceneMgr init!");
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
                _scene = new Scene();
            }
            IEnumerator<float> loadItor = CurScene.LoadScene(sceneId, handler);
            while (loadItor.MoveNext())
            {
                yield return Timing.WaitForOneFrame;
            }
        }
    }
}
