/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/11/11 22:22:29
** desc:  合并模型组件;
*********************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public enum ModelPart : int
    {
        ModelHead = 1,
        ModelBody,
        ModelHand,
        ModelFeet,
        ModelWeapon
    }

    public class CombineModelComponent : AbsComponent
    {
        private Dictionary<ModelPart, string> _modelDataDict;
        private Dictionary<ModelPart, GameObject> _modelObjectDict;
        private GameObject _model;
        private bool _initModel;

        protected override void InitializeEx()
        {
            base.InitializeEx();
            _modelDataDict = new Dictionary<ModelPart, string>
            {
                [ModelPart.ModelHead] = ModelMgr.singleton.HeadArray[0],
                [ModelPart.ModelBody] = ModelMgr.singleton.BodyArray[0],
                [ModelPart.ModelHand] = ModelMgr.singleton.HandArray[0],
                [ModelPart.ModelFeet] = ModelMgr.singleton.FeetArray[0],
                [ModelPart.ModelWeapon] = ModelMgr.singleton.WeaponArray[0]
            };
            _modelObjectDict = new Dictionary<ModelPart, GameObject>();
        }

        private void OnLoadFinish()
        {
            //初始化;
            _initModel = true;
            CombineModel();
        }

        protected override void UnInitializeEx()
        {
            base.UnInitializeEx();
            _modelDataDict.Clear();
            _model = null;
            _initModel = false;
        }

        public void SetHead(string head)
        {
            var curHead = _modelDataDict[ModelPart.ModelHead];
            if (curHead != head)
            {
                _modelDataDict[ModelPart.ModelHead] = head;
                LoadModelPart(ModelPart.ModelHead, head, AddModelPart);
            }
        }

        public void SetBody(string body)
        {
            var curBody = _modelDataDict[ModelPart.ModelBody];
            if (curBody != body)
            {
                _modelDataDict[ModelPart.ModelBody] = body;
                LoadModelPart(ModelPart.ModelBody, body, AddModelPart);
            }
        }

        public void SetHand(string hand)
        {
            var curHand = _modelDataDict[ModelPart.ModelHand];
            if (curHand != hand)
            {
                _modelDataDict[ModelPart.ModelHand] = hand;
                LoadModelPart(ModelPart.ModelHand, hand, AddModelPart);
            }
        }

        public void SetFeet(string feet)
        {
            var curFeet = _modelDataDict[ModelPart.ModelFeet];
            if (curFeet != feet)
            {
                _modelDataDict[ModelPart.ModelFeet] = feet;
                LoadModelPart(ModelPart.ModelFeet, feet, AddModelPart);
            }
        }

        public void SetWeapon(string weapon)
        {
            var curWeapon = _modelDataDict[ModelPart.ModelWeapon];
            if (curWeapon != weapon)
            {
                _modelDataDict[ModelPart.ModelWeapon] = weapon;
                LoadModelPart(ModelPart.ModelWeapon, weapon, AddModelPart);
            }
        }

        private void LoadModelPart(ModelPart part, string resName, Action<ModelPart, GameObject> callback)
        {

        }

        private void AddModelPart(ModelPart part, GameObject go)
        {
            CombineModel(part);
        }

        private void CombineModel()
        {
            if (!_initModel)
                return;

        }

        private void CombineModel(ModelPart part)
        {
            if (!_initModel)
                return;
            if (part == ModelPart.ModelWeapon)
            {

            }
            else
            {

            }
        }
    }
}