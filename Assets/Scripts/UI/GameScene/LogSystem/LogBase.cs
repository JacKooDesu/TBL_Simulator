using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TBL.UI
{
    public class LogBase
    {
        string message;
        public string Message
        {
            get
            {
                string prefix = $"【{(isPrivate ? "私人" : "公開")}】";
                prefix = RichTextHelper.TextWithStyles(
                    prefix,
                    isPrivate ?
                        new RichTextHelper.Setting<Color>(RichTextHelper.Style.Color, Color.red) :
                        new RichTextHelper.Setting<Color>(RichTextHelper.Style.Color, Color.yellow),
                    new RichTextHelper.SettingBase(RichTextHelper.Style.Bold));
                return $"{prefix} {message}";
            }
        }

        bool isServer;
        bool IsServer
        {
            get => isServer;
        }

        bool isPrivate;
        bool IsPrivate
        {
            get => isPrivate;
        }

        int[] targetPlayers;
        public int[] TargetPlayers
        {
            get => targetPlayers;
        }

        public static List<LogBase> logs = new List<LogBase>();

        public LogBase(string message, bool isServer, bool isPrivate, int[] targetPlayers)
        {
            this.message = message;
            this.isServer = isServer;
            this.isPrivate = isPrivate;
            this.targetPlayers = targetPlayers;
        }
    }
}
