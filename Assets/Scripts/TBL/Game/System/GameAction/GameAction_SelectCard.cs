using System;
using System.Linq;

namespace TBL.Game.Sys
{
    using Networking;
    using Newtonsoft.Json;
    using TBL.Utils;
    using UI.Main;
    using Property = CardEnum.Property;
    public class GameAction_SelectCard : GameAction
    {
        protected override ActionType ActionType => ActionType.SelectCard;
        public record ActionData(Property[] Cards, int Count = 1);
        public ActionData Data { get; private set; }
        public Property[] Result { get; private set; }
        #region SERVER
        public GameAction_SelectCard(Player player, ActionData data) : base(player)
        {
            Data = data;
            Callback.AddListener(SetResponse);
        }
        public override Func<ActionRequestPacket> RequestCreate => _RequestCreate;
        ActionRequestPacket _RequestCreate()
        {
            string data = JsonConvert.SerializeObject(Data);
            return new(ActionType, data);
        }

        public override void SetResponse(ActionResponsePacket packet)
        {
            Result = JsonConvert.DeserializeObject<Property[]>(packet.Data);
        }
        #endregion

        #region CLIENT
        public GameAction_SelectCard(ActionRequestPacket request)
        {
            Data = JsonConvert.DeserializeObject<ActionData>(request.Data);
        }
        public override void Execute()
        {
            var menu =
                MainUIManager.Singleton
                             .TempMenuManager
                             .CardTempMenu
                             .Create(new(Data.Cards, Data.Count));
            menu.OnConfirm.AutoRemoveListener(Response);
        }
        void Response(Property[] x)
        {
            IPlayerStandalone.Me.Send(
                SendType.Cmd,
                new ActionResponsePacket()
                    .WithData(JsonConvert.SerializeObject(x))
                    .WithType(ActionType));
        }
        #endregion
    }
}