/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2021/11/14 20:45:53
** desc:  ÌØÐ§;
*********************************************************************************/

using Framework.ObjectPoolModule;

namespace Framework.FxModule
{
    public partial class Fx : IPool
    {
        public int ID { get; private set; }
        public FxData Data { get; private set; }

        public void OnInitialize()
        {

        }

        public void OnUninitialize()
        {

        }

        public void Update()
        {

        }

        void IPool.OnGet(params object[] args)
        {
            Data = args[0] as FxData;
            ID = FxMgr.singleton.FxID;
        }

        void IPool.OnRelease()
        {
            ID = 0;
            PoolMgr.singleton.ReleaseCsharpObject(Data);
            Data = null;
        }
    }
}