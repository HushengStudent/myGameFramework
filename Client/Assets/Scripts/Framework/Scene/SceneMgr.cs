/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2017/12/25 00:28:32
** desc:  场景管理;
*********************************************************************************/

using System.Collections;

namespace Framework
{
    public class SceneMgr : Singleton<SceneMgr>
    {
        public Scene CurScene { get; private set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            CurScene = null;
        }

        public IEnumerator TransToScene(int sceneId, SceneLoadEventHandler handler)
        {
            if (CurScene != null)
            {
                var unloadItor = CurScene.UnloadScene();
                while (unloadItor.MoveNext())
                {
                    yield return CoroutineMgr.WaitForEndOfFrame;
                }
            }
            else
            {
                CurScene = new Scene();
            }
            var itor = CurScene.LoadScene(sceneId, handler);
            while (itor.MoveNext())
            {
                yield return CoroutineMgr.WaitForEndOfFrame;
            }
        }
    }
}
