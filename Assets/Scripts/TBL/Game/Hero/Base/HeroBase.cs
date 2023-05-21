using UnityEngine;
using TBL.Card;
using System.Collections;


namespace TBL.Game.Hero
{
    using Judgement;
    using GameActionData;
    public abstract class HeroBase : MonoBehaviour
    {
        [SerializeField, Header("角色名")]
        protected string heroName = "None";
        public string HeroName { get => heroName; }
        [SerializeField]
        protected Sprite avatar;
        public Sprite Avatar
        {
            get => avatar;
        }

        [SerializeField, Header("角色屬性")]
        protected HeroType heroType;
        public HeroType HeroType
        {
            get => heroType;
        }

        public bool isHiding = false;
        [SerializeField]
        protected HeroGender gender;
        public HeroGender Gender
        {
            get => gender;
        }

        [SerializeField, Header("使用者")]
        protected NetworkPlayer playerStatus;
        public NetworkPlayer PlayerStatus
        {
            set { playerStatus = value; }
            get => playerStatus;
        }

        public HeroSkill[] skills;
        public HeroMission mission;

        protected NetworkRoomManager manager;
        protected NetworkJudgement judgement;
        protected NetCanvas.GameScene netCanvas;

        public void Init(NetworkPlayer player)
        {
            PlayerStatus = player;
            if (heroType == HeroType.Hidden)
                isHiding = true;

            skills = new HeroSkill[0];
            BindSkill();
            BindSpecialMission();

            manager = ((NetworkRoomManager.singleton) as NetworkRoomManager);
            judgement = manager.Judgement;
            netCanvas = FindObjectOfType<NetCanvas.GameScene>();
        }

        protected abstract void BindSkill();

        protected abstract void BindSpecialMission();

        public virtual void OnGetCard(CardObject card)
        {

        }

        // only run on server
        public void CheckSkill()
        {
            for (int i = 0; i < skills.Length; ++i)
            {
                var s = skills[i];

                if (s.limited)
                    continue;

                if (s.roundLimited)
                    continue;

                if (s.checker == null)
                    continue;

                if (s.checker())
                {
                    if (s.autoActivate)
                        judgement.UseSkill(new SkillActionData(user: playerStatus.playerIndex, skill: i));
                    else
                        playerStatus.netHeroSkillCanActivate[i] = true;
                }
                else
                {
                    playerStatus.netHeroSkillCanActivate[i] = false;
                }
            }
        }
    }
}

