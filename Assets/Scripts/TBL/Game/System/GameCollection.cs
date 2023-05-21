using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

namespace TBL.Game.Sys
{
    [System.Serializable]
    public abstract class GameCollection<T>
    {
        // public GameCollection(int length)
        // {
        //     collection = new List<T>(length);
        // }
        [SerializeField] List<T> collection = new List<T>();

        public T this[int index] { get => collection[index]; }
        public void Add(T t) => collection.Add(t);
        public void AddRange(IEnumerable<T> cards) => collection.AddRange(cards);
        public void MoveTo(int index, GameCollection<T> target)
        {
            var c = collection[index];
            collection.RemoveAt(index);
            target.Add(c);
        }

        public List<T> Filter(Predicate<T> predicate) => collection.FindAll(predicate);
        public int Count => collection.Count;


        public int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

    }
}