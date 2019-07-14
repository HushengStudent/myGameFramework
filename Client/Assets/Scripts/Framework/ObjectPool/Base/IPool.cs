/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/20 00:13:50
** desc:  对象池组件接口;
*********************************************************************************/

using System;

namespace Framework
{
    public interface IPool //: IDisposable
    {
        void OnGet(params Object[] args);
        void OnRelease();
    }
}
