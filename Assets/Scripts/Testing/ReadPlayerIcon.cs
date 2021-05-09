using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TBL.Testing
{
    public class ReadPlayerIcon : MonoBehaviour
    {
        public RawImage image;
        void Start()
        {
            Texture2D texture = FileManager.LoadImage("/Temp/", "temp", "png");
            image.texture = texture;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
