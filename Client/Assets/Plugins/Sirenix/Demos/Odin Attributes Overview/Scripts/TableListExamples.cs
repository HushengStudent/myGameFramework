namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;
    using Sirenix.OdinInspector;
    using System.Collections.Generic;
    using System;

    public class TableListExamples : SerializedMonoBehaviour
    {
        [TableList]
        public List<A> TableList = new List<A>();

        [Space(10)]
        [TableList]
        public List<D> CombineColumns = new List<D>();
    }

    public class A
    {
        [TableColumnWidth(50)]
        public bool Toggle;

        public string Message;

        [TableColumnWidth(160)]
        [HorizontalGroup("Actions")]
        public void Test1() { }

        [HorizontalGroup("Actions")]
        public void Test2() { }
    }

    public class B : A
    {
        [HorizontalGroup("Actions")]
        public void Test3() { }
    }

    public class C : A
    {
        [HorizontalGroup("Actions")]
        public void Test4() { }
    }

    [Serializable]
    public class D
    {
        [LabelWidth(30)]
        [TableColumnWidth(130)]
        [VerticalGroup("Combined")]
        public string A;

        [LabelWidth(30)]
        [VerticalGroup("Combined")]
        public string B;

        [Multiline(2), Space(3)]
        public string fields;
    }
}