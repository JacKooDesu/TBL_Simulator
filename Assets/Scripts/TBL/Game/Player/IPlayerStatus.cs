namespace TBL.Game
{
    /// <summary>
    /// 提供玩家狀態，由伺服端請求所有玩家Update。
    /// </summary>
    /// <typeparam name="T">狀態類型</typeparam>
    public interface IPlayerStatus<T>
    {
        PlayerStatusType Type();
        T Current();
        void Update(T value);
    }

    public enum PlayerStatusType
    {
        Hero,
        Skill
    }
}