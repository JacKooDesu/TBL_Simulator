using System;

namespace TBL.Game
{
    /// <summary>
    /// 提供玩家狀態，由伺服端請求所有玩家Update。
    /// </summary>
    /// <typeparam name="T">狀態類型</typeparam>
    public interface IPlayerStatus<T> : IPlayerStatus
    {
        T Current();
        void Update(T value);
        void Update<S>(S status) where S : IPlayerStatus<T>;
        event Action<T> OnChanged;
    }

    public interface IPlayerStatus
    {
        PlayerStatusType Type();
        // void Update(IPlayerStatus value);
    }

    public enum PlayerStatusType
    {
        Profile,
        Team,
        Card,
        Hero,
        Skill,
        Reciver
    }

    public enum TeamEnum
    {
        None = 0,
        Blue,
        Red,
        Green
    }

    [Flags]
    public enum ReceiveEnum
    {
        Locked = 1 << 1,
        Skipped = 1 << 2
    }
}