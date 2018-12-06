/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/08/20 01:44:34
** desc:  Object扩展;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class ObjectEx : IPool
    {
        protected ObjectEx()
        {
            ID = IdGenerateHelper.GenerateId();
            Enable = false;
        }

        public long ID { get; private set; }
        public bool Enable { get; set; }

        /// <summary>
        /// 对象池Get;
        /// </summary>
        /// <param name="args"></param>
        public virtual void OnGet(params System.Object[] args) { }

        /// <summary>
        /// 对象池Release;
        /// </summary>
        public virtual void OnRelease() { }
    }
}