using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace TBL.Game.Sys
{
    using Game.Networking;
    public abstract class GameAction
    {
        public GameAction() { }
        public GameAction(Player player)
        {
            this.Player = player;
            Callback = new();
            DiscardCallback = new();
        }

        #region  SERVER
        public Player Player { get; private set; }
        protected abstract ActionType ActionType { get; }
        public abstract Func<ActionRequestPacket> RequestCreate { get; }
        public abstract void SetResponse(ActionResponsePacket packet);
        public UnityEvent<ActionResponsePacket> Callback { get; }
        public UnityEvent DiscardCallback { get; }
        #endregion

        #region  CLIENT
        public abstract void Execute();
        #endregion

        public int ID { get; private set; }
        static int ID_CURRENT = 0;

        public static GameAction Create(ActionRequestPacket request) =>
            request.ActionType switch
            {
                ActionType.SelectPlayer => new GameAction_SelectPlayer(request),
                ActionType.SelectCard => new GameAction_SelectCard(request),
                _ => null
            };
    }

    // TODO: 未完成
    public class GameActionContainer : GameAction
    {
        public GameActionContainer(ActionRequestPacket request) : base()
        {
        }

        Queue<GameAction> Actions { get; set; } = new();
        protected override ActionType ActionType => throw new NotImplementedException();

        public override Func<ActionRequestPacket> RequestCreate => throw new NotImplementedException();

        public GameAction Next() => Actions.Dequeue();

        public override void Execute()
        {
            throw new NotImplementedException();
        }

        public override void SetResponse(ActionResponsePacket packet)
        {
            throw new NotImplementedException();
        }
    }

    public enum ActionType
    {
        SelectPlayer,
        SelectCard,
        SelectOption,
    }
}
