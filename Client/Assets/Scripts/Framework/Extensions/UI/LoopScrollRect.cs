/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/16 00:45:42
** desc:  UI复用;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    public enum ScroolRectDirection
    {
        Horizontal = 0,
        Vertical = 1
    }

    public delegate void OnLoopItemUpdate(int index, GameObject go);

    [RequireComponent(typeof(RectTransform))]
    public class LoopScrollRect : Graphic
    {
        /// <summary>
        /// 模板;
        /// </summary>
        [SerializeField]
        private GameObject _prefab;
        [SerializeField]
        private ScroolRectDirection _direction = ScroolRectDirection.Vertical;
        [SerializeField]
        private Vector2 _itemSize;
        /// <summary>
        /// 并列数量;
        /// </summary>
        [SerializeField]
        private int _size = 1;
        /// <summary>
        /// 水平偏移;
        /// </summary>
        [SerializeField]
        private float _offsetX = 0f;
        /// <summary>
        /// 垂直偏移;
        /// </summary>
        [SerializeField]
        private float _offsetY = 0f;

        private bool _initState = false;
        private float _dragLength = 0f;
        private int _count = 0;
        private List<GameObject> _prefabList;
        private OnLoopItemUpdate onLoopItemUpdate;

        protected override void Awake()
        {
            base.Awake();
            material = defaultGraphicMaterial;
            color = new Color(0, 0, 0, 0);
        }

        public void InitScrollRect(OnLoopItemUpdate callback)
        {
            if (_initState)
            {
                LogUtil.LogUtility.PrintWarning("[LoopScrollRect]can not init LoopScrollRect again!");
                return;
            }
            _initState = true;
            _prefabList = new List<GameObject>();
            _dragLength = 0f;
            _count = 0;
            onLoopItemUpdate = callback;
        }

        public void SetCount(int count)
        {
            if (_prefab == null)
            {
                LogUtil.LogUtility.PrintWarning("[LoopScrollRect]not set prefab!");
                return;
            }
            if (!_initState)
            {
                LogUtil.LogUtility.PrintWarning("[LoopScrollRect]not init!");
                return;
            }
            if (count < 0)
            {
                LogUtil.LogUtility.PrintError("[LoopScrollRect]count can not <1!");
                return;
            }
            _count = count;
        }

        private void ClearList()
        {

        }

        private void MoveList()
        {

        }

        private void ResetPosition()
        {

        }

        private void DragList()
        {

        }
    }
}
