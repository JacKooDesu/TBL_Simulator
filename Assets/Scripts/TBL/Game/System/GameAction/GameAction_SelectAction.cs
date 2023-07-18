using System;
using System.Linq;
using Newtonsoft.Json;

namespace TBL.Game.Sys
{
    using Networking;
    using TBL.Utils;
    using UI.Main;
    public class GameAction_SelectAction : GameAction
    {
        protected override ActionType ActionType => ActionType.SelectAction;
        public record OptionBase(string name);
        public record Option(string name, Action action) : OptionBase(name);
        public record ActionData(OptionBase[] options);
        public ActionData Data { get; private set; }
        #region SERVER
        public GameAction_SelectAction(Player player, ActionData data) : base(player)
        {
            Data = data;
        }

        public override Func<ActionRequestPacket> RequestCreate => _PacketCreate;

        ActionRequestPacket _PacketCreate()
        {
            OptionBase[] converted = Data.options.Select(x => new OptionBase(x.name)).ToArray();
            string data = JsonConvert.SerializeObject(
                Data with { options = converted });
            return new(ActionType, data);
        }

        public override void SetResponse(ActionResponsePacket packet)
        {
            if (Int32.TryParse(packet.Data, out var value))
                Result = value;

            var option = Data.options[value];
            if (option is not Option)
                return;

            (option as Option).action();
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
            menu.OnConfirm.AutoRemoveListener(Response);
        }
        void Response(int x)
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