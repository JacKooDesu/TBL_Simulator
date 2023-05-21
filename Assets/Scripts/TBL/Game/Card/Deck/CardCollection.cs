using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine;

namespace TBL.Game
{
    [Serializable]
    public class CardCollection
    {
        [SerializeField] List<CardData> collection = new List<CardData>();

        public CardData this[int index] { get => collection[index]; }

        public void Add(CardData c) => collection.Add(c);
        public void AddRange(IEnumerable<CardData> cards) => collection.AddRange(cards);
        public void MoveTo(int index, CardCollection target)
        {
            var c = collection[index];
            collection.RemoveAt(index);
            target.Add(c);
        }

        public List<CardData> Filter(Predicate<CardData> predicate) => collection.FindAll(predicate);
        public List<CardData> Filter(CardEnum.Property property) => Filter((CardData c) => c.Property.Contains(property));
    }
}