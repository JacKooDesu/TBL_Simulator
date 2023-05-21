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
    where TResult : IResource
    {
        TResult Init(TInit res);
    }

    public interface IResource<TInit, UInit, TResult> : IResource
    where TResult : IResource
    {
        TResult Init(TInit res1, UInit res2);
    }

    public interface IResource<TInit, UInit, VInit, TResult> : IResource
    where TResult : IResource
    {
        TResult Init(TInit res1, UInit res2, VInit res3);
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