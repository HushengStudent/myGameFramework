/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/06/24 00:39:54
** desc:  行为树事件;
*********************************************************************************/

namespace Framework.BehaviorTreeModule
{
    public partial class BehaviorTree
    {
        public delegate void OnBTStartEventHandler();
        public delegate void OnBTSuccesstEventHandler();
        public delegate void OnBTFailureEventHandler();
        public delegate void OnBTResetEventHandler();

        public delegate void OnBTNodeStartEventHandler();
        public delegate void OnBTNodeSuccesstEventHandler();
        public delegate void OnBTNodeFailureEventHandler();
        public delegate void OnBTNodeResetEventHandler();
    }
}