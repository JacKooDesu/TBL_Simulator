using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
namespace TBL.Game
{
    using UnityEngine.Events;
    using Utils;
    [JsonObject, Serializable]
    public class PhaseQuestStatus : IPlayerStatus<PhaseQuestStatus>
    {
        public PhaseQuestStatus() { }

        public enum QuestType
        {
            DrawCard,
            SelectCard,
            PassCard,
            AskRecieve,
            ReceiveCard
        }
        [JsonProperty]
        List<QuestType> quests = new();
        [JsonIgnore]
        public ReadOnlyCollection<QuestType> Quest => quests.AsReadOnly();

        public UnityEvent<PhaseQuestStatus> OnChanged { get; } = new();

        public PhaseQuestStatus Current() => this;
        public PlayerStatusType Type() => PlayerStatusType.Quest;
        public void Update(PhaseQuestStatus value)
        {
            this.quests = value.quests;
            OnChanged.Invoke(this);
        }

        public void Update<S>(S status)
        where S : IPlayerStatus<PhaseQuestStatus>
            => Update(status);

        public void Update(IPlayerStatus value) =>
            Update(value as PhaseQuestStatus);

        public void AddQuest(QuestType r)
        {
            quests.Add(r);
            OnChanged.Invoke(this);
        }

        public bool FinishQuest(QuestType q)
        {
            if (quests.Remove(q))
            {
                OnChanged.Invoke(this);
                return true;
            }
            return false;
        }
    }
}