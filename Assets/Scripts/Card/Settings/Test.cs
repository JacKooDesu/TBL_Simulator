using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TBL.Action;

namespace TBL.Card
{
    public class Test : CardSetting
    {
        public override void OnUse(NetworkPlayer user, int originID)
        {
            base.OnUse(user, originID);

            var manager = NetworkRoomManager.singleton as NetworkRoomManager;
            var netCanvas = FindObjectOfType<NetCanvas.GameScene>();

            netCanvas.BindSelectPlayer(manager.GetOtherPlayers(), (i) =>
            {
                user.CmdTestCardAction(new CardAction(user.playerIndex, i, ID, originID, GetUniqueID()));
            });
        }

        // only run on server
        // 目前先寫死
        enum ActionType
        {
            Draw,
            IMGood
        }
        struct Setting
        {
            public TBL.Settings.TeamSetting.TeamEnum specifyTeam;
            public ActionType type;

            public Setting(TBL.Settings.TeamSetting.TeamEnum team, ActionType type)
            {
                specifyTeam = team;
                this.type = type;
            }
        }

        static Setting[] actionSettings ={
            new Setting(TBL.Settings.TeamSetting.TeamEnum.Blue,ActionType.Draw),
            new Setting(TBL.Settings.TeamSetting.TeamEnum.Blue,ActionType.IMGood),
            new Setting(TBL.Settings.TeamSetting.TeamEnum.Red,ActionType.Draw),
            new Setting(TBL.Settings.TeamSetting.TeamEnum.Red,ActionType.IMGood),
            new Setting(TBL.Settings.TeamSetting.TeamEnum.Green,ActionType.Draw),
            new Setting(TBL.Settings.TeamSetting.TeamEnum.Green,ActionType.IMGood),
        };

        public override void OnEffect(NetworkRoomManager manager, CardAction ca)
        {
            base.OnEffect(manager, ca);

            var setting = actionSettings[ca.suffix % 6];

            if (manager.players[ca.target].team.team == setting.specifyTeam)
            {
                switch (setting.type)
                {
                    case ActionType.Draw:
                        print("抽牌");
                        break;

                    case ActionType.IMGood:
                        print("我是一個好人");
                        break;
                }
            }
            else
            {
                switch (setting.type)
                {
                    case ActionType.Draw:
                        print("我是一個好人");
                        break;

                    case ActionType.IMGood:
                        print("抽牌");
                        break;
                }
            }
        }
    }
}

