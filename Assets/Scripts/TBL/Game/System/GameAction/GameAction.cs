using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace TBL.Game.Sys
{
    using Game.Networking;
    using TBL.Utils;

    public abstract class GameAction
    {
        public GameAction() { }
        public GameAction(Player player)
        {
            this.Player = player;
            CompleteCallback = new();
            DiscardCallback = new();
        }

        #region  SERVER
        public object Result { get; protected set; }
        public Player Player { get; private set; }
        protected abstract ActionType ActionType { get; }
        public abstract Func<ActionRequestPacket> RequestCreate { get; }
        public abstract void SetResponse(ActionResponsePacket packet);
        public UnityEvent<object> CompleteCallback { get; }
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
                ActionType.SelectAction => new GameAction_SelectAction(request),
                _ => null
            };

        public GameAction WhenDiscard(Func<Action> action)
        {
            DiscardCallback.AutoRemoveListener(action());
            return this;
        }

        public GameAction AndThen<T>(Action<T> nextAction)
        {
            CompleteCallback.AutoRemoveListener(_ => nextAction((T)_));
            return this;
        }

        public GameAction AddToFlow()
        {
            Manager.Instance.AddGameAction(this);
            return this;
        }
    }

    public enum ActionType
    {
        SelectPlayer,
        SelectCard,
        SelectAction,
    }
}
