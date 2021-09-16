using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JacDev.Utils.TooltipSystem
{
    [ExecuteInEditMode()]
    public class Tooltip : MonoBehaviour
    {
        public Text headerText;
        public Text contentText;
        public LayoutElement layoutElement;

        public int characterWrapLimit;

        public void SetText(string content, string header = "")
        {
            if (string.IsNullOrEmpty(header))
            {
                headerText.gameObject.SetActive(false);
            }
            else
            {
                headerText.gameObject.SetActive(true);
                headerText.text = header;
            }

            contentText.text = content;

            int headerLength = headerText.text.Length;
            int contentLength = contentText.text.Length;

            layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit) ? true : false;
        }

        private void Update()
        {
            if (Application.isEditor)
            {
                int headerLength = headerText.text.Length;
                int contentLength = contentText.text.Length;

                layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit) ? true : false;
            }

            Vector2 position = Input.mousePosition;
            Vector2 pivot = new Vector2(position.x / Screen.width, position.y / Screen.height);

            (transform as RectTransform).pivot = pivot;
            transform.position = position;
        }
    }
}