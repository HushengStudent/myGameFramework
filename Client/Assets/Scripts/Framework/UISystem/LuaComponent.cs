/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2017/12/28 00:32:14
** desc:  Lua组件;
*********************************************************************************/

using LuaInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using LogUtil;

namespace Framework
{
    public class LuaComponent : MonoBehaviour
    {
        [SerializeField]
        public string ComponentName;

        private Button curButton;
        private Image curImage;
        private Slider curSlider;

        private Button CurButton
        {
            get
            {
                if (null == curButton)
                {
                    curButton = this.gameObject.GetComponent<Button>();
                }
                return curButton;
            }
        }

        private Image CurImage
        {
            get
            {
                if (null == curImage)
                {
                    curImage = this.gameObject.GetComponent<Image>();
                }
                return curImage;
            }
        }

        private Slider CurSlider
        {
            get
            {
                if (null == curSlider)
                {
                    curSlider = this.gameObject.GetComponent<Slider>();
                }
                return curSlider;
            }
        }

        public void AddClick(UnityAction callBack)
        {
            if (null == CurButton) LogUtility.PrintError("[LuaComponent]Button is null.");
            CurButton.onClick.RemoveAllListeners();
            CurButton.onClick.AddListener(callBack);
        }

        public void SetSprite(string atlas, string icon)
        {
            if (null == CurImage) LogUtility.PrintError("[LuaComponent]Image is null.");
            //CurImage.sprite = 
            CurImage.SetNativeSize();
        }

        public void SetSliderValue(float value)
        {
            if (null == CurSlider) LogUtility.PrintError("[LuaComponent]Slider is null.");
            CurSlider.value = value;
        }
    }
}
