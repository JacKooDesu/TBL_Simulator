using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace JacDev.Utils.UISlicker
{
    [RequireComponent(typeof(CanvasGroup))]
    public class AlphaSlicker : SlickerBase
    {
        [System.Serializable]
        public class AlphaSetting : Setting<float> { }
        public List<AlphaSetting> settings = new List<AlphaSetting>();
        public AlphaSetting origin = new AlphaSetting();

        private void OnEnable()
        {
            origin.Init("origin", GetComponent<CanvasGroup>().alpha, origin.time);
        }

        public override void Slick(string name)
        {
            base.Slick(name);
            foreach (Setting<float> a in settings)
            {
                if (a.name == name)
                {
                    BindTween(DOTween.To(() => GetComponent<CanvasGroup>().alpha, x => GetComponent<CanvasGroup>().alpha = x, a.set, a.time).
                        OnStart(() => a.onBegin.Invoke()).
                        OnComplete(() => a.onComplete.Invoke()));
                }
            }
        }

        public override void SlickBack()
        {
            base.SlickBack();
            BindTween(DOTween.To(() => GetComponent<CanvasGroup>().alpha, x => GetComponent<CanvasGroup>().alpha = x, origin.set, origin.time).
                OnStart(() => origin.onBegin.Invoke()).
                OnComplete(() => origin.onComplete.Invoke()));
        }
    }
}
