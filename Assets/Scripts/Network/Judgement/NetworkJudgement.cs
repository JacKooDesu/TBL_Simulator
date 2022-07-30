using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

namespace TBL
{
    public partial class NetworkJudgement : NetworkBehaviour
    {
        public Settings.RoundSetting roundSetting;
        public Settings.HeroList heroList;       // 英雄列表
        public SyncList<int> hasUsedHeros = new SyncList<int>();

        [SerializeField, SyncVar(hook = nameof(OnTimeChange))] int timer;
        void OnTimeChange(int oldTime, int newTime)
        {
            netCanvas.timeTextUI.text = newTime.ToString();
        }

        [SerializeField, SyncVar(hook = nameof(OnCurrentPlayerChange))] public int currentRoundPlayerIndex;
        void OnCurrentPlayerChange(int oldPlayer, int newPlayer)
        {

        }

        [SyncVar] public int playerReadyCount = 0;

        TBL.NetCanvas.GameScene netCanvas;
        NetworkRoomManager manager;

        // 階段
        public enum Phase
        {
            Draw,
            ChooseToSend,
            Sending,
            Reacting,
            HeroSkillReacting,
            HeroSkillConfirm,
            CardEventing,
            Result
        }

        [Header("輪設定")]
        [SyncVar(hook = nameof(OnCurrentPhaseChange))] public Phase currentPhase;
        [SyncVar, HideInInspector] public Phase lastPhase;
        public void ChangePhase(Phase phase)
        {
            lastPhase = currentPhase;
            currentPhase = phase;
        }
        public void OnCurrentPhaseChange(Phase oldPhase, Phase newPhase)
        {
            if (newPhase == Phase.Reacting || newPhase == Phase.Sending)
            {
                netCanvas.ResetUI();
            }
            print($"System - 階段更新: {newPhase}");
            manager.LocalPlayer.CmdSetPhase(newPhase);
            netCanvas.RemoveAllTempMenu();
        }

        [SyncVar] public bool currentRoundHasSendCard;
        [SyncVar] public int currentRoundSendingCardId;
        [SyncVar] public int currentSendingPlayer;
        [SyncVar] public bool currentSendReverse;
        [SyncVar] public int playerIntercept = -1;

        public void ResetRoundTrigger(int hasSendCard = -1, int sendReverse = -1)
        {
            if (hasSendCard != -1)
                currentRoundHasSendCard = hasSendCard == 1;

            if (sendReverse != -1)
                currentSendReverse = sendReverse == 1;
        }

        private void Start()
        {
            netCanvas = FindObjectOfType<TBL.NetCanvas.GameScene>();
            manager = ((NetworkRoomManager)NetworkManager.singleton);

            if (isServer)
                StartCoroutine(WaitAllPlayerInit());
        }
    }

}
