using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TBL.Card;
using TBL.GameAction;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace TBL
{
    using Util;
    using Hero;
    public partial class NetworkPlayer : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnHeroIndexChange))] public int heroIndex = -1;
        void OnHeroIndexChange(int oldVar, int newVar)
        {
            hero = Instantiate(manager.Judgement.heroList.heros[newVar]);

            hero.Init(this);

            if (isLocalPlayer)
                netCanvas.InitPlayerStatus();

            if (isServer)
            {
                netHeroSkillCanActivate.Clear();
                for (int i = 0; i < hero.skills.Length; ++i)
                    netHeroSkillCanActivate.Add(false);
            }

            netHeroSkillCanActivate.Callback += OnUpdateHeroSkillCanActivate;

            netCanvas.playerUIs[manager.players.IndexOf(this)].UpdateHero();
        }
        public HeroBase hero;
        public SyncList<bool> netHeroSkillCanActivate = new SyncList<bool>();
        public void OnUpdateHeroSkillCanActivate(SyncList<bool>.Operation op, int index, bool oldItem, bool newItem)
        {
            if (isLocalPlayer)
            {
                List<Option> options = new List<Option>();
                List<UnityEngine.Events.UnityAction> actions = new List<UnityEngine.Events.UnityAction>();
                for (int i = 0; i < netHeroSkillCanActivate.Count; ++i)
                {
                    if (!netHeroSkillCanActivate[i]) continue;

                    var skill = hero.skills[i];
                    int x = i;
                    if (!skill.autoActivate)
                    {
                        var option = new Option
                        {
                            str = skill.name,
                            onSelect = async () =>
                            {
                                var canceltoken = new CancellationTokenSource();
                                var task = skill.localAction(canceltoken);

                                var tempPhase = judgement.currentPhase;

                                await TaskExtend.WaitUntil(
                                    () => task.IsCompleted,
                                    () => judgement.currentPhase != tempPhase);
                                if (judgement.currentPhase != tempPhase)
                                {
                                    canceltoken.Cancel();
                                    return;
                                }
                                print("Client Skill Action Activate!");

                                CmdUseSkill(task.Result);
                            },
                            type = OptionType.SKILL
                        };

                        options.Add(option);
                    }
                }

                if (options.Count == 0)
                {
                    netCanvas.heroSkillData.ClearEvent(UnityEngine.EventSystems.EventTriggerType.PointerClick);
                    netCanvas.heroSkillData.animator.SetTrigger("Return");
                    return;
                }

                netCanvas.heroSkillData.animator.SetTrigger("Blink");
                netCanvas.heroSkillData.BindEvent(() => netCanvas.InitMenu(options));
            }

        }

        [Command]
        public void CmdChangeHeroState(bool hiding)
        {
            hero.isHiding = hiding;
            RpcChangeHeroState(hiding);
        }
        [ClientRpc]
        public void RpcChangeHeroState(bool hiding)
        {
            hero.isHiding = hiding;
            RpcUpdateHeroUI();
        }

        [Command]
        public void CmdSetSkillCanActivate(int index, bool b)
        {
            netHeroSkillCanActivate[index] = b;
        }

        [Command]
        public void CmdSetSkillLimited(int index, bool b)
        {
            hero.skills[index].limited = b;
        }
    }
}

