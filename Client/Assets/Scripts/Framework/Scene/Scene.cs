/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/17 03:29:00
** desc:  场景类;
*********************************************************************************/

using System.Collections;

namespace Framework.SceneModule
{
    public class Scene
    {
        public int SceneID { get; private set; }

        public IEnumerator UnloadScene()
        {
            yield return CoroutineMgr.WaitForEndOfFrame;
        }

        public IEnumerator LoadScene(int id, SceneLoadEventHandler handler)
        {
            SceneID = id;
            yield return CoroutineMgr.WaitForEndOfFrame;
        }
    }
}
