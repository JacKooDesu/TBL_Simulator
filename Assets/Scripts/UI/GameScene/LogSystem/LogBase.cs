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
                return message;
            }
        }

        bool isServer;
        public bool IsServer
        {
            get => isServer;
        }

        bool isPrivate;
        public bool IsPrivate
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
