/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/08 23:42:22
** desc:  Editor Coroutine Runner
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Framework.EditorModule
{
    public static class EditorCoroutineRunner
    {
        private class EditorCoroutine : IEnumerator
        {
            private Stack<IEnumerator> _executionStack;

            public EditorCoroutine(IEnumerator iterator)
            {
                _executionStack = new Stack<IEnumerator>();
                _executionStack.Push(iterator);
            }

            public bool MoveNext()
            {
                var i = _executionStack.Peek();
                if (i.MoveNext())
                {
                    object result = i.Current;
                    if (result != null && result is IEnumerator)
                    {
                        _executionStack.Push((IEnumerator)result);
                    }
                    return true;
                }
                else
                {
                    if (_executionStack.Count > 1)
                    {
                        _executionStack.Pop();
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
                return _executionStack.Contains(iterator);
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
            foreach (var editorCoroutine in _editorCoroutineList)
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
                foreach (var iterator in _buffer)
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