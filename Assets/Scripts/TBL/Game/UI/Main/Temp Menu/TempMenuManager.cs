using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TBL.Game.UI.Main
{
    public class TempMenuManager : MonoBehaviour
    {
        [SerializeField] CardTempMenu cardTempMenu;
        public CardTempMenu CardTempMenu => cardTempMenu;
        [SerializeField] ActionTempMenu actionTempMenu;
        public ActionTempMenu ActionTempMenu => actionTempMenu;
    }
}
