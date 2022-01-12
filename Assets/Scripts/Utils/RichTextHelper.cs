using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RichTextHelper
{
    public enum Style
    {
        Color,
        Bold,
        Italics
    }

    public class SettingBase
    {

        public Style style;

        public SettingBase(Style style)
        {
            this.style = style;
        }
    }

    public class Setting<T> : SettingBase
    {
        public T suffix;

        public Setting(Style style, T suffix) : base(style)
        {
            this.style = style;
            this.suffix = suffix;
        }
    }

    public static string TextWithStyles(string s, params SettingBase[] settings)
    {
        string final = s;
        List<SettingBase> sList = new List<SettingBase>(settings);

        if (sList.Find((s) => s.style == Style.Bold) != null)
            final = TextWithBold(final);

        if (sList.Find((s) => s.style == Style.Italics) != null)
            final = TextWithItalics(final);

        if (sList.Find((s) => s.style == Style.Color) != null)
        {
            var setting = sList.Find((s) => s.style == Style.Color) as Setting<Color>;
            final = TextWithColor(final, setting.suffix);
        }

        return final;
    }

    public static string TextWithItalics(string s)
    {
        return $"<i>{s}</i>";
    }

    public static string TextWithBold(string s)
    {
        return $"<b>{s}</b>";
    }

    public static string TextWithColor(string s, Color c)
    {
        string final = "";
        final += $"<color=#{ColorUtility.ToHtmlStringRGBA(c)}>";
        final += s;
        final += "</color>";
        return final;
    }
}
