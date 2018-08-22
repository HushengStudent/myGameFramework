/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/08/22 23:21:45
** desc:  List¿©’π;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class ListEx<T> : IPool
    {
        private List<T> _data = new List<T>();

        public List<T> Data { get { return _data; } set { _data = value; } }

        public void OnGet(params object[] args)
        {

        }

        public void OnRelease()
        {
            Data.Clear();
        }
    }
}