using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine;

namespace TBL.Game
{
    [Serializable]
    public class CardCollection : Sys.GameCollection<CardData>
    {
        public List<CardData> Filter(CardEnum.Property property) => base.Filter((CardData c) => c.Property.Contains(property));
    }
}