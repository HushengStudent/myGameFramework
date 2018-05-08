/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/08 23:42:22
** desc:  Editor Coroutine Runner
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    public static class EditorCoroutineRunner
    {
        private class EditorCoroutine : IEnumerator
        {
            private Stack<IEnumerator> _executionStack;

            public EditorCoroutine(IEnumerator iterator)
            {
                this._executionStack = new Stack<IEnumerator>();
                this._executionStack.Push(iterator);
            }

            public bool MoveNext()
            {
                IEnumerator i = this._executionStack.Peek();
                if (i.MoveNext())
                {
                    object result = i.Current;
                    if (result != null && result is IEnumerator)
                    {
                        this._executionStack.Push((IEnumerator)result);
                    }
                    return true;
                }
                else
                {
                    if (this._executionStack.Count > 1)
                    {
                        this._executionStack.Pop();
                        return true;
                    }
                }
                return false;
            }

            public void Reset()
            {
                throw new System.NotSupportedException("This Operation Is Not Supported.");
            }

            public object Current
            {
                get { return _executionStack.Peek().Current; }
            }

            public bool Find(IEnumerator iterator)
            {
                return this._executionStack.Contains(iterator);
            }
        }

        private static List<EditorCoroutine> _editorCoroutineList;
        private static List<IEnumerator> _buffer;

        public static IEnumerator StartEditorCoroutine(IEnumerator iterator)
        {
            if (_editorCoroutineList == null)
            {
                _editorCoroutineList = new List<EditorCoroutine>();
            }
            if (_buffer == null)
            {
                _buffer = new List<IEnumerator>();
            }
            if (_editorCoroutineList.Count == 0)
            {
                EditorApplication.update += Update;
            }
            _buffer.Add(iterator);
            return iterator;
        }

        private static bool Find(IEnumerator iterator)
        {
            foreach (EditorCoroutine editorCoroutine in _editorCoroutineList)
            {
                if (editorCoroutine.Find(iterator))
                {
                    return true;
                }
            }
            return false;
        }

        private static void Update()
        {
            _editorCoroutineList.RemoveAll(coroutine => { return coroutine.MoveNext() == false; });
            if (_buffer.Count > 0)
            {
                foreach (IEnumerator iterator in _buffer)
                {
                    if (!Find(iterator))
                    {
                        _editorCoroutineList.Add(new EditorCoroutine(iterator));
                    }
                }
                _buffer.Clear();
            }
            if (_editorCoroutineList.Count == 0)
            {
                EditorApplication.update -= Update;
            }
        }
    }
}