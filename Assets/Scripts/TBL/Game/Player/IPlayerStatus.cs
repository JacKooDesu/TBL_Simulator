namespace TBL.Game
{
    // TODO: Add event hook for value changed
    public delegate void OnChanged<T>();
    
    /// <summary>
    /// 提供玩家狀態，由伺服端請求所有玩家Update。
    /// </summary>
    /// <typeparam name="T">狀態類型</typeparam>
    public interface IPlayerStatus<T> : IPlayerStatus
    {
        T Current();
        void Update(IPlayerStatus value);
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
}