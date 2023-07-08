using Cysharp.Threading.Tasks;

namespace TBL.Game
{
    using Sys;
    public interface ICardFunction
    {
        // void Step();
        void ExecuteAction(Player user, Manager manager);
        // delegate UniTask ExecuteAction(Player user, Player target, Manager manager);
        // ExecuteAction Execute { get; }
    }
}