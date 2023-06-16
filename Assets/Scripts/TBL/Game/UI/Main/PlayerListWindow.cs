using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;

namespace TBL.Game.UI.Main
{
    using UI;
    using Sys;
    using Utils;
    public class PlayerListWindow : Window, ISetupWith<IPlayerStandalone>
    {
        [SerializeField] PlayerListItem prefab;
        [SerializeField] Transform content;
        [Header("顏色設定")]
        [SerializeField] Color roundHostColor;
        [SerializeField] Color cardPassingColor;

        [Header("Visual Setting")]
        [SerializeField] float blinkTime = .8f;

        Dictionary<IPlayerStandalone, PlayerListItem> playerItemDict = new();

        public void Setup(IPlayerStandalone res)
        {
            var item = Instantiate(prefab, content);
            item.Init(res);
            item.Bind();

            foreach (var (k, v) in playerItemDict)
                v.transform.SetSiblingIndex(k.player.ProfileStatus.Id);

            playerItemDict.Add(res, item);
        }

        /// <summary>
        /// 進入玩家選擇
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="nextAction">id 為玩家順位，於Profile Status內</param>
        public void EnterPlayerSelect(
            Predicate<IPlayerStandalone> condition = null,
            Action<int> nextAction = null)
        {
            condition = condition ?? (_ => true);
            List<SmartEventTrigger> triggers = new(playerItemDict.Count);
            CancellationTokenSource cts = new();
            Action<int> action = _ =>
            {
                nextAction?.Invoke(_);
                cts.Cancel();
                triggers.ForEach(t => t.ForceDestroy());
            };
            foreach (var (standalone, ui) in playerItemDict)
            {
                if (condition(standalone))
                {
                    var trigger = SmartEventTrigger.Bind(ui.gameObject);
                    trigger.Add(
                        new(EventTriggerType.PointerClick,
                            _ => action(standalone.player.ProfileStatus.Id)),
                        true);
                    triggers.Add(trigger);
                    ui.GetComponent<Image>().Blink(Color.yellow * .5f, blinkTime, cts.Token, true).Forget();
                }
            }
        }
    }
}
