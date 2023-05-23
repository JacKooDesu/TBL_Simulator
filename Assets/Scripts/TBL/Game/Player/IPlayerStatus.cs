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
        ProfileStatus,
        TeamStatus,
        CardStatus,
        Hero,
        Skill
    }

    public enum TeamEnum
    {
        Blue,
        Red,
        Green
    }
}