/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/13 00:50:27
** desc:  自定义异常;
*********************************************************************************/

using System;

namespace Framework
{
    [Serializable]
    public class GameException : Exception
    {
        private string _name;
        private string _message;
        private Exception _innerException;

        public GameException() : base() { }

        public GameException(string name, string message) : base(message)
        {
            _name = name;
            _message = message;
        }

        public GameException(string name, string message, Exception innerException) : base(message, innerException)
        {

            _name = name;
            _message = message;
            _innerException = innerException;
        }

        public void PrintException()
        {
            LogHelper.PrintError($"[{_name}]{_message}");
            if (_innerException != null)
            {
                LogHelper.PrintError($"[{_name}]{_innerException.Message.ToString()}");
            }
        }
    }
}