/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2017/12/25 00:30:02
** desc:  UI管理;
*********************************************************************************/

using UnityEngine;

namespace Framework.UIModule
{
    public class UIMgr : MonoSingleton<UIMgr>
    {
        public GameObject UIRoot { get; private set; }
    }
}
