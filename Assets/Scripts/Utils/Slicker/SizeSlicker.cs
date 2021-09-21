using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

namespace JacDev.Utils.UISlicker
{
    [AddComponentMenu("JacDev/UI Slicker/Size Slicker")]
    public class SizeSlicker : SlickerBase
    {
        [System.Serializable]
        public class SizeSetting : Setting<Vector2> { }
        public List<SizeSetting> settings = new List<SizeSetting>();
        public SizeSetting origin = new SizeSetting();

        private void OnEnable()
        {
            origin.Init("origin", rect.sizeDelta, origin.time);
        }

        public override void Slick(string name)
        {
            base.Slick(name);
            foreach (SizeSetting v in settings)
            {
                if (v.name == name)
                {
                    BindTween(DOTween.To(() => rect.sizeDelta, x => rect.sizeDelta = x, v.set, v.time).
                        OnStart(() => v.onBegin.Invoke()).
                        OnComplete(() => v.onComplete.Invoke()));
                }
            }
        }

        public override void SlickBack()
        {
            base.SlickBack();
            BindTween(DOTween.To(() => rect.sizeDelta, x => rect.sizeDelta = x, origin.set, origin.time).
                OnStart(() => origin.onBegin.Invoke()).
                OnComplete(() => origin.onComplete.Invoke()));
        }
    }
}