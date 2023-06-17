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

        public T MoveTo(T item, GameCollection<T> target)
        {
            var index = collection.IndexOf(item);
            if (index == -1)
            {
                Debug.LogWarning("Item is not in target collection!");
                return item;
            }
            return MoveTo(index, target);
        }

        public T MoveTo(Predicate<T> predicate, GameCollection<T> target) =>
            MoveTo(First(predicate), target);

        public List<T> Filter(Predicate<T> predicate) => collection.FindAll(predicate);
        public T First(Predicate<T> predicate) => Filter(predicate).First();
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