using System.Collections.Generic;
namespace TBL.UI
{
    interface IWindowable
    {
        void Init(List<Option> options, int defaultIndex);
        void Cancel();
        void Close();

    }
}
