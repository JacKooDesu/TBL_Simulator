namespace TBL.Game
{
    /// <summary>
    /// 提供玩家狀態，由伺服端請求所有玩家Update。
    /// </summary>
    /// <typeparam name="T">狀態類型</typeparam>
    public interface IPlayerStatus<T> : IPlayerStatus
    {
        PlayerStatusType Type();
        T Current();
        // void Update(IPlayerStatus value);
    }
    public interface IPlayerStatus
    {
        void Update(IPlayerStatus value);
    }

    public enum PlayerStatusType
    {
        ProfileStatus,
        CardStatus,
        Hero,
        Skill
    }
}