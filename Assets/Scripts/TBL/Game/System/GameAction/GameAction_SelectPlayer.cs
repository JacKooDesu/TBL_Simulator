using System;
using System.Linq;
using UnityEngine.Events;
using Newtonsoft.Json;

namespace TBL.Game.Sys
{
    using Networking;
    using UI.Main;

    public class GameAction_SelectPlayer : GameAction
    {
        protected override ActionType ActionType => ActionType.SelectPlayer;
        public int[] Targets { get; private set; }

        #region SERVER
        public GameAction_SelectPlayer(Player player, int[] targets) : base(player)
        {
            Targets = targets;
        }
        public override Func<ActionRequestPacket> PacketCreate => _PacketCreate;
        ActionRequestPacket _PacketCreate()
        {
            string data = JsonConvert.SerializeObject(Targets);
            return new(ActionType, data);
        }
        #endregion

        #region CLIENT
        public GameAction_SelectPlayer(ActionRequestPacket request)
        {
            Targets = JsonConvert.DeserializeObject<int[]>(request.Data);
        }
        public override void Execute()
        {
            MainUIManager.Singleton
                         .PlayerListWindow
                         .EnterPlayerSelect(
                            x => Targets.Contains(x.player.ProfileStatus.Id),
                            Response);
        }
        void Response(int x)
        {
            IPlayerStandalone.Me.Send(
                SendType.Cmd,
                new ActionResponsePacket()
                    .WithData($"{x}")
                    .WithType(ActionType.SelectPlayer));
        }
        #endregion
    }
}
