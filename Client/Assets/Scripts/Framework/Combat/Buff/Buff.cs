/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/06 22:40:14
** desc:  Buff;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class Buff
    {
        private BuffEventHandler _onInitHandler = null;
        private BuffEventHandler _onFinishHandler = null;

        public ulong LeftTime;
        public int BuffId { get; private set; }

        public void InitBuff(int buffId, ulong leftTime, BuffEventHandler onInit, BuffEventHandler onFinish)
        {
            BuffId = buffId;
            LeftTime = leftTime;
            _onInitHandler = onInit;
            _onFinishHandler = onFinish;
        }

        public void CallOnInitHandler(Buff buff)
        {
            if (_onInitHandler != null)
            {
                _onInitHandler(buff);
            }
        }

        public void CallOnFinishHandler(Buff buff)
        {
            if (_onFinishHandler != null)
            {
                _onFinishHandler(buff);
            }
        }
    }
}