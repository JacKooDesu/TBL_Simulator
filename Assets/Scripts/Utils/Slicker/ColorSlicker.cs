using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace JacDev.Utils.UISlicker
{
    [AddComponentMenu("JacDev/UI Slicker/Color Slicker")]
    public class ColorSlicker : SlickerBase
    {
        [System.Serializable]   // 不知道怎麼解決，無法序列化泛型
        public class ColorSetting : Setting<Color> { }
        public List<ColorSetting> settings = new List<ColorSetting>();
        public ColorSetting origin = new ColorSetting();

        private void OnEnable()
        {
            origin.Init("origin", GetComponent<Graphic>().color, origin.time);
        }

        public override void Slick(string name)
        {
            base.Slick(name);
            foreach (Setting<Color> c in settings)
            {
                if (c.name == name)
                {
                    BindTween(DOTween.To(() => GetComponent<Graphic>().color, x => GetComponent<Graphic>().color = x, c.set, c.time).
                        OnStart(() => c.onBegin.Invoke()).
                        OnComplete(() => c.onComplete.Invoke()));
                    // Tween<Color>(c, GetComponent<Graphic>().color);
                }
            }
        }

        public override void SlickBack()
        {
            base.SlickBack();
            BindTween(DOTween.To(() => GetComponent<Graphic>().color, x => GetComponent<Graphic>().color = x, origin.set, origin.time).
                OnStart(() => origin.onBegin.Invoke()).
                OnComplete(() => origin.onComplete.Invoke()));
            // Tween<Color>(origin, GetComponent<Graphic>().color);
        }
    }

}
