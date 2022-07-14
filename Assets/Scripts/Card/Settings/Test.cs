using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TBL.GameAction;

namespace TBL.Card
{
    public class Test : CardSetting
    {
        public override string Tip
        {
            get
            {
                var setting = actionSettings[GetUniqueID() % 6];
                var manager = NetworkRoomManager.singleton as NetworkRoomManager;
                string finalText = "";
                switch (setting.specifyTeam)
                {
                    case Settings.TeamSetting.TeamEnum.Blue:
                        finalText += $"{manager.teamSetting.BlueTeam.GetRichName()} - " + (setting.type == ActionType.Draw ? "抽一張牌。" : "說：我是一名好人。") + '\n';
                        finalText += $"{manager.teamSetting.RedTeam.GetRichName()} / {manager.teamSetting.GreenTeam.GetRichName()} - " + (setting.type == ActionType.Draw ? "說：我是一名好人。" : "抽一張牌。");
                        break;

                    case Settings.TeamSetting.TeamEnum.Red:
                        finalText += $"{manager.teamSetting.RedTeam.GetRichName()} - " + (setting.type == ActionType.Draw ? "抽一張牌。" : "說：我是一名好人。") + '\n';
                        finalText += $"{manager.teamSetting.GreenTeam.GetRichName()} / {manager.teamSetting.BlueTeam.GetRichName()} - " + (setting.type == ActionType.Draw ? "說：我是一名好人。" : "抽一張牌。");
                        break;

                    case Settings.TeamSetting.TeamEnum.Green:
                        finalText += $"{manager.teamSetting.GreenTeam.GetRichName()} - " + (setting.type == ActionType.Draw ? "抽一張牌。" : "說：我是一名好人。") + '\n';
                        finalText += $"{manager.teamSetting.RedTeam.GetRichName()} / {manager.teamSetting.BlueTeam.GetRichName()} - " + (setting.type == ActionType.Draw ? "說：我是一名好人。" : "抽一張牌。");
                        break;
                }

                return finalText;
            }
        }
        
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
            var target = manager.players[ca.target];

            if (target.Team.team == setting.specifyTeam)
            {
                switch (setting.type)
                {
                    case ActionType.Draw:
                        print("抽牌");
                        target.TargetGetTest(true);
                        break;

                    case ActionType.IMGood:
                        print("我是一個好人");
                        target.TargetGetTest(false);
                        break;
                }
            }
            else
            {
                switch (setting.type)
                {
                    case ActionType.Draw:
                        print("我是一個好人");
                        target.TargetGetTest(false);
                        break;

                    case ActionType.IMGood:
                        print("抽牌");
                        target.TargetGetTest(true);
                        break;
                }
            }
        }
    }
}

