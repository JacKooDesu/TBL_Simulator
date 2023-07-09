using System;
using System.Linq;

namespace TBL.Game.Sys
{
    using Networking;
    using Newtonsoft.Json;
    using UI.Main;
    using Property = CardEnum.Property;
    public class GameAction_SelectCard : GameAction
    {
        protected override ActionType ActionType => ActionType.SelectCard;
        public Property[] Cards { get; private set; }
        #region SERVER
        public GameAction_SelectCard(Player player, Property[] cards) : base(player)
        {
            Cards = cards;
        }
        public override Func<ActionRequestPacket> PacketCreate => _PacketCreate;
        ActionRequestPacket _PacketCreate()
        {
            string data = JsonConvert.SerializeObject(Cards);
            return new(ActionType, data);
        }

        #endregion

        #region CLIENT
        public GameAction_SelectCard(ActionRequestPacket request)
        {
            Cards = JsonConvert.DeserializeObject<Property[]>(request.Data);
        }
        public override void Execute()
        {
            // MainUIManager.Singleton
            //              .Create
        }
        void Response(Property x)
        {
            IPlayerStandalone.Me.Send(
            SendType.Cmd,
                new ActionResponsePacket()
                    .WithData($"{x}")
                    .WithType(ActionType));
        }
        #endregion
    }
}