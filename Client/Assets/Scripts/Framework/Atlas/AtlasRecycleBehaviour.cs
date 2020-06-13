/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/08/18 23:26:14
** desc:  ÕººØ Õ∑≈;
*********************************************************************************/

using UnityEngine;

namespace Framework
{
    public class AtlasRecycleBehaviour : MonoBehaviour
    {
        private string _atlasPath;
        private string _spriteName;
        private bool _isRelease;

        public void OnInit(string atlasPath, string spriteName)
        {
            _atlasPath = atlasPath;
            _spriteName = spriteName;
            _isRelease = false;
        }

        public void OnRelease()
        {
            if (!_isRelease)
            {
                _isRelease = true;
                AtlasMgr.singleton.ReleaseSprite(this, _atlasPath);
            }
        }

        private void OnDestroy()
        {
            if (_isRelease)
            {
                return;
            }
            if (AtlasMgr.ApplicationIsPlaying)
            {
                LogHelper.PrintWarning($"[AtlasRecycleBehaviour]auto recycle atlas at:{name}");
                OnRelease();
            }
        }
    }
}
