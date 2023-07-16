using System;
using System.Linq;
using Newtonsoft.Json;

namespace TBL.Game.Sys
{
    using Networking;
    using UI.Main;
    public class GameAction_SelectAction : GameAction
    {
        protected override ActionType ActionType => ActionType.SelectOption;
        public record Option(string name, Action action);
        public record ActionData(Option[] options);
        public ActionData Data { get; private set; }
        #region SERVER
        public GameAction_SelectAction(Player player, ActionData data) : base(player)
        {
            Data = data;
        }

        public override Func<ActionRequestPacket> RequestCreate => _PacketCreate;

        ActionRequestPacket _PacketCreate()
        {
            string data = JsonConvert.SerializeObject(Data);
            return new(ActionType, data);
        }

        public override void SetResponse(ActionResponsePacket packet)
        {
            if (Int32.TryParse(packet.Data, out var value))
                Result = value;
        }
        #endregion

        #region CLIENT
        public GameAction_SelectAction(ActionRequestPacket request)
        {
            Data = JsonConvert.DeserializeObject<ActionData>(request.Data);
        }
        public override void Execute()
        {
            var menu =
                MainUIManager.Singleton
                             .TempMenuManager
                             .ActionTempMenu
                             .Create(new(Data.options));
        }
        void Response(object x)
        {
            IPlayerStandalone.Me.Send(
            SendType.Cmd,
            new ActionResponsePacket()
            .WithData($"{x}")
            .WithType(ActionType)
            );
        }
        #endregion
    }
}