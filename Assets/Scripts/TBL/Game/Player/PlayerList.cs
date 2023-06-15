using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace TBL.Game
{
    using Utils;
    using Setting;
    using Game.Sys;
    using Game.Networking;
    [System.Serializable]
    public class PlayerList
    //  : Sys.IResource<int, TeamSetting, HeroSetting, PlayerList>
    {
        [SerializeField] List<Player> players;
        public ReadOnlyCollection<Player> Players => players.AsReadOnly();
        public Player this[int index] { get => Players[index]; }

        public PlayerCollection Blue = new PlayerCollection();
        public PlayerCollection Red = new PlayerCollection();
        public PlayerCollection Green = new PlayerCollection();

        public PlayerList Init(params IPlayerStandalone[] standalones)
        {
            var list = new List<Player>(standalones.Length);
            foreach (var s in standalones)
                list.Add(new(s, true));

            players = list;
            return this;
        }

        public void SetupTeam(TeamSetting teamSetting, HeroSetting heroSetting, bool withoutShuffle = false)
        {
            // FIXME: 暫時將玩家名稱設定
            for (int i = 0; i < players.Count; ++i)
                players[i].ProfileStatus.Update(new($"P{i}", i));

            List<Player> pList = withoutShuffle ? new(players) : players;
            var pQueue = new Queue<Player>(pList.Shuffle());

            var set = teamSetting.PlayerSets.Find(s => s.PlayerCount == players.Count);
            var playerCount = set.PlayerCount;
            var usedHero = new List<int>();
            var iter = 0;
            void SetupTeam(ref PlayerCollection teamCollection, TeamEnum team, int c)
            {
                for (int i = 0; i < c; ++i)
                {
                    var p = pQueue.Dequeue();
                    var id = iter;

                    // p.ProfileStatus.Update(new($"P{id}", id));
                    p.UpdateStatus(record: new(PlayerStatusType.Team, new TeamStatus(team)));
                    // p.TeamStatus.Update(team);
                    // p.UpdateStatus(new ProfileStatus());
                    // p.UpdateStatus(new TeamStatus(team, PlayerStatusType.Team));
                    // p.UpdateStatus(PlayerStatusType.TeamStatus, new ValueTypeStatus<TeamEnum>(team, PlayerStatusType.TeamStatus));

                    // TODO: Hero Enum, Hero List, Banned Hero ... etc
                    // TODO: Hero draw shoud placed after game init
                    // TODO: Which Player can choose from 2 hero
                    // p.HeroStatus.Update(new HeroStatus());

                    teamCollection.Add(p);
                    iter++;
                }
            }

            SetupTeam(ref Blue, TeamEnum.Blue, set.Blue);
            SetupTeam(ref Red, TeamEnum.Red, set.Red);
            SetupTeam(ref Green, TeamEnum.Green, set.Green);
        }

        public Player QueryById(int id) => players.Find(p => p.ProfileStatus.Id == id);

        public void Foreach(Action<Player> action)
        {
            foreach (var p in players)
                action(p);
        }
    }
}