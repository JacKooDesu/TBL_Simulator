using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace TBL.Utils
{
    public static class GraphicExtend
    {
        // TODO: Need be implement
        /// <summary>
        /// NOT IMPLEMENT
        /// </summary>
        public static UniTask ColorTween<T>(this T graphic, Color target)
        where T : Graphic
        {
            throw new NotImplementedException();
        }

        public static UniTask Blink<T>(this T graphic, Color color, float time, CancellationToken ct, bool loop = false)
        where T : Graphic
        {
            DOTween.Kill(graphic, true);
            var origin = graphic.color;
            var tween = DOTween.To(
                () => graphic.color,
                (x) => graphic.color = x,
                color,
                time
            );
            if (loop)
                tween.SetLoops(-1, LoopType.Yoyo);

            tween.OnKill(() => graphic.color = origin);

            return tween.Play().WithCancellation(cancellationToken: ct);
        }
    }
}