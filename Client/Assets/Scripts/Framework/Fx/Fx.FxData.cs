/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2021/11/14 21:08:20
** desc:  ÌØÐ§;
*********************************************************************************/

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

            public void OnGet(params object[] args)
            {
                Reset();
            }

            public void OnRelease()
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