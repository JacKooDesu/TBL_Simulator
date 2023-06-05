using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using System.Linq;

namespace TBL.Game.Sys
{
    using Utils;
    [System.Serializable]
    public abstract class GameCollection<T>
    {
        // public GameCollection(int length)
        // {
        //     collection = new List<T>(length);
        // }
        [SerializeField] protected List<T> collection = new List<T>();

        public T this[int index] { get => collection[index]; }
        public void Add(T t) => collection.Add(t);
        public GameCollection<T> AddRange(IEnumerable<T> cards)
        {
            collection.AddRange(cards);
            return this;
        }
        public T MoveTo(int index, GameCollection<T> target)
        {
            var c = collection[index];
            collection.RemoveAt(index);
            target.Add(c);
            return c;
        }

        public List<T> Filter(Predicate<T> predicate) => collection.FindAll(predicate);
        public int Count => collection.Count;

        public GameCollection<T> Shuffle()
        {
            collection.Shuffle();
            return this;
        }
        #region NOT_IMPLEMENTED
        // public int IndexOf(T item)
        // {
        //     throw new NotImplementedException();
        // }

        // public void Insert(int index, T item)
        // {
        //     throw new NotImplementedException();
        // }

        // public void RemoveAt(int index)
        // {
        //     throw new NotImplementedException();
        // }

        // public void Clear()
        // {
        //     throw new NotImplementedException();
        // }

        // public bool Contains(T item)
        // {
        //     throw new NotImplementedException();
        // }

        // public void CopyTo(T[] array, int arrayIndex)
        // {
        //     throw new NotImplementedException();
        // }

        // public bool Remove(T item)
        // {
        //     throw new NotImplementedException();
        // }
        #endregion
    }
}