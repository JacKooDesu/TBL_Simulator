using UnityEngine;
using TBL.Card;

namespace TBL
{
    public abstract class Hero : MonoBehaviour
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

        public TBL.HeroSkill[] skills;
        public System.Func<bool> specialMission;

        public void Init(NetworkPlayer player)
        {
            PlayerStatus = player;
            if (heroType == HeroType.Hidden)
                isHiding = true;

            skills = new HeroSkill[0];
            BindSkill();
        }

        protected abstract void BindSkill();

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

                if (s.checker == null)
                    continue;

                if (s.checker.Invoke())
                    playerStatus.netHeroSkillCanActivate[i] = true;

            }
        }
    }
}

