using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace JacDev.Utils.UISlicker
{
    [System.Serializable]
    public class SlickerBase : MonoBehaviour
    {
        protected RectTransform rect
        {
            get
            {
                return this.transform as RectTransform;
            }
        }
        protected Sequence sequence;

        public virtual void Slick(string name)
        {
            DOTween.Kill(gameObject);
        }
        public virtual void SlickBack() { }

        protected void BindTween(Tween tween)
        {
            sequence.Kill();
            sequence.Append(tween);
        }

        // protected void Tween<T>(Setting<T> s, T from)
        // {
        //     iTween.ValueTo(gameObject,
        //         iTween.Hash(
        //         "from", from,
        //         "to", s.set,
        //         "time", s.time,
        //         "easetype", s.easeType,
        //         "onupdate", "TweenCallback"
        //         ));
        // }
    }

    [System.Serializable]
    public class Setting<T>
    {
        public string name;
        [SerializeField] public T set;
        public float time = .2f;
        public Ease easeType = Ease.OutQuad;
        [HideInInspector] public System.Action onBegin = ()=>{};
        [HideInInspector] public System.Action onComplete = ()=>{};

        public void Init(string name, T set, float time)
        {
            this.name = name;
            this.set = set;
            this.time = time;
        }
    }
}

