/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/07 22:19:02
** desc:  UI事件系统测试;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.EventModule
{
    public class UIEventTest : MonoBehaviour
    {
        void Start()
        {
            UIEventMgr<EventTestEnum>.AddEvent(EventTestEnum.TestEnum, OnEvent1);
            UIEventMgr<EventTestEnum>.AddEvent<int>(EventTestEnum.TestEnum, OnEvent2);
        }

        void OnGUI()
        {
            if (GUI.Button(new Rect(10, 10, 200, 50), "Event1 Fire"))
            {
                UIEventMgr<EventTestEnum>.FireEvent(EventTestEnum.TestEnum);
            }
            if (GUI.Button(new Rect(10, 70, 200, 50), "Event1 Remove"))
            {
                UIEventMgr<EventTestEnum>.RemoveEvent(EventTestEnum.TestEnum, OnEvent1);
            }

            if (GUI.Button(new Rect(10, 130, 200, 50), "Event2 Fire"))
            {
                UIEventMgr<EventTestEnum>.FireEvent<int>(EventTestEnum.TestEnum, 12345);
            }
            if (GUI.Button(new Rect(10, 200, 200, 50), "Event2 Remove"))
            {
                UIEventMgr<EventTestEnum>.RemoveEvent<int>(EventTestEnum.TestEnum, OnEvent2);
            }
        }

        private void OnEvent1()
        {
            Debug.Log("=====>OnEvent1!");
        }

        private void OnEvent2(int value)
        {
            Debug.Log("=====>OnEvent2 " + value.ToString() + " !");
        }
    }

    public enum EventTestEnum
    {
        TestEnum,
    }
}
