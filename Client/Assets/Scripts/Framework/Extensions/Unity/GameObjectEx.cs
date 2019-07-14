/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/31 23:57:59
** desc:  GameObject扩展;
*********************************************************************************/

using UnityEngine;

namespace Framework
{
    public delegate void GameObjectExLoadFinishHandler(GameObjectEx go);
    public delegate void GameObjectExDestroyHandler(GameObjectEx go);

    public class GameObjectEx
    {
        private Vector3 _position = Vector3.zero;
        private Vector3 _scale = Vector3.zero;
        private Vector3 _rotation = Vector3.zero;
        private AssetBundleAssetProxy proxy;
        private GameObjectExLoadFinishHandler _loadFinishHandler;
        private GameObjectExDestroyHandler _destroyHandler;

        public bool IsLoadFinish { get; private set; }
        public GameObject gameObject { get; private set; }
        public Transform Trans { get; private set; }
        public AbsEntity Entity { get; private set; }
        public string ResPath { get; private set; }

        public void Init(AbsEntity entity, string path, bool isAsync = true)
        {
            Entity = entity;
            ResPath = path;
            IsLoadFinish = false;
            proxy = ResourceMgr.Instance.LoadAssetAsync(ResPath);
            proxy.AddLoadFinishCallBack(() =>
            {
                gameObject = proxy.GetInstantiateObject<GameObject>();
                gameObject.name = entity.UID.ToString();
                IsLoadFinish = true;
                Trans = gameObject.transform;
                if (_loadFinishHandler != null)
                {
                    _loadFinishHandler(this);
                }
            });
        }

        public void Init(AbsEntity entity, bool isAsync = true)
        {
            Entity = entity;
            IsLoadFinish = false;
            ModelComponent modelComp = entity.GetComponent<ModelComponent>();

        }

        public void Uninit()
        {
            if (_destroyHandler != null)
            {
                _destroyHandler(this);
            }
            if (IsLoadFinish)
            {
                proxy.ReleaseInstantiateObject<GameObject>(gameObject);
            }
            proxy.UnloadProxy();
            Trans = null;
            Entity = null;
            ResPath = string.Empty;
            _loadFinishHandler = null;
            _destroyHandler = null;
        }

        public void AddLoadFinishHandler(GameObjectExLoadFinishHandler handler)
        {
            _loadFinishHandler += handler;
        }

        public void RemoveLoadFinishHandler(GameObjectExLoadFinishHandler handler)
        {
            _loadFinishHandler -= handler;
        }

        public void AddDestroyHandler(GameObjectExDestroyHandler handler)
        {
            _destroyHandler += handler;
        }

        public void SetLocalPosition(float x, float y, float z)
        {
            _position = new Vector3(x, y, z);
            if (gameObject)
            {
                gameObject.transform.localPosition = _position;
            }
        }

        public void SetLocalScale(float x, float y, float z)
        {
            _scale = new Vector3(x, y, z);
            if (gameObject)
            {
                gameObject.transform.localScale = _scale;
            }
        }

        public void SetLocalRotation(float x, float y, float z)
        {
            _rotation = new Vector3(x, y, z);
            if (gameObject)
            {
                gameObject.transform.localRotation = Quaternion.Euler(_rotation);
            }
        }
    }
}