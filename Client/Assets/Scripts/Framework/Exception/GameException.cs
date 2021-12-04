/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/13 00:50:27
** desc:  自定义异常;
*********************************************************************************/

using Framework.ObjectPoolModule;
using System;
using System.Diagnostics;
using System.Text;

namespace Framework
{
    [Serializable]
    public class GameException : Exception, IPool
    {
        private string _name;
        private StringBuilder _messageBuilder = new StringBuilder();
        private Exception _innerException;

        public GameException() : base() { }

        public GameException(string name) : base()
        {
            _name = name;
        }

        public void PrintException(StackTrace stackTrace = null)
        {
            if (stackTrace != null)
            {
                LogHelper.PrintError($"[{_name}]{_messageBuilder.ToString()}\r\n{stackTrace.ToString()}");
            }
            else
            {
                LogHelper.PrintError($"[{_name}]{_messageBuilder.ToString()}");
            }
        }

        public void AppendMsg(string msg)
        {
            _messageBuilder.AppendLine(msg);
        }

        public bool IsActive
        {
            get
            {
                return _messageBuilder.Length > 0;
            }
        }

        void IPool.OnGet(params object[] args)
        {
        }

        void IPool.OnRelease()
        {
            _name = string.Empty;
            _messageBuilder.Clear();
        }
    }
}