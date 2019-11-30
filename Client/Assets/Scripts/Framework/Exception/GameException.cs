/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/13 00:50:27
** desc:  自定义异常;
*********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    [Serializable]
    public class GameException : Exception
    {
        private string message;
        private Exception innerException;

        public GameException() : base() { }

        public GameException(string message) : base(message)
        {

            this.message = message;
        }

        public GameException(string message, Exception innerException) : base(message, innerException)
        {

            this.innerException = innerException;
            this.message = message;
        }

        public void PrintException()
        {
            LogHelper.PrintError($"[GameException]{message}");
            if (innerException != null)
            {
                LogHelper.PrintError($"[GameException]{innerException.Message.ToString()}");
            }
        }
    }
}