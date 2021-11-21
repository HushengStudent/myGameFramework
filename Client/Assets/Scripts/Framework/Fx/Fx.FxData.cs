/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2021/11/14 21:08:20
** desc:  ÌØÐ§;
*********************************************************************************/

using Framework.ObjectPoolModule;
using UnityEngine;

namespace Framework
{
    public partial class Fx
    {
        public class FxData : IPool
        {
            public string FxPath;
            public float Duration = -1;

            public GameObject Target;
            public bool IsFollow;

            public Vector3 TargetPosition;

            void IPool.OnGet(params object[] args)
            {
                Reset();
            }

            void IPool.OnRelease()
            {
                Reset();
            }

            private void Reset()
            {
                FxPath = string.Empty;
                Duration = -1;

                Target = null;
                IsFollow = false;

                TargetPosition = Vector3.zero;
            }
        }
    }
}