using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace TBL.Game
{
    using Utils;
    using Setting;
    using NetworkPlayer = Networking.NetworkPlayer;
    [System.Serializable]
    public class PlayerList : Sys.IResource<int, TeamSetting, HeroSetting, PlayerList>
    {
        [SerializeField] Player[] players;
        public ReadOnlyCollection<Player> Players;
        public Player this[int index] { get => Players[index]; }

        public PlayerCollection Blue = new PlayerCollection();
        public PlayerCollection Red = new PlayerCollection();
        public PlayerCollection Green = new PlayerCollection();

        public PlayerList Init(int count, TeamSetting teamSetting, HeroSetting heroSetting)
        {
            var set = teamSetting.PlayerSets.Find(s => s.PlayerCount == count);
            var playerCount = set.PlayerCount;
            var usedHero = new List<int>();
            var list = new List<Player>();

            void SetupTeam(ref PlayerCollection teamCollection, TeamEnum team, int c)
            {
                for (int i = 0; i < c; ++i)
                {
                    var p = new Player();
                    p.UpdateStatus(new TeamStatus(team, PlayerStatusType.Team));
                    // p.UpdateStatus(PlayerStatusType.TeamStatus, new ValueTypeStatus<TeamEnum>(team, PlayerStatusType.TeamStatus));

                    // TODO: Hero Enum, Hero List, Banned Hero ... etc
                    // TODO: Hero draw shoud placed after game init
                    // TODO: Which Player can choose from 2 hero
                    // p.HeroStatus.Update(new HeroStatus());

                    list.Add(p);
                    teamCollection.Add(p);
                }
            }

            SetupTeam(ref Blue, TeamEnum.Blue, set.Blue);
            SetupTeam(ref Red, TeamEnum.Red, set.Red);
            SetupTeam(ref Green, TeamEnum.Green, set.Green);

            Players = list.AsReadOnly();
            players = list.ToArray();
            return this;
        }

        public async UniTaskVoid BindPlayer(params NetworkPlayer[] players)
        {
            if (players.Length != Players.Count)
                Debug.LogError("Network Player count not equals to Player count!");

            var plist = new List<NetworkPlayer>(players).Shuffle();
            foreach(var p in plist){
                p.RpcSend("");
            }
            
            await UniTask.CompletedTask;
        }
    }
}