using Cysharp.Threading.Tasks;

namespace TBL.Game.Sys
{
    public interface IResource { }

    /// <summary>
    /// 可被 Manager 集中管理的物件。
    /// </summary>
    /// <typeparam name="TInit">初始化</typeparam>
    /// <typeparam  name="TResult">初始完成回傳</typeparam>
    public interface IResource<TInit, TResult> : IResource
    {
        TResult Init(TInit res);
    }

    /// <summary>
    /// 可被 Manager 集中管理的物件。
    /// </summary>
    /// <typeparam name="TInit">初始化</typeparam>
    public interface IResource<TInit> : IResource
    {
        void Init(TInit res);
    }
}