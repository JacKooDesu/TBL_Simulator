using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace TBL.Game.UI
{
    using Utils;
    public class BannerUI : MonoBehaviour
    {
        [SerializeField] TMP_Text title;
        [SerializeField] Image banner;
        const float TIME_PER_TEXT = .8f;
        CancellationTokenSource cts;
        void Awake()
        {
            gameObject.SetActive(false);
        }

        void ResetCts()
        {
            cts?.Cancel();
            cts = new();
            cts.RegisterRaiseCancelOnDestroy(gameObject);
        }

        public void Show(string text, Color color, float displayTime = 0) =>
            ShowTask(text, color, displayTime).Forget();

        public async UniTask ShowTask(string text, Color color, float displayTime = 0)
        {
            ResetCts();

            title.text = text;
            banner.color = color;
            if (displayTime == 0)
                displayTime = text.Length * TIME_PER_TEXT;

            gameObject.SetActive(true);
            await UniTask.Delay(
                (int)(displayTime * 1000),
                cancellationToken: cts.Token);
            gameObject.SetActive(false);
        }
    }
}
