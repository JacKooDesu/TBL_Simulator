using System;
using System.Collections.Generic;
using UnityEngine;

namespace TBL.Utils
{
    using Object = UnityEngine.Object;
    public static class TransformExtend
    {
        public static void DestroyChildren(this Transform transform)
        {
            for (int i = transform.childCount - 1; i >= 0; --i)
                Object.Destroy(transform.GetChild(i).gameObject);
        }
    }
}