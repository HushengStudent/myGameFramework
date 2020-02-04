/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/06/25 00:04:02
** desc:  CommandBuffer Mgr;
*********************************************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    public class CommandBufferMgr : MonoSingleton<CommandBufferMgr>
    {
        private struct CommandBufferRenderer
        {
            public CBRenderer CBRenderer;
            public Camera Camera;
        }

        private Dictionary<GameObject, CommandBufferRenderer> _cbDict;
        private List<GameObject> _deprecatedList;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _cbDict = new Dictionary<GameObject, CommandBufferRenderer>();
            _deprecatedList = PoolMgr.singleton.GetCsharpList<GameObject>();
        }

        public void DrawRenderer(GameObject go, Camera camera, RawImage rawImage, Material mat = null)
        {

            rawImage.texture = DrawRenderer(go, camera, mat);
        }

        public void DrawRenderer(GameObject go, Camera camera, Renderer renderer, Material mat = null)
        {
            renderer.sharedMaterial.mainTexture = DrawRenderer(go, camera, mat);
        }

        public void ReleaseRenderer(GameObject go)
        {
            CommandBufferRenderer commandBufferRenderer;
            if (_cbDict.TryGetValue(go, out commandBufferRenderer))
            {
                commandBufferRenderer.CBRenderer.Deprecated = true;
            }
        }

        private RenderTexture DrawRenderer(GameObject go, Camera camera, Material mat = null)
        {
            CommandBufferRenderer commandBufferRenderer;
            if (_cbDict.TryGetValue(go, out commandBufferRenderer))
            {
                PoolMgr.singleton.ReleaseCsharpObject(commandBufferRenderer.CBRenderer);
                _cbDict.Remove(go);
            }
            commandBufferRenderer = new CommandBufferRenderer()
            {
                CBRenderer = PoolMgr.singleton.GetCsharpObject<CBRenderer>(),
                Camera = camera
            };
            commandBufferRenderer.CBRenderer.Initialize(go, camera, mat);
            if (commandBufferRenderer.CBRenderer.Deprecated)
            {
                PoolMgr.singleton.ReleaseCsharpObject(commandBufferRenderer.CBRenderer);
                return null;
            }
            _cbDict[go] = commandBufferRenderer;
            camera.gameObject.AddOrGetComponent<CameraCBExecutor>();
            return commandBufferRenderer.CBRenderer.RenderTexture;
        }

        public void Execute(Camera camera)
        {
            if (!camera)
            {
                return;
            }
            if (_cbDict != null && _cbDict.Count > 0)
            {
                foreach (var temp in _cbDict)
                {
                    var go = temp.Key;
                    var cb = temp.Value.CBRenderer;
                    if (!go)
                    {
                        cb.Deprecated = true;
                    }
                    if (cb.Deprecated)
                    {
                        _deprecatedList.Add(temp.Key);
                        continue;
                    }
                    else
                    {
                        if (cb.Camera == camera)
                        {
                            cb.Execute();
                        }
                    }
                }
            }
            if (_deprecatedList.Count > 0)
            {
                for (int i = 0; i < _deprecatedList.Count; i++)
                {
                    var go = _deprecatedList[i];
                    var commandBufferRenderer = _cbDict[go];
                    PoolMgr.singleton.ReleaseCsharpObject(commandBufferRenderer.CBRenderer);
                    _cbDict.Remove(go);
                }
                _deprecatedList.Clear();
            }
        }
    }
}