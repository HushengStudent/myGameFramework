/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/08/22 23:22:02
** desc:  Dictionary¿©’π;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class DictEx<K, V> : IPool
    {
        private Dictionary<K, V> _data = new Dictionary<K, V>();

        public Dictionary<K, V> Data { get { return _data; } set { _data = value; } }

        public void OnGet(params object[] args)
        {
            
        }

        public void OnRelease()
        {
            Data.Clear();
        }
    }
}