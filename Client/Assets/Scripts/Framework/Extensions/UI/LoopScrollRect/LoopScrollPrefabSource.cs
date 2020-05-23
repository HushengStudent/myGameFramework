/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/06/08 21:55:26
** desc:  #####
*********************************************************************************/

using Framework;
using System.Collections.Generic;

namespace UnityEngine.UI
{
    [System.Serializable]
    public class LoopScrollPrefabSource
    {
        public GameObject _prefab;

        private Queue<GameObject> _prefabQueue = new Queue<GameObject>();

        public virtual GameObject GetObject(Transform parent)
        {
            GameObject go;
            if (_prefabQueue.Count > 0)
            {
                go = _prefabQueue.Dequeue();
            }
            else
            {
                go = Object.Instantiate(_prefab);
            }
            go.transform.SetParent(parent, false);
            go.transform.localScale = _prefab.transform.localScale;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.SetActive(true);
            return go;
        }

        public virtual void ReturnObject(Transform trans)
        {
            GameObject go = trans.gameObject;
            go.SetActive(false);
            go.transform.SetParent(PoolMgr.Singleton.Root.transform);
            _prefabQueue.Enqueue(go);
        }

        public void ClearPrefabSource()
        {
            while (_prefabQueue.Count > 0)
            {
                GameObject go = _prefabQueue.Dequeue();
                Object.Destroy(go);
            }
        }
    }
}
