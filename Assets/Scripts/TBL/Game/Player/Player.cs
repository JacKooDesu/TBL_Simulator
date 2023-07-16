﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace TBL.Game
{
    using Game.Sys;
    using Game.Networking;
    using TBL.Utils;

    [System.Serializable]
    public class Player
    {
        [SerializeField] IPlayerStandalone playerStandalone;
        public IPlayerStandalone PlayerStandalone => playerStandalone;
        // TODO: player shoud use property drawer to optimized!
        [SerializeField] ProfileStatus profileStatus = new ProfileStatus();
        public ProfileStatus ProfileStatus => profileStatus;

        [SerializeField] TeamStatus teamStatus = new TeamStatus(0);
        public TeamStatus TeamStatus => teamStatus;

        [SerializeField] CardStatus cardStatus = new CardStatus();
        public CardStatus CardStatus => cardStatus;

        [SerializeField] HeroStatus heroStatus = new HeroStatus();
        public HeroStatus HeroStatus => heroStatus;

        [SerializeField] SkillStatus skillStatus = new SkillStatus();
        public SkillStatus SkillStatus => skillStatus;

        [SerializeField] PhaseQuestStatus phaseQuestStatus = new();
        public PhaseQuestStatus PhaseQuestStatus => phaseQuestStatus;

        [SerializeField] ReceiverStatus receiverStatus = new ReceiverStatus(0);
        public ReceiverStatus ReceiverStatus => receiverStatus;

        List<IPlayerStatus> StatusList => new List<IPlayerStatus>{
            profileStatus,
            teamStatus,
            cardStatus,
            heroStatus,
            skillStatus,
            phaseQuestStatus,
            receiverStatus
        };

        public Player(IPlayerStandalone standalone, bool isServer = false)
        {
            playerStandalone = standalone;
            if (isServer)
                BindServerEvent();
        }

        public void BindServerEvent()
        {
            var handler = playerStandalone.PacketHandler;
            if (!Manager.TryGet()(out var manager))
                throw new Exception("Manager not found!");

            handler.PlayerReadyPacketEvent.AutoRemoveListener(_ => playerStandalone.SetReady());
            handler.UseCardPacketEvent.AddListener(_ => manager.UseCard(this, _.cardId));

            teamStatus.OnChanged.AddListener(_ => playerStandalone.Send(SendType.Rpc, new PlayerStatusPacket(teamStatus)));
            heroStatus.OnChanged.AddListener(_ => playerStandalone.Send(SendType.Rpc, new PlayerStatusPacket(heroStatus)));
            cardStatus.OnChanged.AddListener(_ => playerStandalone.Send(SendType.Rpc, new PlayerStatusPacket(cardStatus)));
            profileStatus.OnChanged.AddListener(_ => playerStandalone.Send(SendType.Rpc, new PlayerStatusPacket(profileStatus)));
            skillStatus.OnChanged.AddListener(_ => playerStandalone.Send(SendType.Rpc, new PlayerStatusPacket(skillStatus)));
            phaseQuestStatus.OnChanged.AddListener(_ => playerStandalone.Send(SendType.Rpc, new PlayerStatusPacket(phaseQuestStatus)));
            receiverStatus.OnChanged.AddListener(_ => playerStandalone.Send(SendType.Rpc, new PlayerStatusPacket(receiverStatus)));
        }
        public void BindClientEvent()
        {

        }

        // 更新狀態
        public void UpdateStatus<S>(S status) where S : IPlayerStatus
            => UpdateStatus(status.Type(), status);
        public void UpdateStatuses<S>(params S[] statuses) where S : IPlayerStatus =>
            statuses.ToList().ForEach(UpdateStatus);

        public void UpdateStatus<T>(PlayerStatusType type, T status)
        where T : IPlayerStatus
        {
            var target = StatusList.Find(s => s.Type() == type);
            if (target == null)
                Debug.LogError("Target status not found!");
            else
                target.Update(status);
        }

        public record UpdateStatusRecord(PlayerStatusType type, IPlayerStatus status);
        public void UpdateStatus(UpdateStatusRecord record) =>
            UpdateStatus(record.type, record.status);
        public void UpdateStatuses(params UpdateStatusRecord[] records) =>
            records.ToList().ForEach(UpdateStatus);

        public void UpdateStatus(PlayerStatusPacket.StatusData data)
        {
            foreach (var s in data.ToArray())
                if (s != null) UpdateStatus(s);
        }

        public void Handler(string data)
        {

        }

        #region  DEBUG
        public void ForceUpdateAll() => UpdateStatuses(StatusList.ToArray());
        #endregion
    }
}
