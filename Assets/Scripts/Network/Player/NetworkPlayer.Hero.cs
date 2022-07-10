using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TBL.Card;
using TBL.Action;
using System;

namespace TBL
{
    public partial class NetworkPlayer : NetworkBehaviour
    {
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

