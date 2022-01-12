using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RichTextHelper
{
    public static string TextWithColor(string s, Color c)
    {
        string final = "";
        final += $"<color=#{ColorUtility.ToHtmlStringRGBA(c)}>";
        final += s;
        final += "</color>";
        return final;
    }
}
