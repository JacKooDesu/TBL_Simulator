using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace TBL.Game
{
    using Setting;
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
            var set = teamSetting.PlayerSets[count];
            var playerCount = set.PlayerCount;
            var usedHero = new List<int>();
            var list = new List<Player>();

            void SetupTeam(ref PlayerCollection teamCollection, TeamEnum team, int c)
            {
                for (int i = 0; i < c; ++i)
                {
                    var p = new Player();

                    p.TeamStatus.Update(new TeamStatus(team));
                    
                    // TODO: Hero Enum, Hero List, Banned Hero ... etc
                    // TODO: Hero draw shoud placed after game init
                    // TODO: Which Player can choose from 2 hero
                    p.HeroStatus.Update(new HeroStatus());

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

        public async UniTask<PlayerList> BindPlayer()
        {
            await UniTask.CompletedTask;
            return this;
        }
    }
}