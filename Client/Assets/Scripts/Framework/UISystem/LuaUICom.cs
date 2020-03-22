/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2017/12/28 00:32:14
** desc:  Lua组件;
*********************************************************************************/

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Framework
{
    public class LuaUICom : MonoBehaviour
    {
        [HideInInspector]
        [SerializeField]
        public LuaUIPanel LuaUIPanel;

        [HideInInspector]
        [SerializeField]
        public LuaUITemplate LuaUITemplate;

        [SerializeField]
        public string LuaUIComName;

        private Button _button;
        private Image _image;
        private Slider _slider;

        private Button Button
        {
            get
            {
                if (null == _button)
                {
                    _button = gameObject.GetComponent<Button>();
                }
                return _button;
            }
        }

        private Image Image
        {
            get
            {
                if (null == _image)
                {
                    _image = gameObject.GetComponent<Image>();
                }
                return _image;
            }
        }

        private Slider Slider
        {
            get
            {
                if (null == _slider)
                {
                    _slider = gameObject.GetComponent<Slider>();
                }
                return _slider;
            }
        }

        public void AddClick(UnityAction callBack)
        {
            if (null == Button)
            {
                LogHelper.PrintError("[LuaUICom]Button is null.");
                return;
            }
            Button.onClick.RemoveAllListeners();
            Button.onClick.AddListener(callBack);
        }

        public void SetSprite(string atlas, string icon)
        {
            if (null == Image)
            {
                LogHelper.PrintError("[LuaUICom]Image is null.");
                return;
            }
            //Image.sprite = 
            Image.SetNativeSize();
        }

        public void SetSliderValue(float value)
        {
            if (null == Slider)
            {
                LogHelper.PrintError("[LuaUICom]Slider is null.");
                return;
            }
            Slider.value = value;
        }

        public void SetGray(bool gray)
        {
            if (null == Image)
            {
                LogHelper.PrintError("[LuaUICom]Image is null.");
                return;
            }
            Image.color = gray ? Color.black : Color.white;
        }

        private void OnDestroy()
        {
            if (_button)
            {
                _button.onClick.RemoveAllListeners();
            }
        }
    }
}
