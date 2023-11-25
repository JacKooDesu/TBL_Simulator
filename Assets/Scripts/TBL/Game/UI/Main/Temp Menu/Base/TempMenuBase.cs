using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TBL.Game.UI.Main
{
    using Core.UI;
    public abstract class TempMenuBase<TData, TSetup, TResponse> : Window
    where TSetup : TempMenuBase<TData, TSetup, TResponse>.SetupDataBase
    {
        public record SetupDataBase(TData[] datas);
        protected TSetup Data { get; set; }
        [SerializeField] protected OptionItemBase<TData> itemPrefab;
        [SerializeField] protected int poolSize = 10;
        [SerializeField] protected Transform itemParent;
        Queue<OptionItemBase<TData>> ItemPool { get; set; } = new();
        Queue<OptionItemBase<TData>> ActivePool { get; set; } = new();
        public UnityEvent<OptionItemBase<TData>> OnSelectEvent { get; private set; } = new();
        public UnityEvent<TResponse> OnConfirm { get; private set; } = new();
        private void Awake() => Init();
        public void Init()
        {
            ItemPool = ItemPool ?? new();
            ActivePool = ActivePool ?? new();
            OnSelectEvent = OnSelectEvent ?? new();
            OnConfirm = OnConfirm ?? new();

            for (int i = 0; i < poolSize; ++i)
                ItemPool.Enqueue(InitItem());
                
            OnConfirm.AddListener(_ => Close());
            Close();
        }
        protected virtual OptionItemBase<TData> InitItem()
        {
            var result = Instantiate(itemPrefab, itemParent);
            result.OnSelect.AddListener(_ => OnSelectEvent.Invoke(result));
            return result;
        }
        public virtual TempMenuBase<TData, TSetup, TResponse> Create(TSetup setup)
        {
            Data = setup;
            foreach (var d in setup.datas)
                CreateItem(d);
            gameObject.SetActive(true);
            return this;
        }
        protected virtual OptionItemBase<TData> CreateItem(TData data)
        {
            if (!ItemPool.TryDequeue(out var result))
                result = InitItem();

            ActivePool.Enqueue(result);
            return result.SetData(data);
        }
        public abstract void Cancel();

        public virtual void Close()
        {
            gameObject.SetActive(false);
            while (ActivePool.TryPeek(out var item))
            {
                item.Disabled();
                ItemPool.Enqueue(item);
                ActivePool.Dequeue();
            }
        }
    }
}
