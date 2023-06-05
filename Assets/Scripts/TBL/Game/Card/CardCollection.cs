using System.Collections.Generic;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using UnityEngine;
using System.Linq;

namespace TBL.Game
{
    [Serializable]
    public class CardCollection : Sys.GameCollection<CardData>
    {
        public List<CardData> Filter(CardEnum.Property property) => base.Filter((CardData c) => c.Property.Contains(property));

        public ReadOnlyCollection<CardData> Blue() => Filter(CardEnum.Property.Blue).AsReadOnly();
        public ReadOnlyCollection<CardData> Red() => Filter(CardEnum.Property.Red).AsReadOnly();
        public ReadOnlyCollection<CardData> Black() => Filter(CardEnum.Property.Black).AsReadOnly();

        public static CardCollection FromList(IEnumerable<int> list)
        {
            var result = new CardCollection();
            result.AddRange(list.Select(x => new CardData(x)));
            return result;
        }

        public List<int> ToIdList() => collection.Select(c => (int)c.Property).ToList();
    }
}