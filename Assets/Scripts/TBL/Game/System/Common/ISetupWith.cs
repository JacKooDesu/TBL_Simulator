namespace TBL.Game.Sys
{
    public interface ISetupWith<T> : ISetupWith
    {
        void Setup(T res);
    }

    public interface ISetupWith { }
}